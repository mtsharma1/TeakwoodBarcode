Option Explicit

Dim fso, shell, sourceDir, installDir, startMenu, desktop, file, shortcut, uninstallKey
Set fso = CreateObject("Scripting.FileSystemObject")
Set shell = CreateObject("WScript.Shell")

sourceDir = fso.GetParentFolderName(WScript.ScriptFullName)
installDir = shell.ExpandEnvironmentStrings("%LOCALAPPDATA%") & "\Programs\Teakwood Barcode"
startMenu = shell.ExpandEnvironmentStrings("%APPDATA%") & "\Microsoft\Windows\Start Menu\Programs\Teakwood Barcode.lnk"
desktop = shell.SpecialFolders("Desktop") & "\Teakwood Barcode.lnk"

If Not fso.FolderExists(installDir) Then fso.CreateFolder(installDir)

For Each file In fso.GetFolder(sourceDir).Files
    If LCase(fso.GetExtensionName(file.Name)) = "exe" Or LCase(fso.GetExtensionName(file.Name)) = "dll" Then
        fso.CopyFile file.Path, installDir & "\" & file.Name, True
    End If
Next
fso.CopyFile sourceDir & "\Uninstall.vbs", installDir & "\Uninstall.vbs", True

Set shortcut = shell.CreateShortcut(startMenu)
shortcut.TargetPath = installDir & "\TeakwoodBarcode.exe"
shortcut.WorkingDirectory = installDir
shortcut.Description = "Teakwood Barcode Label Studio"
shortcut.Save

Set shortcut = shell.CreateShortcut(desktop)
shortcut.TargetPath = installDir & "\TeakwoodBarcode.exe"
shortcut.WorkingDirectory = installDir
shortcut.Description = "Teakwood Barcode Label Studio"
shortcut.Save

uninstallKey = "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\TeakwoodBarcode\"
shell.RegWrite uninstallKey & "DisplayName", "Teakwood Barcode Label Studio", "REG_SZ"
shell.RegWrite uninstallKey & "DisplayVersion", "1.0.0", "REG_SZ"
shell.RegWrite uninstallKey & "Publisher", "Teakwood Leathers", "REG_SZ"
shell.RegWrite uninstallKey & "InstallLocation", installDir, "REG_SZ"
shell.RegWrite uninstallKey & "DisplayIcon", installDir & "\TeakwoodBarcode.exe", "REG_SZ"
shell.RegWrite uninstallKey & "UninstallString", "wscript.exe """ & installDir & "\Uninstall.vbs""", "REG_SZ"
shell.RegWrite uninstallKey & "NoModify", 1, "REG_DWORD"
shell.RegWrite uninstallKey & "NoRepair", 1, "REG_DWORD"

shell.Run """" & installDir & "\TeakwoodBarcode.exe""", 1, False

