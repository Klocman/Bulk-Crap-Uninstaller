## How to file a bug report
Create a new issue with as much description of the problem as possible. Include steps needed for reproduction if the bug is not obvious.
It's also possible to submit bug reports anonymously to my [feedback form](http://klocmansoftware.weebly.com/feedback--contact.html).

## How to suggest a new feature
The same as bug report. Well-explained suggestions are preferred.

## How to donate
Check [README.md](README.md) or main window of BCUninstaller for the donate link. Only donate using the official donate links.

## How to help translate
Translations are stored in .resx files (almost all except for Resources.resx). It's suggested to use the [ResxTranslator](https://github.com/HakanL/resxtranslator) to translate these files. It will save you a lot of work. Most if not all of translators here use it.

### How-to list
1. Download latest version of the translation tool (ResxTranslator) from https://github.com/HakanL/resxtranslator
2. - If you are familiar with how git and GitHub works, you can create a branch, translate it, and then start a pull request.
   - Otherwise download the latest translation pack from the [releases](https://github.com/Klocman/Bulk-Crap-Uninstaller/releases).
3. Run the ResxTranslator and click "File > Open directory". Point it to the extracted translation pack or clone of your branch.
4. Click on any of the items in the left tree, it will open on the right. You will need to translate all of those items, most are quite small.
5. Put the translation into the column marked with your language's code. For example, if you are translating to German use the "de" column.
5. - For a list of all language codes click "Languages > Add > More Languages". If possible, use only neutral languages.
   - If possible, use a more general locale. For example try using "en" or "ru" instead of "en-GB" or "ru-RU".
   - If the column doesn't exist, click "Languages > Add > Your Code". If your code is not available, click "More Languages". 
   - You can hide columns for different languages using the checkboxes below the list.
6. Once in a while click "File > Save all modified" (or ctrl+s) to save your work in case something crashes.
7. Create a pull request or compress the folder you just modified to a .zip and send it back to me. If you don't have my e-mailaddress you can use the [feedback form](http://klocmansoftware.weebly.com/feedback--contact.html).

## How to set up your environment and run tests
Any modern version of Visual Studio should work. You might need to download download [this](https://github.com/Klocman/UpdateSystem) and [this library](https://sourceforge.net/p/kloctoolslibrary/) separately. 
Tests use the test framework included in new Visual Studio versions. Some of the tests require running as 64bit to pass, or some specific applications to be installed.
