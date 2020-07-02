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

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnGUI()
    {
        GUI.Label(
            new Rect(10, 10, 100, 20),
            string.Format("Viability: {0} ::: Power: {1} / Sustenance: {2} / Happiness: {3}", Viability, Power, Sustenance, Happiness),
            new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
    }
}
