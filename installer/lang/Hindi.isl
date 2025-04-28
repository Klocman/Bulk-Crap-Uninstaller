; *** Inno Setup version 5.5.3+ Hindi messages ***
;
; To download user-contributed translations of this file, go to:
;   http://www.jrsoftware.org/files/istrans/
;
; Note: When translating this text, do not add periods (.) to the end of
; messages that didn't have them already, because on those messages Inno
; Setup adds the periods automatically (appending a period would result in
; two periods being displayed).

[LangOptions]
; निम्नलिखित तीन प्रविष्टियाँ बहुत महत्वपूर्ण हैं। कृपया सहायता फ़ाइल में
; '[LangOptions] अनुभाग' विषय को पढ़ें और समझें।
LanguageName=Hindi (हिंदी)
LanguageID=$0439
LanguageCodePage=0
; यदि जिस भाषा में अनुवाद कर रहे हैं, उसके लिए विशेष फॉन्ट फेस या
; आकार की आवश्यकता है, तो निम्न में से किसी भी प्रविष्टि को अनकॉमेंट करें और उन्हें अनुकूलित करें।
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
SetupAppTitle=स्थापना
SetupWindowTitle=स्थापना - %1
UninstallAppTitle=अनइंस्टॉल
UninstallAppFullTitle=अनइंस्टॉल %1

; *** Misc. common
InformationTitle=सूचना
ConfirmTitle=पुष्टि
ErrorTitle=त्रुटि

; *** SetupLdr messages
SetupLdrStartupMessage=यह प्रोग्राम %1 की स्थापना करेगा। क्या आप जारी रखना चाहते हैं?
LdrCannotCreateTemp=अस्थायी फ़ाइल बनाने में असमर्थ। स्थापना रद्द कर दी गई।
LdrCannotExecTemp=अस्थायी फ़ोल्डर में फ़ाइल चलाने में असमर्थ। स्थापना रद्द कर दी गई।

; *** Startup error messages
LastErrorMessage=%1.%n%nत्रुटि %2: %3
SetupFileMissing=फ़ाइल %1 स्थापना फ़ोल्डर में नहीं मिली। कृपया समस्या ठीक करें या प्रोग्राम का कोई अन्य संस्करण खोजें.
SetupFileCorrupt=स्थापना फ़ाइल भ्रष्ट है। कृपया प्रोग्राम का कोई अन्य संस्करण खोजें.
SetupFileCorruptOrWrongVer=स्थापना फ़ाइल भ्रष्ट है, या इस संस्करण के साथ संगत नहीं है। कृपया समस्या ठीक करें या प्रोग्राम का कोई अन्य संस्करण खोजें.
InvalidParameter=एक अमान्य पॉइंटर कमांड लाइन पर पारित किया गया:%n%n%1
SetupAlreadyRunning=स्थापना पहले से चल रही है.
WindowsVersionNotSupported=यह प्रोग्राम आपके वर्तमान Windows संस्करण का समर्थन नहीं करता.
WindowsServicePackRequired=यह प्रोग्राम %1 सेवा पैक %2 या उससे उच्चतर की आवश्यकता है.
NotOnThisPlatform=यह प्रोग्राम %1 पर नहीं चलेगा.
OnlyOnThisPlatform=यह प्रोग्राम केवल %1 पर चलना चाहिए.
OnlyOnTheseArchitectures=यह प्रोग्राम केवल उन Windows संस्करणों पर स्थापित किया जा सकता है जो निम्नलिखित प्रोसेसर आर्किटेक्चर के लिए डिज़ाइन किए गए हैं:%n%n%1
MissingWOW64APIs=आपके Windows संस्करण में आवश्यक WOW64 APIs शामिल नहीं हैं ताकि 64-बिट प्रोग्राम की स्थापना की जा सके। इस समस्या को ठीक करने के लिए, कृपया %1 सेवा पैक स्थापित करें.
WinVersionTooLowError=यह प्रोग्राम %1, संस्करण %2 या उससे उच्चतर की आवश्यकता करता है.
WinVersionTooHighError=यह प्रोग्राम %1, संस्करण %2 या उससे उच्चतर पर स्थापित नहीं किया जा सकता.
AdminPrivilegesRequired=इस प्रोग्राम को स्थापित करने के लिए आपको व्यवस्थापक होना आवश्यक है.
PowerUserPrivilegesRequired=इस प्रोग्राम को स्थापित करने के लिए आपको व्यवस्थापक या शक्तिशाली उपयोगकर्ता समूह का सदस्य होना आवश्यक है.
SetupAppRunningError=स्थापना विंडो बाधित है क्योंकि %1 चल रहा है.%n%nकृपया इन्हें बंद करें, फिर OK पर क्लिक करें जारी रखने के लिए, या रद्द करने के लिए Cancel पर क्लिक करें.
UninstallAppRunningError=अनइंस्टॉल विंडो बाधित है क्योंकि %1 चल रहा है.%n%nकृपया इन्हें बंद करें, फिर OK पर क्लिक करें जारी रखने के लिए, या रद्द करने के लिए Cancel पर क्लिक करें.

; *** Misc. errors
ErrorCreatingDir=फ़ोल्डर "%1" नहीं बनाया जा सका.
ErrorTooManyFilesInDir=फ़ोल्डर "%1" में बहुत अधिक फ़ाइलें होने के कारण फ़ाइल नहीं बनाई जा सकती.

; *** Setup common messages
ExitSetupTitle=स्थापना से बाहर निकलें
ExitSetupMessage=स्थापना अधूरी है। यदि आप अभी बाहर निकलते हैं, तो प्रोग्राम स्थापित नहीं होगा.%n%nआपको स्थापना पूरी करने के लिए इंस्टॉलर को फिर से चलाना होगा.%n%nक्या आप बाहर निकलना चाहते हैं?
AboutSetupMenuItem=&स्थापना के बारे में...
AboutSetupTitle=स्थापना के बारे में
AboutSetupMessage=%1 संस्करण %2%n%3%n%n%1 की होम पेज:%n%4
AboutSetupNote=मूल भाषा संस्करण: अंग्रेज़ी
TranslatorNote=अनुवादक: ChatGPT

; *** Buttons
ButtonBack=< &पिछला
ButtonNext=आ&गे >
ButtonInstall=&स्थापना
ButtonOK=OK
ButtonCancel=रद्द करें
ButtonYes=हाँ
ButtonYesToAll=सभी के लिए हाँ
ButtonNo=नहीं
ButtonNoToAll=सभी के लिए नहीं
ButtonFinish=समाप्त
ButtonBrowse=ब्राउज़...
ButtonWizardBrowse=खोजें...
ButtonNewFolder=नया फ़ोल्डर बनाएँ

; *** "Select Language" dialog messages
SelectLanguageTitle=स्थापना के लिए भाषा चुनें
SelectLanguageLabel=स्थापना के दौरान प्रदर्शित करने के लिए भाषा चुनें:

; *** Common wizard text
ClickNext=जारी रखने के लिए 'आगे' पर क्लिक करें, या बाहर निकलने के लिए 'रद्द करें' पर क्लिक करें.
BeveledLabel=
BrowseDialogTitle=फ़ोल्डर खोजें
BrowseDialogLabel=नीचे सूची में से एक फ़ोल्डर चुनें, फिर OK पर क्लिक करें.
NewFolderName=नया फ़ोल्डर

; *** "Welcome" wizard page
WelcomeLabel1=[name] स्थापना में आपका स्वागत है
WelcomeLabel2=[name/ver] आपके कंप्यूटर पर स्थापित किया जाएगा.%n%nजारी रखने से पहले आपको सभी संबंधित प्रोग्राम बंद करने होंगे.

; *** "Password" wizard page
WizardPassword=पासवर्ड
PasswordLabel1=यह स्थापना पासवर्ड द्वारा संरक्षित है.
PasswordLabel3=कृपया पासवर्ड दर्ज करें, फिर जारी रखने के लिए 'आगे' पर क्लिक करें. पासवर्ड 100% सही दर्ज करें (बड़े और छोटे अक्षरों में भेद होता है).
PasswordEditLabel=&पासवर्ड:
IncorrectPassword=आपने जो पासवर्ड दर्ज किया है वह गलत है. कृपया पुनः प्रयास करें.

; *** "License Agreement" wizard page
WizardLicense=लाइसेंस समझौता
LicenseLabel=कृपया जारी रखने से पहले निम्नलिखित महत्वपूर्ण जानकारी पढ़ें.
LicenseLabel3=कृपया नीचे दिया गया लाइसेंस समझौता पढ़ें. जारी रखने से पहले आपको इसकी शर्तों को स्वीकार करना अनिवार्य है.
LicenseAccepted=मैं &शर्तों को स्वीकार करता हूँ
LicenseNotAccepted=मैं शर्तों को &स्वीकार नहीं करता

; *** "Information" wizard pages
WizardInfoBefore=जानकारी
InfoBeforeLabel=कृपया जारी रखने से पहले निम्नलिखित महत्वपूर्ण जानकारी पढ़ें.
InfoBeforeClickLabel=जब आप स्थापना जारी रखने के लिए तैयार हों, तो 'आगे' पर क्लिक करें.
WizardInfoAfter=जानकारी
InfoAfterLabel=कृपया जारी रखने से पहले निम्नलिखित महत्वपूर्ण जानकारी पढ़ें.
InfoAfterClickLabel=जब आप स्थापना जारी रखने के लिए तैयार हों, तो 'आगे' पर क्लिक करें.

; *** "User Information" wizard page
WizardUserInfo=उपयोगकर्ता जानकारी
UserInfoDesc=कृपया अपनी जानकारी दर्ज करें.
UserInfoName=उपयोगकर्ता का &नाम:
UserInfoOrg=संग&ठन:
UserInfoSerial=&सीरियल:
UserInfoNameRequired=आपको एक नाम दर्ज करना आवश्यक है.

; *** "Select Destination Location" wizard page
WizardSelectDir=गंतव्य फ़ोल्डर चुनें
SelectDirDesc=[name] कहाँ स्थापित किया जाए?
SelectDirLabel3=स्थापना [name] को निम्नलिखित फ़ोल्डर में स्थापित करेगी.
SelectDirBrowseLabel=जारी रखने के लिए 'आगे' पर क्लिक करें. यदि आप कोई अन्य फ़ोल्डर चुनना चाहते हैं, तो 'ब्राउज़' पर क्लिक करें.
DiskSpaceMBLabel=स्थापना के लिए कम से कम [mb] MB खाली स्थान आवश्यक है.
CannotInstallToNetworkDrive=स्थापना नेटवर्क ड्राइव पर नहीं की जा सकती.
CannotInstallToUNCPath=स्थापना UNC पथ पर नहीं की जा सकती.
InvalidPath=आपको पूर्ण पथ दर्ज करना होगा जिसमें ड्राइव अक्षर शामिल हो; उदाहरण:%n%nC:\APP%n%nया एक UNC पथ जैसे:%n%n\\server\share
InvalidDrive=आपके द्वारा चयनित ड्राइव या UNC शेयर मौजूद नहीं है या एक्सेस नहीं किया जा सकता. कृपया कोई अन्य चुनें.
DiskSpaceWarningTitle=अपर्याप्त डिस्क स्थान
DiskSpaceWarning=स्थापना के लिए कम से कम %1 KB की आवश्यकता है, लेकिन चयनित ड्राइव में केवल %2 KB उपलब्ध है.%n%nक्या आप जारी रखना चाहते हैं?
DirNameTooLong=फ़ोल्डर का नाम या पथ बहुत लंबा है.
InvalidDirName=अमान्य फ़ोल्डर नाम.
BadDirName32=फ़ोल्डर नाम में निम्नलिखित वर्ण नहीं होने चाहिए:%n%n%1
DirExistsTitle=फ़ोल्डर पहले से मौजूद है
DirExists=फ़ोल्डर:%n%n%1%n%nपहले से मौजूद है. क्या आप इस फ़ोल्डर का उपयोग करना चाहते हैं?
DirDoesntExistTitle=फ़ोल्डर मौजूद नहीं है
DirDoesntExist=फ़ोल्डर:%n%n%1%n%nमौजूद नहीं है. क्या आप इसे बनाना चाहते हैं?

; *** "Select Components" wizard page
WizardSelectComponents=अतिरिक्त घटक चुनें
SelectComponentsDesc=कौन से अतिरिक्त घटक स्थापित किए जाने चाहिए?
SelectComponentsLabel2=अपने पसंदीदा अतिरिक्त घटक चुनें; जिनकी आप स्थापना नहीं चाहते उन्हें हटाएं. जब आप तैयार हों तो 'आगे' पर क्लिक करें.
FullInstallation=पूर्ण स्थापना
CompactInstallation=बेसिक स्थापना
CustomInstallation=कस्टम स्थापना
NoUninstallWarningTitle=कुछ घटक पहले से स्थापित हैं
NoUninstallWarning=स्थापना बाधित की गई क्योंकि आपके कंप्यूटर पर निम्नलिखित अतिरिक्त घटक पहले से स्थापित हैं:%n%n%1%n%nइन घटकों को हटाने से इन्हें पुनः स्थापित नहीं किया जाएगा.%n%nक्या आप जारी रखना चाहते हैं?
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceMBLabel=इस विकल्प के लिए कम से कम [mb] MB खाली स्थान आवश्यक है.

; *** "Select Additional Tasks" wizard page
WizardSelectTasks=अतिरिक्त कार्य चुनें
SelectTasksDesc=कौन से अतिरिक्त कार्य जोड़े जाने चाहिए?
SelectTasksLabel2=उस कार्य को चुनें जिसे आप [name] की स्थापना के दौरान जोड़ना चाहते हैं, फिर 'आगे' पर क्लिक करें.

; *** "Select Start Menu Folder" wizard page
WizardSelectProgramGroup=स्टार्ट मेन्यू फ़ोल्डर चुनें
SelectStartMenuFolderDesc=प्रोग्राम का शॉर्टकट स्टार्ट मेन्यू में कहाँ रखा जाना चाहिए?
SelectStartMenuFolderLabel3=स्थापना स्टार्ट मेन्यू में निम्नलिखित फ़ोल्डर में प्रोग्राम के शॉर्टकट बनाएगी.
SelectStartMenuFolderBrowseLabel=जारी रखने के लिए 'आगे' पर क्लिक करें. यदि आप कोई अन्य फ़ोल्डर चुनना चाहते हैं, तो 'ब्राउज़' पर क्लिक करें.
MustEnterGroupName=आपको एक फ़ोल्डर का नाम दर्ज करना होगा.
GroupNameTooLong=फ़ोल्डर का नाम या पथ बहुत लंबा है.
InvalidGroupName=अमान्य फ़ोल्डर नाम.
BadGroupName=फ़ोल्डर नाम में निम्नलिखित वर्ण नहीं होने चाहिए:%n%n%1
NoProgramGroupCheck2=&स्टार्ट मेन्यू में फ़ोल्डर न बनाएं

; *** "Ready to Install" wizard page
WizardReady=स्थापना के लिए तैयार
ReadyLabel1=आपके कंप्यूटर पर [name] की स्थापना के लिए तैयार है.
ReadyLabel2a=स्थापना जारी रखने के लिए 'स्थापना' पर क्लिक करें, या समीक्षा/परिवर्तन के लिए 'पिछला' पर क्लिक करें.
ReadyLabel2b=स्थापना जारी रखने के लिए 'स्थापना' पर क्लिक करें.
ReadyMemoUserInfo=उपयोगकर्ता जानकारी:
ReadyMemoDir=गंतव्य फ़ोल्डर:
ReadyMemoType=स्थापना प्रकार:
ReadyMemoComponents=चयनित अतिरिक्त घटक:
ReadyMemoGroup=स्टार्ट मेन्यू में फ़ोल्डर:
ReadyMemoTasks=अतिरिक्त कार्य:

; *** "Preparing to Install" wizard page
WizardPreparing=स्थापना के लिए तैयारी
PreparingDesc=आपके कंप्यूटर पर [name] की स्थापना की तैयारी हो रही है.
PreviousInstallNotCompleted=पिछली स्थापना (या अनइंस्टॉल) अधूरी है. स्थापना पूरी करने के लिए आपको कंप्यूटर को पुनरारंभ करना होगा.%n%nपुनरारंभ के बाद, कृपया [name] की स्थापना पूरी करने के लिए इंस्टॉलर को पुनः चलाएँ.
CannotContinue=स्थापना जारी नहीं रह सकती. बाहर निकलने के लिए 'रद्द करें' पर क्लिक करें.
ApplicationsFound=निम्नलिखित प्रोग्राम उन फ़ाइलों का उपयोग कर रहे हैं जिन्हें स्थापना द्वारा अद्यतन किया जाना है. कृपया इन्हें बंद करें.
ApplicationsFound2=निम्नलिखित प्रोग्राम उन फ़ाइलों का उपयोग कर रहे हैं जिन्हें स्थापना द्वारा अद्यतन किया जाना है. स्थापना से पहले इन्हें बंद करना आवश्यक है. स्थापना पूर्ण होने के बाद, इंस्टॉलर इन्हें फिर से खोल देगा.
CloseApplications=इन प्रोग्रामों को &स्वत: बंद करें
DontCloseApplications=&इन प्रोग्रामों को बंद न करें
ErrorCloseApplications=कुछ प्रोग्राम बंद नहीं हो सके. स्थापना जारी रखने से पहले कृपया उन प्रोग्रामों को बंद करें जो अद्यतन की जाने वाली फ़ाइलों का उपयोग कर रहे हैं.

; *** "Installing" wizard page
WizardInstalling=स्थापना चल रही है
InstallingLabel=कृपया प्रतीक्षा करें, [name] की स्थापना आपके कंप्यूटर पर हो रही है.

; *** "Setup Completed" wizard page
FinishedHeadingLabel=[name] की स्थापना पूरी हुई
FinishedLabelNoIcons=[name] आपके कंप्यूटर पर सफलतापूर्वक स्थापित हो गया है.
FinishedLabel=[name] आपके कंप्यूटर पर सफलतापूर्वक स्थापित हो गया है. इसे उसके आइकन पर क्लिक करके चलाया जा सकता है.
ClickFinish=स्थापना से बाहर निकलने के लिए 'समाप्त' पर क्लिक करें.
FinishedRestartLabel=स्थापना पूरी करने के लिए, आपके कंप्यूटर को पुनरारंभ करना होगा. क्या आप पुनरारंभ करना चाहते हैं?
FinishedRestartMessage=स्थापना पूरी करने के लिए, आपके कंप्यूटर को पुनरारंभ करना होगा.%n%nक्या आप पुनरारंभ करना चाहते हैं?
ShowReadmeCheck=हाँ, मैं README फ़ाइल देखना चाहता हूँ
YesRadio=&हाँ, अभी कंप्यूटर पुनरारंभ करें
NoRadio=&नहीं, मैं बाद में पुनरारंभ करूँगा
RunEntryExec=%1 चलाएं
RunEntryShellExec=%1 प्रदर्शित करें

; *** "Setup Needs the Next Disk" stuff
ChangeDiskTitle=अगला डिस्क आवश्यक
SelectDiskLabel2=कृपया %1 डिस्क डालें और OK पर क्लिक करें.%n%nयदि उस डिस्क पर फ़ाइलें कहीं और स्थित हैं, तो सही पथ दर्ज करें या 'ब्राउज़' पर क्लिक करें.
PathLabel=&पथ:
FileNotInDir2=फ़ाइल "%1" "%2" में नहीं पाई गई. कृपया सही डिस्क डालें या कोई अन्य फ़ोल्डर चुनें.
SelectDirectoryLabel=कृपया अगले डिस्क का स्थान निर्दिष्ट करें.

; *** Installation phase messages
SetupAborted=स्थापना पूरी नहीं हुई.%n%nकृपया त्रुटि को सुधारें और स्थापना को पुनः चलाएँ.
EntryAbortRetryIgnore=पुनः प्रयास करने के लिए 'पुनः प्रयास करें', छोड़ने के लिए 'छोड़ें' (अनुशंसित नहीं), या स्थापना रद्द करने के लिए 'रद्द करें' पर क्लिक करें.

; *** Installation status messages
StatusClosingApplications=प्रोग्राम बंद किए जा रहे हैं...
StatusCreateDirs=फ़ोल्डर बनाए जा रहे हैं...
StatusExtractFiles=फ़ाइलें निकाली जा रही हैं...
StatusCreateIcons=शॉर्टकट बनाए जा रहे हैं...
StatusCreateIniEntries=INI प्रविष्टियाँ बनाई जा रही हैं...
StatusCreateRegistryEntries=Registry प्रविष्टियाँ बनाई जा रही हैं...
StatusRegisterFiles=फ़ाइलें सत्यापित की जा रही हैं...
StatusSavingUninstall=अनइंस्टॉल जानकारी सहेजी जा रही है...
StatusRunProgram=स्थापना पूरी की जा रही है...
StatusRestartingApplications=प्रोग्राम पुनः प्रारंभ किए जा रहे हैं...
StatusRollback=परिवर्तनों को पूर्ववत किया जा रहा है...

; *** Misc. errors
ErrorInternal2=त्रुटि: %1
ErrorFunctionFailedNoCode=%1 विफल हुआ
ErrorFunctionFailed=%1 विफल हुआ; कोड %2
ErrorFunctionFailedWithMessage=%1 विफल हुआ; कोड %2.%n%3
ErrorExecutingProgram=प्रोग्राम निष्पादित करने में विफल:%n%1

; *** Registry errors
ErrorRegOpenKey=Registry कुंजी खोलने में त्रुटि:%n%1\%2
ErrorRegCreateKey=Registry कुंजी बनाने में त्रुटि:%n%1\%2
ErrorRegWriteKey=Registry कुंजी में लिखने में त्रुटि:%n%1\%2

; *** INI errors
ErrorIniEntry=फ़ाइल "%1" में INI प्रविष्टि बनाने में त्रुटि.

; *** File copying errors
FileAbortRetryIgnore=पुनः प्रयास करने के लिए 'पुनः प्रयास करें', फ़ाइल छोड़ने के लिए 'छोड़ें' (अनुशंसित नहीं), या स्थापना रद्द करने के लिए 'रद्द करें' पर क्लिक करें.
FileAbortRetryIgnore2=पुनः प्रयास करने के लिए 'पुनः प्रयास करें', त्रुटि छोड़ने के लिए 'छोड़ें' (अनुशंसित नहीं), या स्थापना रद्द करने के लिए 'रद्द करें' पर क्लिक करें.
SourceIsCorrupted=स्रोत फ़ाइल भ्रष्ट है
SourceDoesntExist=स्रोत फ़ाइल "%1" मौजूद नहीं है
ExistingFileReadOnly=यह फ़ाइल केवल-पढ़ने के रूप में चिह्नित है.%n%nकेवल-पढ़ने का चिह्न हटाने और पुनः प्रयास करने के लिए 'पुनः प्रयास करें', छोड़ने के लिए 'छोड़ें', या स्थापना रद्द करने के लिए 'रद्द करें' पर क्लिक करें.
ErrorReadingExistingDest=फ़ाइल पढ़ने में त्रुटि:
FileExists=फ़ाइल पहले से मौजूद है.%n%nक्या आप इसे बदलना चाहते हैं?
ExistingFileNewer=यह फ़ाइल उस फ़ाइल से नई है जिसे स्थापना द्वारा स्थापित किया जाना है. आपको इसे रखना आवश्यक है.%n%nक्या आप इसे रखना चाहते हैं?
ErrorChangingAttr=फ़ाइल की विशेषताएँ बदलने में त्रुटि:
ErrorCreatingTemp=गंतव्य फ़ोल्डर में अस्थायी फ़ाइल बनाने में त्रुटि:
ErrorReadingSource=स्रोत फ़ाइल पढ़ने में त्रुटि:
ErrorCopying=फ़ाइल कॉपी करते समय त्रुटि:
ErrorReplacingExistingFile=फ़ाइल बदलने में त्रुटि:
ErrorRestartReplace=पुनरारंभ के बाद फ़ाइल बदलना विफल रहा:
ErrorRenamingTemp=गंतव्य फ़ोल्डर में अस्थायी फ़ाइल का नाम बदलने में त्रुटि:
ErrorRegisterServer=DLL/OCX पंजीकृत करने में विफल: %1
ErrorRegSvr32Failed=RegSvr32 विफल, निकास कोड %1
ErrorRegisterTypeLib=टाइप लाइब्रेरी पंजीकृत करने में विफल: %1

; *** Post-installation errors
ErrorOpeningReadme=README फ़ाइल खोलने में त्रुटि.
ErrorRestartingComputer=कंप्यूटर पुनरारंभ करने में विफल. कृपया मैन्युअल रूप से पुनरारंभ करें.

; *** Uninstaller messages
UninstallNotFound=फ़ाइल "%1" नहीं मिली. अनइंस्टॉल नहीं किया जा सकता.
UninstallOpenError=फ़ाइल "%1" खोलने में विफल. अनइंस्टॉल नहीं किया जा सकता.
UninstallUnsupportedVer=अनइंस्टॉलर लॉग फ़ाइल "%1" असमर्थित प्रकार की है. अनइंस्टॉल नहीं किया जा सकता.
UninstallUnknownEntry=अनइंस्टॉलर लॉग में अज्ञात प्रविष्टि (%1) मिली.
ConfirmUninstall=क्या आप वाकई %1 और उससे संबंधित सभी घटकों को अनइंस्टॉल करना चाहते हैं?
UninstallOnlyOnWin64=यह अनइंस्टॉल केवल Windows 64-bit संस्करण पर किया जा सकता है.
OnlyAdminCanUninstall=यह अनइंस्टॉल केवल व्यवस्थापक द्वारा किया जा सकता है.
UninstallStatusLabel=कृपया प्रतीक्षा करें, %1 को आपके कंप्यूटर से अनइंस्टॉल किया जा रहा है.
UninstalledAll=%1 को आपके कंप्यूटर से सफलतापूर्वक अनइंस्टॉल कर दिया गया है.
UninstalledMost=%1 का अनइंस्टॉल पूरा हुआ है.%n%nकुछ घटकों को अनइंस्टॉल नहीं किया जा सका. इन्हें मैन्युअल रूप से अनइंस्टॉल किया जा सकता है.
UninstalledAndNeedsRestart=अनइंस्टॉल पूरा करने के लिए, आपको कंप्यूटर को पुनरारंभ करना होगा.%n%nक्या आप पुनरारंभ करना चाहते हैं?
UninstallDataCorrupted=फ़ाइल "%1" भ्रष्ट है. अनइंस्टॉल नहीं किया जा सकता.

; *** Uninstallation phase messages
ConfirmDeleteSharedFileTitle=साझा फ़ाइल हटाएं?
ConfirmDeleteSharedFile2=सिस्टम ने संकेत दिया है कि निम्नलिखित साझा फ़ाइलें किसी अन्य प्रोग्राम द्वारा उपयोग में नहीं हैं. क्या आप इन्हें हटाना चाहते हैं?%n%nयदि कोई प्रोग्राम अभी भी इन फ़ाइलों का उपयोग कर रहा है, तो ये ठीक से काम नहीं कर सकतीं. यदि आप सुनिश्चित नहीं हैं, तो 'नहीं' चुनें. आपके सिस्टम से इन्हें हटाने से कोई नुकसान नहीं होगा.
SharedFileNameLabel=फ़ाइल का नाम:
SharedFileLocationLabel=स्थान:
WizardUninstalling=अनइंस्टॉल स्थिति
StatusUninstalling=%1 को अनइंस्टॉल किया जा रहा है...

; *** Shutdown block reasons
ShutdownBlockReasonInstallingApp=%1 की स्थापना चल रही है.
ShutdownBlockReasonUninstallingApp=%1 का अनइंस्टॉल चल रहा है.

[CustomMessages]

NameAndVersion=%1 संस्करण %2
AdditionalIcons=अतिरिक्त शॉर्टकट:
CreateDesktopIcon=डेस्कटॉप पर शॉर्टकट बनाएँ
CreateQuickLaunchIcon=क्विक लॉन्च पर शॉर्टकट बनाएँ
ProgramOnTheWeb=%1 वेब पर
UninstallProgram=%1 का अनइंस्टॉल
LaunchProgram=%1 चलाएं
AssocFileExtension=&%1 को %2 फ़ाइल एक्सटेंशन के साथ जोड़ें
AssocingFileExtension=%1 को %2 फ़ाइल एक्सटेंशन के साथ जोड़ा जा रहा है...
AutoStartProgramGroupDescription=स्वचालित प्रारंभ:
AutoStartProgram=%1 को स्वचालित रूप से प्रारंभ करें
AddonHostProgramNotFound=%1 को चुने गए फ़ोल्डर में स्थापित नहीं किया जा सका.%n%nक्या आप जारी रखना चाहते हैं?
