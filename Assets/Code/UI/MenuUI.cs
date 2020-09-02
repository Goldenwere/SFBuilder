using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Linq;
using System.Collections;
using Goldenwere.Unity.UI;
using Goldenwere.Unity;

namespace SFBuilder.UI
{
    /// <summary>
    /// Defines the submenus available in the settings menu
    /// </summary>
    public enum SettingsSubmenu
    {
        audio,
        controls,
        graphics
    }

    /// <summary>
    /// Manages UI and associated input/output for the game's menus
    /// </summary>
    public class MenuUI : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private CanvasGroup                    canvas;
        [SerializeField] private GameObject[]                   canvasMainElements;
        [SerializeField] private GameObject                     canvasRebindWindow;
        [SerializeField] private TMP_Text                       canvasRebindWindowTextIndicator;
        [SerializeField] private GameObject[]                   canvasSettingsElements;
        [SerializeField] private GameObject[]                   canvasSettingsSubmenuActiveBackgrounds;
        [SerializeField] private RectTransform                  canvasSettingsSubmenuContainer;
        [SerializeField] private GameObject[]                   canvasSettingsSubmenuElements;
        [SerializeField] private GameObject                     canvasWarningWindow;
        [SerializeField] private ControlsMenuImages             controlsMenuImages;
        [SerializeField] private SettingsMenuElements           settingsMenuElements;
        [SerializeField] private Image                          startupFadeImage;
        [SerializeField] private AnimationCurve                 transitionCurve;
        [SerializeField] private AnimationCurve                 transitionStartupFade;
        [SerializeField] private AnimationCurve                 transitionStartupToWhite;
        [SerializeField] private Image                          transitionImage;
#pragma warning restore 0649
        /**************/ private bool                           pendingChangesExist;
        /**************/ private SettingsData                   workingSettings;
        /**************/ private SettingsSubmenu                workingSettingsSubmenuState;
        #endregion

        #region Inline Classes
        /// <summary>
        /// Collection of images used for the controls menu
        /// </summary>
        [Serializable]
        protected class ControlsMenuImages
        {
            public Sprite   gamepad_button_east;
            public Sprite   gamepad_button_north;
            public Sprite   gamepad_button_select;
            public Sprite   gamepad_button_south;
            public Sprite   gamepad_button_start;
            public Sprite   gamepad_button_west;
            public Sprite   gamepad_dpad_down;
            public Sprite   gamepad_dpad_left;
            public Sprite   gamepad_dpad_right;
            public Sprite   gamepad_dpad_up;
            public Sprite   gamepad_lthumbstick_down;
            public Sprite   gamepad_lthumbstick_left;
            public Sprite   gamepad_lthumbstick_press;
            public Sprite   gamepad_lthumbstick_right;
            public Sprite   gamepad_lthumbstick_up;
            public Sprite   gamepad_lshoulder;
            public Sprite   gamepad_ltrigger;
            public Sprite   gamepad_rthumbstick_down;
            public Sprite   gamepad_rthumbstick_left;
            public Sprite   gamepad_rthumbstick_press;
            public Sprite   gamepad_rthumbstick_right;
            public Sprite   gamepad_rthumbstick_up;
            public Sprite   gamepad_rshoulder;
            public Sprite   gamepad_rtrigger;

            public Sprite   keyboard_backspace;
            public Sprite   keyboard_caps;
            public Sprite   keyboard_down;
            public Sprite   keyboard_enter;
            public Sprite   keyboard_left;
            public Sprite   keyboard_lshift;
            public Sprite   keyboard_num;
            public Sprite   keyboard_right;
            public Sprite   keyboard_rshift;
            public Sprite   keyboard_scr;
            public Sprite   keyboard_space;
            public Sprite   keyboard_tab;
            public Sprite   keyboard_up;

            public Sprite   mouse_back;
            public Sprite   mouse_forward;
            public Sprite   mouse_left;
            public Sprite   mouse_middle;
            public Sprite   mouse_right;
        }

        /// <summary>
        /// Collection of elements on the settings menu, whose values are set every time the menu loads
        /// </summary>
        [Serializable]
        protected class SettingsMenuElements
        {
            public ControlButton[]              controlButtons;
            public Toggle                       postprocAO;
            public Toggle                       postprocBloom;
            public Toggle                       postprocSSR;
            public Toggle                       invertHorizontal;
            public Toggle                       invertScroll;
            public Toggle                       invertVertical;
            public Toggle                       modifiersHeld;
            public SliderTextLoadExtension      sensitivityMovement;
            public SliderTextLoadExtension      sensitivityRotation;
            public SliderTextLoadExtension      sensitivityZoom;
            public SliderTextLoadExtension      volMusic;
            public SliderTextLoadExtension      volEffects;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Copy the material for transitions since it messes with asset file
        /// </summary>
        private void Start()
        {
            Material copy = new Material(transitionImage.material);
            transitionImage.material = copy;
            workingSettings = SettingsData.Copy(GameSettings.Instance.Settings);

            canvas.gameObject.SetActive(false);
            canvasRebindWindow.SetActive(false);
            canvasWarningWindow.SetActive(false);

            foreach (GameObject g in canvasSettingsElements)
                g.SetActive(false);
            // these need disabled first before re-enabling to ensure proper transition order
            foreach (GameObject g in canvasMainElements)
                g.SetActive(false);

            // Clear elements that were initially added that are now disabled
            UITransitionSystem.Instance.ClearElements();

            // Initialize the controls menu buttons
            foreach (ControlButton cb in settingsMenuElements.controlButtons)
                cb.onClick.AddListener(() => OnSetControl(cb));

            StartCoroutine(StartupAnimation());
            LoadSubmenu(SettingsSubmenu.graphics);
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
            GameEventSystem.LevelTransitioned += OnLevelTransitioned;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
            GameEventSystem.LevelTransitioned -= OnLevelTransitioned;
        }

        /// <summary>
        /// Loads settings data into the UI
        /// </summary>
        private void LoadSettings()
        {
            settingsMenuElements.postprocAO.SetIsOnWithoutNotify(workingSettings.postprocAO);
            settingsMenuElements.postprocBloom.SetIsOnWithoutNotify(workingSettings.postprocBloom);
            settingsMenuElements.postprocSSR.SetIsOnWithoutNotify(workingSettings.postprocSSR);

            settingsMenuElements.volEffects.AssociatedSlider.SetValueWithoutNotify(workingSettings.volEffects);
            settingsMenuElements.volEffects.UpdateText(string.Format("{0:P0}", workingSettings.volEffects));
            settingsMenuElements.volMusic.AssociatedSlider.SetValueWithoutNotify(workingSettings.volMusic);
            settingsMenuElements.volMusic.UpdateText(string.Format("{0:P0}", workingSettings.volMusic));

            foreach (ControlButton cb in settingsMenuElements.controlButtons)
            {
                if ((byte)cb.AssociatedControl < 13)
                {
                    if (cb.ExpectedInput[0] == InputType.Gamepad)
                        SetControlDisplay(cb, workingSettings.controlBindings_Gamepad.First(b => b.Control == cb.AssociatedControl).Path);
                    else
                        SetControlDisplay(cb, workingSettings.controlBindings_Keyboard.First(b => b.Control == cb.AssociatedControl).Path);
                }
                else
                    SetControlDisplay(cb, workingSettings.controlBindings_Other.First(b => b.Control == cb.AssociatedControl).Path);
            }

            settingsMenuElements.sensitivityMovement.AssociatedSlider.SetValueWithoutNotify(workingSettings.controlSetting_SensitivityMovement);
            settingsMenuElements.sensitivityMovement.UpdateText(string.Format("{0:0.##}", workingSettings.controlSetting_SensitivityMovement));
            settingsMenuElements.sensitivityRotation.AssociatedSlider.SetValueWithoutNotify(workingSettings.controlSetting_SensitivityRotation);
            settingsMenuElements.sensitivityRotation.UpdateText(string.Format("{0:0.##}", workingSettings.controlSetting_SensitivityRotation));
            settingsMenuElements.sensitivityZoom.AssociatedSlider.SetValueWithoutNotify(workingSettings.controlSetting_SensitivityZoom);
            settingsMenuElements.sensitivityZoom.UpdateText(string.Format("{0:0.##}", workingSettings.controlSetting_SensitivityZoom));

            settingsMenuElements.modifiersHeld.SetIsOnWithoutNotify(workingSettings.controlSetting_HoldModifiers);
            settingsMenuElements.invertHorizontal.SetIsOnWithoutNotify(workingSettings.controlSetting_InvertHorizontal);
            settingsMenuElements.invertScroll.SetIsOnWithoutNotify(workingSettings.controlSetting_InvertScroll);
            settingsMenuElements.invertVertical.SetIsOnWithoutNotify(workingSettings.controlSetting_InvertVertical);
        }

        /// <summary>
        /// Loads appropriate settings submenus based on the new submenu state
        /// </summary>
        /// <param name="newState">The submenu to load</param>
        private void LoadSubmenu(SettingsSubmenu newState)
        {
            string name;
            switch (newState)
            {
                case SettingsSubmenu.audio:
                    name = "Audio";
                    break;
                case SettingsSubmenu.controls:
                    name = "Controls";
                    break;
                case SettingsSubmenu.graphics:
                default:
                    name = "Graphics";
                    break;
            }

            foreach (GameObject go in canvasSettingsSubmenuElements)
            {
                if (go.name == name)
                {
                    go.SetActive(true);
                    canvasSettingsSubmenuContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, go.GetComponent<RectTransform>().sizeDelta.y);
                }
                else
                    go.SetActive(false);
            }
            foreach (GameObject go in canvasSettingsSubmenuActiveBackgrounds)
            {
                if (go.name == name)
                    go.SetActive(true);
                else
                    go.SetActive(false);
            }
            workingSettingsSubmenuState = newState;
        }

        /// <summary>
        /// On the GameStateChanged event, toggle the menu canvas
        /// </summary>
        /// <param name="prevState">The previous GameState</param>
        /// <param name="currState">The current GameState</param>
        private void OnGameStateChanged(GameState prevState, GameState currState)
        {
            UITransitionSystem.Instance.ClearElements();

            if (currState == GameState.MainMenus && prevState != GameState.MainMenus)
                StartCoroutine(SetActive(true));
            else if (currState != GameState.MainMenus && prevState == GameState.MainMenus)
                StartCoroutine(SetActive(false));
        }

        /// <summary>
        /// On the LevelTransitioned event, animate the transition Image to start/stop hiding level unloading/loading
        /// </summary>
        /// <param name="isStart">Whether starting or ending transition</param>
        private void OnLevelTransitioned(bool isStart)
        {
            if (GameEventSystem.Instance.CurrentGameState == GameState.Gameplay)
                StartCoroutine(AnimateTransition(isStart));
        }

        /// <summary>
        /// Sets the displayed value of a button on the controls menu
        /// </summary>
        /// <param name="element">The button being updated</param>
        /// <param name="path">The current control's full path</param>
        private void SetControlDisplay(Button element, string path)
        {
            string[] pathSplit = path.Split('/');
            TMP_Text text = element.gameObject.FindChild("Text").GetComponent<TMP_Text>();
            Image image = element.gameObject.FindChild("Image").GetComponent<Image>();
            GameObject textObj = element.gameObject.FindChild("Text");
            GameObject imageObj = element.gameObject.FindChild("Image");
            switch (pathSplit[0])
            {
                #region Gamepad
                case "<Gamepad>":
                    textObj.SetActive(false);
                    imageObj.SetActive(true);
                    switch (pathSplit[1])
                    {
                        case "buttonNorth":         image.sprite = controlsMenuImages.gamepad_button_north; break;
                        case "buttonSouth":         image.sprite = controlsMenuImages.gamepad_button_south; break;
                        case "buttonEast":          image.sprite = controlsMenuImages.gamepad_button_east; break;
                        case "buttonWest":          image.sprite = controlsMenuImages.gamepad_button_west; break;
                        case "select":              image.sprite = controlsMenuImages.gamepad_button_select; break;
                        case "start":
                        case "systemButton":        image.sprite = controlsMenuImages.gamepad_button_start; break;
                        case "leftStick":
                            switch (pathSplit[2])
                            {
                                case "up":          image.sprite = controlsMenuImages.gamepad_lthumbstick_up; break;
                                case "down":        image.sprite = controlsMenuImages.gamepad_lthumbstick_down; break;
                                case "left":        image.sprite = controlsMenuImages.gamepad_lthumbstick_left; break;
                                case "right":       image.sprite = controlsMenuImages.gamepad_lthumbstick_right; break;
                            }
                            break;
                        case "rightStick":
                            switch (pathSplit[2])
                            {
                                case "up":          image.sprite = controlsMenuImages.gamepad_rthumbstick_up; break;
                                case "down":        image.sprite = controlsMenuImages.gamepad_rthumbstick_down; break;
                                case "left":        image.sprite = controlsMenuImages.gamepad_rthumbstick_left; break;
                                case "right":       image.sprite = controlsMenuImages.gamepad_rthumbstick_right; break;
                            }
                            break;
                        case "dpad":
                            switch (pathSplit[2])
                            {
                                case "up":          image.sprite = controlsMenuImages.gamepad_dpad_up; break;
                                case "down":        image.sprite = controlsMenuImages.gamepad_dpad_down; break;
                                case "left":        image.sprite = controlsMenuImages.gamepad_dpad_left; break;
                                case "right":       image.sprite = controlsMenuImages.gamepad_dpad_right; break;
                            }
                            break;
                        case "leftShoulder":        image.sprite = controlsMenuImages.gamepad_lshoulder; break;
                        case "rightShoulder":       image.sprite = controlsMenuImages.gamepad_rshoulder; break;
                        case "leftTrigger":
                        case "leftTriggerButton":   image.sprite = controlsMenuImages.gamepad_ltrigger; break;
                        case "rightTrigger":
                        case "rightTriggerButton":  image.sprite = controlsMenuImages.gamepad_rtrigger; break;
                        case "leftStickPress":      image.sprite = controlsMenuImages.gamepad_lthumbstick_press; break;
                        case "rightStickPress":     image.sprite = controlsMenuImages.gamepad_rthumbstick_press; break;
                        default:
                            textObj.SetActive(true);
                            imageObj.SetActive(false);
                            text.text = "";
                            //text.fontSize = GameConstants.MenuControlsFontSizeWord;
                            for (int i = 1; i < pathSplit.Length; i++)
                                text.text += pathSplit[i];
                            break;
                    }
                    break;
                #endregion

                #region Mouse
                case "<Mouse>":
                    textObj.SetActive(false);
                    imageObj.SetActive(true);
                    switch (pathSplit[1])
                    {
                        case "leftButton":      image.sprite = controlsMenuImages.mouse_left; break;
                        case "middleButton":    image.sprite = controlsMenuImages.mouse_middle; break;
                        case "rightButton":     image.sprite = controlsMenuImages.mouse_right; break;
                        case "forward":         image.sprite = controlsMenuImages.mouse_forward; break;
                        case "back":            image.sprite = controlsMenuImages.mouse_back; break;
                        default:
                            textObj.SetActive(true);
                            imageObj.SetActive(false);
                            //text.fontSize = GameConstants.MenuControlsFontSizeWord;
                            text.text = "Mouse" + pathSplit[1];
                            break;
                    }
                    break;
                #endregion

                #region Keyboard
                case "<Keyboard>":
                default:
                    textObj.SetActive(true);
                    imageObj.SetActive(false);
                    // Captures most keys (letters, numbers in number row, and special characters)
                    if (pathSplit[1].Length == 1)
                    {
                        //text.fontSize = GameConstants.MenuControlsFontSizeChar;
                        text.text = pathSplit[1].ToUpper();
                    }

                    else
                    {
                        // Image-based keys
                        switch(pathSplit[1])
                        {
                            case "leftShift":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_lshift;
                                break;
                            case "rightShift":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_rshift;
                                break;
                            case "space":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_space;
                                break;
                            case "backspace":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_backspace;
                                break;
                            case "enter":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_enter;
                                break;
                            case "tab":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_tab;
                                break;
                            case "capsLock":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_caps;
                                break;
                            case "numLock":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_num;
                                break;
                            case "scrollLock":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_scr;
                                break;
                            case "downArrow":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_down;
                                break;
                            case "leftArrow":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_left;
                                break;
                            case "rightArrow":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_right;
                                break;
                            case "upArrow":
                                textObj.SetActive(false);
                                imageObj.SetActive(true);
                                image.sprite = controlsMenuImages.keyboard_up;
                                break;
                            default:
                                string converted = GameConstants.InputNameToChar(pathSplit[1]);
                                text.text = converted;
                                /*
                                if (converted.Length < 4)
                                    text.fontSize = GameConstants.MenuControlsFontSizeChar;
                                else if (converted.Length < 8)
                                    text.fontSize = GameConstants.MenuControlsFontSizeMiniWord;
                                else
                                    text.fontSize = GameConstants.MenuControlsFontSizeWord;
                                */
                                break;
                        }
                    }
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// When the main menu button is pressed, load the main menu
        /// </summary>
        public void OnMainMenuPressed()
        {
            if (pendingChangesExist)
                canvasWarningWindow.SetActive(true);

            else
            {
                foreach (GameObject g in canvasMainElements)
                    g.SetActive(true);
                foreach (GameObject g in canvasSettingsElements)
                    g.SetActive(false);
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                canvasWarningWindow.SetActive(false);
            }
        }

        /// <summary>
        /// When the play button is pressed, load the game
        /// </summary>
        public void OnPlayPressed()
        {
            StartCoroutine(SetActive(false));
            GameEventSystem.Instance.UpdateGameState(GameState.Gameplay);
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Goal);
        }

        /// <summary>
        /// When the quit button is pressed, quit the game
        /// </summary>
        public void OnQuitPressed()
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            Application.Quit();
        }

        /// <summary>
        /// Handler for control buttons to set controls
        /// </summary>
        /// <param name="sender">The button sending the event</param>
        public void OnSetControl(ControlButton sender)
        {
            canvasRebindWindow.SetActive(true);
            string indicator = sender.AssociatedControl.ToString().Replace("Camera_", "").Replace("Gameplay_", "").Replace("Mouse_", "").Replace("Gamepad_", "");
            canvasRebindWindowTextIndicator.text = System.Text.RegularExpressions.Regex.Replace(indicator, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

            // Expected types is an array for use in excluding controls
            // pathStart is only necessary for actions with multiple bindings (i.e. those with bindings for both keyboard and gamepad),
            // which never have more than one expected type
            // Therefore, it is fine to assume expectedTypes[0].ToString() in all cases, because for any other action, it goes unused
            InputAction action = ControlBinding.ControlToAction(sender.AssociatedControl, sender.ExpectedInput[0].ToString(), out int index);
            action.Disable();

            InputActionRebindingExtensions.RebindingOperation rebindOp = action
                .PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse/delta")
                .WithExpectedControlType("Button")
                .OnMatchWaitForAnother(0.1f);
            if (index > -1)
                rebindOp.WithTargetBinding(index);

            if (!sender.ExpectedInput.Contains(InputType.Gamepad))
            {
                rebindOp.WithControlsExcluding("Gamepad");
                rebindOp.WithControlsExcluding("Joystick");
            }

            else
                rebindOp.WithCancelingThrough("Keyboard/escape");

            if (!sender.ExpectedInput.Contains(InputType.Keyboard))
                rebindOp.WithControlsExcluding("Keyboard");

            if (!sender.ExpectedInput.Contains(InputType.Mouse))
                rebindOp.WithControlsExcluding("Mouse");

            rebindOp.Start()
                .OnCancel(callback =>
                {
                    canvasRebindWindow.SetActive(false);
                    rebindOp?.Dispose();
                })
                .OnComplete(callback =>
                {
                    pendingChangesExist = true;
                    ControlBinding cb;              // reference to previous binding
                    ControlBinding newBinding;      // newly created binding after rebindOp
                    int i;                          // index of previous binding
                    switch (sender.AssociatedControl)
                    {
                        case GameControl.Camera_MoveBackward:
                        case GameControl.Camera_MoveForward:
                        case GameControl.Camera_MoveLeft:
                        case GameControl.Camera_MoveRight:
                        case GameControl.Camera_RotateLeft:
                        case GameControl.Camera_RotateRight:
                        case GameControl.Camera_TiltDown:
                        case GameControl.Camera_TiltUp:
                        case GameControl.Camera_ZoomIn:
                        case GameControl.Camera_ZoomOut:
                        case GameControl.Gameplay_CancelAndMenu:
                        case GameControl.Gameplay_Placement:
                        case GameControl.Gameplay_Undo:
                            // For multi-bound bindings, index 0 is keyboard, index 1 is gamepad; for placement, index 2 is present but not rebindable
                            if (sender.ExpectedInput[0] == InputType.Gamepad)
                            {
                                cb = workingSettings.controlBindings_Gamepad.First(b => b.Control == sender.AssociatedControl);
                                i = Array.IndexOf(workingSettings.controlBindings_Gamepad, cb);
                                if (index > -1)
                                {
                                    newBinding = new ControlBinding(workingSettings.controlBindings_Gamepad[i].Control,
                                        action.bindings[index].overridePath);
                                    SetControlDisplay(sender, newBinding.Path);
                                }
                                else
                                {
                                    newBinding = new ControlBinding(workingSettings.controlBindings_Gamepad[i].Control,
                                        action.bindings[1].overridePath);
                                    SetControlDisplay(sender, newBinding.Path);
                                }
                                workingSettings.controlBindings_Gamepad[i] = newBinding;
                            }
                            else
                            {
                                cb = workingSettings.controlBindings_Keyboard.First(b => b.Control == sender.AssociatedControl);
                                i = Array.IndexOf(workingSettings.controlBindings_Keyboard, cb);
                                if (index > -1)
                                {
                                    newBinding = new ControlBinding(workingSettings.controlBindings_Keyboard[i].Control,
                                        action.bindings[index].overridePath);
                                    SetControlDisplay(sender, newBinding.Path);
                                }
                                else
                                {
                                    newBinding = new ControlBinding(workingSettings.controlBindings_Keyboard[i].Control,
                                        action.bindings[0].overridePath);
                                    SetControlDisplay(sender, newBinding.Path);
                                }
                                workingSettings.controlBindings_Keyboard[i] = newBinding;
                            }
                            break;
                        case GameControl.Gamepad_CursorDown:
                        case GameControl.Gamepad_CursorLeft:
                        case GameControl.Gamepad_CursorRight:
                        case GameControl.Gamepad_CursorUp:
                        case GameControl.Gamepad_ZoomToggle:
                        case GameControl.Mouse_ToggleMovement:
                        case GameControl.Mouse_ToggleRotation:
                        case GameControl.Mouse_ToggleZoom:
                        default:
                            cb = workingSettings.controlBindings_Other.First(b => b.Control == sender.AssociatedControl);
                            i = Array.IndexOf(workingSettings.controlBindings_Other, cb);
                            if (index > -1)
                            {
                                newBinding = new ControlBinding(workingSettings.controlBindings_Other[i].Control,
                                    action.bindings[index].overridePath);
                                SetControlDisplay(sender, newBinding.Path);
                            }
                            // since these are not multi-bound, assume index 0
                            else
                            {
                                newBinding = new ControlBinding(workingSettings.controlBindings_Other[i].Control,
                                    action.bindings[0].overridePath);
                                SetControlDisplay(sender, newBinding.Path);
                            }
                            workingSettings.controlBindings_Other[i] = newBinding;
                            break;
                    }
                    canvasRebindWindow.SetActive(false);
                    action.Enable();
                    rebindOp?.Dispose();
                });
        }

        /// <summary>
        /// When the settings menu button is pressed, load the settings menu
        /// </summary>
        public void OnSettingsPressed()
        {
            workingSettings = SettingsData.Copy(GameSettings.Instance.Settings);
            LoadSettings();
            foreach (GameObject g in canvasMainElements)
                g.SetActive(false);
            foreach (GameObject g in canvasSettingsElements)
                g.SetActive(true);
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the revert button is pressed on the settings menu, revert all pending changes
        /// </summary>
        public void OnSettingsRevertPressed()
        {
            pendingChangesExist = false;
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings = SettingsData.Copy(GameSettings.Instance.Settings);
            LoadSettings();
            GameSettings.Instance.SetInputOverrides();
        }

        /// <summary>
        /// When the save button is pressed on the settings menu, save settings
        /// </summary>
        public void OnSettingsSavePressed()
        {
            pendingChangesExist = false;
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            GameSettings.Instance.Settings = SettingsData.Copy(workingSettings);
        }

        /// <summary>
        /// When the audio button is pressed, switch to that submenu
        /// </summary>
        public void OnSettingsSubmenuAudioPressed()
        {
            if (workingSettingsSubmenuState != SettingsSubmenu.audio)
            {
                LoadSubmenu(SettingsSubmenu.audio);
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            }
        }

        /// <summary>
        /// When the controls button is pressed, switch to that submenu
        /// </summary>
        public void OnSettingsSubmenuControlsPressed()
        {
            if (workingSettingsSubmenuState != SettingsSubmenu.controls)
            {
                LoadSubmenu(SettingsSubmenu.controls);
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            }
        }

        /// <summary>
        /// When the graphics button is pressed, switch to that submenu
        /// </summary>
        public void OnSettingsSubmenuGraphicsPressed()
        {
            if (workingSettingsSubmenuState != SettingsSubmenu.graphics)
            {
                LoadSubmenu(SettingsSubmenu.graphics);
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            }
        }

        /// <summary>
        /// Universal method to note that pending changes exist
        /// </summary>
        public void OnValueChanged()
        {
            pendingChangesExist = true;
        }

        /// <summary>
        /// Update effects vol on slider change
        /// </summary>
        /// <param name="val">New volume setting</param>
        public void OnValueChanged_Audio_Effects(float val)
        {
            workingSettings.volEffects = val;
            settingsMenuElements.volEffects.UpdateText(string.Format("{0:P0}", val));
        }

        /// <summary>
        /// Update music vol on slider change
        /// </summary>
        /// <param name="val">New volume setting</param>
        public void OnValueChanged_Audio_Music(float val)
        {
            workingSettings.volMusic = val;
            settingsMenuElements.volMusic.UpdateText(string.Format("{0:P0}", val));
        }

        /// <summary>
        /// Update AO on toggle change
        /// </summary>
        /// <param name="val">New toggle setting</param>
        public void OnValueChanged_Graphics_AO(bool val)
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings.postprocAO = val;
        }

        /// <summary>
        /// Update bloom on toggle change
        /// </summary>
        /// <param name="val">New toggle setting</param>
        public void OnValueChanged_Graphics_Bloom(bool val)
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings.postprocBloom = val;
        }

        /// <summary>
        /// Update SSR on toggle change
        /// </summary>
        /// <param name="val">New toggle setting</param>
        public void OnValueChanged_Graphics_SSR(bool val)
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings.postprocSSR = val;
        }

        /// <summary>
        /// Update the inversion setting for horizontal on toggle change
        /// </summary>
        /// <param name="val">The new inversion setting</param>
        public void OnValueChanged_InvertHorizontal(bool val)
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings.controlSetting_InvertHorizontal = val;
        }

        /// <summary>
        /// Update the inversion setting for scroll on toggle change
        /// </summary>
        /// <param name="val">The new inversion setting</param>
        public void OnValueChanged_InvertScroll(bool val)
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings.controlSetting_InvertScroll = val;
        }

        /// <summary>
        /// Update the inversion setting for vertical on toggle change
        /// </summary>
        /// <param name="val">The new inversion setting</param>
        public void OnValueChanged_InvertVertical(bool val)
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings.controlSetting_InvertVertical = val;
        }

        /// <summary>
        /// Update the held modifiers setting on toggle change
        /// </summary>
        /// <param name="val">The new modifiers are held setting</param>
        public void OnValueChanged_ModifiersHeld(bool val)
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings.controlSetting_HoldModifiers = val;
        }

        /// <summary>
        /// Update movement sensitivity on slider change
        /// </summary>
        /// <param name="val">New sensitivity value</param>
        public void OnValueChanged_SensitivityMovement(float val)
        {
            workingSettings.controlSetting_SensitivityMovement = val;
            settingsMenuElements.sensitivityMovement.UpdateText(string.Format("{0:0.##}", val));
        }

        /// <summary>
        /// Update rotation sensitivity on slider change
        /// </summary>
        /// <param name="val">New sensitivity value</param>
        public void OnValueChanged_SensitivityRotation(float val)
        {
            workingSettings.controlSetting_SensitivityRotation = val;
            settingsMenuElements.sensitivityRotation.UpdateText(string.Format("{0:0.##}", val));
        }

        /// <summary>
        /// Update zoom sensitivity on slider change
        /// </summary>
        /// <param name="val">New sensitivity value</param>
        public void OnValueChanged_SensitivityZoom(float val)
        {
            workingSettings.controlSetting_SensitivityZoom = val;
            settingsMenuElements.sensitivityZoom.UpdateText(string.Format("{0:0.##}", val));
        }

        /// <summary>
        /// Coroutine for animating transition Image
        /// </summary>
        /// <param name="isStart">Whether starting or ending animation</param>
        private IEnumerator AnimateTransition(bool isStart)
        {
            if (isStart)
            {
                transitionImage.gameObject.SetActive(true);
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Transition);
            }

            float t = 0;
            float length = GameConstants.LevelTransitionStartTime;
            float start = GameConstants.LevelTransitionMaxPower;
            float end = GameConstants.LevelTransitionMinPower;

            if (!isStart)
            {
                length = GameConstants.LevelTransitionEndTime;
                start = GameConstants.LevelTransitionMinPower;
                end = GameConstants.LevelTransitionMaxPower;
            }

            while (t <= length)
            {
                transitionImage.material.SetFloat("_Power", Mathf.Lerp(start, end, transitionCurve.Evaluate(t / length)));
                t += Time.deltaTime;
                yield return null;
            }

            if (!isStart)
                transitionImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Animation for setting the main menu canvas active or inactive
        /// </summary>
        /// <param name="active">Whether to fade the canvas in or out</param>
        private IEnumerator SetActive(bool active)
        {
            float t = 0;
            if (active)
            {
                canvas.gameObject.SetActive(true);
                foreach (GameObject g in canvasMainElements)
                    g.SetActive(true);
            }

            while (t <= GameConstants.UITransitionDuration)
            {
                if (active)
                    canvas.alpha = AnimationCurve.Linear(0, 0, 1, 1).Evaluate(t / GameConstants.UITransitionDuration);
                else
                    canvas.alpha = AnimationCurve.Linear(0, 1, 1, 0).Evaluate(t / GameConstants.UITransitionDuration);
                yield return null;
                t += Time.deltaTime;
            }

            if (active)
                canvas.alpha = 1;

            else
            {
                canvas.alpha = 0;
                canvas.gameObject.SetActive(false);
                foreach (GameObject g in canvasMainElements)
                    g.SetActive(false);
            }
        }

        /// <summary>
        /// The animation to play when the game first starts up
        /// </summary>
        private IEnumerator StartupAnimation()
        {
            yield return new WaitForFixedUpdate();
            float t = 0;
            while (t <= GameConstants.UIFirstLoadDuration)
            {
                startupFadeImage.color = new Color(
                    transitionStartupToWhite.Evaluate(t / GameConstants.UIFirstLoadDuration),
                    transitionStartupToWhite.Evaluate(t / GameConstants.UIFirstLoadDuration),
                    transitionStartupToWhite.Evaluate(t / GameConstants.UIFirstLoadDuration), 1);
                yield return null;
                t += Time.deltaTime;
            }
            t = 0;
            while (t <= GameConstants.UIFirstLoadDuration)
            {
                startupFadeImage.color = new Color(1, 1, 1, transitionStartupFade.Evaluate(t / GameConstants.UIFirstLoadDuration));
                yield return null;
                t += Time.deltaTime;
            }

            StartCoroutine(SetActive(true));
        }
        #endregion
    }
}
