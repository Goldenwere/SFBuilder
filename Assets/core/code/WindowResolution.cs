namespace SFBuilder
{
    /// <summary>
    /// Defines resolution settings, grouped by aspect ratio
    /// </summary>
    public enum ResolutionSetting : byte
    {
        // 000 - 031:   16x9
        // 032 - 063:   16x10
        // 064 - 095:   21x9
        // 096 - 127:   32x9
        // 128 - 159:   5x4
        // 160 - 191:   4x3
        // 192 - 223:   3x2
        #region 16x9
        _640x360 = 0,
        _854x480 = 1,
        _960x540 = 2,
        _1024x600 = 3,
        _1138x640 = 4,
        _1280x720 = 5,
        _1366x768 = 6,
        _1600x900 = 7,
        _1776x1000 = 8,
        _1920x1080 = 9,
        _2048x1152 = 10,
        _2560x1440 = 11,
        _3200x1800 = 12,
        _3840x2160 = 13,
        _5120x2880 = 14,
        _7680x4320 = 15,
        _8192x4608 = 16,
        #endregion
        #region 16x10
        _1280x800 = 32,
        _1440x900 = 33,
        _1680x1050 = 34,
        _1920x1200 = 35,
        _2560x1600 = 36,
        _3840x2400 = 37,
        #endregion
        #region 21x9
        _1920x800 = 64,
        _2560x1080 = 65,
        _3440x1440 = 66,
        _3840x1600 = 67,
        _5120x2160 = 68,
        #endregion
        #region 32x9
        _3840x1080 = 96,
        _5120x1440 = 97,
        _6400x1800 = 98,
        _7680x2160 = 99,
        _10240x2880 = 100,
        #endregion
        #region 5x4
        _320x256 = 128,
        _600x480 = 129,
        _1280x1024 = 130,
        _1600x1280 = 131,
        _1800x1440 = 132,
        _2560x2048 = 133,
        _5120x4096 = 134,
        #endregion
        #region 4x3
        _320x240 = 160,
        _384x288 = 161,
        _400x300 = 162,
        _640x480 = 163,
        _800x600 = 164,
        _960x720 = 165,
        _1024x768 = 166,
        _1280x960 = 167,
        _1440x1080 = 168,
        _1920x1440 = 169,
        _2048x1536 = 170,
        _2560x1920 = 171,
        _2800x2100 = 172,
        _3200x2400 = 173,
        _6400x4800 = 174,
        #endregion
        #region 3x2
        _240x160 = 192,
        _480x320 = 193,
        _960x640 = 194,
        _1152x768 = 195,
        _1440x960 = 196,
        _1920x1280 = 197,
        _2160x1440 = 198,
        _2736x1824 = 199,
        _3000x2000 = 200,
        _3240x2160 = 201,
        _4500x3000 = 202,
        #endregion

        _native = 255
    }

    /// <summary>
    /// Define window modes
    /// </summary>
    /// <remarks>
    /// <para/>Borderless is only supported on Windows with the use of a third-party package (with no license).
    /// <para/>It will fall back to windowed on Linux/Mac and will not be available in the demo
    /// </remarks>
    public enum WindowMode : byte
    {
        /// <summary>
        /// Equivalent to Fullscreen Window
        /// </summary>
        Fullscreen = 0,

        /// <summary>
        /// Equivalent to Windowed
        /// </summary>
        Windowed = 1,

        /// <summary>
        /// Third party borderless, fullscreen
        /// </summary>
        Borderless = 2
    }
}