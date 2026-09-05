/*
    EBUninstaller Pro - Windows Update Agent Interop
    Native COM Interop Definitions for .NET 8+ (No tlbimp/ResolveComReference required)
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinUpdateHelper
{
    public enum OperationResultCode
    {
        orcNotStarted = 0,
        orcInProgress = 1,
        orcSucceeded = 2,
        orcSucceededWithErrors = 3,
        orcFailed = 4,
        orcAborted = 5
    }

    [ComImport]
    [Guid("81685850-D706-420C-9A30-E5820305E3DC")]
    [CoClass(typeof(UpdateSessionClass))]
    public interface UpdateSession : IUpdateSession
    {
    }

    [ComImport]
    [Guid("81685850-D706-420C-9A30-E5820305E3DC")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IUpdateSession
    {
        [DispId(1)]
        string ClientApplicationID { get; set; }
        [DispId(2)]
        bool ReadOnly { get; }
        [DispId(3)]
        object WebProxy { get; set; }
        [DispId(4)]
        IUpdateSearcher CreateUpdateSearcher();
        [DispId(5)]
        dynamic CreateUpdateDownloader();
        [DispId(6)]
        IUpdateInstaller CreateUpdateInstaller();
    }

    [ComImport]
    [Guid("4CB43D7F-7EEE-4906-8698-60DA1C38F2FE")]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public class UpdateSessionClass
    {
    }

    [ComImport]
    [Guid("8F45ABF1-F124-4B22-9EF7-23D17F7CD503")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IUpdateSearcher
    {
        [DispId(1)]
        bool CanCheckForUpgrades { get; }
        [DispId(2)]
        dynamic ClientApplicationID { get; set; }
        [DispId(3)]
        bool IncludePotentiallySupersededUpdates { get; set; }
        [DispId(4)]
        dynamic ServerSelection { get; set; }
        [DispId(5)]
        dynamic BeginSearch(string criteria, object onCompleted, object state);
        [DispId(6)]
        dynamic EndSearch(object searchJob);
        [DispId(7)]
        dynamic EscapeString(string unescaped);
        [DispId(8)]
        dynamic QueryHistory(int startIndex, int count);
        [DispId(9)]
        ISearchResult Search(string criteria);
    }

    [ComImport]
    [Guid("D40CFF62-E08C-4498-941A-01E24F0F4181")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISearchResult
    {
        [DispId(1)]
        dynamic ResultCode { get; }
        [DispId(2)]
        dynamic RootCategories { get; }
        [DispId(3)]
        IUpdateCollection Updates { get; }
        [DispId(4)]
        dynamic Warnings { get; }
    }

    [ComImport]
    [Guid("07FDD239-B2C2-4473-AB62-E4B6CDDD7E39")]
    [CoClass(typeof(UpdateCollectionClass))]
    public interface UpdateCollection : IUpdateCollection
    {
    }

    [ComImport]
    [Guid("07FDD239-B2C2-4473-AB62-E4B6CDDD7E39")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IUpdateCollection : IEnumerable
    {
        [DispId(0)]
        IUpdate this[int index] { get; }
        [DispId(1)]
        int Count { get; }
        [DispId(2)]
        bool ReadOnly { get; }
        [DispId(3)]
        int Add(IUpdate value);
        [DispId(4)]
        void Clear();
        [DispId(5)]
        dynamic Copy();
        [DispId(6)]
        int IndexOf(IUpdate value);
        [DispId(7)]
        void Insert(int index, IUpdate value);
        [DispId(8)]
        void RemoveAt(int index);
        [DispId(-4)]
        new IEnumerator GetEnumerator();
    }

    [ComImport]
    [Guid("2EE48F22-AF3C-405E-B397-CD067BF1DB89")]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public class UpdateCollectionClass
    {
    }

    [ComImport]
    [Guid("6A5C5881-C70A-4D4E-9B67-9809CE70CF0A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IUpdate
    {
        [DispId(1)]
        string Title { get; }
        [DispId(2)]
        dynamic AutoSelectOnWebSites { get; }
        [DispId(3)]
        dynamic BundledUpdates { get; }
        [DispId(4)]
        dynamic CanRequireSource { get; }
        [DispId(5)]
        dynamic Categories { get; }
        [DispId(6)]
        dynamic Deadline { get; }
        [DispId(7)]
        dynamic DeltaCompressedContentAvailable { get; }
        [DispId(8)]
        dynamic DeltaCompressedContentPreferred { get; }
        [DispId(9)]
        dynamic Description { get; }
        [DispId(10)]
        dynamic EulaText { get; }
        [DispId(11)]
        dynamic HandlerID { get; }
        [DispId(12)]
        IUpdateIdentity Identity { get; }
        [DispId(13)]
        dynamic Image { get; }
        [DispId(14)]
        dynamic InstallationBehavior { get; }
        [DispId(15)]
        dynamic IsBeta { get; }
        [DispId(16)]
        dynamic IsDownloaded { get; }
        [DispId(17)]
        dynamic IsInstalled { get; }
        [DispId(18)]
        dynamic IsMandatory { get; }
        [DispId(19)]
        bool IsUninstallable { get; }
        [DispId(20)]
        dynamic Languages { get; }
        [DispId(21)]
        DateTime LastDeploymentChangeTime { get; }
        [DispId(22)]
        dynamic MaxDownloadSize { get; }
        [DispId(23)]
        dynamic MinDownloadSize { get; }
        [DispId(24)]
        dynamic MoreInfoUrls { get; }
        [DispId(25)]
        dynamic MsrcSeverity { get; }
        [DispId(26)]
        dynamic RecommendedCpuSpeed { get; }
        [DispId(27)]
        dynamic RecommendedHardDiskSpace { get; }
        [DispId(28)]
        dynamic RecommendedMemory { get; }
        [DispId(29)]
        dynamic ReleaseNotes { get; }
        [DispId(30)]
        dynamic SecurityBulletedIDs { get; }
        [DispId(31)]
        dynamic SupersededUpdateIDs { get; }
        [DispId(32)]
        string SupportUrl { get; }
        [DispId(33)]
        dynamic Type { get; }
        [DispId(34)]
        dynamic UninstallationNotes { get; }
        [DispId(35)]
        dynamic UninstallationBehavior { get; }
        [DispId(36)]
        dynamic UninstallationSteps { get; }
        [DispId(37)]
        dynamic KBArticleIDs { get; }
        [DispId(38)]
        dynamic DeploymentAction { get; }
    }

    [ComImport]
    [Guid("46297823-9940-4C09-AED9-CD3EA6D05968")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IUpdateIdentity
    {
        [DispId(1)]
        int RevisionNumber { get; }
        [DispId(2)]
        string UpdateID { get; }
    }

    [ComImport]
    [Guid("7B5668D8-21A0-4F44-AA50-6D60974D6F16")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IUpdateInstaller
    {
        [DispId(1)]
        dynamic ClientApplicationID { get; set; }
        [DispId(2)]
        bool IsForced { get; set; }
        [DispId(3)]
        dynamic ParentHwnd { get; set; }
        [DispId(4)]
        IUpdateCollection Updates { get; set; }
        [DispId(5)]
        dynamic HistoryInstallerName { get; set; }
        [DispId(6)]
        dynamic HistoryInstallStep { get; set; }
        [DispId(7)]
        bool IsBusy { get; }
        [DispId(8)]
        dynamic AllowSourcePrompts { get; set; }
        [DispId(9)]
        dynamic RebootRequiredBeforeInstallation { get; }
        [DispId(10)]
        dynamic BeginInstall(object onPrepared, object onCompleted, object state);
        [DispId(11)]
        dynamic BeginUninstall(object onPrepared, object onCompleted, object state);
        [DispId(12)]
        dynamic EndInstall(object value);
        [DispId(13)]
        dynamic EndUninstall(object value);
        [DispId(14)]
        dynamic Install();
        [DispId(15)]
        dynamic RunWizard(string dialogTitle);
        [DispId(16)]
        IInstallationResult Uninstall();
    }

    [ComImport]
    [Guid("A43C56D6-7451-48D4-B196-9E9B0452CE67")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IInstallationResult
    {
        [DispId(1)]
        int HResult { get; }
        [DispId(2)]
        dynamic RebootRequired { get; }
        [DispId(3)]
        OperationResultCode ResultCode { get; }
        [DispId(4)]
        dynamic GetUpdateResult(int updateIndex);
    }
}
