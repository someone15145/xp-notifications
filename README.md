# XP Notifications

Valheim mod that tracks skill XP gains in real-time.

## Features
- **Instant Feedback:** Notifications for every skill XP gain.
- **Highly Customizable:** Change text size, position, and format.
- **Smart Throttling:** Optional mode for the Running skill to avoid spam.
- **Placeholders:** Display percent, gained XP, total XP, and current level.

## Notification Format
Custom string format using the following placeholders:
- `{0}`: Progress % | `{1}`: Gained XP | `{2}`: Current XP | `{3}`: Next Level XP | `{4}`: Skill Level

**Example:** `{0}% ({2}/{3}) [+{1}] Lv.{4}`  
**Result:** `Axes: 42.3% (1.27/3.56) [+0.5] Lv.11`

## Configuration

| Setting | Description | Default |
| :--- | :--- | :--- |
| `ShowXPNotifications` | Toggle all notifications | `true` |
| `NotificationFormat` | Text template (see above) | `{0}% ({2}/{3}) [+{1}] Lv.{4}` |
| `NotificationPosition` | `TopLeft`, `Center` | `TopLeft` |
| `NotificationTextSizeXP`| Font size | `14` |
| `RunXPBehavior` | `Disabled`, `Normal`, `Throttling` | `Throttling` |
| `RunXPTimeout` | Seconds between logs in Throttling mode | `2` |

## Installation
1. Install [BepInEx Pack for Valheim](https://thunderstore.io).
2. Place `XPNotifications.dll` into your `BepInEx/plugins` folder.
3. Launch the game to generate the config file.

## Changelog
- **1.0.2** (Current):
  - Added **Throttling** mode for Running skill to reduce spam.
  - Added `{4}` placeholder for current skill level.
- **1.0.1**: 
  - Fixed XP formula. 
  - Added `NotificationPosition` options.
- **1.0.0**: Initial release.

---
*Created by someone15145*
