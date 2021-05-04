Automatic tiling window manager for Windows 10 along the lines of [xmonad](https://xmonad.org/) and [Amethyst](https://ianyh.com/amethyst/).

[![Build Status](https://dev.azure.com/giuseppesorrentino/AmethystWindows/_apis/build/status/glsorre.amethystwindows?branchName=master)](https://dev.azure.com/giuseppesorrentino/AmethystWindows/_build/latest?definitionId=1&branchName=master)
[![Gitter](https://badges.gitter.im/glsorre/amethystwindows.svg)](https://gitter.im/glsorre/amethystwindows?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![GitHub all releases](https://img.shields.io/github/downloads/glsorre/amethystwindows/total)](https://github.com/glsorre/amethystwindows/releases)

## Getting Amethyst Windows

Amethyst Windows is available for direct download as appxbundle on the [release page](https://github.com/glsorre/amethystwindows/releases).

## Key Features

- doesn't use DLL injection to manipulate windows
- integrated with windows 10 virtual desktops
- fast and light

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

Cutomize shortcuts is possible by copying and correctly editing [`defaultHotkeys.json`](https://github.com/glsorre/amethystwindows/blob/master/defaultHotkeys.json) into `%USERPROFILE%/AppData/Roaming/AmethystWindows/hotkeys.json`.

The key field reference can be found at [https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes](https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes).

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

## Contact

Please contact me trough [twitter](https://twitter.com/glsorre) or [gitter](https://gitter.im/glsorre/amethystwindows)

## License

This software is released with the MIT license.

## Contributing

Feel free to fork master and open a PR.

## Credits

I got ideas and code snippets from the following projects:
- [MScholtes/VirtualDesktop](https://github.com/MScholtes/VirtualDesktop);
- [Grabacr07/VirtualDesktop ](https://github.com/Grabacr07/VirtualDesktop);
- [losttech/VirtualDesktop ](https://github.com/losttech/VirtualDesktop).
