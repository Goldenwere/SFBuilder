using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SFBuilder.UI
{
    public class MenuUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private GameObject     canvas;
        [SerializeField] private AnimationCurve transitionCurve;
        [SerializeField] private Image          transitionImage;
#pragma warning restore 0649

        /// <summary>
        /// Copy the material for transitions since it messes with asset file
        /// </summary>
        private void Start()
        {
            Material copy = new Material(transitionImage.material);
            transitionImage.material = copy;
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
        /// When the play button is pressed, load the game
        /// </summary>
        public void OnPlayPressed()
        {
            canvas.SetActive(false);
            GameEventSystem.Instance.UpdateGameState(GameState.Gameplay);
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Goal);
        }

        /// <summary>
        /// On the GameStateChanged event, toggle the menu canvas
        /// </summary>
        /// <param name="prevState">The previous GameState</param>
        /// <param name="currState">The current GameState</param>
        private void OnGameStateChanged(GameState prevState, GameState currState)
        {
            if (currState == GameState.MainMenus)
                canvas.SetActive(true);
            else
                canvas.SetActive(false);
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
    }
}