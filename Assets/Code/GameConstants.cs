namespace SFBuilder
{
    /// <summary>
    /// Keep all hardcoded values here that are settings or related to files outside of code
    /// </summary>
    public struct GameConstants
    {
        /// <summary>
        /// Name of effects group volume in GameAudioMixer
        /// </summary>
        public const string AudioParamEffects = "VolEffects";

        /// <summary>
        /// Name of music group volume in GameAudioMixer
        /// </summary>
        public const string AudioParamMusic = "VolMusic";

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