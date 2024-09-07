; *** Inno Setup version 5.5.3+ Vietnamese messages ***
;Transtator: Duy Quang Le (Hanoi, Vietnam)
;Email: leduyquang753@gmail.com
;
; To download user-contributed translations of this file, go to:
;   http://www.jrsoftware.org/files/istrans/
;
; Note: When translating this text, do not add periods (.) to the end of
; messages that didn't have them already, because on those messages Inno
; Setup adds the periods automatically (appending a period would result in
; two periods being displayed).

[LangOptions]
; The following three entries are very important. Be sure to read and 
; understand the '[LangOptions] section' topic in the help file.
LanguageName=Vietnamese (Tieng Viet)
LanguageID=$042A
LanguageCodePage=0
; If the language you are translating to requires special font faces or
; sizes, uncomment any of the following entries and change them accordingly.
;DialogFontName=
;DialogFontSize=8
;WelcomeFontName=Verdana
;WelcomeFontSize=12
;TitleFontName=Arial
;TitleFontSize=29
;CopyrightFontName=Arial
;CopyrightFontSize=8

[Messages]

; *** Application titles
SetupAppTitle=Cai dat
SetupWindowTitle=Cai dat - %1
UninstallAppTitle=Go cai dat
UninstallAppFullTitle=Go cai dat %1

; *** Misc. common
InformationTitle=Thong tin
ConfirmTitle=Chung nhan
ErrorTitle=Loi

; *** SetupLdr messages
SetupLdrStartupMessage=Chuong trinh nay se cai dat %1. Ban co muon tiep tuc?
LdrCannotCreateTemp=Khong the tao tep tam thoi. Cai dat bi huy bo
LdrCannotExecTemp=Khong the chay tep trong thu muc tam thoi. Cai dat bi huy bo
; *** Startup error messages
LastErrorMessage=%1.%n%nLoi %2: %3
SetupFileMissing=Tep %1 khong duoc tim thay trong thu muc cai dat. Hay sua loi hoac tim ban khac cua chuong trinh.
SetupFileCorrupt=Tep cai dat co hong hoc. Hay tim ban khac cua chuong trinh.
SetupFileCorruptOrWrongVer=Tep cai dat co hong hoc, hoac khong tuong thich voi phien ban cai dat nay. Hay sua loi hoac tim ban khac cua chuong trinh.
InvalidParameter=Mot con tro khong hop le da di qua tren dong lenh:%n%n%1
SetupAlreadyRunning=Cai dat hien dang chay.
WindowsVersionNotSupported=Chuong trinh nay khong ho tro cho phien ban Windows ma ban dang chay.
WindowsServicePackRequired=Chuong trinh nay yeu cau %1 Goi An ninh %2 hoac moi hon.
NotOnThisPlatform=Chuong trinh nay se khong chay tren %1.
OnlyOnThisPlatform=Chuong trinh nay phai chay tren %1.
OnlyOnTheseArchitectures=Chuong trinh nay chi co the cai dat tren nhung phien ban cua Windows duoc thiet ke cho nhung bo vi xu ly sau:%n%n%1
MissingWOW64APIs=Phien ban Windows ban dang chay khong bao gom cac kha nangduoc yeu cau boi cai dat de thuc hien mot su cai dat chuong trinh 64-bit. De sua loi nay, please install Service Hay cai dat Goi An ninh%1.
WinVersionTooLowError=Chuong trinh nay yeu cau %1 phien ban %2 hoac moi hon.
WinVersionTooHighError=Chuong trinh nay khong the duoc cai dat tren %1 phien ban %2 hoac moi hon.
AdminPrivilegesRequired=Ban phai la Adminstrator de cai dat chuong trinh nay.
PowerUserPrivilegesRequired=Ban phai la Adminstrator hoac mot thanh vien cua nhom nguoi dung quyen luc de cai dat chuong trinh nay.
SetupAppRunningError=Viec cai dat bi gian doan vi %1 dang chay.%n%nHay dong chung lai, roi click OK de tiep tuc, hoac Huy de thoat.
UninstallAppRunningError=Viec go cai dat bi gian doan vi %1 dang chay.%n%nHay dong chung lai, roi click OK de tiep tuc, hoac Huy de thoat.
; *** Misc. errors
ErrorCreatingDir=Cai dat khong the tao thu muc "%1"
ErrorTooManyFilesInDir=Khong the tao tep trong thu muc "%1" vi no chua qua nhieu tep
; *** Setup common messages
ExitSetupTitle=Thoat cai dat
ExitSetupMessage=Viec cai dat chua hoan thanh. Neu ban thoat bay gio, chuong trinh se khong duoc cai dat.%n%nBan co the phai chay Cai dat mot lan khac de hoan thanh cai dat.%n%nThoat cai dat?
AboutSetupMenuItem=&Ve cai dat...
AboutSetupTitle=Ve cai dat
AboutSetupMessage=%1 phien ban %2%n%3%n%n%1 trang chu:%n%4
AboutSetupNote=Phien ban ngon ngu goc: Tieng Anh
TranslatorNote=Nguoi dich: Le Duy Quang

; *** Buttons
ButtonBack=< &Truoc
ButtonNext=T&iep >
ButtonInstall=&Cai dat
ButtonOK=OK
ButtonCancel=Huy
ButtonYes=C&o
ButtonYesToAll=Co c&ho tat ca
ButtonNo=&Khong
ButtonNoToAll=Kho&ng cho tat ca
ButtonFinish=Ho&an thanh
ButtonBrowse=Ti&m...
ButtonWizardBrowse=Tim&...
ButtonNewFolder=Tao th&u muc moi

; *** "Select Language" dialog messages
SelectLanguageTitle=Lua chon ngon ngu cai dat
SelectLanguageLabel=Lua chon ngon ngu de hien thi khi cai dat:

; *** Common wizard text
ClickNext=Click Tiep de tiep tuc, hoac Huy de thoat cai dat.
BeveledLabel=
BrowseDialogTitle=Tim thu muc
BrowseDialogLabel=Chon mot thu muc trong danh sach duoi day, roi click OK.
NewFolderName=Thu muc moi

; *** "Welcome" wizard page
WelcomeLabel1=Chao mung toi trinh cai dat [name]
WelcomeLabel2=No se cai dat [name/ver] tren may tinh cua ban.%n%nBan can phai dong tat ca chuong trinh co lien quan truoc khi tiep tuc.

; *** "Password" wizard page
WizardPassword=Mat khau
PasswordLabel1=Viec cai dat duoc bao ve bang mat khau.
PasswordLabel3=Hay nhap mat khau, roi click Tiep de tiep tuc. Can nhap chinh xac 100% (phan biet chu hoa va chu thuong).
PasswordEditLabel=&Mat khau:
IncorrectPassword=Mat khau ban vua nhap vao khong dung. Hay thu lai.

; *** "License Agreement" wizard page
WizardLicense=Hop dong Chuyen giao
LicenseLabel=Hay doc nhung thong tin quan trong sau truoc khi tiep tuc.
LicenseLabel3=Hay doc hop dong Chuyen giao sau day. Ban PHAI chap nhan cac dieu khoan truoc khi tiep tuc.
LicenseAccepted=Toi &chap nhan cac dieu khoan
LicenseNotAccepted=Toi k&hong chap nhan cac dieu khoan

; *** "Information" wizard pages
WizardInfoBefore=Thong tin
InfoBeforeLabel=Hay doc nhung thong tin quan trong sau truoc khi tiep tuc.
InfoBeforeClickLabel=Khi ban da san sang tiep tuc voi Cai dat, click Tiep.
WizardInfoAfter=Thong tin
InfoAfterLabel=Hay doc nhung thong tin quan trong sau truoc khi tiep tuc.
InfoAfterClickLabel=Khi ban da san sang tiep tuc voi Cai dat, click Tiep.

; *** "User Information" wizard page
WizardUserInfo=Thong tin nguoi dung
UserInfoDesc=Hay nhap thong tin cua ban.
UserInfoName=T&en nguoi dung:
UserInfoOrg=T&o chuc:
UserInfoSerial=&Serial:
UserInfoNameRequired=Ban phai dien vao mot ten.

; *** "Select Destination Location" wizard page
WizardSelectDir=Lua chon thu muc dich
SelectDirDesc=[name] nen duoc cai dat o dau?
SelectDirLabel3=Cai dat se cai [name] vao thu muc sau.
SelectDirBrowseLabel=De tiep tuc, click Tiep. Neu ban muon chon thu muc khac, click Tim.
DiskSpaceMBLabel=It nhat [mb] MB o dia trong duoc yeu cau.
CannotInstallToNetworkDrive=Cai dat khong the cai vao mang luoi.
CannotInstallToUNCPath=Cai dat khong the cai vao dia chi UNC.
InvalidPath=Ban phai nhap duong dan day du voi chu cai cua o dia; vi du:%n%nC:\APP%n%nhoac mot duong dan UNC theo mau:%n%n\\server\share
InvalidDrive=O dia hoac UNC share ban da chon khong ton tai hoac khong the truy cap. Hay chon khac.
DiskSpaceWarningTitle=Khong du dung luong dia trong
DiskSpaceWarning=Cai dat can it nhat %1 KB o dia trong de cai dat, nhung o dia da chon chi co %2 KB.%n%nBan co muon tiep tuc?
DirNameTooLong=Ten thu muc hoac duong dan qua dai.
InvalidDirName=Ten thu muc khong hop le.
BadDirName32=Ten thu muc khong duoc co cac ki tu sau:%n%n%1
DirExistsTitle=Thu muc da ton tai
DirExists=Thu muc:%n%n%1%n%nda ton tai. Ban co muon cai thu muc nay?
DirDoesntExistTitle=Thu muc khong ton tai
DirDoesntExist=Thu muc:%n%n%1%n%nkhong ton tai. Ban co muon tao thu muc nay?

; *** "Select Components" wizard page
WizardSelectComponents=Lua chon phan phu tro
SelectComponentsDesc=Nhung phan phu tro nao nen duoc cai dat?
SelectComponentsLabel2=Lua chon nhung phan phu tro ban muon cai dat; xoa nhung cai ban khong muon. Click Tiep khi ban da san sang tiep tuc.
FullInstallation=Cai dat day du
; if possible don't translate 'Compact' as 'Minimal' (I mean 'Minimal' in your language)
CompactInstallation=Cai dat co ban
CustomInstallation=Cai dat theo y thich
NoUninstallWarningTitle=Phan phu tro da ton tai
NoUninstallWarning=Cai dat bi gian doan vi nhung thanh phan phu tro sau da duoc cai dat vao may tinh cua ban:%n%n%1%n%nBo chon nhung cai nay se khong cai dat chung.%n%nBan co muon tiep tuc?
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceMBLabel=Lua chon nay can it nhat [mb] MB o dia trong.

; *** "Select Additional Tasks" wizard page
WizardSelectTasks=Lua chon hanh dong bo sung
SelectTasksDesc=Nhung hanh dong nao can duoc bo sung?
SelectTasksLabel2=Lua chon nhung hanh dong ma ban muon Cai dat bo sung khi cai dat [name], sau do click Tiep.

; *** "Select Start Menu Folder" wizard page
WizardSelectProgramGroup=ua chon thu muc trong menu Start
SelectStartMenuFolderDesc=Loi tat cua chuong trinh nen duoc dat o dau trong menu Start?
SelectStartMenuFolderLabel3=Cai dat se tao loi tat cua chuong trinh trong thu muc sau cua menu Start.
SelectStartMenuFolderBrowseLabel=De tiep tuc, click Tiep. Neu ban muon chon thu muc khac, click Tim.
MustEnterGroupName=Ban phai nhap ten mot thu muc.
GroupNameTooLong=Ten thu muc hoac duong dan qua dai.
InvalidGroupName=Ten thu muc khong hop le.
BadGroupName=Ten thu muc khong duoc chua cac ki tu sau:%n%n%1
NoProgramGroupCheck2=&Khong tao mot thu muc trong menu Start

; *** "Ready to Install" wizard page
WizardReady=San sang cai dat
ReadyLabel1=Cai dat da san sang cai [name] tren may tinh cua ban.
ReadyLabel2a=Click Cai dat de tiep tuc cai dat, hoac click Truoc neu ban muon xem lai hoac thay doi mot so cai dat.
ReadyLabel2b=Click Cai dat de tiep tuc cai dat.
ReadyMemoUserInfo=Thong tin nguoi dung:
ReadyMemoDir=Thu muc dich:
ReadyMemoType=Kieu cai dat:
ReadyMemoComponents=Nhung thanh phan phu tro da chon:
ReadyMemoGroup=Thu muc trong menu Start:
ReadyMemoTasks=Nhung hanh dong bo sung:

; *** "Preparing to Install" wizard page
WizardPreparing=Chuan bi cai dat
PreparingDesc=Cai dat dang chuan bi cai [name] tren may tinh cua ban.
PreviousInstallNotCompleted=Viec (go) cai dat chuong trinh cu khong hoan thanh. Ban se can phai khoi dong lai may tinh de hoan thanh cai dat.%n%nSau khi khoi dong lai, hay chay Cai dat mot lan nua de hoan thanh cai dat [name].
CannotContinue=Cai dat khong the tiep tuc. Click Huy de thoat.
ApplicationsFound=Nhung chuong trinh sau su dung cac tep can duoc cap nhat boi Cai dat. Dieu can thiet la ban phai chap nhan Cai dat dong cac chuong trinh nay.
ApplicationsFound2=Nhung chuong trinh sau su dung cac tep can duoc cap nhat boi Cai dat. Dieu can thiet la ban phai chap nhan Cai dat dong cac chuong trinh nay. Sau khi cai dat hoan thanh, Cai dat se co mo lai chung.
CloseApplications=T&u dong dong nhung chuong trinh nay
DontCloseApplications=&Khong dong nhung chuong trinh nay
ErrorCloseApplications=Cai dat khong the dong tat ca cac chuong trinh. Dieu can thiet la ban phai dong cac chuong trinh su dung tep can duoc cap nhat boi Cai dat truoc khi tiep tuc.

; *** "Installing" wizard page
WizardInstalling=Dang cai dat
InstallingLabel=Hay doi khi Cai dat dang cai [name] tren may tinh cua ban.

; *** "Setup Completed" wizard page
FinishedHeadingLabel=Hoan thanh cai dat [name]
FinishedLabelNoIcons=Da cai dat thanh cong [name] tren may tinh cua ban.
FinishedLabel=Da cai dat thanh cong [name] tren may tinh cua ban. No co the duoc chay bang cach click vao bieu tuong cua no.
ClickFinish=Click Hoan thnah de thoat Cai dat.
FinishedRestartLabel=De hoan thanh cai dat [name], may tinh cua ban phai duoc khoi dong lai. Ban co muon khoi dong lai khong?
FinishedRestartMessage=De hoan thanh cai dat [name], may tinh cua ban phai duoc khoi dong lai.%n%nBan co muon khoi dong lai khong?
ShowReadmeCheck=Co, to muon xem tep README
YesRadio=&Co, khoi dong lai may tinh ngay bay gio
NoRadio=&Khong, toi se khoi dong lai may tinh sau
; used for example as 'Run MyProg.exe'
RunEntryExec=Chay %1
; used for example as 'View Readme.txt'
RunEntryShellExec=Hien thi %1

; *** "Setup Needs the Next Disk" stuff
ChangeDiskTitle=Cai dat can dia tiep theo
SelectDiskLabel2=Hay chen dia %1 va click OK.%n%nNeu nhung tep tren dia do co the tim o noi khac voi duoc hien thi duoi day, nhap duong dan dung hoac click Tim.
PathLabel=&Path:
FileNotInDir2=Tep "%1" khong duoc dat trong "%2". Hay chen dia dung hoac chon thu muc khac.
SelectDirectoryLabel=Hay xac dinh vi tri cua dia tiep theo.

; *** Installation phase messages
SetupAborted=Cai dat khong duoc hoan thanh.%n%nHay sua loi va chay Cai dat lai.
EntryAbortRetryIgnore=Click Thu lai de thu lai, Bo qua de tiep tuc (nhay qua loi), hoac Tu choi de huy cai dat.

; *** Installation status messages
StatusClosingApplications=Dang dong cac chuong trinh...
StatusCreateDirs=Dang tao thu muc...
StatusExtractFiles=Dang giai nen cac tep...
StatusCreateIcons=Dang tao loi tat...
StatusCreateIniEntries=Dang tao dau vao INI...
StatusCreateRegistryEntries=Dang tao dau vao Registry...
StatusRegisterFiles=Dang xac minh cac tep...
StatusSavingUninstall=Dang luu thong tin go cai dat...
StatusRunProgram=Dang hoan thanh cai dat...
StatusRestartingApplications=Dang khoi dong lai cac chuong trinh...
StatusRollback=Dang reset lai cac thay doi...

; *** Misc. errors
ErrorInternal2=Loi: %1
ErrorFunctionFailedNoCode=%1 that bai
ErrorFunctionFailed=%1 that bai; ma %2
ErrorFunctionFailedWithMessage=%1 that bai; ma %2.%n%3
ErrorExecutingProgram=hong the giai nen tep:%n%1

; *** Registry errors
ErrorRegOpenKey=Loi mo chia khoa xac thuc:%n%1\%2
ErrorRegCreateKey=Loi tao chia khoa xac thuc:%n%1\%2
ErrorRegWriteKey=Loi viet chia khoa xac thuc:%n%1\%2

; *** INI errors
ErrorIniEntry=Loi tao dau vao INI trong tep "%1".

; *** File copying errors
FileAbortRetryIgnore=Click thu lai de thu lai, Bo qua de bo qua tep nay (khong khuyen nghi), hoac Tu choi de huy cai dat.
FileAbortRetryIgnore2=Click thu lai de thu lai, Bo qua de bo qua loi (khong khuyen nghi), hoac Tu choi de huy cai dat.
SourceIsCorrupted=Tep nguon bi hu hong
SourceDoesntExist=Tep nguon "%1" khong ton tai
ExistingFileReadOnly=Tep nay da duoc danh dau Chi doc.%n%nClick Thu lai de bo danh dau Chi doc va thu lai, Bo qua de bo qua tep nay, hoac Tu choi de huy cai dat.
ErrorReadingExistingDest=Co loi khi doc tep:
FileExists=Tep da ton tai.%n%nBan co muon thay the?
ExistingFileNewer=Tep nay moi hon tep ma Cai dat muon cai vao. Dieu can thiet la ban giu lai tep nay.%n%nBan co muon giu lai?
ErrorChangingAttr=Co van de khi thay doi thuoc tinh tep:
ErrorCreatingTemp=Co van de khi tao tep trong thu muc dich:
ErrorReadingSource=Co van de khi doc tep nguon:
ErrorCopying=Co van de khi co copy tep:
ErrorReplacingExistingFile=Co van de khi co thay the tep:
ErrorRestartReplace=Thay the khoi dong lai that bai:
ErrorRenamingTemp=Co van de khi co doi ten tep trong thu muc dich:
ErrorRegisterServer=Khong the xac thuc DLL/OCX: %1
ErrorRegSvr32Failed=RegSvr32 that bai voi ma thoat %1
ErrorRegisterTypeLib=Khong the xac thuc thu vien kieu: %1

; *** Post-installation errors
ErrorOpeningReadme=Co van de khi co mo tep README.
ErrorRestartingComputer=Cai dat khong the khoi dong lai may tinh. Hay lam thu cong.

; *** Uninstaller messages
UninstallNotFound=Tep "%1" hong ton tai. Khong the go cai dat
UninstallOpenError=Khong the mo tep "%1". Khong the go cai dat
UninstallUnsupportedVer=Tep dang nhap go cai dat "%1" thuoc kieu tep khong duoc ho tro. Khong the go cai dat
UninstallUnknownEntry=Mot dau vao khong ro rang (%1) duoc ke den trong dang nhap go cai dat
ConfirmUninstall=Ban co thuc su muon go %1 va toan bo thanh phan lien quan?
UninstallOnlyOnWin64=Viec cai dat nay chi co the go tren phien ban Windows 64-bit.
OnlyAdminCanUninstall=Viec cai dat nay chi co the go khi ban la Adminstrator.
UninstallStatusLabel=Hay doi khi %1 duoc go khoi may tinh cua ban.
UninstalledAll=%1 da duoc go thanh cong khoi may tinh cua ban.
UninstalledMost=Go cai dat %1 hoan thanh.%n%nMot so thanh phan khong the go. Chung co the duoc go thu cong.
UninstalledAndNeedsRestart=De hoan thanh go cai dat %1, phai khoi dong lai may tinh.%n%nBan co muon khoi dong lai?
UninstallDataCorrupted=Tep "%1" bi hu hong. Khong the go cai dat
; *** Uninstallation phase messages
ConfirmDeleteSharedFileTitle=Go tep da chia se?
ConfirmDeleteSharedFile2=He thong da chi ra tep da chia se sau khong duoc dung boi bat cu chuong trinh nao khac. Ban co muon go chung?%n%nNeu mot so chuong trinh van con dung tep nay ma no da bi go, chung co the khong hoat dong tot. Neu ban khong hai long, chon Khong. Go bo chung tren he thong cua ban se khong gay ra bat cu thiet hai nao.
SharedFileNameLabel=Ten tep:
SharedFileLocationLabel=Vi tri:
WizardUninstalling=Tinh trang go
StatusUninstalling=Dang go cai dat %1...

; *** Shutdown block reasons
ShutdownBlockReasonInstallingApp=Dang cai dat %1.
ShutdownBlockReasonUninstallingApp=Dang go cai dat %1.

; The custom messages below aren't used by Setup itself, but if you make
; use of them in your scripts, you'll want to translate them.

[CustomMessages]

NameAndVersion=%1 phien ban %2
AdditionalIcons=Nhung loi tat bo sung:
CreateDesktopIcon=Tao mot loi tat &Desktop
CreateQuickLaunchIcon=Tao mot loi tat Khoi dong nh&anh
ProgramOnTheWeb=%1 tren Web
UninstallProgram=Go cai dat %1
LaunchProgram=Khoi chay %1
AssocFileExtension=&Gan ket %1 voi su mo rong tep cua %2
AssocingFileExtension=Dang gan ket %1 voi su mo rong tep cua %2...
AutoStartProgramGroupDescription=Khoi dong:
AutoStartProgram=Tu dong khoi dong %1
AddonHostProgramNotFound=%1 khong the duoc dat vao thu muc ban da chon.%n%nBan co muon tiep tuc?
