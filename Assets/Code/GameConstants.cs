namespace SFBuilder
{
    /// <summary>
    /// Keep all hardcoded values here that are settings or related to files outside of code
    /// </summary>
    public struct GameConstants
    {
        /// <summary>
        /// Wait period for animating post-level-load transition
        /// </summary>
        public const float LevelTransitionEndTime = 2.5f;

        /// <summary>
        /// The maximum value that can be set for the HoloShield Power value
        /// </summary>
        public const float LevelTransitionMaxPower = 4f;

        /// <summary>
        /// The minimum value that can be set for the HoloShield Power value
        /// </summary>
        public const float LevelTransitionMinPower = 0;

        /// <summary>
        /// Wait period for animating pre-level-load transition
        /// </summary>
        public const float LevelTransitionStartTime = 2.5f;

        /// <summary>
        /// How long UI button transitions last
        /// </summary>
        public const float UITransitionDuration = 1f;

        /// <summary>
        /// The delay factor to apply to elements (multiplied by how many are in the UITransitionSystem's current list)
        /// </summary>
        public const float UITransitionDelay = 0.05f;
    }
}