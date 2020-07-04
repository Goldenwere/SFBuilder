using System.Collections.Generic;
using UnityEngine;

namespace SFBuilder.Obj
{
    /// <summary>
    /// The grounder is used for ensuring that a BuilderObject will be placed on fairly flat ground fully containing the building's grounder
    /// </summary>
    public class BuilderObjectGrounder : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private BoxCollider    attachedCollider;
#pragma warning restore 0649
        /**************/ private List<Collider> collidedObjects;
        #endregion
        #region Methods
        /// <summary>
        /// Whether or not the object is gounded (i.e. its grounder is fully contained within a collider, indicating mostly even ground)
        /// </summary>
        public bool IsGrounded  { get; private set; }

        /// <summary>
        /// Instantiate working list on Start
        /// </summary>
        private void Start()
        {
            collidedObjects = new List<Collider>(16);
        }

        /// <summary>
        /// Check if grounded each frame
        /// </summary>
        private void Update()
        {
            bool foundGround = false;
            for (int i = collidedObjects.Count - 1; i >= 0; i--)
            {
                if (collidedObjects[i] == null)
                    collidedObjects.RemoveAt(i);
                else if (collidedObjects[i].bounds.Contains(attachedCollider.bounds.max) && collidedObjects[i].bounds.Contains(attachedCollider.bounds.min))
                    foundGround = true;
            }
            IsGrounded = foundGround;
        }

        /// <summary>
        /// When something enters the grounder, keep track of it
        /// </summary>
        /// <param name="other">The other collider that entered</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.name != "Ranger")
                collidedObjects.Add(other);
        }

        /// <summary>
        /// When something exits the grounder, don't keep track of it anymore
        /// </summary>
        /// <param name="other">The other collider that exited</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.name != "Ranger")
                collidedObjects.Remove(other);
        }
        #endregion
    }
}
