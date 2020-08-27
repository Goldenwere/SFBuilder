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
        #region Fields
        /**************/ public  Vector2            cursorSize;
#pragma warning disable 0649
        [SerializeField] private Sprite             cursor;
        [SerializeField] private ManagementCamera[] cameras;
#pragma warning restore 0649
        /**************/ private bool               drawCursor;
        #endregion

        #region Methods
        /// <summary>
        /// Set cursor state on Start
        /// </summary>
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            drawCursor = true;
        }

        /// <summary>
        /// Subscribe to events on Enable
        /// </summary>
        private void OnEnable()
        {
            foreach(ManagementCamera mc in cameras)
                mc.CameraMouseStateChanged += OnCameraStateChanged;

            GameEventSystem.SettingsUpdated += OnSettingsUpdated;
        }

        /// <summary>
        /// Unsubscribe from events on Disable
        /// </summary>
        private void OnDisable()
        {
            foreach (ManagementCamera mc in cameras)
                mc.CameraMouseStateChanged -= OnCameraStateChanged;

            GameEventSystem.SettingsUpdated -= OnSettingsUpdated;
        }

        /// <summary>
        /// Draw cursor on GUI
        /// </summary>
        private void OnGUI()
        {
            if (drawCursor)
            {
                Vector2 pos = Mouse.current.position.ReadValue();
                pos.y = Screen.height - pos.y;
                GUI.DrawTexture(new Rect(pos, cursorSize), cursor.texture);
            }
        }

        /// <summary>
        /// Handle cursor changes when controller uses mouse
        /// </summary>
        /// <param name="isMouseBeingUsed">Whether the mouse is currently being used (and thus whether the cursor should be hidden)</param>
        private void OnCameraStateChanged(bool isMouseBeingUsed)
        {
            if (GameEventSystem.Instance.CurrentGameState == GameState.Gameplay)
                drawCursor = !isMouseBeingUsed;

            if (drawCursor)
                Cursor.lockState = CursorLockMode.Confined;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Handle changes to settings
        /// </summary>
        private void OnSettingsUpdated()
        {
            
        }
        #endregion
    }
}