using UnityEngine;

namespace SFBuilder.Gameplay
{
    /// <summary>
    /// Manages scene transitioning based on level
    /// </summary>
    /// <remarks>This is a singleton that can be referenced through LevelingSystem.Instance; present in the base level</remarks>
    public class LevelingSystem : MonoBehaviour
    {
        #region Fields
        private int currentLevel;   // directly equivalent to unity scenes, with scene 0 being the base scene
        #endregion
        #region Properties
        /// <summary>
        /// The current level (represents index of scene in build settings
        /// </summary>
        public int CurrentLevel
        {
            get { return currentLevel; }
            set
            {
                currentLevel = value;
                // TO-DO: transitions
            }
        }

        /// <summary>
        /// Singleton instance of LevelingSystem in the game level scene
        /// </summary>
        public static LevelingSystem Instance { get; private set; }
        #endregion
        #region Methods
        /// <summary>
        /// Set singleton instance on Awake and load the current level (currently not saved)
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            currentLevel = 1;
        }
        #endregion
    }
}