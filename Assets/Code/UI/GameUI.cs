using UnityEngine;
using UnityEngine.UI;

namespace SFBuilder.UI
{
    public class GameUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private GameObject mainCanvas;
#pragma warning restore 0649

        /// <summary>
        /// Toggle the main canvas on Awake
        /// </summary>
        private void Awake()
        {
            if (GameEventSystem.Instance.CurrentGameState != GameState.Gameplay)
                mainCanvas.SetActive(false);
            else
                mainCanvas.SetActive(true);

            GameEventSystem.GameStateChanged += OnGameStateChanged;
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
    }
}