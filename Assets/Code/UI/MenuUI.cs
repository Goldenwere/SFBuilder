using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SFBuilder.UI
{
    public class MenuUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private CanvasGroup    canvas;
        [SerializeField] private GameObject[]   canvasMainElements;
        [SerializeField] private Image          startupFadeImage;
        [SerializeField] private AnimationCurve transitionCurve;
        [SerializeField] private AnimationCurve transitionStartupFade;
        [SerializeField] private AnimationCurve transitionStartupToWhite;
        [SerializeField] private Image          transitionImage;
#pragma warning restore 0649

        /// <summary>
        /// Copy the material for transitions since it messes with asset file
        /// </summary>
        private void Start()
        {
            Material copy = new Material(transitionImage.material);
            transitionImage.material = copy;

            canvas.gameObject.SetActive(false);
            foreach (GameObject g in canvasMainElements)
                g.SetActive(false);

            StartCoroutine(StartupAnimation());
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
        /// On the GameStateChanged event, toggle the menu canvas
        /// </summary>
        /// <param name="prevState">The previous GameState</param>
        /// <param name="currState">The current GameState</param>
        private void OnGameStateChanged(GameState prevState, GameState currState)
        {
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
            Application.Quit();
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
    }
}
