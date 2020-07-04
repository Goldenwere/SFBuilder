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
        public int ScoreHappiness           { get; set; }

        /// <summary>
        /// Current power score
        /// </summary>
        public int ScorePower               { get; set; }

        /// <summary>
        /// Current sustenance score
        /// </summary>
        public int ScoreSustenance          { get; set; }

        /// <summary>
        /// Current viability score
        /// </summary>
        public int ScoreViability           { get { return ScorePower + ScoreSustenance + ScoreHappiness; } }

        /// <summary>
        /// Temporary property used for GUI; will eventually be removed
        /// </summary>
        public bool IsPlacing               { get; set; }
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
        /// Temporary GUI drawing to show current values of variables
        /// </summary>
        private void OnGUI()
        {
            if (IsPlacing)
            {
                char signViability = '+';
                char signPower = '+';
                char signSustenance = '+';
                char signHappiness = '+';

                if (PotentialViability < 0)
                    signViability = '\0';
                if (PotentialPower < 0)
                    signPower = '\0';
                if (PotentialSustenance < 0)
                    signSustenance = '\0';
                if (PotentialHappiness < 0)
                    signHappiness = '\0';

                GUI.Label(
                    new Rect(10, 10, 100, 20),
                    string.Format("Level: {12}, Goal: {13} ::: Viability: {0} ({4}{8}) ::: Power: {1} ({5}{9}) / Sustenance: {2} ({6}{10}) / Happiness: {3} ({7}{11})",
                        ScoreViability, ScorePower, ScoreSustenance, ScoreHappiness,
                        signViability, signPower, signSustenance, signHappiness,
                        PotentialViability, PotentialPower, PotentialSustenance, PotentialHappiness,
                        LevelingSystem.Instance.CurrentLevel, GoalSystem.Instance.CurrentGoal + 1),
                    new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
            }

            else
                GUI.Label(
                    new Rect(10, 10, 100, 20),
                    string.Format("Level: {4}, Goal: {5} ::: Viability: {0} ::: Power: {1} / Sustenance: {2} / Happiness: {3}",
                    ScoreViability, ScorePower, ScoreSustenance, ScoreHappiness,
                    LevelingSystem.Instance.CurrentLevel, GoalSystem.Instance.CurrentGoal + 1),
                    new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
        }

        /// <summary>
        /// Applies potential to current scores once a BuilderObject is placed
        /// </summary>
        public void ApplyScore()
        {
            ScorePower += PotentialPower;
            ScoreSustenance += PotentialSustenance;
            ScoreHappiness += PotentialHappiness;
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
            ScoreHappiness -= happiness;
            ScorePower -= power;
            ScoreSustenance -= sustenance;
        }
        #endregion
    }
}