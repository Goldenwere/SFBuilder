using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoring : MonoBehaviour
{
    public GameScoring Instance { get; private set; }
    public int Score { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), string.Format("Score: {0}", Score), new GUIStyle { normal = new GUIStyleState { textColor = Color.magenta }, fontSize = 32 });
    }
}
