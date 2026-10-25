Imports System.IO
Imports System.Runtime.InteropServices
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

                                Dim rPtr As IntPtr = rImg.GetPixels()
                                Dim gPtr As IntPtr = If(gImg IsNot Nothing, gImg.GetPixels(), IntPtr.Zero)
                                Dim bPtr As IntPtr = If(bImg IsNot Nothing, bImg.GetPixels(), IntPtr.Zero)
                                Dim aPtr As IntPtr = If(aImg IsNot Nothing, aImg.GetPixels(), IntPtr.Zero)

                                Dim dstPtr As IntPtr = packed.GetPixels()

                                Dim rStride As Integer = rImg.RowBytes
                                Dim gStride As Integer = If(gImg IsNot Nothing, gImg.RowBytes, 0)
                                Dim bStride As Integer = If(bImg IsNot Nothing, bImg.RowBytes, 0)
                                Dim aStride As Integer = If(aImg IsNot Nothing, aImg.RowBytes, 0)

                                Dim dstStride As Integer = packed.RowBytes

                                For y As Integer = 0 To h - 1

                                    cancel.ThrowIfCancellationRequested()

                                    For x As Integer = 0 To w - 1

                                        Dim rOffset As Integer = y * rStride + x * 4
                                        Dim rColor As Integer = Marshal.ReadInt32(rPtr, rOffset)

                                        Dim rV As Byte = GetLuma8(ColorFromRgba(rColor))

                                        Dim gV As Byte = 255
                                        If gPtr <> IntPtr.Zero Then
                                            Dim gOffset As Integer = y * gStride + x * 4
                                            gV = GetLuma8(ColorFromRgba(Marshal.ReadInt32(gPtr, gOffset)))
                                        End If

                                        Dim bV As Byte = 0
                                        If bPtr <> IntPtr.Zero Then
                                            Dim bOffset As Integer = y * bStride + x * 4
                                            bV = GetLuma8(ColorFromRgba(Marshal.ReadInt32(bPtr, bOffset)))
                                        End If

                                        Dim aV As Byte = 0
                                        If aPtr <> IntPtr.Zero Then
                                            Dim aOffset As Integer = y * aStride + x * 4
                                            aV = GetLuma8(ColorFromRgba(Marshal.ReadInt32(aPtr, aOffset)))
                                        End If

                                        Dim packedValue As Integer =
                                        CInt(rV) Or
                                        (CInt(gV) << 8) Or
                                        (CInt(bV) << 16) Or
                                        (CInt(aV) << 24)

                                        Dim dstOffset As Integer = y * dstStride + x * 4

                                        Marshal.WriteInt32(dstPtr, dstOffset, packedValue)

                                    Next

                                    If y Mod 8 = 0 Then
                                        progress?.Report(y * yInc)
                                    End If

                                Next

                                packed.NotifyPixelsChanged()

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

    Private Shared Function ColorFromRgba(value As Integer) As SKColor

        Return New SKColor(
        CByte(value And &HFF),
        CByte((value >> 8) And &HFF),
        CByte((value >> 16) And &HFF),
        CByte((value >> 24) And &HFF)
    )

    End Function
End Class
