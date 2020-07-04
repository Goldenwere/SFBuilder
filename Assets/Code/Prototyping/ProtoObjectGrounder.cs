using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFBuilder.Prototyping
{
    public class ProtoObjectGrounder : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private BoxCollider protoCollider;
#pragma warning restore 0649
        /**************/
        private List<Collider> collidedObjects;

        public bool IsGrounded { get; private set; }

        private void Start()
        {
            collidedObjects = new List<Collider>(16);
        }

        private void Update()
        {
            bool foundGround = false;
            for (int i = collidedObjects.Count - 1; i >= 0; i--)
            {
                if (collidedObjects[i] == null)
                    collidedObjects.RemoveAt(i);
                else if (collidedObjects[i].bounds.Contains(protoCollider.bounds.max) && collidedObjects[i].bounds.Contains(protoCollider.bounds.min))
                    foundGround = true;
            }
            IsGrounded = foundGround;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name != "Ranger")
                collidedObjects.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.name != "Ranger")
                collidedObjects.Remove(other);
        }
    }
}