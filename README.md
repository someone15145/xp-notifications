# XP Notifications

A simple Valheim mod that shows skill XP gains in the top-left corner.

## Features

- Shows notifications for every skill XP gain
- Customizable notification text
- Displays:
  - Current progress percent
  - XP gained from the action
  - Current XP
  - XP required for the next level
- Optional toggle for running skill XP
- Adjustable text size
- Full support for :contentReference[oaicite:0]{index=0}

## Default Notification Format

```text
{0}%
````

Where:

* `{0}` = current percentage
* `{1}` = XP gained from the action
* `{2}` = current XP
* `{3}` = XP required for the next level

Example:

```text
Axes: 42.3% (1.27/3.56)
```

## Configuration

| Setting                  | Description                        | Default               |
| ------------------------ | ---------------------------------- | --------------------- |
| `ShowXPNotifications`    | Enable or disable XP notifications | `true`                |
| `ShowRunXP`              | Show XP notifications for running  | `false`               |
| `NotificationFormat`     | Custom notification format string  | `{0}% 			    |
| `NotificationTextSizeXP` | Notification text size             | `14`                  |

## Requirements

* BepInEx

## Installation

1. Install BepInEx
2. Place `XPNotifications.dll` into `BepInEx/plugins`
3. Launch the game

## Credits

Created by someone15145