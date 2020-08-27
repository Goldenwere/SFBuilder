using UnityEngine;
using UnityEngine.InputSystem;
using Goldenwere.Unity.Controller;

namespace SFBuilder.Util
{
    /// <summary>
    /// Handles the UI and positioning of the game cursor
    /// </summary>
    public class GameCursor : MonoBehaviour
    {
        /**************/ public  Vector2    cursorSize;
#pragma warning disable 0649
        [SerializeField] private Sprite     cursor;
#pragma warning restore 0649

        private void Start()
        {
            Cursor.visible = false;
        }

        private void OnGUI()
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            pos.y = Screen.height - pos.y;
            GUI.DrawTexture(new Rect(pos, cursorSize), cursor.texture);
        }
    }
}