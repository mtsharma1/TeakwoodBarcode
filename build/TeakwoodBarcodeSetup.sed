[Version]
Class=IEXPRESS
SEDVersion=3

[Options]
PackagePurpose=InstallApp
ShowInstallProgramWindow=0
HideExtractAnimation=0
UseLongFileName=1
InsideCompressed=0
CAB_FixedSize=0
CAB_ResvCodeSigning=0
RebootMode=N
InstallPrompt=%InstallPrompt%
DisplayLicense=
FinishMessage=%FinishMessage%
TargetName=%TargetName%
FriendlyName=%FriendlyName%
AppLaunched=%AppLaunched%
PostInstallCmd=<None>
AdminQuietInstCmd=%AppLaunched%
UserQuietInstCmd=%AppLaunched%
SourceFiles=SourceFiles

[Strings]
InstallPrompt=Install Teakwood Barcode Label Studio?
FinishMessage=Teakwood Barcode Label Studio was installed successfully.
TargetName=E:\My Client\Teakwood\Jitender\Barcode Work\outputs\desktop\TeakwoodBarcode_Installer.exe
FriendlyName=Teakwood Barcode Label Studio Setup
AppLaunched=wscript.exe Install.vbs
FILE0="Install.vbs"
FILE1="Uninstall.vbs"
FILE2="TeakwoodBarcode.exe"
FILE3="D3DCompiler_47_cor3.dll"
FILE4="PenImc_cor3.dll"
FILE5="PresentationNative_cor3.dll"
FILE6="vcruntime140_cor3.dll"
FILE7="wpfgfx_cor3.dll"

[SourceFiles]
SourceFiles0=E:\My Client\Teakwood\Jitender\Barcode Work\build\installer-package\

[SourceFiles0]
%FILE0%=
%FILE1%=
%FILE2%=
%FILE3%=
%FILE4%=
%FILE5%=
%FILE6%=
%FILE7%=
