Imports System.IO

Public Class AssetInfo
    Private _PreviewImage As String
    Public Property PreviewImage As String
        Get
            Return _PreviewImage
        End Get
        Set(value As String)
            _PreviewImage = value

            If IO.File.Exists(value) Then
                Using imgStream As New FileStream(value, FileMode.Open, FileAccess.Read)
                    pPreview.Image = Image.FromStream(imgStream)
                End Using
            End If
        End Set
    End Property
    Public Property Description As String
        Get
            Return lDescription.Text
        End Get
        Set(value As String)
            lDescription.Text = value
            tTip.SetToolTip(lDescription, value)
        End Set
    End Property
    Public WriteOnly Property Tags As String
        Set(value As String)
            tTip.SetToolTip(pPreview, value)
        End Set
    End Property

    Public Property Data As DataGridViewRow
End Class
