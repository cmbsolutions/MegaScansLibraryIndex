<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Button1 = New Button()
        clbTags = New CheckedListBox()
        bTagReload = New Button()
        GroupBox1 = New GroupBox()
        cboSearchByIdOption = New ComboBox()
        sid = New TextBox()
        tSearch = New TextBox()
        Label3 = New Label()
        clbCats = New CheckedListBox()
        clbResolution = New CheckedListBox()
        TableLayoutPanel1 = New TableLayoutPanel()
        bFirst = New Button()
        bPrev = New Button()
        cbPages = New ComboBox()
        bNext = New Button()
        bLast = New Button()
        cblPacks = New CheckedListBox()
        bAddPack = New Button()
        cbPacks = New ComboBox()
        Label2 = New Label()
        bSearch = New Button()
        bAddTag = New Button()
        cbTags = New ComboBox()
        Label1 = New Label()
        DataGridView1 = New DataGridView()
        megascan_id = New DataGridViewTextBoxColumn()
        id = New DataGridViewTextBoxColumn()
        description = New DataGridViewTextBoxColumn()
        previewimage = New DataGridViewTextBoxColumn()
        packname = New DataGridViewTextBoxColumn()
        assettags = New DataGridViewTextBoxColumn()
        resolution = New DataGridViewTextBoxColumn()
        category = New DataGridViewTextBoxColumn()
        dgvClocation = New DataGridViewTextBoxColumn()
        totalsize = New DataGridViewTextBoxColumn()
        filecount = New DataGridViewTextBoxColumn()
        FlowLayoutPanel1 = New FlowLayoutPanel()
        pDebug = New Panel()
        rtbLog = New RichTextBox()
        bDebug = New Button()
        GroupBox1.SuspendLayout()
        TableLayoutPanel1.SuspendLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        pDebug.SuspendLayout()
        SuspendLayout()
        ' 
        ' Button1
        ' 
        Button1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Button1.BackColor = Color.FromArgb(56, 56, 56)
        Button1.FlatAppearance.BorderSize = 0
        Button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(96, 96, 96)
        Button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(125, 125, 125)
        Button1.FlatStyle = FlatStyle.Flat
        Button1.Location = New Point(1185, 19)
        Button1.Name = "Button1"
        Button1.Size = New Size(100, 23)
        Button1.TabIndex = 0
        Button1.Text = "Build database"
        Button1.UseVisualStyleBackColor = False
        ' 
        ' clbTags
        ' 
        clbTags.BackColor = Color.FromArgb(56, 56, 56)
        clbTags.BorderStyle = BorderStyle.None
        clbTags.CheckOnClick = True
        clbTags.ForeColor = Color.WhiteSmoke
        clbTags.FormattingEnabled = True
        clbTags.HorizontalScrollbar = True
        clbTags.Location = New Point(3, 44)
        clbTags.MultiColumn = True
        clbTags.Name = "clbTags"
        clbTags.Size = New Size(259, 72)
        clbTags.TabIndex = 1
        ' 
        ' bTagReload
        ' 
        bTagReload.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        bTagReload.BackColor = Color.FromArgb(56, 56, 56)
        bTagReload.FlatAppearance.BorderSize = 0
        bTagReload.FlatAppearance.MouseDownBackColor = Color.FromArgb(96, 96, 96)
        bTagReload.FlatAppearance.MouseOverBackColor = Color.FromArgb(125, 125, 125)
        bTagReload.FlatStyle = FlatStyle.Flat
        bTagReload.Location = New Point(1185, 48)
        bTagReload.Name = "bTagReload"
        bTagReload.Size = New Size(75, 23)
        bTagReload.TabIndex = 2
        bTagReload.Text = "Reload"
        bTagReload.UseVisualStyleBackColor = False
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(cboSearchByIdOption)
        GroupBox1.Controls.Add(sid)
        GroupBox1.Controls.Add(tSearch)
        GroupBox1.Controls.Add(Label3)
        GroupBox1.Controls.Add(clbCats)
        GroupBox1.Controls.Add(clbResolution)
        GroupBox1.Controls.Add(TableLayoutPanel1)
        GroupBox1.Controls.Add(cblPacks)
        GroupBox1.Controls.Add(bAddPack)
        GroupBox1.Controls.Add(cbPacks)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(clbTags)
        GroupBox1.Controls.Add(bSearch)
        GroupBox1.Controls.Add(bAddTag)
        GroupBox1.Controls.Add(bTagReload)
        GroupBox1.Controls.Add(Button1)
        GroupBox1.Controls.Add(cbTags)
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Dock = DockStyle.Top
        GroupBox1.ForeColor = Color.WhiteSmoke
        GroupBox1.Location = New Point(0, 0)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(1290, 118)
        GroupBox1.TabIndex = 3
        GroupBox1.TabStop = False
        GroupBox1.Text = "Filters"
        ' 
        ' cboSearchByIdOption
        ' 
        cboSearchByIdOption.BackColor = Color.FromArgb(56, 56, 56)
        cboSearchByIdOption.FlatStyle = FlatStyle.Flat
        cboSearchByIdOption.ForeColor = Color.WhiteSmoke
        cboSearchByIdOption.FormattingEnabled = True
        cboSearchByIdOption.Items.AddRange(New Object() {"Starts with", "Ends with", "Contains", "Soundex"})
        cboSearchByIdOption.Location = New Point(862, 28)
        cboSearchByIdOption.Name = "cboSearchByIdOption"
        cboSearchByIdOption.Size = New Size(121, 23)
        cboSearchByIdOption.TabIndex = 18
        ' 
        ' sid
        ' 
        sid.BackColor = Color.FromArgb(56, 56, 56)
        sid.BorderStyle = BorderStyle.FixedSingle
        sid.ForeColor = Color.WhiteSmoke
        sid.Location = New Point(862, 57)
        sid.MaxLength = 25
        sid.Name = "sid"
        sid.PlaceholderText = "Search by ID"
        sid.Size = New Size(121, 23)
        sid.TabIndex = 16
        ' 
        ' tSearch
        ' 
        tSearch.BackColor = Color.FromArgb(56, 56, 56)
        tSearch.BorderStyle = BorderStyle.FixedSingle
        tSearch.ForeColor = Color.WhiteSmoke
        tSearch.Location = New Point(580, 57)
        tSearch.MaxLength = 25
        tSearch.Name = "tSearch"
        tSearch.PlaceholderText = "Search for names"
        tSearch.Size = New Size(276, 23)
        tSearch.TabIndex = 15
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(536, 16)
        Label3.Name = "Label3"
        Label3.Size = New Size(38, 15)
        Label3.TabIndex = 14
        Label3.Text = "Filters"
        ' 
        ' clbCats
        ' 
        clbCats.BackColor = Color.FromArgb(56, 56, 56)
        clbCats.BorderStyle = BorderStyle.None
        clbCats.CheckOnClick = True
        clbCats.ColumnWidth = 60
        clbCats.ForeColor = Color.WhiteSmoke
        clbCats.FormattingEnabled = True
        clbCats.Items.AddRange(New Object() {"3d", "3dplant", "atlas", "brush", "surface", "decal"})
        clbCats.Location = New Point(666, 15)
        clbCats.MultiColumn = True
        clbCats.Name = "clbCats"
        clbCats.Size = New Size(190, 36)
        clbCats.TabIndex = 13
        ' 
        ' clbResolution
        ' 
        clbResolution.BackColor = Color.FromArgb(56, 56, 56)
        clbResolution.BorderStyle = BorderStyle.None
        clbResolution.CheckOnClick = True
        clbResolution.ColumnWidth = 40
        clbResolution.ForeColor = Color.WhiteSmoke
        clbResolution.FormattingEnabled = True
        clbResolution.Items.AddRange(New Object() {"1K", "2K", "4K", "8K"})
        clbResolution.Location = New Point(580, 15)
        clbResolution.MultiColumn = True
        clbResolution.Name = "clbResolution"
        clbResolution.Size = New Size(86, 36)
        clbResolution.TabIndex = 12
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.AutoSize = True
        TableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink
        TableLayoutPanel1.ColumnCount = 5
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 28.0F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 28.0F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 100.0F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 28.0F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 28.0F))
        TableLayoutPanel1.Controls.Add(bFirst, 0, 0)
        TableLayoutPanel1.Controls.Add(bPrev, 1, 0)
        TableLayoutPanel1.Controls.Add(cbPages, 2, 0)
        TableLayoutPanel1.Controls.Add(bNext, 3, 0)
        TableLayoutPanel1.Controls.Add(bLast, 4, 0)
        TableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        TableLayoutPanel1.Location = New Point(580, 86)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Absolute, 30.0F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Absolute, 20.0F))
        TableLayoutPanel1.Size = New Size(212, 30)
        TableLayoutPanel1.TabIndex = 11
        ' 
        ' bFirst
        ' 
        bFirst.FlatAppearance.BorderSize = 0
        bFirst.FlatStyle = FlatStyle.Flat
        bFirst.Image = My.Resources.Resources.button_green_first
        bFirst.Location = New Point(3, 3)
        bFirst.Name = "bFirst"
        bFirst.Size = New Size(22, 23)
        bFirst.TabIndex = 0
        bFirst.UseVisualStyleBackColor = True
        ' 
        ' bPrev
        ' 
        bPrev.FlatAppearance.BorderSize = 0
        bPrev.FlatStyle = FlatStyle.Flat
        bPrev.Image = My.Resources.Resources.button_green_previous
        bPrev.Location = New Point(31, 3)
        bPrev.Name = "bPrev"
        bPrev.Size = New Size(22, 23)
        bPrev.TabIndex = 1
        bPrev.UseVisualStyleBackColor = True
        ' 
        ' cbPages
        ' 
        cbPages.BackColor = Color.FromArgb(56, 56, 56)
        cbPages.DropDownStyle = ComboBoxStyle.DropDownList
        cbPages.FlatStyle = FlatStyle.Flat
        cbPages.ForeColor = Color.WhiteSmoke
        cbPages.FormattingEnabled = True
        cbPages.Location = New Point(59, 3)
        cbPages.Name = "cbPages"
        cbPages.Size = New Size(94, 23)
        cbPages.TabIndex = 2
        ' 
        ' bNext
        ' 
        bNext.FlatAppearance.BorderSize = 0
        bNext.FlatStyle = FlatStyle.Flat
        bNext.Image = My.Resources.Resources.button_green_next
        bNext.Location = New Point(159, 3)
        bNext.Name = "bNext"
        bNext.Size = New Size(22, 23)
        bNext.TabIndex = 3
        bNext.UseVisualStyleBackColor = True
        ' 
        ' bLast
        ' 
        bLast.FlatAppearance.BorderSize = 0
        bLast.FlatStyle = FlatStyle.Flat
        bLast.Image = My.Resources.Resources.button_green_last
        bLast.Location = New Point(187, 3)
        bLast.Name = "bLast"
        bLast.Size = New Size(22, 23)
        bLast.TabIndex = 4
        bLast.UseVisualStyleBackColor = True
        ' 
        ' cblPacks
        ' 
        cblPacks.BackColor = Color.FromArgb(56, 56, 56)
        cblPacks.BorderStyle = BorderStyle.None
        cblPacks.CheckOnClick = True
        cblPacks.ForeColor = Color.WhiteSmoke
        cblPacks.FormattingEnabled = True
        cblPacks.HorizontalScrollbar = True
        cblPacks.Location = New Point(268, 44)
        cblPacks.MultiColumn = True
        cblPacks.Name = "cblPacks"
        cblPacks.Size = New Size(262, 72)
        cblPacks.TabIndex = 7
        ' 
        ' bAddPack
        ' 
        bAddPack.BackColor = Color.FromArgb(56, 56, 56)
        bAddPack.FlatAppearance.BorderSize = 0
        bAddPack.FlatAppearance.MouseDownBackColor = Color.FromArgb(96, 96, 96)
        bAddPack.FlatAppearance.MouseOverBackColor = Color.FromArgb(125, 125, 125)
        bAddPack.FlatStyle = FlatStyle.Flat
        bAddPack.Location = New Point(508, 15)
        bAddPack.Name = "bAddPack"
        bAddPack.Size = New Size(22, 23)
        bAddPack.TabIndex = 10
        bAddPack.Text = "+"
        bAddPack.UseVisualStyleBackColor = False
        ' 
        ' cbPacks
        ' 
        cbPacks.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cbPacks.AutoCompleteSource = AutoCompleteSource.ListItems
        cbPacks.BackColor = Color.FromArgb(56, 56, 56)
        cbPacks.FlatStyle = FlatStyle.Flat
        cbPacks.ForeColor = Color.WhiteSmoke
        cbPacks.FormattingEnabled = True
        cbPacks.Location = New Point(310, 15)
        cbPacks.Name = "cbPacks"
        cbPacks.Size = New Size(192, 23)
        cbPacks.Sorted = True
        cbPacks.TabIndex = 9
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(268, 19)
        Label2.Name = "Label2"
        Label2.Size = New Size(37, 15)
        Label2.TabIndex = 8
        Label2.Text = "Packs"
        ' 
        ' bSearch
        ' 
        bSearch.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        bSearch.BackColor = Color.FromArgb(56, 56, 56)
        bSearch.FlatAppearance.BorderSize = 0
        bSearch.FlatAppearance.MouseDownBackColor = Color.FromArgb(96, 96, 96)
        bSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(125, 125, 125)
        bSearch.FlatStyle = FlatStyle.Flat
        bSearch.Location = New Point(1185, 77)
        bSearch.Name = "bSearch"
        bSearch.Size = New Size(75, 23)
        bSearch.TabIndex = 6
        bSearch.Text = "Search"
        bSearch.UseVisualStyleBackColor = False
        ' 
        ' bAddTag
        ' 
        bAddTag.BackColor = Color.FromArgb(56, 56, 56)
        bAddTag.FlatAppearance.BorderSize = 0
        bAddTag.FlatAppearance.MouseDownBackColor = Color.FromArgb(96, 96, 96)
        bAddTag.FlatAppearance.MouseOverBackColor = Color.FromArgb(125, 125, 125)
        bAddTag.FlatStyle = FlatStyle.Flat
        bAddTag.Location = New Point(240, 15)
        bAddTag.Name = "bAddTag"
        bAddTag.Size = New Size(22, 23)
        bAddTag.TabIndex = 5
        bAddTag.Text = "+"
        bAddTag.UseVisualStyleBackColor = False
        ' 
        ' cbTags
        ' 
        cbTags.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cbTags.AutoCompleteSource = AutoCompleteSource.ListItems
        cbTags.BackColor = Color.FromArgb(56, 56, 56)
        cbTags.FlatStyle = FlatStyle.Flat
        cbTags.ForeColor = Color.WhiteSmoke
        cbTags.FormattingEnabled = True
        cbTags.Location = New Point(42, 15)
        cbTags.Name = "cbTags"
        cbTags.Size = New Size(192, 23)
        cbTags.Sorted = True
        cbTags.TabIndex = 4
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(6, 19)
        Label1.Name = "Label1"
        Label1.Size = New Size(31, 15)
        Label1.TabIndex = 2
        Label1.Text = "Tags"
        ' 
        ' DataGridView1
        ' 
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Columns.AddRange(New DataGridViewColumn() {megascan_id, id, description, previewimage, packname, assettags, resolution, category, dgvClocation, totalsize, filecount})
        DataGridView1.Location = New Point(3, 3)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.ReadOnly = True
        DataGridView1.Size = New Size(1284, 409)
        DataGridView1.TabIndex = 4
        ' 
        ' megascan_id
        ' 
        megascan_id.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        megascan_id.DataPropertyName = "megascan_id"
        megascan_id.FillWeight = 90.0F
        megascan_id.HeaderText = "megascan_id"
        megascan_id.Name = "megascan_id"
        megascan_id.ReadOnly = True
        megascan_id.Width = 101
        ' 
        ' id
        ' 
        id.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        id.DataPropertyName = "id"
        id.FillWeight = 90.0F
        id.HeaderText = "asset id"
        id.Name = "id"
        id.ReadOnly = True
        id.Width = 71
        ' 
        ' description
        ' 
        description.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        description.DataPropertyName = "description"
        description.FillWeight = 90.0F
        description.HeaderText = "Description"
        description.Name = "description"
        description.ReadOnly = True
        description.Width = 92
        ' 
        ' previewimage
        ' 
        previewimage.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        previewimage.DataPropertyName = "previewimage"
        previewimage.FillWeight = 90.0F
        previewimage.HeaderText = "preview"
        previewimage.Name = "previewimage"
        previewimage.ReadOnly = True
        previewimage.Resizable = DataGridViewTriState.True
        previewimage.SortMode = DataGridViewColumnSortMode.NotSortable
        previewimage.Width = 54
        ' 
        ' packname
        ' 
        packname.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        packname.DataPropertyName = "packname"
        packname.FillWeight = 90.0F
        packname.HeaderText = "Pack"
        packname.Name = "packname"
        packname.ReadOnly = True
        packname.Width = 57
        ' 
        ' assettags
        ' 
        assettags.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        assettags.DataPropertyName = "assettags"
        assettags.HeaderText = "Tags"
        assettags.Name = "assettags"
        assettags.ReadOnly = True
        assettags.Width = 56
        ' 
        ' resolution
        ' 
        resolution.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        resolution.DataPropertyName = "resolution"
        resolution.HeaderText = "Resolution"
        resolution.Name = "resolution"
        resolution.ReadOnly = True
        resolution.Width = 88
        ' 
        ' category
        ' 
        category.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        category.DataPropertyName = "category"
        category.HeaderText = "Category"
        category.Name = "category"
        category.ReadOnly = True
        category.Width = 80
        ' 
        ' dgvClocation
        ' 
        dgvClocation.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        dgvClocation.DataPropertyName = "location"
        dgvClocation.HeaderText = "Location"
        dgvClocation.Name = "dgvClocation"
        dgvClocation.ReadOnly = True
        dgvClocation.Width = 78
        ' 
        ' totalsize
        ' 
        totalsize.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        totalsize.DataPropertyName = "totalsize"
        totalsize.HeaderText = "Total size"
        totalsize.Name = "totalsize"
        totalsize.ReadOnly = True
        totalsize.Width = 80
        ' 
        ' filecount
        ' 
        filecount.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        filecount.DataPropertyName = "filecount"
        filecount.HeaderText = "Filecount"
        filecount.Name = "filecount"
        filecount.ReadOnly = True
        filecount.Width = 81
        ' 
        ' FlowLayoutPanel1
        ' 
        FlowLayoutPanel1.AutoScroll = True
        FlowLayoutPanel1.Dock = DockStyle.Fill
        FlowLayoutPanel1.Location = New Point(0, 118)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New Size(1290, 493)
        FlowLayoutPanel1.TabIndex = 6
        ' 
        ' pDebug
        ' 
        pDebug.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        pDebug.Controls.Add(rtbLog)
        pDebug.Controls.Add(bDebug)
        pDebug.Controls.Add(DataGridView1)
        pDebug.Location = New Point(0, -521)
        pDebug.Margin = New Padding(0)
        pDebug.Name = "pDebug"
        pDebug.Size = New Size(1290, 534)
        pDebug.TabIndex = 7
        ' 
        ' rtbLog
        ' 
        rtbLog.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        rtbLog.BackColor = Color.FromArgb(56, 56, 56)
        rtbLog.BorderStyle = BorderStyle.None
        rtbLog.Font = New Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        rtbLog.ForeColor = Color.LimeGreen
        rtbLog.Location = New Point(3, 418)
        rtbLog.Name = "rtbLog"
        rtbLog.Size = New Size(1284, 96)
        rtbLog.TabIndex = 5
        rtbLog.Text = ""
        ' 
        ' bDebug
        ' 
        bDebug.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        bDebug.BackColor = Color.FromArgb(76, 76, 76)
        bDebug.BackgroundImage = My.Resources.Resources.remove
        bDebug.FlatAppearance.BorderSize = 0
        bDebug.FlatStyle = FlatStyle.Flat
        bDebug.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        bDebug.ForeColor = Color.WhiteSmoke
        bDebug.Location = New Point(455, 519)
        bDebug.Margin = New Padding(0)
        bDebug.Name = "bDebug"
        bDebug.Size = New Size(360, 13)
        bDebug.TabIndex = 0
        bDebug.TextAlign = ContentAlignment.TopCenter
        bDebug.UseVisualStyleBackColor = False
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(36, 36, 36)
        ClientSize = New Size(1290, 611)
        Controls.Add(pDebug)
        Controls.Add(FlowLayoutPanel1)
        Controls.Add(GroupBox1)
        DoubleBuffered = True
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Megascans Library Index"
        WindowState = FormWindowState.Maximized
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        TableLayoutPanel1.ResumeLayout(False)
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        pDebug.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents clbTags As CheckedListBox
    Friend WithEvents bTagReload As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cbTags As ComboBox
    Friend WithEvents bAddTag As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents bSearch As Button
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents cblPacks As CheckedListBox
    Friend WithEvents bAddPack As Button
    Friend WithEvents cbPacks As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents bFirst As Button
    Friend WithEvents bPrev As Button
    Friend WithEvents cbPages As ComboBox
    Friend WithEvents bNext As Button
    Friend WithEvents bLast As Button
    Friend WithEvents pDebug As Panel
    Friend WithEvents bDebug As Button
    Friend WithEvents clbResolution As CheckedListBox
    Friend WithEvents clbCats As CheckedListBox
    Friend WithEvents Label3 As Label
    Friend WithEvents tSearch As TextBox
    Friend WithEvents rtbLog As RichTextBox
    Friend WithEvents sid As TextBox
    Friend WithEvents megascan_id As DataGridViewTextBoxColumn
    Friend WithEvents id As DataGridViewTextBoxColumn
    Friend WithEvents description As DataGridViewTextBoxColumn
    Friend WithEvents previewimage As DataGridViewTextBoxColumn
    Friend WithEvents packname As DataGridViewTextBoxColumn
    Friend WithEvents assettags As DataGridViewTextBoxColumn
    Friend WithEvents resolution As DataGridViewTextBoxColumn
    Friend WithEvents category As DataGridViewTextBoxColumn
    Friend WithEvents dgvClocation As DataGridViewTextBoxColumn
    Friend WithEvents totalsize As DataGridViewTextBoxColumn
    Friend WithEvents filecount As DataGridViewTextBoxColumn
    Friend WithEvents cboSearchByIdOption As ComboBox

End Class
