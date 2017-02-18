[Setup]
Uninstallable=not IsPortable()
DisableProgramGroupPage=no

[CustomMessages]
InstallPortableTitle =Portable Installation
pl.InstallPortableTitle =Instalacja przenośna
de.InstallPortableTitle =Tragbar Installation
fr.InstallPortableTitle =Installation Portable      
hu.InstallPortableTitle =Portable telepítés    
sl.InstallPortableTitle =Prenosna namestitev

InstallTypeChoiceTitle =Installation type
pl.InstallTypeChoiceTitle =Typ instalacji
de.InstallTypeChoiceTitle =Installationstyp
fr.InstallTypeChoiceTitle =Type d'installation      
hu.InstallTypeChoiceTitle =Telepítési típus 
sl.InstallTypeChoiceTitle =Vrsta namestitve

InstallStandardTitle =Standard Installation
pl.InstallStandardTitle =Instalacja standardowa
de.InstallStandardTitle =Standardinstallation
fr.InstallStandardTitle =Installation Standard       
hu.InstallStandardTitle =Szabványos telepítés     
sl.InstallStandardTitle =Standardna namestitev

InstallPortableInfo =Portable installation will not register itself in the system, it will only extract files to the specified directory. You can set the directory to anything you''d like, for example to a USB drive. You will be able to move this directory freely since the whole app is inside of it.
pl.InstallPortableInfo =Instalacja przenośna nie zostanie zarejestrowana w systemie, zostaną tylko wypakowane pliki. Pliki mogą zostać wypakowane do dowolnego folderu i mogą być bez problemu przenoszone. Wszystkie pliki używane przez tą aplikację będą przechowywane w wybranym folderze.
de.InstallPortableInfo =Die Portable Installation registriert sich nicht selbst im System, es werden nur Dateien in das angegebene Verzeichnis entpackt. Sie können das Verzeichnis beliebig festlegen, z.B. auf einem USB-Laufwerk. Dieses Verzeichnis können Sie frei verschieben, da darin die gesamte Anwendung ist.
fr.InstallPortableInfo =L'installation Portable ne s'enregistrera pas elle-même dans le système, elle extraira seulement les fichiers dans le dossier spécifié. Vous pouvez placer le dossier sur tout ce que vous souhaitez, par exemple sur un lecteur USB. Vous pourrez déplacer librement ce dossier du moment que l'application complète se trouve à l'intérieur.
hu.InstallPortableInfo =A hordozható telepítés esetén a program nem jegyzi ba magát a rendszerbe, a fájljai csak kicsomagolásra kerülnek a megadott könyvtárba. Ez a könyvtár bárhol lehet, akár egy USB meghajtón is. Ezzel a könyvtárat bárhova magával viheti, mivel az az egész alkalmazást tartalmazza.
sl.InstallPortableInfo =Prenosna namestitev se ne bo registrirala v sistem, bo le kopirala datoteke v doloèen imenik. Lahko nastavite imenik na karkoli vam je všeè, na primer na USB pogon. T aimenik boste lahko premaknili, saj je celotna aplikacija znotraj le-tega.

InstallStandardInfo =This option will install BCUninstaller on your computer as a normal application. Standard uninstaller will be created and it will be visible under "Programs and Features" as well as in other third-party uninstallers.
pl.InstallStandardInfo =Ta aplikacja zostanie zainstalowana i zarejestrowana w tym systemie. Zostanie stworzony deinstalator i będzie on widoczny w panelu sterowania i innych deinstalatorach.
de.InstallStandardInfo =Diese Option installiert BCUninstaller auf Ihrem Computer als normale Anwendung. Ein normales Deinstallationsprogramm wird erstellt, und der Eintrag wird unter "Programme und Features" ebenso wie in anderen Drittanbieter-Uninstallern angezeigt.
fr.InstallStandardInfo =Cette option installera BCUninstaller sur votre ordinateur comme une application normale. Un désinstalleur standard sera créé et sera visible sous "Programmes et Fonctionnalités" aussi bien que pour tout autre désinstalleur tiers.
hu.InstallStandardInfo =Ezzel a lehetõséggel úgy kerül telepítésre a BCUninstaller, mint egy normál alkalmazás. A szabványos eltávolítója megtalálható lesz a "Programok és szolgáltatások" ablakban, valamint az egyéb eltávolítókban.
sl.InstallStandardInfo =Ta možnost bo namestila BCUninstaller v vaš raèunalnik kot obièajno aplikacijo. Ustvarjen bo standarden odstranjevalce, ki bo viden znotraj skupine "Programi in funkcije" kot tudi v drugih odstranjevalcih tretjih oseb.

[Code]
var
  CustomPage: TWizardPage;
  StandardDescLabel: TLabel;
  {*StandardRadioButton: TNewRadioButton;  
  AdvancedRadioButton: TNewRadioButton;    *}
  AdvancedDescLabel: TLabel;
  StandardRadioButton: TNewRadioButton;  
  AdvancedRadioButton: TNewRadioButton;  

function IsPortable(): Boolean;
begin
  if(StandardRadioButton.Checked = True) then
  Result := False
  else
  Result := True;
end;
function IsNotPortable(): Boolean;
begin
  if(StandardRadioButton.Checked = True) then
  Result := True
  else
  Result := False;
end;

procedure InitializeWizard;     

begin
  CustomPage := CreateCustomPage(wpWelcome, CustomMessage('InstallTypeChoiceTitle'), '');
  StandardRadioButton := TNewRadioButton.Create(WizardForm);
  StandardRadioButton.Parent := CustomPage.Surface;
  StandardRadioButton.Checked := True;
  StandardRadioButton.Top := 16;
  StandardRadioButton.Width := CustomPage.SurfaceWidth;
  StandardRadioButton.Font.Style := [fsBold];
  StandardRadioButton.Font.Size := 9;
  StandardRadioButton.Caption := CustomMessage('InstallStandardTitle');
  StandardDescLabel := TLabel.Create(WizardForm);
  StandardDescLabel.Parent := CustomPage.Surface;
  StandardDescLabel.Left := 8;
  StandardDescLabel.Top := StandardRadioButton.Top + StandardRadioButton.Height + 8;
  StandardDescLabel.Width := CustomPage.SurfaceWidth - 10; 
  StandardDescLabel.Height := 60;
  StandardDescLabel.AutoSize := False;
  StandardDescLabel.Wordwrap := True;
  StandardDescLabel.Caption := CustomMessage('InstallStandardInfo');
  AdvancedRadioButton := TNewRadioButton.Create(WizardForm);
  AdvancedRadioButton.Parent := CustomPage.Surface;
  AdvancedRadioButton.Top := StandardDescLabel.Top + StandardDescLabel.Height + 16;
  AdvancedRadioButton.Width := CustomPage.SurfaceWidth;
  AdvancedRadioButton.Font.Style := [fsBold];
  AdvancedRadioButton.Font.Size := 9;
  AdvancedRadioButton.Caption := CustomMessage('InstallPortableTitle');
  AdvancedDescLabel := TLabel.Create(WizardForm);
  AdvancedDescLabel.Parent := CustomPage.Surface;
  AdvancedDescLabel.Left := 8;
  AdvancedDescLabel.Top := AdvancedRadioButton.Top + AdvancedRadioButton.Height + 8;
  AdvancedDescLabel.Width := CustomPage.SurfaceWidth - 10;
  AdvancedDescLabel.Height := 60;
  AdvancedDescLabel.AutoSize := False;
  AdvancedDescLabel.Wordwrap := True;
  AdvancedDescLabel.Caption := CustomMessage('InstallPortableInfo');
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  // initialize result to not skip any page (not necessary, but safer)
  Result := False;

  if PageID = wpSelectProgramGroup then
    Result := IsPortable();       
end;