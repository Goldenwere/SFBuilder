﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SFBuilder.UI
{
    public class MenuUI : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private CanvasGroup            canvas;
        [SerializeField] private GameObject[]           canvasMainElements;
        [SerializeField] private GameObject[]           canvasSettingsElements;
        [SerializeField] private SettingsMenuElements   settingsMenuElements;
        [SerializeField] private Image                  startupFadeImage;
        [SerializeField] private AnimationCurve         transitionCurve;
        [SerializeField] private AnimationCurve         transitionStartupFade;
        [SerializeField] private AnimationCurve         transitionStartupToWhite;
        [SerializeField] private Image                  transitionImage;
#pragma warning restore 0649
        /**************/ private SettingsData           workingSettings;
        #endregion
        #region Inline Classes
        /// <summary>
        /// Collection of elements on the settings menu, whose values are set every time the menu loads
        /// </summary>
        [System.Serializable]
        protected class SettingsMenuElements
        {
            public Toggle   postprocAO;
            public Toggle   postprocBloom;
            public Toggle   postprocSSR;
            public Slider   volMusic;
            public Slider   volEffects;
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
        /// Loads settings data into the UI
        /// </summary>
        private void LoadSettings()
        {
            settingsMenuElements.postprocAO.SetIsOnWithoutNotify(workingSettings.postprocAO);
            settingsMenuElements.postprocBloom.SetIsOnWithoutNotify(workingSettings.postprocBloom);
            settingsMenuElements.postprocSSR.SetIsOnWithoutNotify(workingSettings.postprocSSR);
            settingsMenuElements.volEffects.SetValueWithoutNotify(workingSettings.volEffects);
            settingsMenuElements.volMusic.SetValueWithoutNotify(workingSettings.volMusic);
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
        /// When the save button is pressed on the settings menu, save settings
        /// </summary>
        public void OnSettingsSavePressed()
        {
            GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
            GameSettings.Instance.Settings = workingSettings;
        }

        /// <summary>
        /// Update effects vol on slider change
        /// </summary>
        /// <param name="val">New volume setting</param>
        public void OnValueChanged_Audio_Effects(float val)
        {
            workingSettings.volEffects = val;
        }

        /// <summary>
        /// Update music vol on slider change
        /// </summary>
        /// <param name="val">New volume setting</param>
        public void OnValueChanged_Audio_Music(float val)
        {
            workingSettings.volMusic = val;
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
