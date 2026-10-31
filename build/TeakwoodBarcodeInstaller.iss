#define AppName "Teakwood Barcode Label Studio"
#define AppVersion "1.1.2"
#define AppExeName "TeakwoodBarcode.exe"

[Setup]
AppId={{B32C26B7-BD54-4B0C-BBAF-59D2EB359D61}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Teakwood Leathers
DefaultDirName={localappdata}\Programs\Teakwood Barcode
DefaultGroupName=Teakwood Barcode
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=E:\My Client\Teakwood\Jitender\Barcode Work\outputs\desktop
OutputBaseFilename=TeakwoodBarcode_Setup
Compression=lzma2/max
SolidCompression=yes
Encryption=yes
Password=mithu!@#12$
WizardStyle=modern
UninstallDisplayName={#AppName}
UninstallDisplayIcon={app}\{#AppExeName}
VersionInfoVersion={#AppVersion}
VersionInfoCompany=Teakwood Leathers
VersionInfoDescription={#AppName} Installer

[Files]
Source: "E:\My Client\Teakwood\Jitender\Barcode Work\build\installer-publish\TeakwoodBarcode.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Client\Teakwood\Jitender\Barcode Work\build\installer-publish\*.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\Teakwood Barcode"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\Teakwood Barcode"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent
