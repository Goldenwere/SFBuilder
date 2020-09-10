using UnityEngine;
using UnityEngine.UI;
using SFBuilder.Gameplay;
using System.Linq;
using System.Collections;
using Goldenwere.Unity.UI;

namespace SFBuilder.UI
{
    /// <summary>
    /// Manages UI while in-game
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private AnimationCurve animationCurveForTransitions;
        [SerializeField] private Button         buttonBanishment;
        [SerializeField] private Button         buttonNextGoal;
        [SerializeField] private Button         buttonNextLevel;
        [SerializeField] private Camera         gameCamera;
        [SerializeField] private TypeToIcon[]   icons;
        [SerializeField] private CanvasGroup    mainCanvasGroup;
        [SerializeField] private GameObject[]   mainCanvasTransitionedElements;
        [SerializeField] private GameObject     templateExtraButton;
        [SerializeField] private GameObject     templateRequirementButton;
        [SerializeField] private GameObject     panelOtherButtons;
        [SerializeField] private GameObject     panelPlacement;
        [SerializeField] private int            panelPlacementButtonPadding;
        [SerializeField] private ColorPalette   uiPalette;
        [SerializeField] private GameObject     windowBanishment;
        [SerializeField] private GameObject     windowNextLevel;
#pragma warning restore 0649
        /**************/ private RectTransform  panelOtherButtonsRT;
        /**************/ private RectTransform  panelPlacementRT;
        /**************/ private Transform      panelPlacementButtons;
        /**************/ private RectTransform  windowBanishmentRT;
        /**************/ private RectTransform  windowNextLevelRT;
        /**************/ private bool           windowOpenedBanishment;
        /**************/ private bool           windowOpenedNextLevel;
        /**************/ private bool           workingPlacementState;
        #endregion
        #region Properties
        public static GameUI     Instance       { get; private set; }
        #endregion
        #region Methods
        /// <summary>
        /// Toggle the main canvas on Awake and set singleton instance
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            ColorEnabledElement[] colorElems = GetComponentsInChildren<ColorEnabledElement>();
            foreach (ColorEnabledElement elem in colorElems)
                elem.SetupColors(uiPalette);
            TooltipEnabledElement[] tooltips = GetComponentsInChildren<TooltipEnabledElement>();
            foreach (TooltipEnabledElement tooltip in tooltips)
                tooltip.SetColors(uiPalette.BackgroundAccentColor, uiPalette.ForegroundAccentColor);

            if (GameEventSystem.Instance.CurrentGameState != GameState.Gameplay)
                SetCanvasActive(false);
            else
                SetCanvasActive(true);

            panelOtherButtonsRT = panelOtherButtons.GetComponent<RectTransform>();
            windowBanishmentRT = windowBanishment.GetComponent<RectTransform>();
            windowBanishmentRT.anchoredPosition = new Vector2(0, -100);
            windowNextLevelRT = windowNextLevel.GetComponent<RectTransform>();
            windowNextLevelRT.anchoredPosition = new Vector2(0, -100);
            panelPlacementRT = panelPlacement.GetComponent<RectTransform>();
            panelPlacementButtons = panelPlacement.transform.GetChild(0);
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
            GameEventSystem.PlacementStateChanged += OnPlacementStateChanged;
            GameEventSystem.GoalMet += OnGoalMet;
            GameEventSystem.PlacementPanelUpdateDesired += OnPlacementPanelUpdateDesired;
            GameEventSystem.NextLevelReadyStateChanged += OnNextLevelReadyStateChanged;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
            GameEventSystem.PlacementStateChanged -= OnPlacementStateChanged;
            GameEventSystem.GoalMet -= OnGoalMet;
            GameEventSystem.PlacementPanelUpdateDesired -= OnPlacementPanelUpdateDesired;
            GameEventSystem.NextLevelReadyStateChanged -= OnNextLevelReadyStateChanged;
        }

        /// <summary>
        /// Used for clearing out the placement panel
        /// </summary>
        private void ClearPlacementPanel()
        {
            for (int i = 0, count = panelPlacementButtons.childCount; i < count; i++)
                Destroy(panelPlacementButtons.GetChild(i).gameObject);
        }

        /// <summary>
        /// Handler for the GameStateChanged event
        /// </summary>
        /// <param name="prevState">The previous GameState</param>
        /// <param name="newState">The new GameState</param>
        private void OnGameStateChanged(GameState prevState, GameState newState)
        {
            if (newState != GameState.Gameplay)
            {
                SetCanvasActive(false);
                if (windowOpenedBanishment)
                    StartCoroutine(TransitionWindow(false, windowBanishmentRT));
                if (windowOpenedNextLevel)
                    StartCoroutine(TransitionWindow(false, windowNextLevelRT));
            }
            else
                SetCanvasActive(true);
        }

        /// <summary>
        /// Handler for the GoalMet event
        /// </summary>
        /// <param name="isMet">Whether the current goal was met</param>
        private void OnGoalMet(bool isMet)
        {
            if (!buttonNextGoal.interactable && isMet)
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Goal, 0.5f);
            buttonNextGoal.interactable = isMet;
        }

        /// <summary>
        /// Handler for the NextLevelReadyStateChanged event
        /// </summary>
        /// <param name="isReady">Whether the next level is ready or not</param>
        private void OnNextLevelReadyStateChanged(bool isReady)
        {
            buttonNextLevel.gameObject.SetActive(isReady);
        }

        /// <summary>
        /// Handler for the PlacementPanelUpdateDesired event
        /// </summary>
        /// <param name="clearOnly">Whether to only clear the panel or to fully set it up</param>
        private void OnPlacementPanelUpdateDesired(bool clearOnly)
        {
            ClearPlacementPanel();
            if (!clearOnly)
                SetupPlacementPanel();
        }

        /// <summary>
        /// Handler for the PlacementStateChanged event
        /// </summary>
        /// <param name="isPlacing">Whether in the placing state or not</param>
        private void OnPlacementStateChanged(bool isPlacing, bool objectWasPlacedOrUndone)
        {
            if (workingPlacementState != isPlacing)
            {
                StartCoroutine(TransitionPlacementUI(isPlacing));
                workingPlacementState = isPlacing;
            }

            if (objectWasPlacedOrUndone && GameSettings.Instance.Settings.accessibility_CameraShake)
                StartCoroutine(ShakeCamera());
        }

        /// <summary>
        /// Sets canvas elements as active
        /// </summary>
        /// <param name="isActive">Whether to set canvas elements active or inactive</param>
        private void SetCanvasActive(bool isActive)
        {
            foreach (GameObject g in mainCanvasTransitionedElements)
                g.SetActive(isActive);
            StartCoroutine(TransitionCanvasOpacity(isActive));
        }

        /// <summary>
        /// Sets up the placement panel UI each goal
        /// </summary>
        private void SetupPlacementPanel()
        {
            int buttonCount = 0;

            foreach (GoalItem g in GoalSystem.Instance.CurrentGoalWorkingSet.goalRequirements)
            {
                RectTransform rt = Instantiate(templateRequirementButton, panelPlacementButtons, false).GetComponent<RectTransform>();
                Vector3 pos = rt.anchoredPosition;
                pos.x = (rt.rect.width / 2) + (buttonCount * rt.rect.width) + (panelPlacementButtonPadding * (buttonCount + 1));
                rt.anchoredPosition = pos;
                rt.GetComponent<BuilderButton>().SetupButton(new ButtonInfo { count = g.goalStructureCount, id = (int)g.goalStructureID, req = true }, uiPalette);
                buttonCount++;
            }

            foreach (GoalItem g in GoalSystem.Instance.CurrentGoalWorkingSet.goalExtras)
            {
                RectTransform rt = Instantiate(templateExtraButton, panelPlacementButtons, false).GetComponent<RectTransform>();
                Vector3 pos = rt.anchoredPosition;
                pos.x = (rt.rect.width / 2) + (buttonCount * rt.rect.width) + (panelPlacementButtonPadding * (buttonCount + 1));
                rt.anchoredPosition = pos;
                rt.GetComponent<BuilderButton>().SetupButton(new ButtonInfo { count = g.goalStructureCount, id = (int)g.goalStructureID, req = false }, uiPalette);
                buttonCount++;
            }

            TooltipEnabledElement[] tooltips = panelPlacementButtons.GetComponentsInChildren<TooltipEnabledElement>();
            foreach (TooltipEnabledElement tooltip in tooltips)
                tooltip.SetColors(uiPalette.BackgroundAccentColor, uiPalette.ForegroundAccentColor);
        }

        /// <summary>
        /// Returns an icon based on a specified ObjectType
        /// </summary>
        /// <param name="type">The ObjectType whose icon is requested</param>
        /// <returns>The icon associated with the ObjectType specified or null if no icon is available</returns>
        public Sprite GetIcon(ObjectType type)
        {
            return icons.FirstOrDefault(i => i.Type == type).Icon;
        }

        /// <summary>
        /// When the BanishButton is pressed, open the banishment confirmation prompt
        /// </summary>
        public void OnBanishButtonPressed()
        {
            windowOpenedBanishment = true;
            StartCoroutine(TransitionWindow(true, windowBanishmentRT));
            StartCoroutine(TransitionPlacementUI(true));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the CancelBanishButton is pressed, close the banishment confirmation prompt
        /// </summary>
        public void OnCancelBanishButtonPressed()
        {
            windowOpenedBanishment = false;
            StartCoroutine(TransitionWindow(false, windowBanishmentRT));
            StartCoroutine(TransitionPlacementUI(false));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the CancelNextLevelButton is pressed, close the next level confirmation prompt
        /// </summary>
        public void OnCancelNextLevelButtonPressed()
        {
            windowOpenedNextLevel = false;
            StartCoroutine(TransitionWindow(false, windowNextLevelRT));
            StartCoroutine(TransitionPlacementUI(false));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the ConfirmBanishButton is pressed, banish a level
        /// </summary>
        public void OnConfirmBanishButtonPressed()
        {
            windowOpenedBanishment = false;
            GameEventSystem.Instance.CallForBanishment();
            StartCoroutine(TransitionWindow(false, windowBanishmentRT));
            StartCoroutine(TransitionPlacementUI(false));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the ConfirmNextLevelButton is pressed, move on to the next level
        /// </summary>
        public void OnConfirmNextLevelButtonPressed()
        {
            windowOpenedNextLevel = false;
            StartCoroutine(TransitionWindow(false, windowNextLevelRT));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Goal);
            LevelingSystem.Instance.CurrentLevel++;
        }

        /// <summary>
        /// When the NextLevelButton is pressed, open the next level confirmation prompt
        /// </summary>
        public void OnNextLevelButtonPressed()
        {
            windowOpenedNextLevel = true;
            StartCoroutine(TransitionWindow(true, windowNextLevelRT));
            StartCoroutine(TransitionPlacementUI(true));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the NextGoalButton is pressed, advance the goal and disable button interaction
        /// </summary>
        public void OnNextGoalButtonPressed()
        {
            GameEventSystem.Instance.CallToAdvanceGoal();
            buttonNextGoal.interactable = false;
        }

        /// <summary>
        /// When an object is placed, shake the camera if the setting is enabled
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShakeCamera()
        {
            Vector3 initPos = gameCamera.transform.position;
            float t = 0;

            while (t <= GameConstants.UITransitionDuration / 2)
            {
                gameCamera.transform.position = new Vector3(
                    gameCamera.transform.position.x + Random.Range(-0.050f, 0.050f),
                    gameCamera.transform.position.y + Random.Range(-0.025f, 0.025f),
                    gameCamera.transform.position.z + Random.Range(-0.050f, 0.050f));
                t += Time.deltaTime;
                yield return null;
            }

            Vector3 postShakePos = gameCamera.transform.position;
            t = 0;
            while (t <= GameConstants.UITransitionDuration / 2)
            {
                gameCamera.transform.position = Vector3.Lerp(postShakePos, initPos, animationCurveForTransitions.Evaluate(t / (GameConstants.UITransitionDuration / 2)));
                t += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Transitions the opacity of the game UI canvas
        /// </summary>
        /// <param name="isActive">Whether to set opacity to 1 or 0</param>
        private IEnumerator TransitionCanvasOpacity(bool isActive)
        {
            float t = 0;
            float start = System.Convert.ToInt32(!isActive);
            float end = System.Convert.ToInt32(isActive);
            AnimationCurve curve = AnimationCurve.Linear(0, start, 1, end);
            while (t <= GameConstants.UITransitionDuration)
            {
                mainCanvasGroup.alpha = curve.Evaluate(t / GameConstants.UITransitionDuration);
                yield return null;
                t += Time.deltaTime;
            }
            mainCanvasGroup.alpha = end;
        }

        /// <summary>
        /// Transitions the Placement UI's position by sliding it out of the way when placing and sliding it back when no longer placing
        /// </summary>
        private IEnumerator TransitionPlacementUI(bool isPlacing)
        {
            float t = 0;

            // Numbers are based off of what is set in inspector and what would put them out of view with reference canvas set at 100x100
            Vector3 start = panelPlacementRT.localPosition;
            Vector3 end = new Vector3(panelPlacementRT.localPosition.x, -100, panelPlacementRT.localPosition.z);

            Vector2 startOtherMin = panelOtherButtonsRT.offsetMin;
            Vector2 startOtherMax = panelOtherButtonsRT.offsetMax;
            Vector2 endOtherMin = new Vector2(-50, 0);
            Vector2 endOtherMax = new Vector2(50, -90);

            if (!isPlacing)
            {
                end = new Vector3(panelPlacementRT.localPosition.x, -50, panelPlacementRT.localPosition.z);
                endOtherMin = new Vector2(0, 0);
                endOtherMax = new Vector2(0, 90);
            }

            while (t <= GameConstants.UITransitionDuration)
            {
                panelPlacementRT.localPosition = Vector3.LerpUnclamped(start, end, animationCurveForTransitions.Evaluate(t / GameConstants.UITransitionDuration));
                panelOtherButtonsRT.offsetMin = Vector2.LerpUnclamped(startOtherMin, endOtherMin, animationCurveForTransitions.Evaluate(t / GameConstants.UITransitionDuration));
                panelOtherButtonsRT.offsetMax = Vector2.LerpUnclamped(startOtherMax, endOtherMax, animationCurveForTransitions.Evaluate(t / GameConstants.UITransitionDuration));
                t += Time.deltaTime;
                yield return null;
            }

            panelPlacementRT.localPosition = end;
            panelOtherButtonsRT.offsetMin = endOtherMin;
            panelOtherButtonsRT.offsetMax = endOtherMax;
        }

        /// <summary>
        /// Coroutine for animating the banishment window
        /// </summary>
        /// <param name="setActive">Whether the window is being enabled or disabled</param>
        private IEnumerator TransitionWindow(bool setActive, RectTransform window)
        {
            float t = 0;
            Vector2 start = new Vector2(0, -100);
            Vector3 end = Vector2.zero;
            if (!setActive)
            {
                end = start;
                start = Vector2.zero;
            }

            while (t <= GameConstants.UITransitionDuration)
            {
                window.anchoredPosition = Vector2.LerpUnclamped(start, end, animationCurveForTransitions.Evaluate(t / GameConstants.UITransitionDuration));
                yield return null;
                t += Time.deltaTime;
            }
            window.anchoredPosition = end;
        }
        #endregion
    }

    /// <summary>
    /// Struct for associating ObjectTypes to Icons
    /// </summary>
    [System.Serializable]
    public struct TypeToIcon
    {
        public Sprite       Icon;
        public ObjectType   Type;
    }

    /// <summary>
    /// Struct for associating colors to UI per scene
    /// </summary>
    [System.Serializable]
    public struct ColorPalette
    {
        public Color BackgroundPrimaryColor;
        public Color BackgroundSecondaryColor;
        public Color BackgroundAccentColor;
        public Color ForegroundPrimaryColor;
        public Color ForegroundSecondaryColor;
        public Color ForegroundAccentColor;
        public Color PlacementExtraColor;
        public Color PlacementRequiredColor;
    }
}