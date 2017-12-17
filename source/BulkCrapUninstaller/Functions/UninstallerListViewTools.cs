/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Forms;
using BulkCrapUninstaller.Functions.Ratings;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Events;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.IO;
using Klocman.Localising;
using Klocman.Resources;
using Klocman.Tools;
using UninstallTools;
using UninstallTools.Controls;
using UninstallTools.Factory;
using UninstallTools.Lists;
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Functions
{
    internal static class Constants
    {
        public static Color VerifiedColor = Color.FromArgb(unchecked((int)0xffccffcc));
        public static Color UnverifiedColor = Color.FromArgb(unchecked((int)0xffbbddff));
        public static Color InvalidColor = Color.FromArgb(unchecked((int)0xffE0E0E0));
        public static Color UnregisteredColor = Color.FromArgb(unchecked((int)0xffffcccc));
        public static Color WindowsFeatureColor = Color.FromArgb(unchecked((int)0xffddbbff));
        public static Color WindowsStoreAppColor = Color.FromArgb(unchecked((int)0xffa3ffff));
    }

    internal class UninstallerListViewTools : IDisposable
    {
        private static readonly string RatingCacheFilename =
            Path.Combine(Program.AssemblyLocation.FullName, "RatingCashe.xml");

        private readonly FilterCondition _filteringFilterCondition = new FilterCondition { FilterText = string.Empty };

        private readonly UninstallerIconGetter _iconGetter = new UninstallerIconGetter();
        private readonly TypedObjectListView<ApplicationUninstallerEntry> _listView;
        private readonly List<object> _objectsToUpdate = new List<object>();

        private readonly UninstallerRatingManager _ratingManager
            = new UninstallerRatingManager(WindowsTools.GetUniqueUserId());

        private readonly MainWindow _reference;
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;
        private bool _abortPostprocessingThread;
        private ITestEntry _filteringOverride;
        private Thread _finalizerThread;
        private bool _firstRefresh = true;
        private bool _listRefreshIsRunning;
        private IEnumerable<ApplicationUninstallerEntry> _allUninstallers;

        public bool CheckIsAppDisposed()
        {
            return _listView.ListView.IsDisposed || _listView.ListView.Disposing
                || _reference.IsDisposed || _reference.Disposing;
        }

        internal UninstallerListViewTools(MainWindow reference)
        {
            _reference = reference;
            _listView = new TypedObjectListView<ApplicationUninstallerEntry>(reference.uninstallerObjectListView);
            SetupListView();

            _reference.filterEditor1.TargetFilterCondition = _filteringFilterCondition;

            // Start the processing thread when user changes the test certificates option
            _settings.Subscribe((x, y) =>
            {
                if (_firstRefresh)
                    return;
                if (y.NewValue) StartProcessingThread(FilteredUninstallers);
                else
                {
                    StopProcessingThread(false);

                    if (CheckIsAppDisposed()) return;

                    _listView.ListView.SuspendLayout();
                    _listView.ListView.RefreshObjects(
                        AllUninstallers.Where(u => u.IsCertificateValid(true).HasValue).ToList());
                    _listView.ListView.ResumeLayout();
                }
            }, x => x.AdvancedTestCertificates, this);

            // Refresh items marked as invalid after corresponding setting change
            _settings.Subscribe((x, y) =>
            {
                if (CheckIsAppDisposed()) return;

                if (!_firstRefresh)
                    _listView.ListView.RefreshObjects(AllUninstallers.Where(u => !u.IsValid).ToList());
            }, x => x.AdvancedTestInvalid, this);

            // Refresh items marked as orphans after corresponding setting change
            _settings.Subscribe((x, y) =>
            {
                if (CheckIsAppDisposed()) return;

                if (!_firstRefresh)
                    _listView.ListView.UpdateColumnFiltering();
            }, x => x.AdvancedDisplayOrphans, this);

            AfterFiltering += (x, y) => StartProcessingThread(FilteredUninstallers);

            UninstallerFileLock = new object();

            _reference.FormClosed += (x, y) =>
            {
                // Prevent the thread from accessing disposed resources before getting aborted.
                StopProcessingThread(false);
                ProcessRatingsFinalize();
            };

            _settings.Subscribe((sender, args) => ProcessRatingsInitialize(), x => x.MiscUserRatings, this);
            //ProcessRatingsInitialize(); Is always called once at the start by the above
        }

        public ITestEntry FilteringOverride
        {
            get { return _filteringOverride; }
            set
            {
                if (_filteringOverride == value) return;
                _filteringOverride = value;
                UpdateColumnFiltering();
            }
        }

        public IEnumerable<ApplicationUninstallerEntry> AllUninstallers
        {
            get { return _allUninstallers ?? Enumerable.Empty<ApplicationUninstallerEntry>(); }
            private set { _allUninstallers = value; }
        }

        public IEnumerable<ApplicationUninstallerEntry> FilteredUninstallers
            => CheckIsAppDisposed() ? AllUninstallers : _listView.ListView.FilteredObjects.Cast<ApplicationUninstallerEntry>();

        public bool FirstRefreshCompleted => !_firstRefresh;

        public bool ListRefreshIsRunning
        {
            get { return _listRefreshIsRunning; }
            private set
            {
                if (value != _listRefreshIsRunning)
                {
                    _listRefreshIsRunning = value;
                    ListRefreshIsRunningChanged?.Invoke(this, new ListRefreshEventArgs(value, !FirstRefreshCompleted));
                }
            }
        }

        /// <summary>
        ///     Faster than SelectedUninstallers.Count()
        /// </summary>
        public int SelectedUninstallerCount => _listView.ListView.CheckBoxes
            ? _listView.CheckedObjects.Count
            : _listView.SelectedObjects.Count;

        public IEnumerable<ApplicationUninstallerEntry> SelectedUninstallers => _listView.ListView.CheckBoxes
            ? _listView.CheckedObjects
            : _listView.SelectedObjects;

        /// <summary>
        ///     External lock to the uninstall system.
        /// </summary>
        public object UninstallerFileLock { get; set; }

        public void Dispose()
        {
            _iconGetter?.Dispose();
            StopProcessingThread(false);
            _ratingManager.Dispose();
        }

        private void ProcessRatingsFinalize()
        {
            if (_settings.Settings.MiscUserRatings)
            {
                new Thread(() =>
                {
                    try
                    {
                        _ratingManager.UploadRatings();
                    }
                    catch
                    {
                        //TODO: Handle this better?
                    }
                    try
                    {
                        _ratingManager.SerializeCache(RatingCacheFilename);
                    }
                    catch
                    {
                        FlushRatings();
                    }
                    _ratingManager.Dispose();
                })
                { IsBackground = false, Name = "ProcessRatingDispose_Thread" }.Start();
            }
            else
            {
                FlushRatings();
            }
        }

        private void ProcessRatingsInitialize()
        {
            if (_settings.Settings.MiscUserRatings)
            {
                new Thread(() =>
                {
                    try
                    {
                        _ratingManager.DeserializeCache(RatingCacheFilename);
                    }
                    catch (Exception ex)
                    {
                        FlushRatings();
                        PremadeDialogs.GenericError(ex);
                    }

                    // If _ratingManager has no ratings it means that deserialization failed so we need to fetch from db
                    // Otherwise fetch at most every few hours, unless user manually clears the cache
                    if (!WindowsTools.IsNetworkAvailable() || (_ratingManager.RatingCount > 0
                                                               &&
                                                               (DateTime.Now - _settings.Settings.MiscRatingCacheDate)
                                                                   .Duration() < _settings.Settings._CacheUpdateRate))
                        return;

                    try
                    {
                        _ratingManager.FetchRatings();
                        _settings.Settings.MiscRatingCacheDate = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                })
                { IsBackground = false, Name = "ProcessRatingInit_Thread" }.Start();
            }
            else
            {
                FlushRatings();
            }
        }

        private void FlushRatings()
        {
            try
            {
                _ratingManager.ClearRatings();
                UninstallerRatingManager.DeleteCache(RatingCacheFilename);
            }
            catch
            {
                //Ignore errors, the cashe won't be accessed anyways
            }

            _settings.Settings.MiscRatingCacheDate = DateTime.MinValue;
        }

        public event EventHandler AfterFiltering;
        public event EventHandler<ListRefreshEventArgs> ListRefreshIsRunningChanged;
        public event EventHandler<CountingUpdateEventArgs> UninstallerPostprocessingProgressUpdate;

        /*public bool DisplayWindowsFeatures()
        {
            if (ListRefreshIsRunning)
                return false;

            ListRefreshIsRunning = true;
            _reference.LockApplication(true);

            var error = LoadingDialog.ShowDialog(Localisable.LoadingDialogTitleLoadingWindowsFeatures, x =>
            {
                var items = UninstallManager.GetWindowsFeaturesList(y =>
                {
                    x.SetMaximum(y.TotalCount);
                    x.SetProgress(y.CurrentCount);
                });

                AllUninstallers =
                    AllUninstallers.Where(e => e.UninstallerKind != UninstallerType.Dism).Concat(items).ToList();
                _listView.ListView.SafeInvoke(() => _listView.ListView.SetObjects(AllUninstallers, false));
            });

            if (error != null)
                PremadeDialogs.GenericError(error);

            _reference.LockApplication(false);
            ListRefreshIsRunning = false;

            return error == null;
        }*/

        /// <summary>
        ///     Get total size of all visible uninstallers.
        /// </summary>
        public FileSize GetFilteredSize()
        {
            return FilteredUninstallers.Select(x => x.EstimatedSize).DefaultIfEmpty(FileSize.Empty)
                .Aggregate((size1, size2) => size1 + size2);
        }

        /// <summary>
        ///     Get total size of selected uninstallers
        /// </summary>
        /// <returns></returns>
        public FileSize GetSelectedSize()
        {
            return SelectedUninstallers.Select(x => x.EstimatedSize).DefaultIfEmpty(FileSize.Empty)
                .Aggregate((size1, size2) => size1 + size2);
        }

        public void InitiateListRefresh()
        {
            if (ListRefreshIsRunning || CheckIsAppDisposed())
                return;

            ListRefreshIsRunning = true;

            _reference.LockApplication(true);

            StopProcessingThread(false);
            if (CheckIsAppDisposed())
                return;

            _listView.ListView.SuspendLayout();
            _listView.ListView.BeginUpdate();

            var dialog = LoadingDialog.Show(_reference, Localisable.LoadingDialogTitlePopulatingList,
                ListRefreshThread, new Point(-35, -35), ContentAlignment.BottomRight);

            dialog.FormClosed += (sender, args) =>
            {
                if (dialog.Error != null)
                {
                    if (dialog.Error is OperationCanceledException)
                        return;
                    throw new Exception("Uncaught exception in ListRefreshThread", dialog.Error);
                }

                if (CheckIsAppDisposed() || args.CloseReason == CloseReason.WindowsShutDown ||
                    args.CloseReason == CloseReason.ApplicationExitCall ||
                    args.CloseReason == CloseReason.FormOwnerClosing ||
                    args.CloseReason == CloseReason.TaskManagerClosing) return;

                var oldList = _listView.ListView.SmallImageList;
                _listView.ListView.SmallImageList = _iconGetter.IconList;
                oldList?.Dispose();

                _listView.ListView.SetObjects(AllUninstallers);

                try
                {
                    _listView.ListView.EndUpdate();
                    _listView.ListView.ResumeLayout();
                    _listView.ListView.Focus();
                }
                catch (ObjectDisposedException)
                {
                }

                _reference.LockApplication(false);

                // Run events
                ListRefreshIsRunning = false;

                // Set after running events
                _firstRefresh = false;
            };

            dialog.StartWork();
        }


        public void DeselectAllItems(object sender, EventArgs e)
        {
            var selected = _listView.ListView.CheckBoxes ? _listView.CheckedObjects : _listView.SelectedObjects;
            var subtracted = selected.Except(FilteredUninstallers);
            ChangeSelection(subtracted);
        }

        private void ChangeSelection(IEnumerable<ApplicationUninstallerEntry> newSelection)
        {
            _listView.ListView.BeginUpdate();

            var items = newSelection.ToList();
            if (_listView.ListView.CheckBoxes)
                _listView.ListView.CheckedObjects = items;
            _listView.ListView.SelectedObjects = items;

            _listView.ListView.EndUpdate();
            _listView.ListView.Refresh();
            _listView.ListView.Focus();
        }

        public void InvertSelectedItems(object sender, EventArgs e)
        {
            var selected = _listView.ListView.CheckBoxes ? _listView.CheckedObjects : _listView.SelectedObjects;
            var inverted = FilteredUninstallers.Except(selected);
            ChangeSelection(inverted);
        }

        public void SelectAllItems(object sender, EventArgs e)
        {
            var selected = _listView.ListView.CheckBoxes ? _listView.CheckedObjects : _listView.SelectedObjects;
            var added = selected.Union(FilteredUninstallers);
            ChangeSelection(added);
        }

        public void RefreshList()
        {
            if (CheckIsAppDisposed())
                return;

            _listView.ListView.UpdateColumnFiltering();
            //_listView.ListView.BuildList(true); No need, UpdateColumnFiltering already does this
        }

        /// <summary>
        ///     Select first item starting with the keycode.
        ///     If keycode leads to a valid selection true is returned. Otherwise, if there is nothing relevant to select false is
        ///     returned.
        /// </summary>
        public bool SelectItemFromKeystroke(Keys keyCode)
        {
            var keyName = keyCode.ToLetterOrNumberString();

            if (keyName != null)
            {
                var selectedObj = FilteredUninstallers.FirstOrDefault(x => x.DisplayName.StartsWith(keyName, StringComparison.InvariantCultureIgnoreCase));

                _listView.ListView.DeselectAll();

                if (selectedObj != null)
                {
                    _listView.ListView.SelectObject(selectedObj, true);
                    _listView.ListView.EnsureModelVisible(selectedObj);

                    return true;
                }
            }
            return false;
        }

        public void StopProcessingThread(bool block)
        {
            if (_finalizerThread == null || !_finalizerThread.IsAlive) return;

            _abortPostprocessingThread = true;

            if (!block) return;

            do
            {
                Thread.Sleep(100);
                // Process events in case we are blocking ui thread and the worker thread is trying to invoke.
                // TODO Reimplement the whole thing to avoid having to do this
                Application.DoEvents();
            } while (_finalizerThread.IsAlive);
        }

        public void UpdateColumnFiltering()
        {
            _listView.ListView.EmptyListMsg = AllUninstallers.Any()
                ? Localisable.SearchNothingFoundMessage
                : null;

            _listView.ListView.UpdateColumnFiltering();
        }

        private void ListRefreshThread(LoadingDialogInterface dialogInterface)
        {
            dialogInterface.SetSubProgressVisible(true);
            var progressMax = 0;
            var uninstallerEntries = ApplicationUninstallerFactory.GetUninstallerEntries(x =>
            {
                progressMax = x.TotalCount + 2;
                dialogInterface.SetMaximum(progressMax);
                dialogInterface.SetProgress(x.CurrentCount, x.Message);

                var inner = x.Inner;
                if (inner != null)
                {
                    dialogInterface.SetSubMaximum(inner.TotalCount);
                    dialogInterface.SetSubProgress(inner.CurrentCount, inner.Message);
                }
                else
                {
                    dialogInterface.SetSubMaximum(-1);
                    dialogInterface.SetSubProgress(0, string.Empty);
                }

                if (dialogInterface.Abort)
                    throw new OperationCanceledException();
            });

            dialogInterface.SetProgress(progressMax - 1, Localisable.Progress_Finishing_Startup);
            dialogInterface.SetSubMaximum(StartupManager.Factories.Count);
            var i = 0;
            var startupEntries = new List<StartupEntryBase>();
            foreach (var factory in StartupManager.Factories)
            {
                dialogInterface.SetSubProgress(i++, factory.Key);
                try
                {
                    startupEntries.AddRange(factory.Value());
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }

            dialogInterface.SetProgress(progressMax, Localisable.Progress_Finishing, true);
            dialogInterface.SetSubMaximum(3);
            dialogInterface.SetSubProgress(0, string.Empty);

            if (!string.IsNullOrEmpty(Program.InstalledRegistryKeyName))
                uninstallerEntries.RemoveAll(x => PathTools.PathsEqual(x.RegistryKeyName, Program.InstalledRegistryKeyName));

            AllUninstallers = uninstallerEntries;

            dialogInterface.SetSubProgress(1, Localisable.MainWindow_Statusbar_RefreshingStartup);
            try
            {
                ReassignStartupEntries(false, startupEntries);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }

            dialogInterface.SetSubProgress(2, Localisable.Progress_Finishing_Icons);
            try
            {
                _iconGetter.UpdateIconList(AllUninstallers);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }

            dialogInterface.SetSubProgressVisible(false);

            // Fixes loading gettings stuck on finalizing if main window is minimized
            _reference.SafeInvoke(() =>
            {
                if (_reference.WindowState == FormWindowState.Minimized)
                    _reference.WindowState = FormWindowState.Normal;
            });
        }

        /// <summary>
        ///     Return a filter equivalent to current basic filtering settings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Filter> GenerateEquivalentFilter()
        {
            var results = new List<Filter>();

            if (string.IsNullOrEmpty(_filteringFilterCondition.FilterText))
                results.Add(new Filter("Include all", false, new FilterCondition("!",
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsOrphaned))
                { InvertResults = true }));
            else
                results.Add(new Filter(_filteringFilterCondition.FilterText, false,
                    (FilterCondition)_filteringFilterCondition.Clone()));

            if (_settings.Settings.FilterHideMicrosoft)
                results.Add(new Filter("Published by Microsoft", true, new FilterCondition("Microsoft",
                    ComparisonMethod.Contains, nameof(ApplicationUninstallerEntry.Publisher))));

            if (!_settings.Settings.FilterShowStoreApps)
                results.Add(new Filter("Store Apps", true, new FilterCondition(nameof(UninstallerType.StoreApp),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind))));

            if (!_settings.Settings.FilterShowWinFeatures)
                results.Add(new Filter("Windows Features", true, new FilterCondition(nameof(UninstallerType.WindowsFeature),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind))));

            if (!_settings.Settings.AdvancedDisplayOrphans)
                results.Add(new Filter("Orphaned apps", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsOrphaned))));

            if (!_settings.Settings.FilterShowProtected)
                results.Add(new Filter("Protected apps", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsProtected))));

            if (!_settings.Settings.FilterShowSystemComponents)
                results.Add(new Filter("System Components", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.SystemComponent))));

            if (!_settings.Settings.FilterShowUpdates)
                results.Add(new Filter("Updates", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsUpdate))));

            return results;
        }

        private bool ListViewFilter(object obj)
        {
            var entry = obj as ApplicationUninstallerEntry;

            if (entry == null) return false;

            if (FilteringOverride != null) return FilteringOverride.TestEntry(entry) == true;

            if (_settings.Settings.FilterHideMicrosoft && !string.IsNullOrEmpty(entry.Publisher) &&
                entry.Publisher.Contains("Microsoft"))
                return false;

            if (!_settings.Settings.FilterShowStoreApps && entry.UninstallerKind == UninstallerType.StoreApp)
                return false;

            if (!_settings.Settings.FilterShowWinFeatures && entry.UninstallerKind == UninstallerType.WindowsFeature)
                return false;

            if (!_settings.Settings.AdvancedDisplayOrphans && entry.IsOrphaned) return false;

            if (!_settings.Settings.FilterShowProtected && entry.IsProtected) return false;

            if (!_settings.Settings.FilterShowSystemComponents && entry.SystemComponent) return false;

            if (!_settings.Settings.FilterShowUpdates && entry.IsUpdate) return false;

            if (string.IsNullOrEmpty(_filteringFilterCondition.FilterText)) return true;

            return _filteringFilterCondition.TestEntry(entry) == true;
        }

        private void SetupListView()
        {
            _reference.uninstallerObjectListView.VirtualMode = false;

            _reference.olvColumnDisplayName.AspectName = ApplicationUninstallerEntry.RegistryNameDisplayName;
            _reference.olvColumnDisplayName.GroupKeyGetter = ListViewDelegates.GetFirstCharGroupKeyGetter;

            _reference.olvColumnDisplayName.ImageGetter = _iconGetter.ColumnImageGetter;

            _reference.olvColumnStartup.AspectGetter = x =>
            {
                var obj = x as ApplicationUninstallerEntry;
                return (obj?.StartupEntries != null && obj.StartupEntries.Any()).ToYesNo();
            };

            _reference.olvColumnPublisher.AspectName = ApplicationUninstallerEntry.RegistryNamePublisher;
            _reference.olvColumnPublisher.GroupKeyGetter = ListViewDelegates.ColumnPublisherGroupKeyGetter;

            _reference.olvColumnDisplayVersion.AspectName = ApplicationUninstallerEntry.RegistryNameDisplayVersion;
            _reference.olvColumnDisplayVersion.GroupKeyGetter = ListViewDelegates.DisplayVersionGroupKeyGetter;

            _reference.olvColumnUninstallString.AspectName = ApplicationUninstallerEntry.RegistryNameUninstallString;
            _reference.olvColumnUninstallString.GroupKeyGetter = ListViewDelegates.ColumnUninstallStringGroupKeyGetter;

            _reference.olvColumnInstallDate.AspectGetter = x =>
            {
                var obj = x as ApplicationUninstallerEntry;
                return obj?.InstallDate.Date ?? DateTime.MinValue;
            };
            //_reference.olvColumnInstallDate.AspectName = ApplicationUninstallerEntry.RegistryNameInstallDate;
            _reference.olvColumnInstallDate.AspectToStringConverter = x =>
            {
                if (!(x is DateTime)) return Localisable.Empty;
                var entry = (DateTime)x;
                return entry.IsDefault() ? Localisable.Empty : entry.ToShortDateString();
            };

            _reference.olvColumnGuid.AspectGetter = ListViewDelegates.ColumnGuidAspectGetter;
            _reference.olvColumnGuid.GroupKeyGetter = ListViewDelegates.ColumnGuidGroupKeyGetter;

            _reference.olvColumnSystemComponent.AspectName = ApplicationUninstallerEntry.RegistryNameSystemComponent;
            _reference.olvColumnSystemComponent.AspectToStringConverter = ListViewDelegates.BoolToYesNoAspectConverter;
            _reference.olvColumnSystemComponent.GroupKeyToTitleConverter = ListViewDelegates.BoolToYesNoAspectConverter;

            _reference.olvColumnIs64.AspectGetter =
                y => (y as ApplicationUninstallerEntry)?.Is64Bit.GetLocalisedName();

            _reference.olvColumnProtected.AspectToStringConverter = ListViewDelegates.BoolToYesNoAspectConverter;
            _reference.olvColumnProtected.GroupKeyToTitleConverter = ListViewDelegates.BoolToYesNoAspectConverter;

            _reference.olvColumnInstallLocation.AspectName = ApplicationUninstallerEntry.RegistryNameInstallLocation;
            _reference.olvColumnInstallLocation.GroupKeyGetter = ListViewDelegates.ColumnInstallLocationGroupKeyGetter;

            _reference.olvColumnInstallSource.AspectName = ApplicationUninstallerEntry.RegistryNameInstallSource;
            _reference.olvColumnInstallSource.GroupKeyGetter = ListViewDelegates.ColumnInstallSourceGroupKeyGetter;

            _reference.olvColumnRegistryKeyName.AspectName = "RegistryKeyName";

            _reference.olvColumnUninstallerKind.AspectGetter =
                y => (y as ApplicationUninstallerEntry)?.UninstallerKind.GetLocalisedName();

            _reference.olvColumnAbout.AspectName = "AboutUrl";
            _reference.olvColumnAbout.GroupKeyGetter = x =>
            {
                var entry = x as ApplicationUninstallerEntry;
                var aboutUri = entry?.GetAboutUri();
                return aboutUri?.Host ?? Localisable.Empty;
            };

            _reference.olvColumnQuietUninstallString.AspectName =
                ApplicationUninstallerEntry.RegistryNameQuietUninstallString;
            _reference.olvColumnQuietUninstallString.GroupKeyGetter =
                ListViewDelegates.ColumnQuietUninstallStringGroupKeyGetter;

            _reference.olvColumnSize.TextAlign = HorizontalAlignment.Right;
            _reference.olvColumnSize.AspectGetter = ListViewDelegates.ColumnSizeAspectGetter;
            _reference.olvColumnSize.AspectToStringConverter = ListViewDelegates.AspectToStringConverter;
            _reference.olvColumnSize.GroupKeyGetter = ListViewDelegates.ColumnSizeGroupKeyGetter;
            _reference.olvColumnSize.GroupKeyToTitleConverter = x => x.ToString();

            // Rating stuff
            _reference.olvColumnRating.AspectGetter = x =>
            {
                var entry = x as ApplicationUninstallerEntry;
                return string.IsNullOrEmpty(entry?.RatingId) ? RatingEntry.NotAvailable : _ratingManager.GetRating(entry.RatingId);
            };

            _reference.olvColumnRating.Renderer = new RatingRenderer();

            _reference.olvColumnRating.GroupKeyGetter = x =>
            {
                var model = x as ApplicationUninstallerEntry;

                if (!_settings.Settings.MiscUserRatings
                    || string.IsNullOrEmpty(model?.RatingId)
                    || _ratingManager.RatingCount <= 0)
                    return Localisable.NotAvailable;

                var rating = _ratingManager.GetRating(model.RatingId);

                if (rating.IsEmpty || (!rating.AverageRating.HasValue && !rating.MyRating.HasValue))
                    return CommonStrings.Unknown;

                return (rating.MyRating.HasValue ? "Your rating:" : "Average rating:") + " " +
                       RatingEntry.ToRating(rating.MyRating ?? (int)rating.AverageRating);
            };

            _reference.uninstallerObjectListView.CellClick += (x, y) =>
            {
                if (y.Column == null || (y.ModifierKeys != Keys.None) || !y.Column.Equals(_reference.olvColumnRating))
                    return;

                var model = y.Model as ApplicationUninstallerEntry;

                if (model == null)
                    return;

                RateEntries(new[] { model }, _reference.uninstallerObjectListView.PointToScreen(y.Location));
            };
            // Rating stuff end

            _reference.uninstallerObjectListView.PrimarySortColumn = _reference.olvColumnDisplayName;
            _reference.uninstallerObjectListView.SecondarySortColumn = _reference.olvColumnPublisher;
            _reference.uninstallerObjectListView.Sorting = SortOrder.Ascending;

            _reference.uninstallerObjectListView.AdditionalFilter = new ModelFilter(ListViewFilter);
            _reference.uninstallerObjectListView.UseFiltering = true;

            _reference.uninstallerObjectListView.FormatRow += UninstallerObjectListView_FormatRow;

            UninstallerPostprocessingProgressUpdate += (x, y) =>
            {
                lock (_objectsToUpdate)
                {
                    if (y.Tag != null)
                        _objectsToUpdate.Add(y.Tag);

                    if (y.Value == y.Maximum || y.Value % 35 == 0)
                    {
                        try
                        {
                            _listView.ListView.RefreshObjects(_objectsToUpdate);
                        }
                        catch (InvalidOperationException ex)
                        {
                            // The list view got disposed before we could update it.
                            _abortPostprocessingThread = true;
                            Debug.Fail(ex.Message, ex.StackTrace);
                        }
                        _objectsToUpdate.Clear();
                    }
                }
            };

            _listView.ListView.AfterSorting += (x, y) => { AfterFiltering?.Invoke(x, y); };
        }

        public void RateEntries(ApplicationUninstallerEntry[] entries, Point location)
        {
            if (!_settings.Settings.MiscUserRatings)
            {
                MessageBoxes.RatingsDisabled();
            }
            else if (!entries.Any() || entries.All(x => string.IsNullOrEmpty(x.RatingId)))
            {
                MessageBoxes.RatingUnavailable();
            }
            else
            {
                var title = entries.Length == 1
                    ? entries[0].DisplayName
                    : string.Format(CultureInfo.CurrentCulture, Localisable.RateTitle_Counted, entries.Length);

                var result = RatingPopup.ShowRateDialog(_reference, title, location);

                if (result == UninstallerRating.Unknown)
                    return;

                foreach (var entry in entries.Where(x => !string.IsNullOrEmpty(x.RatingId)))
                {
                    _ratingManager.SetMyRating(entry.RatingId, result);
                }
            }
        }

        private void StartProcessingThread(IEnumerable<ApplicationUninstallerEntry> itemsToProcess)
        {
            StopProcessingThread(true);

            _finalizerThread = new Thread(UninstallerPostprocessingThread)
            { Name = "UninstallerPostprocessingThread", IsBackground = true, Priority = ThreadPriority.Lowest };

            _abortPostprocessingThread = false;
            _finalizerThread.Start(itemsToProcess);
        }

        private void UninstallerObjectListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            var entry = e.Model as ApplicationUninstallerEntry;
            if (entry == null) return;

            if (entry.UninstallerKind == UninstallerType.WindowsFeature)
            {
                e.Item.BackColor = Constants.WindowsFeatureColor;
            }
            else if (entry.UninstallerKind == UninstallerType.StoreApp)
            {
                e.Item.BackColor = Constants.WindowsStoreAppColor;
            }
            else if (entry.IsOrphaned)
            {
                e.Item.BackColor = Constants.UnregisteredColor;
            }
            else if (!entry.IsValid && _settings.Settings.AdvancedTestInvalid)
            {
                e.Item.BackColor = Constants.InvalidColor;
            }
            else if (_settings.Settings.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    e.Item.BackColor = result.Value ? Constants.VerifiedColor : Constants.UnverifiedColor;
            }
        }

        private void UninstallerPostprocessingThread(object targets)
        {
            var items = targets as IEnumerable<ApplicationUninstallerEntry>;
            if (items == null)
                return;

            var targetList = items as IList<ApplicationUninstallerEntry> ?? items.ToList();
            var currentCount = 1;
            foreach (var uninstaller in targetList)
            {
                if (_abortPostprocessingThread)
                {
                    UninstallerPostprocessingProgressUpdate?.Invoke(this, new CountingUpdateEventArgs(0, 0, 0));
                    return;
                }

                var sendTag = true;
                if (_settings.Settings.AdvancedTestCertificates)
                {
                    lock (UninstallerFileLock)
                    {
                        sendTag = uninstaller.GetCertificate() != null;
                    }
                }

                var countingUpdateEventArgs = new CountingUpdateEventArgs(0, targetList.Count, currentCount);
                if (sendTag) countingUpdateEventArgs.Tag = uninstaller;

                UninstallerPostprocessingProgressUpdate?.Invoke(this, countingUpdateEventArgs);
                currentCount++;
            }
        }

        internal void ReassignStartupEntries(bool refreshListView, IEnumerable<StartupEntryBase> items)
        {
            // Using DoForEach to avoid multiple enumerations
            StartupManager.AssignStartupEntries(AllUninstallers
                .DoForEach(x => { if (x != null) x.StartupEntries = null; }), items);

            if (refreshListView)
                RefreshList();
        }

        public sealed class ListRefreshEventArgs : EventArgs
        {
            public ListRefreshEventArgs(bool value, bool firstRefresh)
            {
                NewValue = value;
                FirstRefresh = firstRefresh;
            }

            public bool NewValue { get; private set; }
            public bool FirstRefresh { get; private set; }
        }
    }
}