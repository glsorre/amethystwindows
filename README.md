# Amethyst Windows v2

The **dynamic / automatic tiling window manager** for **windows 10/11** along the lines of [ianyh/Amethyst](https://github.com/ianyh/Amethyst).

A very quick screencast of basic functions is [available](https://www.youtube.com/embed/AWN_KehMzHc).

## Features

- integrated with **windows 10/11 virtual desktops**
- fully **customizable shortcuts**
- supports **multi-monitor** set-ups
- **automatic updates**
- **open source**
- available for **x86, x86-64, ARM** architectures

# Documentation

## Default Keyboard Shortcuts
Amethyst Windows uses two modifiers.

| Shortcut                  | Description                          |
|---------------------------|--------------------------------------|
| `alt + shift`             | mod1                                 |
| `alt + shift + win`       | mod2                                 |


The keyboard shortcuts configured are:

| Shortcut       | Description                                  |
| -------------- | -------------------------------------------- |
| `mod1 + space` | Rotate layouts clockwise                     |
| `mod2 + space` | Rotate layouts counterclockwise              |
| `mod1 + enter` | Swap focused window to main pane             |
| `mod2 + enter` | Swap focused window to secondary pane        |
| `mod1 + H`     | Swap focused window counterclockwise         |
| `mod1 + L`     | Swap focused window clockwise                |
| `mod1 + J`     | Move focus to previous window                |
| `mod1 + K`     | Move focus to next window                    |
| `mod1 + P`     | Move focus to previous monitor               |
| `mod1 + N`     | Move focus to next monitor                   |
| `mod2 + H`     | Shrink main pane                             |
| `mod2 + L`     | Expand main pane                             |
| `mod2 + J`     | Move window to previous monitor              |
| `mod2 + K`     | Move window to next monitor                  |
| `mod1 + Z`     | Force windows to be revalutated              |
| `mod2 + left`  | Throw focused window to virtualdesktop left  |
| `mod2 + right` | Throw focused window to virtualdesktop right |
| `mod1 + 1`     | Switch to virtualdesktop 1                   |
| `mod1 + 2`     | Switch to virtualdesktop 2                   |
| `mod1 + 3`     | Switchto virtualdesktop 3                    |
| `mod1 + 4`     | Switch to virtualdesktop 4                   |
| `mod1 + 5`     | Switch to virtualdesktop 5                   |
| `mod2 + 1`     | Throw focused window to virtualdesktop 1     |
| `mod2 + 2`     | Throw focused window to virtualdesktop 2     |
| `mod2 + 3`     | Throw focused window to virtualdesktop 3     |
| `mod2 + 4`     | Throw focused window to virtualdesktop 4     |
| `mod2 + 5`     | Throw focused window to virtualdesktop 5     |

## Layouts

### Horizontal

This layout has one column per window, with each window extending the full height of the screen.

### Vertical

The rotated version of Horizontal, where each window takes up an entire row, extending the full width of the screen.

### Monocle

In this layout, the currently focused window takes up the entire screen, and the other windows are not visible at all.

### Tall Left

The default layout. This gives you one "main pane" on the left, and one other pane on the right. The main window is placed in the main pane (extending the full height of the screen), and all remaining windows are placed in the other pane. The main pane can be shrinked/expanded.

### Tall Right

Same of Tall left but with main pane on the right.

### Wide Top

The rotated version of tall.

### Wide Bottom

Same of Wide Top but with main pane on the bottom.

### 3 Tall Left

Gives a layout with three panes: a main one central, a secondary one on the left and a third one where all other windows are distributed.

### 3 Tall Right

Same of 3 Tall RIght with secondary pane on the right.

# About

## Contact

Please contact me trough [twitter](https://twitter.com/glsorre) or [gitter](https://gitter.im/glsorre/amethystwindows)

## License

This software is released with the [MIT license](https://github.com/glsorre/amethystwindows/blob/master/LICENSE).
