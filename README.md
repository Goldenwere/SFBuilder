# Moon Settler demo repository

Copyright (C) 2020 Goldenwere

## About

### Description

This is the repository for the Moon Settler demo. Moon Settler (codenamed SFBuilder) is a sci-fi puzzle / city-building hybrid. The player is given 'goals' to complete, which contain a set of required and optional buildings as well as a minimum viability requirement. Viability is the sum of three settlement stats that must be kept above 0 in order to move on: happiness, power, and sustenance. Buildings placed by the player may have a base score that adds or subtracts to those stats. Buildings can also affect others in range in the same way, making building placement strategy and planning ahead important.

### Inspirations

Moon Settler is largely inspired by the game ISLANDERS, another puzzle city-building hybrid. ISLANDERS has a singular scoring system dependant on building placement and buildings in range. Moon Settler's stats are inspired by more 'traditional' (non-puzzle-like) builder games such as MyColony and Cities: Skylines. The way bonuses/penalties encourage placing certain buildings in close proximity while others far away is partly inspired by Aven Colony.

### Repo

Moon Settler's demo is FOSS under [GPLv3](LICENSE.md) (uses third party assets whose licenses differ). The demo is intended to be a fully functional proof-of-concept with most of the game's core features implemented. Features such as different modes (sandbox, infinite), new building category(ies), level-pack support, etc. are potential plans which won't be featured in the demo. This also goes for expanding the available levels and adding new ones. Any minor UX changes (improved camera, tooltip improvements, control bindings, etc.) will ideally be made to the demo as well, as the two former plans are external to this project, and the latter should be quite independent from a majority of the game's code besides MenuUI. The full game will be in a separate private repository, with source code being made available soon after the game's release.

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
| SFBuilder.Gameplay | Code related to game-play systems, such as scoring, goals, and and leveling |
| SFBuilder.Obj | Code related to objects in the game, such as the placement system and BuilderObject code |
| SFBuilder.UI | Anything related to user interfaces, such as menus, HUD elements, etc |

Code inside the Goldenwere folder and related namespaces come from the [Goldenwere Standard Unity repo](https://github.com/Goldenwere/GW-Standard-Unity). These are general-purposed and, while mostly originally made for Moon Settler, are unrelated to Moon Settler's code and are/will be made available in the standard repo.

#### SFBuilder

Because ObjectType is needed for multiple namespaces, it is contained in the SFBuilder namespace. This enum assigns a specific integer ID (to prevent them from changing when using serialized fields in Unity) to each type. Also in this namespace is the GameEventSystem, which namespaces use to communicate without creating inter-tangled depenedencies. While some events are mainly used to pass data from one class to another, some are used by many, such as for handling changes in the game's GameState. GameConstants contains any SFBuilder-related constants that could change multiple times over the course of development. Keeping them in one place makes them easier to find in order to change without figuring out which classes they are used in. The GameAudioSystem handles sound effect requests from various other classes as well as playing music. GameSettings saves/loads settings and allows for external manipulation of these settings. Stored in SettingsData are graphical and sound related settings. Most of these classes are singletons, as they handle passing data around.

#### SFBuilder.Gameplay

The LevelingSystem is a simple singleton which manages level transitions based on what level the player is on. The GoalSystem, another singleton, handles goals for the player, including transitioning, verifying, and loading from the saved state of the level. GoalContainer is a struct used to represent a goal (containing required/optional GoalItems and minimum viability), while GoalItem is another struct which assigns a count to an ObjectType. GameScoring, another singleton, handles the scoring related to object placement. Nothing really interesting happens here besides calling UI events related to score updates and the score updates (and saving said updates) themselves.

#### SFBuilder.Obj

Building placement rules can be found in the static methods of BuilderObject. Scores are found via the BuilderObjectRanger's collision trigger handlers. Valid placement is assisted by the BuilderObjectGrounder and collision code contained in BuilderObject. BuilderObjectTypeToPrefab is a small utility structure for assigning prefabs to types per level scene in Unity's inspector. The placement system uses Linq to search through a serialize array of said type when instantiating BuilderObject-based GameObjects. Only PlacementSystem is a singleton, other classes are treated as traditional objects in OOP.

#### SFBuilder.UI

There are a few often-used classes in this namespace. BuilderButton is for a prefab created upon loading a goal, which instantiates (spawns) buildings when the player clicks on them. ColorEnabledElement is a simple way of allowing UI elements to have colors applied via GameUI on GameUI's Awake() (which is called for each game level's Awake that isn't the base level). TransitionedUIElement works in a similar manner, where the UITransitionSystem handles enabling elements in a specific order so that UI elements animate as expected (for example, the main menu animates the title and buttons from the top down; BuilderButtons animate from left to right). StatIndicator is used for text elements which indicate stats. Only the Totals and CurrentGoal/CurrentGoalMinimumViability from ScoreType are used when assigning StatIndicator's types in inspector, but handle updates to potential as well as total. GameUI and MenuUI are singletons that handle their respective UI's. MenuUI exposes handlers for settings menu elements' OnValueChanged events and the menu buttons' OnPressed events. GameUI handles setting up the placement panel and various OnPressed events. Both handle their respective transitions/animations as well. The PostProcessingHandler does just that - handles post processing by loading post processing settings when they are updated and on Start().