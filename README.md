# Moon Settler demo repository

Copyright (C) 2020 Goldenwere

## About

### Description

This is the repository for the Moon Settler demo. Moon Settler (codenamed SFBuilder) is a sci-fi puzzle / city-building hybrid. The player is given 'goals' to complete, which contain a set of required and optional buildings as well as a minimum viability requirement. Viability is the sum of three settlement stats that must be kept above 0 in order to move on: happiness, power, and sustenance. Buildings placed by the player may have a base score that adds or subtracts to those stats. Buildings can also affect others in range in the same way, making building placement strategy and planning ahead important.

### Inspirations

Moon Settler is largely inspired by the game [ISLANDERS](https://en.wikipedia.org/wiki/Islanders_(video_game)), another puzzle city-building hybrid. ISLANDERS has a singular scoring system dependant on building placement and buildings in range. Moon Settler's stats are inspired by more 'traditional' (non-puzzle-like) builder games such as [MyColony](https://www.apewebapps.com/my-colony/) and [Cities: Skylines](https://en.wikipedia.org/wiki/Cities:_Skylines). The way bonuses/penalties encourage placing certain buildings in close proximity while others far away is partly inspired by [Aven Colony](http://avencolony.com/).

### Repo

Moon Settler's demo is FOSS under [GPLv3](GPLv3.md) (uses third party assets and some other assets whose licenses differ, see [LICENSE.md](LICENSE.md) for clarifications). The demo is intended to be a fully functional proof-of-concept with most of the game's core features implemented. Features such as different modes (sandbox, infinite), new building category(ies), level-pack support, etc. are potential plans which won't be featured in the demo. This also goes for expanding the available levels and adding new ones. Any minor UX changes (improved camera, tooltip improvements, control bindings, etc.) will ideally be made to the demo as well, as the two former plans are external to this project, and the latter should be quite independent from a majority of the game's code besides MenuUI. The full game will be in a separate private repository, with source code being made available soon after the game's release.

## Repo

The repository is a Unity project. See [ProjectVersion](ProjectSettings/ProjectVersion.txt) for which version of Unity the project folder must be opened in. The project's Assets folder is organized into the following manner:
| Folder | Description |
| :--- | :--- |
| [Animations](Assets/Animations) | Animations for models, organized into Group_# |
| [Code](Assets/Code) | Game code, organized by namespace/assembly (with the parent namespace SFBuilder being directly under /Code) |
| [Materials](Assets/Materials) | Materials in use by the project, organized by type |
| [Models](Assets/Models) | Game models, organized by type, with buildings organized into Group_# and terrain named by the scene it is found in |
| [Prefabs](Assets/Prefabs) | Prefabbed objects, with uncategorized prefabs directly under /Prefabs and buildings organized into Group_# |
| [Scenes](Assets/Scenes) | Game scenes; Base always loads first and only unloads on Application.Quit(), others are loaded additively, with their associated data in folders named after them |
| [Settings](Assets/Settings) | Any game settings, such as input, audio mixing, and post processing |
| [Sounds](Assets/Sounds) | Contains game audio, organized into /Music and /Effects (UI/etc) |
| [Textures](Assets/Textures) | Game textures, organized by type |
| [ThirdParty](Assets/ThirdParty) | ThirdParty assets, organized by creator then project; licenses available in each folder |

### Code

Code is organized into the SFBuilder namespace.
| Namespace | Description |
| :--- | :--- |
| SFBuilder | Core game code such as game data, events, constants, and miscellaneous code |
| SFBuilder.Editor | Editor-related code |
| SFBuilder.Gameplay | Code related to game-play systems, such as scoring, goals, and and leveling |
| SFBuilder.Obj | Code related to objects in the game, such as the placement system and BuilderObject code |
| SFBuilder.UI | Anything related to user interfaces, such as menus, HUD elements, etc. |
| SFBuilder.Util | Miscellaneous utility classes/etc. |

Code inside the Goldenwere folder and related namespaces come from the [Goldenwere Standard Unity repo](https://github.com/Goldenwere/GW-Standard-Unity). These are general-purposed and, while mostly originally made for Moon Settler, are unrelated to Moon Settler's code and are/will be made available in the standard repo.

#### SFBuilder

| File | Description |
| :--- | :---------- |
| AccessibilityDefines | Contains accessibility-related enums |
| GameAudioSystem | Handles sound effect requests from various other classes as well as playing music |
| GameConstants | Singular place to define constant hard-coded values; Some of these are methods which associate a value with another |
| GameEventSystem | Used by other namespaces to communicate without creating inter-tangled dependencies; can be as simple as class-to-class or as large as class-reading-gamestate |
| GameSave | Player save data related objects and manager |
| GameSettings | Player settings data (display, audio, controls, accessibility) related objects and manager |
| InputDefines | Contains input-related enums
| ObjectType | Assigns specific int-based ID (to prevent breaking-changes when using serialized fields) to placed BuilderObjects; used across multiple namespaces, thus why in SFBuilder |
| UnityEventSystemExtension | Extends the Unity EventSystem with `SelectedGameObjectChanged` event |
| WindowResolution | Contains definitions of window resolutions |

#### SFBuilder.Editor

Contains editor scripts (currently just ControlButtonEditor, to ensure Button-extended variables show up in the editor).

#### SFBuilder.Gameplay

| File | Description |
| :--- | :---------- |
| GameScoring | Singleton which handles the scoring related to object placement; bridges the gap between UI and object placement, actual score data is set in GameSave |
| GoalContainer | Struct used to represent a goal (containing required/optional GoalItems and minimum viability) |
| GoalItem | Struct which assigns a count to an ObjectType |
| GoalSystem | Singleton which handles goals for the player, including transitioning, verifying, and loading from the saved state of the level |
| LevelingSystem | Singleton which manages level transitions based on what level the player is on |

#### SFBuilder.Obj

| File | Description |
| :--- | :---------- |
| BuilderObject | Contains placement rules in static methods as well as the code which maintains the placed structures that are core to the game |
| BuilderObjectGrounder | Ensures that a BuilderObject is grounded before the player can place it |
| BuilderObjectRanger | Tracks other BuilderObjects using trigger-based handlers for use in scoring |
| BuilderObjectTypeToPrefab | Structure for placement system to associate types to prefabs in inspector (as prefabs are in sets which differ from level to level) |
| PlacementSystem | Singleton which allows the player to place BuilderObjects and update related Gameplay systems as needed; uses Linq to traverse BuilderObjectTypeToPrefab |

#### SFBuilder.UI

| File | Description |
| :--- | :---------- |
| BuilderButton | Upon loading a goal, prefabbed BuilderButtons are created which handle spawning buildings when the player clicks on them and indicating the number of buildings the player can place |
| ColorEnabledElement | Holds references for applying colors based on a level's theme (has method called in GameUI to apply the colors) |
| ControlButton | Extends the UnityEngine.UI.Button to allow for associating controls-related information to buttons |
| GameUI | Handles the in-game UI |
| KeyboardNavigableDropdown | Extends TMP_Dropdown to re-implement UI-navigation (present in regular TMP_Dropdown, but making the Unity UI more keyboard/gamepad friendly broke this functionality); Note that some steps must be taken in order to fully get this to work, see Before Running/Building |
| MenuUI | Handles all of the main menu's selectables |
| RadialStatIndicator | Associates a radial UI element with a stat to display (combines "potential" and "total," therefore cannot be used with all stats) |
| ResizeableUI | A more controlled version of ScaleableUI (useful if a UI element already has StyleableText attached) |
| ScaleableUI | Applies UI scaling settings to UI elements |
| StyleableText | Applies font style settings to text elements |
| TextStatIndicator | Associates UI elements with a stat to display. Like RadialStatIndicator, it combines "potential" and "total," but can be used with other stats as well (i.e. CurrentGoal/CurrentGoalMinimumViability) |
| TextType | Enum which defines what type of material preset to use for text |
| TransitionedUIElement | Allows for UI elements to be transitioned by the UITransitionSystem |
| UIAssets | Singleton which holds references to numerous UI-related assets |
| UITransitionSystem | Transitions TransitionedUIElements in order rather than instantly enabling all at once |

#### SFBuilder.Util

| File | Description |
| :--- | :---------- |
| ControllerSettings | Ensures camera controller settings are applied (attached to controller) |
| GameCursor | Renders a custom cursor in place of the hardware cursor which can be resized from the settings menu; Also handles locking the cursor based on camera state |
| GraphicsHandler | Ensures graphic settings are applied (attached to a global scene object) |
| PostProcessingHandler | Ensures post-processing settings are applied (attached to post-processing volumes) |

### Before Running/Building

Some packages need editing before building/testing the game due to decisions which interfere with the project.

#### TMPro

In `TMP_Dropdown.DropdownItem`, completely remove the following code:

```
public virtual void OnPointerEnter(PointerEventData eventData)
{
    EventSystem.current.SetSelectedGameObject(gameObject);
}
```

This interferes with UI navigation. Be sure to also remove the IPointerEnterHandler from the class declaration.