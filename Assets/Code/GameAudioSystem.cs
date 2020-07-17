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
#pragma warning restore 0649
        /**************/ private int                    audioSourceIterator;
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
        Transition,
        Undo
    }
}