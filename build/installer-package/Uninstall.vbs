Option Explicit

Dim fso, shell, installDir, startMenu, desktop
Set fso = CreateObject("Scripting.FileSystemObject")
Set shell = CreateObject("WScript.Shell")

installDir = shell.ExpandEnvironmentStrings("%LOCALAPPDATA%") & "\Programs\Teakwood Barcode"
startMenu = shell.ExpandEnvironmentStrings("%APPDATA%") & "\Microsoft\Windows\Start Menu\Programs\Teakwood Barcode.lnk"
desktop = shell.SpecialFolders("Desktop") & "\Teakwood Barcode.lnk"

If fso.FileExists(startMenu) Then fso.DeleteFile startMenu, True
If fso.FileExists(desktop) Then fso.DeleteFile desktop, True
On Error Resume Next
shell.RegDelete "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\TeakwoodBarcode\"
On Error GoTo 0

shell.Run "cmd.exe /c ping 127.0.0.1 -n 3 >nul & rmdir /s /q """ & installDir & """", 0, False

