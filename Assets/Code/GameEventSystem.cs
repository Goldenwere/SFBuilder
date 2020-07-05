using UnityEngine;

namespace SFBuilder
{
    public delegate void GoalChange(bool isUndo, int id, bool isRequired);
    public delegate void ScoreChange(ScoreType scoreChanged, int number);

    public class GameEventSystem : MonoBehaviour
    {
        public static GameEventSystem Instance { get; private set; }

        /// <summary>
        /// Used for updating goals
        /// </summary>
        public static event GoalChange GoalChanged;

        /// <summary>
        /// Used for updating UI
        /// </summary>
        public static event ScoreChange ScoreWasChanged;

        /// <summary>
        /// Used for updating score system
        /// </summary>
        public static event ScoreChange ScoreUpdateDesired;

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
        /// Use this to update a goal
        /// </summary>
        /// <param name="isUndo">Whether to increase or decrease a goal value (undo increases, normal decreases)</param>
        /// <param name="id">The id of the BuilderObject associated with the goal</param>
        /// <param name="isRequired">Whether the BuilderObject was a requirement</param>
        public void UpdateGoalAmount(bool isUndo, int id, bool isRequired)
        {
            GoalChanged?.Invoke(isUndo, id, isRequired);
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
    /// Info sent to score system or UI for updating
    /// </summary>
    public enum ScoreType
    {
        PotentialHappiness,
        PotentialPower,
        PotentialSustenance,
        TotalHappiness,
        TotalPower,
        TotalSustenance
    }
}