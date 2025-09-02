# Hardcore

Adds Hardcore mode to Schedule I. You get one life.

**Note:** This mod **requires** Bars' fork of S1API. You can get it [here](https://github.com/ifBars/S1API/releases/tag/v1.7.0).

![Creating a new game](https://raw.githubusercontent.com/k073l/s1-hardcore/master/assets/NewGame.png)
![Death Screen](https://raw.githubusercontent.com/k073l/s1-hardcore/master/assets/DeathScreen.png)

## Installation
1. Install MelonLoader
2. Extract the zip file
3. Place the dll file into the Mods directory for your branch
    - For none/beta use IL2CPP
    - For alternate/alternate beta use Mono
4. Install S1API (Bars' fork) if you haven't already (same instructions as above)
5. Launch the game 
6. Preferences file will appear once you quit the game

## Configuration
1. Open the config file in `UserData/MelonLoader.cfg`
2. Edit the config file
```ini
[Hardcore]
# Show a heart icon in the HUD when in Hardcore mode
ShowHeartHUD = true
# Size of the heart icon in the HUD
HeartHUDSize = 32
# Horizontal offset from the right side of the screen (in px)
HeartHUDXOffset = 5
# Vertical offset from the top of the screen (in px)
HeartHUDYOffset = 5
```
3. Save the config file
4. Start the game

## Notes
- When enabled, Hardcore mode will go into effect when you save the game
- You can only enable Hardcore mode at the start of a new game
- Hardcore mode cannot be disabled once enabled. You can tell if it's enabled by the heart icon in the HUD (if enabled in config)
  (this isn't entirely true, you can edit the JSON at `<SaveGameDirectory>/Modded/Saveables/HardcoreSave/hardcore_mode.json`, which is created on save)
- If you die, you can explore the world as a ghost (noclip mode). Upon rejoining the game, you will still be a ghost.
- Multiplayer is likely broken. But then, is it really Hardcore if you have help?
