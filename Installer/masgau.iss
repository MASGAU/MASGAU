[_ISTool]
EnableISX=true

[Setup]
AppMutex=MASGAU
AppName=MASGAU
AppVerName=MASGAU 0.9.2
MinVersion=4.1,4.0
DefaultDirName={pf}\MASGAU
DefaultGroupName=MASGAU
Compression=lzma/Ultra64
SolidCompression=true
OutputBaseFilename=MASGAU-0.9.2-Setup
AppCopyright=2012
ChangesAssociations=true
WizardImageFile=..\Graphics\installer_logo.bmp
WizardSmallImageFile=..\Graphics\installer_logo_small.bmp
WizardImageStretch=true
SetupIconFile=..\Graphics\masgau.ico
AllowRootDirectory=true
DirExistsWarning=no
VersionInfoVersion=0.9.2
VersionInfoProductName=MASGAU
VersionInfoProductVersion=0.9.2
LicenseFile=..\Docs\gpl-2.0.txt
InternalCompressLevel=Ultra64
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\masgau.ico
VersionInfoCompany=Matthew Barbour

[Files]
Source: c:\ISSI\include\isxdl\isxdl.dll; Flags: dontcopy

// MASGAU Component
Source: ..\Dependencies\7-Zip\7z32.exe; DestDir: {app}; DestName: 7z.exe; Check: ThirtyTwoCheck(); Components: MASGAU\Core; Flags: IgnoreVersion overwritereadonly replacesameversion; 
Source: ..\Dependencies\7-Zip\7z64.exe; DestDir: {app}; DestName: 7z.exe; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Dependencies\7-Zip\7z32.dll; DestDir: {app}; DestName: 7z.dll; Check: ThirtyTwoCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\Dependencies\7-Zip\7z64.dll; DestDir: {app}; DestName: 7z.dll; Check: SixtyFourCheck(); Components: MASGAU\Core;  Flags: IgnoreVersion overwritereadonly replacesameversion;
Source: ..\MASGAU.Updater\updates.xml; DestDir: {app};  Components: MASGAU\Core;
Source: ..\gpl-2.0.txt; DestDir: {app}; Components: MASGAU\Core;
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


[Registry]
// File association
Root: HKCR; SubKey: .gb7; ValueType: string; ValueData: MASGAUArchive; Flags:  UninsDeleteKey ; 
Root: HKCR; SubKey: MASGAUArchive; ValueType: string; ValueData: "MASGAU Save Archive"; Flags: UninsDeleteKey  ; 
Root: HKCR; SubKey: MASGAUArchive\DefaultIcon; ValueType: string; ValueData: {app}\masgau.ico,0; Flags:  UninsDeleteKey ; 
Root: HKCR; SubKey: MASGAUArchive\shell\open\command; ValueType: string; ValueData: "{app}\MASGAU.Restore.WPF.exe"" ""%1"; Flags:  UninsDeleteKey ; 
// Installation folder key
Root: HKLM; SubKey: Software\MASGAU; ValueType: string; ValueName: InstallPath; ValueData: {app}; Components: MASGAU; Flags: UninsDeleteValue; 
Root: HKLM; SubKey: Software\MASGAU; ValueType: string; ValueData: {app}; Components: MASGAU; Flags: UninsDeleteKey  ; 
Root: HKLM; SubKey: Software\MASGAU; ValueType: string; ValueName: Version; ValueData: 0.9.1; Components: MASGAU; Flags:  UninsDeleteValue; 

[Messages]
WinVersionTooLowError=MASGAU requires Windows NT4, Windows 98 or later.

[Icons]
Name: {group}\MASGAU (Single User Mode); Filename: {app}\MASGAU.Main.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Components: MASGAU\Core; 
Name: {group}\MASGAU (All Users Mode); Filename: {app}\MASGAU.Main.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Parameters: -allusers; Components: MASGAU\Core; 
Name: {group}\Analyzer; Filename: {app}\MASGAU.Analyzer.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Components: MASGAU\Analyzer; 
Name: {group}\Monitor; Filename: {app}\MASGAU.Monitor.WPF.exe; IconFilename: {app}\masgau.ico; Flags: CreateOnlyIfFileExists; Components: MASGAU\Monitor; 
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
      MsgBox('MASGAU needs the Microsoft .NET 4.0 Client Profile to be installed by an Administrator', mbInformation, MB_OK);
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
Name: MASGAU\Backup; Description: Backup Task; Types: full; 
Name: MASGAU\Debug; Description: Debug Files; Types: full; 
Name: DataFiles; Description: Data Files;
Name: DataFiles\PCData; Description: PC Games; Types: full compact; 
Name: DataFiles\PSData; Description: PlayStation Games; Types: full; 
Name: DataFiles\SysData; Description: System Files; Types: full; 
Name: DataFiles\DeprecatedData; Description: Deprecated; Types: full; 

[Types]
Name: full; Description: The Whole Shebang;
Name: compact; Description: The Bare Essentials;
Name: custom; Description: Your Way; Flags: IsCustom;
