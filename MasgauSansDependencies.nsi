;NSIS Modern User Interface
;Basic Example Script
;Written by Joost Verburg
; Used to be anyway. SanMadJack's all up in here.

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"
  !include "WordFunc.nsh"
  !include "LogicLib.nsh"
  !include "Sections.nsh"
  !include "x64.nsh"
  !include "FileAssociation.nsh"
  
;--------------------------------
;General

  ;Name and file
  Name "MASGAU 0.7"
  OutFile "MASGAU-0.7-NoDeps.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\MASGAU"
  
  ;Get installation folder from registry if available
  InstallDirRegKey HKLM "Software\MASGAU" "InstallPath"

  SetCompress force
  SetCompressor lzma
  RequestExecutionLevel admin


;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING
  !define MUI_ICON "Graphics\masgau.ico"
  !define MUI_UNINSTALLICON "Graphics\masgau.ico"
  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP "Graphics\masgau_logo.bmp"
  !define MUI_HEADERIMAGE_UNBITMAP "Graphics\masgau_logo.bmp"
  !define MUI_HEADERIMAGE_RIGHT

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "Docs\gpl-2.0.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro VersionCompare
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

  
;--------------------------------
;Installer Sections

Section "MASGAU" MASGAU

	SectionIn RO

  SetOutPath "$INSTDIR"
  
  ;ADD YOUR OWN FILES HERE...
  File "Masgau\bin\Release\Masgau.exe"
  File "MasgauTask\bin\Release\MasgauTask.exe"
  File "Masgau\bin\Release\Interop.IWshRuntimeLibrary.dll"
  File "Masgau\bin\Release\Windows7ProgressBar.dll"
	SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\MASGAU"
  CreateShortCut "$SMPROGRAMS\MASGAU\MASGAU - All Users Mode.lnk" "$INSTDIR\Masgau.exe" "/allusers" "$INSTDIR\Masgau.exe" 0
  CreateShortCut "$SMPROGRAMS\MASGAU\MASGAU - Single User Mode.lnk" "$INSTDIR\Masgau.exe" "" "$INSTDIR\Masgau.exe" 0
  CreateShortCut "$SMPROGRAMS\MASGAU\Uninstall MASGAU.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0

  ;Register file type
  ${registerExtension} "$INSTDIR\MasgauTask.exe" ".gb7" "MASGAU Save Game Archive"
  
  ;Store installation folder
  WriteRegStr HKLM "Software\MASGAU" "InstallPath" $INSTDIR
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

SectionEnd

Section "MASGAU Monitor" MASGAUMonitor
  SetOutPath "$INSTDIR"
  File "MasgauMonitor\bin\Release\MasgauMonitor.exe"
SetShellVarContext all
  CreateShortCut "$SMPROGRAMS\MASGAU\MASGAU Monitor.lnk" "$INSTDIR\MasgauMonitor.exe" "" "$INSTDIR\MasgauMonitor.exe" 0
SectionEnd

Section "MASGAU Analyzer" MASGAUAnalyzer
  SetOutPath "$INSTDIR"
  File "MasgauAnalyzer\bin\Release\MasgauAnalyzer.exe"
SetShellVarContext all
  CreateShortCut "$SMPROGRAMS\MASGAU\MASGAU Analyzer.lnk" "$INSTDIR\MasgauAnalyzer.exe" "" "$INSTDIR\MasgauAnalyzer.exe" 0
SectionEnd

Section "PC Game Profiles" PCGameProfiles
  SetOutPath "$INSTDIR"
  File "Masgau\games.xml"
  File "Masgau\mods.xml"
  File "Masgau\system.xml"
SectionEnd

Section "PlayStation Profiles" PSGameProfiles
  SetOutPath "$INSTDIR"
  File "Masgau\ps1.xml"
  File "Masgau\ps2.xml"
  File "Masgau\ps3.xml"
  File "Masgau\psp.xml"
SectionEnd

;--------------------------------
  ;Language strings
  LangString DESC_MASGAU ${LANG_ENGLISH} "The core of MASGAU."
  LangString DESC_MASGAUMONITOR ${LANG_ENGLISH} "A tray app that watches for new saves."
  LangString DESC_MASGAUANALYZER ${LANG_ENGLISH} "A tool that generates save game reports for submitting via e-mail."
  LangString DESC_PCGAMES ${LANG_ENGLISH} "XMl Profiles for Windows games. Required for detecting Windows games."
  LangString DESC_PSGAMES ${LANG_ENGLISH} "XML profiles for PlayStation games. Required for detecting Playstation games."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${MASGAU} $(DESC_MASGAU)
    !insertmacro MUI_DESCRIPTION_TEXT ${MASGAUMonitor} $(DESC_MASGAUMONITOR)
    !insertmacro MUI_DESCRIPTION_TEXT ${MASGAUAnalyzer} $(DESC_MASGAUANALYZER)
    !insertmacro MUI_DESCRIPTION_TEXT ${PCGameProfiles} $(DESC_PCGAMES)
    !insertmacro MUI_DESCRIPTION_TEXT ${PSGameProfiles} $(DESC_PSGAMES)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...

  Delete "$INSTDIR\*"
  RMDir "$INSTDIR"

  Delete "$SMPROGRAMS\MASGAU\*"
  RMDir "$SMPROGRAMS\MASGAU"
SetShellVarContext all
  Delete "$SMPROGRAMS\MASGAU\*"
  RMDir "$SMPROGRAMS\MASGAU"
  
  
  ${unregisterExtension} ".gb7" "MASGAU Save Game Archive"

  DeleteRegKey /ifempty HKLM "Software\MASGAU"

SectionEnd
