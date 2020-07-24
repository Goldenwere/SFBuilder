using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        private const int maxLevels = 2;
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
                if (value <= maxLevels)
                {
                    StartCoroutine(LoadNewScene(currentLevel, value));
                    currentLevel = value;
                }
                else
                {
                    GameEventSystem.Instance.UpdateGameState(GameState.MainMenus);
                }
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

            // First set the private member to 1 (replaced with save data) so that it will be the same when setting CurrentLevel
            currentLevel = 1;
            CurrentLevel = 1;
        }

        /// <summary>
        /// Used for loading a new scene
        /// </summary>
        /// <param name="oldSceneIndex">The old scene to unload</param>
        /// <param name="newSceneIndex">The new scene to load</param>
        private IEnumerator LoadNewScene(int oldSceneIndex, int newSceneIndex)
        {
            if (GameEventSystem.Instance.CurrentGameState == GameState.Gameplay)
            {
                GameEventSystem.Instance.TransitionLevelRequest(true);
                yield return new WaitForSecondsRealtime(GameConstants.LevelTransitionStartTime);
            }

            if (oldSceneIndex > 0 && oldSceneIndex != newSceneIndex)
            {
                AsyncOperation unload = SceneManager.UnloadSceneAsync(oldSceneIndex);
                while (!unload.isDone)
                    yield return null;
            }

            AsyncOperation load = SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive);
            while (!load.isDone)
                yield return null;

            load.allowSceneActivation = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newSceneIndex));

            Resources.UnloadUnusedAssets();

            yield return new WaitForFixedUpdate();
            GameEventSystem.Instance.TransitionLevelRequest(false);
        }
        #endregion
    }
}