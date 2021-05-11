/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace Klocman.Native
{
    public enum CSIDL
    {
        //Application Data
        //C:\Documents and Settings\Administrator\Application Data
        CSIDL_APPDATA = 0x1a,

        //CD Burning
        //C:\Documents and Settings\Administrator\Local Settings\Application Data\Microsoft\CD Burning
        CSIDL_CDBURN_AREA = 0x3b,

        //Common Administrative Tools
        //C:\Documents and Settings\All Users\Start Menu\Programs\Administrative Tools
        CSIDL_COMMON_ADMINTOOLS = 0x2f,

        //Common Application Data
        //C:\Documents and Settings\All Users\Application Data
        CSIDL_COMMON_APPDATA = 0x23,

        //Common Desktop
        //C:\Documents and Settings\All Users\Desktop
        CSIDL_COMMON_DESKTOPDIRECTORY = 0x19,

        //Common Documents
        //C:\Documents and Settings\All Users\Documents
        CSIDL_COMMON_DOCUMENTS = 0x2e,

        //Common Favorites
        //C:\Documents and Settings\All Users\Favorites
        CSIDL_COMMON_FAVORITES = 0x1f,

        //Common Music
        //C:\Documents and Settings\All Users\Documents\My Music
        CSIDL_COMMON_MUSIC = 0x35,

        //Common Pictures
        //C:\Documents and Settings\All Users\Documents\My Pictures
        CSIDL_COMMON_PICTURES = 0x36,

        //Common Start Menu
        //C:\Documents and Settings\All Users\Start Menu
        CSIDL_COMMON_STARTMENU = 0x16,

        //Common Start Menu Programs
        //C:\Documents and Settings\All Users\Start Menu\Programs
        CSIDL_COMMON_PROGRAMS = 0x17,

        //Common Startup
        //C:\Documents and Settings\All Users\Start Menu\Programs\Startup
        CSIDL_COMMON_STARTUP = 0x18,

        //Common Templates
        //C:\Documents and Settings\All Users\Templates
        CSIDL_COMMON_TEMPLATES
            = 0x2d,

        //Common Video
        //C:\Documents and Settings\All Users\Documents\My Videos
        CSIDL_COMMON_VIDEO = 0x37,

        //Cookies
        //C:\Documents and Settings\Administrator\Cookies
        CSIDL_COOKIES = 0x21,

        //Desktop
        //C:\Documents and Settings\Administrator\Desktop
        CSIDL_DESKTOPDIRECTORY = 0x10,

        //Favorites
        //C:\Documents and Settings\Administrator\Favorites
        CSIDL_FAVORITES = 0x06,

        //Fonts
        //C:\WINDOWS\Fonts
        CSIDL_FONTS = 0x14,

        //History
        //C:\Documents and Settings\Administrator\Local Settings\History
        CSIDL_HISTORY = 0x22,

        //Local Application Data
        //C:\Documents and Settings\Administrator\Local Settings\Application Data
        CSIDL_LOCAL_APPDATA = 0x1c,

        //My Documents
        //C:\Documents and Settings\Administrator\My Documents
        CSIDL_PERSONAL = 0x05,

        //My Music
        //C:\Documents and Settings\Administrator\My Documents\My Music
        CSIDL_MYMUSIC = 0x0d,

        //My Pictures
        //C:\Documents and Settings\Administrator\My Documents\My Pictures
        CSIDL_MYPICTURES = 0x27,

        //NetHood
        //C:\Documents and Settings\Administrator\NetHood
        CSIDL_NETHOOD = 0x13,

        //PrintHood
        //C:\Documents and Settings\Administrator\PrintHood
        CSIDL_PRINTHOOD = 0x1b,

        //Profile Folder
        //C:\Documents and Settings\Administrator
        CSIDL_PROFILE = 0x28,

        //Program Files
        //C:\Program Files
        CSIDL_PROGRAM_FILES = 0x26,

        //Program Files - Common
        //C:\Program Files\Common Files
        CSIDL_PROGRAM_FILES_COMMON = 0x2b,

        //Recent
        //C:\Documents and Settings\Administrator\Recent
        CSIDL_RECENT = 0x08,

        //Send To
        //C:\Documents and Settings\Administrator\SendTo
        CSIDL_SENDTO = 0x09,

        //Start Menu
        //C:\Documents and Settings\Administrator\Start Menu
        CSIDL_STARTMENU = 0x0b,

        //Start Menu Programs
        //C:\Documents and Settings\Administrator\Start Menu\Programs
        CSIDL_PROGRAMS = 0x02,

        //Startup
        //C:\Documents and Settings\Administrator\Start Menu\Programs\Startup
        CSIDL_STARTUP = 0x07,

        //System Directory
        //C:\WINDOWS\system32
        CSIDL_SYSTEM = 0x25,

        //Templates
        //C:\Documents and Settings\Administrator\Templates
        CSIDL_TEMPLATES = 0x15,

        //Temporary Folder
        //C:\Documents and Settings\Administrator\Local Settings\Temp\
        // was empty at the listing, probably 0?

        //Temporary Internet Files
        //C:\Documents and Settings\Administrator\Local Settings\Temporary Internet Files
        CSIDL_INTERNET_CACHE = 0x20,

        //Windows Directory
        //C:\WINDOWS
        CSIDL_WINDOWS = 0x24
    }
}