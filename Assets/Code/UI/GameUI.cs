using UnityEngine;
using SFBuilder.Obj;
using System.Linq;

namespace SFBuilder.UI
{
    public class GameUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private GameObject     mainCanvas;
        [SerializeField] private TypeToIcon[]   icons;
#pragma warning restore 0649
        public GameUI   Instance    { get; private set; }

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