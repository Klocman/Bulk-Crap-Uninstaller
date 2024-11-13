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

; *** Uygulama baþlýklarý
SetupAppTitle=Kurulum yardýmcýsý
SetupWindowTitle=%1 - Kurulum yardýmcýsý
UninstallAppTitle=Kaldýrma yardýmcýsý
UninstallAppFullTitle=%1 kaldýrma yardýmcýsý

; *** Çeþitli ortak metinler
InformationTitle=Bilgi
ConfirmTitle=Onay
ErrorTitle=Hata

; *** Kurulum yükleyici iletileri
SetupLdrStartupMessage=%1 uygulamasý kurulacak. Ýlerlemek istiyor musunuz?
LdrCannotCreateTemp=Geçici dosya oluþturulamadýðýndan kurulum iptal edildi
LdrCannotExecTemp=Geçici klasördeki dosya çalýþtýrýlamadýðýndan kurulum iptal edildi
HelpTextNote=

; *** Baþlangýç hata iletileri
LastErrorMessage=%1.%n%nHata %2: %3
SetupFileMissing=Kurulum klasöründe %1 dosyasý eksik. Lütfen sorunu çözün ya da uygulamanýn yeni bir kopyasýyla yeniden deneyin.
SetupFileCorrupt=Kurulum dosyalarý bozulmuþ. Lütfen uygulamanýn yeni bir kopyasýyla yeniden kurmayý deneyin.
SetupFileCorruptOrWrongVer=Kurulum dosyalarý bozulmuþ ya da bu kurulum yardýmcýsý sürümü ile uyumlu deðil. Lütfen sorunu çözün ya da uygulamanýn yeni bir kopyasýyla yeniden kurmayý deneyin.
InvalidParameter=Komut satýrýnda geçersiz bir parametre yazýlmýþ:%n%n%1
SetupAlreadyRunning=Kurulum yardýmcýsý zaten çalýþýyor.
WindowsVersionNotSupported=Bu uygulama, bilgisayarýnýzda yüklü olan Windows sürümü ile uyumlu deðil.
WindowsServicePackRequired=Bu uygulama, %1 hizmet paketi %2 ve üzerindeki sürümler ile çalýþýr.
NotOnThisPlatform=Bu uygulama, %1 üzerinde çalýþmaz.
OnlyOnThisPlatform=Bu uygulama, %1 üzerinde çalýþtýrýlmalýdýr.
OnlyOnTheseArchitectures=Bu uygulama, yalnýzca þu iþlemci mimarileri için tasarlanmýþ Windows sürümleriyle çalýþýr:%n%n%1
MissingWOW64APIs=Çalýþtýrdýðýnýz Windows sürümü, 64 bit yükleme gerçekleþtirmek için Yükleyici tarafýndan gerekli iþlevselliði içermiyor. Bu hatayý gidermek için Service Pack %1'i yüklemeniz gerekir.
WinVersionTooLowError=Bu uygulama için %1 sürüm %2 ya da üzeri gereklidir.
WinVersionTooHighError=Bu uygulama, '%1' sürüm '%2' ya da üzerine kurulamaz.
AdminPrivilegesRequired=Bu uygulamayý kurmak için Yönetici yetkileri olan bir kullanýcý ile oturum açýlmýþ olmalýdýr.
PowerUserPrivilegesRequired=Bu uygulamayý kurarken, Yönetici ya da Güçlü Kullanýcýlar grubundaki bir kullanýcý ile oturum açýlmýþ olmasý gereklidir.
SetupAppRunningError=Kurulum yardýmcýsý %1 uygulamasýnýn çalýþmakta olduðunu algýladý.%n%nLütfen uygulamanýn çalýþan tüm kopyalarýný kapatýp, ilerlemek için Tamam, kurulum yardýmcýsýndan çýkmak için Ýptal üzerine týklayýn.
UninstallAppRunningError=Kaldýrma yardýmcýsý, %1 uygulamasýnýn çalýþmakta olduðunu algýladý.%n%nLütfen uygulamanýn çalýþan tüm kopyalarýný kapatýp, ilerlemek için Tamam ya da kaldýrma yardýmcýsýndan çýkmak için Ýptal üzerine týklayýn.

; *** Baþlangýç sorularý
PrivilegesRequiredOverrideTitle=Kurulum kipini seçin
PrivilegesRequiredOverrideInstruction=Kurulum kipini seçin
PrivilegesRequiredOverrideText1=%1 tüm kullanýcýlar için (yönetici izinleri gerekir) ya da yalnýzca sizin hesabýnýz için kurulabilir.
PrivilegesRequiredOverrideText2=%1 yalnýzca sizin hesabýnýz için ya da tüm kullanýcýlar için (yönetici izinleri gerekir) kurulabilir.
PrivilegesRequiredOverrideAllUsers=&Tüm kullanýcýlar için kurulsun
PrivilegesRequiredOverrideAllUsersRecommended=&Tüm kullanýcýlar için kurulsun (önerilir)
PrivilegesRequiredOverrideCurrentUser=&Yalnýzca geçerli kullanýcý için kurulsun
PrivilegesRequiredOverrideCurrentUserRecommended=&Yalnýzca geçerli kullanýcý için kurulsun (önerilir)

; *** Çeþitli hata metinleri
ErrorCreatingDir=Kurulum yardýmcýsý "%1" klasörünü oluþturamadý.
ErrorTooManyFilesInDir="%1" klasörü içinde çok sayýda dosya olduðundan bir dosya oluþturulamadý

; *** Ortak kurulum iletileri
ExitSetupTitle=Kurulum yardýmcýsýndan çýk
ExitSetupMessage=Kurulum tamamlanmadý. Þimdi çýkarsanýz, uygulama kurulmayacak.%n%nKurulumu tamamlamak için istediðiniz zaman kurulum yardýmcýsýný yeniden çalýþtýrabilirsiniz.%n%nKurulum yardýmcýsýndan çýkýlsýn mý?
AboutSetupMenuItem=Kurulum h&akkýnda...
AboutSetupTitle=Kurulum hakkýnda
AboutSetupMessage=%1 %2 sürümü%n%3%n%n%1 ana sayfa:%n%4
AboutSetupNote=
TranslatorNote=

; *** Düðmeler
ButtonBack=< Ö&nceki
ButtonNext=&Sonraki >
ButtonInstall=&Kur
ButtonOK=Tamam
ButtonCancel=Ýptal
ButtonYes=E&vet
ButtonYesToAll=&Tümüne evet
ButtonNo=&Hayýr
ButtonNoToAll=Tümüne ha&yýr
ButtonFinish=&Bitti
ButtonBrowse=&Göz at...
ButtonWizardBrowse=Göz a&t...
ButtonNewFolder=Ye&ni klasör oluþtur

; *** "Kurulum dilini seçin" sayfasý iletileri
SelectLanguageTitle=Kurulum Yardýmcýsý dilini seçin
SelectLanguageLabel=Kurulum süresince kullanýlacak dili seçin.

; *** Ortak metinler
ClickNext=Ýlerlemek için Sonraki, çýkmak için Ýptal üzerine týklayýn.
BeveledLabel=
BrowseDialogTitle=Klasöre göz at
BrowseDialogLabel=Aþaðýdaki listeden bir klasör seçip, Tamam üzerine týklayýn.
NewFolderName=Yeni klasör 

; *** "Karþýlama" sayfasý
WelcomeLabel1=[name] Kurulum yardýmcýsýna hoþ geldiniz.
WelcomeLabel2=Bilgisayarýnýza [name/ver] uygulamasý kurulacak.%n%nÝlerlemeden önce çalýþan diðer tüm uygulamalarý kapatmanýz önerilir.

; *** "Parola" sayfasý
WizardPassword=Parola
PasswordLabel1=Bu kurulum parola korumalýdýr.
PasswordLabel3=Lütfen parolayý yazýn ve ilerlemek için Sonraki üzerine týklayýn. Parolalar büyük küçük harflere duyarlýdýr.
PasswordEditLabel=&Parola:
IncorrectPassword=Yazdýðýnýz parola doðru deðil. Lütfen yeniden deneyin.

; *** "Lisans anlaþmasý" sayfasý
WizardLicense=Lisans anlaþmasý
LicenseLabel=Lütfen ilerlemeden önce aþaðýdaki önemli bilgileri okuyun.
LicenseLabel3=Lütfen aþaðýdaki lisans anlaþmasýný okuyun. Uygulamayý kurmak için bu anlaþmayý kabul etmelisiniz.
LicenseAccepted=Anlaþmayý kabul &ediyorum.
LicenseNotAccepted=Anlaþmayý kabul et&miyorum.

; *** "Bilgiler" sayfasý
WizardInfoBefore=Bilgiler
InfoBeforeLabel=Lütfen ilerlemeden önce aþaðýdaki önemli bilgileri okuyun.
InfoBeforeClickLabel=Uygulamayý kurmaya hazýr olduðunuzda Sonraki üzerine týklayýn.
WizardInfoAfter=Bilgiler
InfoAfterLabel=Lütfen ilerlemeden önce aþaðýdaki önemli bilgileri okuyun.
InfoAfterClickLabel=Uygulamayý kurmaya hazýr olduðunuzda Sonraki üzerine týklayýn.

; *** "Kullanýcý bilgileri" sayfasý
WizardUserInfo=Kullanýcý bilgileri
UserInfoDesc=Lütfen bilgilerinizi yazýn.
UserInfoName=K&ullanýcý adý:
UserInfoOrg=Ku&rum:
UserInfoSerial=&Seri numarasý:
UserInfoNameRequired=Bir ad yazmalýsýnýz.

; *** "Kurulum konumunu seçin" sayfasý
WizardSelectDir=Kurulum konumunu seçin
SelectDirDesc=[name] nereye kurulsun?
SelectDirLabel3=[name] uygulamasý þu klasöre kurulacak.
SelectDirBrowseLabel=Ýlerlemek icin Sonraki üzerine týklayýn. Farklý bir klasör seçmek için Göz at üzerine týklayýn.
DiskSpaceGBLabel=En az [gb] GB boþ disk alaný gereklidir.
DiskSpaceMBLabel=En az [mb] MB boþ disk alaný gereklidir.
CannotInstallToNetworkDrive=Uygulama bir að sürücüsü üzerine kurulamaz.
CannotInstallToUNCPath=Uygulama bir UNC yolu üzerine (\\yol gibi) kurulamaz.
InvalidPath=Sürücü adý ile tam yolu yazmalýsýnýz. Örnek: %n%nC:\APP%n%n ya da þu þekilde bir UNC yolu:%n%n\\sunucu\paylaþým
InvalidDrive=Sürücü ya da UNC paylaþýmý yok ya da eriþilemiyor. Lütfen baþka bir tane seçin.
DiskSpaceWarningTitle=Yeterli boþ disk alaný yok
DiskSpaceWarning=Kurulum için %1 KB boþ alan gerekli, ancak seçilmiþ sürücüde yalnýzca %2 KB boþ alan var.%n%nGene de ilerlemek istiyor musunuz?
DirNameTooLong=Klasör adý ya da yol çok uzun.
InvalidDirName=Klasör adý geçersiz.
BadDirName32=Klasör adlarýnda þu karakterler bulunamaz:%n%n%1
DirExistsTitle=Klasör zaten var
DirExists=Klasör:%n%n%1%n%nzaten var. Kurulum için bu klasörü kullanmak ister misiniz?
DirDoesntExistTitle=Klasör bulunamadý
DirDoesntExist=Klasör:%n%n%1%n%nbulunamadý.Klasörün oluþturmasýný ister misiniz?

; *** "Bileþenleri seçin" sayfasý
WizardSelectComponents=Bileþenleri seçin
SelectComponentsDesc=Hangi bileþenler kurulacak?
SelectComponentsLabel2=Kurmak istediðiniz bileþenleri seçin; kurmak istemediðiniz bileþenlerin iþaretini kaldýrýn. Ýlerlemeye hazýr olduðunuzda Sonraki üzerine týklayýn.
FullInstallation=Tam kurulum
; Olabiliyorsa 'Compact' ifadesini kendi dilinizde 'Minimal' anlamýnda çevirmeyin
CompactInstallation=Normal kurulum
CustomInstallation=Özel kurulum
NoUninstallWarningTitle=Bileþenler zaten var
NoUninstallWarning=Þu bileþenlerin bilgisayarýnýzda zaten kurulu olduðu algýlandý:%n%n%1%n%n Bu bileþenlerin iþaretlerinin kaldýrýlmasý bileþenleri kaldýrmaz.%n%nGene de ilerlemek istiyor musunuz?
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceGBLabel=Seçilmiþ bileþenler için diskte en az [gb] GB boþ alan bulunmasý gerekli.
ComponentsDiskSpaceMBLabel=Seçilmiþ bileþenler için diskte en az [mb] MB boþ alan bulunmasý gerekli.

; *** "Ek iþlemleri seçin" sayfasý
WizardSelectTasks=Ek iþlemleri seçin
SelectTasksDesc=Baþka hangi iþlemler yapýlsýn?
SelectTasksLabel2=[name] kurulumu sýrasýnda yapýlmasýný istediðiniz ek iþleri seçin ve Sonraki üzerine týklayýn.

; *** "Baþlat menüsü klasörünü seçin" sayfasý
WizardSelectProgramGroup=Baþlat menüsü klasörünü seçin
SelectStartMenuFolderDesc=Uygulamanýn kýsayollarý nereye eklensin?
SelectStartMenuFolderLabel3=Kurulum yardýmcýsý uygulama kýsayollarýný aþaðýdaki Baþlat menüsü klasörüne ekleyecek.
SelectStartMenuFolderBrowseLabel=Ýlerlemek için Sonraki üzerine týklayýn. Farklý bir klasör seçmek için Göz at üzerine týklayýn.
MustEnterGroupName=Bir klasör adý yazmalýsýnýz.
GroupNameTooLong=Klasör adý ya da yol çok uzun.
InvalidGroupName=Klasör adý geçersiz.
BadGroupName=Klasör adýnda þu karakterler bulunamaz:%n%n%1
NoProgramGroupCheck2=Baþlat menüsü klasörü &oluþturulmasýn

; *** "Kurulmaya hazýr" sayfasý
WizardReady=Kurulmaya hazýr
ReadyLabel1=[name] bilgisayarýnýza kurulmaya hazýr.
ReadyLabel2a=Kuruluma baþlamak için Sonraki üzerine, ayarlarý gözden geçirip deðiþtirmek için Önceki üzerine týklayýn.
ReadyLabel2b=Kuruluma baþlamak için Sonraki üzerine týklayýn.
ReadyMemoUserInfo=Kullanýcý bilgileri:
ReadyMemoDir=Kurulum konumu:
ReadyMemoType=Kurulum türü:
ReadyMemoComponents=Seçilmiþ bileþenler:
ReadyMemoGroup=Baþlat menüsü klasörü:
ReadyMemoTasks=Ek iþlemler:

; *** TDownloadWizardPage wizard page and DownloadTemporaryFile
DownloadingLabel=Ek dosyalar indiriliyor...
ButtonStopDownload=Ýndirmeyi &durdur
StopDownload=Ýndirmeyi durdurmak istediðinize emin misiniz?
ErrorDownloadAborted=Ýndirme durduruldu
ErrorDownloadFailed=Ýndirilemedi: %1 %2
ErrorDownloadSizeFailed=Boyut alýnamadý: %1 %2
ErrorFileHash1=Dosya karmasý doðrulanamadý: %1
ErrorFileHash2=Dosya karmasý geçersiz: %1 olmasý gerekirken %2
ErrorProgress=Adým geçersiz: %1 / %2
ErrorFileSize=Dosya boyutu geçersiz: %1 olmasý gerekirken %2

; *** "Kuruluma hazýrlanýlýyor" sayfasý
WizardPreparing=Kuruluma hazýrlanýlýyor
PreparingDesc=[name] bilgisayarýnýza kurulmaya hazýrlanýyor.
PreviousInstallNotCompleted=Önceki uygulama kurulumu ya da kaldýrýlmasý tamamlanmamýþ. Bu kurulumun tamamlanmasý için bilgisayarýnýzý yeniden baþlatmalýsýnýz.%n%nBilgisayarýnýzý yeniden baþlattýktan sonra iþlemi tamamlamak için [name] kurulum yardýmcýsýný yeniden çalýþtýrýn.
CannotContinue=Kurulum yapýlamadý. Çýkmak için Ýptal üzerine týklayýn.
ApplicationsFound=Kurulum yardýmcýsý tarafýndan güncellenmesi gereken dosyalar, þu uygulamalar tarafýndan kullanýyor. Kurulum yardýmcýsýnýn bu uygulamalarý otomatik olarak kapatmasýna izin vermeniz önerilir.
ApplicationsFound2=Kurulum yardýmcýsý tarafýndan güncellenmesi gereken dosyalar, þu uygulamalar tarafýndan kullanýyor. Kurulum yardýmcýsýnýn bu uygulamalarý otomatik olarak kapatmasýna izin vermeniz önerilir. Kurulum tamamlandýktan sonra, uygulamalar yeniden baþlatýlmaya çalýþýlacak.
CloseApplications=&Uygulamalar kapatýlsýn
DontCloseApplications=Uygulamalar &kapatýlmasýn
ErrorCloseApplications=Kurulum yardýmcýsý uygulamalarý kapatamadý. Kurulum yardýmcýsý tarafýndan güncellenmesi gereken dosyalarý kullanan uygulamalarý el ile kapatmanýz önerilir.
PrepareToInstallNeedsRestart=Kurulum için bilgisayarýn yeniden baþlatýlmasý gerekiyor. Bilgisayarý yeniden baþlattýktan sonra [name] kurulumunu tamamlamak için kurulum yardýmcýsýný yeniden çalýþtýrýn.%n%nBilgisayarý þimdi yeniden baþlatmak ister misiniz?

; *** "Kuruluyor" sayfasý
WizardInstalling=Kuruluyor
InstallingLabel=Lütfen [name] bilgisayarýnýza kurulurken bekleyin.

; *** "Kurulum Tamamlandý" sayfasý
FinishedHeadingLabel=[name] kurulum yardýmcýsý tamamlanýyor
FinishedLabelNoIcons=Bilgisayarýnýza [name] kurulumu tamamlandý.
FinishedLabel=Bilgisayarýnýza [name] kurulumu tamamlandý. Simgeleri yüklemeyi seçtiyseniz, simgelere týklayarak uygulamayý baþlatabilirsiniz.
ClickFinish=Kurulum yardýmcýsýndan çýkmak için Bitti üzerine týklayýn.
FinishedRestartLabel=[name] kurulumunun tamamlanmasý için, bilgisayarýnýz yeniden baþlatýlmalý. Þimdi yeniden baþlatmak ister misiniz?
FinishedRestartMessage=[name] kurulumunun tamamlanmasý için, bilgisayarýnýz yeniden baþlatýlmalý.%n%nÞimdi yeniden baþlatmak ister misiniz?
ShowReadmeCheck=Evet README dosyasý görüntülensin
YesRadio=&Evet, bilgisayar þimdi yeniden baþlatýlsýn
NoRadio=&Hayýr, bilgisayarý daha sonra yeniden baþlatacaðým
; used for example as 'Run MyProg.exe'
RunEntryExec=%1 çalýþtýrýlsýn
; used for example as 'View Readme.txt'
RunEntryShellExec=%1 görüntülensin

; *** "Kurulum için sýradaki disk gerekli" iletileri
ChangeDiskTitle=Kurulum yardýmcýsý sýradaki diske gerek duyuyor
SelectDiskLabel2=Lütfen %1 numaralý diski takýp Tamam üzerine týklayýn.%n%nDiskteki dosyalar aþaðýdakinden farklý bir klasörde bulunuyorsa, doðru yolu yazýn ya da Göz at üzerine týklayarak doðru klasörü seçin.
PathLabel=&Yol:
FileNotInDir2="%1" dosyasý "%2" içinde bulunamadý. Lütfen doðru diski takýn ya da baþka bir klasör seçin.
SelectDirectoryLabel=Lütfen sonraki diskin konumunu belirtin.

; *** Kurulum aþamasý iletileri
SetupAborted=Kurulum tamamlanamadý.%n%nLütfen sorunu düzelterek kurulum yardýmcýsýný yeniden çalýþtýrýn.
EntryAbortRetryIgnore=Çýkýþ: iptal et, Tekrar: tekrarla, Sonraki: devam et
AbortRetryIgnoreSelectAction=Yapýlacak iþlemi seçin
AbortRetryIgnoreRetry=&Yeniden denensin
AbortRetryIgnoreIgnore=&Sorun yok sayýlýp ilerlensin
AbortRetryIgnoreCancel=Kurulum iptal edilsin

; *** Kurulum durumu iletileri
StatusClosingApplications=Uygulamalar kapatýlýyor...
StatusCreateDirs=Klasörler oluþturuluyor...
StatusExtractFiles=Dosyalar ayýklanýyor...
StatusCreateIcons=Kýsayollar oluþturuluyor...
StatusCreateIniEntries=INI kayýtlarý oluþturuluyor...
StatusCreateRegistryEntries=Kayýt Defteri kayýtlarý oluþturuluyor...
StatusRegisterFiles=Dosyalar kaydediliyor...
StatusSavingUninstall=Kaldýrma bilgileri kaydediliyor...
StatusRunProgram=Kurulum tamamlanýyor...
StatusRestartingApplications=Uygulamalar yeniden baþlatýlýyor...
StatusRollback=Deðiþiklikler geri alýnýyor...

; *** Çeþitli hata iletileri
ErrorInternal2=Ýç hata: %1
ErrorFunctionFailedNoCode=%1 tamamlanamadý.
ErrorFunctionFailed=%1 tamamlanamadý; kod %2
ErrorFunctionFailedWithMessage=%1 tamamlanamadý; kod %2.%n%3
ErrorExecutingProgram=Þu dosya yürütülemedi:%n%1

; *** Kayýt defteri hatalarý
ErrorRegOpenKey=Kayýt defteri anahtarý açýlýrken bir sorun çýktý:%n%1%2
ErrorRegCreateKey=Kayýt defteri anahtarý eklenirken bir sorun çýktý:%n%1%2
ErrorRegWriteKey=Kayýt defteri anahtarý yazýlýrken bir sorun çýktý:%n%1%2

; *** INI hatalarý
ErrorIniEntry="%1" dosyasýna INI kaydý eklenirken bir sorun çýktý.

; *** Dosya kopyalama hatalarý
FileAbortRetryIgnore=&Bu dosya atlansýn (önerilmez)
FileAbortRetryIgnore2=&Sorun yok sayýlýp ilerlensin (önerilmez)
FileAbortRetryIgnoreSkipNotRecommended=&Bu dosya atlansýn (önerilmez)
FileAbortRetryIgnoreIgnoreNotRecommended=&Sorun yok sayýlýp ilerlensin (önerilmez)
SourceIsCorrupted=Kaynak dosya bozulmuþ
SourceDoesntExist="%1" kaynak dosyasý bulunamadý
ExistingFileReadOnly=Var olan dosya salt okunabilir olarak iþaretlenmiþ olduðundan üzerine yazýlamadý.
ExistingFileReadOnly2=Var olan dosya salt okunabilir olarak iþaretlenmiþ olduðundan üzerine yazýlamadý.
ExistingFileReadOnlyRetry=&Salt okunur iþareti kaldýrýlýp yeniden denensin
ExistingFileReadOnlyKeepExisting=&Var olan dosya korunsun
ErrorReadingExistingDest=Var olan dosya okunmaya çalýþýlýrken bir sorun çýktý.
FileExists=Dosya zaten mevcut.%n%nüzerine yazmak ister misiniz?
ExistingFileNewer=Mevcut dosya, kurulmakta olan dosyadan daha yenidir. Mevcut dosyayý saklamanýz önerilir.%n%nMevcut dosyayý saklamak istiyor musunuz?

FileExistsSelectAction=Yapýlacak iþlemi seçin
FileExists2=Dosya zaten var.
FileExistsOverwriteExisting=&Var olan dosyanýn üzerine yazýlsýn
FileExistsKeepExisting=Var &olan dosya korunsun
FileExistsOverwriteOrKeepAll=&Sonraki çakýþmalarda da bu iþlem yapýlsýn
ExistingFileNewerSelectAction=Yapýlacak iþlemi seçin
ExistingFileNewer2=Var olan dosya, kurulum yardýmcýsý tarafýndan yazýlmaya çalýþýlandan daha yeni.
ExistingFileNewerOverwriteExisting=&Var olan dosyanýn üzerine yazýlsýn
ExistingFileNewerKeepExisting=Var &olan dosya korunsun (önerilir)
ExistingFileNewerOverwriteOrKeepAll=&Sonraki çakýþmalarda bu iþlem yapýlsýn
ErrorChangingAttr=Var olan dosyanýn öznitelikleri deðiþtirilirken bir sorun çýktý:
ErrorCreatingTemp=Kurulum klasöründe bir dosya oluþturulurken sorun çýktý:
ErrorReadingSource=Kaynak dosya okunurken sorun çýktý:
ErrorCopying=Dosya kopyalanýrken sorun çýktý:
ErrorReplacingExistingFile=Var olan dosya deðiþtirilirken sorun çýktý:
ErrorRestartReplace=Yeniden baþlatmada üzerine yazýlamadý:
ErrorRenamingTemp=Kurulum klasöründeki bir dosyanýn adý deðiþtirilirken sorun çýktý:
ErrorRegisterServer=DLL/OCX kayýt edilemedi: %1
ErrorRegSvr32Failed=RegSvr32 iþlemi þu kod ile tamamlanamadý: %1
ErrorRegisterTypeLib=Tür kitaplýðý kayýt defterine eklenemedi: %1

; *** Kaldýrma sýrasýnda görüntülenecek ad iþaretleri
; used for example as 'My Program (32-bit)'
UninstallDisplayNameMark=%1 (%2)
; used for example as 'My Program (32-bit, All users)'
UninstallDisplayNameMarks=%1 (%2, %3)
UninstallDisplayNameMark32Bit=32 bit
UninstallDisplayNameMark64Bit=64 bit
UninstallDisplayNameMarkAllUsers=Tüm kullanýcýlar
UninstallDisplayNameMarkCurrentUser=Geçerli kullanýcý

; *** Kurulum sonrasý hatalarý
ErrorOpeningReadme=README dosyasý açýlýrken sorun çýktý.
ErrorRestartingComputer=Kurulum yardýmcýsý bilgisayarýnýzý yeniden baþlatamýyor. Lütfen bilgisayarýnýzý yeniden baþlatýn.

; *** Kaldýrma yardýmcýsý iletileri
UninstallNotFound="%1" dosyasý bulunamadý. Uygulama kaldýrýlamýyor.
UninstallOpenError="%1" dosyasý açýlamadý. Uygulama kaldýrýlamýyor.
UninstallUnsupportedVer="%1" uygulama kaldýrma günlük dosyasýnýn biçimi, bu kaldýrma yardýmcýsý sürümü tarafýndan anlaþýlamadý. Uygulama kaldýrýlamýyor.
UninstallUnknownEntry=Kaldýrma günlüðünde bilinmeyen bir kayýt (%1) bulundu.
ConfirmUninstall=%1 uygulamasýný tüm bileþenleri ile birlikte tamamen kaldýrmak istediðinize emin misiniz?
UninstallOnlyOnWin64=Bu kurulum yalnýzca 64 bit Windows üzerinden kaldýrýlabilir.
OnlyAdminCanUninstall=Bu kurulum yalnýzca yönetici yetkileri olan bir kullanýcý tarafýndan kaldýrýlabilir.
UninstallStatusLabel=Lütfen %1 uygulamasý bilgisayarýnýzdan kaldýrýlýrken bekleyin.
UninstalledAll=%1 uygulamasý bilgisayarýnýzdan kaldýrýldý.
UninstalledMost=%1 uygulamasý kaldýrýldý.%n%nBazý bileþenler kaldýrýlamadý. Bunlarý el ile silebilirsiniz.
UninstalledAndNeedsRestart=%1 kaldýrma iþleminin tamamlanmasý için bilgisayarýnýzýn yeniden baþlatýlmasý gerekli.%n%nÞimdi yeniden baþlatmak ister misiniz?
UninstallDataCorrupted="%1" dosyasý bozulmuþ. Kaldýrýlamýyor.

; *** Kaldýrma aþamasý iletileri
ConfirmDeleteSharedFileTitle=Paylaþýlan dosya silinsin mi?
ConfirmDeleteSharedFile2=Sisteme göre, paylaþýlan þu dosya baþka bir uygulama tarafýndan kullanýlmýyor ve kaldýrýlabilir. Bu paylaþýlmýþ dosyayý silmek ister misiniz?%n%nBu dosya, baþka herhangi bir uygulama tarafýndan kullanýlýyor ise, silindiðinde diðer uygulama düzgün çalýþmayabilir. Emin deðilseniz Hayýr üzerine týklayýn. Dosyayý sisteminizde býrakmanýn bir zararý olmaz.
SharedFileNameLabel=Dosya adý:
SharedFileLocationLabel=Konum:
WizardUninstalling=Kaldýrma durumu
StatusUninstalling=%1 kaldýrýlýyor...

; *** Kapatmayý engelleme nedenleri
ShutdownBlockReasonInstallingApp=%1 kuruluyor.
ShutdownBlockReasonUninstallingApp=%1 kaldýrýlýyor.

; The custom messages below aren't used by Setup itself, but if you make
; use of them in your scripts, you'll want to translate them.

[CustomMessages]

NameAndVersion=%1 %2 sürümü
AdditionalIcons=Ek simgeler:
CreateDesktopIcon=Masaüstü simg&esi oluþturulsun
CreateQuickLaunchIcon=Hýzlý baþlat simgesi &oluþturulsun
ProgramOnTheWeb=%1 sitesi
UninstallProgram=%1 uygulamasýný kaldýr
LaunchProgram=%1 uygulamasýný çalýþtýr
AssocFileExtension=%1 &uygulamasý ile %2 dosya uzantýsý iliþkilendirilsin
AssocingFileExtension=%1 uygulamasý ile %2 dosya uzantýsý iliþkilendiriliyor...
AutoStartProgramGroupDescription=Baþlangýç:
AutoStartProgram=%1 otomatik olarak baþlatýlsýn
AddonHostProgramNotFound=%1 seçtiðiniz klasörde bulunamadý.%n%nYine de ilerlemek istiyor musunuz?
