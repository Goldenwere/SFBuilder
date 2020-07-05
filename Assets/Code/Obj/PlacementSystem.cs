using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
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
        [SerializeField] private GodGameCamera              gameCam;
        [SerializeField] private GameObject[]               prefabs;
        [SerializeField] private int                        prefabUndoMaxCount;
        [SerializeField] private GameObject                 prototypeCanvas;
        [SerializeField] private float                      rotationAngleMagnitude;
#pragma warning restore 0649
        /**************/ private bool                       isPlacing;
        /**************/ private bool                       prefabHadFirstHit;
        /**************/ private BuilderObject              prefabInstance;
        /**************/ private LinkedList<BuilderObject>  prefabsPlaced;
        /**************/ private bool                       workingModifierMouseZoom;
        #endregion
        #region Properties
        public static PlacementSystem   Instance    { get; private set; }
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
        /// Instantiate list on Start and subscribe to the newGoal event
        /// </summary>
        private void Start()
        {
            prefabsPlaced = new LinkedList<BuilderObject>();
            GoalSystem.newGoal += OnNewGoal;
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
                    prefabInstance.transform.position = Vector3.Lerp(prefabInstance.transform.position, hit.point, Time.deltaTime * 25);

                else
                {
                    prefabInstance.transform.position = hit.point;
                    prefabHadFirstHit = true;
                }
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
                prefabInstance = Instantiate(prefabs[id]).GetComponent<BuilderObject>();
                prefabHadFirstHit = false;
                prefabInstance.IsPlaced = false;
                isPlacing = true;
                prototypeCanvas.SetActive(false);
                return prefabInstance;
            }

            return null;
        }

        /// <summary>
        /// On the ObjectRotation input event, rotate a BuilderObject as long as the zoom modifier isn't toggled
        /// </summary>
        /// <param name="context"></param>
        public void OnObjectRotation(InputAction.CallbackContext context)
        {
            if (!workingModifierMouseZoom && context.performed && isPlacing)
                if (context.ReadValue<float>() > 0)
                    prefabInstance.transform.Rotate(Vector3.up, -rotationAngleMagnitude);
                else
                    prefabInstance.transform.Rotate(Vector3.up, rotationAngleMagnitude);
        }

        /// <summary>
        /// On the Placement input event, place a BuilderObject if its instance IsValid
        /// </summary>
        /// <param name="context">The related context to the input event</param>
        public void OnPlacement(InputAction.CallbackContext context)
        {
            if (context.performed && isPlacing && prefabInstance.IsValid)
            {
                prefabInstance.IsPlaced = true;
                prefabsPlaced.AddFirst(prefabInstance);
                if (prefabsPlaced.Count > prefabUndoMaxCount)
                    prefabsPlaced.RemoveLast();
                prefabInstance = null;
                isPlacing = false;
                prefabHadFirstHit = false;
                prototypeCanvas.SetActive(true);
                GoalSystem.Instance.VerifyForNextGoal();
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
                isPlacing = true;
                prefabHadFirstHit = true;
                prefabInstance = prefabsPlaced.First.Value;
                prefabsPlaced.RemoveFirst();
                prefabInstance.IsPlaced = false;
                prototypeCanvas.SetActive(false);
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
