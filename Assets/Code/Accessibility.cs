namespace SFBuilder
{
    /// <summary>
    /// Definitions of supported game fonts
    /// </summary>
    /// <remarks>Righteous is the default game font</remarks>
    public enum FontStyle : byte
    {
        Righteous = 0,
        OpenDyslexic = 1,
        Tiresias = 2
    }

    /// <summary>
    /// Definitions of font size settings
    /// </summary>
    /// <remarks>Medium is the default font size</remarks>
    public enum FontSize : byte
    {
        Small = 0,
        Medium = 1,
        Large = 2
    }

    /// <summary>
    /// Definitions of cursor size settings
    /// </summary>
    /// <remarks>Use Hardware=0 to disable custom cursor; Medium is the default cursor size</remarks>
    public enum CursorSize : byte
    {
        Hardware = 0,
        VerySmall = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
        VeryLarge = 5
    }
}