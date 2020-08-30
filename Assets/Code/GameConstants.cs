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
        public const double __GAME_VERSION = 0.19;

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
        /// Converts a keyboard key from InputSystem to the symbol it represents
        /// </summary>
        /// <param name="name">Identifier used in InputSystem</param>
        /// <returns>A symbol or shorthand identifier for the key provided</returns>
        public static string InputNameToChar(string name)
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
        /// Adds color tags (HTML, treated as a GameConstant) to a number depending on if it's positive or negative
        /// </summary>
        /// <param name="num">The number being converted</param>
        /// <returns>Number as string surrounded by tags usable by TMPro</returns>
        public static string    UITooltipColorTag(double num)
        {
            if (num > 0)
                return "<color=#00ffff>+" + num.ToString() + "</color>";
            else
                return "<color=#ff0000>\u2011" + System.Math.Abs(num).ToString() + "</color>";
        }
    }
}