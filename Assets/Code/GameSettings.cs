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
#pragma warning disable 0649
        [SerializeField] private InputActionAsset   defaultActionMap;
#pragma warning restore 0649
        /**************/ private SettingsData       settings;
        #endregion

        #region Properties
        /// <summary>
        /// The default action map for use in determining bindings
        /// </summary>
        public InputActionMap       DefaultActionMap { get { return defaultActionMap.actionMaps[0]; } }

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
                    controlBindings_Gamepad = new ControlBinding[]
                    {
                        new ControlBinding { control = GameControl.Camera_MoveBackward,     path = "<Gamepad>/leftStick/down" },
                        new ControlBinding { control = GameControl.Camera_MoveForward,      path = "<Gamepad>/leftStick/up" },
                        new ControlBinding { control = GameControl.Camera_MoveLeft,         path = "<Gamepad>/leftStick/left" },
                        new ControlBinding { control = GameControl.Camera_MoveRight,        path = "<Gamepad>/leftStick/right" },
                        new ControlBinding { control = GameControl.Camera_RotateLeft,       path = "<Gamepad>/leftTrigger" },
                        new ControlBinding { control = GameControl.Camera_RotateRight,      path = "<Gamepad>/rightTrigger" },
                        new ControlBinding { control = GameControl.Camera_TiltDown,         path = "<Gamepad>/rightShoulder" },
                        new ControlBinding { control = GameControl.Camera_TiltUp,           path = "<Gamepad>/leftShoulder" },
                        new ControlBinding { control = GameControl.Camera_ZoomIn,           path = "<Gamepad>/leftStick/up" },
                        new ControlBinding { control = GameControl.Camera_ZoomOut,          path = "<Gamepad>/leftStick/down" },
                        new ControlBinding { control = GameControl.Gameplay_CancelAndMenu,  path = "<Gamepad>/start" },
                        new ControlBinding { control = GameControl.Gameplay_Placement,      path = "<Gamepad>/buttonSouth" },
                        new ControlBinding { control = GameControl.Gameplay_Undo,           path = "<Gamepad>/buttonWest" },
                    },
                    controlBindings_Keyboard = new ControlBinding[]
                    {
                        new ControlBinding { control = GameControl.Camera_MoveBackward,     path = "<Keyboard>/s" },
                        new ControlBinding { control = GameControl.Camera_MoveForward,      path = "<Keyboard>/w" },
                        new ControlBinding { control = GameControl.Camera_MoveLeft,         path = "<Keyboard>/a" },
                        new ControlBinding { control = GameControl.Camera_MoveRight,        path = "<Keyboard>/d" },
                        new ControlBinding { control = GameControl.Camera_RotateLeft,       path = "<Keyboard>/q" },
                        new ControlBinding { control = GameControl.Camera_RotateRight,      path = "<Keyboard>/e" },
                        new ControlBinding { control = GameControl.Camera_TiltDown,         path = "<Keyboard>/f" },
                        new ControlBinding { control = GameControl.Camera_TiltUp,           path = "<Keyboard>/r" },
                        new ControlBinding { control = GameControl.Camera_ZoomIn,           path = "<Keyboard>/g" },
                        new ControlBinding { control = GameControl.Camera_ZoomOut,          path = "<Keyboard>/t" },
                        new ControlBinding { control = GameControl.Gameplay_CancelAndMenu,  path = "<Keyboard>/escape" },
                        new ControlBinding { control = GameControl.Gameplay_Placement,      path = "<Keyboard>/enter" },
                        new ControlBinding { control = GameControl.Gameplay_Undo,           path = "<Keyboard>/backspace" },
                    },
                    controlBindings_Other = new ControlBinding[]
                    {
                        new ControlBinding { control = GameControl.Gamepad_CursorDown,      path = "<Gamepad>/rightStick/down" },
                        new ControlBinding { control = GameControl.Gamepad_CursorLeft,      path = "<Gamepad>/rightStick/left" },
                        new ControlBinding { control = GameControl.Gamepad_CursorRight,     path = "<Gamepad>/rightStick/right" },
                        new ControlBinding { control = GameControl.Gamepad_CursorUp,        path = "<Gamepad>/rightStick/up" },
                        new ControlBinding { control = GameControl.Gamepad_ZoomToggle,      path = "<Gamepad>/leftStickPress" },
                        new ControlBinding { control = GameControl.Mouse_ToggleMovement,    path = "<Mouse>/leftButton" },
                        new ControlBinding { control = GameControl.Mouse_ToggleRotation,    path = "<Mouse>/rightButton" },
                        new ControlBinding { control = GameControl.Mouse_ToggleZoom,        path = "<Keyboard>/ctrl" },
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
        public ControlBinding[] controlBindings_Gamepad;
        public ControlBinding[] controlBindings_Keyboard;
        public ControlBinding[] controlBindings_Other;

        public bool             controlSetting_HoldModifiers;
        public bool             controlSetting_InvertHorizontal;
        public bool             controlSetting_InvertScroll;
        public bool             controlSetting_InvertVertical;
        public float            controlSetting_SensitivityMovement;
        public float            controlSetting_SensitivityRotation;
        public float            controlSetting_SensitivityZoom;

        public bool             postprocAO;
        public bool             postprocBloom;
        public bool             postprocSSR;

        public float            volEffects;
        public float            volMusic;
    }

    /// <summary>
    /// Structure and functions for game control bindings
    /// </summary>
    public struct ControlBinding
    {
        public GameControl  control;
        public string       path;

        /// <summary>
        /// Converts a control to InputAction
        /// </summary>
        /// <param name="control">The control being converted</param>
        /// <param name="pathStart">Index indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="index">Composite actions have an associated index for a specific part of the composite; this is otherwise -1 for non-composites</param>
        public static InputAction ControlToAction(GameControl control, string pathStart, out int index)
        {
            switch (control)
            {
                case GameControl.Gamepad_CursorDown:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "down");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case GameControl.Gamepad_CursorLeft:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "left");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case GameControl.Gamepad_CursorRight:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "right");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case GameControl.Gamepad_CursorUp:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "MoveCursor", pathStart, "up");
                    return GameSettings.Instance.DefaultActionMap.FindAction("MoveCursor");
                case GameControl.Gamepad_ZoomToggle:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("GamepadToggleZoom");
                case GameControl.Mouse_ToggleMovement:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("MouseToggleMovement");
                case GameControl.Mouse_ToggleRotation:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("MouseToggleRotation");
                case GameControl.Mouse_ToggleZoom:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("MouseToggleZoom");
                case GameControl.Camera_MoveBackward:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "down");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GameControl.Camera_MoveForward:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "up");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GameControl.Camera_MoveLeft:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "left");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GameControl.Camera_MoveRight:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionMovement", pathStart, "right");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionMovement");
                case GameControl.Camera_RotateLeft:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "left");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GameControl.Camera_RotateRight:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "right");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GameControl.Camera_TiltDown:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "down");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GameControl.Camera_TiltUp:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionRotation", pathStart, "up");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionRotation");
                case GameControl.Camera_ZoomIn:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionZoom", pathStart, "positive");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionZoom");
                case GameControl.Camera_ZoomOut:
                    index = GetIndex(GameSettings.Instance.DefaultActionMap, "ActionZoom", pathStart, "negative");
                    return GameSettings.Instance.DefaultActionMap.FindAction("ActionZoom");
                case GameControl.Gameplay_CancelAndMenu:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("Menu");
                case GameControl.Gameplay_Placement:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("Placement");
                case GameControl.Gameplay_Undo:
                default:
                    index = -1;
                    return GameSettings.Instance.DefaultActionMap.FindAction("Undo");
            }
        }

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
            return map.FindAction(action).bindings[GetIndex(map, action, pathStart, compositeName)].path;
        }
    }
}