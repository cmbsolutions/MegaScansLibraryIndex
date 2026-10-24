<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AssetInfo
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        pPreview = New PictureBox()
        lDescription = New Label()
        tTip = New ToolTip(components)
        CType(pPreview, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pPreview
        ' 
        pPreview.Image = My.Resources.Resources.nopreview
        pPreview.Location = New Point(4, 4)
        pPreview.Margin = New Padding(0)
        pPreview.Name = "pPreview"
        pPreview.Size = New Size(128, 128)
        pPreview.SizeMode = PictureBoxSizeMode.StretchImage
        pPreview.TabIndex = 0
        pPreview.TabStop = False
        pPreview.WaitOnLoad = True
        ' 
        ' lDescription
        ' 
        lDescription.AutoEllipsis = True
        lDescription.ForeColor = Color.WhiteSmoke
        lDescription.Location = New Point(4, 135)
        lDescription.Name = "lDescription"
        lDescription.Size = New Size(128, 35)
        lDescription.TabIndex = 1
        lDescription.Text = "Label1"
        lDescription.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' tTip
        ' 
        tTip.BackColor = Color.FromArgb(CByte(96), CByte(96), CByte(96))
        tTip.ForeColor = Color.Gold
        ' 
        ' AssetInfo
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(36), CByte(36), CByte(36))
        Controls.Add(lDescription)
        Controls.Add(pPreview)
        Name = "AssetInfo"
        Size = New Size(136, 170)
        CType(pPreview, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pPreview As PictureBox
    Friend WithEvents lDescription As Label
    Friend WithEvents tTip As ToolTip

End Class
