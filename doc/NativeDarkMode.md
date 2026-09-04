# Experimental native dark-mode integration

This patch consolidates the Windows Forms adapters investigated against BCU's real
main window, Properties, Settings, uninstall wizard, progress and leftover review.
It is an opt-in developer integration, not a release-wide runtime migration.

For the current status, evidence limitations and next investigations, see the
[WIP continuation guide](NativeDarkMode-Handoff.md).

## Build and enable

The ordinary solution/application build remains .NET 8. Setting the GUI project's
`EnableNativeDarkMode=true` property selects .NET 10 for that project only. The
supporting libraries retain their .NET 8 targets. The opt-in GUI output is separated
under `bin/NativeDark` to avoid overwriting the default GUI output.

On Windows, with .NET 8 and 10 SDKs, the .NET 10 Desktop runtime and Visual Studio
desktop/C++ build tools installed, run:

```powershell
./Build-NativeDark.ps1 -DotNet10 'C:/path/to/dotnet10/dotnet.exe'
```

The script first builds the ordinary application/dependencies using VS MSBuild and
the .NET 8 SDK, including the existing COM reference. It then publishes the GUI
with the .NET 10 SDK and `BuildProjectReferences=false`, reusing those built libraries.
Do not skip the dependency build after changing shared libraries. This is a GUI
build helper; existing helper-executable/installer packaging still needs validation.

Launch the opt-in executable with `--dark-mode` to activate the adapters. With no
switch it stays light. `--light-mode` takes precedence if both are present. The
comparison is case-insensitive. The ordinary .NET 8 build ignores these theme
switches. No theme setting is persisted, and there is no live light/dark switching
or automatic system-theme following in this patch. In both runtime builds, scoped
monochrome icons now follow high-contrast colors, including changes while open.

The existing manifest, elevation requirements, single-instance mutex, application
settings, startup checks, scanning and uninstall/cleanup behavior remain intact.
This executable is the real application, not the safe fixture host. Do not treat
the earlier inspection harness's disconnected actions as properties of this build.

## Code organization

`Theming/ThemeManager.cs` is the entry point. Initialization runs once after the
entry point initializes text rendering and visual styles, before dependency setup
can display application windows. It sets the launch mode, optional shared-dialog
heading color and the two existing application-row palettes. High contrast prevents
initial opt-in; dark control adapters also check high contrast before applying.
The separate icon bindings remain active in high contrast, including classic mode.

Small constructor hooks opt individual, inspected surfaces into:

- Exposed ObjectListView header, selection, background and hyperlink styles.
- Known BCU monochrome menu/instruction icons, copied without mutating shared assets.
- Explicit Settings/wizard page backgrounds and Properties grid header/selection colors.
- The shared loading/progress handle-lifecycle workaround.
- BCU's existing custom message box for the leftover confidence-details OK dialog.

`NativeObjectListView` remains an ObjectListView subclass. In both builds it uses
system contrast colors for selected rows and hyperlink cells. On .NET 10, adapted
ordinary Details lists also opt into implicit theming,
repaint simple group-caption label rectangles after native drawing, and fill the
disabled background before upstream owner-drawn rows. It does not skip native group
drawing. Virtual lists, RTL and rich group layouts are outside the workaround.

`ProgressBarLifecycleAdapter` defers the same theme opt-out used in the tested
.NET 10 control until handle initialization completes, repeating it after recreation.
It ignores disposed/stale handles. It retains native progress values and painting.

The central service avoids repeated list/menu/progress registration with weak keys.
Image bindings retain shared source bitmaps and own only their recolored copies.
System-color notifications refresh them on the UI thread; disposal detaches the
notifications and releases copies. Colored artwork is rejected as a whole, and
installed-program icons are not passed to the monochrome helper. See the
[contrast icon follow-up](NativeDarkMode-Icons.md) for scope and verification.
The subsequent [selection/link follow-up](NativeDarkMode-ContrastStates.md) covers
highlighted menu icons, paired selection colors and live hyperlink-color recovery.
The [row/grid follow-up](NativeDarkMode-RowGrid.md) suppresses main-list status tints
in high contrast, restores them on recovery, and pairs Properties selection colors.
The [legend/treemap follow-up](NativeDarkMode-LegendTreemap.md) explains that contrast
behavior in the legend and paints neutral, outlined treemap tiles with system
selection colors while retaining the original geometry and category palette.

Two small support changes are included: optional heading colors for the shared
CustomMessageBox (null retains its old behavior), and a serialization annotation on
the existing runtime-only CustomNoteDialog property required by the .NET 10 analyzer.
The Settings cache explanation wraps to its available width in adapted mode.

The accompanying [shared column DPI fix](ColumnDpi.md) applies to both runtimes and
both light/dark modes: initial and hidden columns scale, manual widths survive DPI
callbacks, and new saved layouts include their DPI. Legacy saved pixel widths are
preserved rather than guessing their original DPI.

## Evidence and limitations

The source consolidation builds on isolated real-window probes on Windows 11 build
26100, DPI 192 (200%), .NET Desktop 10.0.11. Those probes exercised the main inventory
list/refresh, Properties, seven Settings pages, wizard selection/navigation, synthetic
progress states and leftover confidence review. Fixtures never validated actual
uninstall execution, process termination, registry backup or junk deletion.

The integrated application was built in both default .NET 8 and opt-in .NET 10 forms.
An external, unelevated checker loads those actual assemblies and checks default-off,
dark opt-in, light override, repeated application, shared image preservation, the
real leftover-review constructor hook, and progress recreation/pending-callback
disposal. It does not run the production entry point or modify its manifest.

An external visual host subsequently replayed the compiled integration DLL's main
window, Properties, seven Settings pages, wizard, progress and leftover review.
It verified the copied assembly's SHA-256 against the published DLL. No new theme
regression was observed in this scoped Windows 11 / 200% pass. See the
[compiled-DLL visual report](NativeDarkMode-Visual.md) for exact coverage and host
limitations; this does not validate the production entry point or elevation path.

Remaining release work is tracked in [the regression checklist](NativeDarkMode-Checklist.md).
The [integrated main-window replay](NativeDarkMode-MainReplay.md) fixes stale
treemap tiles after an empty search and legend overlap with scaled scrollbars,
and stabilizes neutral tile colors during contrast-mode filtering/reload.
The [search dropdown follow-up](NativeDarkMode-Dropdowns.md) refreshes native theme
associations and cached background colors for the two filter dropdowns during live
contrast changes, with ordinary dark-mode recovery.
The [application-cell follow-up](NativeDarkMode-RowCells.md) refreshes the background
copies created by hyperlink formatting, so entire status rows follow contrast
changes and recover together.
The [search-field follow-up](NativeDarkMode-Search.md) clears its stale native
background brush during palette changes, retaining system colors and search state.
The [main-button follow-up](NativeDarkMode-Buttons.md) uses built-in managed drawing
for standard buttons/sidebar checkboxes in high contrast and restores their
ordinary style afterward.
The [primary list-checkbox follow-up](NativeDarkMode-ListCheckboxes.md) uses current
contrast colors while preserving glyph geometry and ordinary renderer behavior.
A subsequent [DPI/high-contrast investigation](NativeDarkMode-Platform.md) confirmed
startup/transition theme guards, but found fixed-column truncation in both light
and dark modes and unreadable black icons in the dark high-contrast fallback.
The column-width issue is now addressed by the shared fix above. The icon follow-up
also fixes the observed black-on-dark failure and verifies live recovery; physical
monitor moves and the full native contrast-scheme matrix remain open.
A full production-executable matrix and an authorized complete-inventory/uninstall
test are still required. The .NET 10 build currently
reports NU1510 for the existing Microsoft.VisualBasic package reference and SYSLIB0057
for the existing certificate constructor. Neither warning is hidden by this patch.

Native borders, disabled text, tooltips and HTML content are not fully adapted.
Native MessageBox callers other than the one inspected leftover-details call can
still be light. The separate general TabControl descendant-theming issue has no
global workaround here; the real Settings dropdowns worked in the inspected flow.

The existing all-bad-confidence leftover list issue is not changed: its constructor
checks and disables Hide bad confidence, leaving every finding filtered out. It
reproduces in the original light flow and should receive a separate behavioral fix.

No prototype bootstrap, synthetic fixtures, source-copy substitutions, telemetry
overrides, scan exclusions, hard-coded language, action interceptors or local error
logging are included in this application patch.
