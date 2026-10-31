$ErrorActionPreference='Stop'
$root='E:\My Client\Teakwood\Jitender\Barcode Work'
$excel=New-Object -ComObject Excel.Application
try {
    $excel.DisplayAlerts=$false
    $excel.EnableEvents=$false
    $excel.Visible=$true
    Write-Output 'Opening base workbook'
    $wb=$excel.Workbooks.Open("$root\build\base.xlsx")
    $component=$wb.VBProject.VBComponents.Import("$root\build\Barcode.bas")
    $control=$wb.Worksheets.Item('Control')
    $control.Range('B27:J27').Merge()
    $control.Range('B27').Value2='PDF export includes every data row once. Print the PDF at Actual size / 100%.'
    $actions=@(@('Import CSV','ImportCSV',7),@('Preview label','PreviewLabel',9),@('Print one test label','PrintTest',11),@('Print selected range','PrintRange',13),@('Export ZPL','ExportZPL',15))
    foreach($a in $actions){
        $anchor=$control.Range("F$($a[2])")
        $button=$control.Shapes.AddShape(5,$anchor.Left,$anchor.Top,250,29)
        $button.TextFrame2.TextRange.Text=$a[0]
        $button.TextFrame2.TextRange.Font.Name='Arial'
        $button.TextFrame2.TextRange.Font.Size=12
        $button.TextFrame2.TextRange.Font.Fill.ForeColor.RGB=16777215
        $button.Fill.ForeColor.RGB=3496740
        $button.Line.Visible=0
        $button.OnAction=$a[1]
        if($a[1] -eq 'ExportZPL'){$button.Width=110}
    }
    $anchor=$control.Range('H15');$button=$control.Shapes.AddShape(5,$anchor.Left,$anchor.Top,210,29)
    $button.TextFrame2.TextRange.Text='Export all labels to PDF';$button.TextFrame2.TextRange.Font.Size=11;$button.OnAction='ExportAllPDF'
    foreach($a in @(@('Settings','PrinterSettings','F4'),@('Edit data','ShowData','H4'))){
        $anchor=$control.Range($a[2]);$button=$control.Shapes.AddShape(5,$anchor.Left,$anchor.Top,100,24)
        $button.TextFrame2.TextRange.Text=$a[0];$button.TextFrame2.TextRange.Font.Size=11;$button.OnAction=$a[1]
    }
    foreach($name in @('Label','Settings','Data')){
        $sheet=$wb.Worksheets.Item($name)
        $anchor=if($name -eq 'Label'){$sheet.Range('H2')}elseif($name -eq 'Settings'){$sheet.Range('C28')}else{$sheet.Range('I3')}
        $button=$sheet.Shapes.AddShape(5,$anchor.Left,$anchor.Top,100,24)
        $button.TextFrame2.TextRange.Text='Back to controls';$button.TextFrame2.TextRange.Font.Size=10;$button.OnAction='Home'
    }
    $destination="$root\outputs\barcode\Teakwood_Barcode.xlsm"
    $wb.SaveAs($destination,52)
    Write-Output 'Saved macro workbook; importing CSV'
    $prefix="'Teakwood_Barcode.xlsm'!"
    $excel.Run($prefix+'ImportFile',"$root\ITEMLABLE7776.csv")
    $symbols=$excel.Run($prefix+'CodeSymbols','02224567891016')
    Write-Output "Code 128 symbols: $symbols"
    $zpl=$excel.Run($prefix+'Generate',1,$true,1)
    [IO.File]::WriteAllText("$root\build\sample203.zpl",$zpl)
    $wb.Worksheets.Item('Settings').Range('C6').Value2=300
    $zpl300=$excel.Run($prefix+'Generate',1,$false,1)
    [IO.File]::WriteAllText("$root\build\sample300.zpl",$zpl300)
    $wb.Worksheets.Item('Settings').Range('C6').Value2=203
    $batch=$excel.Run($prefix+'BuildBatch',$false)
    if(([regex]::Matches($batch,'\^XA')).Count -ne 2){throw 'Expected two labels in batch'}
    if($wb.Worksheets.Item('Data').Range('A2').Value2 -cne '02224567891016'){throw 'Leading zero lost'}
    Write-Output 'Import and batch checks passed: 2 records, 2 labels, barcode retained as text.'
    # Verify re-rendering reacts to record edits without changing the delivered data.
    $wb.Worksheets.Item('Data').Range('F3').Value2='Brown'
    $changed=$excel.Run($prefix+'Generate',2,$false,1)
    if(-not $changed.Contains('_42_72_6F_77_6E')){throw 'Colour change did not flow into print output'}
    $wb.Worksheets.Item('Data').Range('F3').Value2='Black'
    $excel.Run($prefix+'Generate',1,$true,1) | Out-Null
    $excel.Run($prefix+'WriteAllPDF',"$root\outputs\barcode\Teakwood_All_Labels.pdf")
    Write-Output 'PDF exported from workbook VBA.'
    $label=$wb.Worksheets.Item('Label')
    $label.Activate()
    $excel.ActiveWindow.DisplayGridlines=$false
    $excel.ActiveWindow.Zoom=85
    $label.Range('A1:L29').CopyPicture(1,2)
    $chart=$label.ChartObjects().Add(0,600,900,650)
    $chart.Activate();$chart.Chart.Paste()
    $chart.Chart.Export("$root\build\Label.png") | Out-Null
    $chart.Delete()
    $control.Activate();$excel.ActiveWindow.Zoom=90
    $control.Range('A1:K31').CopyPicture(1,2)
    $chart=$control.ChartObjects().Add(0,800,1000,760)
    $chart.Activate();$chart.Chart.Paste()
    $chart.Chart.Export("$root\build\Control-final.png") | Out-Null
    $chart.Delete()
    $control.Range('C7').Select()
    $data=$wb.Worksheets.Item('Data');$data.Activate();$data.Range('A1:G4').CopyPicture(1,2)
    $chart=$data.ChartObjects().Add(0,800,1000,160);$chart.Activate();$chart.Chart.Paste();$chart.Chart.Export("$root\build\Data-final.png") | Out-Null;$chart.Delete()
    $control.Activate()
    $excel.CalculateFull()
    $wb.Save()
    Write-Output "Saved: $destination"
    $wb.Close($false)
} finally {$excel.Quit()}
