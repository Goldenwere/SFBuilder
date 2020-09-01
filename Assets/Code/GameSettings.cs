using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
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
                SetInputOverrides();
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
            SetInputOverrides();
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
                    SettingsData loadedData = (SettingsData)xs.Deserialize(txtReader);

                    if (loadedData.saveVersion < GameConstants.__GAME_VERSION)
                        Settings = UpdateFile(loadedData);
                    else
                        Settings = loadedData;
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
                settings = SettingsData.Default;
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

        /// <summary>
        /// Used for updating the settings file when it is out of date
        /// </summary>
        /// <param name="loadedData">The originally loaded file data</param>
        /// <returns>The updated file data</returns>
        private SettingsData UpdateFile(SettingsData loadedData)
        {
            // Before v0.19, there were no controls and no form of save-file versioning
            if (loadedData.saveVersion < 0.01)
            {
                SettingsData _default = SettingsData.Default;
                loadedData.controlBindings_Gamepad = _default.controlBindings_Gamepad;
                loadedData.controlBindings_Keyboard = _default.controlBindings_Keyboard;
                loadedData.controlBindings_Other = _default.controlBindings_Other;
                loadedData.controlSetting_HoldModifiers = _default.controlSetting_HoldModifiers;
                loadedData.controlSetting_InvertHorizontal = _default.controlSetting_InvertHorizontal;
                loadedData.controlSetting_InvertScroll = _default.controlSetting_InvertScroll;
                loadedData.controlSetting_InvertVertical = _default.controlSetting_InvertVertical;
                loadedData.controlSetting_SensitivityMovement = _default.controlSetting_SensitivityMovement;
                loadedData.controlSetting_SensitivityRotation = _default.controlSetting_SensitivityRotation;
                loadedData.controlSetting_SensitivityZoom = _default.controlSetting_SensitivityZoom;
            }

            loadedData.saveVersion = GameConstants.__GAME_VERSION;
            return loadedData;
        }

        /// <summary>
        /// Use to set input overrides; this is done with Saving, but exposed in case of Reverting
        /// </summary>
        public void SetInputOverrides()
        {
            foreach(ControlBinding cb in settings.controlBindings_Gamepad)
            {
                InputAction action = ControlBinding.ControlToAction(cb.Control, "Gamepad", out int i);
                if (i > -1)
                {
                    InputBinding binding = action.actionMap.bindings[i];
                    binding.overridePath = cb.Path;
                    DefaultActionMap.ApplyBindingOverride(i, binding);
                }
                else
                {
                    InputBinding binding = action.bindings[1];
                    binding.overridePath = cb.Path;
                    DefaultActionMap.ApplyBindingOverride(DefaultActionMap.bindings.IndexOf(b => b.action == action.name), binding);
                }
            }

            foreach (ControlBinding cb in settings.controlBindings_Keyboard)
            {
                InputAction action = ControlBinding.ControlToAction(cb.Control, "Keyboard", out int i);
                if (i > -1)
                {
                    InputBinding binding = action.actionMap.bindings[i];
                    binding.overridePath = cb.Path;
                    DefaultActionMap.ApplyBindingOverride(i, binding);
                }
                else
                {
                    InputBinding binding = action.bindings[1];
                    binding.overridePath = cb.Path;
                    DefaultActionMap.ApplyBindingOverride(DefaultActionMap.bindings.IndexOf(b => b.action == action.name), binding);
                }
            }

            foreach (ControlBinding cb in settings.controlBindings_Other)
            {
                // It is fine to assume gamepad, as that is the only composite (and is always gamepad) and thus the only one requiring pathStart
                InputAction action = ControlBinding.ControlToAction(cb.Control, "Gamepad", out int i);
                if (i > -1)
                {
                    InputBinding binding = action.actionMap.bindings[i];
                    binding.overridePath = cb.Path;
                    DefaultActionMap.ApplyBindingOverride(i, binding);
                }
                else
                {
                    InputBinding binding = action.bindings[0];
                    binding.overridePath = cb.Path;
                    DefaultActionMap.ApplyBindingOverride(DefaultActionMap.bindings.IndexOf(b => b.action == action.name), binding);
                }
            }
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

        public double           saveVersion;

        public float            volEffects;
        public float            volMusic;

        /// <summary>
        /// Defines what the default settings are for new players
        /// </summary>
        public static SettingsData Default => new SettingsData
        {
            controlBindings_Gamepad = new ControlBinding[]
            {
                new ControlBinding(GameControl.Camera_MoveBackward,     "<Gamepad>/leftStick/down"),
                new ControlBinding(GameControl.Camera_MoveForward,      "<Gamepad>/leftStick/up"),
                new ControlBinding(GameControl.Camera_MoveLeft,         "<Gamepad>/leftStick/left"),
                new ControlBinding(GameControl.Camera_MoveRight,        "<Gamepad>/leftStick/right"),
                new ControlBinding(GameControl.Camera_RotateLeft,       "<Gamepad>/leftTrigger"),
                new ControlBinding(GameControl.Camera_RotateRight,      "<Gamepad>/rightTrigger"),
                new ControlBinding(GameControl.Camera_TiltDown,         "<Gamepad>/rightShoulder"),
                new ControlBinding(GameControl.Camera_TiltUp,           "<Gamepad>/leftShoulder"),
                new ControlBinding(GameControl.Camera_ZoomIn,           "<Gamepad>/leftStick/up"),
                new ControlBinding(GameControl.Camera_ZoomOut,          "<Gamepad>/leftStick/down"),
                new ControlBinding(GameControl.Gameplay_CancelAndMenu,  "<Gamepad>/start"),
                new ControlBinding(GameControl.Gameplay_Placement,      "<Gamepad>/buttonSouth"),
                new ControlBinding(GameControl.Gameplay_Undo,           "<Gamepad>/buttonWest"),
            },
            controlBindings_Keyboard = new ControlBinding[]
            {
                new ControlBinding(GameControl.Camera_MoveBackward,     "<Keyboard>/s"),
                new ControlBinding(GameControl.Camera_MoveForward,      "<Keyboard>/w"),
                new ControlBinding(GameControl.Camera_MoveLeft,         "<Keyboard>/a"),
                new ControlBinding(GameControl.Camera_MoveRight,        "<Keyboard>/d"),
                new ControlBinding(GameControl.Camera_RotateLeft,       "<Keyboard>/q"),
                new ControlBinding(GameControl.Camera_RotateRight,      "<Keyboard>/e"),
                new ControlBinding(GameControl.Camera_TiltDown,         "<Keyboard>/f"),
                new ControlBinding(GameControl.Camera_TiltUp,           "<Keyboard>/r"),
                new ControlBinding(GameControl.Camera_ZoomIn,           "<Keyboard>/g"),
                new ControlBinding(GameControl.Camera_ZoomOut,          "<Keyboard>/t"),
                new ControlBinding(GameControl.Gameplay_CancelAndMenu,  "<Keyboard>/escape"),
                new ControlBinding(GameControl.Gameplay_Placement,      "<Keyboard>/enter"),
                new ControlBinding(GameControl.Gameplay_Undo,           "<Keyboard>/backspace"),
            },
            controlBindings_Other = new ControlBinding[]
            {
                new ControlBinding(GameControl.Gamepad_CursorDown,      "<Gamepad>/rightStick/down"),
                new ControlBinding(GameControl.Gamepad_CursorLeft,      "<Gamepad>/rightStick/left"),
                new ControlBinding(GameControl.Gamepad_CursorRight,     "<Gamepad>/rightStick/right"),
                new ControlBinding(GameControl.Gamepad_CursorUp,        "<Gamepad>/rightStick/up"),
                new ControlBinding(GameControl.Gamepad_ZoomToggle,      "<Gamepad>/leftStickPress"),
                new ControlBinding(GameControl.Mouse_ToggleMovement,    "<Mouse>/leftButton"),
                new ControlBinding(GameControl.Mouse_ToggleRotation,    "<Mouse>/rightButton"),
                new ControlBinding(GameControl.Mouse_ToggleZoom,        "<Keyboard>/ctrl"),
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

        /// <summary>
        /// Creates a deep copy of SettingsData
        /// </summary>
        /// <param name="other">The existing SettingsData being copied</param>
        /// <returns>The copied SettingsData</returns>
        public static SettingsData Copy(SettingsData other)
        {
            SettingsData newData = (SettingsData)other.MemberwiseClone();

            newData.controlBindings_Gamepad = new ControlBinding[other.controlBindings_Gamepad.Length];
            newData.controlBindings_Keyboard = new ControlBinding[other.controlBindings_Keyboard.Length];
            newData.controlBindings_Other = new ControlBinding[other.controlBindings_Other.Length];
            for (int i = 0; i < newData.controlBindings_Gamepad.Length; i++)
                newData.controlBindings_Gamepad[i] = ControlBinding.Copy(other.controlBindings_Gamepad[i]);
            for (int i = 0; i < newData.controlBindings_Keyboard.Length; i++)
                newData.controlBindings_Keyboard[i] = ControlBinding.Copy(other.controlBindings_Keyboard[i]);
            for (int i = 0; i < newData.controlBindings_Other.Length; i++)
                newData.controlBindings_Other[i] = ControlBinding.Copy(other.controlBindings_Other[i]);
            return newData;
        }
    }

    /// <summary>
    /// Structure and functions for game control bindings
    /// </summary>
    public struct ControlBinding : IXmlSerializable
    {
        public GameControl Control { get; private set; }
        public string      Path { get; private set; }

        /// <summary>
        /// Create new control binding
        /// </summary>
        /// <param name="_control">The control associated with the binding</param>
        /// <param name="_path">The path which InputSystem uses for input from the control</param>
        public ControlBinding(GameControl _control, string _path)
        {
            Control = _control;
            Path = _path;
        }

        /// <summary>
        /// Create deep copy of an existing ControlBinding
        /// </summary>
        /// <param name="other">The original ControlBinding</param>
        /// <returns>The copied ControlBinding</returns>
        public static ControlBinding Copy(ControlBinding other)
        {
            return new ControlBinding(other.Control, string.Copy(other.Path));
        }

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

        /// <summary>
        /// Implementation of GetScheme
        /// </summary>
        /// <returns>null</returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return (null);
        }

        /// <summary>
        /// Implementation of ReadXml
        /// </summary>
        /// <param name="reader">The reader for the file</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (Enum.TryParse(reader.GetAttribute("control"), out GameControl _control))
                Control = _control;
            Path = reader.GetAttribute("path");
            reader.Skip();
        }

        /// <summary>
        /// Implementation of WriteXml
        /// </summary>
        /// <param name="writer">The writer for the file</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("control", Control.ToString());
            writer.WriteAttributeString("path", Path);
        }
    }
}