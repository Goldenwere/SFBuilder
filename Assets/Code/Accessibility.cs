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
        Small = 0,      // 075% normal size, also applies a scale of 0.75 to HUD icons
        Medium = 1,     // 100% normal size, also applies a scale of 1.00 to HUD icons
        Large = 2       // 125% normal size, also applies a scale of 1.25 to HUD icons
    }

    /// <summary>
    /// Definitions of font format styles for use in sizing text elements after applying a universal FontSize setting
    /// </summary>
    public enum FontFormat : byte
    {
        Title = 0,              // normal 12.0, use for Settings/Game title and rebinding control indicator
        Heading = 1,            // normal 10.0, use for submenu titles and window prompt text
        Subheading = 2,         // normal 6.0, use for submenu subtitles
        Label = 3,              // normal 5.0, use for settings labels and slider values
        InnerLabel = 4,         // normal 3.5, use for dropdown/button inner text in settings menu and save/exit/revert buttons
        Tooltip = 5,            // normal 2.5, use for tooltips and version number
        GameWindow = 6,         // normal 5.5, use for text info in game window popups
        GameWindowSubText = 7,  // normal 3.0, use for lower text in game window popups
        HUDIndicators = 8,      // normal 3.0, use for indicators (stats, building count)
        MainButtons = 9         // normal 8.0, use for main menu buttons
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