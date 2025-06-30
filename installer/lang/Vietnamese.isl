; *** Inno Setup version 6.1.0+ Vietnamese messages ***
; Translated by Vu Khac Hiep (email: vukhachiep@gmail.com)
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
LanguageName=Vietnamese
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
SetupAppTitle=Cài đặt
SetupWindowTitle=Cài đặt - %1
UninstallAppTitle=Gỡ cài đặt
UninstallAppFullTitle=Gỡ cài đặt - %1

; *** Misc. common
InformationTitle=Thông tin
ConfirmTitle=Xác nhận
ErrorTitle=Lỗi

; *** SetupLdr messages
SetupLdrStartupMessage=Chương trình này sẽ cài đặt %1. Bạn có muốn tiếp tục không?
LdrCannotCreateTemp=Không thể tạo tệp tạm thời. Cài đặt bị hủy bỏ
LdrCannotExecTemp=Không thể chạy tệp trong thư mục tạm thời. Cài đặt bị hủy bỏ
HelpTextNote=

; *** Startup error messages
LastErrorMessage=%1.%n%nLỗi %2: %3
SetupFileMissing=Tệp %1 bị thiếu trong thư mục cài đặt. Hãy sửa lỗi hoặc lấy một bản sao mới của chương trình.
SetupFileCorrupt=Các tệp cài đặt đã bị hỏng. Hãy sửa lỗi hoặc lấy một bản sao của chương trình.
SetupFileCorruptOrWrongVer=Các tệp cài đặt bị hỏng, hoặc không tương thích với bản cài đặt này. Hãy sửa lỗi hoặc lấy một bản sao mới của chương trình.
InvalidParameter=Một thông số không hợp lệ đã được đưa vào dòng lệnh:%n%n%1
SetupAlreadyRunning=Cài đặt này đang chạy.
WindowsVersionNotSupported=Chương trình này không tương thích với phiên bản Windows bạn đang chạy.
WindowsServicePackRequired=Chương trình này yêu cầu %1 Service Pack %2 hoặc mới hơn.
NotOnThisPlatform=Chương trình này sẽ không chạy trên %1.
OnlyOnThisPlatform=Chương trình này phải chạy trên %1.
OnlyOnTheseArchitectures=Chương trình này chỉ có thể được cài đặt trên phiên bản Windows được thiết kế cho các hệ vi xử lí:%n%n%1
WinVersionTooLowError=Chương trình này yêu cầu %1 phiên bản %2 hoặc mới hơn.
WinVersionTooHighError=Chương trình này không thể được cài đặt trên %1 phiên bản %2 hoặc mới hơn.
AdminPrivilegesRequired=Bạn phải được đăng nhập như người quản trị khi cài đặt chương trình này.
PowerUserPrivilegesRequired=Bạn phải được đăng nhập như người quản trị hoặc thành viên trong nhóm Người dùng mạnh khi cài đặt chương trình này.
SetupAppRunningError=Cài đặt phát hiện %1 đang chạy.%n%nHãy đóng tất cả các tiến trình của nó ngay, rồi click OK để tiếp tục, hoặc Hủy để thoát.
UninstallAppRunningError=Gỡ cài đặt phát hiện %1 đang chạy.%n%nHãy đóng tất cả các tiến trình của nó ngay, rồi click OK để tiếp tục, hoặc Hủy để thoát.

; *** Startup questions
PrivilegesRequiredOverrideTitle=Select Setup Install Mode
PrivilegesRequiredOverrideInstruction=Select install mode
PrivilegesRequiredOverrideText1=%1 can be installed for all users (requires administrative privileges), or for you only.
PrivilegesRequiredOverrideText2=%1 can be installed for you only, or for all users (requires administrative privileges).
PrivilegesRequiredOverrideAllUsers=Install for &all users
PrivilegesRequiredOverrideAllUsersRecommended=Install for &all users (recommended)
PrivilegesRequiredOverrideCurrentUser=Install for &me only
PrivilegesRequiredOverrideCurrentUserRecommended=Install for &me only (recommended)

; *** Misc. errors
ErrorCreatingDir=Cài đặt không thể tạo ra thư mục "%1"
ErrorTooManyFilesInDir=Không thể tạo một tệp trong thư mục "%1" vì nó chứa quá nhiều tệp

; *** Setup common messages
ExitSetupTitle=Thoát cài đặt
ExitSetupMessage=Cài đặt chưa hoàn thành. Nếu bạn thoát bây giờ, chương trình sẽ không được cài đặt.%n%nBạn có thể chạy lại Cài đặt một lần khác để hoàn thành cài đặt.%n%nThoát ngay?
AboutSetupMenuItem=&Về trình cài đặt...
AboutSetupTitle=Về trình cài đặt
AboutSetupMessage=%1 phiên bản %2%n%3%n%n%1 trang chủ:%n%4
AboutSetupNote=
TranslatorNote=Giao diện người dùng tiếng Việt bởi: Vũ Khắc Hiệp

; *** Buttons
ButtonBack=< &Trước
ButtonNext=T&iếp >
ButtonInstall=&Cài đặt
ButtonOK=OK
ButtonCancel=Hủy
ButtonYes=&Có
ButtonYesToAll=Có c&ho tất cả
ButtonNo=&Không
ButtonNoToAll=Khô&ng cho tất cả
ButtonFinish=&Hoàn thành
ButtonBrowse=&Duyệt...
ButtonWizardBrowse=D&uyệt...
ButtonNewFolder=Tạ&o thư mục mới

; *** "Select Language" dialog messages
SelectLanguageTitle=Chọn ngôn ngữ cài đặt
SelectLanguageLabel=Chọn ngôn ngữ để sử dụng khi cài đặt:

; *** Common wizard text
ClickNext=Nhấn Tiếp để tiếp tục, hoặc Hủy để thoát cài đặt
BeveledLabel=
BrowseDialogTitle=Tìm thư mục
BrowseDialogLabel=Chọn một thư mục trong danh sách sau rồi ấn OK.
NewFolderName=Tạo thư mục mới

; *** "Welcome" wizard page
WelcomeLabel1=Chào mừng tới trình cài đặt [name]
WelcomeLabel2=Chương trình này sẽ cài [name/ver] trên máy tính của bạn.%n%nChúng tôi khuyên bạn đóng mọi chương trình khác lại trước khi cài đặt.

; *** "Password" wizard page
WizardPassword=Mật khẩu
PasswordLabel1=Việc cài đặt được bảo vệ bằng mật khẩu.
PasswordLabel3=Hãy nhập mật khẩu, rồi nhấn Tiếp để tiếp tục. Mật khẩu phân biệt chữ hoa/thường.
PasswordEditLabel=&Mật khẩu:
IncorrectPassword=Mật khẩu bạn đã nhập không đúng. Hãy thử lại.

; *** "License Agreement" wizard page
WizardLicense=Thỏa thuận cấp phép
LicenseLabel=Hãy đọc những thông tin quan trọng sau trước khi tiếp tục.
LicenseLabel3=Hãy đọc Thỏa thuận cấp phép sau. Bạn phải chấp nhận các điều khoản của cài đặt này trước khi tiếp tục.
LicenseAccepted=Tô&i chấp nhận thỏa thuận
LicenseNotAccepted=Tôi khôn&g chấp nhận thỏa thuận

; *** "Information" wizard pages
WizardInfoBefore=Thông tin
InfoBeforeLabel=Hãy đọc những thông tin quan trọng sau trước khi tiếp tục.
InfoBeforeClickLabel=Khi bạn đã sẵn sàng cài đặt tiếp, click Tiếp.
WizardInfoAfter=Thông tin
InfoAfterLabel=Hãy đọc những thông tin quan trọng sau trước khi tiếp tục.
InfoAfterClickLabel=Khi bạn đã sẵn sàng cài đặt tiếp, click Tiếp.

; *** "User Information" wizard page
WizardUserInfo=Thông tin người dùng
UserInfoDesc=Hãy nhập thông tin của bạn.
UserInfoName=Tên n&gười dùng:
UserInfoOrg=Tổ c&hức:
UserInfoSerial=&Số serial:
UserInfoNameRequired=Bạn phải nhập một tên.

; *** "Select Destination Location" wizard page
WizardSelectDir=Chọn vị trí cài đặt
SelectDirDesc=[name] nên được cài đặt ở đâu?
SelectDirLabel3=[name] sẽ được cài đặt vào thư mục sau:
SelectDirBrowseLabel=Để tiếp tục. nhấn Tiếp. Nếu bạn muốn chọn một thư mục khác, nhấn Duyệt.
DiskSpaceGBLabel=Cần có ít nhất [gb] GB ổ đĩa trống.
DiskSpaceMBLabel=Cần có ít nhất [mb] MB ổ đĩa trống.
CannotInstallToNetworkDrive=Cài đặt không thể cài vào một ổ đĩa mạng.
CannotInstallToUNCPath=Cài đặt không thể cài vào đường dẫn UNC.
InvalidPath=Bạn phải nhập đường dẫn đầy đủ với chữ cái ổ đĩa, ví dụ:%n%nC:\APP%n%nhoặc một đường dẫn UNC theo mẫu:%n%n\\server\share
InvalidDrive=Ổ đĩa hoặc chia sẻ UNC bạn đã chọn không tồn tại hoặc không truy cập được. Hãy chọn cái khác.
DiskSpaceWarningTitle=Không đủ dung lượng đĩa
DiskSpaceWarning=Cài đặt yêu cầu ít nhất %1 KB dung lượng trống để cài đặt, nhưng ổ đĩa đã chọn chỉ còn %2KB.%n%nBạn muốn tiếp tục bằng mọi giá?
DirNameTooLong=Tên thư mục hoặc đường dẫn quá dài.
InvalidDirName=Tên thư mục không hợp lệ.
BadDirName32=Tên thư mục không được chứa các kí tự sau:%n%n%1
DirExistsTitle=Thư mục đã tồn tại
DirExists=Thư mục:%n%n%1%n%nđã tồn tại. Bạn có muốn cài đặt vào thư mục đó bằng mọi giá?
DirDoesntExistTitle=Thư mục không tồn tại
DirDoesntExist=Thư mục:%n%n%1%n%nkhông tồn tại. Bạn có muốn tạo thư mục không?

; *** "Select Components" wizard page
WizardSelectComponents=Chọn các thành phần
SelectComponentsDesc=Những thành phần nào nên được cài đặt?
SelectComponentsLabel2=Chọn các thành phần bạn muốn cài đặt, bỏ chọn các thành phần bạn không muốn. Click Tiếp khi bạn đã sẵn sàng để tiếp tục.
FullInstallation=Cài đặt đầy đủ
; if possible don't translate 'Compact' as 'Minimal' (I mean 'Minimal' in your language)
CompactInstallation=Cài đặt rút gọn
CustomInstallation=Cài đặt tủy chỉnh
NoUninstallWarningTitle=Thành phần đã tồn tại
NoUninstallWarning=Cài đặt phát hiện các thành phần sau đã được cài đặt trên máy tính của bạn:%n%n%1%n%nBỏ chọn những thành phần này sẽ không cài đặt chúng.%n%nBạn có muốn tiếp tục bằng mọi giá?
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceGBLabel=Lựa chọn này yêu cầu ít nhất [gb] GB không gian đĩa.
ComponentsDiskSpaceMBLabel=Lựa chọn này yêu cầu ít nhất [mb] MB không gian đĩa.

; *** "Select Additional Tasks" wizard page
WizardSelectTasks=Chọn các tác vụ bổ sung
SelectTasksDesc=Các tác vụ bổ sung nào nên được thực hiện?
SelectTasksLabel2=Chọn các tác vụ bổ sung mà bạn muốn cài đặt thực hiện khi cài đặt [name], rồi nhấn Tiếp.

; *** "Select Start Menu Folder" wizard page
WizardSelectProgramGroup=Chọn thư mục bắt đầu
SelectStartMenuFolderDesc=Các lối tắt đến chương trình nên được đặt ở đâu?
SelectStartMenuFolderLabel3=Cài đặt sẽ tạo các lối tắt đến chương trình trong thư mục bắt đầu sau.
SelectStartMenuFolderBrowseLabel=Để tiếp tục, click Tiếp. Nếu bạn muốn chọn thư mục khác, click Duyệt.
MustEnterGroupName=Bạn phải nhập tên một thư mục.
GroupNameTooLong=Tên thư mục hoặc đường dẫn quá dài.
InvalidGroupName=Tên thư mục không hợp lệ.
BadGroupName=Tên thư mục không được chứa các kí tự sau:%n%n%1
NoProgramGroupCheck2=&Không tạo thư mục bắt đầu

; *** "Ready to Install" wizard page
WizardReady=Sẵn sàng cài đặt
ReadyLabel1=[name] đã sẵn sàng để dược cài đặt trên máy tính của bạn.
ReadyLabel2a=Click Cài đặt để tiếp tục, hoặc click Trước nếu bạn muốn xem lại/thay đổi bất kì cài đặt nào.
ReadyLabel2b=Click Cài đặt để tiếp tục cài đặt.
ReadyMemoUserInfo=Thông tin người dùng:
ReadyMemoDir=Vị trí đích:
ReadyMemoType=Kiểu cài đặt:
ReadyMemoComponents=Các thành phần được chọn:
ReadyMemoGroup=Thư mục bắt đầu:
ReadyMemoTasks=Các tác vụ bổ sung:

; *** TDownloadWizardPage wizard page and DownloadTemporaryFile
DownloadingLabel=Đang tải các tập tin bổ sung...
ButtonStopDownload=&Dừng tải xuống
StopDownload=Bạn có chắc chắn muốn dừng tải xuống không?
ErrorDownloadAborted=Tải xuống bị hủy bỏ
ErrorDownloadFailed=Tải xuống không thành công: %1 %2
ErrorDownloadSizeFailed=Getting size failed: %1 %2
ErrorFileHash1=File hash failed: %1
ErrorFileHash2=Invalid file hash: expected %1, found %2
ErrorProgress=Invalid progress: %1 of %2
ErrorFileSize=Invalid file size: expected %1, found %2

; *** "Preparing to Install" wizard page
WizardPreparing=Chuẩn bị cài đặt
PreparingDesc=[name] đang chuẩn bị được cài đặt trên máy tính của bạn.
PreviousInstallNotCompleted=Việc cài đặt/gỡ bỏ một chương trình chưa được hoàn tất trước đó. Bạn sẽ phải khởi động lại máy tính để hoàn tất cài đặt đó.%n%nSau khi chởi động lại, chạy Cài đặt một lần nữa để hoàn tất cài đặt [name].
CannotContinue=Cài đặt không thể tiếp tục. Nhấn Hủy để thoát.
ApplicationsFound=Những chương trình sau đang sử dụng các tệp cần được cập nhật bởi trình cài đặt. Chúng tôi khuyên bạn cho phép Cài đặt đóng các chương trình này.
ApplicationsFound2=Những chương trình sau đang sử dụng các tệp cần được cập nhật bởi trình cài đặt. Chúng tôi khuyên bạn cho phép Cài đặt đóng các chương trình này. Sau khi hoàn thành cài đặt, chúng tôi sẽ thử khởi động lại các chương trình này.
CloseApplications=Tự độn&g đóng các chương trình này
DontCloseApplications=Không đóng các chương t&rình này
ErrorCloseApplications=Cài đặt không thể đóng mọi chương trình. Chúng tôi khuyên bạn đóng các chương trình đang sử dụng các tệp cần được cập nhật bởi Cài đặt một cách thủ công trước khi tiếp tục.
PrepareToInstallNeedsRestart=Setup must restart your computer. After restarting your computer, run Setup again to complete the installation of [name].%n%nWould you like to restart now?

; *** "Installing" wizard page
WizardInstalling=Đang cài đặt
InstallingLabel=Hãy đợi khi [name] đang được cài đặt trên máy tính của bạn.

; *** "Setup Completed" wizard page
FinishedHeadingLabel=Hoàn thành cài đặt [name]
FinishedLabelNoIcons=[name] đã được cài đặt xong trên máy tính của bạn.
FinishedLabel=[name] đã được cài đặt xong trên máy tính của bạn. Chương trình có thể được khởi động bằng cách click vào lối tắt đến chương trình.
ClickFinish=Click Hoàn thành để thoát Cài đặt.
FinishedRestartLabel=Để hoàn thành cài đặt [name], máy tính của bạn cần đươc khởi động lại. Bạn có muốn khởi động lại ngay?
FinishedRestartMessage=Để hoàn thành cài đặt [name], máy tính của bạn cần đươc khởi động lại.%n%nBạn có muốn khởi động lại ngay?
ShowReadmeCheck=Có, tôi muốn xem tệp README
YesRadio=&Có, khởi động lại máy tính ngay
NoRadio=&Không, tôi sẽ khởi động lại máy tính sau
; used for example as 'Run MyProg.exe'
RunEntryExec=Chạy %1
; used for example as 'View Readme.txt'
RunEntryShellExec=Xem %1

; *** "Setup Needs the Next Disk" stuff
ChangeDiskTitle=Cài đặt cần đĩa tiếp theo
SelectDiskLabel2=Hãy chèn đĩa %1 và click OK.%n%nNếu các tệp trên đĩa này có thể được tìm thấy trên một thư mục khác với được hiển thị dưới đây, nhập đường dẫn hoặc click Duyệt.
PathLabel=Đườ&ng dẫn:
FileNotInDir2=Tệp "%1" không thể được xác định trong "%2". Hãy chọn đia xđúng hoặc chọn thư mục khác.
SelectDirectoryLabel=Hãy chọn vị trí của đĩa tiếp theo.

; *** Installation phase messages
SetupAborted=Cài đặt không được hoàn thành.%n%nHãy sửa lỗi và chạy Cài đặt lại.
AbortRetryIgnoreSelectAction=Chọn hành động
AbortRetryIgnoreRetry=&Thử lại
AbortRetryIgnoreIgnore=&Bỏ qua lỗi và tiếp tục
AbortRetryIgnoreCancel=Hủy

; *** Installation status messages
StatusClosingApplications=Đang đóng các chương trình...
StatusCreateDirs=Đang tạo các thư mục...
StatusExtractFiles=Đang giải nén các tệp...
StatusCreateIcons=Đang tạo các lối tắt...
StatusCreateIniEntries=Đang tạo các đầu vào INI...
StatusCreateRegistryEntries=Đang tạo các đầu vào registry...
StatusRegisterFiles=Đang đăng kí các tệp...
StatusSavingUninstall=Đang lưu thông tin gỡ cài đặt...
StatusRunProgram=Đang hoàn thành cài đặt...
StatusRestartingApplications=Đang khởi động lại các chương trình...
StatusRollback=Đang hoàn lại các thay đổi...

; *** Misc. errors
ErrorInternal2=Lỗi nội bộ: %1
ErrorFunctionFailedNoCode=%1 thất bại
ErrorFunctionFailed=%1 thất bại với mã lỗi %2
ErrorFunctionFailedWithMessage=%1 thất bại với mã lỗi %2.%n%3
ErrorExecutingProgram=Không thể chạy tệp:%n%1

; *** Registry errors
ErrorRegOpenKey=Lỗi khi mở registry:%n%1\%2
ErrorRegCreateKey=Lỗi khi tạo registry:%n%1\%2
ErrorRegWriteKey=Lỗi khi viết registry:%n%1\%2

; *** INI errors
ErrorIniEntry=Lỗi tạo đầu vào INI cho tệp "%1".

; *** File copying errors
FileAbortRetryIgnoreSkipNotRecommended=&Bỏ qua tệp này (không khuyến nghị)
FileAbortRetryIgnoreIgnoreNotRecommended=&Bỏ qua để tiếp tục bằng mọi giá (không khuyến nghị)
SourceIsCorrupted=Tệp nguồn bị hỏng
SourceDoesntExist=Tệp nguồn "%1" không tồn tại
ExistingFileReadOnly2=Tệp đã tồn tại với đánh dấu chỉ đọc.
ExistingFileReadOnlyRetry=&Xóa thuộc tính chỉ đọc và thử lại
ExistingFileReadOnlyKeepExisting=&Giữ tập tin hiện có
ErrorReadingExistingDest=Một lỗi đã xảy ra khi đọc tệp:
FileExistsSelectAction=Select action
FileExists2=Tệp đã tồn tại.
FileExistsOverwriteExisting=G&hi đè tệp hiện có
FileExistsKeepExisting=&Giữ tệp hiện có
FileExistsOverwriteOrKeepAll=&Do this for the next conflicts
ExistingFileNewerSelectAction=Select action
ExistingFileNewer2=Tệp hiện có mới hơn tệp mà Thiết lập đang cố gắng cài đặt.
ExistingFileNewerOverwriteExisting=&Ghi đè tệp hiện có
ExistingFileNewerKeepExisting=&Giữ tệp hiện có (khuyến nghị)
ExistingFileNewerOverwriteOrKeepAll=&Do this for the next conflicts
ErrorChangingAttr=Một lỗi đã xảy ra khi thay đổi thuộc tính của tệp sau:
ErrorCreatingTemp=Một lỗi đã xảy ra khi tạo một tệp trong thư mục đích:
ErrorReadingSource=Một lỗi đã xảy ra khi đọc tệp nguồn:
ErrorCopying=Một lỗi đã xảy ra khi sao chép tệp:
ErrorReplacingExistingFile=Một lỗi đã xảy ra khi thay thế tệp:
ErrorRestartReplace=Khởi động lại & Thay thế (RestartReplace) thất bại:
ErrorRenamingTemp=Một lỗi đã xảy ra khi đổi tên tệp trong thư mục đích:
ErrorRegisterServer=Không thể đăng kí DLL/OCX: %1
ErrorRegSvr32Failed=RegSvr32 thất bại với mã thoát %1
ErrorRegisterTypeLib=Không thể đăng kí thư viện kiểu: %1

; *** Uninstall display name markings
; used for example as 'My Program (32-bit)'
UninstallDisplayNameMark=%1 (%2)
; used for example as 'My Program (32-bit, All users)'
UninstallDisplayNameMarks=%1 (%2, %3)
UninstallDisplayNameMark32Bit=32-bit
UninstallDisplayNameMark64Bit=64-bit
UninstallDisplayNameMarkAllUsers=All users
UninstallDisplayNameMarkCurrentUser=Current user

; *** Post-installation errors
ErrorOpeningReadme=Một lỗi đã xảy ra khi mở tệp README.
ErrorRestartingComputer=Cài đặt không thể khởi động lại máy tính. Hãy làm việc này một cách thủ công.

; *** Uninstaller messages
UninstallNotFound=Tệp "%1" không tồn tại. Không thể gỡ cài đặt.
UninstallOpenError=Tệp "%1" không thể được mở. Không thể gỡ cài đặt
UninstallUnsupportedVer=Tệp nhật kí gỡ cài đặt "%1" có định dạng không thể được xác định bởi phiên bản gỡ cài đặt này. Không thể gỡ cài đặt
UninstallUnknownEntry=Một đầu vào không xác định (%1) đã bị phát hiện trong nhật kí gỡ cài đặt
ConfirmUninstall=Bạn có muốn dỡ bỏ hoàn toàn %1 và mọi thành phần của nó?
UninstallOnlyOnWin64=Cài đặt này chỉ có thể được gỡ bỏ trên Windows 64 bit.
OnlyAdminCanUninstall=Cài đặt này chỉ có thể được gỡ bỏ bằng một người dùng có quyền người quản trị.
UninstallStatusLabel=Hãy đợi khi %1 được gỡ khỏi máy tính của bạn.
UninstalledAll=%1 đã được gỡ bỏ thành công khỏi máy tính của bạn.
UninstalledMost=%1 đã được gỡ bỏ thành công.%n%nMột số thành phần không thể được gỡ bỏ. Hãy làm việc này một cách thủ công.
UninstalledAndNeedsRestart=Để hoàn thành việc gỡ cài đặt %1, bạn phải khởi động lại máy tính.%n%nBạn có muốn khởi động lại ngay?
UninstallDataCorrupted=Tệp "%1" bị hỏng. Không thể gỡ cài đặt

; *** Uninstallation phase messages
ConfirmDeleteSharedFileTitle=Gỡ bỏ tệp được chia sẻ?
ConfirmDeleteSharedFile2=Hệ thống chỉ ra các tệp được chia sẻ sau không được sử dụng bởi chương trình nào. Bạn có muốn gỡ bỏ tệp này?%n%nNếu có một chương trình vẫn sử dụng tệp này mà tệp bị gỡ bỏ, chúng có thể không chạy tốt. Nếu bạn không chắc chắn, chọn Không. Để lại tệp trên hệ thống của bạn sẽ không gây ra tổn hại.
SharedFileNameLabel=Tên tệp:
SharedFileLocationLabel=Vị trí:
WizardUninstalling=Trạng thái gỡ cài đặt
StatusUninstalling=Đang gỡ cài đặt %1...

; *** Shutdown block reasons
ShutdownBlockReasonInstallingApp=Đang cài đặt %1.
ShutdownBlockReasonUninstallingApp=Đang gỡ cài đặt %1.

; The custom messages below aren't used by Setup itself, but if you make
; use of them in your scripts, you'll want to translate them.

[CustomMessages]

NameAndVersion=%1 phiên bản %2
AdditionalIcons=Các lối tắt bổ sung:
CreateDesktopIcon=Tạo một &lối tắt trên Desktop
CreateQuickLaunchIcon=Tạo một lối tắt &Khởi động nhanh
ProgramOnTheWeb=%1 trên Web
UninstallProgram=Gỡ cài đặt %1
LaunchProgram=Khởi động %1
AssocFileExtension=&Gán %1 với đuôi tệp %2
AssocingFileExtension=Đang gán %1 với đuôi tệp %2...
AutoStartProgramGroupDescription=Khởi động:
AutoStartProgram=Tự động khởi động %1
AddonHostProgramNotFound=%1 không thể được xác định trong thư mục bạn đã chọn.%n%nBạn có muốn tiếp tục bằng mọi giá?
