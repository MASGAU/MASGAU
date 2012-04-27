#define MyAppName "MASGAU"
#define MyAppVersion "0.9.1"
#define MyAppPublisher "Matthew Barbour"
#define MyAppURL "http://masgau.org/"

[Setup]
AppMutex={#MyAppName}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
MinVersion=4.1,5.01
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
Compression=lzma/Ultra64
SolidCompression=true
OutputBaseFilename={#MyAppName}-{#MyAppVersion}-Setup
AppCopyright=2012
ChangesAssociations=true
WizardImageFile=..\Graphics\installer_logo.bmp
WizardSmallImageFile=..\Graphics\installer_logo_small.bmp
WizardImageStretch=true
SetupIconFile=..\Graphics\masgau.ico
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
Source: ..\Dependencies\isxdl.dll; Flags: dontcopy

// MASGAU Component
Source: ..\Dependencies\7-Zip\7z32.exe; DestDir: {app}; DestName: 7z.exe; Check: ThirtyTwoCheck(); Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Dependencies\7-Zip\7z64.exe; DestDir: {app}; DestName: 7z.exe; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Dependencies\7-Zip\7z32.dll; DestDir: {app}; DestName: 7z.dll; Check: ThirtyTwoCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Dependencies\7-Zip\7z64.dll; DestDir: {app}; DestName: 7z.dll; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater\updates.xml; DestDir: {app};  Components: MASGAU\Core;
Source: ..\Docs\gpl-2.0.txt; DestDir: {app}; Components: MASGAU\Core;
Source: ..\Data\Data\games.xsd; DestDir: {app}\Data; Components: MASGAU\Core; 
Source: ..\Graphics\masgau.ico; DestDir: {app};  Components: MASGAU\Core;
// Main DLLs
Source: ..\MASGAU\bin\Release\MASGAU.dll; DestDir: {app}; Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU\bin\Release\MASGAU.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.Main\bin\Release\MASGAU.Main.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Main\bin\Release\MASGAU.Main.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Windows\bin\Release\MASGAU.Windows.dll; DestDir: {app}; Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Windows\bin\Release\MASGAU.Windows.pdb; DestDir: {app}; Components: MASGAU\Debug;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.WPF\bin\Release\MASGAU.WPF.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.WPF\bin\Release\MASGAU.WPF.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater\bin\Release\MASGAU.Updater.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater\bin\Release\MASGAU.Updater.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Translations\bin\Release\Translations.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Translations\bin\Release\Translations.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
// Main EXEs
Source: ..\MASGAU.Restore.WPF\bin\Release\MASGAU.Restore.WPF.exe; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Restore.WPF\bin\Release\MASGAU.Restore.WPF.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Main.WPF\bin\Release\MASGAU.Main.WPF.exe; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Main.WPF\bin\Release\MASGAU.Main.WPF.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater.WPF\bin\Release\MASGAU.Updater.WPF.exe; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater.WPF\bin\Release\MASGAU.Updater.WPF.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
// Backup Task Component
Source: ..\MASGAU.Console\bin\Release\MASGAU.Console.dll; DestDir: {app};  Components: MASGAU\Backup; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Console\bin\Release\MASGAU.Console.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Console.Windows\bin\Release\MASGAU.Console.Windows.exe; DestDir: {app};  Components: MASGAU\Backup; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Console.Windows\bin\Release\MASGAU.Console.Windows.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
// Analyzer component 
Source: ..\MASGAU.Analyzer\bin\Release\MASGAU.Analyzer.dll; DestDir: {app}; Components: MASGAU\Analyzer; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.Analyzer\bin\Release\MASGAU.Analyzer.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.Analyzer.WPF\bin\Release\MASGAU.Analyzer.WPF.exe; DestDir: {app};  Components: MASGAU\Analyzer; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Analyzer.WPF\bin\Release\MASGAU.Analyzer.WPF.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;
// Monitor component 
Source: ..\MASGAU.Monitor\bin\Release\MASGAU.Monitor.dll; DestDir: {app}; Components: MASGAU\Monitor; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.Monitor\bin\Release\MASGAU.Monitor.pdb; DestDir: {app}; Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.Monitor.WPF\bin\Release\MASGAU.Monitor.WPF.exe; DestDir: {app};  Components: MASGAU\Monitor; Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Monitor.WPF\bin\Release\MASGAU.Monitor.WPF.pdb; DestDir: {app};  Components: MASGAU\Debug; Flags: IgnoreVersion overwritereadonly replacesameversion;

// PC Game Data component 
Source: ..\Data\Data\games.xml; DestDir: {app}\Data;  Components: DataFiles\PCData; 
Source: ..\Data\Data\mods.xml; DestDir: {app}\Data;  Components: DataFiles\PCData;
Source: ..\Data\Data\windows.xml; DestDir: {app}\Data;  Components: DataFiles\PCData;
Source: ..\Data\Data\dos.xml; DestDir: {app}\Data;  Components: DataFiles\PCData;
Source: ..\Data\Data\scummvm.xml; DestDir: {app}\Data;  Components: DataFiles\PCData;
Source: ..\Data\Data\steam.xml; DestDir: {app}\Data;  Components: DataFiles\PCData;
Source: ..\Data\Data\flash.xml; DestDir: {app}\Data;  Components: DataFiles\PCData;
// Deprecated game data
Source: ..\Data\Data\deprecated.xml; DestDir: {app}\Data;  Components: DataFiles\DeprecatedData;
// PlayStation Game Data Component 
Source: ..\Data\Data\ps1.xml; DestDir: {app}\Data; Components: DataFiles\PSData; 
Source: ..\Data\Data\ps2.xml; DestDir: {app}\Data;  Components: DataFiles\PSData;
Source: ..\Data\Data\ps3.xml; DestDir: {app}\Data;  Components: DataFiles\PSData;
Source: ..\Data\Data\psp.xml; DestDir: {app}\Data;  Components: DataFiles\PSData;
// System Data component 
Source: ..\Data\Data\system.xml; DestDir: {app}\Data; Components: DataFiles\SysData; 

// Translations
Source: ..\Translations\Strings\strings.xsd; DestDir: {app}\Strings; Components: Langs\EN; 
Source: ..\Translations\Strings\en.xml; DestDir: {app}\Strings; Components: Langs\EN; 
Source: ..\Translations\Strings\nb-NO.xml; DestDir: {app}\Strings; Components: Langs\NO;


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
Name: {group}\{cm:analyzer}; Filename: {app}\MASGAU.Analyzer.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Components: MASGAU\Analyzer; 
Name: {group}\{cm:monitor}; Filename: {app}\MASGAU.Monitor.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Components: MASGAU\Monitor; 
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
debug=Debug Files
backupTask=Backup Task
dataFiles=Data Files
pcData=PC Games
psData=PlayStation Games
sysData=System Files
deprecated=Deprecated
language=Language Files
english=English
norwegian=Norwegian
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

procedure isxdl_AddFile(URL, Filename: PChar);
external 'isxdl_AddFile@files:isxdl.dll stdcall';
function isxdl_DownloadFiles(hWnd: Integer): Integer;
external 'isxdl_DownloadFiles@files:isxdl.dll stdcall';
function isxdl_SetOption(Option, Value: PChar): Integer;
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
      if Exec(ExpandConstant(dotnetRedistPath), '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then begin
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
Name: "MASGAU\Analyzer"; Description: "{cm:analyzer}"; Types: full
Name: "MASGAU\Monitor"; Description: "{cm:monitor}"; Types: full
Name: "MASGAU\Backup"; Description: "{cm:backupTask}"; Types: full
Name: "MASGAU\Debug"; Description: "{cm:debug}"; Types: full
Name: "DataFiles"; Description: "{cm:dataFiles}"
Name: "DataFiles\PCData"; Description: "{cm:pcData}"; Types: full compact
Name: "DataFiles\PSData"; Description: "{cm:psData}"; Types: full
Name: "DataFiles\SysData"; Description: "{cm:sysData}"; Types: full
Name: "DataFiles\DeprecatedData"; Description: "{cm:deprecated}"; Types: full
Name: "Langs"; Description: "{cm:language}"
Name: "Langs\EN"; Description: "{cm:english}"; Types: full compact custom; Flags: fixed
Name: "Langs\NO"; Description: "{cm:norwegian}"; Types: full

[Types]
Name: full; Description: {cm:full};
Name: compact; Description: {cm:compact};
Name: custom; Description: {cm:custom}; Flags: IsCustom;

#include "no.iss"
