using System.Linq;
using System.Collections.Generic;

namespace SFBuilder
{
    /// <summary>
    /// Keep all hardcoded values here that are settings or related to files outside of code
    /// </summary>
    public struct GameConstants
    {
        /// <summary>
        /// The current game version
        /// </summary>
        public const double __GAME_VERSION = 0.20;

        /// <summary>
        /// Name of effects group volume in GameAudioMixer
        /// </summary>
        public const string AudioParamEffects = "VolEffects";

        /// <summary>
        /// Name of music group volume in GameAudioMixer
        /// </summary>
        public const string AudioParamMusic = "VolMusic";

        /// <summary>
        /// Path to save file
        /// </summary>
        public const string DataPathSave = "/save.json";

        /// <summary>
        /// Path to settings file
        /// </summary>
        public const string DataPathSettings = "/settings.xml";

        /// <summary>
        /// Multiplier used on goalset length for switching from easy goalsets to hard goalsets
        /// </summary>
        public const float  InfiniPlayFromEasyToHard = 1.75f;

        /// <summary>
        /// How much viability increases for each new easy goal preset
        /// </summary>
        public const int    InfiniPlayEasyViabilityIncrease = 50;

        /// <summary>
        /// How much viability increases for each new hard goal preset
        /// </summary>
        public const int    InfiniPlayHardViabilityIncrease = 75;

        /// <summary>
        /// Wait period for animating post-level-load transition
        /// </summary>
        public const float  LevelTransitionEndTime = 2.5f;

        /// <summary>
        /// The maximum value that can be set for the HoloShield Power value
        /// </summary>
        public const float  LevelTransitionMaxPower = 4f;

        /// <summary>
        /// The minimum value that can be set for the HoloShield Power value
        /// </summary>
        public const float  LevelTransitionMinPower = 0;

        /// <summary>
        /// Wait period for animating pre-level-load transition
        /// </summary>
        public const float  LevelTransitionStartTime = 2.5f;

        /// <summary>
        /// Length of time to animate fading music sources
        /// </summary>
        public const float  MusicFadeTime = 2.0f;

        /// <summary>
        /// Crossfading tracks involves going between 0 and this number / visa-versa; this is set less than 1 so that music normally is quieter than in-game sounds
        /// </summary>
        public const float  MusicSourceMaxVolume = 0.5f;

        /// <summary>
        /// Wait period between playing tracks (should be at least a minute longer than the longest music track in the game)
        /// </summary>
        public const float  MusicWaitTime = 360f;

        /// <summary>
        /// Sets how long the bouncy animation for placement lasts
        /// </summary>
        public const float  PlacementAnimationDuration = 0.5f;

        /// <summary>
        /// Sets the duration for the initial load transitions
        /// </summary>
        public const float  UIFirstLoadDuration = 2.0f;

        /// <summary>
        /// How long UI button transitions last
        /// </summary>
        public const float  UITransitionDuration = 0.66f;

        /// <summary>
        /// The delay factor to apply to elements (multiplied by how many are in the UITransitionSystem's current list)
        /// </summary>
        public const float  UITransitionDelay = 0.25f;

        /// <summary>
        /// How long UI button transitions last for radial elements
        /// </summary>
        public const float  UITransitionRadialDuration = 0.10f;

        /// <summary>
        /// Translates the global UIScale to a fontSize based on a StyleableText's FontFormat
        /// </summary>
        /// <param name="setting">The UIScale setting</param>
        /// <param name="format">The FontFormat of the text element</param>
        /// <returns>Float for use in TMP_Text.fontSize</returns>
        public static float         UIScaleToFloat(UIScale setting, FontFormat format)
        {
            switch (setting)
            {
                case UIScale.Large:
                {
                    switch (format)
                    {
                        case FontFormat.Title:              return 15.0f;
                        case FontFormat.Heading:            return 12.5f;
                        case FontFormat.Subheading:         return 7.50f;
                        case FontFormat.Label:              return 6.50f;
                        case FontFormat.InnerLabel:         return 4.38f;
                        case FontFormat.Tooltip:            return 3.13f;
                        case FontFormat.GameWindow:         return 6.88f;
                        case FontFormat.GameWindowSubText:  return 3.75f;
                        case FontFormat.HUDIndicators:      return 4.00f;
                        case FontFormat.MainButtons:
                        default:                            return 10.0f;
                    }
                }
                case UIScale.Small:
                {
                    switch (format)
                    {
                        case FontFormat.Title:              return 9.00f;
                        case FontFormat.Heading:            return 7.50f;
                        case FontFormat.Subheading:         return 4.50f;
                        case FontFormat.Label:              return 3.75f;
                        case FontFormat.InnerLabel:         return 2.63f;
                        case FontFormat.Tooltip:            return 1.88f;
                        case FontFormat.GameWindow:         return 4.13f;
                        case FontFormat.GameWindowSubText:  return 2.25f;
                        case FontFormat.HUDIndicators:      return 3.00f;
                        case FontFormat.MainButtons:
                        default:                            return 6.00f;
                    }
                }
                case UIScale.Medium:
                default:
                {
                    switch (format)
                    {
                        case FontFormat.Title:              return 12.0f;
                        case FontFormat.Heading:            return 10.0f;
                        case FontFormat.Subheading:         return 6.00f;
                        case FontFormat.Label:              return 5.00f;
                        case FontFormat.InnerLabel:         return 3.50f;
                        case FontFormat.Tooltip:            return 2.50f;
                        case FontFormat.GameWindow:         return 5.50f;
                        case FontFormat.GameWindowSubText:  return 3.00f;
                        case FontFormat.HUDIndicators:      return 3.50f;
                        case FontFormat.MainButtons:
                        default:                            return 8.00f;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a keyboard key from InputSystem to the symbol it represents
        /// </summary>
        /// <param name="name">Identifier used in InputSystem</param>
        /// <returns>A symbol or shorthand identifier for the key provided</returns>
        public static string        InputNameToChar(string name)
        {
            switch (name)
            {
                case "backquote":       return "`";
                case "leftBracket":     return "[";
                case "rightBracket":    return "]";
                case "minus":           return "-";
                case "plus":            return "+";
                case "equals":          return "=";
                case "semicolon":       return ";";
                case "quote":           return "'";
                case "backslash":       return "\\";
                case "comma":           return ",";
                case "period":          return ".";
                case "slash":           return "/";
                case "leftCtrl":        return "L CTRL";
                case "rightCtrl":       return "R CTRL";
                case "leftAlt":         return "L ALT";
                case "rightAlt":        return "R ALT";
                case "ctrl":            return "CTRL";
                case "alt":             return "ALT";
                case "shift":           return "SHIFT";
                case "insert":          return "INS";
                case "delete":          return "DEL";
                case "home":            return "HOME";
                case "end":             return "END";
                case "pageUp":          return "PgUp";
                case "pageDown":        return "PgDn";
                case "escape":          return "ESC";
                case "numpadMultiply":  return "NUM*";
                case "numpadDivide":    return "NUM/";
                case "numpadPlus":      return "NUM+";
                case "numpadMinus":     return "NUM-";
                case "numpadEquals":    return "NUM=";
                case "numpadPeriod":    return "NUM.";
                case "numpadEnter":     return "NUM\nEnter";
                case "numpad1":         return "NUM1";
                case "numpad2":         return "NUM2";
                case "numpad3":         return "NUM3";
                case "numpad4":         return "NUM4";
                case "numpad5":         return "NUM5";
                case "numpad6":         return "NUM6";
                case "numpad7":         return "NUM7";
                case "numpad8":         return "NUM8";
                case "numpad9":         return "NUM9";
                case "numpad0":         return "NUM0";
                case "contextMenu":     return "Context\nMenu";
                default:                return name;
            }
        }

        /// <summary>
        /// Converts the ResolutionSetting enum to an array of strings
        /// </summary>
        /// <param name="rangeStart">The range to start from in the enum</param>
        /// <param name="rangeCount">The count after start to end at in enum</param>
        /// <returns>The converted list containing the values specified by range</returns>
        public static List<string>  ResolutionEnumToString(int rangeStart = 0, int rangeCount = 255)
        {
            return System.Enum.GetValues(typeof(ResolutionSetting))
                .Cast<ResolutionSetting>()
                .Select(x => x.ToString())
                .ToList().GetRange(rangeStart, rangeCount);
        }

        /// <summary>
        /// Accounts for available resolutions in ResolutionSetting for use in ResolutionEnumToString
        /// </summary>
        /// <param name="selected">The selected aspect ratio</param>
        /// <param name="start">The place to start in the EnumToString list</param>
        /// <param name="count">The count to end after in the EnumToString list</param>
        public static void          ResolutionRatioToRange(int selected, out int start, out int count)
        {
            switch (selected)
            {
                case 6:  start = 55; count = 11; break;
                case 5:  start = 40; count = 15; break;
                case 4:  start = 33; count = 07; break;
                case 3:  start = 28; count = 05; break;
                case 2:  start = 23; count = 05; break;
                case 1:  start = 17; count = 06; break;
                case 0:
                default: start = 00; count = 17; break;
            }
        }

        /// <summary>
        /// Converts a resolution setting to an integer array
        /// </summary>
        /// <param name="selected">The selected setting to parse</param>
        /// <returns>An integer array of size 2, with 0=width, 1=height</returns>
        public static int[]         ResolutionToArray(ResolutionSetting selected)
        {
            string[] split = selected.ToString().Replace("_", "").Split('x');
            return new int[] { int.Parse(split[0]), int.Parse(split[1]) };
        }

        /// <summary>
        /// Adds color tags (HTML, treated as a GameConstant) to a number depending on if it's positive or negative
        /// </summary>
        /// <param name="num">The number being converted</param>
        /// <returns>Number as string surrounded by tags usable by TMPro</returns>
        public static string        UITooltipColorTag(double num)
        {
            if (num > 0)
                return "<color=#00ffff>+" + num.ToString() + "</color>";
            else
                return "<color=#ff0000>\u2011" + System.Math.Abs(num).ToString() + "</color>";
        }
    }
}