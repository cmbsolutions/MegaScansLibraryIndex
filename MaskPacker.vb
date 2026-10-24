Imports System.IO
Imports SkiaSharp

Public Class MaskPacker
    Public Shared Sub Pack(root As String, suffixes As List(Of String), outputFile As String, progress As IProgress(Of Integer), cancel As Threading.CancellationToken)
        Dim exts = {".png", ".tga", ".jpg", ".jpeg"}

        Dim paths As New List(Of String)

        For Each s In suffixes
            Dim p = FindFirstMatch(root, s, exts)
            paths.Add(p)
        Next

        paths.AddRange(New String(4 - paths.Count - 1) {}) ' pad to 4 with nulls

        If suffixes.Count > 4 Then Throw New ArgumentException("Too many suffixes, max 4 for RGBA channels.")

        Try
            Using rImg As SKBitmap = SKBitmap.Decode(paths(0))

                Dim w = rImg.Width
                Dim h = rImg.Height
                Dim yInc As Integer = w \ h


                Using gImg As SKBitmap = LoadAndFit(paths(1), w, h)
                    Using bImg As SKBitmap = LoadAndFit(paths(2), w, h)
                        Using aImg As SKBitmap = LoadAndFit(paths(3), w, h)

                            Using packed As SKBitmap = New SKBitmap(w, h, SKColorType.Rgba8888, SKAlphaType.Unpremul)

                                For y As Integer = 0 To h - 1
                                    cancel.ThrowIfCancellationRequested()

                                    For x As Integer = 0 To w - 1

                                        Dim rV As Byte = GetLuma8(rImg.GetPixel(x, y))

                                        Dim gV As Byte =
                                        If(gImg IsNot Nothing, GetLuma8(gImg.GetPixel(x, y)), CByte(255))

                                        Dim bV As Byte =
                                        If(bImg IsNot Nothing, GetLuma8(bImg.GetPixel(x, y)), CByte(0))

                                        Dim aV As Byte =
                                        If(aImg IsNot Nothing, GetLuma8(aImg.GetPixel(x, y)), CByte(0))

                                        packed.SetPixel(x, y, New SKColor(rV, gV, bV, aV))
                                    Next

                                    If y Mod 8 = 0 Then progress?.Report(y * yInc)
                                Next

                                progress?.Report((h * yInc) + 1) ' 100% + done signal
                                Using skImage As SKImage = SKImage.FromBitmap(packed)
                                    Using encoded As SKData = skImage.Encode(SKEncodedImageFormat.Png, 100)
                                        File.WriteAllBytes(outputFile, encoded.ToArray())
                                    End Using
                                End Using
                                progress?.Report((h * yInc) + 2) ' 100% + done signal
                            End Using
                        End Using
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Trace.WriteLine(ex)
        End Try
    End Sub

    Private Shared Function FindFirstMatch(dir As String, stem As String, exts() As String) As String
        For Each f In Directory.EnumerateFiles(dir)
            Dim fn = Path.GetFileNameWithoutExtension(f)
            If fn.EndsWith(stem, StringComparison.OrdinalIgnoreCase) AndAlso exts.Contains(Path.GetExtension(f).ToLower) Then
                Return f
            End If
        Next

        Return Nothing
    End Function

    ' Load file as RGBA, resize to w/h if needed.
    ' Returns Nothing if path missing.
    Private Shared Function LoadAndFit(path As String, w As Integer, h As Integer) As SKBitmap
        If String.IsNullOrWhiteSpace(path) OrElse Not File.Exists(path) Then
            Return New SKBitmap(w, h, SKColorType.Rgba8888, SKAlphaType.Unpremul) ' blank
        End If

        Dim img = SKBitmap.Decode(path)
        If img.Width <> w OrElse img.Height <> h Then
            Dim resized As New SKBitmap(w, h, SKColorType.Rgba8888, SKAlphaType.Unpremul)
            Using canvas As New SKCanvas(resized)
                canvas.Scale(w / img.Width, h / img.Height)
                Dim opt As New SKSamplingOptions
                canvas.DrawBitmap(img, 0, 0, opt)
            End Using
            Return resized
        End If
        Return img
    End Function

    ' Convert pixel to grayscale via luminance, return 0-255.
    Private Shared Function GetLuma8(px As SKColor) As Byte
        ' sRGB-ish luma coefficients
        Dim l = 0.2126F * px.Red + 0.7152F * px.Green + 0.0722F * px.Blue
        If l < 0 Then l = 0
        If l > 255 Then l = 255
        Return CByte(l)
    End Function
End Class
