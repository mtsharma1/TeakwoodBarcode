$ErrorActionPreference='Stop'
$solution='E:\My Client\Teakwood\Jitender\Barcode Work\BarcodeDesktop\TeakwoodBarcode.sln'
$ide=New-Object -ComObject VisualStudio.DTE.17.0
$ide.UserControl=$true
$ide.MainWindow.Visible=$true
$ide.Solution.Open($solution)
Write-Output 'Opened solution in Visual Studio 2022.'
$designer='{7651a700-06e5-11d1-8ebd-00a0c90f26ea}'
$window=$ide.ItemOperations.OpenFile('E:\My Client\Teakwood\Jitender\Barcode Work\BarcodeDesktop\MainForm.vb',$designer)
$window.Activate()
Write-Output ('Opened view: '+$window.Caption)
