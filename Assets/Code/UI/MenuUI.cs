using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Goldenwere.Unity.UI;
using Goldenwere.Unity;

namespace SFBuilder.UI
{
    /// <summary>
    /// Defines the submenus available in the settings menu
    /// </summary>
    public enum SettingsSubmenu
    {
        accessibility,
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
        [SerializeField] private RectTransform                  canvasSettingsSubmenuContainer;
        [SerializeField] private GameObject                     canvasWarningWindow;
        [SerializeField] private ControlsMenuImages             controlsMenuImages;
        [SerializeField] private OtherElements                  otherElements;
        [SerializeField] private SettingsMenuElements           settingsMenuElements;
        [SerializeField] private Image                          startupFadeImage;
        [SerializeField] private AnimationCurve                 transitionCurve;
        [SerializeField] private AnimationCurve                 transitionStartupFade;
        [SerializeField] private AnimationCurve                 transitionStartupToWhite;
        [SerializeField] private Image                          transitionImage;
#pragma warning restore 0649
        /**************/ private bool                           pendingChangesExist;
        /**************/ private GameObject                     previouslySelectedElement;
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
        /// Collection of misc elements (some need manual navigation updated or need to be set as the selected element when menu state changes)
        /// </summary>
        [Serializable]
        protected class OtherElements
        {
            public Button       mainMenuOptionPlay;
            public Button       mainMenuOptionQuit;
            public Button       mainMenuOptionSettings;
            public ScrollRect   scrollRectSettingsMenu;
            public Button       settingsMenuOptionMenu;
            public Button       settingsMenuOptionRevert;
            public Button       settingsMenuOptionSave;
            public Button       settingsSubOptionAccessibility;
            public Button       settingsSubOptionAudio;
            public Button       settingsSubOptionControls;
            public Button       settingsSubOptionGraphics;
            public GameObject   submenuAccessibility;
            public GameObject   submenuAccessibilityButtonBackground;
            public GameObject   submenuAudio;
            public GameObject   submenuAudioButtonBackground;
            public GameObject   submenuControls;
            public GameObject   submenuControlsButtonBackground;
            public GameObject   submenuGraphics;
            public GameObject   submenuGraphicsButtonBackground;
            public Button       windowSettingsBack;
            public Button       windowSettingsSave;
            public Button       windowSettingsRevert;
        }

        /// <summary>
        /// Collection of elements on the settings menu, whose values are set every time the menu loads
        /// </summary>
        [Serializable]
        protected class SettingsMenuElements
        {
            [Header("Accessibility")]
            public Toggle                       accessibilityCameraShake;
            public Toggle                       accessibilityCameraSmoothing;
            public TMP_Dropdown                 accessibilityFontSize;
            public TMP_Dropdown                 accessibilityFontStyle;
            [Space]
            public ControlButton[]              controlButtons;
            [Space]
            [Header("Display")]
            public Toggle                       displayAnim;
            public TMP_Dropdown                 displayCursor;
            public SliderTextLoadExtension      displayFOV;
            public SliderTextLoadExtension      displayFramerate;
            public TMP_Dropdown                 displayRatio;
            public TMP_Dropdown                 displayResolution;
            public Toggle                       displayVsync;
            public TMP_Dropdown                 displayWindow;
            [Header("Post Processing")]
            public Toggle                       postprocAO;
            public Toggle                       postprocBloom;
            public Toggle                       postprocSSR;
            [Header("Other Controls")]
            public Toggle                       invertHorizontal;
            public Toggle                       invertScroll;
            public Toggle                       invertVertical;
            public Toggle                       modifiersHeld;
            public SliderTextLoadExtension      sensitivityMovement;
            public SliderTextLoadExtension      sensitivityRotation;
            public SliderTextLoadExtension      sensitivityZoom;
            [Header("Audio")]
            public SliderTextLoadExtension      volMusic;
            public SliderTextLoadExtension      volEffects;
        }
        #endregion

        #region Methods
        #region Unity methods
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

            InitializeUIElements();

            StartCoroutine(StartupAnimation());
            LoadSubmenu(SettingsSubmenu.graphics);

            StartCoroutine(WaitUntilSelectableIsActive(otherElements.mainMenuOptionPlay));
            previouslySelectedElement = otherElements.mainMenuOptionPlay.gameObject;
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
            GameEventSystem.LevelTransitioned += OnLevelTransitioned;
            UnityEventSystemExtension.SelectedGameObjectChanged += OnSelectedGameObjectChanged;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
            GameEventSystem.LevelTransitioned -= OnLevelTransitioned;
            UnityEventSystemExtension.SelectedGameObjectChanged -= OnSelectedGameObjectChanged;
        }

        /// <summary>
        /// If one hits enter on a slider, it looses focus, because Unity(TM)
        /// </summary>
        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null &&
                GameEventSystem.Instance.CurrentGameState == GameState.MainMenus &&
                !Mouse.current.leftButton.isPressed)
            {
                if (previouslySelectedElement.TryGetComponent(out Selectable s))
                    StartCoroutine(WaitUntilSelectableIsActive(s));
            }
        }
        #endregion

        #region Settings menu related methods
        /// <summary>
        /// Sets up resolution/ratio dropdowns based on underlying settings
        /// </summary>
        private void HandleResolutionElements()
        {
            if ((byte)workingSettings.display_Resolution == 255)
            {
                settingsMenuElements.displayRatio.interactable = false;
                // Default to 16:9 for the other options in the list
                List<string> resOptions = GameConstants.ResolutionEnumToString(0, 32);
                List<TMP_Dropdown.OptionData> newOptions = resOptions.Select(x => new TMP_Dropdown.OptionData(x.Replace("_", ""))).ToList();
                newOptions.Add(new TMP_Dropdown.OptionData("Native"));
                settingsMenuElements.displayResolution.options = newOptions;
                settingsMenuElements.displayResolution.SetValueWithoutNotify(settingsMenuElements.displayResolution.options.Count - 1);
            }

            else
            {
                settingsMenuElements.displayRatio.interactable = true;
                HandleResolutionElements((byte)workingSettings.display_Resolution / 32);
            }
        }

        /// <summary>
        /// Sets up resolution/ratio dropdowns based on selected ratio
        /// </summary>
        /// <param name="selectedRatio">The ratio setting</param>
        private void HandleResolutionElements(int selectedRatio)
        {
            int startRange = 32 * selectedRatio;
            List<string> resOptions = GameConstants.ResolutionEnumToString(startRange, 32);
            settingsMenuElements.displayRatio.SetValueWithoutNotify(selectedRatio);

            List<TMP_Dropdown.OptionData> newOptions = resOptions.Select(x => new TMP_Dropdown.OptionData(x.Replace("_", ""))).ToList();
            newOptions.Add(new TMP_Dropdown.OptionData("Native"));
            settingsMenuElements.displayResolution.options = newOptions;
            settingsMenuElements.displayResolution.SetValueWithoutNotify((byte)workingSettings.display_Resolution - startRange);
        }

        /// <summary>
        /// Adds event listeners to the settings menu options
        /// </summary>
        private void InitializeUIElements()
        {
            // Initialize the controls menu buttons
            foreach (ControlButton cb in settingsMenuElements.controlButtons)
                cb.onClick.AddListener(() => OnSetControl(cb));

            #region Initialize remaining controls elements
            settingsMenuElements.sensitivityMovement.AssociatedSlider.onValueChanged.AddListener(val => {
                OnValueChanged(false);
                workingSettings.controlSetting_SensitivityMovement = val;
                settingsMenuElements.sensitivityMovement.UpdateText(string.Format("{0:0.##}", val));
            });

            settingsMenuElements.sensitivityRotation.AssociatedSlider.onValueChanged.AddListener(val => {
                OnValueChanged(false);
                workingSettings.controlSetting_SensitivityRotation = val;
                settingsMenuElements.sensitivityRotation.UpdateText(string.Format("{0:0.##}", val));
            });

            settingsMenuElements.sensitivityZoom.AssociatedSlider.onValueChanged.AddListener(val => {
                OnValueChanged(false);
                workingSettings.controlSetting_SensitivityZoom = val;
                settingsMenuElements.sensitivityZoom.UpdateText(string.Format("{0:0.##}", val));
            });

            settingsMenuElements.modifiersHeld.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.modifiersHeld);
                workingSettings.controlSetting_HoldModifiers = val;
            });

            settingsMenuElements.invertHorizontal.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.invertHorizontal);
                workingSettings.controlSetting_InvertHorizontal = val;
            });

            settingsMenuElements.invertVertical.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.invertVertical);
                workingSettings.controlSetting_InvertVertical = val;
            });

            settingsMenuElements.invertScroll.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.invertScroll);
                workingSettings.controlSetting_InvertScroll = val;
            });
            #endregion

            #region Initialize accessibility elements
            settingsMenuElements.accessibilityCameraShake.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.accessibilityCameraShake);
                workingSettings.accessibility_CameraShake = val;
            });

            settingsMenuElements.accessibilityCameraSmoothing.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.accessibilityCameraSmoothing);
                workingSettings.accessibility_CameraSmoothing = val;
            });

            settingsMenuElements.accessibilityFontSize.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.accessibilityFontSize);
                workingSettings.accessibility_FontSize = (FontSize)val;
            });

            settingsMenuElements.accessibilityFontStyle.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.accessibilityFontStyle);
                workingSettings.accessibility_FontStyle = (FontStyle)val;
            });
            #endregion

            #region Initialize display elements
            settingsMenuElements.displayAnim.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.displayAnim);
                workingSettings.other_ObjectAnimations = val;
            });

            settingsMenuElements.displayCursor.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.displayCursor);
                workingSettings.display_Cursor = (CursorSize)val;
            });

            settingsMenuElements.displayFOV.AssociatedSlider.onValueChanged.AddListener(val => {
                OnValueChanged(false);
                workingSettings.display_FOV = (int)val;
                settingsMenuElements.displayFOV.UpdateText(string.Format("{0:###}", val));
            });

            settingsMenuElements.displayFramerate.AssociatedSlider.onValueChanged.AddListener(val => {
                OnValueChanged(false);
                workingSettings.display_Framerate = (int)val * 10;
                settingsMenuElements.displayFramerate.UpdateText(string.Format("{0:###}", val * 10));
            });

            settingsMenuElements.displayRatio.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.displayRatio);
                HandleResolutionElements(val);
            });

            settingsMenuElements.displayResolution.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.displayResolution);

                if (val == settingsMenuElements.displayResolution.options.Count - 1)
                {
                    workingSettings.display_Resolution = ResolutionSetting._native;
                    HandleResolutionElements();
                }

                else
                    workingSettings.display_Resolution = (ResolutionSetting)(settingsMenuElements.displayRatio.value * val);
            });

            settingsMenuElements.displayVsync.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.displayVsync);
                workingSettings.display_Vsync = val;
            });

            settingsMenuElements.displayWindow.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.displayWindow);
                workingSettings.display_Window = (WindowMode)val;
            });

            settingsMenuElements.postprocAO.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.postprocAO);
                workingSettings.postprocAO = val;
            });

            settingsMenuElements.postprocBloom.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.postprocBloom);
                workingSettings.postprocBloom = val;
            });

            settingsMenuElements.postprocSSR.onValueChanged.AddListener(val => {
                OnValueChanged(settingsMenuElements.postprocSSR);
                workingSettings.postprocSSR = val;
            });
            #endregion

            #region Initilize audio elements
            settingsMenuElements.volEffects.AssociatedSlider.onValueChanged.AddListener(val => {
                OnValueChanged(false);
                workingSettings.volEffects = val;
                settingsMenuElements.volEffects.UpdateText(string.Format("{0:P0}", val));
            });

            settingsMenuElements.volMusic.AssociatedSlider.onValueChanged.AddListener(val => {
                OnValueChanged(false);
                workingSettings.volMusic = val;
                settingsMenuElements.volMusic.UpdateText(string.Format("{0:P0}", val));
            });
            #endregion

            #region Initialize main menu buttons
            otherElements.mainMenuOptionPlay.onClick.AddListener(() => {
                StartCoroutine(SetActive(false));
                GameEventSystem.Instance.UpdateGameState(GameState.Gameplay);
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Goal);
            });

            otherElements.mainMenuOptionSettings.onClick.AddListener(() => {
                workingSettings = SettingsData.Copy(GameSettings.Instance.Settings);
                LoadSettings();
                foreach (GameObject g in canvasMainElements)
                    g.SetActive(false);
                foreach (GameObject g in canvasSettingsElements)
                    g.SetActive(true);
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                StartCoroutine(WaitUntilSelectableIsActive(otherElements.settingsMenuOptionMenu));
            });

            otherElements.mainMenuOptionQuit.onClick.AddListener(() => {
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                Application.Quit();
            });
            #endregion

            #region Initialize settings menu/submenu/window buttons
            otherElements.settingsMenuOptionMenu.onClick.AddListener(() => SettingsToMain());
            otherElements.settingsMenuOptionSave.onClick.AddListener(() => SettingsSave());
            otherElements.settingsMenuOptionRevert.onClick.AddListener(() => SettingsRevert());

            otherElements.settingsSubOptionAccessibility.onClick.AddListener(() => {
                if (workingSettingsSubmenuState != SettingsSubmenu.accessibility)
                {
                    LoadSubmenu(SettingsSubmenu.accessibility);
                    GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                }
                StartCoroutine(WaitUntilSelectableIsActive(settingsMenuElements.accessibilityCameraShake));
            });

            otherElements.settingsSubOptionAudio.onClick.AddListener(() => {
                if (workingSettingsSubmenuState != SettingsSubmenu.audio)
                {
                    LoadSubmenu(SettingsSubmenu.audio);
                    GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                }
                StartCoroutine(WaitUntilSelectableIsActive(settingsMenuElements.volMusic.AssociatedSlider));
            });

            otherElements.settingsSubOptionControls.onClick.AddListener(() => {
                if (workingSettingsSubmenuState != SettingsSubmenu.controls)
                {
                    LoadSubmenu(SettingsSubmenu.controls);
                    GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                }
                StartCoroutine(WaitUntilSelectableIsActive(settingsMenuElements.controlButtons[0]));
            });

            otherElements.settingsSubOptionGraphics.onClick.AddListener(() => {
                if (workingSettingsSubmenuState != SettingsSubmenu.graphics)
                {
                    LoadSubmenu(SettingsSubmenu.graphics);
                    GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                }
                StartCoroutine(WaitUntilSelectableIsActive(settingsMenuElements.postprocAO));
            });

            otherElements.windowSettingsBack.onClick.AddListener(() => {
                canvasWarningWindow.SetActive(false);
                StartCoroutine(WaitUntilSelectableIsActive(otherElements.settingsMenuOptionMenu));
            });

            otherElements.windowSettingsSave.onClick.AddListener(() => {
                SettingsSave();
                SettingsToMain();
            });

            otherElements.windowSettingsRevert.onClick.AddListener(() => {
                SettingsRevert();
                SettingsToMain();
            });
            #endregion
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
                if ((byte)cb.AssociatedControl < 17 || (byte)cb.AssociatedControl > 20)
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

            HandleResolutionElements();
            settingsMenuElements.displayAnim.SetIsOnWithoutNotify(workingSettings.other_ObjectAnimations);
            settingsMenuElements.displayCursor.SetValueWithoutNotify((int)workingSettings.display_Cursor);
            settingsMenuElements.displayFOV.AssociatedSlider.SetValueWithoutNotify(workingSettings.display_FOV);
            settingsMenuElements.displayFOV.UpdateText(workingSettings.display_FOV);
            settingsMenuElements.displayFramerate.AssociatedSlider.SetValueWithoutNotify(workingSettings.display_Framerate / 10);
            settingsMenuElements.displayFramerate.UpdateText(workingSettings.display_Framerate);
            settingsMenuElements.displayVsync.SetIsOnWithoutNotify(workingSettings.display_Vsync);
            settingsMenuElements.displayWindow.SetValueWithoutNotify((int)workingSettings.display_Window);
            settingsMenuElements.accessibilityCameraShake.SetIsOnWithoutNotify(workingSettings.accessibility_CameraShake);
            settingsMenuElements.accessibilityCameraSmoothing.SetIsOnWithoutNotify(workingSettings.accessibility_CameraSmoothing);
            settingsMenuElements.accessibilityFontSize.SetValueWithoutNotify((int)workingSettings.accessibility_FontSize);
            settingsMenuElements.accessibilityFontStyle.SetValueWithoutNotify((int)workingSettings.accessibility_FontStyle);
        }

        /// <summary>
        /// Loads appropriate settings submenus based on the new submenu state
        /// </summary>
        /// <param name="newState">The submenu to load</param>
        private void LoadSubmenu(SettingsSubmenu newState)
        {
            Navigation accessNav = otherElements.settingsSubOptionAccessibility.navigation;
            Navigation audioNav = otherElements.settingsSubOptionAudio.navigation;
            Navigation controlsNav = otherElements.settingsSubOptionControls.navigation;
            Navigation graphicsNav = otherElements.settingsSubOptionGraphics.navigation;
            GameObject active;
            switch (newState)
            {
                case SettingsSubmenu.accessibility:
                    otherElements.submenuAccessibility.SetActive(true);
                    otherElements.submenuAudio.SetActive(false);
                    otherElements.submenuControls.SetActive(false);
                    otherElements.submenuGraphics.SetActive(false);
                    otherElements.submenuAccessibilityButtonBackground.SetActive(true);
                    otherElements.submenuAudioButtonBackground.SetActive(false);
                    otherElements.submenuControlsButtonBackground.SetActive(false);
                    otherElements.submenuGraphicsButtonBackground.SetActive(true);
                    active = otherElements.submenuAccessibility;
                    accessNav.selectOnDown = settingsMenuElements.accessibilityCameraShake;
                    audioNav.selectOnDown = settingsMenuElements.accessibilityCameraShake;
                    controlsNav.selectOnDown = settingsMenuElements.accessibilityCameraShake;
                    graphicsNav.selectOnDown = settingsMenuElements.accessibilityCameraShake;
                    break;
                case SettingsSubmenu.audio:
                    otherElements.submenuAccessibility.SetActive(false);
                    otherElements.submenuAudio.SetActive(true);
                    otherElements.submenuControls.SetActive(false);
                    otherElements.submenuGraphics.SetActive(false);
                    otherElements.submenuAccessibilityButtonBackground.SetActive(false);
                    otherElements.submenuAudioButtonBackground.SetActive(true);
                    otherElements.submenuControlsButtonBackground.SetActive(false);
                    otherElements.submenuGraphicsButtonBackground.SetActive(false);
                    active = otherElements.submenuAudio;
                    accessNav.selectOnDown = settingsMenuElements.volMusic.AssociatedSlider;
                    audioNav.selectOnDown = settingsMenuElements.volMusic.AssociatedSlider;
                    controlsNav.selectOnDown = settingsMenuElements.volMusic.AssociatedSlider;
                    graphicsNav.selectOnDown = settingsMenuElements.volMusic.AssociatedSlider;
                    break;
                case SettingsSubmenu.controls:
                    otherElements.submenuAccessibility.SetActive(false);
                    otherElements.submenuAudio.SetActive(false);
                    otherElements.submenuControls.SetActive(true);
                    otherElements.submenuGraphics.SetActive(false);
                    otherElements.submenuAccessibilityButtonBackground.SetActive(false);
                    otherElements.submenuAudioButtonBackground.SetActive(false);
                    otherElements.submenuControlsButtonBackground.SetActive(true);
                    otherElements.submenuGraphicsButtonBackground.SetActive(false);
                    active = otherElements.submenuControls;
                    accessNav.selectOnDown = settingsMenuElements.controlButtons[0];
                    audioNav.selectOnDown = settingsMenuElements.controlButtons[0];
                    controlsNav.selectOnDown = settingsMenuElements.controlButtons[0];
                    graphicsNav.selectOnDown = settingsMenuElements.controlButtons[0];
                    break;
                case SettingsSubmenu.graphics:
                default:
                    otherElements.submenuAccessibility.SetActive(false);
                    otherElements.submenuAudio.SetActive(false);
                    otherElements.submenuControls.SetActive(false);
                    otherElements.submenuGraphics.SetActive(true);
                    otherElements.submenuAccessibilityButtonBackground.SetActive(false);
                    otherElements.submenuAudioButtonBackground.SetActive(false);
                    otherElements.submenuControlsButtonBackground.SetActive(false);
                    otherElements.submenuGraphicsButtonBackground.SetActive(true);
                    active = otherElements.submenuGraphics;
                    accessNav.selectOnDown = settingsMenuElements.displayCursor;
                    audioNav.selectOnDown = settingsMenuElements.displayCursor;
                    controlsNav.selectOnDown = settingsMenuElements.displayCursor;
                    graphicsNav.selectOnDown = settingsMenuElements.displayCursor;
                    break;
            }
            canvasSettingsSubmenuContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, active.GetComponent<RectTransform>().sizeDelta.y);
            workingSettingsSubmenuState = newState;
            otherElements.settingsSubOptionAccessibility.navigation = accessNav;
            otherElements.settingsSubOptionAudio.navigation = audioNav;
            otherElements.settingsSubOptionControls.navigation = controlsNav;
            otherElements.settingsSubOptionGraphics.navigation = graphicsNav;
        }

        /// <summary>
        /// Condenses repetition of element listeners with playing sound, re-attaching elements to UI nav, and setting pending changes
        /// </summary>
        /// <param name="playAudio">Whether to play audio (defaults to true)</param>
        /// <param name="selectable">Selectable that needs re-attached to navigable UI (optional, e.g. sliders don't need this)</param>
        private void OnValueChanged(bool playAudio = true, Selectable selectable = null)
        {
            if (playAudio)
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            pendingChangesExist = true;
            if (selectable != null)
                StartCoroutine(WaitUntilSelectableIsActive(selectable));
        }
        #endregion

        #region GameEventSystem handlers
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
        /// Handler for changed selected game object in EventSystem for use in autoscrolling controls menu
        /// </summary>
        /// <param name="prev">The previously selected gameobject</param>
        /// <param name="curr">The currently selected gameobject</param>
        private void OnSelectedGameObjectChanged(GameObject prev, GameObject curr)
        {
            if (curr != null)
            {
                previouslySelectedElement = curr;

                if (curr.CompareTag("scrollable"))
                {
                    Canvas.ForceUpdateCanvases();
                    RectTransform rt = otherElements.submenuControls.transform.parent.GetComponent<RectTransform>();
                    Vector2 newPos = rt.anchoredPosition;
                    Vector2 sub = (Vector2)otherElements.scrollRectSettingsMenu.transform.InverseTransformPoint(rt.position)
                        - (Vector2)otherElements.scrollRectSettingsMenu.transform.InverseTransformPoint(curr.transform.position);
                    newPos.y = sub.y - 50;
                    if (newPos.y < 0)
                        newPos.y = 0;
                    rt.anchoredPosition = newPos;
                }
            }
        }
        #endregion

        #region Controls-related methods
        /// <summary>
        /// Handler for control buttons to set controls
        /// </summary>
        /// <param name="sender">The button sending the event</param>
        private void OnSetControl(ControlButton sender)
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
                        case GameControl.Gamepad_CursorDown:
                        case GameControl.Gamepad_CursorLeft:
                        case GameControl.Gamepad_CursorRight:
                        case GameControl.Gamepad_CursorUp:
                        case GameControl.UI_Click:
                        case GameControl.UI_NavDown:
                        case GameControl.UI_NavLeft:
                        case GameControl.UI_NavRight:
                        case GameControl.UI_NavUp:
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
                    StartCoroutine(WaitUntilSelectableIsActive(sender));
                    rebindOp?.Dispose();
                });
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
                        switch (pathSplit[1])
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
        #endregion

        #region Main settings buttons
        /// <summary>
        /// Reverts pending changes in settings menu to saved GameSettings
        /// </summary>
        private void SettingsRevert()
        {
            pendingChangesExist = false;
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            workingSettings = SettingsData.Copy(GameSettings.Instance.Settings);
            LoadSettings();
            GameSettings.Instance.SetInputOverrides();
        }

        /// <summary>
        /// Saves pending settings changes to GameSettings
        /// </summary>
        private void SettingsSave()
        {
            pendingChangesExist = false;
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            GameSettings.Instance.Settings = SettingsData.Copy(workingSettings);
        }

        /// <summary>
        /// Goes back to the main menu from settings (unless there are pending changes, in which a warning pops up)
        /// </summary>
        private void SettingsToMain()
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);

            if (pendingChangesExist)
            {
                canvasWarningWindow.SetActive(true);
                StartCoroutine(WaitUntilSelectableIsActive(otherElements.windowSettingsBack));
            }

            else
            {
                foreach (GameObject g in canvasMainElements)
                    g.SetActive(true);
                foreach (GameObject g in canvasSettingsElements)
                    g.SetActive(false);
                canvasWarningWindow.SetActive(false);
                StartCoroutine(WaitUntilSelectableIsActive(otherElements.mainMenuOptionPlay));
            }
        }
        #endregion

        #region Coroutines
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

        /// <summary>
        /// Due to canvas animations, selectable may not be active for a brief period; wait until active to select it
        /// </summary>
        /// <param name="selectable">The selectable to select for UI navigation purposes</param>
        private IEnumerator WaitUntilSelectableIsActive(Selectable selectable)
        {
            yield return new WaitForEndOfFrame();
            while (!selectable.isActiveAndEnabled)
                yield return null;

            selectable.Select();
            selectable.OnSelect(null);
        }
        #endregion
        #endregion
    }
}
