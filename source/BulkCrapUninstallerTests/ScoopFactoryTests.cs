using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Factory;
using UninstallTools.Factory.InfoAdders;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    [DoNotParallelize]
    public class ScoopFactoryTests
    {
        [TestMethod]
        public void CreateUninstallerEntry_InstalledArchitectureMissingFromManifest_UsesTopLevelFields()
        {
            const string appName = "bcu-synthetic-scoop-architecture-fallback";
            const string version = "1.0.0-synthetic";
            var scoopRoot = Path.Combine(Path.GetTempPath(), $"bcu-synthetic-scoop-{Guid.NewGuid():N}");
            var currentDirectory = Path.Combine(scoopRoot, "apps", appName, "current");
            var executablePath = Path.Combine(currentDirectory, "synthetic-app.exe");
            var scoopUserPathField = typeof(ScoopFactory).GetField("_scoopUserPath",
                BindingFlags.NonPublic | BindingFlags.Static);
            var powershellPathField = typeof(ScoopFactory).GetField("_powershellPath",
                BindingFlags.NonPublic | BindingFlags.Static);
            var scriptPathField = typeof(ScoopFactory).GetField("_scriptPath",
                BindingFlags.NonPublic | BindingFlags.Static);

            Assert.IsNotNull(scoopUserPathField);
            Assert.IsNotNull(powershellPathField);
            Assert.IsNotNull(scriptPathField);
            var originalScoopUserPath = scoopUserPathField.GetValue(null);
            var originalPowershellPath = powershellPathField.GetValue(null);
            var originalScriptPath = scriptPathField.GetValue(null);

            try
            {
                Directory.CreateDirectory(currentDirectory);
                File.WriteAllBytes(executablePath, Array.Empty<byte>());
                File.WriteAllText(Path.Combine(currentDirectory, "install.json"),
                    "{\"architecture\":\"64bit\"}");
                File.WriteAllText(Path.Combine(currentDirectory, "manifest.json"),
                    "{" +
                    $"\"version\":\"{version}\"," +
                    "\"shortcuts\":[[\"synthetic-app.exe\",\"Synthetic Scoop App\"]]," +
                    "\"bin\":\"synthetic-app.exe\"," +
                    "\"env_add_path\":\"synthetic-tools\"," +
                    "\"architecture\":{\"arm64\":{\"url\":\"https://example.invalid/synthetic.zip\"}}" +
                    "}");
                scoopUserPathField.SetValue(null, scoopRoot);
                powershellPathField.SetValue(null, Path.Combine(scoopRoot, "synthetic-powershell.exe"));
                scriptPathField.SetValue(null, Path.Combine(scoopRoot, "synthetic-scoop.ps1"));

                var result = ScoopFactory.CreateUninstallerEntry(appName, version, false,
                    new AppExecutablesSearcher());

                Assert.IsNotNull(result);
                Assert.IsTrue(result.GetSortedExecutables().Contains(executablePath));
            }
            finally
            {
                scoopUserPathField.SetValue(null, originalScoopUserPath);
                powershellPathField.SetValue(null, originalPowershellPath);
                scriptPathField.SetValue(null, originalScriptPath);
                if (Directory.Exists(scoopRoot))
                    Directory.Delete(scoopRoot, true);
            }
        }
    }
}
