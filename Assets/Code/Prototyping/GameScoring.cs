using UnityEngine;

public class GameScoring : MonoBehaviour
{
    public static GameScoring Instance { get; private set; }

    public int ScoreHappiness { get; set; }
    public int ScorePower { get; set; }
    public int ScoreSustenance { get; set; }
    public int ScoreViability { get { return ScorePower + ScoreSustenance + ScoreHappiness; } }

    public int PotentialHappiness { get; set; }
    public int PotentialPower { get; set; }
    public int PotentialSustenance { get; set; }
    public int PotentialViability { get { return PotentialPower + PotentialSustenance + PotentialHappiness; } }

    public bool IsPlacing { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

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
                    ProtoLevelSystem.Instance.CurrentLevel, ProtoGoalSystem.Instance.CurrentGoal + 1),
                new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
        }

        else
            GUI.Label(
                new Rect(10, 10, 100, 20),
                string.Format("Level: {4}, Goal: {5} ::: Viability: {0} ::: Power: {1} / Sustenance: {2} / Happiness: {3}", 
                ScoreViability, ScorePower, ScoreSustenance, ScoreHappiness,
                ProtoLevelSystem.Instance.CurrentLevel, ProtoGoalSystem.Instance.CurrentGoal + 1),
                new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
    }

    public void ApplyScore()
    {
        ScorePower += PotentialPower;
        ScoreSustenance += PotentialSustenance;
        ScoreHappiness += PotentialHappiness;
        PotentialPower = 0;
        PotentialSustenance = 0;
        PotentialHappiness = 0;
    }

    public void RevokeScore(int happiness, int power, int sustenance)
    {
        ScoreHappiness -= happiness;
        ScorePower -= power;
        ScoreSustenance -= sustenance;
    }
}
