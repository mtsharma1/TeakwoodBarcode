Imports System.Windows.Forms
Imports System.Drawing

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle()
        shell = New TableLayoutPanel()
        header = New TableLayoutPanel()
        heading = New Label()
        btnLabelDetails = New Button()
        fileBar = New TableLayoutPanel()
        upload = New Button()
        sourceLabel = New Label()
        fileNote = New Label()
        lblWorksheet = New Label()
        sheetCombo = New ComboBox()
        ClearData = New Button()
        content = New TableLayoutPanel()
        dataArea = New TableLayoutPanel()
        summary = New Label()
        tabs = New TabControl()
        printTab = New TabPage()
        grid = New DataGridView()
        rawTab = New TabPage()
        rawGrid = New DataGridView()
        actions = New FlowLayoutPanel()
        btnTickAll = New Button()
        btnClearTicks = New Button()
        lblQuantity = New Label()
        quantity = New NumericUpDown()
        btnApplyQuantity = New Button()
        previewArea = New TableLayoutPanel()
        lblPreview = New Label()
        canvas = New LabelCanvas()
        dimensions = New TableLayoutPanel()
        lblSize = New Label()
        sizeCombo = New ComboBox()
        lblWidth = New Label()
        labelWidth = New NumericUpDown()
        lblHeight = New Label()
        labelHeight = New NumericUpDown()
        lblLayout = New Label()
        layoutCombo = New ComboBox()
        printers = New TableLayoutPanel()
        lblPrinter = New Label()
        printerCombo = New ComboBox()
        btnRefreshPrinters = New Button()
        commands = New TableLayoutPanel()
        btnPreviewBatch = New Button()
        btnExportPdf = New Button()
        btnTestPrint = New Button()
        btnPrintBatch = New Button()
        lblPrintHelp = New Label()
        status = New Label()
        uploadDialog = New OpenFileDialog()
        pdfDialog = New SaveFileDialog()
        shell.SuspendLayout()
        header.SuspendLayout()
        fileBar.SuspendLayout()
        content.SuspendLayout()
        dataArea.SuspendLayout()
        tabs.SuspendLayout()
        printTab.SuspendLayout()
        CType(grid, System.ComponentModel.ISupportInitialize).BeginInit()
        rawTab.SuspendLayout()
        CType(rawGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        actions.SuspendLayout()
        CType(quantity, System.ComponentModel.ISupportInitialize).BeginInit()
        previewArea.SuspendLayout()
        dimensions.SuspendLayout()
        CType(labelWidth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(labelHeight, System.ComponentModel.ISupportInitialize).BeginInit()
        printers.SuspendLayout()
        commands.SuspendLayout()
        SuspendLayout()
        ' 
        ' shell
        ' 
        shell.ColumnCount = 1
        shell.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 20F))
        shell.Controls.Add(header, 0, 0)
        shell.Controls.Add(fileBar, 0, 1)
        shell.Controls.Add(content, 0, 2)
        shell.Controls.Add(status, 0, 3)
        shell.Dock = DockStyle.Fill
        shell.Location = New Point(0, 0)
        shell.Name = "shell"
        shell.Padding = New Padding(22, 16, 22, 12)
        shell.RowCount = 4
        shell.RowStyles.Add(New RowStyle(SizeType.Absolute, 66F))
        shell.RowStyles.Add(New RowStyle(SizeType.Absolute, 70F))
        shell.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        shell.RowStyles.Add(New RowStyle(SizeType.Absolute, 45F))
        shell.Size = New Size(1464, 921)
        shell.TabIndex = 0
        ' 
        ' header
        ' 
        header.ColumnCount = 2
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 185F))
        header.Controls.Add(heading, 0, 0)
        header.Controls.Add(btnLabelDetails, 1, 0)
        header.Dock = DockStyle.Fill
        header.Location = New Point(25, 19)
        header.Name = "header"
        header.RowCount = 1
        header.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
        header.Size = New Size(1414, 60)
        header.TabIndex = 0
        ' 
        ' heading
        ' 
        heading.AutoSize = True
        heading.Dock = DockStyle.Fill
        heading.Font = New Font("Segoe UI", 21F, FontStyle.Bold)
        heading.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        heading.Location = New Point(3, 0)
        heading.Name = "heading"
        heading.Size = New Size(1223, 60)
        heading.TabIndex = 0
        heading.Text = "TEAKWOOD  /  Label studio"
        heading.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' btnLabelDetails
        ' 
        btnLabelDetails.BackColor = Color.White
        btnLabelDetails.Cursor = Cursors.Hand
        btnLabelDetails.Dock = DockStyle.Fill
        btnLabelDetails.FlatStyle = FlatStyle.Flat
        btnLabelDetails.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnLabelDetails.Location = New Point(1232, 3)
        btnLabelDetails.MinimumSize = New Size(0, 32)
        btnLabelDetails.Name = "btnLabelDetails"
        btnLabelDetails.Size = New Size(179, 54)
        btnLabelDetails.TabIndex = 1
        btnLabelDetails.Text = "Shared label details"
        btnLabelDetails.UseVisualStyleBackColor = False
        ' 
        ' fileBar
        ' 
        fileBar.ColumnCount = 4
        fileBar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 180F))
        fileBar.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        fileBar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 215F))
        fileBar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112F))
        fileBar.Controls.Add(upload, 0, 0)
        fileBar.Controls.Add(sourceLabel, 1, 0)
        fileBar.Controls.Add(fileNote, 1, 1)
        fileBar.Controls.Add(lblWorksheet, 2, 0)
        fileBar.Controls.Add(sheetCombo, 2, 1)
        fileBar.Controls.Add(ClearData, 3, 0)
        fileBar.Dock = DockStyle.Fill
        fileBar.Location = New Point(25, 85)
        fileBar.Name = "fileBar"
        fileBar.RowCount = 2
        fileBar.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        fileBar.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        fileBar.Size = New Size(1414, 64)
        fileBar.TabIndex = 1
        ' 
        ' upload
        ' 
        upload.BackColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        upload.Cursor = Cursors.Hand
        upload.Dock = DockStyle.Fill
        upload.FlatStyle = FlatStyle.Flat
        upload.ForeColor = Color.White
        upload.Location = New Point(3, 3)
        upload.MinimumSize = New Size(0, 32)
        upload.Name = "upload"
        fileBar.SetRowSpan(upload, 2)
        upload.Size = New Size(174, 58)
        upload.TabIndex = 0
        upload.Text = "Upload Excel / CSV"
        upload.UseVisualStyleBackColor = False
        ' 
        ' sourceLabel
        ' 
        sourceLabel.AutoEllipsis = True
        sourceLabel.AutoSize = True
        sourceLabel.Dock = DockStyle.Fill
        sourceLabel.Location = New Point(183, 0)
        sourceLabel.Name = "sourceLabel"
        sourceLabel.Padding = New Padding(12, 7, 5, 0)
        sourceLabel.Size = New Size(901, 32)
        sourceLabel.TabIndex = 1
        sourceLabel.Text = "No file uploaded"
        ' 
        ' fileNote
        ' 
        fileNote.AutoSize = True
        fileNote.Dock = DockStyle.Fill
        fileNote.ForeColor = Color.DimGray
        fileNote.Location = New Point(183, 32)
        fileNote.Name = "fileNote"
        fileNote.Padding = New Padding(12, 0, 0, 0)
        fileNote.Size = New Size(901, 32)
        fileNote.TabIndex = 2
        fileNote.Text = ".xlsx, .xlsm and .csv • Files remain unchanged"
        ' 
        ' lblWorksheet
        ' 
        lblWorksheet.AutoSize = True
        lblWorksheet.Location = New Point(1090, 0)
        lblWorksheet.Name = "lblWorksheet"
        lblWorksheet.Size = New Size(84, 21)
        lblWorksheet.TabIndex = 3
        lblWorksheet.Text = "Worksheet"
        ' 
        ' sheetCombo
        ' 
        sheetCombo.Dock = DockStyle.Fill
        sheetCombo.DropDownStyle = ComboBoxStyle.DropDownList
        sheetCombo.FormattingEnabled = True
        sheetCombo.Location = New Point(1090, 35)
        sheetCombo.Name = "sheetCombo"
        sheetCombo.Size = New Size(209, 29)
        sheetCombo.TabIndex = 4
        ' 
        ' ClearData
        ' 
        ClearData.BackColor = Color.White
        ClearData.Cursor = Cursors.Hand
        ClearData.Dock = DockStyle.Fill
        ClearData.FlatStyle = FlatStyle.Flat
        ClearData.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        ClearData.Location = New Point(1305, 3)
        ClearData.MinimumSize = New Size(0, 32)
        ClearData.Name = "ClearData"
        fileBar.SetRowSpan(ClearData, 2)
        ClearData.Size = New Size(106, 58)
        ClearData.TabIndex = 5
        ClearData.Text = "Clear data"
        ClearData.UseVisualStyleBackColor = False
        ' 
        ' content
        ' 
        content.ColumnCount = 2
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 440F))
        content.Controls.Add(dataArea, 0, 0)
        content.Controls.Add(previewArea, 1, 0)
        content.Dock = DockStyle.Fill
        content.Location = New Point(25, 155)
        content.Name = "content"
        content.RowCount = 1
        content.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
        content.Size = New Size(1414, 706)
        content.TabIndex = 2
        ' 
        ' dataArea
        ' 
        dataArea.ColumnCount = 1
        dataArea.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 20F))
        dataArea.Controls.Add(summary, 0, 0)
        dataArea.Controls.Add(tabs, 0, 1)
        dataArea.Controls.Add(actions, 0, 2)
        dataArea.Dock = DockStyle.Fill
        dataArea.Location = New Point(0, 10)
        dataArea.Margin = New Padding(0, 10, 18, 0)
        dataArea.Name = "dataArea"
        dataArea.RowCount = 3
        dataArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 38F))
        dataArea.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        dataArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 85F))
        dataArea.Size = New Size(956, 696)
        dataArea.TabIndex = 0
        ' 
        ' summary
        ' 
        summary.AutoSize = True
        summary.Dock = DockStyle.Fill
        summary.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        summary.Location = New Point(3, 0)
        summary.Name = "summary"
        summary.Size = New Size(950, 38)
        summary.TabIndex = 0
        summary.Text = "Upload a file to get started"
        ' 
        ' tabs
        ' 
        tabs.Controls.Add(printTab)
        tabs.Controls.Add(rawTab)
        tabs.Dock = DockStyle.Fill
        tabs.Location = New Point(3, 41)
        tabs.Name = "tabs"
        tabs.SelectedIndex = 0
        tabs.Size = New Size(950, 567)
        tabs.TabIndex = 1
        ' 
        ' printTab
        ' 
        printTab.Controls.Add(grid)
        printTab.Location = New Point(4, 30)
        printTab.Name = "printTab"
        printTab.Size = New Size(942, 533)
        printTab.TabIndex = 0
        printTab.Text = "Print items"
        printTab.UseVisualStyleBackColor = True
        ' 
        ' grid
        ' 
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(246), CByte(248), CByte(246))
        grid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        grid.BackgroundColor = Color.White
        grid.BorderStyle = BorderStyle.None
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        DataGridViewCellStyle2.ForeColor = Color.White
        grid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        grid.ColumnHeadersHeight = 45
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = SystemColors.Window
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9.5F)
        DataGridViewCellStyle3.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(CByte(215), CByte(233), CByte(223))
        DataGridViewCellStyle3.SelectionForeColor = Color.Black
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.False
        grid.DefaultCellStyle = DataGridViewCellStyle3
        grid.Dock = DockStyle.Fill
        grid.EnableHeadersVisualStyles = False
        grid.GridColor = Color.FromArgb(CByte(231), CByte(234), CByte(231))
        grid.Location = New Point(0, 0)
        grid.Name = "grid"
        grid.RowHeadersVisible = False
        grid.RowHeadersWidth = 51
        grid.RowTemplate.Height = 31
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        grid.Size = New Size(942, 533)
        grid.TabIndex = 0
        ' 
        ' rawTab
        ' 
        rawTab.Controls.Add(rawGrid)
        rawTab.Location = New Point(4, 29)
        rawTab.Name = "rawTab"
        rawTab.Size = New Size(942, 534)
        rawTab.TabIndex = 1
        rawTab.Text = "Uploaded worksheet"
        rawTab.UseVisualStyleBackColor = True
        ' 
        ' rawGrid
        ' 
        rawGrid.AllowUserToAddRows = False
        rawGrid.AllowUserToDeleteRows = False
        rawGrid.AllowUserToResizeRows = False
        DataGridViewCellStyle4.BackColor = Color.FromArgb(CByte(246), CByte(248), CByte(246))
        rawGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle4
        rawGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        rawGrid.BackgroundColor = Color.White
        rawGrid.BorderStyle = BorderStyle.None
        rawGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        DataGridViewCellStyle5.BackColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        DataGridViewCellStyle5.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        DataGridViewCellStyle5.ForeColor = Color.White
        rawGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle5
        rawGrid.ColumnHeadersHeight = 45
        DataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = SystemColors.Window
        DataGridViewCellStyle6.Font = New Font("Segoe UI", 9.5F)
        DataGridViewCellStyle6.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(CByte(215), CByte(233), CByte(223))
        DataGridViewCellStyle6.SelectionForeColor = Color.Black
        DataGridViewCellStyle6.WrapMode = DataGridViewTriState.False
        rawGrid.DefaultCellStyle = DataGridViewCellStyle6
        rawGrid.Dock = DockStyle.Fill
        rawGrid.EnableHeadersVisualStyles = False
        rawGrid.GridColor = Color.FromArgb(CByte(231), CByte(234), CByte(231))
        rawGrid.Location = New Point(0, 0)
        rawGrid.Name = "rawGrid"
        rawGrid.ReadOnly = True
        rawGrid.RowHeadersVisible = False
        rawGrid.RowHeadersWidth = 51
        rawGrid.RowTemplate.Height = 31
        rawGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        rawGrid.Size = New Size(942, 534)
        rawGrid.TabIndex = 0
        ' 
        ' actions
        ' 
        actions.Controls.Add(btnTickAll)
        actions.Controls.Add(btnClearTicks)
        actions.Controls.Add(lblQuantity)
        actions.Controls.Add(quantity)
        actions.Controls.Add(btnApplyQuantity)
        actions.Dock = DockStyle.Fill
        actions.Location = New Point(3, 614)
        actions.Name = "actions"
        actions.Padding = New Padding(0, 9, 0, 0)
        actions.Size = New Size(950, 79)
        actions.TabIndex = 2
        ' 
        ' btnTickAll
        ' 
        btnTickAll.AutoSize = True
        btnTickAll.BackColor = Color.White
        btnTickAll.Cursor = Cursors.Hand
        btnTickAll.FlatStyle = FlatStyle.Flat
        btnTickAll.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnTickAll.Location = New Point(3, 12)
        btnTickAll.MinimumSize = New Size(0, 32)
        btnTickAll.Name = "btnTickAll"
        btnTickAll.Size = New Size(145, 34)
        btnTickAll.TabIndex = 0
        btnTickAll.Text = "Tick all"
        btnTickAll.UseVisualStyleBackColor = False
        ' 
        ' btnClearTicks
        ' 
        btnClearTicks.AutoSize = True
        btnClearTicks.BackColor = Color.White
        btnClearTicks.Cursor = Cursors.Hand
        btnClearTicks.FlatStyle = FlatStyle.Flat
        btnClearTicks.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnClearTicks.Location = New Point(154, 12)
        btnClearTicks.MinimumSize = New Size(0, 32)
        btnClearTicks.Name = "btnClearTicks"
        btnClearTicks.Size = New Size(145, 34)
        btnClearTicks.TabIndex = 1
        btnClearTicks.Text = "Clear ticks"
        btnClearTicks.UseVisualStyleBackColor = False
        ' 
        ' lblQuantity
        ' 
        lblQuantity.AutoSize = True
        lblQuantity.Location = New Point(314, 19)
        lblQuantity.Margin = New Padding(12, 10, 0, 0)
        lblQuantity.Name = "lblQuantity"
        lblQuantity.Size = New Size(70, 21)
        lblQuantity.TabIndex = 2
        lblQuantity.Text = "Quantity"
        ' 
        ' quantity
        ' 
        quantity.Location = New Point(390, 16)
        quantity.Margin = New Padding(6, 7, 6, 0)
        quantity.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        quantity.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        quantity.Name = "quantity"
        quantity.Size = New Size(75, 29)
        quantity.TabIndex = 3
        quantity.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' btnApplyQuantity
        ' 
        btnApplyQuantity.AutoSize = True
        btnApplyQuantity.BackColor = Color.White
        btnApplyQuantity.Cursor = Cursors.Hand
        btnApplyQuantity.FlatStyle = FlatStyle.Flat
        btnApplyQuantity.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnApplyQuantity.Location = New Point(474, 12)
        btnApplyQuantity.MinimumSize = New Size(0, 32)
        btnApplyQuantity.Name = "btnApplyQuantity"
        btnApplyQuantity.Size = New Size(151, 34)
        btnApplyQuantity.TabIndex = 4
        btnApplyQuantity.Text = "Apply qty to ticked"
        btnApplyQuantity.UseVisualStyleBackColor = False
        ' 
        ' previewArea
        ' 
        previewArea.ColumnCount = 1
        previewArea.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 20F))
        previewArea.Controls.Add(lblPreview, 0, 0)
        previewArea.Controls.Add(canvas, 0, 1)
        previewArea.Controls.Add(dimensions, 0, 2)
        previewArea.Controls.Add(printers, 0, 3)
        previewArea.Controls.Add(commands, 0, 4)
        previewArea.Controls.Add(lblPrintHelp, 0, 5)
        previewArea.Dock = DockStyle.Fill
        previewArea.Location = New Point(974, 10)
        previewArea.Margin = New Padding(0, 10, 0, 0)
        previewArea.Name = "previewArea"
        previewArea.RowCount = 6
        previewArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 34F))
        previewArea.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        previewArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 130F))
        previewArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 75F))
        previewArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 90F))
        previewArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 76F))
        previewArea.Size = New Size(440, 696)
        previewArea.TabIndex = 1
        ' 
        ' lblPreview
        ' 
        lblPreview.AutoSize = True
        lblPreview.Dock = DockStyle.Fill
        lblPreview.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        lblPreview.Location = New Point(3, 0)
        lblPreview.Name = "lblPreview"
        lblPreview.Size = New Size(434, 34)
        lblPreview.TabIndex = 0
        lblPreview.Text = "Selected row preview"
        ' 
        ' canvas
        ' 
        canvas.BackColor = Color.FromArgb(CByte(229), CByte(234), CByte(231))
        canvas.Dock = DockStyle.Fill
        canvas.Location = New Point(3, 37)
        canvas.Name = "canvas"
        canvas.Size = New Size(434, 285)
        canvas.TabIndex = 1
        ' 
        ' dimensions
        ' 
        dimensions.ColumnCount = 4
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 70F))
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 68F))
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        dimensions.Controls.Add(lblSize, 0, 0)
        dimensions.Controls.Add(sizeCombo, 1, 0)
        dimensions.Controls.Add(lblWidth, 0, 1)
        dimensions.Controls.Add(labelWidth, 1, 1)
        dimensions.Controls.Add(lblHeight, 2, 1)
        dimensions.Controls.Add(labelHeight, 3, 1)
        dimensions.Controls.Add(lblLayout, 0, 2)
        dimensions.Controls.Add(layoutCombo, 1, 2)
        dimensions.Dock = DockStyle.Fill
        dimensions.Location = New Point(3, 328)
        dimensions.Name = "dimensions"
        dimensions.Padding = New Padding(0, 10, 0, 0)
        dimensions.RowCount = 3
        dimensions.RowStyles.Add(New RowStyle(SizeType.Percent, 34F))
        dimensions.RowStyles.Add(New RowStyle(SizeType.Percent, 33F))
        dimensions.RowStyles.Add(New RowStyle(SizeType.Percent, 33F))
        dimensions.Size = New Size(434, 124)
        dimensions.TabIndex = 2
        ' 
        ' lblSize
        ' 
        lblSize.AutoSize = True
        lblSize.Location = New Point(3, 10)
        lblSize.Name = "lblSize"
        lblSize.Size = New Size(38, 21)
        lblSize.TabIndex = 0
        lblSize.Text = "Size"
        ' 
        ' sizeCombo
        ' 
        dimensions.SetColumnSpan(sizeCombo, 3)
        sizeCombo.Dock = DockStyle.Fill
        sizeCombo.DropDownStyle = ComboBoxStyle.DropDownList
        sizeCombo.FormattingEnabled = True
        sizeCombo.Items.AddRange(New Object() {"75 × 50 mm", "100 × 50 mm", "100 × 75 mm", "50 × 30 mm", "40 × 25 mm", "Custom"})
        sizeCombo.Location = New Point(73, 13)
        sizeCombo.Name = "sizeCombo"
        sizeCombo.Size = New Size(358, 29)
        sizeCombo.TabIndex = 1
        ' 
        ' lblWidth
        ' 
        lblWidth.AutoSize = True
        lblWidth.Location = New Point(3, 48)
        lblWidth.Name = "lblWidth"
        lblWidth.Size = New Size(48, 37)
        lblWidth.TabIndex = 2
        lblWidth.Text = "W (mm)"
        ' 
        ' labelWidth
        ' 
        labelWidth.DecimalPlaces = 1
        labelWidth.Dock = DockStyle.Fill
        labelWidth.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        labelWidth.Location = New Point(73, 51)
        labelWidth.Maximum = New Decimal(New Integer() {200, 0, 0, 0})
        labelWidth.Minimum = New Decimal(New Integer() {25, 0, 0, 0})
        labelWidth.Name = "labelWidth"
        labelWidth.Size = New Size(142, 29)
        labelWidth.TabIndex = 3
        labelWidth.Value = New Decimal(New Integer() {75, 0, 0, 0})
        ' 
        ' lblHeight
        ' 
        lblHeight.AutoSize = True
        lblHeight.Location = New Point(221, 48)
        lblHeight.Name = "lblHeight"
        lblHeight.Size = New Size(48, 37)
        lblHeight.TabIndex = 4
        lblHeight.Text = "H (mm)"
        ' 
        ' labelHeight
        ' 
        labelHeight.DecimalPlaces = 1
        labelHeight.Dock = DockStyle.Fill
        labelHeight.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        labelHeight.Location = New Point(289, 51)
        labelHeight.Maximum = New Decimal(New Integer() {200, 0, 0, 0})
        labelHeight.Minimum = New Decimal(New Integer() {15, 0, 0, 0})
        labelHeight.Name = "labelHeight"
        labelHeight.Size = New Size(142, 29)
        labelHeight.TabIndex = 5
        labelHeight.Value = New Decimal(New Integer() {50, 0, 0, 0})
        ' 
        ' lblLayout
        ' 
        lblLayout.AutoSize = True
        lblLayout.Location = New Point(3, 85)
        lblLayout.Name = "lblLayout"
        lblLayout.Size = New Size(57, 21)
        lblLayout.TabIndex = 6
        lblLayout.Text = "Layout"
        ' 
        ' layoutCombo
        ' 
        dimensions.SetColumnSpan(layoutCombo, 3)
        layoutCombo.Dock = DockStyle.Fill
        layoutCombo.DropDownStyle = ComboBoxStyle.DropDownList
        layoutCombo.FormattingEnabled = True
        layoutCombo.Items.AddRange(New Object() {"Full retail details", "Compact: barcode + price"})
        layoutCombo.Location = New Point(73, 88)
        layoutCombo.Name = "layoutCombo"
        layoutCombo.Size = New Size(358, 29)
        layoutCombo.TabIndex = 7
        ' 
        ' printers
        ' 
        printers.ColumnCount = 2
        printers.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        printers.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 90F))
        printers.Controls.Add(lblPrinter, 0, 0)
        printers.Controls.Add(printerCombo, 0, 1)
        printers.Controls.Add(btnRefreshPrinters, 1, 1)
        printers.Dock = DockStyle.Fill
        printers.Location = New Point(3, 458)
        printers.Name = "printers"
        printers.RowCount = 2
        printers.RowStyles.Add(New RowStyle(SizeType.Absolute, 25F))
        printers.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        printers.Size = New Size(434, 69)
        printers.TabIndex = 3
        ' 
        ' lblPrinter
        ' 
        lblPrinter.AutoSize = True
        lblPrinter.Location = New Point(3, 0)
        lblPrinter.Name = "lblPrinter"
        lblPrinter.Size = New Size(126, 21)
        lblPrinter.TabIndex = 0
        lblPrinter.Text = "Windows printer"
        ' 
        ' printerCombo
        ' 
        printerCombo.Dock = DockStyle.Fill
        printerCombo.DropDownStyle = ComboBoxStyle.DropDownList
        printerCombo.FormattingEnabled = True
        printerCombo.Location = New Point(3, 28)
        printerCombo.Name = "printerCombo"
        printerCombo.Size = New Size(338, 29)
        printerCombo.TabIndex = 1
        ' 
        ' btnRefreshPrinters
        ' 
        btnRefreshPrinters.BackColor = Color.White
        btnRefreshPrinters.Cursor = Cursors.Hand
        btnRefreshPrinters.Dock = DockStyle.Fill
        btnRefreshPrinters.FlatStyle = FlatStyle.Flat
        btnRefreshPrinters.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnRefreshPrinters.Location = New Point(347, 28)
        btnRefreshPrinters.MinimumSize = New Size(0, 32)
        btnRefreshPrinters.Name = "btnRefreshPrinters"
        btnRefreshPrinters.Size = New Size(84, 38)
        btnRefreshPrinters.TabIndex = 2
        btnRefreshPrinters.Text = "Refresh"
        btnRefreshPrinters.UseVisualStyleBackColor = False
        ' 
        ' commands
        ' 
        commands.ColumnCount = 2
        commands.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        commands.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        commands.Controls.Add(btnPreviewBatch, 0, 0)
        commands.Controls.Add(btnExportPdf, 1, 0)
        commands.Controls.Add(btnTestPrint, 0, 1)
        commands.Controls.Add(btnPrintBatch, 1, 1)
        commands.Dock = DockStyle.Fill
        commands.Location = New Point(3, 533)
        commands.Name = "commands"
        commands.RowCount = 2
        commands.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        commands.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        commands.Size = New Size(434, 84)
        commands.TabIndex = 4
        ' 
        ' btnPreviewBatch
        ' 
        btnPreviewBatch.BackColor = Color.White
        btnPreviewBatch.Cursor = Cursors.Hand
        btnPreviewBatch.Dock = DockStyle.Fill
        btnPreviewBatch.FlatStyle = FlatStyle.Flat
        btnPreviewBatch.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnPreviewBatch.Location = New Point(3, 3)
        btnPreviewBatch.MinimumSize = New Size(0, 32)
        btnPreviewBatch.Name = "btnPreviewBatch"
        btnPreviewBatch.Size = New Size(211, 36)
        btnPreviewBatch.TabIndex = 0
        btnPreviewBatch.Text = "Preview print batch"
        btnPreviewBatch.UseVisualStyleBackColor = False
        ' 
        ' btnExportPdf
        ' 
        btnExportPdf.BackColor = Color.White
        btnExportPdf.Cursor = Cursors.Hand
        btnExportPdf.Dock = DockStyle.Fill
        btnExportPdf.FlatStyle = FlatStyle.Flat
        btnExportPdf.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnExportPdf.Location = New Point(220, 3)
        btnExportPdf.MinimumSize = New Size(0, 32)
        btnExportPdf.Name = "btnExportPdf"
        btnExportPdf.Size = New Size(211, 36)
        btnExportPdf.TabIndex = 1
        btnExportPdf.Text = "Export batch PDF"
        btnExportPdf.UseVisualStyleBackColor = False
        ' 
        ' btnTestPrint
        ' 
        btnTestPrint.BackColor = Color.White
        btnTestPrint.Cursor = Cursors.Hand
        btnTestPrint.Dock = DockStyle.Fill
        btnTestPrint.FlatStyle = FlatStyle.Flat
        btnTestPrint.ForeColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnTestPrint.Location = New Point(3, 45)
        btnTestPrint.MinimumSize = New Size(0, 32)
        btnTestPrint.Name = "btnTestPrint"
        btnTestPrint.Size = New Size(211, 36)
        btnTestPrint.TabIndex = 2
        btnTestPrint.Text = "Print 1 test label"
        btnTestPrint.UseVisualStyleBackColor = False
        ' 
        ' btnPrintBatch
        ' 
        btnPrintBatch.BackColor = Color.FromArgb(CByte(34), CByte(67), CByte(54))
        btnPrintBatch.Cursor = Cursors.Hand
        btnPrintBatch.Dock = DockStyle.Fill
        btnPrintBatch.FlatStyle = FlatStyle.Flat
        btnPrintBatch.ForeColor = Color.White
        btnPrintBatch.Location = New Point(220, 45)
        btnPrintBatch.MinimumSize = New Size(0, 32)
        btnPrintBatch.Name = "btnPrintBatch"
        btnPrintBatch.Size = New Size(211, 36)
        btnPrintBatch.TabIndex = 3
        btnPrintBatch.Text = "Print batch"
        btnPrintBatch.UseVisualStyleBackColor = False
        ' 
        ' lblPrintHelp
        ' 
        lblPrintHelp.AutoSize = True
        lblPrintHelp.Dock = DockStyle.Fill
        lblPrintHelp.ForeColor = Color.FromArgb(CByte(117), CByte(82), CByte(26))
        lblPrintHelp.Location = New Point(3, 620)
        lblPrintHelp.Name = "lblPrintHelp"
        lblPrintHelp.Padding = New Padding(0, 8, 0, 0)
        lblPrintHelp.Size = New Size(434, 76)
        lblPrintHelp.TabIndex = 5
        lblPrintHelp.Text = "Set the same label size in the printer driver. Compact labels omit manufacturer details. Check one printed label before a full batch."
        ' 
        ' status
        ' 
        status.AutoSize = True
        status.Dock = DockStyle.Fill
        status.ForeColor = Color.DimGray
        status.Location = New Point(25, 864)
        status.Name = "status"
        status.Padding = New Padding(0, 8, 0, 0)
        status.Size = New Size(1414, 45)
        status.TabIndex = 3
        status.Text = "Barcodes are preserved as text. Excel does not need to be installed."
        ' 
        ' uploadDialog
        ' 
        uploadDialog.Filter = "Excel / CSV (*.xlsx;*.xlsm;*.csv)|*.xlsx;*.xlsm;*.csv|All files (*.*)|*.*"
        uploadDialog.Title = "Upload Excel or CSV data"
        ' 
        ' pdfDialog
        ' 
        pdfDialog.FileName = "Teakwood_Labels.pdf"
        pdfDialog.Filter = "PDF (*.pdf)|*.pdf"
        pdfDialog.Title = "Export barcode labels"
        ' 
        ' MainForm
        ' 
        AutoScaleDimensions = New SizeF(120F, 120F)
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = Color.FromArgb(CByte(247), CByte(248), CByte(246))
        ClientSize = New Size(1464, 921)
        Controls.Add(shell)
        Font = New Font("Segoe UI", 9.5F)
        MinimumSize = New Size(1150, 820)
        Name = "MainForm"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Teakwood Labels"
        shell.ResumeLayout(False)
        shell.PerformLayout()
        header.ResumeLayout(False)
        header.PerformLayout()
        fileBar.ResumeLayout(False)
        fileBar.PerformLayout()
        content.ResumeLayout(False)
        dataArea.ResumeLayout(False)
        dataArea.PerformLayout()
        tabs.ResumeLayout(False)
        printTab.ResumeLayout(False)
        CType(grid, System.ComponentModel.ISupportInitialize).EndInit()
        rawTab.ResumeLayout(False)
        CType(rawGrid, System.ComponentModel.ISupportInitialize).EndInit()
        actions.ResumeLayout(False)
        actions.PerformLayout()
        CType(quantity, System.ComponentModel.ISupportInitialize).EndInit()
        previewArea.ResumeLayout(False)
        previewArea.PerformLayout()
        dimensions.ResumeLayout(False)
        dimensions.PerformLayout()
        CType(labelWidth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(labelHeight, System.ComponentModel.ISupportInitialize).EndInit()
        printers.ResumeLayout(False)
        printers.PerformLayout()
        commands.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents shell As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents header As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents heading As System.Windows.Forms.Label
    Friend WithEvents btnLabelDetails As System.Windows.Forms.Button
    Friend WithEvents fileBar As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents upload As System.Windows.Forms.Button
    Friend WithEvents ClearData As System.Windows.Forms.Button
    Friend WithEvents sourceLabel As System.Windows.Forms.Label
    Friend WithEvents fileNote As System.Windows.Forms.Label
    Friend WithEvents lblWorksheet As System.Windows.Forms.Label
    Friend WithEvents sheetCombo As System.Windows.Forms.ComboBox
    Friend WithEvents content As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents dataArea As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents summary As System.Windows.Forms.Label
    Friend WithEvents tabs As System.Windows.Forms.TabControl
    Friend WithEvents printTab As System.Windows.Forms.TabPage
    Friend WithEvents rawTab As System.Windows.Forms.TabPage
    Friend WithEvents grid As System.Windows.Forms.DataGridView
    Friend WithEvents rawGrid As System.Windows.Forms.DataGridView
    Friend WithEvents colPrint As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents colQuantity As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colRecord As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colBarcode As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colStyleCode As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colProduct As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colSize As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colColour As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colMRP As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colVendorSku As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents actions As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnTickAll As System.Windows.Forms.Button
    Friend WithEvents btnClearTicks As System.Windows.Forms.Button
    Friend WithEvents lblQuantity As System.Windows.Forms.Label
    Friend WithEvents quantity As System.Windows.Forms.NumericUpDown
    Friend WithEvents btnApplyQuantity As System.Windows.Forms.Button
    Friend WithEvents previewArea As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblPreview As System.Windows.Forms.Label
    Friend WithEvents canvas As Global.Teakwood.Barcode.LabelCanvas
    Friend WithEvents dimensions As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblSize As System.Windows.Forms.Label
    Friend WithEvents sizeCombo As System.Windows.Forms.ComboBox
    Friend WithEvents lblWidth As System.Windows.Forms.Label
    Friend WithEvents lblHeight As System.Windows.Forms.Label
    Friend WithEvents labelWidth As System.Windows.Forms.NumericUpDown
    Friend WithEvents labelHeight As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblLayout As System.Windows.Forms.Label
    Friend WithEvents layoutCombo As System.Windows.Forms.ComboBox
    Friend WithEvents printers As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblPrinter As System.Windows.Forms.Label
    Friend WithEvents printerCombo As System.Windows.Forms.ComboBox
    Friend WithEvents btnRefreshPrinters As System.Windows.Forms.Button
    Friend WithEvents commands As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnPreviewBatch As System.Windows.Forms.Button
    Friend WithEvents btnExportPdf As System.Windows.Forms.Button
    Friend WithEvents btnTestPrint As System.Windows.Forms.Button
    Friend WithEvents btnPrintBatch As System.Windows.Forms.Button
    Friend WithEvents lblPrintHelp As System.Windows.Forms.Label
    Friend WithEvents status As System.Windows.Forms.Label
    Friend WithEvents uploadDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents pdfDialog As System.Windows.Forms.SaveFileDialog
End Class
