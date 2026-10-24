Imports System.Drawing.Imaging
Imports MegascansLibraryIndex.Models

Public Class Form1
    Private dbc As DataContext
    Const BASE = "H:\megascanslibrary\CustomDownloads"
    Private WithEvents modalPanel As New ModalWindow
    Private backdropImage As New PictureBox
    Private tc As CustomTextTraceListener

    Private cp, tp As Integer
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        dbc = New DataContext
        ' Configure the modal panel (similar to a bootstrap modal backdrop)
        modalPanel.Size = Me.ClientSize
        'modalPanel.BackColor = Color.Transparent ' Semi-transparent black
        modalPanel.Visible = False
        modalPanel.SendToBack()
        modalPanel.CenterModalContent()
        'modalPanel.Dock = DockStyle.Fill
        Me.Controls.Add(modalPanel)

        ' Setup backdrop image as a faded background
        'backdropImage.Dock = DockStyle.Fill
        'backdropImage.SizeMode = PictureBoxSizeMode.StretchImage
        'modalPanel.Controls.Add(backdropImage)

        ' Configure the picture box

        tc = New CustomTextTraceListener(rtbLog)
        cboSearchByIdOption.SelectedIndex = 2
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If MessageBox.Show("This will rebuild the entire file index and may take a while. Are you sure?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.No Then
            Return
        End If
        Cursor = Cursors.WaitCursor

        dbc.StartIndex()
        MessageBox.Show("Done")
        Cursor = Cursors.Default
    End Sub

    Private Sub bTagReload_Click(sender As Object, e As EventArgs) Handles bTagReload.Click
        Dim tags = dbc.GetTags
        Dim x = New AutoCompleteStringCollection()

        x.AddRange(tags.Select(Function(c) c.Value).ToArray)
        cbTags.Items.AddRange(tags.Select(Function(c) c.Value).ToArray)
        cbTags.AutoCompleteCustomSource = x
        cbTags.Refresh()

        Dim packs = dbc.GetPacks
        x.Clear()
        x.AddRange(packs.Select(Function(c) c.Value).ToArray)
        cbPacks.Items.AddRange(packs.Select(Function(c) c.Value).ToArray)
        cbPacks.AutoCompleteCustomSource = x
        cbPacks.Refresh()
    End Sub

    Private Sub bAddTag_Click(sender As Object, e As EventArgs) Handles bAddTag.Click
        clbTags.Items.Add(cbTags.Text, True)
    End Sub

    Private Sub bSearch_Click(sender As Object, e As EventArgs) Handles bSearch.Click
        cp = 0
        LoadData()
    End Sub

    Private Sub DisableButtons()
        bFirst.Enabled = False
        bNext.Enabled = False
        bPrev.Enabled = False
        bLast.Enabled = False
        cbPages.Enabled = False
    End Sub

    Private Sub EnableButtons()
        bFirst.Enabled = True
        bNext.Enabled = True
        bPrev.Enabled = True
        bLast.Enabled = True
        cbPages.Enabled = True
    End Sub

    Private Sub LoadData()
        DisableButtons()
        Dim filters As New List(Of DataFilter)

        If sid.Text.Length > 0 Then
            Dim dtf As DataFilter = Nothing
            Select Case cboSearchByIdOption.SelectedIndex
                Case 0 'Startswith
                    dtf = New DataFilter With {
                        .filterfieldname = "m.id",
                        .filterComparison = "BEGINSWITH"
                    }
                Case 1 ' endswith
                    dtf = New DataFilter With {
                        .filterfieldname = "m.id",
                        .filterComparison = "ENDSWITH"
                    }
                Case 2 'contains
                    dtf = New DataFilter With {
                        .filterfieldname = "m.id",
                        .filterComparison = "CONTAINS"
                    }
                Case 3 ' soundex
                    dtf = New DataFilter With {
                        .filterfieldname = "m.soundex_id",
                        .filterComparison = "SOUNDEX"
                    }
            End Select

            dtf.filterCompareTo.Add(sid.Text)

            filters.Add(dtf)
        Else
            If clbTags.CheckedItems.Count > 0 Then
                Dim dtf As New DataFilter With {
                    .filterfieldname = "tpm.assettags",
                    .filterComparison = "LIKE"
                }
                For Each i In clbTags.CheckedItems
                    dtf.filterCompareTo.Add(i.ToString)
                Next

                filters.Add(dtf)
            End If

            If cblPacks.CheckedItems.Count > 0 Then
                Dim dtf As New DataFilter With {
                    .filterfieldname = "p.name",
                    .filterComparison = "IN"
                }
                For Each i In cblPacks.CheckedItems
                    dtf.filterCompareTo.Add(i.ToString)
                Next

                filters.Add(dtf)
            End If

            If clbResolution.CheckedItems.Count > 0 Then
                Dim dtf As New DataFilter With {
                    .filterfieldname = "m.resolution",
                    .filterComparison = "IN"
                }
                For Each itm In clbResolution.CheckedItems
                    dtf.filterCompareTo.Add(clbResolution.GetItemText(itm))
                Next

                filters.Add(dtf)
            End If

            If clbCats.CheckedItems.Count > 0 Then
                Dim dtf As New DataFilter With {
                    .filterfieldname = "m.category",
                    .filterComparison = "IN"
                }
                For Each itm In clbCats.CheckedItems
                    dtf.filterCompareTo.Add(clbCats.GetItemText(itm))
                Next

                filters.Add(dtf)
            End If

            If tSearch.Text.Length > 0 Then
                Dim words = tSearch.Text.Split(" "c)

                Dim dtf As New DataFilter With {
                        .filterfieldname = "m.description",
                        .filterComparison = "LIKE"
                    }

                For i As Integer = 0 To words.Length - 1
                    dtf.filterCompareTo.Add(words(i))
                Next

                filters.Add(dtf)
            End If
        End If

        Dim dte As ExtendedDatatable = Nothing

        If filters.Count > 0 Then
            dte = dbc.GetFiltered(filters, cp)
        Else
            dte = dbc.GetFilteredByNothing(cp)
        End If

        'If clbTags.CheckedItems.Count > 0 And cblPacks.CheckedItems.Count > 0 Then
        '    dte = dbc.GetFilteredByTagsAndPacks(itms, pitms, resosandcats, cp)
        'ElseIf clbTags.CheckedItems.Count > 0 And cblPacks.CheckedItems.Count = 0 Then
        '    dte = dbc.GetFilteredByTags(itms, resosandcats, cp)
        'ElseIf clbTags.CheckedItems.Count = 0 And cblPacks.CheckedItems.Count > 0 Then
        '    dte = dbc.GetFilteredByPacks(pitms, resosandcats, cp)
        'Else
        '    dte = dbc.GetFilteredByNothing(resosandcats, cp)
        'End If

        DataGridView1.DataSource = dte.dt

        cp = dte.CurrentPage
        tp = dte.TotalPages

        BuildPages()

        ShowAssets()
        EnableButtons()
    End Sub

    Private Sub BuildPages()
        bFirst.Enabled = Not cp = 0
        bLast.Enabled = Not cp = tp - 1
        bPrev.Enabled = cp > 0
        bNext.Enabled = cp < tp - 1

        RemoveHandler cbPages.SelectedIndexChanged, AddressOf cbPages_SelectedIndexChanged

        cbPages.Items.Clear()

        For i As Integer = 0 To tp - 1
            cbPages.Items.Add($"{i + 1}/{tp}")

            If cp = i Then
                cbPages.SelectedIndex = i
            End If
        Next
        AddHandler cbPages.SelectedIndexChanged, AddressOf cbPages_SelectedIndexChanged
    End Sub

    Private Sub ShowAssets()
        Try
            FlowLayoutPanel1.SuspendLayout()
            For Each ctrl As Control In FlowLayoutPanel1.Controls
                If TypeOf ctrl Is AssetInfo Then
                    Dim ai = DirectCast(ctrl, AssetInfo)

                    If ai IsNot Nothing Then
                        RemoveHandler ai.pPreview.Click, AddressOf modalPanel.ZoomAsset

                        If ai.pPreview.Image IsNot Nothing Then
                            ai.pPreview.Image.Dispose()
                            ai.pPreview.Image = Nothing
                        End If

                        ctrl.Dispose()
                    End If
                End If
            Next

            FlowLayoutPanel1.Controls.Clear()

            For Each row As DataGridViewRow In DataGridView1.Rows
                If row.IsNewRow Then Continue For

                Dim ctrl As New AssetInfo With {
                    .Name = row.Cells.Item(0).Value.ToString,
                    .PreviewImage = $"{BASE}\{row.Cells.Item(3).Value}",
                    .Description = row.Cells.Item(2).Value.ToString,
                    .Tags = row.Cells.Item(5).Value.ToString,
                    .Data = row
                }

                AddHandler ctrl.pPreview.Click, AddressOf modalPanel.ZoomAsset

                FlowLayoutPanel1.Controls.Add(ctrl)
            Next
            FlowLayoutPanel1.ResumeLayout()
            'FlowLayoutPanel1.Update()
            FlowLayoutPanel1.ScrollControlIntoView(FlowLayoutPanel1.Controls.Item(0))

            GC.Collect()
        Catch ex As Exception
            Trace.WriteLine(ex)
        End Try
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        modalPanel.Size = Me.ClientSize ' Resize modal to cover the form on resize
        modalPanel.CenterModalContent()
    End Sub

    Private Sub modalPanel_Click(sender As Object, e As EventArgs) Handles modalPanel.Click
        modalPanel.SendToBack()
        modalPanel.Visible = False
        modalPanel.pbPreview.Image.Dispose()
    End Sub

    Private Sub bAddPack_Click(sender As Object, e As EventArgs) Handles bAddPack.Click
        cblPacks.Items.Add(cbPacks.Text, True)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'dbc.BuildFileIndex()
        bTagReload.PerformClick()

        For i As Integer = 0 To clbResolution.Items.Count - 1
            clbResolution.SetItemChecked(i, True)
        Next
        For i As Integer = 0 To clbCats.Items.Count - 1
            clbCats.SetItemChecked(i, True)
        Next
    End Sub

    Private Sub bNext_Click(sender As Object, e As EventArgs) Handles bNext.Click
        cp += 1
        LoadData()
    End Sub

    Private Sub bLast_Click(sender As Object, e As EventArgs) Handles bLast.Click
        cp = tp - 1
        LoadData()
    End Sub

    Private Sub bPrev_Click(sender As Object, e As EventArgs) Handles bPrev.Click
        cp -= 1
        LoadData()
    End Sub

    Private Sub bFirst_Click(sender As Object, e As EventArgs) Handles bFirst.Click
        cp = 0
        LoadData()
    End Sub

    Private Sub cbPages_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbPages.SelectedIndexChanged
        cp = cbPages.SelectedIndex
        LoadData()
    End Sub

    Private Sub bDebug_Click(sender As Object, e As EventArgs) Handles bDebug.Click
        If pDebug.Top = 0 Then
            pDebug.Top = -521
        Else
            pDebug.Top = 0
        End If
    End Sub

    Private Sub tSearch_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles tSearch.PreviewKeyDown
        If e.KeyCode = Keys.Enter Then
            bSearch.PerformClick()
        End If
    End Sub

    Private Sub sid_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles sid.PreviewKeyDown
        If e.KeyCode = Keys.Enter Then
            bSearch.PerformClick()
        End If
    End Sub
End Class
