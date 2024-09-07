; *** Inno Setup version 5.5.3+ Hungarian messages with "a(z)" definite articles ***
; Copyright (C) 1999-2012 Korn�l P�l
; All rights reserved.
; E-mail: kornelpal@gmail.com
; Hungarian Inno Setup translation home page: http://www.kornelpal.hu/ishu
;
; You can download the versions with "a" and "az" definite articles and read
; about the usage of different Hungarian definite articles on this page.
;
; For conditions of use and distribution see Readme.htm file contained in the
; Hungarian Inno Setup messages package available on the above home page.
;
; *** Inno Setup 5.1.11+ verzi� magyar �zenetek "a(z)" hat�rozott n�vel�kkel ***
; Copyright (C) 1999-2012 P�l Korn�l
; Minden jog fenntartva.
; E-mail: kornelpal@gmail.com
; Magyar Inno Setup oldal: http://www.palkornel.hu/innosetup
;
; Az oldalr�l let�lthet� az "a" �s az "az" n�vel�ket tartalmaz� v�ltozat, �s
; olvashatsz a k�l�nb�z� magyar hat�rozott n�vel�k haszn�lat�r�l is.
;
; A haszn�lat �s a tov�bbad�s felt�telei a fenti oldalr�l let�lthet� Magyar
; Inno Setup �zenetek csomagban tal�lhat� Fontos.htm f�jlban olvashat�ak.
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
LanguageName=Magyar
LanguageID=$040E
LanguageCodePage=1250
; If the language you are translating to requires special font faces or
; sizes, uncomment any of the following entries and change them accordingly.
;DialogFontName=
;DialogFontSize=8
;WelcomeFontName=Verdana
;WelcomeFontSize=12
TitleFontName=Arial CE
;TitleFontSize=29
CopyrightFontName=Arial CE
;CopyrightFontSize=8

[Messages]

; *** Application titles
SetupAppTitle=Telep�t�
SetupWindowTitle=%1 Telep�t�
UninstallAppTitle=Elt�vol�t�
UninstallAppFullTitle=%1 Elt�vol�t�

; *** Misc. common
InformationTitle=Inform�ci�k
ConfirmTitle=Meger�s�t�s
ErrorTitle=Hiba

; *** SetupLdr messages
SetupLdrStartupMessage=A(z) %1 telep�t�sre fog ker�lni. K�v�nja folytatni a telep�t�st?
LdrCannotCreateTemp=Nem lehet �tmeneti f�jlt l�trehozni. A telep�t�s megszakadt
LdrCannotExecTemp=Az �tmeneti k�nyvt�rban nem lehet f�jlt v�grehajtani. A telep�t�s megszakadt

; *** Startup error messages
LastErrorMessage=%1.%n%nHiba %2: %3
SetupFileMissing=A(z) %1 f�jl hi�nyzik a telep�t� k�nyvt�r�b�l. H�r�tsa el a hib�t, vagy szerezzen be egy �j m�solatot a programr�l.
SetupFileCorrupt=A telep�t�f�jlok megs�r�ltek. Szerezzen be egy �j m�solatot a programr�l.
SetupFileCorruptOrWrongVer=A telep�t�f�jlok megs�r�ltek, vagy nem kompatibilisek a Telep�t� jelen verzi�j�val. H�r�tsa el a hib�t, vagy szerezzen be egy �j m�solatot a programr�l.
InvalidParameter=Az egyik parancssorban �tadott param�ter �rv�nytelen:%n%n%1
SetupAlreadyRunning=A Telep�t� m�r fut.
WindowsVersionNotSupported=A program nem t�mogatja a Windows sz�m�t�g�p�n fut� verzi�j�t.
WindowsServicePackRequired=A program futtat�s�hoz %1 Service Pack %2 vagy k�s�bbi verzi� sz�ks�ges.
NotOnThisPlatform=Ez a program nem futtathat� %1 alatt.
OnlyOnThisPlatform=Ezt a programot %1 alatt kell futtatni.
OnlyOnTheseArchitectures=Ezt a programot csak a Windows k�vetkez� processzorarchitekt�r�khoz tervezett v�ltozataira lehet telep�teni:%n%n%1
MissingWOW64APIs=A Windows �n �ltal futtatott verzi�ja nem tartalmazza a Telep�t� �ltal a 64-bites telep�t�s elv�gz�s�hez ig�nyelt funkcionalit�st. A hiba elh�r�t�s�hoz a Service Pack %1 telep�t�se sz�ks�ges.
WinVersionTooLowError=A program a %1 %2 vagy k�s�bbi verzi�j�t ig�nyli.
WinVersionTooHighError=A programot nem lehet a %1 %2 vagy k�s�bbi verzi�j�ra telep�teni.
AdminPrivilegesRequired=A program telep�t�s�hez rendszergazdak�nt kell bejelentkezni.
PowerUserPrivilegesRequired=A program telep�t�s�hez rendszergazdak�nt vagy a kiemelt felhaszn�l�k csoport tagjak�nt kell bejelentkezni.
SetupAppRunningError=A Telep�t� meg�llap�totta, hogy a(z) %1 jelenleg fut.%n%nK�rem, z�rja be az �sszes p�ld�ny�t, majd a folytat�shoz kattintson az OK gombra, vagy a M�gse gombra a kil�p�shez.
UninstallAppRunningError=Az Elt�vol�t� meg�llap�totta, hogy a(z) %1 jelenleg fut.%n%nK�rem, z�rja be az �sszes p�ld�ny�t, majd a folytat�shoz kattintson az OK gombra, vagy a M�gse gombra a kil�p�shez.

; *** Misc. errors
ErrorCreatingDir=A Telep�t� nem tudta l�trehozni a(z) "%1" k�nyvt�rat
ErrorTooManyFilesInDir=Nem hozhat� l�tre f�jl a(z) "%1" k�nyvt�rban, mert az m�r t�l sok f�jlt tartalmaz

; *** Setup common messages
ExitSetupTitle=Kil�p�s a Telep�t�b�l
ExitSetupMessage=A telep�t�s m�g nem fejez�d�tt be. Ha most kil�p, a program nem ker�l telep�t�sre.%n%nA Telep�t�t k�s�bb is futtathatja a telep�t�s befejez�s�hez.%n%nKil�p a Telep�t�b�l?
AboutSetupMenuItem=&N�vjegy...
AboutSetupTitle=Telep�t� n�vjegye
AboutSetupMessage=%1 %2 verzi�%n%3%n%nAz %1 honlapja:%n%4
AboutSetupNote=
TranslatorNote=Magyar v�ltozat:%nCopyright (C) 1999-2012 P�l Korn�l%nMinden jog fenntartva.%n%nMagyar Inno Setup oldal:%nhttp://www.palkornel.hu/innosetup

; *** Buttons
ButtonBack=< &Vissza
ButtonNext=&Tov�bb >
ButtonInstall=&Telep�t�s
ButtonOK=OK
ButtonCancel=M�gse
ButtonYes=&Igen
ButtonYesToAll=Igen, &mindet
ButtonNo=&Nem
ButtonNoToAll=&Egyiket sem
ButtonFinish=&Befejez�s
ButtonBrowse=&Tall�z�s...
ButtonWizardBrowse=T&all�z�s...
ButtonNewFolder=�&j mappa

; *** "Select Language" dialog messages
SelectLanguageTitle=V�lasszon telep�t�si nyelvet
SelectLanguageLabel=V�lassza ki a telep�t�s sor�n haszn�land� nyelvet:

; *** Common wizard text
ClickNext=A folytat�shoz kattintson a Tov�bb gombra, vagy a M�gse gombra a Telep�t�b�l t�rt�n� kil�p�shez.
BeveledLabel=
BrowseDialogTitle=Tall�z�s a mapp�k k�z�tt
BrowseDialogLabel=V�lasszon egy mapp�t az al�bbi list�b�l, majd kattintson az OK gombra.
NewFolderName=�j mappa

; *** "Welcome" wizard page
WelcomeLabel1=�dv�zli a(z) [name] Telep�t�var�zsl�.
WelcomeLabel2=A(z) [name/ver] a sz�m�t�g�p�re fog ker�lni.%n%nA telep�t�s folytat�sa el�tt aj�nlott minden m�s fut� alkalmaz�st bez�rni.

; *** "Password" wizard page
WizardPassword=Jelsz�
PasswordLabel1=Ez a telep�t�s jelsz�val van v�dve.
PasswordLabel3=Adja meg a jelsz�t, majd a folytat�shoz kattintson a Tov�bb gombra. A jelszavakban a kis- �s a nagybet�k k�l�nb�z�nek sz�m�tanak.
PasswordEditLabel=&Jelsz�:
IncorrectPassword=A megadott jelsz� helytelen. Pr�b�lja �jra.

; *** "License Agreement" wizard page
WizardLicense=Licencszerz�d�s
LicenseLabel=Olvassa el a k�vetkez� fontos inform�ci�kat a folytat�s el�tt.
LicenseLabel3=K�rem, olvassa el az al�bbi licencszerz�d�st. El kell fogadnia a szerz�d�s felt�teleit a telep�t�s folytat�sa el�tt.
LicenseAccepted=&Elfogadom a szerz�d�st
LicenseNotAccepted=&Nem fogadom el a szerz�d�st

; *** "Information" wizard pages
WizardInfoBefore=Inform�ci�k
InfoBeforeLabel=Olvassa el a k�vetkez� fontos inform�ci�kat a folytat�s el�tt.
InfoBeforeClickLabel=Ha felk�sz�lt a telep�t�s folytat�s�ra, kattintson a Tov�bb gombra.
WizardInfoAfter=Inform�ci�k
InfoAfterLabel=Olvassa el a k�vetkez� fontos inform�ci�kat a folytat�s el�tt.
InfoAfterClickLabel=Ha felk�sz�lt a telep�t�s folytat�s�ra, kattintson a Tov�bb gombra.

; *** "User Information" wizard page
WizardUserInfo=Felhaszn�l� adatai
UserInfoDesc=K�rem, adja meg az adatait.
UserInfoName=&Felhaszn�l�n�v:
UserInfoOrg=&Szervezet:
UserInfoSerial=&Sorozatsz�m:
UserInfoNameRequired=Meg kell adnia egy nevet.

; *** "Select Destination Location" wizard page
WizardSelectDir=V�lasszon telep�t�si helyet
SelectDirDesc=Hova ker�lj�n telep�t�sre a(z) [name]?
SelectDirLabel3=A Telep�t� a(z) [name] alkalmaz�st a k�vetkez� mapp�ba fogja telep�teni.
SelectDirBrowseLabel=A folytat�shoz kattintson a Tov�bb gombra. M�sik mappa kiv�laszt�s�hoz kattintson a Tall�z�s gombra.
DiskSpaceMBLabel=Legal�bb [mb] MB szabad lemezter�letre van sz�ks�g.
CannotInstallToNetworkDrive=A Telep�t� nem tud h�l�zati meghajt�ra telep�teni.
CannotInstallToUNCPath=A Telep�t� nem tud h�l�zati UNC el�r�si �tra telep�teni.
InvalidPath=Teljes �tvonalat �rjon be a meghajt� bet�jel�vel; p�ld�ul:%n%nC:\Alkalmaz�s%n%nvagy egy h�l�zati �tvonalat a k�vetkez� alakban:%n%n\\kiszolg�l�\megoszt�s
InvalidDrive=A kiv�lasztott meghajt� vagy h�l�zati megoszt�s nem l�tezik vagy nem �rhet� el. V�lasszon m�sikat.
DiskSpaceWarningTitle=Nincs el�g szabad lemezter�let a meghajt�n
DiskSpaceWarning=A Telep�t�nek legal�bb %1 KB szabad lemezter�letre van sz�ks�ge, de a kiv�lasztott meghajt�n csak %2 KB �ll rendelkez�sre.%n%nMindenk�ppen folytatni k�v�nja?
DirNameTooLong=A mappan�v vagy az �tvonal t�l hossz�.
InvalidDirName=A mappan�v �rv�nytelen.
BadDirName32=A mappanevekben nem szerepelhetnek a k�vetkez� karakterek:%n%n%1
DirExistsTitle=A mappa m�r l�tezik
DirExists=A k�vetkez� mappa m�r l�tezik:%n%n%1 %n%nEbbe a mapp�ba k�v�nja telep�teni a programot?
DirDoesntExistTitle=A mappa nem l�tezik
DirDoesntExist= A k�vetkez� mappa nem l�tezik:%n%n%1%n%nL�tre k�v�nja hozni a mapp�t?

; *** "Select Components" wizard page
WizardSelectComponents=�sszetev�k kiv�laszt�sa
SelectComponentsDesc=Mely �sszetev�k ker�ljenek telep�t�sre?
SelectComponentsLabel2=V�lassza ki a telep�tend� �sszetev�ket; t�r�lje a telep�teni nem k�v�nt �sszetev�ket. Kattintson a Tov�bb gombra, ha k�szen �ll a folytat�sra.
FullInstallation=Teljes telep�t�s
; if possible don't translate 'Compact' as 'Minimal' (I mean 'Minimal' in your language)
CompactInstallation=Szok�sos telep�t�s
CustomInstallation=Egy�ni telep�t�s
NoUninstallWarningTitle=L�tez� �sszetev�
NoUninstallWarning=A Telep�t� meg�llap�totta, hogy a k�vetkez� �sszetev�k m�r telep�tve vannak a sz�m�t�g�p�re:%n%n%1%n%nEzen �sszetev�k kijel�l�s�nek t�rl�se nem t�vol�tja el azokat a sz�m�t�g�p�r�l.%n%nMindenk�ppen folytatja?
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceMBLabel=A jelenlegi kijel�l�s legal�bb [mb] MB lemezter�letet ig�nyel.

; *** "Select Additional Tasks" wizard page
WizardSelectTasks=Jel�lj�n ki kieg�sz�t� feladatokat
SelectTasksDesc=Mely kieg�sz�t� feladatok ker�ljenek v�grehajt�sra?
SelectTasksLabel2=Jel�lje ki, mely kieg�sz�t� feladatokat hajtsa v�gre a Telep�t� a(z) [name] telep�t�se sor�n, majd kattintson a Tov�bb gombra.

; *** "Select Start Menu Folder" wizard page
WizardSelectProgramGroup=V�lasszon mapp�t a Start men�ben
SelectStartMenuFolderDesc=Hova helyezze a Telep�t� a program parancsikonjait?
SelectStartMenuFolderLabel3=A Telep�t� a program parancsikonjait a Start men� k�vetkez� mapp�j�ban fogja l�trehozni.
SelectStartMenuFolderBrowseLabel=A folytat�shoz kattintson a Tov�bb gombra. M�sik mappa kiv�laszt�s�hoz kattintson a Tall�z�s gombra.
MustEnterGroupName=Meg kell adnia egy mappanevet.
GroupNameTooLong=A mappan�v vagy az �tvonal t�l hossz�.
InvalidGroupName=A mappan�v �rv�nytelen.
BadGroupName=A mappa nev�ben nem szerepelhetnek a k�vetkez� karakterek:%n%n%1
NoProgramGroupCheck2=&Ne hozzon l�tre mapp�t a Start men�ben

; *** "Ready to Install" wizard page
WizardReady=A Telep�t� felk�sz�lt
ReadyLabel1=A Telep�t� felk�sz�lt a(z) [name] sz�m�t�g�p�re t�rt�n� telep�t�s�re.
ReadyLabel2a=Kattintson a Telep�t�s gombra a folytat�shoz, vagy a Vissza gombra a be�ll�t�sok �ttekint�s�hez, megv�ltoztat�s�hoz.
ReadyLabel2b=Kattintson a Telep�t�s gombra a folytat�shoz.
ReadyMemoUserInfo=Felhaszn�l� adatai:
ReadyMemoDir=Telep�t�s helye:
ReadyMemoType=Telep�t�s t�pusa:
ReadyMemoComponents=V�lasztott �sszetev�k:
ReadyMemoGroup=Start men� mapp�ja:
ReadyMemoTasks=Kieg�sz�t� feladatok:

; *** "Preparing to Install" wizard page
WizardPreparing=Felk�sz�l�s a telep�t�sre
PreparingDesc=A Telep�t� felk�sz�l a(z) [name] sz�m�t�g�p�re t�rt�n� telep�t�s�re.
PreviousInstallNotCompleted=Egy kor�bbi program telep�t�se/elt�vol�t�sa nem fejez�d�tt be. �jra kell ind�tania a sz�m�t�g�p�t a m�sik telep�t�s befejez�s�hez.%n%nA sz�m�t�g�pe �jraind�t�sa ut�n ism�t futtassa a Telep�t�t a(z) [name] telep�t�s�nek befejez�s�hez.
CannotContinue=A telep�t�s nem folytathat�. A kil�p�shez kattintson a M�gse gombra.
ApplicationsFound=A k�vetkez� alkalmaz�sok olyan f�jlokat haszn�lnak, amelyeket a Telep�t�nek friss�teni kell. Aj�nlott, hogy enged�lyezze a Telep�t�nek ezen alkalmaz�sok automatikus bez�r�s�t.
ApplicationsFound2=A k�vetkez� alkalmaz�sok olyan f�jlokat haszn�lnak, amelyeket a Telep�t�nek friss�teni kell. Aj�nlott, hogy enged�lyezze a Telep�t�nek ezen alkalmaz�sok automatikus bez�r�s�t. A telep�t�s befejez�se ut�n a Telep�t� megk�s�rli az alkalmaz�sok �jraind�t�s�t.
CloseApplications=&Alkalmaz�sok automatikus bez�r�sa
DontCloseApplications=&Ne z�rja be az alkalmaz�sokat
ErrorCloseApplications=A Telep�t� nem tudott minden alkalmaz�st automatikusan bez�rni. A folytat�s el�tt aj�nlott minden, a Telep�t� �ltal friss�tend� f�jlokat haszn�l� alkalmaz�st bez�rni.

; *** "Installing" wizard page
WizardInstalling=Telep�t�s �llapota
InstallingLabel=Legyen t�relemmel, am�g a(z) [name] sz�m�t�g�p�re t�rt�n� telep�t�se folyik.

; *** "Setup Completed" wizard page
FinishedHeadingLabel=A(z) [name] Telep�t�var�zsl� befejez�se
FinishedLabelNoIcons=A(z) [name] telep�t�se befejez�d�tt.
FinishedLabel=A(z) [name] telep�t�se befejez�d�tt. Az alkalmaz�st a l�trehozott ikonok kiv�laszt�s�val ind�thatja.
ClickFinish=Kattintson a Befejez�s gombra a Telep�t�b�l t�rt�n� kil�p�shez.
FinishedRestartLabel=A(z) [name] telep�t�s�nek befejez�s�hez �jra kell ind�tani a sz�m�t�g�pet. �jraind�tja most?
FinishedRestartMessage=A(z) [name] telep�t�s�nek befejez�s�hez �jra kell ind�tani a sz�m�t�g�pet.%n%n�jraind�tja most?
ShowReadmeCheck=Igen, szeretn�m elolvasni a FONTOS f�jlt
YesRadio=&Igen, �jraind�tom
NoRadio=&Nem, k�s�bb ind�tom �jra
; used for example as 'Run MyProg.exe'
RunEntryExec=%1 futtat�sa
; used for example as 'View Readme.txt'
RunEntryShellExec=%1 megtekint�se

; *** "Setup Needs the Next Disk" stuff
ChangeDiskTitle=A Telep�t�nek sz�ks�ge van a k�vetkez� lemezre
SelectDiskLabel2=Helyezze be a(z) %1. lemezt �s kattintson az OK gombra.%n%nHa a f�jlok a lemez egy a megjelen�tett�l k�l�nb�z� mapp�j�ban tal�lhat�k, �rja be a helyes �tvonalat vagy kattintson a Tall�z�s gombra.
PathLabel=�&tvonal:
FileNotInDir2=A(z) "%1" f�jl nem tal�lhat� a k�vetkez� helyen: "%2". Helyezze be a megfelel� lemezt vagy v�lasszon egy m�sik mapp�t.
SelectDirectoryLabel=Adja meg a k�vetkez� lemez hely�t.

; *** Installation phase messages
SetupAborted=A telep�t�s nem fejez�d�tt be.%n%nH�r�tsa el a hib�t, �s futtassa �jra a Telep�t�t.
EntryAbortRetryIgnore=Kil�p�s: megszak�t�s, Ism�t: megism�tl�s, Tov�bb: folytat�s

; *** Installation status messages
StatusClosingApplications=Alkalmaz�sok bez�r�sa...
StatusCreateDirs=K�nyvt�rak l�trehoz�sa...
StatusExtractFiles=F�jlok kibont�sa...
StatusCreateIcons=Parancsikonok l�trehoz�sa...
StatusCreateIniEntries=INI bejegyz�sek l�trehoz�sa...
StatusCreateRegistryEntries=Rendszerle�r� bejegyz�sek l�trehoz�sa...
StatusRegisterFiles=F�jlok regisztr�l�sa...
StatusSavingUninstall=Elt�vol�t� inform�ci�k ment�se...
StatusRunProgram=Telep�t�s befejez�se...
StatusRestartingApplications=Alkalmaz�sok �jraind�t�sa...
StatusRollback=V�ltoztat�sok visszavon�sa...

; *** Misc. errors
ErrorInternal2=Bels� hiba: %1
ErrorFunctionFailedNoCode=Sikertelen %1
ErrorFunctionFailed=Sikertelen %1; k�d: %2
ErrorFunctionFailedWithMessage=Sikertelen %1; k�d: %2.%n%3
ErrorExecutingProgram=Nem hajthat� v�gre a f�jl:%n%1

; *** Registry errors
ErrorRegOpenKey=Nem nyithat� meg a rendszerle�r� kulcs:%n%1\%2
ErrorRegCreateKey=Nem hozhat� l�tre a rendszerle�r� kulcs:%n%1\%2
ErrorRegWriteKey=Nem m�dos�that� a rendszerle�r� kulcs:%n%1\%2

; *** INI errors
ErrorIniEntry=Hiba az INI bejegyz�s l�trehoz�sa k�zben a(z) "%1" f�jlban.

; *** File copying errors
FileAbortRetryIgnore=Kil�p�s: megszak�t�s, Ism�t: megism�tl�s, Tov�bb: a f�jl �tl�p�se (nem aj�nlott)
FileAbortRetryIgnore2=Kil�p�s: megszak�t�s, Ism�t: megism�tl�s, Tov�bb: folytat�s (nem aj�nlott)
SourceIsCorrupted=A forr�sf�jl megs�r�lt
SourceDoesntExist=A(z) "%1" forr�sf�jl nem l�tezik
ExistingFileReadOnly=A f�jl csak olvashat�k�nt van jel�lve.%n%nKil�p�s: megszak�t�s, Ism�t: csak olvashat� jel�l�s megsz�ntet�se, �s megism�tl�s, Tov�bb: a f�jl �tl�p�se (nem aj�nlott)
ErrorReadingExistingDest=Hiba l�pett fel a f�jl olvas�sa k�zben:
FileExists=A f�jl m�r l�tezik.%n%nFel�l k�v�nja �rni?
ExistingFileNewer=A l�tez� f�jl �jabb a telep�t�sre ker�l�n�l. Aj�nlott a l�tez� f�jl megtart�sa.%n%nMeg k�v�nja tartani a l�tez� f�jlt?
ErrorChangingAttr=Hiba l�pett fel a f�jl attrib�tum�nak m�dos�t�sa k�zben:
ErrorCreatingTemp=Hiba l�pett fel a f�jl telep�t�si k�nyvt�rban t�rt�n� l�trehoz�sa k�zben:
ErrorReadingSource=Hiba l�pett fel a forr�sf�jl olvas�sa k�zben:
ErrorCopying=Hiba l�pett fel a f�jl m�sol�sa k�zben:
ErrorReplacingExistingFile=Hiba l�pett fel a l�tez� f�jl cser�je k�zben:
ErrorRestartReplace=A f�jl cser�je az �jraind�t�s ut�n sikertelen volt:
ErrorRenamingTemp=Hiba l�pett fel f�jl telep�t�si k�nyvt�rban t�rt�n� �tnevez�se k�zben:
ErrorRegisterServer=Nem lehet regisztr�lni a DLL-t/OCX-et: %1
ErrorRegSvr32Failed=Sikertelen RegSvr32. A visszaadott k�d: %1
ErrorRegisterTypeLib=Nem lehet regisztr�lni a t�pust�rat: %1

; *** Post-installation errors
ErrorOpeningReadme=Hiba l�pett fel a FONTOS f�jl megnyit�sa k�zben.
ErrorRestartingComputer=A Telep�t� nem tudta �jraind�tani a sz�m�t�g�pet. Ind�tsa �jra k�zileg.

; *** Uninstaller messages
UninstallNotFound=A(z) "%1" f�jl nem l�tezik. Nem t�vol�that� el.
UninstallOpenError=A(z) "%1" f�jl nem nyithat� meg. Nem t�vol�that� el
UninstallUnsupportedVer=A(z) "%1" elt�vol�t�si napl�f�jl form�tum�t nem tudja felismerni az elt�vol�t� jelen verzi�ja. Az elt�vol�t�s nem folytathat�
UninstallUnknownEntry=Egy ismeretlen bejegyz�s (%1) tal�lhat� az elt�vol�t�si napl�f�jlban
ConfirmUninstall=Biztosan el k�v�nja t�vol�tani a(z) %1 programot �s minden �sszetev�j�t?
UninstallOnlyOnWin64=Ezt a telep�t�st csak 64-bites Windowson lehet elt�vol�tani.
OnlyAdminCanUninstall=Ezt a telep�t�st csak adminisztr�ci�s jogokkal rendelkez� felhaszn�l� t�vol�thatja el.
UninstallStatusLabel=Legyen t�relemmel, am�g a(z) %1 sz�m�t�g�p�r�l t�rt�n� elt�vol�t�sa befejez�dik.
UninstalledAll=A(z) %1 sikeresen el lett t�vol�tva a sz�m�t�g�pr�l.
UninstalledMost=A(z) %1 elt�vol�t�sa befejez�d�tt.%n%nN�h�ny elemet nem lehetetett elt�vol�tani. T�r�lje k�zileg.
UninstalledAndNeedsRestart=A(z) %1 elt�vol�t�s�nak befejez�s�hez �jra kell ind�tania a sz�m�t�g�p�t.%n%n�jraind�tja most?
UninstallDataCorrupted=A(z) "%1" f�jl s�r�lt. Nem t�vol�that� el.

; *** Uninstallation phase messages
ConfirmDeleteSharedFileTitle=T�rli a megosztott f�jlt?
ConfirmDeleteSharedFile2=A rendszer azt jelzi, hogy a k�vetkez� megosztott f�jlra m�r nincs sz�ks�ge egyetlen programnak sem. Elt�vol�tja a megosztott f�jlt?%n%nHa m�s programok m�g mindig haszn�lj�k a megosztott f�jlt, akkor az elt�vol�t�sa ut�n lehet, hogy nem fognak megfelel�en m�k�dni. Ha bizonytalan, v�lassza a Nemet. A f�jl megtart�sa nem okoz probl�m�t a rendszerben.
SharedFileNameLabel=F�jln�v:
SharedFileLocationLabel=Helye:
WizardUninstalling=Elt�vol�t�s �llapota
StatusUninstalling=%1 elt�vol�t�sa...

; *** Shutdown block reasons
ShutdownBlockReasonInstallingApp=%1 telep�t�se.
ShutdownBlockReasonUninstallingApp=%1 elt�vol�t�sa.

; The custom messages below aren't used by Setup itself, but if you make
; use of them in your scripts, you'll want to translate them.

[CustomMessages]

NameAndVersion=%1 %2 verzi�
AdditionalIcons=Tov�bbi ikonok:
CreateDesktopIcon=Ikon l�trehoz�sa az &Asztalon
CreateQuickLaunchIcon=Ikon l�trehoz�sa a &Gyorsind�t�s eszk�zt�ron
ProgramOnTheWeb=%1 a weben
UninstallProgram=%1 elt�vol�t�sa
LaunchProgram=%1 elind�t�sa
AssocFileExtension=A(z) %1 &t�rs�t�sa a(z) %2 f�jlkiterjeszt�ssel
AssocingFileExtension=A(z) %1 t�rs�t�sa a(z) %2 f�jlkiterjeszt�ssel...
AutoStartProgramGroupDescription=Ind�t�pult:
AutoStartProgram=%1 automatikus ind�t�sa
AddonHostProgramNotFound=A(z) %1 nem tal�lhat� a kiv�lasztott mapp�ban.%n%nMindenk�ppen folytatni k�v�nja?