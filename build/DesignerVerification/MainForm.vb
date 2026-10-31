Imports System.ComponentModel
Imports System.Data
Imports System.Data.Common
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.IO
Imports System.Windows.Forms

Public Partial Class MainForm
    Private settings As New LabelSettings()
    Private items As New BindingList(Of ItemRow)
    Private sheets As New List(Of ImportedSheet)
    Private updating As Boolean = True
    Private ReadOnly startupFile As String

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(initialFile As String)
        Me.New()
        startupFile = initialFile
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If DesignMode OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime Then Return
        grid.AutoGenerateColumns = False
        settings = LabelSettings.Load()
        updating = True
        labelWidth.Value = Math.Clamp(settings.WidthMm, 25D, 200D)
        labelHeight.Value = Math.Clamp(settings.HeightMm, 15D, 200D)
        sizeCombo.SelectedIndex = 5
        layoutCombo.SelectedIndex = If(settings.Compact, 1, 0)
        If settings.WidthMm = 75 AndAlso settings.HeightMm = 50 Then sizeCombo.SelectedIndex = 0
        RefreshPrinters()
        updating = False
    End Sub

    Private Async Sub MainForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        If Not DesignMode AndAlso Not String.IsNullOrWhiteSpace(startupFile) Then Await ImportPath(startupFile)
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not DesignMode AndAlso LicenseManager.UsageMode <> LicenseUsageMode.Designtime Then RunAction(Sub() ReadSettings().Save())
    End Sub

    Private Sub LayoutChanged(sender As Object, e As EventArgs) Handles layoutCombo.SelectedIndexChanged
        UpdatePreview()
    End Sub

    Private Sub ItemSelectionChanged(sender As Object, e As EventArgs) Handles grid.SelectionChanged
        UpdatePreview()
    End Sub

    Private Sub ItemCellDirty(sender As Object, e As EventArgs) Handles grid.CurrentCellDirtyStateChanged
        If grid.IsCurrentCellDirty Then grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Sub ItemValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellValueChanged
        If Not updating Then UpdateSummary()
    End Sub

    Private Sub ItemDataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles grid.DataError
        e.ThrowException = False
        status.Text = "Quantity must be a whole number between 1 and 10,000."
    End Sub

    Private Sub TickAll_Click(sender As Object, e As EventArgs) Handles btnTickAll.Click
        SetTicks(True)
    End Sub

    Private Sub ClearTicks_Click(sender As Object, e As EventArgs) Handles btnClearTicks.Click
        SetTicks(False)
    End Sub

    Private Sub ApplyQuantity_Click(sender As Object, e As EventArgs) Handles btnApplyQuantity.Click
        If Not CommitGrid() Then Return
        For Each item In items.Where(Function(i) i.Print)
            item.Quantity = CInt(quantity.Value)
        Next
        grid.Refresh()
        UpdateSummary()
    End Sub

    Private Sub PreviewBatch_Click(sender As Object, e As EventArgs) Handles btnPreviewBatch.Click
        RunAction(AddressOf UsingPreview)
    End Sub

    Private Sub ExportPdf_Click(sender As Object, e As EventArgs) Handles btnExportPdf.Click
        RunAction(AddressOf ExportPdf)
    End Sub

    Private Sub TestPrint_Click(sender As Object, e As EventArgs) Handles btnTestPrint.Click
        RunAction(Sub() PrintLabels(True))
    End Sub

    Private Sub PrintBatch_Click(sender As Object, e As EventArgs) Handles btnPrintBatch.Click
        RunAction(Sub() PrintLabels(False))
    End Sub

    Private Sub RefreshPrinters_Click(sender As Object, e As EventArgs) Handles btnRefreshPrinters.Click
        RefreshPrinters()
    End Sub

    Private Sub LabelDetails_Click(sender As Object, e As EventArgs) Handles btnLabelDetails.Click
        Using form As New DetailsForm(settings)
            If form.ShowDialog(Me) = DialogResult.OK Then
                RunAction(Sub() settings.Save())
                UpdatePreview()
            End If
        End Using
    End Sub

    Private Sub ClearData_Click(sender As Object, e As EventArgs) Handles ClearData.Click
        grid.CancelEdit()
        updating = True
        items = New BindingList(Of ItemRow)()
        sheets.Clear()
        grid.DataSource = items
        rawGrid.DataSource = Nothing
        sheetCombo.Items.Clear()
        sourceLabel.Text = "No file uploaded"
        updating = False
        UpdateSummary()
        UpdatePreview()
        status.Text = "Loaded data cleared. Your source file has not been changed."
    End Sub

    Private Async Sub UploadClick(sender As Object, e As EventArgs) Handles upload.Click
        If uploadDialog.ShowDialog(Me) = DialogResult.OK Then Await ImportPath(uploadDialog.FileName)
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
    Private Sub ChangeSheet(sender As Object, e As EventArgs) Handles sheetCombo.SelectedIndexChanged
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
    Private Sub ChangePreset(sender As Object, e As EventArgs) Handles sizeCombo.SelectedIndexChanged
        If updating OrElse sizeCombo.SelectedIndex = 5 Then Return
        Dim presets As Decimal(,) = {{75, 50}, {100, 50}, {100, 75}, {50, 30}, {40, 25}}
        updating = True
        labelWidth.Value = presets(sizeCombo.SelectedIndex, 0) : labelHeight.Value = presets(sizeCombo.SelectedIndex, 1)
        layoutCombo.SelectedIndex = If(sizeCombo.SelectedIndex >= 3, 1, 0)
        updating = False : UpdatePreview()
    End Sub
    Private Sub DimensionsChanged(sender As Object, e As EventArgs) Handles labelWidth.ValueChanged, labelHeight.ValueChanged
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
    Private Sub ValidateQuantity(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles grid.CellValidating
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colQuantity.Index Then Return
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
        pdfDialog.Title = $"Save {pages.Count} label pages"
        If pdfDialog.ShowDialog(Me) <> DialogResult.OK Then Return
        PdfExporter.Save(pdfDialog.FileName, pages)
        status.Text = $"Saved {pages.Count} pages to {pdfDialog.FileName}. Print at Actual size / 100%."
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
