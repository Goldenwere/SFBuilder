using System.Collections.Generic;
using Goldenwere.Unity;
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
        /**************/ private GrounderFace   currentFacePosition;
        /**************/ private const float    rayLength = 0.25f;
        #endregion
        #region Methods
        /// <summary>
        /// Whether or not the object is gounded (i.e. its grounder is fully contained within a collider, indicating mostly even ground)
        /// </summary>
        public bool IsGrounded  { get; private set; }

        /// <summary>
        /// Instantiate current face position on Start
        /// </summary>
        private void Start()
        {
            currentFacePosition = new GrounderFace();
        }

        /// <summary>
        /// Update the current face position each frame
        /// </summary>
        private void Update()
        {
            currentFacePosition.CornerA = attachedCollider.transform.TransformPoint(attachedCollider.center + new Vector3(attachedCollider.size.x, -attachedCollider.size.y, attachedCollider.size.z) * 0.5f);
            currentFacePosition.CornerB = attachedCollider.transform.TransformPoint(attachedCollider.center + new Vector3(-attachedCollider.size.x, -attachedCollider.size.y, attachedCollider.size.z) * 0.5f);
            currentFacePosition.CornerC = attachedCollider.transform.TransformPoint(attachedCollider.center + new Vector3(-attachedCollider.size.x, -attachedCollider.size.y, -attachedCollider.size.z) * 0.5f);
            currentFacePosition.CornerD = attachedCollider.transform.TransformPoint(attachedCollider.center + new Vector3(attachedCollider.size.x, -attachedCollider.size.y, -attachedCollider.size.z) * 0.5f);
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
        #endregion
    }
}
