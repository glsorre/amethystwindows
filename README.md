# Amethyst Windows

[![Build Status](https://dev.azure.com/giuseppesorrentino/AmethystWindows/_apis/build/status/glsorre.amethystwindows?branchName=master)](https://dev.azure.com/giuseppesorrentino/AmethystWindows/_build/latest?definitionId=1&branchName=master)
[![Gitter](https://badges.gitter.im/glsorre/amethystwindows.svg)](https://gitter.im/glsorre/amethystwindows?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![GitHub labels](https://img.shields.io/github/labels/glsorre/amethystwindows/help%20wanted)](https://github.com/glsorre/amethystwindows/labels/help%20wanted)

The **dynamic / automatic tiling window manager** for **windows 10** along the lines of [ianyh/Amethyst](https://github.com/ianyh/Amethyst).

A very quick screencast of basic functions is [available](https://www.youtube.com/embed/AWN_KehMzHc).

## Features

- integrated with **windows 10 virtual desktops**
- fully **customizable shortcuts**
- supports **multi-monitor** set-ups
- **automatic updates**
- **open source**
- available for **x86, x86-64 and ARM** architectures

## Install Amethyst Windows

1. Download the appinstaller file from release page
2. Install it trough [MS App Installer](https://www.microsoft.com/en-us/p/app-installer/9nblggh4nns1?activetab=pivot:overviewtab)
3. Do not forget to star the project :)

## Screenshots

![Main Page](https://cdn.jsdelivr.net/gh/glsorre/amethystwindows@site/_assets/main.png)

![Settings Page](https://cdn.jsdelivr.net/gh/glsorre/amethystwindows@site/_assets/settings.png)


# Documentation

## Keyboard Shortcuts

Amethyst Windows uses two modifiers.

| Shortcut                  | Description                          |
|---------------------------|--------------------------------------|
| `alt + shift`             | mod1                                 |
| `alt + shift + win`       | mod2                                 |


The keyboard shortcuts configured are:

| Shortcut                  | Description                                   |
|---------------------------|-----------------------------------------------|
| `mod1 + space`            | Rotate layouts clockwise                      |
| `mod2 + space`            | Rotate layouts counterclockwise               |
| `mod1 + enter`            | Swap focused window to main window            |
| `mod1 + H`                | Swap focused window counterclockwise          |
| `mod1 + L`                | Swap focused window clockwise                 |
| `mod1 + J`                | Move focus to previous window                 |
| `mod1 + K`                | Move focus to next window                     |
| `mod1 + P`                | Move focus to previous monitor                |
| `mod1 + N`                | Move focus to next monitor                    |
| `mod2 + L`                | Expand main pane                              |
| `mod2 + H`                | Shrink main pane                              |
| `mod2 + K`                | Move window to next monitor                   |
| `mod2 + J`                | Move window to previous monitor               |
| `mod1 + Z`                | Force windows to be revalutated               |
| `mod2 + left`             | Throw focused window to virtualdesktop left   |
| `mod2 + right`            | Throw focused window to virtualdesktop right  |
| `mod2 + 1`                | Throw focused window to virtualdesktop 1      |
| `mod2 + 2`                | Throw focused window to virtualdesktop 2      |
| `mod2 + 3`                | Throw focused window to virtualdesktop 3      |
| `mod2 + 4`                | Throw focused window to virtualdesktop 4      |
| `mod2 + 5`                | Throw focused window to virtualdesktop 5      |

### Customize Shortcuts

1. Copy [`defaultHotkeys.json`](https://github.com/glsorre/amethystwindows/blob/master/defaultHotkeys.json) into `%USERPROFILE%/AppData/Roaming/AmethystWindows/hotkeys.json`.
2. Edit the copied file:
    - [key field reference](https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes)
    - [modifiers reference](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey#parameters)
3. Relaunch the application:
    - click on exit from the systray
    - relaunch.

## Layouts

### Horizontal
This layout has one column per window, with each window extending the full height of the screen.

### Vertical
The rotated version of Horizontal, where each window takes up an entire row, extending the full width of the screen.

### HorizontalGrid
This layout places the windows in grid occuping space in horizontal when necessary.

### VerticalGrid
This layout places the windows in grid occuping space in vertical when necessary.

### Monocle
In this layout, the currently focused window takes up the entire screen, and the other windows are not visible at all.

### Wide
The rotated version of tall.

### Tall
The default layout. This gives you one "main pane" on the left, and one other pane on the right. The main window is placed in the main pane (extending the full height of the screen), and all remaining windows are placed in the other pane. The main pane can be shrinked/expanded.

# About

## Contact

Please contact me trough [twitter](https://twitter.com/glsorre) or [gitter](https://gitter.im/glsorre/amethystwindows)

## License

This software is released with the [MIT license](https://github.com/glsorre/amethystwindows/blob/master/LICENSE).

## Contributing

Feel free to fork master and open a PR.

I will add issues where I think you could start from and label them as help wanted.

## Credits

A big credit goes to [ianyh/Amethyst](https://ianyh.com/amethyst/). This is simply its port on the windows 10 operating system.

I got ideas and code snippets from the following projects:
- [MScholtes/VirtualDesktop](https://github.com/MScholtes/VirtualDesktop);
- [Grabacr07/VirtualDesktop ](https://github.com/Grabacr07/VirtualDesktop);
- [losttech/VirtualDesktop ](https://github.com/losttech/VirtualDesktop)

