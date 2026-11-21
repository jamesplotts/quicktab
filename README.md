# Quick Tab

Source: https://github.com/jamesplotts/quicktab
Download: https://www.nexusmods.com/railroader/mods/2379 (or the GitHub releases)

A Railroader 0.6 mod. Right-click a car and open its window straight on the tab you want, instead of the Car tab:

| Car | Menu item | Opens on |
|---|---|---|
| Freight cars (box, gondola, hopper, flat, tank, stock, ...) | **Waybill** | Operations |
| Passenger cars | **Passengers** | Passenger |
| Maintenance of way cars | **MOW** | MOW |
| Locomotives, tenders, cabooses | none | |

**Ctrl+click** on a car opens it on the same tab. The game's **Inspect** item still opens the Car tab.

## Install

Copy this folder (`QuickTab.dll`, `0Harmony.dll`, `Info.json`) to `Documents\Railroader\Mods\QuickTab\`. It's a DLL mod, so turn on **Preferences > Features > Experimental > Allow Unsafe DLL Mods** and restart, then enable it under **Mods** on the main menu.

## How it works

- Harmony patches on `CarPickable.HandleShowContextMenu` and `ContextMenu.Show` add the menu item. Destination uses the same hook, and the two mods sit side by side.
- A postfix on `CarInspector.Configure` sets the window's selected tab when Quick Tab opens it.
- A prefix on `CarPickable.Activate` does the same for Ctrl+click.

## Licence

MIT. Harmony (MIT) is included; see `LICENSE-Harmony.txt`.
