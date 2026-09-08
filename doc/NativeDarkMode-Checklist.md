# Native dark-mode regression checklist

Status as of 2026-09-08. A checked item records the specific evidence below; it does
not imply broader platform or real-execution coverage. Complete the open gates in
an appropriate disposable test environment before proposing a default-on release.

## Consolidated patch checks

- [x] Source baseline includes v6.3 and the current upstream master (`30da609`).
- [ ] Unified .NET 10 solution restores and builds with Visual Studio 2026 full MSBuild.
- [x] The special GUI framework override and separate native-dark output path are removed.
- [x] .NET 10 runtime checker: no switch leaves adapters disabled.
- [x] .NET 10 runtime checker: --dark-mode enables adapters.
- [x] .NET 10 runtime checker: --light-mode overrides --dark-mode.
- [x] Repeated initialization/list/image/menu application does not duplicate adaptation.
- [x] Shared bitmap pixels remain unchanged; real leftover constructor uses the new list and style hook.
- [x] Progress handle recreation and disposal with a pending callback finish without an exception.
- [x] Launch the real production entry point through its normal elevation/startup path.
- [ ] Replay the full visual matrix below against the integrated executable.
- [ ] Verify packaging includes required runtimes/helpers and preserves installer upgrade behavior.
- [ ] Validate clean startup, close/restart, existing-instance handling and culture selection.

The assembly checker runs unelevated with empty/in-memory controls; it invokes no
production startup, inventory, backup or uninstall workflow. The .NET 10 cases used
runtime 10.0.11 at DPI 192 with high contrast off. A historical .NET 8 bypass case
predates the solution-wide framework migration.

On 2026-09-05, a hash-identical staged .NET 10 `BCUninstaller.exe --dark-mode`
followed the application manifest/UAC path and reached a responsive elevated
inventory. Windows integrity isolation allowed pixel and top-level-window
observation but prevented Computer Use from driving the elevated controls. The
detailed replay therefore used `dotnet BCUninstaller.dll --dark-mode`, which invokes
the production managed entry point but does not close rows explicitly scoped to the
production executable.

The exact executable also reached ready light inventories with no switch and with
`--dark-mode --light-mode`. Those two .NET 10 light screenshots were byte-identical.
A historical .NET 8 output also remained light. A second exact dark executable exited with
code 0 after 3.2 seconds while the first remained responsive and the sole BCU
process. Exact-executable graceful close, `Application.Restart`, and interactive
culture handling remain open, so the combined lifecycle and light rows stay
unchecked.

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
- [ ] Light comparison: the normal build with no switch or --light-mode retains normal behavior.

## Platform and accessibility gates

The [platform investigation](NativeDarkMode-Platform.md) records real high-contrast
startup/transition/recovery and a 96-logical/192-native DPI comparison. It found
fixed-width column truncation in both light and dark modes, plus unreadable black
icons in the actual dark high-contrast fallback. The DPI issue is outside this
focused dark-mode change. The [icon fix](NativeDarkMode-Icons.md) addresses the
observed black-on-dark failure; physical DPI moves and other native contrast
palettes remain open.

- [x] Actual contrast startup suppresses dark opt-in; an already-dark process disables/re-enables its dark flags across the observed system transition.
- [x] Reproduced unchanged fixed-column pixel widths at control DPI 96 and 192, including the light-mode comparison.
- [x] Recolor scoped monochrome icons on real dark high-contrast startup, live entry and recovery; dynamically replaced completion image remains readable.
- [x] Deterministic white/black/custom foreground, source preservation, colored-artwork exclusion and image binding disposal checks.
- [x] Selected dropdown icons, checked toolbar icons and selected rows in configured dark contrast and a controlled light system-color table.
- [x] Normal/visited/hovered link colors use HotTrack in high contrast; existing cells recover without rebuilding rows or losing selection. See [selection/link follow-up](NativeDarkMode-ContrastStates.md).
- [x] Application status rows use system contrast backgrounds and restore normal/colorblind tints; existing row identity, checks, selection and horizontal scroll survive.
- [x] Actual Properties selection pairs highlight colors across contrast palettes and data-source replacement. See [row/grid follow-up](NativeDarkMode-RowGrid.md).
- [x] Legend explains neutral status colors and stays opaque in contrast; category visibility and ordinary palette/fade recover.
- [x] Treemap uses outlined system-color tiles/selection without changing grouping, geometry or selected objects; cached brushes are released. See [legend/treemap follow-up](NativeDarkMode-LegendTreemap.md).
- [x] Integrated main-list filtering/reload, empty-result treemap clearing and borderless legend positioning in the scoped 200% DPI contrast replay. See [main-window follow-up](NativeDarkMode-MainReplay.md).
- [x] Populate all four Properties pages from one real installed entry across dark, light and two live contrast palettes. See [Properties follow-up](NativeDarkMode-Properties.md).
- [x] Keyboard-only main-window focus/navigation.
- [x] Correct selected text and popup palette in normal search-filter dropdowns after a live contrast transition. See [dropdown follow-up](NativeDarkMode-Dropdowns.md) for runtime/scope limits.
- [x] Refresh copied hyperlink/plain-cell backgrounds in live contrast with certificate highlighting enabled. See [cell-background follow-up](NativeDarkMode-RowCells.md).
- [x] Refresh the search edit's native background brush across live contrast changes, including disabled reload and recovery. See [search-field follow-up](NativeDarkMode-Search.md).
- [x] Use a palette-aware fallback for the main window's standard buttons/sidebar checkboxes, including keyboard focus and recovery. See [button follow-up](NativeDarkMode-Buttons.md) for native/automated reload coverage.
- [x] Correct primary list-checkbox painting in controlled contrast transitions; see [scope and remaining checks](NativeDarkMode-ListCheckboxes.md).
- [x] Correct checkbox columns in uninstall confirmation/related-app lists while preserving aspect putters and printing.
- [x] Adapt standard buttons/checkboxes in inspected Settings, Wizard, Properties, loading, Progress and Junk-review entry points.
- [ ] Review buttons and custom painting in uninspected windows.
- [x] Render nested/context menus and actual main/progress toolbar overflow across dark, light and two live contrast palettes. Source contains no split/drop-down button controls. See [menu/overflow follow-up](NativeDarkMode-Menus.md).
- [ ] Native Settings selection of other contrast themes.

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
- [x] Clipboard and real save dialogs; selected versus filtered export semantics.
- [ ] Audit remaining uninspected windows and native MessageBox/HTML surfaces.
- [ ] Decide minimum runtime and servicing requirements; resolve packaging/build warnings as appropriate.
- [ ] Decide persisted preference/system-following scope separately; the current patch is startup-only and opt-in.

The keyboard-only main-window pass exercised Ctrl+F/F3/Escape search handling,
list movement and Space checking, the Menu-key context menu, Tab/Shift+Tab, and
Alt+Enter Properties. UI Automation retained a stale search-field focus report
after some transfers, while the visible filter, check state, context-menu focus and
Properties transitions established the behavior. The broader visible-focus and
screen-reader gate remains open.

With two applications checked, the Program name clipboard command and XML export
retained both applications before and after a filter hid one from view. Both native
Save-dialog XML exports were byte-identical, and a Properties grid value was saved
through the real Windows dialog. The filtered status count reported only the visible
checked row while selected size and export used the complete persistent checked set;
that pre-existing display mismatch is outside this theme pass.

Keep the existing all-bad-confidence filtering bug separate from theme acceptance.
Do not interpret synthetic fixture completion as evidence of successful removal.
