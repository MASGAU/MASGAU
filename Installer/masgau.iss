#define MyAppName "MASGAU"
#define MyAppVersion "0.10.0"
#define MyAppPublisher "Matthew Barbour"
#define MyAppURL "http://masgau.org/"

[Setup]
AppMutex={#MyAppName}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
MinVersion=0,5.01
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
Compression=lzma/Ultra64
SolidCompression=true
OutputBaseFilename={#MyAppName}-{#MyAppVersion}-Setup
AppCopyright=2012
ChangesAssociations=true
WizardImageFile=installer_logo.bmp
WizardSmallImageFile=installer_logo_small.bmp
WizardImageStretch=true
SetupIconFile=..\MASGAU\masgau.ico
AllowRootDirectory=true
DirExistsWarning=no
VersionInfoVersion={#MyAppVersion}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
LicenseFile=..\Docs\gpl-2.0.txt
InternalCompressLevel=Ultra64
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\masgau.ico
VersionInfoCompany={#MyAppPublisher}

[Files]
// This is just an installer library
Source: ..\Dependencies\isxdl.dll; Flags: dontcopy
// Core component
// Updater XML
Source: ..\MASGAU.Updater\updates.xml; DestDir: {app};  Components: MASGAU\Core;
// Documentation
Source: ..\Docs\gpl-2.0.txt; DestDir: {app}; Components: MASGAU\Core;
// Program icon
Source: ..\MASGAU\masgau.ico; DestDir: {app};  Components: MASGAU\Core;
// General libraries
Source: ..\Libs\Collections\bin\Release\Collections.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Collections\bin\Release\Collections.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\MVC.Translator\bin\Release\MVC.Translator.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\MVC.Translator\bin\Release\MVC.Translator.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Config\bin\Release\Config.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Config\bin\Release\Config.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Exceptions\bin\Release\Exceptions.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Exceptions\bin\Release\Exceptions.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Logger\bin\Release\Logger.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Logger\bin\Release\Logger.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\MVC\bin\Release\MVC.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\MVC\bin\Release\MVC.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Translator\bin\Release\Translator.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Translator\bin\Release\Translator.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\XmlData\bin\Release\XmlData.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\XmlData\bin\Release\XmlData.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
// Ribbon dlls
Source: ..\MASGAU.Main.WPF\bin\Release\RibbonControlsLibrary.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Main.WPF\bin\Release\RibbonControlsLibrary.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Main.WPF\bin\Release\Microsoft.Windows.Shell.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Main.WPF\bin\Release\Microsoft.Windows.Shell.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
// 7-zip DLLs
Source: ..\Dependencies\7-Zip\7z32.exe; DestDir: {app}; DestName: 7z.exe; Check: ThirtyTwoCheck(); Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Dependencies\7-Zip\7z64.exe; DestDir: {app}; DestName: 7z.exe; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Dependencies\7-Zip\7z32.dll; DestDir: {app}; DestName: 7z.dll; Check: ThirtyTwoCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Dependencies\7-Zip\7z64.dll; DestDir: {app}; DestName: 7z.dll; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
// E-mail DLLS
Source: ..\Libs\Email\bin\Release\Email.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Email\bin\Release\Email.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Email\bin\Release\ActiveUp.Net.Common.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Email\bin\Release\ActiveUp.Net.Dns.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Email\bin\Release\ActiveUp.Net.Smtp.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
// Windows-specific general libaries
Source: ..\Libs\WPF\bin\Release\WPF.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\WPF\bin\Release\WPF.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\MVC.WPF\bin\Release\MVC.WPF.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\MVC.WPF\bin\Release\MVC.WPF.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Email.WPF\bin\Release\Email.WPF.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Email.WPF\bin\Release\Email.WPF.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Translator.WPF\bin\Release\Translator.WPF.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Libs\Translator.WPF\bin\Release\Translator.WPF.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
// General MASGAU libraries
Source: ..\MASGAU\bin\Release\MASGAU.Common.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU\bin\Release\MASGAU.Common.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.Updater\bin\Release\MASGAU.Updater.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater\bin\Release\MASGAU.Updater.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
// Windows-specific MASGAU libraries
Source: ..\MASGAU.Windows\bin\Release\MASGAU.Windows.dll; DestDir: {app}; Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Windows\bin\Release\MASGAU.Windows.pdb; DestDir: {app}; Components: MASGAU\Debug;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.WPF\bin\Release\MASGAU.WPF.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.WPF\bin\Release\MASGAU.WPF.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
// Main EXEs
Source: ..\MASGAU.Main.WPF\bin\Release\MASGAU.exe; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Main.WPF\bin\Release\MASGAU.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Restore.WPF\bin\Release\MASGAU.Restore.exe; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Restore.WPF\bin\Release\MASGAU.Restore.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater.WPF\bin\Release\MASGAU.Updater.exe; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater.WPF\bin\Release\MASGAU.Updater.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;

// Edit warning
Source: ..\Docs\DO_NOT_EDIT_INSTRUCTIONS_INSIDE.txt; DestDir: {app}\Data; Components: DataFiles; 
// Games schema
Source: ..\Data\Data\games.xsd; DestDir: {app}\Data; Components: DataFiles; 
// Android data component
Source: ..\Data\Data\!test.xml; DestDir: {app}\Data; Components: DataFiles\Test; 
// Deprecated game data
Source: ..\Data\Data\deprecated.xml; DestDir: {app}\Data;  Components: DataFiles\DeprecatedData;
// Game Data component 
Source: ..\Data\Data\games.xml; DestDir: {app}\Data;  Components: DataFiles\GameData; 
// System Data component 
Source: ..\Data\Data\system.xml; DestDir: {app}\Data; Components: DataFiles\SysData; 

// Translations
Source: ..\Libs\Translator\Strings\strings.xsd; DestDir: {app}\Strings; Components: Langs; 
Source: ..\Translations\Strings\en.xml; DestDir: {app}\Strings; Components: Langs\EN; 
Source: ..\Translations\Strings\nb-NO.xml; DestDir: {app}\Strings; Components: Langs\NO;
Source: ..\Translations\Strings\fr.xml; DestDir: {app}\Strings; Components: Langs\FR;


[Registry]
// File association
Root: HKCR; SubKey: .gb7; ValueType: string; ValueData: {#MyAppName}Archive; Flags:  UninsDeleteKey ; 
Root: HKCR; SubKey: {#MyAppName}Archive; ValueType: string; ValueData: "{#MyAppName} Save Archive"; Flags: UninsDeleteKey  ; 
Root: HKCR; SubKey: {#MyAppName}Archive\DefaultIcon; ValueType: string; ValueData: {app}\masgau.ico,0; Flags:  UninsDeleteKey ; 
Root: HKCR; SubKey: {#MyAppName}Archive\shell\open\command; ValueType: string; ValueData: "{app}\MASGAU.Restore.WPF.exe"" ""%1"; Flags:  UninsDeleteKey ; 
// Installation folder key
Root: HKLM; SubKey: Software\{#MyAppName}; ValueType: string; ValueName: InstallPath; ValueData: {app}; Components: MASGAU; Flags: UninsDeleteValue; 
Root: HKLM; SubKey: Software\{#MyAppName}; ValueType: string; ValueData: {app}; Components: MASGAU; Flags: UninsDeleteKey  ; 
Root: HKLM; SubKey: Software\{#MyAppName}; ValueType: string; ValueName: Version; ValueData: 0.10; Components: MASGAU; Flags:  UninsDeleteValue; 

[Messages]
WinVersionTooLowError={#MyAppName} requires %1 version %2 or later.

[Icons]
Name: {group}\{cm:singleUser,{#MyAppName}}; Filename: {app}\MASGAU.Main.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Components: MASGAU\Core; 
Name: {group}\{cm:allUser,{#MyAppName}}; Filename: {app}\MASGAU.Main.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Parameters: -allusers; Components: MASGAU\Core; 
Name: {group}\{cm:uninstall,{#MyAppName}}; Filename: {uninstallexe}; Components: MASGAU\Core; 
Name: {group}\GPL v2; Filename: {app}\gpl-2.0.txt; Components: MASGAU\Core; 
Name: {group}\{cm:changelog}; Filename: {app}\changelog.txt; Components: MASGAU\Core; 

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[CustomMessages]
adminNeeded=%1 needs the Microsoft .NET 4.0 Client Profile to be installed by an Administrator
downloading_Title=Downloading Microsoft .NET 4.0 Framework
downloading_Description=%1 needs to install the Microsoft .NET 4.0 Framework. Please wait while Setup is downloading extra files to your computer.
dependencies_Title=Dependencies to install:
singleUser=%1 (Single User Mode)
allUser=%1 (All Users Mode)
analyzer=Analyzer
monitor=Monitor
uninstall=Uninstall %1
changelog=Changelog
core=Core
debug=Debug Files (To Help Me Help You)
backupTask=Backup Task
dataFiles=Data Files
gameData=Games
testData=Test (FAKE) Data For Debugging
sysData=System Files (Mostly Account Data For Game-Related Programs)
deprecated=Deprecated (Obsolete Data So You Can Restore Older Archives)
language=Language Files
english=English
norwegian=Norwegian
french=French
full=The Whole Shebang
compact=The Bare Essentials
custom=Your Way

[ThirdPartySettings]
CompileLogMethod=append

[Code]
var
  dotnetRedistPath: string;
  downloadNeeded: boolean;
  dotNetNeeded: boolean;
  memoDependenciesNeeded: string;

procedure isxdl_AddFile(URL, Filename: PAnsiChar);
external 'isxdl_AddFile@files:isxdl.dll stdcall';
function isxdl_DownloadFiles(hWnd: Integer): Integer;
external 'isxdl_DownloadFiles@files:isxdl.dll stdcall';
function isxdl_SetOption(Option, Value: PAnsiChar): Integer;
external 'isxdl_SetOption@files:isxdl.dll stdcall';


const
// This one is the client profile
  dotnetRedistURL = 'http://download.microsoft.com/download/7/B/6/7B629E05-399A-4A92-B5BC-484C74B5124B/dotNetFx40_Client_setup.exe';
// This one is the full framework
//   dotnetRedistURL = 'http://download.microsoft.com/download/1/B/E/1BE39E79-7E39-46A3-96FF-047F95396215/dotNetFx40_Full_setup.exe';
  // local system for testing...	
  // dotnetRedistURL = 'http://192.168.1.1/dotnetfx.exe';

function InitializeSetup(): Boolean;

begin
  Result := true;
  dotNetNeeded := false;

  // Check for required netfx installation
  if (not RegKeyExists(HKLM, 'Software\Microsoft\.NETFramework\policy\v4.0')) then begin
    dotNetNeeded := true;
    if (not IsAdminLoggedOn()) then begin
      MsgBox(FmtMessage(CustomMessage('adminNeeded'), ['{#MyAppName}']), mbInformation, MB_OK);
      Result := false;
    end else begin
      memoDependenciesNeeded := memoDependenciesNeeded + '      .NET 4.0 Client Profile' #13;
      dotnetRedistPath := ExpandConstant('{src}\dotnetfx.exe');
      if not FileExists(dotnetRedistPath) then begin
        dotnetRedistPath := ExpandConstant('{tmp}\dotnetfx.exe');
        if not FileExists(dotnetRedistPath) then begin
          isxdl_AddFile(dotnetRedistURL, dotnetRedistPath);
          downloadNeeded := true;
        end;
      end;
      SetIniString('install', 'dotnetRedist', dotnetRedistPath, ExpandConstant('{tmp}\dep.ini'));
    end;
  end;

end;

function NextButtonClick(CurPage: Integer): Boolean;
var
  hWnd: Integer;
  ResultCode: Integer;
  Pars: String;

begin
  Result := true;

  if CurPage = wpReady then begin

    hWnd := StrToInt(ExpandConstant('{wizardhwnd}'));

    // don't try to init isxdl if it's not needed because it will error on < ie 3
    if downloadNeeded then begin

      isxdl_SetOption('label', CustomMessage('downloading_Title'));
      isxdl_SetOption('description', FmtMessage(CustomMessage('downloading_Description'), ['{#MyAppName}']));
      if isxdl_DownloadFiles(hWnd) = 0 then Result := false;
    end;
    if (Result = true) and (dotNetNeeded = true) then begin
      Pars := '';
        if WizardSilent then Pars := '/passive';
      if Exec(ExpandConstant(dotnetRedistPath), Pars, '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then begin
         // handle success if necessary; ResultCode contains the exit code
         if not (ResultCode = 0) then begin
           Result := false;
         end;
      end else begin
         // handle failure if necessary; ResultCode contains the error code
         Result := false;
      end;
    end;



  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  s: string;

begin
  if memoDependenciesNeeded <> '' then s := s + CustomMessage('dependencies_Title') + NewLine + memoDependenciesNeeded + NewLine;
  s := s + MemoDirInfo + NewLine + NewLine;

  Result := s
end;


function ThirtyTwoCheck(): Boolean;
begin
    if IsWin64 then begin
      Result:= FALSE;
    end else begin
      Result:= TRUE;
    end;
end;

function SixtyFourCheck(): Boolean;
begin
    if IsWin64 then begin
      Result:= TRUE;
    end else begin
      Result:= FALSE;
    end;
end;

[InnoIDE_Settings]
LogFileOverwrite=false

[Dirs]

[Components]
Name: "MASGAU"; Description: "{#MyAppName}"
Name: "MASGAU\Core"; Description: "{cm:core}"; Types: full compact custom; Flags: fixed
Name: "MASGAU\Debug"; Description: "{cm:debug}"; Types: full
Name: "DataFiles"; Description: "{cm:dataFiles}"; Types: full compact custom; Flags: fixed
Name: "DataFiles\GameData"; Description: "{cm:gameData}"; Types: full compact custom; Flags: fixed
Name: "DataFiles\SysData"; Description: "{cm:sysData}"; Types: full
Name: "DataFiles\DeprecatedData"; Description: "{cm:deprecated}"; Types: custom
Name: "DataFiles\Test"; Description: "{cm:testData}"; Types: custom
Name: "Langs"; Description: "{cm:language}"; Types: full compact custom; Flags: fixed
Name: "Langs\EN"; Description: "{cm:english}"; Types: full compact custom; Flags: fixed
Name: "Langs\NO"; Description: "{cm:norwegian}"; Types: full
Name: "Langs\FR"; Description: "{cm:french}"; Types: full

[Types]
Name: full; Description: {cm:full};
Name: compact; Description: {cm:compact};
Name: custom; Description: {cm:custom}; Flags: IsCustom;

#include "no.iss"
