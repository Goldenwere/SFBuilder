using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoObject : MonoBehaviour
{
    public  bool                IsCollided { get; private set; }
    private List<GameObject>    collidedObjects;

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
