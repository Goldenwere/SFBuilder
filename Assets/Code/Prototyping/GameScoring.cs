using UnityEngine;

public class GameScoring : MonoBehaviour
{
    public static GameScoring Instance { get; private set; }
    public int Score { get; set; }
    public int Potential { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), string.Format("Score: {0} | Potential: {1}", Score, Potential), new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
    }
}
