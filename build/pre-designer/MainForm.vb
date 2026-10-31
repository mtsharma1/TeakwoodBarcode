Imports System.ComponentModel
Imports System.Data
Imports System.Data.Common
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.IO
Imports System.Windows.Forms

Public Class MainForm
    Inherits Form
    Private ReadOnly settings As LabelSettings = LabelSettings.Load()
    Private items As New BindingList(Of ItemRow)
    Private sheets As New List(Of ImportedSheet)
    Private ReadOnly grid As New DataGridView
    Private ReadOnly rawGrid As New DataGridView
    Private ReadOnly sourceLabel As New Label
    Private ReadOnly status As New Label
    Private ReadOnly summary As New Label
    Private ReadOnly sheetCombo As New ComboBox
    Private ReadOnly printerCombo As New ComboBox
    Private ReadOnly sizeCombo As New ComboBox
    Private ReadOnly layoutCombo As New ComboBox
    Private ReadOnly labelWidth As New NumericUpDown
    Private ReadOnly labelHeight As New NumericUpDown
    Private ReadOnly quantity As New NumericUpDown
    Private ReadOnly canvas As New LabelCanvas
    Private ReadOnly upload As Button
    Private updating As Boolean
    Private ReadOnly green As Color = Color.FromArgb(34, 67, 54)

    Public Sub New(Optional initialFile As String = Nothing)
        Text = "Teakwood Labels"
        Size = New Size(1480, 960)
        MinimumSize = New Size(1150, 820)
        StartPosition = FormStartPosition.CenterScreen
        Font = New Font("Segoe UI", 9.5F)
        BackColor = Color.FromArgb(247, 248, 246)
        AutoScaleMode = AutoScaleMode.Dpi
        Dim shell As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 4, .Padding = New Padding(22, 16, 22, 12)}
        shell.RowStyles.Add(New RowStyle(SizeType.Absolute, 66))
        shell.RowStyles.Add(New RowStyle(SizeType.Absolute, 70))
        shell.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        shell.RowStyles.Add(New RowStyle(SizeType.Absolute, 45))
        Controls.Add(shell)
        Dim header As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2}
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 185))
        Dim heading As New Label With {.Text = "TEAKWOOD  /  Label studio", .Font = New Font("Segoe UI", 21, FontStyle.Bold), .ForeColor = green, .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleLeft}
        Dim details = MakeButton("Shared label details", False)
        header.Controls.Add(heading, 0, 0) : header.Controls.Add(details, 1, 0)
        shell.Controls.Add(header, 0, 0)
        Dim fileBar As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 4, .RowCount = 2}
        fileBar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 180))
        fileBar.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        fileBar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 215))
        upload = MakeButton("Upload Excel / CSV", True)
        Dim ClearData = MakeButton("Clear DataAdapter", True)


        fileBar.Controls.Add(upload, 0, 0) : fileBar.SetRowSpan(upload, 2)
        sourceLabel.Text = "No file uploaded"
        sourceLabel.AutoEllipsis = True : sourceLabel.Dock = DockStyle.Fill : sourceLabel.Padding = New Padding(12, 7, 5, 0)
        fileBar.Controls.Add(sourceLabel, 1, 0)




        Dim fileNote As New Label With {.Text = ".xlsx, .xlsm and .csv • Files are read locally and remain unchanged", .ForeColor = Color.DimGray, .Dock = DockStyle.Fill, .Padding = New Padding(12, 0, 0, 0)}
        fileBar.Controls.Add(fileNote, 1, 1)
        sheetCombo.DropDownStyle = ComboBoxStyle.DropDownList : sheetCombo.Dock = DockStyle.Fill
        fileBar.Controls.Add(New Label With {.Text = "Worksheet", .AutoSize = True}, 2, 0)
        fileBar.Controls.Add(sheetCombo, 2, 1)
        shell.Controls.Add(fileBar, 0, 1)
        Dim content As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2}
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 440))
        shell.Controls.Add(content, 0, 2)
        Dim dataArea As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 3, .Margin = New Padding(0, 10, 18, 0)}
        dataArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 38))
        dataArea.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        dataArea.RowStyles.Add(New RowStyle(SizeType.Absolute, 85))
        summary.Dock = DockStyle.Fill : summary.Text = "Upload a file to get started" : summary.Font = New Font(Font, FontStyle.Bold)
        dataArea.Controls.Add(summary, 0, 0)
        Dim tabs As New TabControl With {.Dock = DockStyle.Fill}
        Dim printTab As New TabPage("Print items")
        Dim rawTab As New TabPage("Uploaded worksheet")
        ConfigureGrid(grid) : ConfigureGrid(rawGrid)
        grid.AutoGenerateColumns = False
        grid.Columns.Add(New DataGridViewCheckBoxColumn With {.Name = "Print", .HeaderText = "Print", .DataPropertyName = "Print", .Width = 58})
        grid.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "Quantity", .HeaderText = "Qty", .DataPropertyName = "Quantity", .Width = 55})
        AddColumn("Record", "Row", 48)
        AddColumn("Barcode", "Barcode", 165)
        AddColumn("StyleCode", "Style code", 200)
        AddColumn("Product", "Product", 220)
        AddColumn("Size", "Size", 55)
        AddColumn("Colour", "Colour", 75)
        AddColumn("MRP", "MRP (Rs.)", 90)
        AddColumn("VendorSku", "Vendor SKU", 160)
        grid.Columns("MRP").DefaultCellStyle.Format = "0.00"
        grid.Columns("MRP").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        grid.Columns("Quantity").DefaultCellStyle.BackColor = Color.FromArgb(255, 244, 211)
        grid.Columns("Quantity").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        rawGrid.ReadOnly = True : rawGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        printTab.Controls.Add(grid) : rawTab.Controls.Add(rawGrid)
        tabs.TabPages.AddRange({printTab, rawTab}) : dataArea.Controls.Add(tabs, 0, 1)
        Dim actions As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(0, 9, 0, 0), .WrapContents = True}
        Dim selectAll = MakeButton("Tick all", False), selectNone = MakeButton("Clear ticks", False), applyQty = MakeButton("Apply qty to ticked", False)
        For Each b In {selectAll, selectNone, applyQty} : b.AutoSize = True : b.Height = 34 : b.Dock = DockStyle.None : Next
        quantity.Minimum = 1 : quantity.Maximum = 10000 : quantity.Value = 1 : quantity.Width = 75 : quantity.Margin = New Padding(6, 7, 6, 0)
        actions.Controls.AddRange({selectAll, selectNone, New Label With {.Text = "Quantity", .AutoSize = True, .Margin = New Padding(12, 10, 0, 0)}, quantity, applyQty})
        dataArea.Controls.Add(actions, 0, 2) : content.Controls.Add(dataArea, 0, 0)
        Dim right As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 6, .Margin = New Padding(0, 10, 0, 0)}
        right.RowStyles.Add(New RowStyle(SizeType.Absolute, 34))
        right.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        right.RowStyles.Add(New RowStyle(SizeType.Absolute, 130))
        right.RowStyles.Add(New RowStyle(SizeType.Absolute, 75))
        right.RowStyles.Add(New RowStyle(SizeType.Absolute, 90))
        right.RowStyles.Add(New RowStyle(SizeType.Absolute, 76))
        right.Controls.Add(New Label With {.Text = "Selected row preview", .Dock = DockStyle.Fill, .Font = New Font(Font, FontStyle.Bold)}, 0, 0)
        right.Controls.Add(canvas, 0, 1)
        Dim dimensions As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 4, .RowCount = 3, .Padding = New Padding(0, 10, 0, 0)}
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 70))
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 68))
        dimensions.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        sizeCombo.Items.AddRange({"75 × 50 mm", "100 × 50 mm", "100 × 75 mm", "50 × 30 mm", "40 × 25 mm", "Custom"})
        sizeCombo.DropDownStyle = ComboBoxStyle.DropDownList : sizeCombo.Dock = DockStyle.Fill
        dimensions.Controls.Add(New Label With {.Text = "Size", .AutoSize = True}, 0, 0) : dimensions.Controls.Add(sizeCombo, 1, 0) : dimensions.SetColumnSpan(sizeCombo, 3)
        For Each n In {labelWidth, labelHeight} : n.DecimalPlaces = 1 : n.Increment = 0.5D : n.Maximum = 200 : n.Dock = DockStyle.Fill : Next
        labelWidth.Minimum = 25 : labelHeight.Minimum = 15
        dimensions.Controls.Add(New Label With {.Text = "W (mm)", .AutoSize = True}, 0, 1) : dimensions.Controls.Add(labelWidth, 1, 1)
        dimensions.Controls.Add(New Label With {.Text = "H (mm)", .AutoSize = True}, 2, 1) : dimensions.Controls.Add(labelHeight, 3, 1)
        layoutCombo.Items.AddRange({"Full retail details", "Compact: barcode + price"})
        layoutCombo.DropDownStyle = ComboBoxStyle.DropDownList : layoutCombo.Dock = DockStyle.Fill
        dimensions.Controls.Add(New Label With {.Text = "Layout", .AutoSize = True}, 0, 2) : dimensions.Controls.Add(layoutCombo, 1, 2) : dimensions.SetColumnSpan(layoutCombo, 3)
        right.Controls.Add(dimensions, 0, 2)
        Dim printers As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 2}
        printers.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100)) : printers.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 90))
        printers.Controls.Add(New Label With {.Text = "Windows printer", .AutoSize = True}, 0, 0)
        printerCombo.DropDownStyle = ComboBoxStyle.DropDownList : printerCombo.Dock = DockStyle.Fill
        printers.Controls.Add(printerCombo, 0, 1)
        Dim refresh = MakeButton("Refresh", False) : printers.Controls.Add(refresh, 1, 1)
        right.Controls.Add(printers, 0, 3)
        Dim commands As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 2}
        commands.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50)) : commands.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        Dim preview = MakeButton("Preview print batch", False), pdf = MakeButton("Export batch PDF", False), test = MakeButton("Print 1 test label", False), print = MakeButton("Print batch", True)
        commands.Controls.Add(preview, 0, 0) : commands.Controls.Add(pdf, 1, 0) : commands.Controls.Add(test, 0, 1) : commands.Controls.Add(print, 1, 1)
        right.Controls.Add(commands, 0, 4)
        right.Controls.Add(New Label With {.Text = "Set the same label size in the printer driver. Compact labels omit manufacturer details. Check one printed label before a full batch.", .Dock = DockStyle.Fill, .ForeColor = Color.FromArgb(117, 82, 26), .Padding = New Padding(0, 8, 0, 0)}, 0, 5)
        content.Controls.Add(right, 1, 0)
        status.Dock = DockStyle.Fill : status.ForeColor = Color.DimGray : status.Padding = New Padding(0, 8, 0, 0)
        status.Text = "Barcodes are preserved as text. Excel does not need to be installed."
        shell.Controls.Add(status, 0, 3)
        updating = True
        labelWidth.Value = Math.Clamp(settings.WidthMm, 25D, 200D) : labelHeight.Value = Math.Clamp(settings.HeightMm, 15D, 200D)
        sizeCombo.SelectedIndex = 5 : layoutCombo.SelectedIndex = If(settings.Compact, 1, 0)
        If settings.WidthMm = 75 AndAlso settings.HeightMm = 50 Then sizeCombo.SelectedIndex = 0
        updating = False
        RefreshPrinters()
        AddHandler upload.Click, AddressOf UploadClick
        AddHandler sheetCombo.SelectedIndexChanged, AddressOf ChangeSheet
        AddHandler sizeCombo.SelectedIndexChanged, AddressOf ChangePreset
        AddHandler labelWidth.ValueChanged, AddressOf DimensionsChanged
        AddHandler labelHeight.ValueChanged, AddressOf DimensionsChanged
        AddHandler layoutCombo.SelectedIndexChanged, Sub() UpdatePreview()
        AddHandler grid.SelectionChanged, Sub() UpdatePreview()
        AddHandler grid.CurrentCellDirtyStateChanged, Sub() If grid.IsCurrentCellDirty Then grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
        AddHandler grid.CellValueChanged, Sub() UpdateSummary()
        AddHandler grid.CellValidating, AddressOf ValidateQuantity
        AddHandler grid.DataError,
            Sub(sender, e)
                e.ThrowException = False
                status.Text = "Quantity must be a whole number between 1 and 10,000."
            End Sub
        AddHandler selectAll.Click, Sub() SetTicks(True)
        AddHandler selectNone.Click, Sub() SetTicks(False)
        AddHandler applyQty.Click,
            Sub()
                If Not CommitGrid() Then Return
                For Each item In items.Where(Function(i) i.Print) : item.Quantity = CInt(quantity.Value) : Next
                grid.Refresh() : UpdateSummary()
            End Sub
        AddHandler preview.Click, Sub() RunAction(Sub() UsingPreview())
        AddHandler pdf.Click, Sub() RunAction(Sub() ExportPdf())
        AddHandler test.Click, Sub() RunAction(Sub() PrintLabels(True))
        AddHandler print.Click, Sub() RunAction(Sub() PrintLabels(False))
        AddHandler refresh.Click, Sub() RefreshPrinters()
        AddHandler details.Click,
            Sub()
                Using form As New DetailsForm(settings)
                    If form.ShowDialog(Me) = DialogResult.OK Then
                        RunAction(Sub() settings.Save()) : UpdatePreview()
                    End If
                End Using
            End Sub
        AddHandler FormClosing, Sub() RunAction(Sub() ReadSettings().Save())
        If Not String.IsNullOrWhiteSpace(initialFile) Then AddHandler Shown, Async Sub() Await ImportPath(initialFile)
    End Sub

    Private Function MakeButton(text As String, primary As Boolean) As Button
        Return New Button With {.Text = text, .Dock = DockStyle.Fill, .FlatStyle = FlatStyle.Flat, .BackColor = If(primary, green, Color.White), .ForeColor = If(primary, Color.White, green), .Cursor = Cursors.Hand, .Margin = New Padding(3), .MinimumSize = New Size(0, 32)}
    End Function
    Private Sub ConfigureGrid(view As DataGridView)
        view.Dock = DockStyle.Fill : view.BackgroundColor = Color.White : view.BorderStyle = BorderStyle.None
        view.AllowUserToAddRows = False : view.AllowUserToDeleteRows = False : view.AllowUserToResizeRows = False
        view.RowHeadersVisible = False : view.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        view.EnableHeadersVisualStyles = False : view.ColumnHeadersHeight = 45
        view.ColumnHeadersDefaultCellStyle.BackColor = green : view.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        view.ColumnHeadersDefaultCellStyle.Font = New Font(Font, FontStyle.Bold)
        view.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 233, 223)
        view.DefaultCellStyle.SelectionForeColor = Color.Black
        view.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 248, 246)
        view.RowTemplate.Height = 31 : view.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        view.GridColor = Color.FromArgb(231, 234, 231)
    End Sub
    Private Sub AddColumn(name As String, title As String, width As Integer)
        grid.Columns.Add(New DataGridViewTextBoxColumn With {.Name = name, .HeaderText = title, .DataPropertyName = name, .Width = width, .ReadOnly = True, .SortMode = DataGridViewColumnSortMode.NotSortable})
    End Sub
    Private Async Sub UploadClick(sender As Object, e As EventArgs)
        Using dialog As New OpenFileDialog With {.Title = "Upload Excel or CSV data", .Filter = "Excel / CSV (*.xlsx;*.xlsm;*.csv)|*.xlsx;*.xlsm;*.csv|All files (*.*)|*.*"}
            If dialog.ShowDialog(Me) = DialogResult.OK Then Await ImportPath(dialog.FileName)
        End Using
    End Sub
    Private Async Function ImportPath(path As String) As Task
        upload.Enabled = False : UseWaitCursor = True : status.Text = "Reading file…"
        Try
            Dim loaded = Await Task.Run(Function() ImportService.Read(path))
            sheets = loaded
            updating = True
            sheetCombo.Items.Clear()
            For Each sheet In sheets : sheetCombo.Items.Add(sheet.Name) : Next
            sourceLabel.Text = path
            Dim firstMatching = sheets.FindIndex(Function(s) s.Rows.Count > 0 AndAlso ItemMapper.Headers.All(Function(h) s.Rows(0).Any(Function(v) String.Equals(v.Trim(), h, StringComparison.OrdinalIgnoreCase))))
            sheetCombo.SelectedIndex = Math.Max(0, firstMatching)
            updating = False
            ChangeSheet(Me, EventArgs.Empty)
        Catch ex As Exception
            status.Text = "Upload failed. Previously loaded data is unchanged."
            MessageBox.Show(Me, ex.Message, "Upload file", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            updating = False : upload.Enabled = True : UseWaitCursor = False
        End Try
    End Function
    Private Sub ChangeSheet(sender As Object, e As EventArgs)
        If updating OrElse sheetCombo.SelectedIndex < 0 Then Return
        Dim sheet = sheets(sheetCombo.SelectedIndex)
        Dim raw As New DataTable
        Dim cols = If(sheet.Rows.Count = 0, 0, sheet.Rows.Max(Function(r) r.Length))
        For c = 0 To cols - 1
            Dim caption = If(sheet.Rows.Count > 0 AndAlso c < sheet.Rows(0).Length, sheet.Rows(0)(c), "Column " & (c + 1))
            raw.Columns.Add("C" & c, GetType(String)).Caption = caption
        Next
        For Each row In sheet.Rows.Skip(1)
            Dim r = raw.NewRow()
            For c = 0 To row.Length - 1 : r(c) = row(c) : Next
            raw.Rows.Add(r)
        Next
        rawGrid.DataSource = raw
        For c = 0 To rawGrid.Columns.Count - 1 : rawGrid.Columns(c).HeaderText = raw.Columns(c).Caption : Next
        Try
            items = ItemMapper.Map(sheet)
            status.Text = "File loaded. Tick items and edit quantities. Barcodes must be Text in the source; missing leading zeros cannot be inferred."
        Catch ex As Exception
            items = New BindingList(Of ItemRow)
            status.Text = "Worksheet cannot be printed: " & ex.Message
            MessageBox.Show(Me, ex.Message & vbCrLf & "You can still inspect it in Uploaded worksheet.", "Check worksheet", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
        grid.DataSource = items
        UpdateSummary() : UpdatePreview()
    End Sub
    Private Sub ChangePreset(sender As Object, e As EventArgs)
        If updating OrElse sizeCombo.SelectedIndex = 5 Then Return
        Dim presets As Decimal(,) = {{75, 50}, {100, 50}, {100, 75}, {50, 30}, {40, 25}}
        updating = True
        labelWidth.Value = presets(sizeCombo.SelectedIndex, 0) : labelHeight.Value = presets(sizeCombo.SelectedIndex, 1)
        layoutCombo.SelectedIndex = If(sizeCombo.SelectedIndex >= 3, 1, 0)
        updating = False : UpdatePreview()
    End Sub
    Private Sub DimensionsChanged(sender As Object, e As EventArgs)
        If updating Then Return
        updating = True : sizeCombo.SelectedIndex = 5 : updating = False
        UpdatePreview()
    End Sub
    Private Function ReadSettings() As LabelSettings
        settings.WidthMm = labelWidth.Value : settings.HeightMm = labelHeight.Value
        settings.Compact = layoutCombo.SelectedIndex = 1
        settings.PrinterName = If(TryCast(printerCombo.SelectedItem, String), "")
        Return settings
    End Function
    Private Sub UpdatePreview()
        If updating Then Return
        canvas.Scene = Nothing
        Dim item = TryCast(grid.CurrentRow?.DataBoundItem, ItemRow)
        If item IsNot Nothing Then
            Try
                canvas.Scene = LabelEngine.Build(item, ReadSettings())
            Catch ex As Exception
                canvas.ErrorText = ex.Message
            End Try
        Else
            canvas.ErrorText = "Upload a file, then select a row to preview its label."
        End If
        canvas.Invalidate()
    End Sub
    Private Sub UpdateSummary()
        Dim chosen = items.Where(Function(i) i.Print).ToList()
        summary.Text = $"{items.Count:N0} records    {chosen.Count:N0} ticked    {chosen.Sum(Function(i) CLng(i.Quantity)):N0} labels to print"
    End Sub
    Private Sub ValidateQuantity(sender As Object, e As DataGridViewCellValidatingEventArgs)
        If e.RowIndex < 0 OrElse grid.Columns(e.ColumnIndex).Name <> "Quantity" Then Return
        Dim value As Integer
        If Not Integer.TryParse(CStr(e.FormattedValue), value) OrElse value < 1 OrElse value > 10000 Then
            e.Cancel = True : grid.Rows(e.RowIndex).ErrorText = "Quantity must be a whole number from 1 to 10,000. Press Esc to cancel."
            status.Text = grid.Rows(e.RowIndex).ErrorText
        Else
            grid.Rows(e.RowIndex).ErrorText = ""
        End If
    End Sub
    Private Function CommitGrid() As Boolean
        If Not Validate() OrElse Not grid.EndEdit() Then Return False
        Return True
    End Function
    Private Sub SetTicks(value As Boolean)
        If Not CommitGrid() Then Return
        For Each item In items : item.Print = value : Next
        grid.Refresh() : UpdateSummary()
    End Sub
    Private Function GetBatch() As List(Of LabelScene)
        If Not CommitGrid() Then Throw New ArgumentException("Correct the quantity currently being edited.")
        Return LabelEngine.Batch(items, ReadSettings())
    End Function
    Private Sub UsingPreview()
        Using form As New BatchPreviewForm(GetBatch())
            form.ShowDialog(Me)
        End Using
    End Sub
    Private Sub ExportPdf()
        Dim pages = GetBatch()
        Using dialog As New SaveFileDialog With {.Filter = "PDF (*.pdf)|*.pdf", .FileName = "Teakwood_Labels.pdf", .Title = $"Save {pages.Count} label pages"}
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            PdfExporter.Save(dialog.FileName, pages)
            status.Text = $"Saved {pages.Count} pages to {dialog.FileName}. Print at Actual size / 100%."
        End Using
    End Sub
    Private Sub PrintLabels(one As Boolean)
        If Not CommitGrid() Then Return
        Dim pages As List(Of LabelScene)
        If one Then
            Dim item = TryCast(grid.CurrentRow?.DataBoundItem, ItemRow)
            If item Is Nothing Then Throw New ArgumentException("Select a row for the test label.")
            pages = New List(Of LabelScene) From {LabelEngine.Build(item, ReadSettings())}
        Else
            pages = GetBatch()
        End If
        Dim printer = ReadSettings().PrinterName
        Using doc = LabelPrinting.CreateDocument(pages, printer)
            If MessageBox.Show(Me, $"Print {pages.Count} label(s) on {printer}?" & vbCrLf & $"Media: {settings.WidthMm} × {settings.HeightMm} mm. Load matching labels and check the driver size.", "Print labels", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then Return
            doc.Print()
            status.Text = $"Sent {pages.Count} label(s) to {printer}. Check the printer for completion."
        End Using
    End Sub
    Private Sub RefreshPrinters()
        Try
            Dim prior = If(TryCast(printerCombo.SelectedItem, String), settings.PrinterName)
            printerCombo.Items.Clear()
            For Each name As String In PrinterSettings.InstalledPrinters : printerCombo.Items.Add(name) : Next
            If printerCombo.Items.Contains(prior) Then
                printerCombo.SelectedItem = prior
            Else
                Dim zebra = printerCombo.Items.Cast(Of String)().FirstOrDefault(Function(n) n.Contains("Zebra", StringComparison.OrdinalIgnoreCase) OrElse n.Contains("EasierSoft", StringComparison.OrdinalIgnoreCase))
                If zebra IsNot Nothing Then printerCombo.SelectedItem = zebra
            End If
        Catch ex As Exception
            status.Text = "Could not read printers: " & ex.Message
        End Try
    End Sub
    Private Sub RunAction(action As Action)
        Try
            action()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Teakwood Labels", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub
End Class
