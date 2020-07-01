using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoObject : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private ProtoObjectGrounder    grounder;
    [SerializeField] private Material               materialNormal;
    [SerializeField] private Material               materialPlacing;
    [SerializeField] private MeshRenderer           objectBody;
#pragma warning restore 0649
    /**************/ private List<GameObject>   collidedObjects;
    /**************/ private bool               isPlaced;

    public bool IsCollided  { get; private set; }
    public bool IsGrounded  { get { return grounder.IsGrounded; } }
    public bool IsPlaced
    {
        get { return isPlaced; }
        set
        {
            isPlaced = value;
            if (isPlaced)
                objectBody.material = materialNormal;
            else
                objectBody.material = materialPlacing;
        }
    }
    public bool IsValid     { get { return IsGrounded && !IsCollided; } }

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
