using UnityEngine;
using UnityEngine.UI;
using SFBuilder.Obj;
using System.Linq;
using System.Collections;

namespace SFBuilder.UI
{
    public class GameUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private AnimationCurve animationCurveForTransitions;
        [SerializeField] private Button         buttonBanishment;
        [SerializeField] private Button         buttonNextGoal;
        [SerializeField] private TypeToIcon[]   icons;
        [SerializeField] private GameObject     mainCanvas;
        [SerializeField] private GameObject     panelPlacement;
        [SerializeField] private GameObject     windowBanishment;
#pragma warning restore 0649
        /**************/ private RectTransform  panelPlacementRT;
        /**************/ private RectTransform  windowBanishmentRT;
        public static GameUI     Instance       { get; private set; }

        /// <summary>
        /// Toggle the main canvas on Awake and set singleton instance
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            if (GameEventSystem.Instance.CurrentGameState != GameState.Gameplay)
                mainCanvas.SetActive(false);
            else
                mainCanvas.SetActive(true);

            windowBanishmentRT = windowBanishment.GetComponent<RectTransform>();
            windowBanishmentRT.anchoredPosition = new Vector2(0, -100);
            panelPlacementRT = panelPlacement.GetComponent<RectTransform>();
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
            GameEventSystem.PlacementStateChanged += OnPlacementStateChanged;
            GameEventSystem.GoalMet += OnGoalMet;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
            GameEventSystem.PlacementStateChanged -= OnPlacementStateChanged;
            GameEventSystem.GoalMet -= OnGoalMet;
        }

        /// <summary>
        /// Handler for the GameStateChanged event
        /// </summary>
        /// <param name="prevState">The previous GameState</param>
        /// <param name="newState">The new GameState</param>
        private void OnGameStateChanged(GameState prevState, GameState newState)
        {
            if (newState != GameState.Gameplay)
                mainCanvas.SetActive(false);
            else
                mainCanvas.SetActive(true);
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
        /// Handler for the PlacementStateChanged event
        /// </summary>
        /// <param name="isPlacing">Whether in the placing state or not</param>
        private void OnPlacementStateChanged(bool isPlacing)
        {
            StartCoroutine(TransitionPlacementUI(isPlacing));
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
            StartCoroutine(TransitionWindowBanishment(true));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the CancelBanishButton is pressed, close the banishment confirmation prompt
        /// </summary>
        public void OnCancelBanishButtonPressed()
        {
            StartCoroutine(TransitionWindowBanishment(false));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// When the ConfirmBanishButton is pressed, banish a level
        /// </summary>
        public void OnConfirmBanishButtonPressed()
        {
            GameEventSystem.Instance.CallForBanishment();
            StartCoroutine(TransitionWindowBanishment(false));
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
        }

        /// <summary>
        /// Transitions the Placement UI's position by sliding it out of the way when placing and sliding it back when no longer placing
        /// </summary>
        private IEnumerator TransitionPlacementUI(bool isPlacing)
        {
            float t = 0;
            Vector3 start = panelPlacementRT.localPosition;
            Vector3 end = new Vector3(panelPlacementRT.localPosition.x, -20, panelPlacementRT.localPosition.z);
            if (!isPlacing)
                end = new Vector3(panelPlacementRT.localPosition.x, 0, panelPlacementRT.localPosition.z);
            while (t <= GameConstants.UITransitionDuration)
            {
                panelPlacementRT.localPosition = Vector3.LerpUnclamped(start, end, animationCurveForTransitions.Evaluate(t / GameConstants.UITransitionDuration));
                t += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Coroutine for animating the banishment window
        /// </summary>
        /// <param name="setActive">Whether the window is being enabled or disabled</param>
        private IEnumerator TransitionWindowBanishment(bool setActive)
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
                windowBanishmentRT.anchoredPosition = Vector2.LerpUnclamped(start, end, animationCurveForTransitions.Evaluate(t / GameConstants.UITransitionDuration));
                yield return null;
                t += Time.deltaTime;
            }
            windowBanishmentRT.anchoredPosition = end;
        }
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
}