using UnityEngine;
using SFBuilder.Obj;
using System.Linq;

namespace SFBuilder.UI
{
    public class GameUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private GameObject     buttonBanishment;
        [SerializeField] private GameObject     buttonNextGoal;
        [SerializeField] private TypeToIcon[]   icons;
        [SerializeField] private GameObject     mainCanvas;
        [SerializeField] private GameObject     panelPlacement;
        [SerializeField] private GameObject     windowBanishment;
#pragma warning restore 0649
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
            windowBanishmentRT.anchoredPosition = new Vector2(0, 100);
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
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

        }

        /// <summary>
        /// When the CancelBanishButton is pressed, close the banishment confirmation prompt
        /// </summary>
        public void OnCancelBanishButtonPressed()
        {

        }

        /// <summary>
        /// When the ConfirmBanishButton is pressed, banish a level
        /// </summary>
        public void OnConfirmBanishButtonPressed()
        {
            GameEventSystem.Instance.CallForBanishment();
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