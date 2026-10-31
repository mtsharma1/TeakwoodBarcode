from pathlib import Path
root=Path(__file__).resolve().parent.parent/'BarcodeDesktop'
p=root/'MainForm.vb'
old=p.read_text(encoding='utf-8-sig')
rest=old[old.index('    Private Async Sub UploadClick'):]
rest=rest.replace('Private Async Sub UploadClick(sender As Object, e As EventArgs)','Private Async Sub UploadClick(sender As Object, e As EventArgs) Handles upload.Click')
rest=rest.replace('Private Sub ChangeSheet(sender As Object, e As EventArgs)','Private Sub ChangeSheet(sender As Object, e As EventArgs) Handles sheetCombo.SelectedIndexChanged')
rest=rest.replace('Private Sub ChangePreset(sender As Object, e As EventArgs)','Private Sub ChangePreset(sender As Object, e As EventArgs) Handles sizeCombo.SelectedIndexChanged')
rest=rest.replace('Private Sub DimensionsChanged(sender As Object, e As EventArgs)','Private Sub DimensionsChanged(sender As Object, e As EventArgs) Handles labelWidth.ValueChanged, labelHeight.ValueChanged')
rest=rest.replace('Private Sub ValidateQuantity(sender As Object, e As DataGridViewCellValidatingEventArgs)','Private Sub ValidateQuantity(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles grid.CellValidating')
start=rest.index('        Using dialog As New OpenFileDialog')
end=rest.index('        End Using',start)+len('        End Using')
rest=rest[:start]+'''        If uploadDialog.ShowDialog(Me) = DialogResult.OK Then Await ImportPath(uploadDialog.FileName)'''+rest[end:]
start=rest.index('        Using dialog As New SaveFileDialog')
end=rest.index('        End Using',start)+len('        End Using')
rest=rest[:start]+'''        pdfDialog.Title = $"Save {pages.Count} label pages"
        If pdfDialog.ShowDialog(Me) <> DialogResult.OK Then Return
        PdfExporter.Save(pdfDialog.FileName, pages)
        status.Text = $"Saved {pages.Count} pages to {pdfDialog.FileName}. Print at Actual size / 100%."'''+rest[end:]
prefix='''Imports System.ComponentModel
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

'''
p.write_text(prefix+rest,encoding='utf-8')
printing=root/'Printing.vb'
body=printing.read_text(encoding='utf-8-sig')
printing.write_text(body[:body.index('Public Class BatchPreviewForm')],encoding='utf-8')
print('Moved controls out of business code and connected named event handlers.')
