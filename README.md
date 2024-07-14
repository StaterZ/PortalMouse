# Portal Mouse
**PortalMouse** is a lightweight program designed to once and for all fix your awful Windows multi-monitor experience by allowing you to set up portals along the edges of your monitors.

Say goodbye to the frustration of dealing with inconsistent mouse movement between screens with different pixel densities but the same physical size!
With PortalMouse, you can seamlessly navigate across your monitors without encountering any awkward jumps.

## Main Features
- **Seamless Navigation:** PortalMouse uses Windows LowLevelMouseHook API to move the cursor seamlessly between any two portals.
- **Partial Edge Support:** Set a portal to only cover half the edge, or why not exactly 100 pixels? Or have multiple portals on the same edge.
- **Pixel Perfect:** Set the exact pixel/percentage each portal should start and stop at. PortalMouse also utilizes fractional pixels internally to move the cursor pixel perfectly!
- **Display Scale Support:** Full support for Windows display scale

## Getting Started
To install PortalMouse, simply follow these steps:

- Download [The Latest Release](https://github.com/StaterZ/PortalMouse/releases) here on github.
- Unzip the downloaded file and move the directory somewhere you'd like to have it.
- Configure the `config.json` inside as needed and refer to the included `Documentation.md` for how to configure it.
  - By default, the config contains a simple example wrapping the cursor around screen1.
  - You can test your configuration by launching the exe and using the "Reload" action on the programs system tray icon to quickly iterate on the config.
- Unless already doing so from setting up the configuration, launch the exe.
- To have the program auto-launch on startup, you can refer to [this guide by Microsoft](https://support.microsoft.com/en-us/windows/add-an-app-to-run-automatically-at-startup-in-windows-10-150da165-dcd9-7230-517b-cf3c295d89dd).

## Build PortalMouse
To build PortalMouse, simply clone this repository and open it in VisualStudio.

To create a final executable, simply follow these steps:
- Right-click the `PortalMouse` assembly in the *Solution Explorer* panel.
- Select *Publish* from the dropdown.
- Press the *Publish* button in the opened panel.
- Wait for VisualStudio to complete the publishing process.
- Press the *Open folder* link in the green *Publish succeeded* box.
- Rename and move the folder somewhere you'd like to have it.
