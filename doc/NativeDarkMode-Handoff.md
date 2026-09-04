# WIP native dark mode: continuation guide

This is an unfinished, opt-in implementation for review and continuation. It grew
out of investigating the dark-mode problem discussed in
[PR #962](https://github.com/BCUninstaller/Bulk-Crap-Uninstaller/pull/962).
It does not implement that PR's broader core refactor or remove COM dependencies.

## Start here

Read [build instructions and architecture](NativeDarkMode.md), then the
[regression checklist](NativeDarkMode-Checklist.md). Build on Windows with
`Build-NativeDark.ps1` and launch `bin/NativeDark/publish/BCUninstaller.exe --dark-mode`.
The build needs Visual Studio desktop/C++ tooling plus .NET 8 and .NET 10 SDKs.
The default application stays on .NET 8. Only the optional GUI build targets
.NET 10; dependencies retain their existing targets. No switch means light mode,
and `--light-mode` overrides `--dark-mode`.

The executable retains its real elevation and uninstall behavior. The external
development host used for inspection disconnected execution actions; those
protections are not part of the application or this PR.

## Implemented scope

- Native .NET 10 dark-mode opt-in with scoped adapters for inspected main-window,
  Settings, Properties, wizard, progress and leftover-review controls.
- List headers/group captions, status rows, selection/link colors, monochrome
  icons, search fields/dropdowns, legend/treemap and progress lifecycle handling.
- Live high-contrast entry/recovery fixes, including main-window buttons and
  primary list checkboxes. Several accessibility fixes also apply to .NET 8.
- A shared ObjectListView column-DPI fix, documented separately in
  [ColumnDpi.md](ColumnDpi.md). This could be reviewed or extracted independently.

`source/BulkCrapUninstaller/Theming` contains the adapters. Shared-library changes
are also present in KlocTools, ObjectListView and SimpleTreeMap. The patch includes
some related layout/refresh fixes documented in the individual follow-up notes.

## Latest validation and its limits

Both default .NET 8 and opt-in .NET 10 builds passed. The latest external checker
recorded 1,313 assertions across eight runs: .NET 10 ordinary default/dark/light
override (158/160/158), .NET 8 dark bypass (155), and light/dark high contrast on
both runtimes (172/169 in each palette). Button and checkbox replays cover
mouse/Space checking, palette recovery and restoration of Windows appearance.
These are local observations, not CI results.

The environment was Windows 11 build 26100 at 200% DPI, with Desktop runtimes
10.0.11 and 8.0.30. The visual host loaded the built application assemblies and
exercised actual form classes, but did not run the production entry point.
The light-contrast test applied the installed hcwhite.theme color table while
high contrast was active; it was not native Settings selection of that theme.

The external prototype harnesses, raw logs, inventory screenshots and local
`artifacts/` directories mentioned in the follow-up documents are not included
in this PR. Those references describe historical evidence, not files available
in this checkout. The build helper and implementation are included; the automated
assertion counts cannot currently be reproduced from this checkout alone.
Making an isolated, portable regression harness is therefore a useful review task.

Existing build warnings NU1510 and SYSLIB0057 remain unresolved. The documented
all-bad-confidence leftover-filtering issue is separate and unchanged.

## Bounded next investigations

1. Properties pages with real application data; some specialist pages were
   inspected only in missing-data states.
2. Remaining menu/overflow/split-button states, tooltips, native dialogs and HTML
   surfaces. Avoid interpreting inspected dialog coverage as global coverage.
3. Keyboard-only navigation, visible focus and screen-reader names/check states
   across those surfaces.

After these investigations, consolidate findings before extending individual
adapters. The release checklist still contains 34 open checks, many overlapping
validation areas rather than known implementation defects: real startup and
elevation, packaging/helpers, Windows and DPI coverage, localization/RTL, resource
stability, and disposable uninstall/backup/cleanup workflows.

Persisted preference, automatic system-theme following, runtime migration and
default-on rollout remain design decisions. This draft is not ready to merge as
a release feature.
