/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Factory.InfoAdders
{
    public interface IMissingInfoAdder
    {
        void AddMissingInformation(ApplicationUninstallerEntry target);
    }
}