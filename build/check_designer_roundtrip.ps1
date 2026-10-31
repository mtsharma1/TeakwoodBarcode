$ErrorActionPreference='Stop'
$copy='E:\My Client\Teakwood\Jitender\Barcode Work\build\DesignerVerification'
New-Item -ItemType Directory -Force -Path $copy | Out-Null
Get-ChildItem -LiteralPath 'E:\My Client\Teakwood\Jitender\Barcode Work\BarcodeDesktop' -File | Where-Object {$_.Extension -in '.vb','.vbproj','.resx','.Config' -or $_.Name -eq 'NuGet.Config'} | Copy-Item -Destination $copy
Copy-Item -LiteralPath 'E:\My Client\Teakwood\Jitender\Barcode Work\BarcodeDesktop\TeakwoodBarcode.sln' -Destination "$copy\DesignerVerification.sln"
& 'C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/MSBuild.exe' "$copy\DesignerVerification.sln" /restore /p:Configuration=Debug /verbosity:minimal /nologo
if ($LASTEXITCODE -ne 0) {throw 'Verification copy build failed'}
$ide=New-Object -ComObject VisualStudio.DTE.17.0
$ide.UserControl=$true
$ide.MainWindow.Visible=$true
$ide.Solution.Open("$copy\DesignerVerification.sln")
Start-Sleep -Seconds 5
$view=$ide.ItemOperations.OpenFile("$copy\MainForm.vb")
$view.Activate()
$ide.ExecuteCommand('View.ViewDesigner')
Write-Output 'Opened isolated Designer verification copy.'
Start-Sleep -Seconds 15
$ide.ExecuteCommand('File.SaveAll')
Write-Output 'Saved isolated Designer verification copy.'
Write-Output ('Columns retained: '+[bool](Select-String -LiteralPath "$copy\MainForm.Designer.vb" -Pattern 'Columns.AddRange' -Quiet))
