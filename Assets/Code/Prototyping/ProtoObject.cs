using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoObject : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private ProtoObjectGrounder grounder;
#pragma warning restore 0649
    /**************/ private List<GameObject> collidedObjects;

    public bool IsGrounded { get { return grounder.IsGrounded; } }
    public bool IsCollided { get; private set; }

    private void Start()
    {
        collidedObjects = new List<GameObject>(16);
    }

    private void OnTriggerEnter(Collider other)
    {
        IsCollided = true;
        collidedObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        collidedObjects.Remove(other.gameObject);
        if (collidedObjects.Count == 0)
            IsCollided = false;
    }
}
