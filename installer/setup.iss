[Setup]
AppName=像素黑猫桌宠
AppVersion=1.0.1
AppPublisher=ChenZhongyu
DefaultDirName={autopf}\PixelCatDesktopPet
DefaultGroupName=像素黑猫桌宠
OutputDir=E:\Kimi_Agent_智能桌宠定制\installer
OutputBaseFilename=PixelCatDesktopPet-v1.0.1-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
LicenseFile=E:\Kimi_Agent_智能桌宠定制\LICENSE

[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "E:\Kimi_Agent_智能桌宠定制\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\像素黑猫桌宠"; Filename: "{app}\像素黑猫桌宠.exe"
Name: "{group}\卸载"; Filename: "{uninstallexe}"
Name: "{autodesktop}\像素黑猫桌宠"; Filename: "{app}\像素黑猫桌宠.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\像素黑猫桌宠.exe"; Description: "启动像素黑猫桌宠"; Flags: nowait postinstall skipifsilent
