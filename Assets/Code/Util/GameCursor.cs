using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Goldenwere.Unity.Controller;
using System.Collections;

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
        [SerializeField] private ManagementCamera[] cameras;
        [SerializeField] private Sprite             cursor;
        [SerializeField] private float              cursorHideSqrVelocityThreshold = 1.0f;
        [SerializeField] private bool               restoreCursorPositionAfterShown = true;
#pragma warning restore 0649
        /**************/ private CursorLockMode     defaultLockMode;
        /**************/ private bool               drawCursor;
        /**************/ private bool               hardwareCursor;
        /**************/ private bool               isMouseBeingUsed;
        /**************/ private Vector2            prevMousePos;
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

            Cursor.lockState = defaultLockMode;
            drawCursor = true;
            OnSettingsUpdated();
        }

        /// <summary>
        /// Determines if cursor is over an interactable and toggles mouse controls accordingly for cameras; also, check to see if the cursor needs hidden
        /// </summary>
        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject() && cameras[0].controlMouseEnabled)
                foreach (ManagementCamera cam in cameras)
                    cam.controlMouseEnabled = false;
            else if (!EventSystem.current.IsPointerOverGameObject() && !cameras[0].controlMouseEnabled)
                foreach (ManagementCamera cam in cameras)
                    cam.controlMouseEnabled = true;

            // First, check if the camera is using mouse and the cursor is still visible
            if (isMouseBeingUsed && drawCursor)
            {
                // Second, only hide the cursor if the velocity is great enough
                if (cameras[0].CurrentCameraVelocity.sqrMagnitude >= cursorHideSqrVelocityThreshold)
                {
                    Cursor.visible = false;
                    drawCursor = false;
                    prevMousePos = Mouse.current.position.ReadValue();
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
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
            if (drawCursor && !hardwareCursor)
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
        private void OnCameraStateChanged(bool _isMouseBeingUsed)
        {
            if (GameEventSystem.Instance.CurrentGameState == GameState.Gameplay)
            {
                if (isMouseBeingUsed && !_isMouseBeingUsed)
                {
                    Cursor.lockState = defaultLockMode;
                    if (restoreCursorPositionAfterShown && !drawCursor)
                        StartCoroutine(WaitToRepositionCursor());
                    if (hardwareCursor)
                        Cursor.visible = true;
                    if (!restoreCursorPositionAfterShown)
                        drawCursor = true;
                }
                isMouseBeingUsed = _isMouseBeingUsed;
            }
        }

        /// <summary>
        /// Handle changes to settings
        /// </summary>
        private void OnSettingsUpdated()
        {
            StartCoroutine(WaitToSetCursorSize());
        }

        /// <summary>
        /// Wait for a fixed update before setting cursor size to prevent odd behaviour
        /// </summary>
        private IEnumerator WaitToSetCursorSize()
        {
            yield return new WaitForFixedUpdate();

            if (GameSettings.Instance.Settings.display_Cursor == CursorSize.Hardware)
            {
                hardwareCursor = true;
                Cursor.visible = true;
            }
            else
            {
                hardwareCursor = false;
                Cursor.visible = false;
                cursorSize = new Vector2(Screen.width / (((int)CursorSize.VeryLarge + 1 - (int)GameSettings.Instance.Settings.display_Cursor) * 16),
                    Screen.width / (((int)CursorSize.VeryLarge + 1 - (int)GameSettings.Instance.Settings.display_Cursor) * 16));
            }
        }

        /// <summary>
        /// Wait for a fixed update before setting cursor position
        /// </summary>
        private IEnumerator WaitToRepositionCursor()
        {
            yield return new WaitForFixedUpdate();
            Vector2 flipped = prevMousePos;
            flipped.y = Screen.height - prevMousePos.y;
#if UNITY_EDITOR
            flipped.y = prevMousePos.y;
#endif
            InputSystem.QueueDeltaStateEvent(Mouse.current.position, prevMousePos);
            Mouse.current.WarpCursorPosition(flipped);
            drawCursor = true;
        }
#endregion
    }
}