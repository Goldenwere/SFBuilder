using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;
using Goldenwere.Unity;
using Goldenwere.Unity.Controller;
using SFBuilder.Gameplay;

namespace SFBuilder.Obj
{
    /// <summary>
    /// The PlacementSystem allows the player to place BuilderObjects and update related Gameplay systems as needed
    /// </summary>
    public class PlacementSystem : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private GodGameCamera                  gameCam;
        [SerializeField] private float                          positionPrecision;
        [SerializeField] private BuilderObjectTypeToPrefab[]    prefabs;
        [SerializeField] private int                            prefabUndoMaxCount;
        [SerializeField] private float                          rotationAngleMagnitude;
#pragma warning restore 0649
        /**************/ private bool                           isPlacing;
        /**************/ private bool                           prefabHadFirstHit;
        /**************/ private BuilderObject                  prefabInstance;
        /**************/ private LinkedList<BuilderObject>      prefabsPlaced;
        /**************/ private float                          workingLastRotation;
        /**************/ private bool                           workingModifierMouseZoom;
        #endregion
        #region Properties
        public static PlacementSystem   Instance    { get; private set; }
        public bool IsPlacing
        {
            get { return isPlacing; }
            set
            {
                if (value != isPlacing)
                    GameEventSystem.Instance.UpdatePlacementState(value);
                isPlacing = value;

                if (isPlacing)
                {
                    gameCam.CameraModifiersAreEnabled = false;
                }

                else
                {
                    gameCam.CameraModifiersAreEnabled = true;
                    prefabHadFirstHit = false;
                    prefabInstance = null;
                }
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// Set singleton on Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        /// <summary>
        /// Instantiate list on Start
        /// </summary>
        private void Start()
        {
            prefabsPlaced = new LinkedList<BuilderObject>();
            gameCam.CameraModifiersAreEnabled = true;

            if (GameEventSystem.Instance.CurrentGameState != GameState.Gameplay)
                gameCam.cameraMotionIsFrozen = true;
            else
                gameCam.cameraMotionIsFrozen = false;
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
            GoalSystem.newGoal += OnNewGoal;
            GameEventSystem.LevelBanished += OnLevelBanished;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
            GoalSystem.newGoal -= OnNewGoal;
            GameEventSystem.LevelBanished -= OnLevelBanished;
        }

        /// <summary>
        /// Lerp or set the instance's position based on cursor position
        /// </summary>
        /// <remarks>(may eventually have a gamepad cursor for gamepad support, otherwise will just rely on camera movement in order to move a BuilderObject's position)</remarks>
        private void Update()
        {
            if (isPlacing && Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 1000f))
            {
                if (prefabHadFirstHit)
                    prefabInstance.transform.position = Vector3.Lerp(prefabInstance.transform.position, hit.point.ToPrecision(positionPrecision, true, false, true), Time.deltaTime * 25);

                else
                {
                    prefabInstance.transform.position = hit.point.ToPrecision(positionPrecision, true, false, true);
                    prefabHadFirstHit = true;
                }
            }
        }

        /// <summary>
        /// On the Menu input event, load the menu
        /// </summary>
        /// <param name="context"></param>
        public void OnMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (isPlacing)
                {
                    Destroy(prefabInstance.gameObject);
                    IsPlacing = false;
                }

                else
                    GameEventSystem.Instance.UpdateGameState(GameState.MainMenus);
            }
        }

        /// <summary>
        /// When a BuilderObject is selected (UI.Button press), spawn an instance of it
        /// </summary>
        /// <param name="id">The id of the BuilderObject to instantiate</param>
        /// <returns>The instance of the BuilderObject (for the UI.Button to track via event subscription)</returns>
        public BuilderObject OnObjectSelected(int id)
        {
            if (!isPlacing)
            {
                prefabInstance = Instantiate(prefabs.First(p => p.type == (ObjectType)id).prefab).GetComponent<BuilderObject>();
                prefabInstance.transform.Rotate(Vector3.up, workingLastRotation);
                prefabInstance.IsPlaced = false;
                IsPlacing = true;
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Button);
                return prefabInstance;
            }

            return null;
        }

        /// <summary>
        /// On the ObjectRotation input event, rotate a BuilderObject as long as the zoom modifier isn't toggled
        /// </summary>
        /// <param name="context">The related context to the input event</param>
        public void OnObjectRotation(InputAction.CallbackContext context)
        {
            if (!workingModifierMouseZoom && context.performed && isPlacing)
            {
                if (context.ReadValue<float>() > 0)
                {
                    prefabInstance.transform.Rotate(Vector3.up, -rotationAngleMagnitude);
                    workingLastRotation -= rotationAngleMagnitude;
                    if (workingLastRotation < -360)
                        workingLastRotation += 360;
                }
                else
                {
                    prefabInstance.transform.Rotate(Vector3.up, rotationAngleMagnitude);
                    workingLastRotation += rotationAngleMagnitude;
                    if (workingLastRotation > 360)
                        workingLastRotation += 360;
                }
            }
        }

        /// <summary>
        /// On the Placement input event, place a BuilderObject if its instance IsValid
        /// </summary>
        /// <param name="context">The related context to the input event</param>
        public void OnPlacement(InputAction.CallbackContext context)
        {
            if (context.performed && isPlacing && prefabInstance.IsValid)
            {
                GameSave.Instance.AddBuilderObject(prefabInstance.transform.position, prefabInstance.transform.rotation, prefabInstance.Type);
                prefabInstance.IsPlaced = true;
                prefabInstance.transform.position = prefabInstance.transform.position.ToPrecision(positionPrecision, true, false, true);
                prefabsPlaced.AddFirst(prefabInstance);
                if (prefabsPlaced.Count > prefabUndoMaxCount)
                    prefabsPlaced.RemoveLast();
                IsPlacing = false;
                GoalSystem.Instance.VerifyForNextGoal();
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Placement);
            }
        }

        /// <summary>
        /// On the Undo input event, undo a previously placed BuilderObject
        /// </summary>
        /// <param name="context">The related context to the input event</param>
        public void OnUndo(InputAction.CallbackContext context)
        {
            if (context.performed && prefabsPlaced.Count > 0)
            {
                if (isPlacing)
                    Destroy(prefabInstance.gameObject);
                IsPlacing = true;
                prefabHadFirstHit = true;
                prefabInstance = prefabsPlaced.First.Value;
                prefabsPlaced.RemoveFirst();
                prefabInstance.IsPlaced = false;
                GoalSystem.Instance.VerifyForNextGoal();
                GameAudioSystem.Instance.PlaySound(AudioClipDefinition.Undo);
                GameSave.Instance.PopBuilderObject();
            }
        }

        /// <summary>
        /// On the ZoomMouseModifier input event, track the modifier (to prevent rotating objects while zooming)
        /// </summary>
        /// <param name="context">The related context to the input event</param>
        public void OnZoomMouseModifier(InputAction.CallbackContext context)
        {
            if (gameCam.settingModifiersAreToggled)
                workingModifierMouseZoom = !workingModifierMouseZoom;
            else
                workingModifierMouseZoom = context.performed;
        }

        /// <summary>
        /// Handler for the GameStateChanged event
        /// </summary>
        /// <param name="prevState">The previous GameState</param>
        /// <param name="newState">The new GameState</param>
        private void OnGameStateChanged(GameState prevState, GameState newState)
        {
            if (newState != GameState.Gameplay)
                gameCam.cameraMotionIsFrozen = true;
            else
                gameCam.cameraMotionIsFrozen = false;
        }

        /// <summary>
        /// On the LevelBanished event, reset the PlacementSystem
        /// </summary>
        private void OnLevelBanished()
        {
            prefabsPlaced.Clear();
        }

        /// <summary>
        /// On the NewGoal event, clear the prefabs placed list to prevent undoing past-goal BuilderObjects
        /// </summary>
        /// <param name="newGoal">The new goal level (unused by this method)</param>
        private void OnNewGoal(int newGoal)
        {
            prefabsPlaced.Clear();
        }
        #endregion
    }
}
