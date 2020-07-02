using UnityEngine;

public class GameScoring : MonoBehaviour
{
    public static GameScoring Instance { get; private set; }
    public int Score { get; set; }
    public int Potential { get; set; }

    public int Viability { get { return Power + Sustenance + Happiness; } }
    public int Power { get; set; }
    public int Sustenance { get; set; }
    public int Happiness { get; set; }

    public int ViabilityPotential { get; set; }
    public int PowerPotential { get; set; }
    public int SustenancePotential { get; set; }
    public int HappinessPotential { get; set; }

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

            if (ViabilityPotential < 0)
                signViability = '-';
            if (PowerPotential < 0)
                signPower = '-';
            if (SustenancePotential < 0)
                signSustenance = '-';
            if (HappinessPotential < 0)
                signHappiness = '-';

            GUI.Label(
                new Rect(10, 10, 100, 20),
                string.Format("Viability: {0} ({4}{8}) ::: Power: {1} ({5}{9}) / Sustenance: {2} ({6}{10}) / Happiness: {3} ({7}{11})", 
                    Viability, Power, Sustenance, Happiness,
                    signViability, signPower, signSustenance, signHappiness,
                    ViabilityPotential, PowerPotential, SustenancePotential, HappinessPotential),
                new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
        }

        else
            GUI.Label(
                new Rect(10, 10, 100, 20),
                string.Format("Viability: {0} ::: Power: {1} / Sustenance: {2} / Happiness: {3}", Viability, Power, Sustenance, Happiness),
                new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
    }
}
