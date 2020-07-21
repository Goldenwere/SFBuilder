using System.Collections;
using System.Linq;
using UnityEngine;

namespace SFBuilder
{
    /// <summary>
    /// Used for playing music and sound effects without having to manage multiple references in UI
    /// </summary>
    public class GameAudioSystem : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
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
        }

        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
        }

        private IEnumerator FadeSources()
        {
            float t = 0;
            while (t <= GameConstants.MusicFadeTime)
            {
                audioSources[0].volume = AnimationCurve.Linear(0, GameConstants.MusicSourceMaxVolume, 1, 0).Evaluate(t / GameConstants.MusicFadeTime);
                audioSources[1].volume = AnimationCurve.Linear(0, 0, 1, GameConstants.MusicSourceMaxVolume).Evaluate(t / GameConstants.MusicFadeTime);
                yield return null;
            }
            audioSources[0].volume = 0;
            audioSources[1].volume = GameConstants.MusicSourceMaxVolume;
        }

        private void OnGameStateChanged(GameState prev, GameState curr)
        {
            if (!leftMenuForFirstTime)
            {
                StartCoroutine(FadeSources());
                leftMenuForFirstTime = true;
                currentMusicIndex = Random.Range(0, musicTracks.Length);
                musicSources[musicSourceIterator].clip = musicTracks[currentMusicIndex];
                musicSources[musicSourceIterator].Play();

                musicSourceIterator++;
                if (musicSourceIterator >= musicSources.Length)
                    musicSourceIterator = 0;
            }
        }

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

        private IEnumerator WaitToPlayNextTrack()
        {
            yield return new WaitForSecondsRealtime(GameConstants.MusicWaitTime);

            int newIndex = currentMusicIndex;
            while (newIndex == currentMusicIndex)
                newIndex = Random.Range(0, musicTracks.Length);
            currentMusicIndex = newIndex;

            PlayNextTrack();
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