using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoObjectGrounder : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private BoxCollider    protoCollider;
#pragma warning restore 0649
    /**************/ private List<Collider> collidedObjects;

    public bool IsGrounded { get; private set; }

    private void Start()
    {
        collidedObjects = new List<Collider>(16);
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

    private void Update()
    {
        bool foundGround = false;
        if (collidedObjects.Count > 0)
            foreach (Collider obj in collidedObjects)
                if (obj.bounds.Contains(protoCollider.bounds.max) && obj.bounds.Contains(protoCollider.bounds.min))
                    foundGround = true;
        IsGrounded = foundGround;
    }
}
