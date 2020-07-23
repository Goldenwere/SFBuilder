using UnityEngine;

namespace SFBuilder.Gameplay
{
    /// <summary>
    /// Manages the scoring system of Happiness, Power, and Sustenance and their sum Viability
    /// </summary>
    /// <remarks>This is a singleton that can be referenced through GameScoring.Instance; present in each level scene except the base scene</remarks>
    public class GameScoring : MonoBehaviour
    {
        #region Properties
        /// <summary>
        /// Singleton instance of GameScoring in the game level scene
        /// </summary>
        public static GameScoring Instance  { get; private set; }

        /// <summary>
        /// Potential happiness score of a BuilderObject being placed; applied once it is placed
        /// </summary>
        public int PotentialHappiness       { get; set; }

        /// <summary>
        /// Potential power score of a BuilderObject being placed; applied once it is placed
        /// </summary>
        public int PotentialPower           { get; set; }

        /// <summary>
        /// Potential sustenance score of a BuilderObject being placed; applied once it is placed
        /// </summary>
        public int PotentialSustenance      { get; set; }

        /// <summary>
        /// Potential viability score of a BuilderObject being placed; does not need applied to viability
        /// </summary>
        public int PotentialViability       { get { return PotentialPower + PotentialSustenance + PotentialHappiness; } }

        /// <summary>
        /// Current happiness score
        /// </summary>
        public int TotalHappiness           { get; set; }

        /// <summary>
        /// Current power score
        /// </summary>
        public int TotalPower               { get; set; }

        /// <summary>
        /// Current sustenance score
        /// </summary>
        public int TotalSustenance          { get; set; }

        /// <summary>
        /// Current viability score
        /// </summary>
        public int TotalViability           { get { return TotalPower + TotalSustenance + TotalHappiness; } }
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
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.ScoreUpdateDesired += OnScoreWasChanged;
            GameEventSystem.LevelBanished += OnLevelBanished;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.ScoreUpdateDesired += OnScoreWasChanged;
            GameEventSystem.LevelBanished -= OnLevelBanished;
        }

        /// <summary>
        /// Applies potential to current scores once a BuilderObject is placed
        /// </summary>
        public void ApplyScore()
        {
            if (PotentialHappiness != 0)
            {
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialHappiness, PotentialHappiness);
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalHappiness, TotalHappiness);
            }
            if (PotentialPower != 0)
            {
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialPower, PotentialPower);
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalPower, TotalPower);
            }
            if (PotentialSustenance != 0)
            {
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialSustenance, PotentialSustenance);
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalSustenance, TotalSustenance);
            }
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialViability, PotentialViability);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalViability, TotalViability);

            TotalPower += PotentialPower;
            TotalSustenance += PotentialSustenance;
            TotalHappiness += PotentialHappiness;
            PotentialPower = 0;
            PotentialSustenance = 0;
            PotentialHappiness = 0;
        }

        /// <summary>
        /// Revokes a score when a BuilderObject is undone
        /// </summary>
        /// <param name="happiness">BuilderObject's placed happiness value</param>
        /// <param name="power">BuilderObject's placed power value</param>
        /// <param name="sustenance">BuilderObject's placed sustenace value</param>
        public void RevokeScore(int happiness, int power, int sustenance)
        {
            if (happiness != 0)
            {
                TotalHappiness -= happiness;
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalHappiness, TotalHappiness);
            }
            if (power != 0)
            {
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialPower, PotentialPower);
                TotalPower -= power;
            }
            if (sustenance != 0)
            {
                GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalSustenance, TotalSustenance);
                TotalSustenance -= sustenance;
            }
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalViability, TotalViability);
        }

        /// <summary>
        /// On the LevelBanished event, clear scores
        /// </summary>
        private void OnLevelBanished()
        {
            TotalHappiness = 0;
            TotalPower = 0;
            TotalSustenance = 0;
            PotentialHappiness = 0;
            PotentialPower = 0;
            PotentialSustenance = 0;

            GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialHappiness, PotentialHappiness);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalHappiness, TotalHappiness);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialPower, PotentialPower);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalPower, TotalPower);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialSustenance, PotentialSustenance);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalSustenance, TotalSustenance);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialViability, PotentialViability);
            GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalViability, TotalViability);
        }

        /// <summary>
        /// Update the score system with a delta amount
        /// </summary>
        /// <param name="type">The score being updated</param>
        /// <param name="scoreDelta">The delta value</param>
        private void OnScoreWasChanged(ScoreType type, int scoreDelta)
        {
            switch(type)
            {
                case ScoreType.PotentialHappiness:
                    PotentialHappiness += scoreDelta;
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialHappiness, PotentialHappiness);
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialViability, PotentialViability);
                    break;
                case ScoreType.PotentialPower:
                    PotentialPower += scoreDelta;
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialPower, PotentialPower);
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialViability, PotentialViability);
                    break;
                case ScoreType.PotentialSustenance:
                    PotentialSustenance += scoreDelta;
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialSustenance, PotentialSustenance);
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.PotentialViability, PotentialViability);
                    break;
                case ScoreType.TotalHappiness:
                    TotalHappiness += scoreDelta;
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalHappiness, TotalHappiness);
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalViability, TotalViability);
                    break;
                case ScoreType.TotalPower:
                    TotalPower += scoreDelta;
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalPower, TotalPower);
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalViability, TotalViability);
                    break;
                case ScoreType.TotalSustenance:
                    TotalSustenance += scoreDelta;
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalSustenance, TotalSustenance);
                    GameEventSystem.Instance.UpdateScoreUI(ScoreType.TotalViability, TotalViability);
                    break;
            }
        }
        #endregion
    }
}