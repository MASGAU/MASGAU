#define MyAppName "MASGAU"
#define MyAppVersion "0.99.0"
#define MyAppPublisher "Matthew Barbour"
#define MyAppURL "http://masgau.org/"
#define Mode "Release"
#define Stability "Beta"
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
OutputBaseFilename={#MyAppName}-{#MyAppVersion}-{#Stability}-Setup
AppCopyright=2012
ChangesAssociations=true
WizardImageFile=installer_logo.bmp
WizardSmallImageFile=installer_logo_small.bmp
WizardImageStretch=true
SetupIconFile=..\MASGAU.Common\masgau.ico
AllowRootDirectory=true
DirExistsWarning=no
VersionInfoProductName={#MyAppName}
VersionInfoCompany={#MyAppPublisher}
VersionInfoCopyright=2012
VersionInfoVersion={#MyAppVersion}
VersionInfoProductVersion={#MyAppVersion}
LicenseFile=..\Docs\gpl-2.0.txt
InternalCompressLevel=Ultra64
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\masgau.ico

// Wipes out old program files, data files are preservde
[InstallDelete]
Type: files; Name: "{app}\*.*";
Type: files; Name: "{app}\libs\*.*";
Type: files; Name: "{commonappdata}\masgau\config.xml";
Type: filesandordirs; Name: "{group}"

// Wipes out the original file association
[Run]

[Files]
// This is just an installer library
Source: ..\Dependencies\isxdl.dll; Flags: dontcopy
// Core component
Source: ..\MASGAU.Common\settings.xml; DestDir: {app};  Components: MASGAU\Portable;
// Updater XML
Source: ..\MASGAU.Common\updates.xml; DestDir: {app};  Components: MASGAU\Core;
// Documentation
Source: ..\Docs\gpl-2.0.txt; DestDir: {app}; Components: MASGAU\Core; 
// Program icon
//Source: ..\MASGAU\masgau.ico; DestDir: {app};  Components: MASGAU\Core; 
// MASGAU Files
Source: ..\MASGAU.WPF\bin\{#Mode}\MASGAU.exe; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.WPF\bin\{#Mode}\*.dll; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\MASGAU.WPF\bin\{#Mode}\*.pdb; DestDir: {app};  Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
// 7-zip DLLs
Source: ..\Dependencies\7-Zip\7z32.exe; DestDir: {app}; DestName: 7z.exe; Check: ThirtyTwoCheck(); Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion;   
Source: ..\Dependencies\7-Zip\7z64.exe; DestDir: {app}; DestName: 7z.exe; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Dependencies\7-Zip\7z32.dll; DestDir: {app}; DestName: 7z.dll; Check: ThirtyTwoCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Dependencies\7-Zip\7z64.dll; DestDir: {app}; DestName: 7z.dll; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion; 

// Translations
Source: ..\Libs\Translator\Strings\strings.xsd; DestDir: {app}\strings; Components: Langs; 
Source: ..\MASGAU.Common\Strings\TRANSLATOR README.txt; DestDir: {app}\strings; Components: Langs; 
Source: ..\MASGAU.Common\Strings\*en.xml; DestDir: {app}\strings; Components: Langs\EN; 
//Source: ..\MASGAU.Common\Strings\*fr.xml; DestDir: {app}\strings; Components: Langs\FR; 
//Source: ..\MASGAU.Common\Strings\*no.xml; DestDir: {app}\strings; Components: Langs\NO; 


[Registry]
// File association
Root: HKCR; SubKey: .gb7; ValueType: string; ValueData: {#MyAppName}Archive; Flags: deletekey UninsDeleteKey; Components: MASGAU\FileAssociation; 
Root: HKCR; SubKey: {#MyAppName}Archive; ValueType: string; ValueData: "{#MyAppName} Save Archive"; Flags: deletekey UninsDeleteKey  ; Components: MASGAU\FileAssociation; 
Root: HKCR; SubKey: {#MyAppName}Archive\DefaultIcon; ValueType: string; ValueData: "{app}\MASGAU.exe,0"; Flags:  UninsDeleteKey ; Components: MASGAU\FileAssociation; 
Root: HKCR; SubKey: {#MyAppName}Archive\shell\Restore\command; ValueType: string; ValueData: """{app}\MASGAU.exe"" ""%1"""; Flags:  UninsDeleteKey ; Components: MASGAU\FileAssociation; 
Root: HKCR; SubKey: {#MyAppName}Archive\shell\Restore in All Users Mode\command; ValueType: string; ValueData: """{app}\MASGAU.exe"" ""-allusers"" ""%1"""; Flags:  UninsDeleteKey ; Components: MASGAU\FileAssociation; 

Root: HKCR; SubKey: Applications\MASGAU.exe; ValueType: string; ValueData: ""; Flags:  deletekey UninsDeleteKey ; Components: MASGAU\FileAssociation; 
Root: HKCR; SubKey: Applications\MASGAU.exe\shell\Restore\command; ValueType: string; ValueData: """{app}\MASGAU.exe"" ""%1"""; Flags:  UninsDeleteKey ; Components: MASGAU\FileAssociation; 
Root: HKCR; SubKey: Applications\MASGAU.exe\shell\Restore in All Users Mode\command; ValueType: string; ValueData: """{app}\MASGAU.exe"" ""-allusers"" ""%1"""; Flags:  UninsDeleteKey ; Components: MASGAU\FileAssociation; 

// Installation folder key
Root: HKLM; SubKey: Software\{#MyAppName}; ValueType: string; ValueName: InstallPath; ValueData: {app}; Components: MASGAU; Flags: UninsDeleteValue; 
Root: HKLM; SubKey: Software\{#MyAppName}; ValueType: string; ValueData: {app}; Components: MASGAU; Flags: UninsDeleteKey  ; 
Root: HKLM; SubKey: Software\{#MyAppName}; ValueType: string; ValueName: Version; ValueData: {#MyAppVersion}; Components: MASGAU; Flags:  UninsDeleteValue; 

[Messages]
WinVersionTooLowError={#MyAppName} requires %1 version %2 or later.

[Icons]
Name: {group}\{cm:singleUser,{#MyAppName}}; Filename: {app}\MASGAU.exe; Flags: CreateOnlyIfFileExists; Components: MASGAU\Shortcuts; 
Name: {group}\{cm:allUser,{#MyAppName}}; Filename: {app}\MASGAU.exe; Flags: CreateOnlyIfFileExists; Parameters: -allusers; Components: MASGAU\Shortcuts; 
Name: {app}\{cm:allUser,{#MyAppName}}; Filename: {app}\MASGAU.exe; Flags: CreateOnlyIfFileExists; Parameters: -allusers; Components: MASGAU\Shortcuts; 
Name: {group}\{cm:uninstall,{#MyAppName}}; Filename: {uninstallexe}; Components: MASGAU\Shortcuts; 
Name: {group}\GPL v2; Filename: {app}\gpl-2.0.txt; Components: MASGAU\Shortcuts; 
Name: {group}\{cm:changelog}; Filename: {app}\changelog.txt; Components: MASGAU\Shortcuts; 

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[CustomMessages]
adminNeeded=%1 needs the Microsoft .NET 4.0 Client Profile to be installed by an Administrator
downloading_Title=Downloading Microsoft .NET 4.0 Framework
downloading_Description=%1 needs to install the Microsoft .NET 4.0 Framework. Please wait while Setup is downloading extra files to your computer.
dependencies_Title=Dependencies to install:
singleUser=%1 (Single User Mode)
allUser=%1 (All Users Mode)

registry=Create registry entries so that you can double-click to restore a file
shortcut=Create Shortcuts in the Start Menu

analyzer=Analyzer
monitor=Monitor
uninstall=Uninstall %1
changelog=Changelog
core=Core
debug=Debug Files (To Help Me Help You)
backupTask=Backup Task
data=Data Files
gameData=Games
testData=Test (FAKE) Data For Debugging
full=The Whole Shebang
compact=The Bare Essentials
custom=Your Way
portable=Create a portable installation

language=Language Files
english=English
norwegian=Norwegian
french=French


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
Name: "MASGAU\FileAssociation"; Description: "{cm:registry}"; Types: full compact custom
Name: "MASGAU\Shortcuts"; Description: "{cm:shortcut}"; Types: full compact custom
Name: "MASGAU\Portable"; Description: "{cm:portable}"; Types: custom
Name: "Langs"; Description: "{cm:language}"; Types: full compact custom; Flags: fixed
Name: "Langs\EN"; Description: "{cm:english}"; Types: full compact custom; Flags: fixed
//Name: "Langs\NO"; Description: "{cm:norwegian}"; Types: full custom
//Name: "Langs\FR"; Description: "{cm:french}"; Types: full custom

[Types]
Name: full; Description: {cm:full};
Name: compact; Description: {cm:compact};
Name: custom; Description: {cm:custom}; Flags: IsCustom;

#include "no.iss"
