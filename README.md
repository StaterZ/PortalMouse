# Portal Mouse
**PortalMouse** is a lightweight program designed to enhance your multi-monitor experience by allowing you to set up portals along the edges of your monitors.

Say goodbye to the frustration of dealing with inconsistent mouse movement between screens with different pixel densities but the same physical size.
With PortalMouse, you can seamlessly navigate across your monitors without encountering any awkward jumps.

## Features
- **Seamless Navigation:** Set up portals along the edges of your monitors to navigate between any two edges seamlessly.
- **Partial Edge Support:** Customize your portal setup by defining partial edges, allowing for unique mapping configurations such as dividing a large screens edge into different sections.

## Getting Started
To install PortalMouse, simply follow these steps:

- Download [The Latest Release](https://github.com/StaterZ/PortalMouse/releases) here on github.
- Unzip the downloaded file and move the directory somewhere you'd like to have it.
- Configure the `config.json` inside as needed and refer to the included `Documentation.md` for how to configure it.
  - By default, the config contains a simple example wrapping the cursor around screen1.
  - You can test your configuration by launching the exe and using the "Reload" action on the programs system tray icon to quickly iterate on the config.
- To have the program auto-launch on startup, you can refer to this guide by Microsoft: [LINK](https://support.microsoft.com/en-us/windows/add-an-app-to-run-automatically-at-startup-in-windows-10-150da165-dcd9-7230-517b-cf3c295d89dd).
- Unless already doing so from setting up the configuration, launch the exe.

## Build PortalMouse
To build PortalMouse, simply follow these steps:

- Clone this repository to your local machine.
- Open it in VisualStudio by double-clicking `PortalMouse.sln`.
- Right-click the `PortalMouse` assembly in `Solution Explorer`.
- Select `Publish` from the dropdown.
- Press the `Publish` button in the opened panel.
- Wait for VisualStudio to complete the publishing process.
- Press the `Open folder` link in the green `Publish succeeded` box.
- Rename and move the folder somewhere you'd like to have it.
- Refer to the configure step and forwards of the **Getting Started** section to see how to use the program.
