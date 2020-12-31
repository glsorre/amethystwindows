# Amethyst Windows

Tiling window manager for Windows 10 along the lines of [xmonad](https://xmonad.org/) and [Amethyst](https://ianyh.com/amethyst/).

[![Build status](https://build.appcenter.ms/v0.1/apps/8ec48c76-c96e-470c-88e2-b8e660f5dc44/branches/master/badge)](https://appcenter.ms)
[![Gitter](https://badges.gitter.im/glsorre/amethystwindows.svg)](https://gitter.im/glsorre/amethystwindows?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![GitHub release (by tag)](https://img.shields.io/github/downloads/glsorre/amethystwindows/latest/total?label=download)](https://github.com/glsorre/amethystwindows/releases)

## Status

It's far from being production ready but functional for my need.

**I am looking for testers and contributors.**

## Key Features

- doesn't use DLL injection to manipulate windows
- supports windows 10 virtual desktops
- fast and light

## Background

The starting idea was to have a copy of [Amethyst](https://ianyh.com/amethyst/) working on Windows 10. Then it mixed with the concept of a more minimalist apporach in terms of Keyboard shortcuts.

To obtain that I got ideas and code snippets from the following projects:
- [MScholtes/VirtualDesktop](https://github.com/MScholtes/VirtualDesktop);
- [Grabacr07/VirtualDesktop ](https://github.com/Grabacr07/VirtualDesktop);
- [losttech/VirtualDesktop ](https://github.com/losttech/VirtualDesktop).

## Keyboard Shortcut

Amethyst Windows uses one modifier: `shift + alt` known as `mod1`.

For now the keyboard shortcut configured are:

| Shortcut                  | Description                          |
|---------------------------|--------------------------------------|
| `mod1 + space`            | Rotate layouts                       |
| `mod1 + enter`            | Swap focused window to main window   |
| `mod1 + H`                | Swap focused window counterclockwise |
| `mod1 + L`                | Swap focused window clockwise        |
| `mod1 + J`                | Change focus counterclockwise        |
| `mod1 + K`                | Change focus clockwise               |

## Layouts

### Horizontal
This layout has one column per window, with each window extending the full height of the screen.

### Vertical
The rotated version of Column, where each window takes up an entire row, extending the full width of the screen.

### HorizontalGrid
This layout places the windows in grid occuping space in horizontal when necessary.

### VerticalGrid
This layout places the windows in grid occuping space in vertical when necessary.

### Monocle
In this layout, the currently focused window takes up the entire screen, and the other windows are not visible at all.

### Tall
The default layout. This gives you one "main pane" on the left, and one other pane on the right. The main window is placed in the main pane (extending the full height of the screen), and all remaining windows are placed in the other pane.

### Wide
The rotated version of tall.

## License

This software is released with the MIT license.

## Contributing

Feel free to fork and open a PR.
