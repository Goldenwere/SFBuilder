using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SFBuilder
{
    /// <summary>
    /// Manages game settings
    /// </summary>
    public class GameSettings : MonoBehaviour
    {
        #region Fields
        [SerializeField] private InputActionMap     defaultActionMap;
        /**************/ private SettingsData       settings;
        #endregion

        #region Properties
        /// <summary>
        /// The default action map for use in determining bindings
        /// </summary>
        public InputActionMap       DefaultActionMap { get { return defaultActionMap; } }

        /// <summary>
        /// Singleton instance of GameSettings in the base scene
        /// </summary>
        public static GameSettings  Instance { get; private set; }

        /// <summary>
        /// The current settings
        /// </summary>
        public SettingsData         Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                GameEventSystem.Instance.NotifySettingsChanged();
                SaveSettings();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set singleton instance on Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            LoadSettings();
        }

        /// <summary>
        /// Loads game settings
        /// </summary>
        private void LoadSettings()
        {
            if (File.Exists(Application.persistentDataPath + GameConstants.DataPathSettings))
            {
                XmlSerializer xs;
                TextReader txtReader = null;

                try
                {
                    xs = new XmlSerializer(typeof(SettingsData));
                    txtReader = new StreamReader(Application.persistentDataPath + GameConstants.DataPathSettings);
                    Settings = (SettingsData)xs.Deserialize(txtReader);
                }

                catch (Exception)
                {
                    // TO-DO: singleton exception handler that opens a UI canvas outputting errors
                }

                finally
                {
                    if (txtReader != null)
                        txtReader.Close();
                }
            }

            else
            {
                settings = new SettingsData
                {
                    controlBindings_Gamepad = new ControlBinding_Generic[]
                    {
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveBackward,      path = "<Gamepad>/leftStick/down" },
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveForward,       path = "<Gamepad>/leftStick/up" },
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveLeft,          path = "<Gamepad>/leftStick/left" },
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveRight,         path = "<Gamepad>/leftStick/right" },
                        new ControlBinding_Generic { control = GenericControl.Camera_RotateLeft,        path = "<Gamepad>/leftTrigger" },
                        new ControlBinding_Generic { control = GenericControl.Camera_RotateRight,       path = "<Gamepad>/rightTrigger" },
                        new ControlBinding_Generic { control = GenericControl.Camera_TiltDown,          path = "<Gamepad>/rightShoulder" },
                        new ControlBinding_Generic { control = GenericControl.Camera_TiltUp,            path = "<Gamepad>/leftShoulder" },
                        new ControlBinding_Generic { control = GenericControl.Camera_ZoomIn,            path = "<Gamepad>/leftStick/up" },
                        new ControlBinding_Generic { control = GenericControl.Camera_ZoomOut,           path = "<Gamepad>/leftStick/down" },
                        new ControlBinding_Generic { control = GenericControl.Gameplay_CancelAndMenu,   path = "<Gamepad>/start" },
                        new ControlBinding_Generic { control = GenericControl.Gameplay_Placement,       path = "<Gamepad>/buttonSouth" },
                        new ControlBinding_Generic { control = GenericControl.Gameplay_Undo,            path = "<Gamepad>/buttonWest" },
                    },
                    controlBindings_Keyboard = new ControlBinding_Generic[]
                    {
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveBackward,      path = "<Keyboard>/s" },
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveForward,       path = "<Keyboard>/w" },
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveLeft,          path = "<Keyboard>/a" },
                        new ControlBinding_Generic { control = GenericControl.Camera_MoveRight,         path = "<Keyboard>/d" },
                        new ControlBinding_Generic { control = GenericControl.Camera_RotateLeft,        path = "<Keyboard>/q" },
                        new ControlBinding_Generic { control = GenericControl.Camera_RotateRight,       path = "<Keyboard>/e" },
                        new ControlBinding_Generic { control = GenericControl.Camera_TiltDown,          path = "<Keyboard>/f" },
                        new ControlBinding_Generic { control = GenericControl.Camera_TiltUp,            path = "<Keyboard>/r" },
                        new ControlBinding_Generic { control = GenericControl.Camera_ZoomIn,            path = "<Keyboard>/g" },
                        new ControlBinding_Generic { control = GenericControl.Camera_ZoomOut,           path = "<Keyboard>/t" },
                        new ControlBinding_Generic { control = GenericControl.Gameplay_CancelAndMenu,   path = "<Keyboard>/escape" },
                        new ControlBinding_Generic { control = GenericControl.Gameplay_Placement,       path = "<Keyboard>/enter" },
                        new ControlBinding_Generic { control = GenericControl.Gameplay_Undo,            path = "<Keyboard>/backspace" },
                    },
                    controlBindings_Other = new ControlBinding_Other[]
                    {
                        new ControlBinding_Other { control = OtherControl.Gamepad_CursorDown,           path = "<Gamepad>/rightStick/down" },
                        new ControlBinding_Other { control = OtherControl.Gamepad_CursorLeft,           path = "<Gamepad>/rightStick/left" },
                        new ControlBinding_Other { control = OtherControl.Gamepad_CursorRight,          path = "<Gamepad>/rightStick/right" },
                        new ControlBinding_Other { control = OtherControl.Gamepad_CursorUp,             path = "<Gamepad>/rightStick/up" },
                        new ControlBinding_Other { control = OtherControl.Gamepad_ZoomToggle,           path = "<Gamepad>/leftStickPress" },
                        new ControlBinding_Other { control = OtherControl.Mouse_ToggleMovement,         path = "<Mouse>/leftButton" },
                        new ControlBinding_Other { control = OtherControl.Mouse_ToggleRotation,         path = "<Mouse>/rightButton" },
                        new ControlBinding_Other { control = OtherControl.Mouse_ToggleZoom,             path = "<Keyboard>/ctrl" },
                    },
                    controlSetting_HoldModifiers = false,
                    controlSetting_InvertHorizontal = false,
                    controlSetting_InvertScroll = false,
                    controlSetting_InvertVertical = false,
                    controlSetting_SensitivityMovement = 1,
                    controlSetting_SensitivityRotation = 1,
                    controlSetting_SensitivityZoom = 1,
                    postprocAO = false,
                    postprocBloom = false,
                    postprocSSR = false,
                    volEffects = 1.0f,
                    volMusic = 1.0f
                };
                if (SaveSettings())
                    LoadSettings();
            }
        }

        /// <summary>
        /// Save game settings
        /// </summary>
        /// <returns>Whether there was an error or not</returns>
        private bool SaveSettings()
        {
            XmlSerializer xs;
            TextWriter txtWriter = null;

            try
            {
                xs = new XmlSerializer(typeof(SettingsData));
                txtWriter = new StreamWriter(Application.persistentDataPath + GameConstants.DataPathSettings);
                xs.Serialize(txtWriter, settings);
                return true;
            }

            catch (Exception)
            {
                // TO-DO: singleton exception handler that opens a UI canvas outputting errors
            }

            finally
            {
                if (txtWriter != null)
                    txtWriter.Close();
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Data structure for game settings
    /// </summary>
    public struct SettingsData
    {
        public ControlBinding_Generic[] controlBindings_Gamepad;
        public ControlBinding_Generic[] controlBindings_Keyboard;
        public ControlBinding_Other[]   controlBindings_Other;

        public bool                     controlSetting_HoldModifiers;
        public bool                     controlSetting_InvertHorizontal;
        public bool                     controlSetting_InvertScroll;
        public bool                     controlSetting_InvertVertical;
        public float                    controlSetting_SensitivityMovement;
        public float                    controlSetting_SensitivityRotation;
        public float                    controlSetting_SensitivityZoom;

        public bool                     postprocAO;
        public bool                     postprocBloom;
        public bool                     postprocSSR;

        public float                    volEffects;
        public float                    volMusic;
    }

    /// <summary>
    /// Contains utility functions for use in control bindings
    /// </summary>
    public static class ControlBinding
    {
        /// <summary>
        /// Gets the index of a composite action
        /// </summary>
        /// <param name="map">The action map being searched</param>
        /// <param name="action">The action being found</param>
        /// <param name="pathStart">String indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="compositeName">The specific part of the composite (e.g. Vector2 --> up, down, left, right)</param>
        /// <returns>The index of a specific binding of a composite</returns>
        public static int GetIndex(InputActionMap map, string action, string pathStart, string compositeName)
        {
            return map.FindAction(action).bindings.IndexOf(b => b.isPartOfComposite && b.name == compositeName && b.path.Contains(pathStart));
        }

        /// <summary>
        /// Gets the path of a button action
        /// </summary>
        /// <param name="map">The action map being searched</param>
        /// <param name="action">The action being found</param>
        /// <param name="pathStart">Index indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <returns>The full input path of a binding</returns>
        public static string GetPath(InputActionMap map, string action, int pathStart)
        {
            return map.FindAction(action).bindings[pathStart].path;
        }

        /// <summary>
        /// Gets the path of a composite action
        /// </summary>
        /// <param name="map">The action map being searched</param>
        /// <param name="action">The action being found</param>
        /// <param name="pathStart">String indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="compositeName">The specific part of the composite (e.g. Vector2 --> up, down, left, right)</param>
        /// <returns>The full input path of a binding</returns>
        public static string GetPath(InputActionMap map, string action, string pathStart, string compositeName)
        {
            int i = map.FindAction(action).bindings.IndexOf(b => b.isPartOfComposite && b.name == compositeName && b.path.Contains(pathStart));
            return map.FindAction(action).bindings[i].path;
        }
    }

    /// <summary>
    /// Data structure for associating GenericControl to a path
    /// </summary>
    public struct ControlBinding_Generic
    {
        public GenericControl   control;
        public string           path;

        /// <summary>
        /// Converts a control to InputAction
        /// </summary>
        /// <param name="control">The control being converted</param>
        /// <param name="pathStart">Index indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="index">Composite actions have an associated index for a specific part of the composite; this is otherwise -1 for non-composites</param>
        public static InputAction ControlToAction(GenericControl control, string pathStart, out int index)
        {
            switch (control)
            {
                case GenericControl.Camera_MoveBackward:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "down");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GenericControl.Camera_MoveForward:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "up");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GenericControl.Camera_MoveLeft:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "left");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GenericControl.Camera_MoveRight:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "right");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GenericControl.Camera_RotateLeft:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "left");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GenericControl.Camera_RotateRight:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "right");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GenericControl.Camera_TiltDown:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "down");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GenericControl.Camera_TiltUp:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "up");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GenericControl.Camera_ZoomIn:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionZoom", pathStart, "positive");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionZoom");
                case GenericControl.Camera_ZoomOut:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "ActionZoom", pathStart, "negative");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionZoom");
                case GenericControl.Gameplay_CancelAndMenu:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("Menu");
                case GenericControl.Gameplay_Placement:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("Placement");
                case GenericControl.Gameplay_Undo:
                default:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("Undo");
            }
        }
    }

    /// <summary>
    /// Data structure for associating OtherControl to a path
    /// </summary>
    public struct ControlBinding_Other
    {
        public OtherControl     control;
        public string           path;

        /// <summary>
        /// Converts a control to InputAction
        /// </summary>
        /// <param name="control">The control being converted</param>
        /// <param name="pathStart">Index indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="index">Composite actions have an associated index for a specific part of the composite; this is otherwise -1 for non-composites</param>
        public static InputAction ControlToAction(OtherControl control, string pathStart, out int index)
        {
            switch (control)
            {
                case OtherControl.Gamepad_CursorDown:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "down");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case OtherControl.Gamepad_CursorLeft:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "left");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case OtherControl.Gamepad_CursorRight:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "right");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case OtherControl.Gamepad_CursorUp:
                    index = ControlBinding.GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "up");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case OtherControl.Gamepad_ZoomToggle:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("GamepadToggleZoom");
                case OtherControl.Mouse_ToggleMovement:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("MouseToggleMovement");
                case OtherControl.Mouse_ToggleRotation:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("MouseToggleRotation");
                case OtherControl.Mouse_ToggleZoom:
                default:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("MouseToggleZoom");
            }
        }
    }
}