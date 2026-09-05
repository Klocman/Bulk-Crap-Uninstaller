/*
    EBUninstaller Pro - Quick System Optimization Wizard Test Suite
*/

using System;
using BulkCrapUninstaller.Forms.Wizards;
using NUnit.Framework;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class WizardAndOptimizationTests
    {
        [Test]
        public void TestOptimizationWizardCreationAndLifecycle()
        {
            var wizard = new QuickOptimizationWizard();
            Assert.IsNotNull(wizard);
            Assert.IsTrue(wizard.Text.Contains("EBUninstaller Pro"));

            wizard.Dispose();
        }
    }
}
