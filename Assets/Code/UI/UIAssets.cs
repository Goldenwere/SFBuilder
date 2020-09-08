using UnityEngine;
using TMPro;

namespace SFBuilder.UI
{
    public class UIAssets : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private ControlIcons       controlIcons;
        [SerializeField] private FontCollection[]   fonts;
#pragma warning restore 0649

        public ControlIcons     ControlIconRefs { get { return controlIcons; } }
        public FontCollection[] Fonts           { get { return fonts; } }
        public static UIAssets  Instance        { get; private set; }

        /// <summary>
        /// Set singleton instance on Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }
    }

    /// <summary>
    /// Structure for associating TextTypes with materials
    /// </summary>
    [System.Serializable]
    public struct MaterialPresetsCollection
    {
        public TextType type;
        public Material material;
    }

    /// <summary>
    /// Structure for associating FontStyles with presets and font assets
    /// </summary>
    [System.Serializable]
    public struct FontCollection
    {
        public FontStyle                    style;
        public TMP_FontAsset                font;
        public MaterialPresetsCollection[]  presets;
    }

    /// <summary>
    /// Structure for referencing images used in the controls menu
    /// </summary>
    [System.Serializable]
    public struct ControlIcons
    {
        public Sprite gamepad_button_east;
        public Sprite gamepad_button_north;
        public Sprite gamepad_button_select;
        public Sprite gamepad_button_south;
        public Sprite gamepad_button_start;
        public Sprite gamepad_button_west;
        public Sprite gamepad_dpad_down;
        public Sprite gamepad_dpad_left;
        public Sprite gamepad_dpad_right;
        public Sprite gamepad_dpad_up;
        public Sprite gamepad_lthumbstick_down;
        public Sprite gamepad_lthumbstick_left;
        public Sprite gamepad_lthumbstick_press;
        public Sprite gamepad_lthumbstick_right;
        public Sprite gamepad_lthumbstick_up;
        public Sprite gamepad_lshoulder;
        public Sprite gamepad_ltrigger;
        public Sprite gamepad_rthumbstick_down;
        public Sprite gamepad_rthumbstick_left;
        public Sprite gamepad_rthumbstick_press;
        public Sprite gamepad_rthumbstick_right;
        public Sprite gamepad_rthumbstick_up;
        public Sprite gamepad_rshoulder;
        public Sprite gamepad_rtrigger;

        public Sprite keyboard_backspace;
        public Sprite keyboard_caps;
        public Sprite keyboard_down;
        public Sprite keyboard_enter;
        public Sprite keyboard_left;
        public Sprite keyboard_lshift;
        public Sprite keyboard_num;
        public Sprite keyboard_right;
        public Sprite keyboard_rshift;
        public Sprite keyboard_scr;
        public Sprite keyboard_space;
        public Sprite keyboard_tab;
        public Sprite keyboard_up;

        public Sprite mouse_back;
        public Sprite mouse_forward;
        public Sprite mouse_left;
        public Sprite mouse_middle;
        public Sprite mouse_right;
    }
}