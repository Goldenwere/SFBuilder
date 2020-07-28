using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SFBuilder
{
    /// <summary>
    /// Used for playing music and sound effects without having to manage multiple references in UI
    /// </summary>
    public class GameAudioSystem : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private AudioMixer             audioMixer;
        [SerializeField] private AudioSource[]          audioSources;
        [SerializeField] private AudioClipAssociation[] audioClips;
        [SerializeField] private AudioSource[]          musicSources;
        [SerializeField] private AudioClip              menuTrack;
        [SerializeField] private AudioClip[]            musicTracks;
#pragma warning restore 0649
        /**************/ private int                    audioSourceIterator;
        /**************/ private int                    currentMusicIndex;
        /**************/ private bool                   leftMenuForFirstTime;
        /**************/ private int                    musicSourceIterator;
        #endregion
        #region Properties
        /// <summary>
        /// Singleton instance of the GameAudioSystem in the base scene
        /// </summary>
        public static GameAudioSystem   Instance { get; private set; }
        #endregion
        #region Methods
        /// <summary>
        /// Set singleton instance on Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        /// <summary>
        /// Plays the menu track at Start
        /// </summary>
        private void Start()
        {
            musicSources[musicSourceIterator].clip = menuTrack;
            musicSources[musicSourceIterator].Play();

            musicSourceIterator++;
            if (musicSourceIterator >= musicSources.Length)
                musicSourceIterator = 0;

            LoadAudioSettings();
        }

        /// <summary>
        /// On Enable, subscribe to the GameStateChanged event
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
            GameEventSystem.SettingsUpdated += OnSettingsUpdated;
        }

        /// <summary>
        /// On Disable, unsubscribe from the GameStateChanged event
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
            GameEventSystem.SettingsUpdated -= OnSettingsUpdated;
        }

        /// <summary>
        /// Loads audio settings
        /// </summary>
        private void LoadAudioSettings()
        {
            audioMixer.SetFloat(GameConstants.AudioParamEffects, Mathf.Log10(GameSettings.Instance.Settings.volEffects) * 20);
            audioMixer.SetFloat(GameConstants.AudioParamMusic, Mathf.Log10(GameSettings.Instance.Settings.volMusic) * 20);
        }

        /// <summary>
        /// When leaving the menu for the first time, fade between menu music and new track in case menu music is still playing
        /// </summary>
        /// <param name="prev">The previous gamestate</param>
        /// <param name="curr">The new gamestate</param>
        private void OnGameStateChanged(GameState prev, GameState curr)
        {
            if (!leftMenuForFirstTime && curr == GameState.Gameplay)
            {
                StartCoroutine(FadeSources());
                leftMenuForFirstTime = true;
                currentMusicIndex = Random.Range(0, musicTracks.Length);
                musicSources[musicSourceIterator].clip = musicTracks[currentMusicIndex];
                musicSources[musicSourceIterator].Play();

                musicSourceIterator++;
                if (musicSourceIterator >= musicSources.Length)
                    musicSourceIterator = 0;

                StartCoroutine(WaitToPlayNextTrack());
            }
        }

        /// <summary>
        /// Handler for the SettingsUpdated event
        /// </summary>
        private void OnSettingsUpdated()
        {
            LoadAudioSettings();
        }

        /// <summary>
        /// Used for playing the next track after WaitToPlayNextTrack is finished
        /// </summary>
        private void PlayNextTrack()
        {
            StopCoroutine(WaitToPlayNextTrack());

            musicSources[musicSourceIterator].clip = musicTracks[currentMusicIndex];
            musicSources[musicSourceIterator].Play();

            musicSourceIterator++;
            if (musicSourceIterator >= musicSources.Length)
                musicSourceIterator = 0;

            StartCoroutine(WaitToPlayNextTrack());
        }

        /// <summary>
        /// Use to play an audio clip
        /// </summary>
        /// <param name="clipToPlay">The clip to play in the audio system</param>
        /// <param name="delay">Optional delay before playing the sound</param>
        public void PlaySound(AudioClipDefinition clipToPlay, float delay = 0)
        {
            audioSources[audioSourceIterator].Stop();
            audioSources[audioSourceIterator].clip = audioClips.First(assoc => assoc.sound == clipToPlay).clip;
            audioSources[audioSourceIterator].PlayDelayed(delay);

            audioSourceIterator++;
            if (audioSourceIterator >= audioSources.Length)
                audioSourceIterator = 0;
        }

        /// <summary>
        /// Coroutine for fading between the first and second audio sources after leaving the menu for the first time
        /// </summary>
        private IEnumerator FadeSources()
        {
            float t = 0;
            while (t <= GameConstants.MusicFadeTime)
            {
                musicSources[0].volume = AnimationCurve.Linear(0, GameConstants.MusicSourceMaxVolume, 1, 0).Evaluate(t / GameConstants.MusicFadeTime);
                musicSources[1].volume = AnimationCurve.Linear(0, 0, 1, GameConstants.MusicSourceMaxVolume).Evaluate(t / GameConstants.MusicFadeTime);
                t += Time.deltaTime;
                yield return null;
            }

            musicSources[0].Stop();
            musicSources[0].volume = GameConstants.MusicSourceMaxVolume;
            musicSources[1].volume = GameConstants.MusicSourceMaxVolume;
        }

        /// <summary>
        /// Coroutine for waiting to play the next track and to grab a new index (which cannot be the old one)
        /// </summary>
        private IEnumerator WaitToPlayNextTrack()
        {
            yield return new WaitForSecondsRealtime(GameConstants.MusicWaitTime);

            int newIndex = currentMusicIndex;
            while (newIndex == currentMusicIndex)
                newIndex = Random.Range(0, musicTracks.Length);
            currentMusicIndex = newIndex;

            PlayNextTrack();
        }
        #endregion
    }

    /// <summary>
    /// Used for tying audio clips to 
    /// </summary>
    [System.Serializable]
    public struct AudioClipAssociation
    {
        public AudioClip            clip;
        public AudioClipDefinition  sound;
    }

    /// <summary>
    /// Defined audio clips that other classes can play using the event system
    /// </summary>
    public enum AudioClipDefinition
    {
        Button,
        Goal,
        Placement,
        Pop,
        Transition,
        Undo
    }
}