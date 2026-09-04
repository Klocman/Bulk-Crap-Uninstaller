# ObjectListView column DPI scaling

Fixed columns now scale with DPI in the shared ObjectListView control. This applies
to the default .NET 8 application and the opt-in .NET 10 application, in both light
and dark modes. It is independent of the dark-theme adapters.

At 200%, the leftover Confidence column changes from 103 to 206 device pixels and
the progress Status column from 105 to 210. Their full labels are readable in the
inspected English dialogs. The existing fill column continues to occupy the remaining
client width.

## Behavior

- Initial designer widths and min/max limits are interpreted at 96 DPI, then scaled once.
- BeginInit/EndInit defer scaling if a native handle appears before designer columns are ready.
- Hidden columns participate. Columns added after initialization use current device-pixel units.
- Subsequent DPI callbacks retain unrounded logical dimensions, avoiding cumulative rounding drift.
- A changed user/programmatic width becomes the new logical width at the current DPI.
- Fill columns use the remaining client area; they are not multiplied from an old fill width.
- Recreating a handle, rebuilding visible columns and duplicate DPI callbacks do not rescale twice.
- DPI changes no longer call AutoResizeColumns, which could overwrite the user's column widths.

The implementation is in `source/ObjectListView/ObjectListView.Dpi.cs`, with small
lifecycle and state-serialization hooks in `ObjectListView.cs`. It does not change
the existing font-scaling workaround or add theme-specific conditions.

## Saved layouts

New XML states retain device-pixel widths and add an optional ColumnDpi field.
Restoring them at another DPI converts widths to that DPI. The existing column
order, visibility and sorting state remain in the same format. Older readers can
ignore the additional XML field and continue using device pixels.

Legacy states without ColumnDpi retain their pixel widths exactly (subject to the
control's existing width limits). Their original DPI is unknown, so guessing would
risk changing a user's deliberate sizing. An existing narrow saved layout therefore
needs a manual resize or layout reset; new saves then carry DPI metadata. There is
no automatic migration based on a guessed old display scale.

## Validation, 2026-09-04

Both application builds pass. An external checker against the compiled shared
library passes 21 assertions in each of three runs:

| Runtime | Startup control DPI | Result |
| --- | --- | --- |
| .NET 10.0.11 | 192, native per-monitor awareness | 21 passed |
| .NET 10.0.11 | 96, DPI-unaware process on a 200% display | 21 passed |
| .NET 8.0.30 | 192, native per-monitor awareness | 21 passed |

The checker exercises startup, hidden/locked/fill columns, native handle recreation,
manual and programmatic resizing, ten 120/144/96/192-DPI round trips, duplicate DPI
callbacks, visibility rebuild, DPI-tagged restore before handle creation, repeated
restore, legacy XML, an empty virtual list, early designer handles and late runtime
columns. The 21 assertions are the same suite repeated across the three runs, not
63 distinct scenarios.

The fractional transitions invoke the real protected DPI callback through a test
subclass. They do not emulate a complete Windows monitor move. Physical mixed-DPI
monitor transitions and native 100/125/150% display configurations remain release
gates. The 96-DPI process is bitmap-scaled by Windows on this display.

Visual inspection of the real compiled leftover and progress forms confirms full
Confidence/Status labels at native 192 DPI. The light leftover dialog also shows
the corrected widths. A native header drag changes the progress Status width from
210 to 280 pixels; regrouping preserves it and adjusts the fill column. No uninstall
or cleanup action was executed. The four existing theme smoke-check cases also pass.

External evidence is under `prototypes/ColumnDpiCheck`, `artifacts/column-dpi-check`
and the visual host's timestamped logs. Build identity for the final functional build:

- BCUninstaller.dll SHA-256: `D83AF186233FC332E6B0B7F770F7E18078AEDEDFE8A3C719E546DBE5AE2D048F`.
- ObjectListView.dll SHA-256: `7016ACFA11A44FEA15364DEC4E03569333C3F5B7196FAB1556986FAEA391AABF`.

The high-contrast icon issue found during the earlier platform investigation is
separate and remains open.
