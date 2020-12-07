using UnityEngine;

namespace SFBuilder
{
    public delegate void DoubleBoolDelegate(bool valA, bool valB);
    public delegate void BoolDelegate(bool val);
    public delegate void GameStateChange(GameState prevState, GameState newState);
    public delegate void GenericDelegate();
    public delegate void GoalChange(bool isUndo, int id, bool isRequired);
    public delegate void ScoreChange(ScoreType scoreChanged, int number);

    /// <summary>
    /// The GameEventSystem is used for passing data and updates to subscribed classes
    /// </summary>
    /// <remarks>This is a singleton that can be referenced through LevelingSystem.Instance; present in the base level</remarks>
    public class GameEventSystem : MonoBehaviour
    {
        #region Properties
        /// <summary>
        /// The current state of the game
        /// </summary>
        public GameState                CurrentGameState { get; private set; }

        /// <summary>
        /// Singleton instance of GameEventSystem in the base scene
        /// </summary>
        public static GameEventSystem   Instance { get; private set; }
        #endregion
        #region Events
        /// <summary>
        /// Used for handling gamestate changes
        /// </summary>
        public static event GameStateChange     GameStateChanged;

        /// <summary>
        /// Used for advancing goals
        /// </summary>
        public static event GenericDelegate     GoalChanged;

        /// <summary>
        /// Used for updating goals
        /// </summary>
        public static event GoalChange          GoalInfoChanged;

        /// <summary>
        /// Used for allowing to move on to next goal
        /// </summary>
        public static event BoolDelegate        GoalMet;

        /// <summary>
        /// Used for reseting a level back to its original state
        /// </summary>
        public static event GenericDelegate     LevelBanished;

        /// <summary>
        /// Used for transitioning between scenes
        /// </summary>
        public static event BoolDelegate        LevelTransitioned;

        /// <summary>
        /// Used for updating whether the next level is ready or not
        /// </summary>
        public static event BoolDelegate        NextLevelReadyStateChanged;

        /// <summary>
        /// Used for updating the placement panel UI
        /// </summary>
        public static event BoolDelegate        PlacementPanelUpdateDesired;

        /// <summary>
        /// Used for notifying placement state change
        /// </summary>
        public static event DoubleBoolDelegate  PlacementStateChanged;

        /// <summary>
        /// Used for notifying that a scene was activated
        /// </summary>
        public static event GenericDelegate     SceneActivated;

        /// <summary>
        /// Used for updating UI
        /// </summary>
        public static event ScoreChange         ScoreWasChanged;

        /// <summary>
        /// Used for updating score system
        /// </summary>
        public static event ScoreChange         ScoreUpdateDesired;

        /// <summary>
        /// Used for notifying of change in settings
        /// </summary>
        public static event GenericDelegate     SettingsUpdated;
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

            CurrentGameState = GameState.MainMenus;
        }

        /// <summary>
        /// Advances the goal
        /// </summary>
        public void CallToAdvanceGoal()
        {
            GoalChanged?.Invoke();
        }

        /// <summary>
        /// Banishes a level back to its original state
        /// </summary>
        public void CallForBanishment()
        {
            LevelBanished?.Invoke();
            NextLevelReadyStateChanged?.Invoke(false);
            GoalMet?.Invoke(false);
            GoalChanged?.Invoke();
        }

        /// <summary>
        /// Allows/Disallows for moving on to next level
        /// </summary>
        /// <param name="isReady">Whether the player can move on to the next level or not</param>
        public void NotifyLevelReadyState(bool isReady)
        {
            NextLevelReadyStateChanged?.Invoke(isReady);
        }

        /// <summary>
        /// Notifies that a scene was activated
        /// </summary>
        public void NotifySceneActivated()
        {
            SceneActivated?.Invoke();
        }

        /// <summary>
        /// Notifies that GameSettings has updated
        /// </summary>
        public void NotifySettingsChanged()
        {
            SettingsUpdated?.Invoke();
        }

        /// <summary>
        /// Updates the current game state
        /// </summary>
        /// <param name="newState">The new GameState to be in</param>
        public void UpdateGameState(GameState newState)
        {
            GameStateChanged?.Invoke(CurrentGameState, newState);
            CurrentGameState = newState;
        }

        /// <summary>
        /// Use this to update a goal
        /// </summary>
        /// <param name="isUndo">Whether to increase or decrease a goal value (undo increases, normal decreases)</param>
        /// <param name="id">The id of the BuilderObject associated with the goal</param>
        /// <param name="isRequired">Whether the BuilderObject was a requirement</param>
        public void UpdateGoalAmount(bool isUndo, int id, bool isRequired)
        {
            GoalInfoChanged?.Invoke(isUndo, id, isRequired);
        }

        /// <summary>
        /// Updates the GoalMet state
        /// </summary>
        /// <param name="isMet">Whether the current goal was met or not</param>
        public void UpdateGoalMetState(bool isMet)
        {
            GoalMet?.Invoke(isMet);
        }

        /// <summary>
        /// Use this to update the placement panel UI
        /// </summary>
        /// <param name="clearOnly">Whether to only clear the UI or to fully set it up</param>
        public void UpdatePlacementPanel(bool clearOnly = false)
        {
            PlacementPanelUpdateDesired?.Invoke(clearOnly);
        }

        /// <summary>
        /// Notifies handlers that the placement state has changed
        /// </summary>
        /// <param name="isPlacing">Whether in the placing state or not</param>
        /// <param name="objectWasPlaced">Whether an object was placed</param>
        public void UpdatePlacementState(bool isPlacing, bool objectWasPlacedOrUndone)
        {
            PlacementStateChanged?.Invoke(isPlacing, objectWasPlacedOrUndone);
        }

        /// <summary>
        /// Update the score system with a delta amount
        /// </summary>
        /// <param name="type">The score being updated</param>
        /// <param name="scoreDelta">The delta value</param>
        public void UpdateScoreSystem(ScoreType type, int scoreDelta)
        {
            ScoreUpdateDesired?.Invoke(type, scoreDelta);
        }

        /// <summary>
        /// Update the UI with a new score value
        /// </summary>
        /// <param name="type">The score that was updated</param>
        /// <param name="score">The new value of the score</param>
        public void UpdateScoreUI(ScoreType type, int score)
        {
            ScoreWasChanged?.Invoke(type, score);
        }

        /// <summary>
        /// Engage in transitioning for level loading
        /// </summary>
        /// <param name="isStart">Whether to start transition (before load) or end transition (after load)</param>
        public void TransitionLevelRequest(bool isStart)
        {
            LevelTransitioned?.Invoke(isStart);
        }
        #endregion
    }

    /// <summary>
    /// Info sent to buttons for initialization
    /// </summary>
    public struct ButtonInfo
    {
        public int  count;
        public int  id;
        public bool req;
    }

    /// <summary>
    /// Defines what the current state of the game is
    /// </summary>
    public enum GameState
    {
        MainMenus,
        Gameplay
    }

    /// <summary>
    /// Info sent to score system or UI for updating
    /// </summary>
    public enum ScoreType
    {
        PotentialHappiness,
        PotentialPower,
        PotentialSustenance,
        PotentialViability,
        TotalHappiness,
        TotalPower,
        TotalSustenance,
        TotalViability,
        CurrentGoal,
        CurrentGoalMinimumViability
    }
}