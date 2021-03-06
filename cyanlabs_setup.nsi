;To use this script:
;  1. Download NSIS (http://nsis.sourceforge.net/Download) and install
;  2. Add something like the following into your post-build script
;       if $(ConfigurationName) == Release ("C:\Program Files (x86)\NSIS\makensis.exe" /DProductName="$(TargetName)" $(ProjectDir)cyanlabs_setup.nsi)
;  3. Build your project. 
;

; Main constants - define following constants as you want them displayed in your installation wizard
!define PRODUCT_NAME "${ProductName}"
!define PRODUCT_PUBLISHER "Cyanlabs"
!define PRODUCT_WEB_SITE "https://cyanlabs.net"

; Following constants you should never change
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!include "MUI2.nsh"
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install-full.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

Function createdesktopicon
  CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\${PRODUCT_NAME}.exe" ""
FunctionEnd

!define MUI_WELCOMEPAGE_TITLE "${PRODUCT_NAME} Setup Wizard"
!define MUI_WELCOMEPAGE_TEXT "Setup will guide you through the installation of ${PRODUCT_NAME}."
!define MUI_DIRECTORYPAGE_TEXT_TOP "Setup will install ${PRODUCT_NAME} in the following folder."
!define MUI_FINISHPAGE_TITLE "${PRODUCT_NAME} Installed"
!define MUI_FINISHPAGE_TEXT "${PRODUCT_NAME} has been installed on your computer."
!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Create Desktop Shortcut"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION createdesktopicon
!define MUI_FINISHPAGE_RUN "$INSTDIR\${PRODUCT_NAME}.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Launch ${PRODUCT_NAME}"
!define MUI_FINISHPAGE_RUN_NOTCHECKED
!define MUI_FINISHPAGE_LINK "View online documentation"
!define MUI_FINISHPAGE_LINK_LOCATION "https://cyanlabs.net/applications/${PRODUCT_NAME}"
!define MUI_FINISHPAGE_CANCEL_ENABLED
!define /date MyTIMESTAMP "%d-%m-%Y"

; Wizard pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

; Replace the constants bellow to hit suite your project
Name "${PRODUCT_NAME}"
!system "md bin\Deploy"
OutFile "bin\Deploy\VSXVolume_${MyTIMESTAMP}.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_NAME}"

ShowInstDetails show
ShowUnInstDetails show

; Following lists the files you want to include, go through this list carefully!
Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File "bin\Release\*"
SectionEnd

Section -Post
  ;Following lines will make uninstaller work - do not change anything, unless you really want to.
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "Application was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove ${PRODUCT_NAME}?" IDYES +2
  Abort
FunctionEnd

; Removal of application files and reg keys.
Section Uninstall
  RMDir /r "$INSTDIR"
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  SetAutoClose true
SectionEnd