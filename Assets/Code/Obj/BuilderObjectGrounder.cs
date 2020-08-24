using System.Collections.Generic;
using UnityEngine;

namespace SFBuilder.Obj
{
    /// <summary>
    /// The grounder is used for ensuring that a BuilderObject will be placed on fairly flat ground fully containing the building's grounder
    /// </summary>
    public class BuilderObjectGrounder : MonoBehaviour
    {
        /// <summary>
        /// Utility class for keeping track of the four corners of the grounder
        /// </summary>
        protected class GrounderFace
        {
            public Vector3 CornerA;
            public Vector3 CornerB;
            public Vector3 CornerC;
            public Vector3 CornerD;
        }

        #region Fields
#pragma warning disable 0649
        [SerializeField] private BoxCollider    attachedCollider;
#pragma warning restore 0649
        /**************/ private List<Collider> collidedObjects;
        /**************/ private GrounderFace   currentFacePosition;
        /**************/ private const float    rayLength = 0.25f;
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
            currentFacePosition = new GrounderFace();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(currentFacePosition.CornerA, 0.5f);
            Gizmos.DrawSphere(currentFacePosition.CornerB, 0.5f);
            Gizmos.DrawSphere(currentFacePosition.CornerC, 0.5f);
            Gizmos.DrawSphere(currentFacePosition.CornerD, 0.5f);
        }

        /// <summary>
        /// Update the current face position each frame
        /// </summary>
        private void Update()
        {
            currentFacePosition.CornerA = attachedCollider.bounds.center + new Vector3(attachedCollider.size.x, -attachedCollider.size.y, attachedCollider.size.z) * 0.5f;
            currentFacePosition.CornerB = attachedCollider.bounds.center + new Vector3(-attachedCollider.size.x, -attachedCollider.size.y, attachedCollider.size.z) * 0.5f;
            currentFacePosition.CornerC = attachedCollider.bounds.center + new Vector3(-attachedCollider.size.x, -attachedCollider.size.y, -attachedCollider.size.z) * 0.5f;
            currentFacePosition.CornerD = attachedCollider.bounds.center + new Vector3(attachedCollider.size.x, -attachedCollider.size.y, -attachedCollider.size.z) * 0.5f;
        }

        /// <summary>
        /// Check if grounded each physics frame
        /// </summary>
        private void FixedUpdate()
        {
            IsGrounded = 
                Physics.Raycast(new Ray(currentFacePosition.CornerA, Vector3.down), rayLength) &&
                Physics.Raycast(new Ray(currentFacePosition.CornerB, Vector3.down), rayLength) &&
                Physics.Raycast(new Ray(currentFacePosition.CornerC, Vector3.down), rayLength) &&
                Physics.Raycast(new Ray(currentFacePosition.CornerD, Vector3.down), rayLength);
        }

        /// <summary>
        /// When something enters the grounder, keep track of it
        /// </summary>
        /// <param name="other">The other collider that entered</param>
        private void OnTriggerEnter(Collider other)
        {
            if (collidedObjects == null)
                collidedObjects = new List<Collider>(16);
            if (other.name != "Ranger")
                collidedObjects.Add(other);
        }

        /// <summary>
        /// When something exits the grounder, don't keep track of it anymore
        /// </summary>
        /// <param name="other">The other collider that exited</param>
        private void OnTriggerExit(Collider other)
        {
            if (collidedObjects == null)
                collidedObjects = new List<Collider>(16);
            if (other.name != "Ranger")
                collidedObjects.Remove(other);
        }
        #endregion
    }
}
