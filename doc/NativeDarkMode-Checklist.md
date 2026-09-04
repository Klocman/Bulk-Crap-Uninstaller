# Native dark-mode regression checklist

Status as of 2026-09-04. A checked item records the specific evidence below; it does
not imply broader platform or real-execution coverage. Complete the open gates in
an appropriate disposable test environment before proposing a default-on release.

## Consolidated patch checks

- [x] Default .NET 8 GUI and dependency build succeeds with VS MSBuild.
- [x] Opt-in .NET 10 GUI publishes while dependencies retain .NET 8 targets.
- [x] Separate output paths preserve the default GUI binary.
- [x] .NET 10 runtime checker: no switch leaves adapters disabled.
- [x] .NET 10 runtime checker: --dark-mode enables adapters.
- [x] .NET 10 runtime checker: --light-mode overrides --dark-mode.
- [x] .NET 8 runtime checker: --dark-mode leaves adapters disabled.
- [x] Repeated initialization/list/image/menu application does not duplicate adaptation.
- [x] Shared bitmap pixels remain unchanged; real leftover constructor uses the new list and style hook.
- [x] Progress handle recreation and disposal with a pending callback finish without an exception.
- [ ] Launch the real production entry point through its normal elevation/startup path.
- [ ] Replay the full visual matrix below against the integrated executable.
- [ ] Verify packaging includes required runtimes/helpers and preserves installer upgrade behavior.
- [ ] Validate clean startup, close/restart, existing-instance handling and culture selection.

The assembly checker runs unelevated with empty/in-memory controls; it invokes no
production startup, inventory, backup or uninstall workflow. The .NET 10 cases used
runtime 10.0.11; the .NET 8 case used runtime 8.0.30, all at DPI 192, high contrast off.

## Visual matrix

Earlier probes passed the following on one Windows 11 / 200% configuration. These
items remain unchecked for the consolidated production executable.

A subsequent [compiled-DLL visual replay](NativeDarkMode-Visual.md) covered the
following narrower cases in an external unelevated host. These checks exercise the
integrated form classes and constructor hooks, but do not close the production
entry-point or full visual-matrix gates below.

- [x] Main inventory: 468 visible entries, row/legend/treemap colors, group captions, search, checkbox selection and ungrouping.
- [x] Filtered refresh: dark disabled list, two blue loading bars, preserved filter and restored input.
- [x] Properties: four pages, multiple fixture applications, grid and copy/save menu rendering; missing-data states only on specialist pages.
- [x] Settings: seven pages, language dropdown and wrapped cache explanation.
- [x] Wizard: related selection, confirmation, normal empty-process-page skip, options and summary; finish returns without an execution caller.
- [x] Progress: all eight synthetic statuses, grouped/ungrouped rendering, marquee-to-determinate recreation and dynamically replaced completion icon; no worker started.
- [x] Leftovers: mixed-confidence fixture, initial selection, context menu, confidence details and returned selection; no deletion.
- [x] Light comparison: same opt-in DLL with --light-mode, leftover review and progress/completion retain original light colors and assets.

The complete production-executable matrix remains:

- [ ] Main window: row palettes, legend/treemap consistency, selected and unchecked rows, links, menus/search.
- [ ] Grouped/ungrouped lists: captions, collapse/expand, sorting, keyboard and checkbox selection.
- [ ] Refresh: disabled list background, both progress bars, filtered/unfiltered completion and restored input.
- [ ] Properties: all pages, grid sorting/selection, copy/save menus and multiple applications.
- [ ] Settings: all seven pages, dropdowns, direct Interface opening, grouping binding, cache-text wrapping.
- [ ] Wizard: related/confirmation/process/options/summary pages, backward navigation, sorting, exclusions and totals.
- [ ] Progress: waiting/running/paused/completed/failed/skipped/protected/invalid, grouped rows and completion icon.
- [ ] Progress: continuous/marquee recreation, selected objects across updates and repeated dialog opening/closing.
- [ ] Leftovers: confidence groups, threshold selection, low-confidence warnings, details, filtering and checked results.
- [ ] Dialogs: headings, icons, default/cancel buttons, ownership, keyboard access, focus and long localized explanations.
- [ ] Light comparison: default build and opt-in build with no switch or --light-mode retain normal behavior.

## Platform and accessibility gates

The [platform investigation](NativeDarkMode-Platform.md) records real high-contrast
startup/transition/recovery and a 96-logical/192-native DPI comparison. It found
fixed-width column truncation in both light and dark modes, plus unreadable black
icons in the actual dark high-contrast fallback. The [column fix](ColumnDpi.md) now
addresses the first finding. The [icon fix](NativeDarkMode-Icons.md) addresses the
observed black-on-dark failure; physical DPI moves and other native contrast
palettes remain open.

- [x] Actual contrast startup suppresses dark opt-in; an already-dark process disables/re-enables its dark flags across the observed system transition.
- [x] Reproduced unchanged fixed-column pixel widths at control DPI 96 and 192, including the light-mode comparison.
- [x] Scale fixed/hidden columns and constraints; preserve user sizing through repeated DPI callback tests, handle recreation and saved-layout restore in .NET 8/10.
- [ ] Validate the column fix during physical mixed-DPI monitor moves and native 100/125/150% display sessions.
- [x] Recolor scoped monochrome icons on real dark high-contrast startup, live entry and recovery; dynamically replaced completion image remains readable.
- [x] Deterministic white/black/custom foreground, source preservation, colored-artwork exclusion and image binding disposal checks in both runtimes.
- [x] Selected dropdown icons, checked toolbar icons and selected rows in configured dark contrast and a controlled light system-color table; both runtimes checked while contrast is active.
- [x] Normal/visited/hovered link colors use HotTrack in high contrast; existing cells recover without rebuilding rows or losing selection. See [selection/link follow-up](NativeDarkMode-ContrastStates.md).
- [x] Application status rows use system contrast backgrounds and restore normal/colorblind tints; existing row identity, checks, selection and horizontal scroll survive.
- [x] Actual Properties selection pairs highlight colors across contrast palettes and data-source replacement. See [row/grid follow-up](NativeDarkMode-RowGrid.md).
- [x] Legend explains neutral status colors and stays opaque in contrast; category visibility and ordinary palette/fade recover.
- [x] Treemap uses outlined system-color tiles/selection without changing grouping, geometry or selected objects; cached brushes are released. See [legend/treemap follow-up](NativeDarkMode-LegendTreemap.md).
- [x] Integrated main-list filtering/reload, empty-result treemap clearing and borderless legend positioning in the scoped 200% DPI contrast replay. See [main-window follow-up](NativeDarkMode-MainReplay.md).
- [ ] Real Properties page data and keyboard-only main-window focus/navigation.
- [x] Correct selected text and popup palette in normal search-filter dropdowns after a live contrast transition. See [dropdown follow-up](NativeDarkMode-Dropdowns.md) for runtime/scope limits.
- [x] Refresh copied hyperlink/plain-cell backgrounds in live contrast with certificate highlighting enabled. See [cell-background follow-up](NativeDarkMode-RowCells.md).
- [x] Refresh the search edit's native background brush across live contrast changes, including disabled reload and recovery. See [search-field follow-up](NativeDarkMode-Search.md).
- [x] Use a palette-aware fallback for the main window's standard buttons/sidebar checkboxes, including keyboard focus and recovery. See [button follow-up](NativeDarkMode-Buttons.md) for native/automated reload coverage.
- [x] Correct primary list-checkbox painting in controlled contrast transitions; see [scope and remaining checks](NativeDarkMode-ListCheckboxes.md).
- [x] Correct checkbox columns in uninstall confirmation/related-app lists while preserving aspect putters and printing.
- [x] Adapt standard buttons/checkboxes in inspected Settings, Wizard, Properties, loading, Progress and Junk-review entry points.
- [ ] Review buttons and custom painting in uninspected windows.
- [ ] Native Settings selection of other contrast themes and overflow/split-button states.

- [ ] Windows versions supported by the intended release, including Windows 10.
- [ ] 100%, 125%, 150%, 200% DPI; moves between monitors of different DPI.
- [ ] High contrast at startup and when changed while windows are open; restart recovery.
- [ ] Keyboard-only navigation, visible focus, screen-reader names/state announcements.
- [ ] Long translations and RTL; fixed-width/truncated column labels remain usable.
- [ ] Verify unsupported virtual/RTL/rich-group lists retain their normal native rendering.
- [ ] Large inventories and repeated refresh/dialog cycles: memory/GDI-handle stability and responsiveness.

## Behavior and rollout gates

- [ ] Compare complete inventory under normal authorized elevation, including external helpers.
- [ ] Disposable uninstall fixture: quiet/loud, cancellation, skip, terminate, retries and worker concurrency.
- [ ] Restore points, sleep/shutdown prevention and walk-away dialogs through the real workflow.
- [ ] Junk discovery, process checks, backup/cancel/failure, preview and deletion against disposable targets.
- [ ] Clipboard and real save dialogs; selected versus filtered export semantics.
- [ ] Audit remaining uninspected windows and native MessageBox/HTML surfaces.
- [ ] Decide minimum runtime and servicing requirements; resolve packaging/build warnings as appropriate.
- [ ] Decide persisted preference/system-following scope separately; the current patch is startup-only and opt-in.

Keep the existing all-bad-confidence filtering bug separate from theme acceptance.
Do not interpret synthetic fixture completion as evidence of successful removal.
