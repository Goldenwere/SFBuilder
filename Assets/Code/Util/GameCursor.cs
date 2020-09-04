using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
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
        /**************/ private CursorLockMode     defaultLockMode;
        #endregion

        #region Methods
        /// <summary>
        /// Set cursor state on Start
        /// </summary>
        private void Start()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
                defaultLockMode = CursorLockMode.Confined;
            else
                defaultLockMode = CursorLockMode.None;

            Cursor.visible = false;
            Cursor.lockState = defaultLockMode;
            drawCursor = true;
        }

        /// <summary>
        /// Determines if cursor is over an interactable and toggles mouse controls accordingly for cameras
        /// </summary>
        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject() && cameras[0].controlMouseEnabled)
                foreach (ManagementCamera cam in cameras)
                    cam.controlMouseEnabled = false;
            else if (!EventSystem.current.IsPointerOverGameObject() && !cameras[0].controlMouseEnabled)
                foreach (ManagementCamera cam in cameras)
                    cam.controlMouseEnabled = true;
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
                Cursor.lockState = defaultLockMode;
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