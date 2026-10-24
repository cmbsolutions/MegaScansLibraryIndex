<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModalWindow
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
        rtbInfo = New RichTextBox()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        tsmUberPacker = New ToolStripMenuItem()
        tsmCreatePackedFile = New ToolStripMenuItem()
        ToolStripSeparator1 = New ToolStripSeparator()
        tsmAO = New ToolStripMenuItem()
        tsmRoughness = New ToolStripMenuItem()
        tsmSpecular = New ToolStripMenuItem()
        tsmGloss = New ToolStripMenuItem()
        tsmDisplacement = New ToolStripMenuItem()
        tsmCavity = New ToolStripMenuItem()
        tsmBump = New ToolStripMenuItem()
        modalContent = New Panel()
        lblStatus = New Label()
        btnCancel = New Button()
        pBar = New ProgressBar()
        TabControl1 = New TabControl()
        TabPage1 = New TabPage()
        TabPage2 = New TabPage()
        pbPreview = New PictureBox()
        OpenglRenderer1 = New OpenGLRenderer()
        ContextMenuStrip1.SuspendLayout()
        modalContent.SuspendLayout()
        TabControl1.SuspendLayout()
        TabPage1.SuspendLayout()
        TabPage2.SuspendLayout()
        CType(pbPreview, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' rtbInfo
        ' 
        rtbInfo.BackColor = Color.FromArgb(CByte(56), CByte(56), CByte(56))
        rtbInfo.BorderStyle = BorderStyle.None
        rtbInfo.ContextMenuStrip = ContextMenuStrip1
        rtbInfo.Dock = DockStyle.Right
        rtbInfo.ForeColor = Color.WhiteSmoke
        rtbInfo.Location = New Point(519, 0)
        rtbInfo.Margin = New Padding(0)
        rtbInfo.MaximumSize = New Size(300, 1024)
        rtbInfo.MinimumSize = New Size(300, 512)
        rtbInfo.Name = "rtbInfo"
        rtbInfo.ReadOnly = True
        rtbInfo.Size = New Size(300, 512)
        rtbInfo.TabIndex = 1
        rtbInfo.Text = "Asset ID:" & vbLf & "<asset_id>" & vbLf & "Name:" & vbLf & "<name>" & vbLf & "Description:" & vbLf & "<description>" & vbLf & "Tags:" & vbLf & "<tags>" & vbLf & "Category:" & vbLf & "<category>" & vbLf & "Resolution:" & vbLf & "<resolution>" & vbLf & "Size:" & vbLf & "<size>" & vbLf & "Number of files:" & vbLf & "<filecount>" & vbLf & "Location:" & vbLf & "file://<dgvlocation>"
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {tsmUberPacker, tsmCreatePackedFile, ToolStripSeparator1, tsmAO, tsmRoughness, tsmSpecular, tsmGloss, tsmDisplacement, tsmCavity, tsmBump})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(288, 208)
        ' 
        ' tsmUberPacker
        ' 
        tsmUberPacker.Name = "tsmUberPacker"
        tsmUberPacker.Size = New Size(287, 22)
        tsmUberPacker.Text = "Create packed file by selecting 4 textures"
        ' 
        ' tsmCreatePackedFile
        ' 
        tsmCreatePackedFile.Name = "tsmCreatePackedFile"
        tsmCreatePackedFile.Size = New Size(287, 22)
        tsmCreatePackedFile.Text = "Create packed file"
        ' 
        ' ToolStripSeparator1
        ' 
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(284, 6)
        ' 
        ' tsmAO
        ' 
        tsmAO.Checked = True
        tsmAO.CheckOnClick = True
        tsmAO.CheckState = CheckState.Checked
        tsmAO.Name = "tsmAO"
        tsmAO.Size = New Size(287, 22)
        tsmAO.Text = "AO"
        ' 
        ' tsmRoughness
        ' 
        tsmRoughness.Checked = True
        tsmRoughness.CheckOnClick = True
        tsmRoughness.CheckState = CheckState.Checked
        tsmRoughness.Name = "tsmRoughness"
        tsmRoughness.Size = New Size(287, 22)
        tsmRoughness.Text = "Roughness"
        ' 
        ' tsmSpecular
        ' 
        tsmSpecular.CheckOnClick = True
        tsmSpecular.Name = "tsmSpecular"
        tsmSpecular.Size = New Size(287, 22)
        tsmSpecular.Text = "Specular"
        ' 
        ' tsmGloss
        ' 
        tsmGloss.Checked = True
        tsmGloss.CheckOnClick = True
        tsmGloss.CheckState = CheckState.Checked
        tsmGloss.Name = "tsmGloss"
        tsmGloss.Size = New Size(287, 22)
        tsmGloss.Text = "Gloss"
        ' 
        ' tsmDisplacement
        ' 
        tsmDisplacement.Checked = True
        tsmDisplacement.CheckOnClick = True
        tsmDisplacement.CheckState = CheckState.Checked
        tsmDisplacement.Name = "tsmDisplacement"
        tsmDisplacement.Size = New Size(287, 22)
        tsmDisplacement.Text = "Displacement"
        ' 
        ' tsmCavity
        ' 
        tsmCavity.CheckOnClick = True
        tsmCavity.Name = "tsmCavity"
        tsmCavity.Size = New Size(287, 22)
        tsmCavity.Text = "Cavity"
        ' 
        ' tsmBump
        ' 
        tsmBump.CheckOnClick = True
        tsmBump.Name = "tsmBump"
        tsmBump.Size = New Size(287, 22)
        tsmBump.Text = "Bump"
        ' 
        ' modalContent
        ' 
        modalContent.Controls.Add(TabControl1)
        modalContent.Controls.Add(lblStatus)
        modalContent.Controls.Add(btnCancel)
        modalContent.Controls.Add(pBar)
        modalContent.Controls.Add(rtbInfo)
        modalContent.Location = New Point(50, 50)
        modalContent.Margin = New Padding(0)
        modalContent.MaximumSize = New Size(1319, 512)
        modalContent.MinimumSize = New Size(819, 512)
        modalContent.Name = "modalContent"
        modalContent.Size = New Size(819, 512)
        modalContent.TabIndex = 2
        ' 
        ' lblStatus
        ' 
        lblStatus.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        lblStatus.ForeColor = Color.WhiteSmoke
        lblStatus.Location = New Point(567, 466)
        lblStatus.Name = "lblStatus"
        lblStatus.Size = New Size(190, 20)
        lblStatus.TabIndex = 4
        lblStatus.Text = "Status:"
        lblStatus.TextAlign = ContentAlignment.MiddleCenter
        lblStatus.UseCompatibleTextRendering = True
        lblStatus.Visible = False
        ' 
        ' btnCancel
        ' 
        btnCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnCancel.Image = My.Resources.Resources.cancel
        btnCancel.Location = New Point(791, 489)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New Size(25, 23)
        btnCancel.TabIndex = 3
        btnCancel.UseVisualStyleBackColor = True
        btnCancel.Visible = False
        ' 
        ' pBar
        ' 
        pBar.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        pBar.Location = New Point(517, 489)
        pBar.Name = "pBar"
        pBar.Size = New Size(275, 23)
        pBar.Style = ProgressBarStyle.Continuous
        pBar.TabIndex = 2
        pBar.Value = 50
        pBar.Visible = False
        ' 
        ' TabControl1
        ' 
        TabControl1.Alignment = TabAlignment.Left
        TabControl1.Controls.Add(TabPage1)
        TabControl1.Controls.Add(TabPage2)
        TabControl1.Dock = DockStyle.Fill
        TabControl1.Location = New Point(0, 0)
        TabControl1.Multiline = True
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(519, 512)
        TabControl1.SizeMode = TabSizeMode.Fixed
        TabControl1.TabIndex = 5
        ' 
        ' TabPage1
        ' 
        TabPage1.BackColor = Color.FromArgb(CByte(56), CByte(56), CByte(56))
        TabPage1.Controls.Add(pbPreview)
        TabPage1.ForeColor = Color.WhiteSmoke
        TabPage1.Location = New Point(27, 4)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(3)
        TabPage1.Size = New Size(488, 504)
        TabPage1.TabIndex = 0
        TabPage1.Text = "2D Preview"
        ' 
        ' TabPage2
        ' 
        TabPage2.BackColor = Color.FromArgb(CByte(56), CByte(56), CByte(56))
        TabPage2.Controls.Add(OpenglRenderer1)
        TabPage2.ForeColor = Color.WhiteSmoke
        TabPage2.Location = New Point(27, 4)
        TabPage2.Name = "TabPage2"
        TabPage2.Padding = New Padding(3)
        TabPage2.Size = New Size(488, 504)
        TabPage2.TabIndex = 1
        TabPage2.Text = "3D Preview"
        ' 
        ' pbPreview
        ' 
        pbPreview.Dock = DockStyle.Fill
        pbPreview.Location = New Point(3, 3)
        pbPreview.Name = "pbPreview"
        pbPreview.Size = New Size(482, 498)
        pbPreview.TabIndex = 0
        pbPreview.TabStop = False
        ' 
        ' OpenglRenderer1
        ' 
        OpenglRenderer1.Dock = DockStyle.Fill
        OpenglRenderer1.Location = New Point(3, 3)
        OpenglRenderer1.Name = "OpenglRenderer1"
        OpenglRenderer1.Size = New Size(482, 498)
        OpenglRenderer1.TabIndex = 0
        ' 
        ' ModalWindow
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(36), CByte(36), CByte(36))
        BackgroundImageLayout = ImageLayout.Center
        Controls.Add(modalContent)
        Margin = New Padding(0)
        Name = "ModalWindow"
        Size = New Size(970, 642)
        ContextMenuStrip1.ResumeLayout(False)
        modalContent.ResumeLayout(False)
        TabControl1.ResumeLayout(False)
        TabPage1.ResumeLayout(False)
        TabPage2.ResumeLayout(False)
        CType(pbPreview, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub
    Friend WithEvents rtbInfo As RichTextBox
    Friend WithEvents modalContent As Panel
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents pBar As ProgressBar
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblStatus As Label
    Friend WithEvents tsmCreatePackedFile As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents tsmAO As ToolStripMenuItem
    Friend WithEvents tsmRoughness As ToolStripMenuItem
    Friend WithEvents tsmSpecular As ToolStripMenuItem
    Friend WithEvents tsmGloss As ToolStripMenuItem
    Friend WithEvents tsmDisplacement As ToolStripMenuItem
    Friend WithEvents tsmCavity As ToolStripMenuItem
    Friend WithEvents tsmBump As ToolStripMenuItem
    Friend WithEvents tsmUberPacker As ToolStripMenuItem
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents pbPreview As PictureBox
    Friend WithEvents OpenglRenderer1 As OpenGLRenderer

End Class
