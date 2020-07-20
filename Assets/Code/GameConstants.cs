﻿using UnityEngine;

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
                return "<color=#ff0000>" + num.ToString() + "</color>";
        }
    }
}