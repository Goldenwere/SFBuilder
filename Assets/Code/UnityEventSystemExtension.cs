using UnityEngine;
using UnityEngine.EventSystems;

namespace SFBuilder
{
    public delegate void PreviousCurrentGameObject(GameObject prev, GameObject curr);

    /// <summary>
    /// Extends UnityEngine.EventSystem because it doesn't have any sort of event for changes in currentSelectedObject
    /// </summary>
    /// <remarks>Thanks Unity very cool</remarks>
    public class UnityEventSystemExtension : EventSystem
    {
        private GameObject                              previousSelectedObject;
        public static event PreviousCurrentGameObject   SelectedGameObjectChanged;
        
        /// <summary>
        /// Invokes the SelectedGameObjectChanged event when previous != current
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (previousSelectedObject != currentSelectedGameObject)
                SelectedGameObjectChanged?.Invoke(previousSelectedObject, currentSelectedGameObject);

            previousSelectedObject = currentSelectedGameObject;
        }
    }
}