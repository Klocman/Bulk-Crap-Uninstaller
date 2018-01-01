[![Donate](https://img.shields.io/badge/donate-paypal%20%2F%20bitcoin-pink.svg)](http://klocmansoftware.weebly.com/donate.html)
[![Download Bulk Crap Uninstaller](https://img.shields.io/sourceforge/dm/bulk-crap-uninstaller.svg)](https://sourceforge.net/projects/bulk-crap-uninstaller/files/latest/download)
[![GitHub release](https://img.shields.io/github/release/klocman/Bulk-Crap-Uninstaller.svg)](https://github.com/Klocman/Bulk-Crap-Uninstaller/releases)
[![license](https://img.shields.io/github/license/klocman/Bulk-Crap-Uninstaller.svg)](https://github.com/Klocman/Bulk-Crap-Uninstaller/blob/master/Licence.txt)
[![Issues](https://img.shields.io/github/issues/klocman/Bulk-Crap-Uninstaller.svg)](https://github.com/Klocman/Bulk-Crap-Uninstaller/issues)

# Bulk-Crap-Uninstaller
Bulk Crap Uninstaller (or BCUninstaller) is a free (as in speech) program uninstaller. It excels at removing large amounts of applications with minimal user input. It can clean up leftovers, detect orphaned applications, run uninstallers according to premade lists, and much more! Even though BCU was made with IT pros in mind, by default it is so straight-forward that anyone can use it.

BCU is fully compatible with Windows Store Apps, Steam, Windows Features and has special support for many uninstalling systems (NSIS, InnoSetup, Msiexec, and many other).

[Visit official homepage](http://klocmansoftware.weebly.com/)

If you need any help please either read the help file included with all releases, or the project's [wiki page](https://github.com/Klocman/Bulk-Crap-Uninstaller/wiki)!

## Download
[![Download at FossHub](https://cloud.githubusercontent.com/assets/14913904/25586209/a84a224e-2e9e-11e7-9332-5f913a9d7cd8.png)](https://www.fosshub.com/Bulk-Crap-Uninstaller.html)

[Download at SourceForge](https://sourceforge.net/p/bulk-crap-uninstaller/)

## System requirements
* OS: Windows Vista or newer is recommended. BCU will also work on XP and 2003 with reduced functionality and possibly some bugs. Both 32bit and 64bit versions are supported.
* .NET: Recommended .NET 4.0 or newer, can run on only .NET 3.5 with reduced functionality.
* RAM: Around 300MB or more of free RAM.
* CPU: Doesn't really matter.
* Free space: 40MB or more (most of it is for temporary update files).
* 7200RPM HDD or better is recommended or scan times can get very long depending on configuration. (not as bad in v4.1 thanks to caching)

## Screenshots
![preview](https://user-images.githubusercontent.com/14913904/34364884-93bcf34e-ea8a-11e7-9aa2-bb229631498a.png)

## Features
* Translated to French, Polish, Slovene, Czech, English, German, Russian. More coming!
* Detect and quietly Windows Store apps
* Detect and quietly uninstall Steam Apps
* Uninstall multiple items at once to speed up the process (with collision prevention)
* Uninstall any number of applications in a single batch
* Minimal user input is required during uninstallation
* Can find and remove leftovers after uninstallation
* Can uninstall many applications even if they don't have any uninstallers
* Detects applications with damaged or missing uninstallers
* Adds quiet uninstall options to some uninstallers, even if they do not support them by default
* Uninstall lists for automation
* Startup manager
* Verification of uninstaller certificates
* Fully portable, settings are saved to a single file

## How can I help?
Please read the [CONTRIBUTING.md](CONTRIBUTING.md) note.

## Compiling
Any modern version of Visual Studio should work. You might need to download [this](https://github.com/Klocman/UpdateSystem) and [this library](https://sourceforge.net/p/kloctoolslibrary/) separately.
