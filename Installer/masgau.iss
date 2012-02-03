[_ISTool]
EnableISX=true

[Setup]
AppName=MASGAU
AppVerName=MASGAU 0.8.0
MinVersion=4.1,4.0
DefaultDirName={pf}\MASGAU
DefaultGroupName=MASGAU
Compression=lzma/Max
SolidCompression=true
OutputBaseFilename=MASGAU-0.8-Setup
AppCopyright=2010
ChangesAssociations=true
WizardImageFile=..\Graphics\installer_logo.bmp
WizardSmallImageFile=..\Graphics\installer_logo_small.bmp
WizardImageStretch=true
SetupIconFile=..\Graphics\masgau.ico
AllowRootDirectory=true
DirExistsWarning=no
VersionInfoVersion=0.8
VersionInfoProductName=MASGAU
VersionInfoProductVersion=0.8
LicenseFile=..\Docs\gpl-2.0.txt

[Files]
Source: c:\ISSI\include\isxdl\isxdl.dll; Flags: dontcopy

// MASGAU Component
Source: ..\MASGAU\bin\Release\MASGAU.exe; DestDir: {app}; Components: MASGAU\Core; Flags: replacesameversion IgnoreVersion; 
Source: ..\Task\bin\Release\Task.exe; DestDir: {app}; Components: MASGAU\Core; Flags: replacesameversion IgnoreVersion; 
Source: ..\Data\games.xsd; DestDir: {app}\Data\; Components: MASGAU\Core; 
Source: ..\Updater\bin\Release\Updater.exe; DestDir: {app}; Components: MASGAU\Core; Flags: replacesameversion IgnoreVersion; 
Source: ..\Updater\updates.xml; DestDir: {app}; Components: MASGAU\Core; 
Source: ..\Dependencies\Windows7ProgressBar.dll; DestDir: {app}; Components: MASGAU\Core; 
Source: ..\Dependencies\7-Zip\7z32.exe; DestDir: {app}; DestName: 7z.exe; Check: ThirtyTwoCheck(); Components: MASGAU\Core; 
Source: ..\Dependencies\7-Zip\7z64.exe; DestDir: {app}; DestName: 7z.exe; Check: SixtyFourCheck(); Components: MASGAU\Core; 
Source: ..\Dependencies\7-Zip\7z32.dll; DestDir: {app}; DestName: 7z.dll; Check: ThirtyTwoCheck(); Components: MASGAU\Core; 
Source: ..\Dependencies\7-Zip\7z64.dll; DestDir: {app}; DestName: 7z.dll; Check: SixtyFourCheck(); Components: MASGAU\Core; 
Source: ..\Docs\changelog.txt; DestDir: {app}; Components: MASGAU\Core; 
Source: ..\Docs\gpl-2.0.txt; DestDir: {app}; Components: MASGAU\Core; 
//Analyzer component 
Source: ..\Analyzer\bin\Release\Analyzer.exe; DestDir: {app}; Components: MASGAU\Analyzer; Flags: replacesameversion IgnoreVersion; 
// Monitor component 
Source: ..\Monitor\bin\Release\Monitor.exe; DestDir: {app}; Components: MASGAU\Monitor; Flags: replacesameversion IgnoreVersion; 
// PC Game Data component 
Source: ..\Data\games.xml; DestDir: {app}\Data\; Components: DataFiles\PCData; 
Source: ..\Data\mods.xml; DestDir: {app}\Data\; Components: DataFiles\PCData; 
// PlayStation Game Data Component 
Source: ..\Data\ps1.xml; DestDir: {app}\Data\; Components: DataFiles\PSData; 
Source: ..\Data\ps2.xml; DestDir: {app}\Data\; Components: DataFiles\PSData; 
Source: ..\Data\ps3.xml; DestDir: {app}\Data\; Components: DataFiles\PSData; 
Source: ..\Data\psp.xml; DestDir: {app}\Data\; Components: DataFiles\PSData; 
// System Data component 
Source: ..\Data\system.xml; DestDir: {app}\Data\; Components: DataFiles\SysData; 

[Registry]
// File association
Root: HKCR; Subkey: ".gb7"; ValueType: string; ValueName: ""; ValueData: "MASGAUArchive"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "MASGAUArchive"; ValueType: string; ValueName: ""; ValueData: "MASGAU Save Archive"; Flags: uninsdeletekey
Root: HKCR; Subkey: "MASGAUArchive\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\MASGAU.exe,0"
Root: HKCR; Subkey: "MASGAUArchive\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\Task.exe"" ""%1"""
// Installation folder key
Root: HKLM; SubKey: Software\MASGAU; ValueType: string; ValueName: InstallPath; ValueData: "{app}"; Components: MASGAU; Flags: UninsDeleteKey; 
Root: HKLM; SubKey: Software\MASGAU; ValueType: string; ValueData: "{app}"; Components: MASGAU; Flags: UninsDeleteKey; 

[Messages]
WinVersionTooLowError=MASGAU requires Windows NT4, Windows 98 or later.

[Icons]
Name: {group}\MASGAU (Single User Mode); Filename: {app}\MASGAU.exe; IconFilename: {app}\MASGAU.exe; Flags: CreateOnlyIfFileExists; Components: MASGAU\Core; 
Name: {group}\MASGAU (All Users Mode); Filename: {app}\MASGAU.exe; IconFilename: {app}\MASGAU.exe; Flags: CreateOnlyIfFileExists; Parameters: /allusers; Components: MASGAU\Core; 
Name: {group}\Analyzer; Filename: {app}\Analyzer.exe; IconFilename: {app}\Analyzer.exe; Flags: CreateOnlyIfFileExists; Components: MASGAU\Analyzer; 
Name: {group}\Monitor; Filename: {app}\Monitor.exe; IconFilename: {app}\Monitor.exe; Flags: CreateOnlyIfFileExists; Components: MASGAU\Monitor; 
Name: {group}\Uninstall MASGAU; Filename: {uninstallexe}; Components: MASGAU\Core; 
Name: {group}\GPL v2; Filename: {app}\gpl-2.0.txt; Components: MASGAU\Core; 
Name: {group}\Changelog; Filename: {app}\changelog.txt; Components: MASGAU\Core; 

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
  dotnetRedistURL = 'http://download.microsoft.com/download/1/B/E/1BE39E79-7E39-46A3-96FF-047F95396215/dotNetFx40_Full_setup.exe';
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
      MsgBox('MASGAU needs the Microsoft .NET 4.0 Framework to be installed by an Administrator', mbInformation, MB_OK);
      Result := false;
    end else begin
      memoDependenciesNeeded := memoDependenciesNeeded + '      .NET 4.0 Framework' #13;
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

      isxdl_SetOption('label', 'Downloading Microsoft .NET 4.0 Framework');
      isxdl_SetOption('description', 'MASGAU needs to install the Microsoft .NET 4.0 Framework. Please wait while Setup is downloading extra files to your computer.');
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
  if memoDependenciesNeeded <> '' then s := s + 'Dependencies to install:' + NewLine + memoDependenciesNeeded + NewLine;
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
Name: MASGAU; Description: MASGAU; 
Name: MASGAU\Core; Description: Core; Flags: fixed; Types: full compact custom; 
Name: MASGAU\Analyzer; Description: Analyzer; Types: full; 
Name: MASGAU\Monitor; Description: Monitor; Types: full; 
Name: DataFiles; Description: Data Files;
Name: DataFiles\PCData; Description: PC Games; Types: full compact; 
Name: DataFiles\PSData; Description: PlayStation Games; Types: full; 
Name: DataFiles\SysData; Description: System Files; Types: full; 

[Types]
Name: full; Description: The Whole Shebang;
Name: compact; Description: The Bare Essentials;
Name: custom; Description: Your Way; Flags: IsCustom;
