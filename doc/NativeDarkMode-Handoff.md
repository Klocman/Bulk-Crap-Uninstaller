# WIP native dark mode: continuation guide

This is an unfinished, opt-in implementation for review and continuation. It grew
out of investigating the dark-mode problem discussed in
[PR #962](https://github.com/BCUninstaller/Bulk-Crap-Uninstaller/pull/962).
It does not implement that PR's broader core refactor or remove COM dependencies.

The branch is synchronized with upstream commit `30da609`, which includes the
v6.3 release tag (`2e53fc1`) and the following README ordering update.

## Start here

Read [build instructions and architecture](NativeDarkMode.md), then the
[regression checklist](NativeDarkMode-Checklist.md). The solution and supporting
projects target .NET 10 and use the repository's normal build/output path. Building
the complete solution requires Visual Studio 2026 full MSBuild with desktop/C++
tools and the .NET 10 SDK because three projects retain COM references. Launch
`bin/Release/AnyCPU/BCUninstaller.exe --dark-mode`. No switch means light mode, and
`--light-mode` overrides `--dark-mode`.

The executable retains its real elevation and uninstall behavior. The external
development host used for inspection disconnected execution actions; those
protections are not part of the application or this PR.

## Implemented scope

- Native .NET 10 dark-mode opt-in with scoped adapters for inspected main-window,
  Settings, Properties, wizard, progress and leftover-review controls.
- List headers/group captions, status rows, selection/link colors, monochrome
  icons, search fields/dropdowns, legend/treemap and progress lifecycle handling.
- Live high-contrast entry/recovery fixes, including main-window buttons and
  primary list checkboxes.

`source/BulkCrapUninstaller/Theming` contains the adapters. Shared-library changes
are also present in KlocTools, ObjectListView and SimpleTreeMap. The patch includes
some related layout/refresh fixes documented in the individual follow-up notes.
Tooltip rendering is now explicitly set through `ThemeManager` for ObjectListView
tooltip event handlers and the two known `ToolTip` owners in Properties/Settings
and the uninstall wizard.

## Latest validation and its limits

After the v6.3 synchronization on 2026-09-08, the .NET 10 application project
restored and compiled locally with the existing warnings. The updated test project
compiled and ran 56 tests: 52 passed, one was skipped, and three rating tests could
not reach `bugsklocman.ddns.net:7721` because the local application firewall blocks
that endpoint. Upstream's v6.3 CI passes the same suite. The v6.3 native launcher
also built successfully with the local MFC-equipped Visual Studio installation.
The refreshed full-solution CI run remains the authoritative post-merge check.

Before the solution-wide .NET 10 migration, the focused repair pass completed fresh
.NET 8 and .NET 10 builds. Fresh external-checker runs recorded 631
assertions: .NET 10 ordinary default/dark/light override (158/160/158) and the
.NET 8 dark bypass (155). A focused STA WinForms host recorded another 155
assertions across .NET 10 dark/light-override and .NET 8 bypass runs (79/38/38).
It checked native dialog names, roles, checked state and initial focus; both known
`ToolTip` owners; ObjectListView cell/header tooltip colors; dialog mapping and
owner-bound worker-thread marshalling; and current/legacy Feedback HTML fixtures.
The dark Feedback fixture and a native tooltip window were rendered to bitmaps and
checked for dark backgrounds and contrasting text colors. These are local
observations, not CI results. An earlier eight-run contrast matrix recorded 1,313
assertions, including button and checkbox replays, but was not repeated in this
repair pass.

The environment was Windows 11 build 26100 at 200% DPI, with Desktop runtimes
10.0.11 and 8.0.30. The visual host loaded the built application assemblies and
exercised actual form classes, but did not run the production entry point.
A subsequent read-only replay populated all four Properties pages from one
installed entry in dark, light and both controlled contrast palettes; no
production defect was found.
A second replay covered nested/context menus and the actual main/progress toolbar
overflow in the same palettes. No split/drop-down button controls exist in source,
and no production defect was found in the applicable surfaces.
The light-contrast test applied the installed hcwhite.theme color table while
high contrast was active; it was not native Settings selection of that theme.

A separate network-dependent dark smoke passed 85 assertions. Because the local
application firewall intentionally blocks `NativeDarkRepairCheck.exe`, the same
compiled harness DLL was launched through the allowed .NET host. The current page
loaded, its `wsite-content` DOM was adapted, its computed background matched the
dark system color, and the rendered content remained readable. The blocked apphost
run produced a themed navigation-error document as expected; that result was not
treated as evidence about MSHTML or the remote page.

A bounded production acceptance pass staged hash-identical copies of the fresh
outputs in disposable portable directories. The exact .NET 10 executable followed
its manifest/UAC elevation path and reached a responsive dark inventory, closing
the production-launch check. Exact .NET 10 runs with no switch and with
`--dark-mode --light-mode`, plus the historical .NET 8 executable, reached ready light
inventories. The two .NET 10 light captures were byte-identical. A second exact dark
executable exited with code 0 after 3.2 seconds while the first remained responsive
and the only BCU process.

Windows integrity isolation prevented interactive automation of those elevated
windows. The remaining UI campaign ran the same managed entry point through the
medium-integrity .NET host. It covered a real 260-entry inventory, grouping, refresh,
multiple-app Properties, all seven Settings pages, live dialogs, the uninstall
wizard through its final summary, keyboard navigation, clipboard commands and
native Save dialogs. The wizard was canceled explicitly on page 5 of 5 before any
worker, cleanup or uninstall action started. Its related and running-app pages
auto-skipped; returning Back also reset the 7-Zip exclusion before it was reapplied.
Those limits leave the detailed production-executable rows open.

The managed entry point saved `es-AR`, closed normally, and relaunched into a ready
Spanish dark inventory. The English and Spanish restart confirmations exposed
localized headings, details and named Yes/No buttons without visible truncation.
English was restored and the app closed normally again; the portable settings file
parsed successfully afterward. This is useful culture and close evidence, but it
does not substitute for an elevated exact-executable `Application.Restart` cycle.

Two checked applications produced byte-identical XML exports before and after a
filter hid one of them. Both exports and the low-sensitivity clipboard command kept
both names, and a Properties value was written through the native Save dialog. The
filtered status count used the visible checked count while selected size and export
used the complete checked set, exposing a pre-existing display mismatch. Local
ignored evidence is under `artifacts/production-acceptance-20260905-025500`.

The external prototype harnesses, raw logs, inventory screenshots and local
`artifacts/` directories mentioned in the follow-up documents are not included
in this PR. Those references describe historical evidence, not files available
in this checkout. The focused repair checker is also a local ignored artifact.
The build helper and implementation are included; the automated assertion counts
cannot currently be reproduced from this checkout alone.
Making an isolated, portable regression harness is therefore a useful review task.

Existing build warnings NU1510 and SYSLIB0057 remain unresolved. The documented
all-bad-confidence leftover-filtering issue is separate and unchanged.

## Bounded next investigations

1. Tooltip rendering and contrast, followed by native dialogs and HTML surfaces.
   - Completed in this handoff: ObjectListView tooltip events and both known BCU
     `ToolTip` components use theme system colors. Inspected `OK`/`OKCancel`
     `MessageBox` flows use the themed dialog in dark mode; unsupported layouts and
     unowned worker-thread calls retain the native implementation. Owner-bound calls
     marshal to the UI thread. `FeedbackWindow` handles both the current Weebly DOM
     and legacy IDs, using literal RGB foreground/background CSS in dark mode. The
     live current page loaded and rendered through a host permitted by the local
     application firewall.
2. Keyboard-only navigation, visible focus and screen-reader metadata on those
   repaired surfaces.
   - Programmatically checked in this handoff: the standard WinForms accessibility
     providers expose localized control names, roles and checkbox checked state, and
     custom dialogs focus the first visible enabled action. The Feedback browser is
     keyboard reachable and receives focus after loading. A real Narrator or other
     screen-reader announcement pass remains open.

3. Broader main-window keyboard and screen-reader checks.
   - Completed in this handoff: an all-keyboard managed-entry-point pass covered
     search focus/clearing, list movement and checking, the row context menu,
     Tab/Shift+Tab and Properties. Observable behavior passed, although UI Automation
     reported stale search focus after some transfers. A real screen-reader
     announcement pass remains open.

4. Next target: repeat the managed UI campaign from a same-integrity production
   executable, including exact restart/culture handling, then cover the wizard's
   related/running-process branches and direct Settings opening from Progress.

After these investigations, consolidate findings before extending individual
adapters. The release checklist still contains 31 open checks, many overlapping
validation areas rather than known implementation defects: real startup and
elevation, packaging/helpers, Windows and DPI coverage, localization/RTL, resource
stability, and disposable uninstall/backup/cleanup workflows.

Persisted preference, automatic system-theme following, runtime migration and
default-on rollout remain design decisions. This draft is not ready to merge as
a release feature.
