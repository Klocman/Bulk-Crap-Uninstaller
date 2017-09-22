/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Factory.InfoAdders
{
    public enum InfoAdderPriority
    {
        RunDeadLast = -2,
        RunLast = -1,
        Normal = 0,
        RunFirst = 1
    }
}