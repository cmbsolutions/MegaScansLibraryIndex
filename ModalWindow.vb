Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading


Public Class ModalWindow
    Private rootFolder As String = ""
    Private resolution As Integer = 0
    Private cts As CancellationTokenSource

    Private fbxLoader As New Assimp.AssimpContext()


    Private suffixOrder As New List(Of KeyValuePair(Of Integer, String)) From {
        New KeyValuePair(Of Integer, String)(10, "_AO"), ' R, priority over Cavity
        New KeyValuePair(Of Integer, String)(11, "_Cavity"), ' R, if no AO
        New KeyValuePair(Of Integer, String)(20, "_Roughness"), ' G
        New KeyValuePair(Of Integer, String)(30, "_Specular"), ' B, if no gloss
        New KeyValuePair(Of Integer, String)(31, "_Gloss"), ' B, priority over Specular
        New KeyValuePair(Of Integer, String)(40, "_Bump"), ' A, if no Displacement
        New KeyValuePair(Of Integer, String)(41, "_Displacement") ' A, priority over Bump
    }

    Public Sub ResizeModal()
        If Me.ClientSize.Width > 1500 And Me.ClientSize.Height > 1000 Then
            pbPreview.Size = New Size(1024, 1024)
            modalContent.Size = New Size(1319, 1024)
        Else
            pbPreview.Size = New Size(512, 512)
            modalContent.Size = New Size(819, 512)
        End If
    End Sub

    Public Sub CenterModalContent()
        ' Center the PictureBox within the modalPanel
        modalContent.Location = New Point((Width - modalContent.Width) \ 2, (Height - modalContent.Height) \ 2)
    End Sub

    Public Sub ZoomAsset(sender As Object, e As EventArgs)
        Dim ctrl As PictureBox = TryCast(sender, PictureBox)

        If ctrl IsNot Nothing Then
            Dim parentCtrl As AssetInfo = TryCast(ctrl.Parent, AssetInfo)
            If parentCtrl IsNot Nothing Then
                BuildInfoSheet(parentCtrl.Data)
            End If

            Using imgStream As New FileStream(parentCtrl.PreviewImage, FileMode.Open, FileAccess.Read)
                pbPreview.Image = Image.FromStream(imgStream)
            End Using

            ' Show the modal panel
            ' Take a screenshot of the current form
            ' Take a screenshot of only the client area (without title bar and borders)
            Dim screenshot As New Bitmap(Me.ClientSize.Width, Me.ClientSize.Height)
            Using g As Graphics = Graphics.FromImage(screenshot)
                g.CopyFromScreen(Me.PointToScreen(Point.Empty), Point.Empty, Me.ClientSize)
            End Using

            ' Apply a fading effect to the screenshot
            Dim fadedImage As Bitmap = ApplyDarkOverlay(screenshot, 170) ' Adjust the alpha (128 for 50% transparency)
            'backdropImage.Image = fadedImage
            BackgroundImage = fadedImage

            ' Show the modal panel
            BringToFront()
            Visible = True
        End If
    End Sub

    Private Sub Load3DPreview()

    End Sub

    Private Function ApplyFadeEffect(image As Bitmap, alpha As Integer) As Bitmap
        ' Creates a new faded bitmap with transparency
        Dim fadedImage As New Bitmap(image.Width, image.Height)
        Using g As Graphics = Graphics.FromImage(fadedImage)
            Dim colorMatrix As New ColorMatrix With {.Matrix33 = alpha / 255.0F}
            Using attributes As New ImageAttributes()
                attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
                g.DrawImage(image, New Rectangle(0, 0, fadedImage.Width, fadedImage.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes)
            End Using
        End Using
        Return fadedImage
    End Function

    Private Function ApplyDarkOverlay(image As Bitmap, alpha As Integer) As Bitmap
        ' Creates a new bitmap with a semi-transparent black overlay
        Dim darkenedImage As New Bitmap(image.Width, image.Height)
        Using g As Graphics = Graphics.FromImage(darkenedImage)
            g.DrawImage(image, 0, 0) ' Draw the original image

            ' Create a semi-transparent black overlay
            Dim overlayBrush As New SolidBrush(Color.FromArgb(alpha, Color.Black))
            g.FillRectangle(overlayBrush, New Rectangle(0, 0, darkenedImage.Width, darkenedImage.Height))
        End Using
        Return darkenedImage
    End Function

    Public Sub BuildInfoSheet(data As DataGridViewRow)

        Dim tpl As String = My.Resources.infotemplate

        For Each c As DataGridViewCell In data.Cells
            tpl = tpl.Replace($"[{c.OwningColumn.DataPropertyName}]", c.Value.ToString)
            If c.OwningColumn.DataPropertyName = "location" Then
                rootFolder = c.Value.ToString
            End If

            If c.OwningColumn.DataPropertyName = "resolution" Then
                Select Case c.Value.ToString
                    Case "8K"
                        resolution = 8192
                    Case "4K"
                        resolution = 4096
                    Case "2K"
                        resolution = 2048
                    Case "1K"
                        resolution = 1024
                    Case Else
                        resolution = 0
                End Select
            End If
        Next
        rtbInfo.Clear()
        rtbInfo.Text = tpl
        ParseAndFormatRichText(rtbInfo)
    End Sub

    Public Sub ParseAndFormatRichText(richTextBox As RichTextBox)
        ' Retrieve the raw text from the RichTextBox and clear it for formatted text
        Dim rawText As String = richTextBox.Text
        richTextBox.Clear()

        ' Regex patterns for bold and color tags
        Dim boldPattern As String = "<b>(.*?)</b>"
        ' Keep track of current position in the RichTextBox
        Dim lastIndex As Integer = 0

        ' Process bold tags
        Dim boldMatches As MatchCollection = Regex.Matches(rawText, boldPattern)
        For Each match As Match In boldMatches
            ' Add text before the match
            If match.Index > lastIndex Then
                richTextBox.AppendText(rawText.Substring(lastIndex, match.Index - lastIndex))
            End If
            ' Add the bold text
            richTextBox.SelectionFont = New Font(richTextBox.Font, FontStyle.Bold)
            richTextBox.AppendText(match.Groups(1).Value)
            lastIndex = match.Index + match.Length
            richTextBox.SelectionFont = New Font(richTextBox.Font, FontStyle.Regular)
        Next
        ' Append remaining text after last bold tag
        richTextBox.AppendText(rawText.Substring(lastIndex))
    End Sub

    Private Sub rtbInfo_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbInfo.LinkClicked
        Process.Start("explorer.exe", e.LinkText)
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        cts.Cancel()
    End Sub

    Private Async Function PackFiles(suffixes As List(Of String), newFilename As String) As Task
        cts = New CancellationTokenSource()

        pBar.Minimum = 0
        pBar.Maximum = resolution
        pBar.Value = 0
        pBar.Visible = True
        btnCancel.Visible = True
        lblStatus.Visible = True
        lblStatus.Text = "Packing in progress..."

        Dim prog As New Progress(Of Integer)(
            Sub(value)
                If value = -1 Then
                    pBar.Value = pBar.Maximum
                    btnCancel.Visible = False
                    pBar.Visible = False
                    lblStatus.Visible = False
                    MessageBox.Show("Packing aborted, no AO file found!", "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                If value = pBar.Maximum + 1 Then
                    lblStatus.Text = "Saving packed file..."
                End If
                If value = pBar.Maximum + 2 Then
                    pBar.Value = pBar.Maximum
                    btnCancel.Visible = False
                    pBar.Visible = False
                    lblStatus.Visible = False
                    MessageBox.Show("Packing complete!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                pBar.Value = Math.Max(pBar.Minimum, Math.Min(value, pBar.Maximum))
            End Sub)

        Try
            Await Task.Run(Sub()
                               MaskPacker.Pack(rootFolder, suffixes, newFilename, prog, cts.Token)
                           End Sub)
        Catch ocex As OperationCanceledException
            Trace.WriteLine("Cancelled")
        Catch ex As Exception
            Trace.WriteLine(ex)
        End Try
    End Function

    Private Sub CheckedChanged(sender As Object, e As EventArgs) Handles tsmAO.CheckedChanged, tsmBump.CheckedChanged, tsmCavity.CheckedChanged, tsmDisplacement.CheckedChanged, tsmGloss.CheckedChanged, tsmRoughness.CheckedChanged, tsmSpecular.CheckedChanged
        Dim checkedCount = {tsmAO, tsmBump, tsmCavity, tsmDisplacement, tsmGloss, tsmRoughness, tsmSpecular}.Count(Function(tsm) tsm.Checked)

        If checkedCount > 4 Then
            MessageBox.Show("You can only select up to 4 channels for packing.", "Too Many Channels", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Dim changedTsm = TryCast(sender, ToolStripMenuItem)
            If changedTsm IsNot Nothing Then
                changedTsm.Checked = False
            End If
        End If
    End Sub

    Private Sub tsmCreatePackedFile_Click(sender As Object, e As EventArgs) Handles tsmCreatePackedFile.Click
        Dim suffixes As New List(Of String)
        If tsmAO.Checked Then suffixes.Add("_AO")
        If tsmRoughness.Checked Then suffixes.Add("_Roughness")
        If tsmSpecular.Checked Then suffixes.Add("_Specular")
        If tsmGloss.Checked Then suffixes.Add("_Gloss")
        If tsmBump.Checked Then suffixes.Add("_Bump")
        If tsmCavity.Checked Then suffixes.Add("_Cavity")
        If tsmDisplacement.Checked Then suffixes.Add("_Displacement")


        'Await PackFiles(suffixes)
    End Sub

    Private Async Sub tsmUberPacker_Click(sender As Object, e As EventArgs) Handles tsmUberPacker.Click
        Try
            Dim ofd As New OpenFileDialog With {
                .Title = "Select maximum of 4 files to pack",
                .Filter = "Image Files|*.png;*.tga;*.jpg;*.jpeg;*.exr",
                .InitialDirectory = rootFolder,
                .Multiselect = True
            }

            Dim sfd As New SaveFileDialog With {
                .Title = "Save packed file as",
                .Filter = "PNG Image|*.png",
                .InitialDirectory = rootFolder,
                .FileName = "packed.png"
            }

            If ofd.ShowDialog() = DialogResult.OK Then
                Dim selectedFiles = ofd.FileNames.Take(4).ToList()
                Dim suffixes As New List(Of String)
                Dim projectedFilename As String = $"{IO.Path.GetFileName(rootFolder)}_packed_"

                For Each file In selectedFiles
                    Dim fileName = Path.GetFileNameWithoutExtension(file)
                    Dim suffix = fileName.Substring(fileName.LastIndexOf("_"c))
                    suffixes.Add(suffix)
                Next
                Dim weightDict = suffixOrder.ToDictionary(Function(kvp) kvp.Value, Function(kvp) kvp.Key)

                suffixes = suffixes.OrderBy(Function(s) If(weightDict.ContainsKey(s), weightDict(s), Integer.MaxValue)).ToList
                For Each suf In suffixes
                    projectedFilename &= suf.Substring(0, 2)
                Next
                sfd.FileName = projectedFilename & ".png"

                If sfd.ShowDialog() = DialogResult.OK Then

                    Await PackFiles(suffixes, sfd.FileName)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
