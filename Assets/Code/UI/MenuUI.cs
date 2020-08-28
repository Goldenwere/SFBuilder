using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
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
        [SerializeField] private ControlsMenuImages             controlsMenuImages;
        [SerializeField] private SettingsMenuElements           settingsMenuElements;
        [SerializeField] private Image                          startupFadeImage;
        [SerializeField] private AnimationCurve                 transitionCurve;
        [SerializeField] private AnimationCurve                 transitionStartupFade;
        [SerializeField] private AnimationCurve                 transitionStartupToWhite;
        [SerializeField] private Image                          transitionImage;
#pragma warning restore 0649
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
        /// Collection of elements related to generic controls menu elements (there are two sets of these - one for keyboard, one for gamepad)
        /// </summary>
        [Serializable]
        protected class GenericControlsMenuElements
        {
            public Button   cameraMovementBackward;
            public Button   cameraMovementForward;
            public Button   cameraMovementLeft;
            public Button   cameraMovementRight;

            public Button   cameraRotateLeft;
            public Button   cameraRotateRight;
            public Button   cameraTiltUp;
            public Button   cameraTiltDown;

            public Button   cameraZoomIn;
            public Button   cameraZoomOut;

            public Button   gameplayCancelAndMenu;
            public Button   gameplayPlacement;
            public Button   gameplayUndo;
        }

        /// <summary>
        /// Collection of remaining controls menu elements (which are single-set)
        /// </summary>
        [Serializable]
        protected class OtherControlsMenuElements
        {
            public Button                   gamepadCursorDown;
            public Button                   gamepadCursorLeft;
            public Button                   gamepadCursorRight;
            public Button                   gamepadCursorUp;

            public Button                   gamepadToggleZoom;

            public Toggle                   modifiersHeld;

            public Toggle                   mouseInvertDeltaHoriztonal;
            public Toggle                   mouseInvertDeltaVertical;
            public Toggle                   mouseInvertZoom;

            public Button                   mouseToggleMovement;
            public Button                   mouseToggleRotation;
            public Button                   mouseToggleZoom;

            public SliderTextLoadExtension  sensitivityMovement;
            public SliderTextLoadExtension  sensitivityRotation;
            public SliderTextLoadExtension  sensitivityZoom;
        }

        /// <summary>
        /// Collection of elements on the settings menu, whose values are set every time the menu loads
        /// </summary>
        [Serializable]
        protected class SettingsMenuElements
        {
            public GenericControlsMenuElements  generalKeyboardControls;
            public GenericControlsMenuElements  generalGamepadControls;
            public OtherControlsMenuElements    otherControls;
            public Toggle                       postprocAO;
            public Toggle                       postprocBloom;
            public Toggle                       postprocSSR;
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
            workingSettings = GameSettings.Instance.Settings;

            canvas.gameObject.SetActive(false);

            foreach (GameObject g in canvasSettingsElements)
                g.SetActive(false);
            // these need disabled first before re-enabling to ensure proper transition order
            foreach (GameObject g in canvasMainElements)
                g.SetActive(false);

            // Clear elements that were initially added that are now disabled
            UITransitionSystem.Instance.ClearElements();

            InitializeButtons();
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
        /// Attaches handlers to buttons
        /// </summary>
        private void InitializeButtons()
        {
            settingsMenuElements.generalGamepadControls.cameraMovementBackward.onClick.AddListener( () => OnSetControl(GenericControl.Camera_MoveBackward,    true));
            settingsMenuElements.generalGamepadControls.cameraMovementForward.onClick.AddListener(  () => OnSetControl(GenericControl.Camera_MoveForward,     true));
            settingsMenuElements.generalGamepadControls.cameraMovementLeft.onClick.AddListener(     () => OnSetControl(GenericControl.Camera_MoveLeft,        true));
            settingsMenuElements.generalGamepadControls.cameraMovementRight.onClick.AddListener(    () => OnSetControl(GenericControl.Camera_MoveRight,       true));
            settingsMenuElements.generalGamepadControls.cameraRotateLeft.onClick.AddListener(       () => OnSetControl(GenericControl.Camera_RotateLeft,      true));
            settingsMenuElements.generalGamepadControls.cameraRotateRight.onClick.AddListener(      () => OnSetControl(GenericControl.Camera_RotateRight,     true));
            settingsMenuElements.generalGamepadControls.cameraTiltDown.onClick.AddListener(         () => OnSetControl(GenericControl.Camera_TiltDown,        true));
            settingsMenuElements.generalGamepadControls.cameraTiltUp.onClick.AddListener(           () => OnSetControl(GenericControl.Camera_TiltUp,          true));
            settingsMenuElements.generalGamepadControls.cameraZoomIn.onClick.AddListener(           () => OnSetControl(GenericControl.Camera_ZoomIn,          true));
            settingsMenuElements.generalGamepadControls.cameraZoomOut.onClick.AddListener(          () => OnSetControl(GenericControl.Camera_ZoomOut,         true));
            settingsMenuElements.generalGamepadControls.gameplayCancelAndMenu.onClick.AddListener(  () => OnSetControl(GenericControl.Gameplay_CancelAndMenu, true));
            settingsMenuElements.generalGamepadControls.gameplayPlacement.onClick.AddListener(      () => OnSetControl(GenericControl.Gameplay_Placement,     true));
            settingsMenuElements.generalGamepadControls.gameplayUndo.onClick.AddListener(           () => OnSetControl(GenericControl.Gameplay_Undo,          true));

            settingsMenuElements.generalKeyboardControls.cameraMovementBackward.onClick.AddListener(() => OnSetControl(GenericControl.Camera_MoveBackward,    false));
            settingsMenuElements.generalKeyboardControls.cameraMovementForward.onClick.AddListener( () => OnSetControl(GenericControl.Camera_MoveForward,     false));
            settingsMenuElements.generalKeyboardControls.cameraMovementLeft.onClick.AddListener(    () => OnSetControl(GenericControl.Camera_MoveLeft,        false));
            settingsMenuElements.generalKeyboardControls.cameraMovementRight.onClick.AddListener(   () => OnSetControl(GenericControl.Camera_MoveRight,       false));
            settingsMenuElements.generalKeyboardControls.cameraRotateLeft.onClick.AddListener(      () => OnSetControl(GenericControl.Camera_RotateLeft,      false));
            settingsMenuElements.generalKeyboardControls.cameraRotateRight.onClick.AddListener(     () => OnSetControl(GenericControl.Camera_RotateRight,     false));
            settingsMenuElements.generalKeyboardControls.cameraTiltDown.onClick.AddListener(        () => OnSetControl(GenericControl.Camera_TiltDown,        false));
            settingsMenuElements.generalKeyboardControls.cameraTiltUp.onClick.AddListener(          () => OnSetControl(GenericControl.Camera_TiltUp,          false));
            settingsMenuElements.generalKeyboardControls.cameraZoomIn.onClick.AddListener(          () => OnSetControl(GenericControl.Camera_ZoomIn,          false));
            settingsMenuElements.generalKeyboardControls.cameraZoomOut.onClick.AddListener(         () => OnSetControl(GenericControl.Camera_ZoomOut,         false));
            settingsMenuElements.generalKeyboardControls.gameplayCancelAndMenu.onClick.AddListener( () => OnSetControl(GenericControl.Gameplay_CancelAndMenu, false));
            settingsMenuElements.generalKeyboardControls.gameplayPlacement.onClick.AddListener(     () => OnSetControl(GenericControl.Gameplay_Placement,     false));
            settingsMenuElements.generalKeyboardControls.gameplayUndo.onClick.AddListener(          () => OnSetControl(GenericControl.Gameplay_Undo,          false));

            settingsMenuElements.otherControls.gamepadCursorDown.onClick.AddListener(   () => OnSetControl(OtherControl.Gamepad_CursorDown));
            settingsMenuElements.otherControls.gamepadCursorLeft.onClick.AddListener(   () => OnSetControl(OtherControl.Gamepad_CursorLeft));
            settingsMenuElements.otherControls.gamepadCursorRight.onClick.AddListener(  () => OnSetControl(OtherControl.Gamepad_CursorRight));
            settingsMenuElements.otherControls.gamepadCursorUp.onClick.AddListener(     () => OnSetControl(OtherControl.Gamepad_CursorUp));
            settingsMenuElements.otherControls.gamepadToggleZoom.onClick.AddListener(   () => OnSetControl(OtherControl.Gamepad_ZoomToggle));
            settingsMenuElements.otherControls.mouseToggleMovement.onClick.AddListener( () => OnSetControl(OtherControl.Mouse_ToggleMovement));
            settingsMenuElements.otherControls.mouseToggleRotation.onClick.AddListener( () => OnSetControl(OtherControl.Mouse_ToggleRotation));
            settingsMenuElements.otherControls.mouseToggleZoom.onClick.AddListener(     () => OnSetControl(OtherControl.Mouse_ToggleZoom));
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

            if (currState == GameState.MainMenus)
                StartCoroutine(SetActive(true));
            else
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
        private void SetControl(Button element, string path)
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
            foreach (GameObject g in canvasMainElements)
                g.SetActive(true);
            foreach (GameObject g in canvasSettingsElements)
                g.SetActive(false);
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
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
        /// Update a control when a control button is pressed
        /// </summary>
        /// <param name="control">The control associated with the button</param>
        /// <param name="isGamepad">Whether the button is associated with the gamepad or keyboard</param>
        public void OnSetControl(GenericControl control, bool isGamepad)
        {
            
        }

        /// <summary>
        /// Update a control when a control button is pressed
        /// </summary>
        /// <param name="control">The control associated with the button</param>
        public void OnSetControl(OtherControl control)
        {

        }

        /// <summary>
        /// When the settings menu button is pressed, load the settings menu
        /// </summary>
        public void OnSettingsPressed()
        {
            workingSettings = GameSettings.Instance.Settings;
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
            workingSettings = GameSettings.Instance.Settings;
            LoadSettings();
        }

        /// <summary>
        /// When the save button is pressed on the settings menu, save settings
        /// </summary>
        public void OnSettingsSavePressed()
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            GameSettings.Instance.Settings = workingSettings;
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
