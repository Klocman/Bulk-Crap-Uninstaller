[![Donate](https://img.shields.io/badge/donate-paypal-brightgreen.svg)](http://klocmansoftware.weebly.com/donate.html)
[![GitHub release](https://img.shields.io/github/release/klocman/Bulk-Crap-Uninstaller.svg)](https://github.com/Klocman/Bulk-Crap-Uninstaller/releases)
[![license](https://img.shields.io/github/license/klocman/Bulk-Crap-Uninstaller.svg)](https://github.com/Klocman/Bulk-Crap-Uninstaller/blob/master/Licence.txt)

### [:warning: Looking for maintainers :warning:](https://github.com/Klocman/Bulk-Crap-Uninstaller/discussions/289)

# Bulk Crap Uninstaller
Bulk Crap Uninstaller (or BCUninstaller) is a free (as in speech) program uninstaller. It excels at removing large amounts of applications with minimal user input. It can clean up leftovers, detect orphaned applications, run uninstallers according to premade lists, and much more! Even though BCU was made with IT pros in mind, by default it is so straight-forward that anyone can use it.

BCU is fully compatible with Windows Store Apps, Steam, Windows Features and has special support for many uninstalling systems (NSIS, InnoSetup, Msiexec, and many other). Check below for a full list of functions.

Bulk Crap Uninstaller is licensed under Apache 2.0 open source license, and can be used in both private and commercial settings for free and with no obligations, as long as no conditions of the license are broken.

[Visit the official homepage](https://www.bcuninstaller.com/) to see the full list of quirks and features!

[Read the online documentation](https://htmlpreview.github.io/?https://github.com/Klocman/Bulk-Crap-Uninstaller/blob/master/doc/BCU_manual.html) if you have any questions or issues (the help file included with all releases). If you didn't find an answer to your question, feel free to [open a new issue](https://github.com/Klocman/Bulk-Crap-Uninstaller/issues/new).

## System requirements
Starting with BCUninstaller v5+ at least Windows 7 SP1 with all Platform Updates (KB2670838, KB2533623, etc.) and .NET6 desktop runtime are required. If you get a DLL error on startup then try running Windows Update. If you get a framework error, install .NET6 either manually or through Windows Update.

_*Note: The Portable version does not require the .NET6 runtime to be installed, since it is included (that's why the portable version is so large)._

If you want to use BCUninstaller on Vista or older, compile the [legacy 4.x branch](https://github.com/Klocman/Bulk-Crap-Uninstaller/tree/legacy-4.x) or download the [latest available 4.x release](https://github.com/Klocman/Bulk-Crap-Uninstaller/releases/tag/v4.16). Make sure you have .Net Framework 4.5 installed with all available updates for your system (it is not installed on XP by default).

## Download
You can get the latest version from the releases page. Alternatively you can download it from one of these hosts:
- [Download from dAppCDN](https://dappcdn.com/download/utilities/bulk-crap-uninstaller)
- [Download from FossHub](https://www.fosshub.com/Bulk-Crap-Uninstaller.html)
- [Download from SourceForge](https://sourceforge.net/projects/bulk-crap-uninstaller/)

## How can I help?
Please check the [contribution](CONTRIBUTING.md) notes!

## Compiling
Development is done on Visual Studio 2022. The solution should just load and build without doing anything extra, provided necessary VS features are installed.
The installer is compiled with InnoSetup v6.4. To make a release you have to first run the `publish.bat` script.

## Screenshots
![preview](../gh-pages/assets/1.png)
![preview](../gh-pages/assets/4.png)
