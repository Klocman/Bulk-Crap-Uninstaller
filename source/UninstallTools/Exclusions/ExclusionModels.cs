/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Exclusions and Whitelist Models
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.Exclusions
{
    public enum ExclusionRuleType
    {
        ApplicationName,
        Publisher,
        FilePath,
        FolderPath,
        RegistryPath,
        ServiceName
    }

    public sealed class ExclusionRule
    {
        public string RuleId { get; set; } = Guid.NewGuid().ToString("N");
        public ExclusionRuleType RuleType { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public override string ToString() => $"[{RuleType}] {Value} ({(IsEnabled ? "Active" : "Disabled")})";
    }
}
