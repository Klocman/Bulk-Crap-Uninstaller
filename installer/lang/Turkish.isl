; *** Inno Setup version 6.1.0+ (edited to support 5.5.3+) Turkish messages ***
; Language	"Turkce" Turkish Translate by "Ceviren"	Kaya Zeren translator@zeron.net
; To download user-contributed translations of this file, go to:
;   https://jrsoftware.org/files/istrans/
;
; Note: When translating this text, do not add periods (.) to the end of
; messages that didn't have them already, because on those messages Inno
; Setup adds the periods automatically (appending a period would result in
; two periods being displayed).

[LangOptions]
; The following three entries are very important. Be sure to read and 
; understand the '[LangOptions] section' topic in the help file.
LanguageName=T<00FC>rk<00E7>e
LanguageID=$041f
LanguageCodePage=1254
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

; *** Uygulama ba�l�klar�
SetupAppTitle=Kurulum yard�mc�s�
SetupWindowTitle=%1 - Kurulum yard�mc�s�
UninstallAppTitle=Kald�rma yard�mc�s�
UninstallAppFullTitle=%1 kald�rma yard�mc�s�

; *** �e�itli ortak metinler
InformationTitle=Bilgi
ConfirmTitle=Onay
ErrorTitle=Hata

; *** Kurulum y�kleyici iletileri
SetupLdrStartupMessage=%1 uygulamas� kurulacak. �lerlemek istiyor musunuz?
LdrCannotCreateTemp=Ge�ici dosya olu�turulamad���ndan kurulum iptal edildi
LdrCannotExecTemp=Ge�ici klas�rdeki dosya �al��t�r�lamad���ndan kurulum iptal edildi
HelpTextNote=

; *** Ba�lang�� hata iletileri
LastErrorMessage=%1.%n%nHata %2: %3
SetupFileMissing=Kurulum klas�r�nde %1 dosyas� eksik. L�tfen sorunu ��z�n ya da uygulaman�n yeni bir kopyas�yla yeniden deneyin.
SetupFileCorrupt=Kurulum dosyalar� bozulmu�. L�tfen uygulaman�n yeni bir kopyas�yla yeniden kurmay� deneyin.
SetupFileCorruptOrWrongVer=Kurulum dosyalar� bozulmu� ya da bu kurulum yard�mc�s� s�r�m� ile uyumlu de�il. L�tfen sorunu ��z�n ya da uygulaman�n yeni bir kopyas�yla yeniden kurmay� deneyin.
InvalidParameter=Komut sat�r�nda ge�ersiz bir parametre yaz�lm��:%n%n%1
SetupAlreadyRunning=Kurulum yard�mc�s� zaten �al���yor.
WindowsVersionNotSupported=Bu uygulama, bilgisayar�n�zda y�kl� olan Windows s�r�m� ile uyumlu de�il.
WindowsServicePackRequired=Bu uygulama, %1 hizmet paketi %2 ve �zerindeki s�r�mler ile �al���r.
NotOnThisPlatform=Bu uygulama, %1 �zerinde �al��maz.
OnlyOnThisPlatform=Bu uygulama, %1 �zerinde �al��t�r�lmal�d�r.
OnlyOnTheseArchitectures=Bu uygulama, yaln�zca �u i�lemci mimarileri i�in tasarlanm�� Windows s�r�mleriyle �al���r:%n%n%1
MissingWOW64APIs=�al��t�rd���n�z Windows s�r�m�, 64 bit y�kleme ger�ekle�tirmek i�in Y�kleyici taraf�ndan gerekli i�levselli�i i�ermiyor. Bu hatay� gidermek i�in Service Pack %1'i y�klemeniz gerekir.
WinVersionTooLowError=Bu uygulama i�in %1 s�r�m %2 ya da �zeri gereklidir.
WinVersionTooHighError=Bu uygulama, '%1' s�r�m '%2' ya da �zerine kurulamaz.
AdminPrivilegesRequired=Bu uygulamay� kurmak i�in Y�netici yetkileri olan bir kullan�c� ile oturum a��lm�� olmal�d�r.
PowerUserPrivilegesRequired=Bu uygulamay� kurarken, Y�netici ya da G��l� Kullan�c�lar grubundaki bir kullan�c� ile oturum a��lm�� olmas� gereklidir.
SetupAppRunningError=Kurulum yard�mc�s� %1 uygulamas�n�n �al��makta oldu�unu alg�lad�.%n%nL�tfen uygulaman�n �al��an t�m kopyalar�n� kapat�p, ilerlemek i�in Tamam, kurulum yard�mc�s�ndan ��kmak i�in �ptal �zerine t�klay�n.
UninstallAppRunningError=Kald�rma yard�mc�s�, %1 uygulamas�n�n �al��makta oldu�unu alg�lad�.%n%nL�tfen uygulaman�n �al��an t�m kopyalar�n� kapat�p, ilerlemek i�in Tamam ya da kald�rma yard�mc�s�ndan ��kmak i�in �ptal �zerine t�klay�n.

; *** Ba�lang�� sorular�
PrivilegesRequiredOverrideTitle=Kurulum kipini se�in
PrivilegesRequiredOverrideInstruction=Kurulum kipini se�in
PrivilegesRequiredOverrideText1=%1 t�m kullan�c�lar i�in (y�netici izinleri gerekir) ya da yaln�zca sizin hesab�n�z i�in kurulabilir.
PrivilegesRequiredOverrideText2=%1 yaln�zca sizin hesab�n�z i�in ya da t�m kullan�c�lar i�in (y�netici izinleri gerekir) kurulabilir.
PrivilegesRequiredOverrideAllUsers=&T�m kullan�c�lar i�in kurulsun
PrivilegesRequiredOverrideAllUsersRecommended=&T�m kullan�c�lar i�in kurulsun (�nerilir)
PrivilegesRequiredOverrideCurrentUser=&Yaln�zca ge�erli kullan�c� i�in kurulsun
PrivilegesRequiredOverrideCurrentUserRecommended=&Yaln�zca ge�erli kullan�c� i�in kurulsun (�nerilir)

; *** �e�itli hata metinleri
ErrorCreatingDir=Kurulum yard�mc�s� "%1" klas�r�n� olu�turamad�.
ErrorTooManyFilesInDir="%1" klas�r� i�inde �ok say�da dosya oldu�undan bir dosya olu�turulamad�

; *** Ortak kurulum iletileri
ExitSetupTitle=Kurulum yard�mc�s�ndan ��k
ExitSetupMessage=Kurulum tamamlanmad�. �imdi ��karsan�z, uygulama kurulmayacak.%n%nKurulumu tamamlamak i�in istedi�iniz zaman kurulum yard�mc�s�n� yeniden �al��t�rabilirsiniz.%n%nKurulum yard�mc�s�ndan ��k�ls�n m�?
AboutSetupMenuItem=Kurulum h&akk�nda...
AboutSetupTitle=Kurulum hakk�nda
AboutSetupMessage=%1 %2 s�r�m�%n%3%n%n%1 ana sayfa:%n%4
AboutSetupNote=
TranslatorNote=

; *** D��meler
ButtonBack=< �&nceki
ButtonNext=&Sonraki >
ButtonInstall=&Kur
ButtonOK=Tamam
ButtonCancel=�ptal
ButtonYes=E&vet
ButtonYesToAll=&T�m�ne evet
ButtonNo=&Hay�r
ButtonNoToAll=T�m�ne ha&y�r
ButtonFinish=&Bitti
ButtonBrowse=&G�z at...
ButtonWizardBrowse=G�z a&t...
ButtonNewFolder=Ye&ni klas�r olu�tur

; *** "Kurulum dilini se�in" sayfas� iletileri
SelectLanguageTitle=Kurulum Yard�mc�s� dilini se�in
SelectLanguageLabel=Kurulum s�resince kullan�lacak dili se�in.

; *** Ortak metinler
ClickNext=�lerlemek i�in Sonraki, ��kmak i�in �ptal �zerine t�klay�n.
BeveledLabel=
BrowseDialogTitle=Klas�re g�z at
BrowseDialogLabel=A�a��daki listeden bir klas�r se�ip, Tamam �zerine t�klay�n.
NewFolderName=Yeni klas�r 

; *** "Kar��lama" sayfas�
WelcomeLabel1=[name] Kurulum yard�mc�s�na ho� geldiniz.
WelcomeLabel2=Bilgisayar�n�za [name/ver] uygulamas� kurulacak.%n%n�lerlemeden �nce �al��an di�er t�m uygulamalar� kapatman�z �nerilir.

; *** "Parola" sayfas�
WizardPassword=Parola
PasswordLabel1=Bu kurulum parola korumal�d�r.
PasswordLabel3=L�tfen parolay� yaz�n ve ilerlemek i�in Sonraki �zerine t�klay�n. Parolalar b�y�k k���k harflere duyarl�d�r.
PasswordEditLabel=&Parola:
IncorrectPassword=Yazd���n�z parola do�ru de�il. L�tfen yeniden deneyin.

; *** "Lisans anla�mas�" sayfas�
WizardLicense=Lisans anla�mas�
LicenseLabel=L�tfen ilerlemeden �nce a�a��daki �nemli bilgileri okuyun.
LicenseLabel3=L�tfen a�a��daki lisans anla�mas�n� okuyun. Uygulamay� kurmak i�in bu anla�may� kabul etmelisiniz.
LicenseAccepted=Anla�may� kabul &ediyorum.
LicenseNotAccepted=Anla�may� kabul et&miyorum.

; *** "Bilgiler" sayfas�
WizardInfoBefore=Bilgiler
InfoBeforeLabel=L�tfen ilerlemeden �nce a�a��daki �nemli bilgileri okuyun.
InfoBeforeClickLabel=Uygulamay� kurmaya haz�r oldu�unuzda Sonraki �zerine t�klay�n.
WizardInfoAfter=Bilgiler
InfoAfterLabel=L�tfen ilerlemeden �nce a�a��daki �nemli bilgileri okuyun.
InfoAfterClickLabel=Uygulamay� kurmaya haz�r oldu�unuzda Sonraki �zerine t�klay�n.

; *** "Kullan�c� bilgileri" sayfas�
WizardUserInfo=Kullan�c� bilgileri
UserInfoDesc=L�tfen bilgilerinizi yaz�n.
UserInfoName=K&ullan�c� ad�:
UserInfoOrg=Ku&rum:
UserInfoSerial=&Seri numaras�:
UserInfoNameRequired=Bir ad yazmal�s�n�z.

; *** "Kurulum konumunu se�in" sayfas�
WizardSelectDir=Kurulum konumunu se�in
SelectDirDesc=[name] nereye kurulsun?
SelectDirLabel3=[name] uygulamas� �u klas�re kurulacak.
SelectDirBrowseLabel=�lerlemek icin Sonraki �zerine t�klay�n. Farkl� bir klas�r se�mek i�in G�z at �zerine t�klay�n.
DiskSpaceGBLabel=En az [gb] GB bo� disk alan� gereklidir.
DiskSpaceMBLabel=En az [mb] MB bo� disk alan� gereklidir.
CannotInstallToNetworkDrive=Uygulama bir a� s�r�c�s� �zerine kurulamaz.
CannotInstallToUNCPath=Uygulama bir UNC yolu �zerine (\\yol gibi) kurulamaz.
InvalidPath=S�r�c� ad� ile tam yolu yazmal�s�n�z. �rnek: %n%nC:\APP%n%n ya da �u �ekilde bir UNC yolu:%n%n\\sunucu\payla��m
InvalidDrive=S�r�c� ya da UNC payla��m� yok ya da eri�ilemiyor. L�tfen ba�ka bir tane se�in.
DiskSpaceWarningTitle=Yeterli bo� disk alan� yok
DiskSpaceWarning=Kurulum i�in %1 KB bo� alan gerekli, ancak se�ilmi� s�r�c�de yaln�zca %2 KB bo� alan var.%n%nGene de ilerlemek istiyor musunuz?
DirNameTooLong=Klas�r ad� ya da yol �ok uzun.
InvalidDirName=Klas�r ad� ge�ersiz.
BadDirName32=Klas�r adlar�nda �u karakterler bulunamaz:%n%n%1
DirExistsTitle=Klas�r zaten var
DirExists=Klas�r:%n%n%1%n%nzaten var. Kurulum i�in bu klas�r� kullanmak ister misiniz?
DirDoesntExistTitle=Klas�r bulunamad�
DirDoesntExist=Klas�r:%n%n%1%n%nbulunamad�.Klas�r�n olu�turmas�n� ister misiniz?

; *** "Bile�enleri se�in" sayfas�
WizardSelectComponents=Bile�enleri se�in
SelectComponentsDesc=Hangi bile�enler kurulacak?
SelectComponentsLabel2=Kurmak istedi�iniz bile�enleri se�in; kurmak istemedi�iniz bile�enlerin i�aretini kald�r�n. �lerlemeye haz�r oldu�unuzda Sonraki �zerine t�klay�n.
FullInstallation=Tam kurulum
; Olabiliyorsa 'Compact' ifadesini kendi dilinizde 'Minimal' anlam�nda �evirmeyin
CompactInstallation=Normal kurulum
CustomInstallation=�zel kurulum
NoUninstallWarningTitle=Bile�enler zaten var
NoUninstallWarning=�u bile�enlerin bilgisayar�n�zda zaten kurulu oldu�u alg�land�:%n%n%1%n%n Bu bile�enlerin i�aretlerinin kald�r�lmas� bile�enleri kald�rmaz.%n%nGene de ilerlemek istiyor musunuz?
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceGBLabel=Se�ilmi� bile�enler i�in diskte en az [gb] GB bo� alan bulunmas� gerekli.
ComponentsDiskSpaceMBLabel=Se�ilmi� bile�enler i�in diskte en az [mb] MB bo� alan bulunmas� gerekli.

; *** "Ek i�lemleri se�in" sayfas�
WizardSelectTasks=Ek i�lemleri se�in
SelectTasksDesc=Ba�ka hangi i�lemler yap�ls�n?
SelectTasksLabel2=[name] kurulumu s�ras�nda yap�lmas�n� istedi�iniz ek i�leri se�in ve Sonraki �zerine t�klay�n.

; *** "Ba�lat men�s� klas�r�n� se�in" sayfas�
WizardSelectProgramGroup=Ba�lat men�s� klas�r�n� se�in
SelectStartMenuFolderDesc=Uygulaman�n k�sayollar� nereye eklensin?
SelectStartMenuFolderLabel3=Kurulum yard�mc�s� uygulama k�sayollar�n� a�a��daki Ba�lat men�s� klas�r�ne ekleyecek.
SelectStartMenuFolderBrowseLabel=�lerlemek i�in Sonraki �zerine t�klay�n. Farkl� bir klas�r se�mek i�in G�z at �zerine t�klay�n.
MustEnterGroupName=Bir klas�r ad� yazmal�s�n�z.
GroupNameTooLong=Klas�r ad� ya da yol �ok uzun.
InvalidGroupName=Klas�r ad� ge�ersiz.
BadGroupName=Klas�r ad�nda �u karakterler bulunamaz:%n%n%1
NoProgramGroupCheck2=Ba�lat men�s� klas�r� &olu�turulmas�n

; *** "Kurulmaya haz�r" sayfas�
WizardReady=Kurulmaya haz�r
ReadyLabel1=[name] bilgisayar�n�za kurulmaya haz�r.
ReadyLabel2a=Kuruluma ba�lamak i�in Sonraki �zerine, ayarlar� g�zden ge�irip de�i�tirmek i�in �nceki �zerine t�klay�n.
ReadyLabel2b=Kuruluma ba�lamak i�in Sonraki �zerine t�klay�n.
ReadyMemoUserInfo=Kullan�c� bilgileri:
ReadyMemoDir=Kurulum konumu:
ReadyMemoType=Kurulum t�r�:
ReadyMemoComponents=Se�ilmi� bile�enler:
ReadyMemoGroup=Ba�lat men�s� klas�r�:
ReadyMemoTasks=Ek i�lemler:

; *** TDownloadWizardPage wizard page and DownloadTemporaryFile
DownloadingLabel=Ek dosyalar indiriliyor...
ButtonStopDownload=�ndirmeyi &durdur
StopDownload=�ndirmeyi durdurmak istedi�inize emin misiniz?
ErrorDownloadAborted=�ndirme durduruldu
ErrorDownloadFailed=�ndirilemedi: %1 %2
ErrorDownloadSizeFailed=Boyut al�namad�: %1 %2
ErrorFileHash1=Dosya karmas� do�rulanamad�: %1
ErrorFileHash2=Dosya karmas� ge�ersiz: %1 olmas� gerekirken %2
ErrorProgress=Ad�m ge�ersiz: %1 / %2
ErrorFileSize=Dosya boyutu ge�ersiz: %1 olmas� gerekirken %2

; *** "Kuruluma haz�rlan�l�yor" sayfas�
WizardPreparing=Kuruluma haz�rlan�l�yor
PreparingDesc=[name] bilgisayar�n�za kurulmaya haz�rlan�yor.
PreviousInstallNotCompleted=�nceki uygulama kurulumu ya da kald�r�lmas� tamamlanmam��. Bu kurulumun tamamlanmas� i�in bilgisayar�n�z� yeniden ba�latmal�s�n�z.%n%nBilgisayar�n�z� yeniden ba�latt�ktan sonra i�lemi tamamlamak i�in [name] kurulum yard�mc�s�n� yeniden �al��t�r�n.
CannotContinue=Kurulum yap�lamad�. ��kmak i�in �ptal �zerine t�klay�n.
ApplicationsFound=Kurulum yard�mc�s� taraf�ndan g�ncellenmesi gereken dosyalar, �u uygulamalar taraf�ndan kullan�yor. Kurulum yard�mc�s�n�n bu uygulamalar� otomatik olarak kapatmas�na izin vermeniz �nerilir.
ApplicationsFound2=Kurulum yard�mc�s� taraf�ndan g�ncellenmesi gereken dosyalar, �u uygulamalar taraf�ndan kullan�yor. Kurulum yard�mc�s�n�n bu uygulamalar� otomatik olarak kapatmas�na izin vermeniz �nerilir. Kurulum tamamland�ktan sonra, uygulamalar yeniden ba�lat�lmaya �al���lacak.
CloseApplications=&Uygulamalar kapat�ls�n
DontCloseApplications=Uygulamalar &kapat�lmas�n
ErrorCloseApplications=Kurulum yard�mc�s� uygulamalar� kapatamad�. Kurulum yard�mc�s� taraf�ndan g�ncellenmesi gereken dosyalar� kullanan uygulamalar� el ile kapatman�z �nerilir.
PrepareToInstallNeedsRestart=Kurulum i�in bilgisayar�n yeniden ba�lat�lmas� gerekiyor. Bilgisayar� yeniden ba�latt�ktan sonra [name] kurulumunu tamamlamak i�in kurulum yard�mc�s�n� yeniden �al��t�r�n.%n%nBilgisayar� �imdi yeniden ba�latmak ister misiniz?

; *** "Kuruluyor" sayfas�
WizardInstalling=Kuruluyor
InstallingLabel=L�tfen [name] bilgisayar�n�za kurulurken bekleyin.

; *** "Kurulum Tamamland�" sayfas�
FinishedHeadingLabel=[name] kurulum yard�mc�s� tamamlan�yor
FinishedLabelNoIcons=Bilgisayar�n�za [name] kurulumu tamamland�.
FinishedLabel=Bilgisayar�n�za [name] kurulumu tamamland�. Simgeleri y�klemeyi se�tiyseniz, simgelere t�klayarak uygulamay� ba�latabilirsiniz.
ClickFinish=Kurulum yard�mc�s�ndan ��kmak i�in Bitti �zerine t�klay�n.
FinishedRestartLabel=[name] kurulumunun tamamlanmas� i�in, bilgisayar�n�z yeniden ba�lat�lmal�. �imdi yeniden ba�latmak ister misiniz?
FinishedRestartMessage=[name] kurulumunun tamamlanmas� i�in, bilgisayar�n�z yeniden ba�lat�lmal�.%n%n�imdi yeniden ba�latmak ister misiniz?
ShowReadmeCheck=Evet README dosyas� g�r�nt�lensin
YesRadio=&Evet, bilgisayar �imdi yeniden ba�lat�ls�n
NoRadio=&Hay�r, bilgisayar� daha sonra yeniden ba�lataca��m
; used for example as 'Run MyProg.exe'
RunEntryExec=%1 �al��t�r�ls�n
; used for example as 'View Readme.txt'
RunEntryShellExec=%1 g�r�nt�lensin

; *** "Kurulum i�in s�radaki disk gerekli" iletileri
ChangeDiskTitle=Kurulum yard�mc�s� s�radaki diske gerek duyuyor
SelectDiskLabel2=L�tfen %1 numaral� diski tak�p Tamam �zerine t�klay�n.%n%nDiskteki dosyalar a�a��dakinden farkl� bir klas�rde bulunuyorsa, do�ru yolu yaz�n ya da G�z at �zerine t�klayarak do�ru klas�r� se�in.
PathLabel=&Yol:
FileNotInDir2="%1" dosyas� "%2" i�inde bulunamad�. L�tfen do�ru diski tak�n ya da ba�ka bir klas�r se�in.
SelectDirectoryLabel=L�tfen sonraki diskin konumunu belirtin.

; *** Kurulum a�amas� iletileri
SetupAborted=Kurulum tamamlanamad�.%n%nL�tfen sorunu d�zelterek kurulum yard�mc�s�n� yeniden �al��t�r�n.
EntryAbortRetryIgnore=��k��: iptal et, Tekrar: tekrarla, Sonraki: devam et
AbortRetryIgnoreSelectAction=Yap�lacak i�lemi se�in
AbortRetryIgnoreRetry=&Yeniden denensin
AbortRetryIgnoreIgnore=&Sorun yok say�l�p ilerlensin
AbortRetryIgnoreCancel=Kurulum iptal edilsin

; *** Kurulum durumu iletileri
StatusClosingApplications=Uygulamalar kapat�l�yor...
StatusCreateDirs=Klas�rler olu�turuluyor...
StatusExtractFiles=Dosyalar ay�klan�yor...
StatusCreateIcons=K�sayollar olu�turuluyor...
StatusCreateIniEntries=INI kay�tlar� olu�turuluyor...
StatusCreateRegistryEntries=Kay�t Defteri kay�tlar� olu�turuluyor...
StatusRegisterFiles=Dosyalar kaydediliyor...
StatusSavingUninstall=Kald�rma bilgileri kaydediliyor...
StatusRunProgram=Kurulum tamamlan�yor...
StatusRestartingApplications=Uygulamalar yeniden ba�lat�l�yor...
StatusRollback=De�i�iklikler geri al�n�yor...

; *** �e�itli hata iletileri
ErrorInternal2=�� hata: %1
ErrorFunctionFailedNoCode=%1 tamamlanamad�.
ErrorFunctionFailed=%1 tamamlanamad�; kod %2
ErrorFunctionFailedWithMessage=%1 tamamlanamad�; kod %2.%n%3
ErrorExecutingProgram=�u dosya y�r�t�lemedi:%n%1

; *** Kay�t defteri hatalar�
ErrorRegOpenKey=Kay�t defteri anahtar� a��l�rken bir sorun ��kt�:%n%1%2
ErrorRegCreateKey=Kay�t defteri anahtar� eklenirken bir sorun ��kt�:%n%1%2
ErrorRegWriteKey=Kay�t defteri anahtar� yaz�l�rken bir sorun ��kt�:%n%1%2

; *** INI hatalar�
ErrorIniEntry="%1" dosyas�na INI kayd� eklenirken bir sorun ��kt�.

; *** Dosya kopyalama hatalar�
FileAbortRetryIgnore=&Bu dosya atlans�n (�nerilmez)
FileAbortRetryIgnore2=&Sorun yok say�l�p ilerlensin (�nerilmez)
FileAbortRetryIgnoreSkipNotRecommended=&Bu dosya atlans�n (�nerilmez)
FileAbortRetryIgnoreIgnoreNotRecommended=&Sorun yok say�l�p ilerlensin (�nerilmez)
SourceIsCorrupted=Kaynak dosya bozulmu�
SourceDoesntExist="%1" kaynak dosyas� bulunamad�
ExistingFileReadOnly=Var olan dosya salt okunabilir olarak i�aretlenmi� oldu�undan �zerine yaz�lamad�.
ExistingFileReadOnly2=Var olan dosya salt okunabilir olarak i�aretlenmi� oldu�undan �zerine yaz�lamad�.
ExistingFileReadOnlyRetry=&Salt okunur i�areti kald�r�l�p yeniden denensin
ExistingFileReadOnlyKeepExisting=&Var olan dosya korunsun
ErrorReadingExistingDest=Var olan dosya okunmaya �al���l�rken bir sorun ��kt�.
FileExists=Dosya zaten mevcut.%n%n�zerine yazmak ister misiniz?
ExistingFileNewer=Mevcut dosya, kurulmakta olan dosyadan daha yenidir. Mevcut dosyay� saklaman�z �nerilir.%n%nMevcut dosyay� saklamak istiyor musunuz?

FileExistsSelectAction=Yap�lacak i�lemi se�in
FileExists2=Dosya zaten var.
FileExistsOverwriteExisting=&Var olan dosyan�n �zerine yaz�ls�n
FileExistsKeepExisting=Var &olan dosya korunsun
FileExistsOverwriteOrKeepAll=&Sonraki �ak��malarda da bu i�lem yap�ls�n
ExistingFileNewerSelectAction=Yap�lacak i�lemi se�in
ExistingFileNewer2=Var olan dosya, kurulum yard�mc�s� taraf�ndan yaz�lmaya �al���landan daha yeni.
ExistingFileNewerOverwriteExisting=&Var olan dosyan�n �zerine yaz�ls�n
ExistingFileNewerKeepExisting=Var &olan dosya korunsun (�nerilir)
ExistingFileNewerOverwriteOrKeepAll=&Sonraki �ak��malarda bu i�lem yap�ls�n
ErrorChangingAttr=Var olan dosyan�n �znitelikleri de�i�tirilirken bir sorun ��kt�:
ErrorCreatingTemp=Kurulum klas�r�nde bir dosya olu�turulurken sorun ��kt�:
ErrorReadingSource=Kaynak dosya okunurken sorun ��kt�:
ErrorCopying=Dosya kopyalan�rken sorun ��kt�:
ErrorReplacingExistingFile=Var olan dosya de�i�tirilirken sorun ��kt�:
ErrorRestartReplace=Yeniden ba�latmada �zerine yaz�lamad�:
ErrorRenamingTemp=Kurulum klas�r�ndeki bir dosyan�n ad� de�i�tirilirken sorun ��kt�:
ErrorRegisterServer=DLL/OCX kay�t edilemedi: %1
ErrorRegSvr32Failed=RegSvr32 i�lemi �u kod ile tamamlanamad�: %1
ErrorRegisterTypeLib=T�r kitapl��� kay�t defterine eklenemedi: %1

; *** Kald�rma s�ras�nda g�r�nt�lenecek ad i�aretleri
; used for example as 'My Program (32-bit)'
UninstallDisplayNameMark=%1 (%2)
; used for example as 'My Program (32-bit, All users)'
UninstallDisplayNameMarks=%1 (%2, %3)
UninstallDisplayNameMark32Bit=32 bit
UninstallDisplayNameMark64Bit=64 bit
UninstallDisplayNameMarkAllUsers=T�m kullan�c�lar
UninstallDisplayNameMarkCurrentUser=Ge�erli kullan�c�

; *** Kurulum sonras� hatalar�
ErrorOpeningReadme=README dosyas� a��l�rken sorun ��kt�.
ErrorRestartingComputer=Kurulum yard�mc�s� bilgisayar�n�z� yeniden ba�latam�yor. L�tfen bilgisayar�n�z� yeniden ba�lat�n.

; *** Kald�rma yard�mc�s� iletileri
UninstallNotFound="%1" dosyas� bulunamad�. Uygulama kald�r�lam�yor.
UninstallOpenError="%1" dosyas� a��lamad�. Uygulama kald�r�lam�yor.
UninstallUnsupportedVer="%1" uygulama kald�rma g�nl�k dosyas�n�n bi�imi, bu kald�rma yard�mc�s� s�r�m� taraf�ndan anla��lamad�. Uygulama kald�r�lam�yor.
UninstallUnknownEntry=Kald�rma g�nl���nde bilinmeyen bir kay�t (%1) bulundu.
ConfirmUninstall=%1 uygulamas�n� t�m bile�enleri ile birlikte tamamen kald�rmak istedi�inize emin misiniz?
UninstallOnlyOnWin64=Bu kurulum yaln�zca 64 bit Windows �zerinden kald�r�labilir.
OnlyAdminCanUninstall=Bu kurulum yaln�zca y�netici yetkileri olan bir kullan�c� taraf�ndan kald�r�labilir.
UninstallStatusLabel=L�tfen %1 uygulamas� bilgisayar�n�zdan kald�r�l�rken bekleyin.
UninstalledAll=%1 uygulamas� bilgisayar�n�zdan kald�r�ld�.
UninstalledMost=%1 uygulamas� kald�r�ld�.%n%nBaz� bile�enler kald�r�lamad�. Bunlar� el ile silebilirsiniz.
UninstalledAndNeedsRestart=%1 kald�rma i�leminin tamamlanmas� i�in bilgisayar�n�z�n yeniden ba�lat�lmas� gerekli.%n%n�imdi yeniden ba�latmak ister misiniz?
UninstallDataCorrupted="%1" dosyas� bozulmu�. Kald�r�lam�yor.

; *** Kald�rma a�amas� iletileri
ConfirmDeleteSharedFileTitle=Payla��lan dosya silinsin mi?
ConfirmDeleteSharedFile2=Sisteme g�re, payla��lan �u dosya ba�ka bir uygulama taraf�ndan kullan�lm�yor ve kald�r�labilir. Bu payla��lm�� dosyay� silmek ister misiniz?%n%nBu dosya, ba�ka herhangi bir uygulama taraf�ndan kullan�l�yor ise, silindi�inde di�er uygulama d�zg�n �al��mayabilir. Emin de�ilseniz Hay�r �zerine t�klay�n. Dosyay� sisteminizde b�rakman�n bir zarar� olmaz.
SharedFileNameLabel=Dosya ad�:
SharedFileLocationLabel=Konum:
WizardUninstalling=Kald�rma durumu
StatusUninstalling=%1 kald�r�l�yor...

; *** Kapatmay� engelleme nedenleri
ShutdownBlockReasonInstallingApp=%1 kuruluyor.
ShutdownBlockReasonUninstallingApp=%1 kald�r�l�yor.

; The custom messages below aren't used by Setup itself, but if you make
; use of them in your scripts, you'll want to translate them.

[CustomMessages]

NameAndVersion=%1 %2 s�r�m�
AdditionalIcons=Ek simgeler:
CreateDesktopIcon=Masa�st� simg&esi olu�turulsun
CreateQuickLaunchIcon=H�zl� ba�lat simgesi &olu�turulsun
ProgramOnTheWeb=%1 sitesi
UninstallProgram=%1 uygulamas�n� kald�r
LaunchProgram=%1 uygulamas�n� �al��t�r
AssocFileExtension=%1 &uygulamas� ile %2 dosya uzant�s� ili�kilendirilsin
AssocingFileExtension=%1 uygulamas� ile %2 dosya uzant�s� ili�kilendiriliyor...
AutoStartProgramGroupDescription=Ba�lang��:
AutoStartProgram=%1 otomatik olarak ba�lat�ls�n
AddonHostProgramNotFound=%1 se�ti�iniz klas�rde bulunamad�.%n%nYine de ilerlemek istiyor musunuz?
