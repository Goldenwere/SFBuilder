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
    [SerializeField] private ProtoObjectRanger      ranger;
    [SerializeField] private ObjectType             type;
#pragma warning restore 0649
    /**************/ private List<GameObject>       collidedObjects;
    /**************/ private bool                   isPlaced;

    public static int   BaseScoreNone       { get { return 00; } }
    public static int   BaseScoreBasic      { get { return 10; } }
    public static int   BaseScoreAdvanced   { get { return 20; } }

    public bool         IsCollided          { get; private set; }
    public bool         IsGrounded          { get { return grounder.IsGrounded; } }
    public bool         IsPlaced
    {
        get { return isPlaced; }
        set
        {
            isPlaced = value;
            if (isPlaced)
            {
                objectBody.material = materialNormal;
                grounder.enabled = false;
                ranger.SetPlaced(true);
                ranger.enabled = false;
            }
            else
            {
                objectBody.material = materialPlacing;
                grounder.enabled = true;
                ranger.SetPlaced(false);
                ranger.enabled = true;
            }
        }
    }
    public bool         IsValid             { get { return IsGrounded && !IsCollided; } }
    public ObjectType   Type                { get { return type; } }

    private void Start()
    {
        collidedObjects = new List<GameObject>(16);
    }

    private void Update()
    {
        if (!IsPlaced)
        {
            if (IsValid)
                objectBody.material.SetVector("_FirstOutlineColor", new Vector4(0.04f, 1, 0.08f, 0.5f));
            else
                objectBody.material.SetVector("_FirstOutlineColor", new Vector4(0.57f, 0, 0, 0.5f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Ranger")
        {
            IsCollided = true;
            collidedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name != "Ranger")
        {
            collidedObjects.Remove(other.gameObject);
            if (collidedObjects.Count == 0)
                IsCollided = false;
        }
    }

    public static int ScoreOfSingleType(ObjectType type)
    {
        switch(type)
        {
            case ObjectType.PrototypeA:
                return BaseScoreNone;
            case ObjectType.PrototypeB:
                return BaseScoreAdvanced;
            case ObjectType.PrototypeC:
                return BaseScoreNone;
        }
        return 0;
    }

    public static int ScoreOfTwoTypes(ObjectType toBePlaced, ObjectType existing)
    {
        switch (toBePlaced)
        {
            case ObjectType.PrototypeA:
                switch (existing)
                {
                    case ObjectType.PrototypeA:
                        return BaseScoreBasic;
                    case ObjectType.PrototypeB:
                        return BaseScoreNone;
                    case ObjectType.PrototypeC:
                        return BaseScoreNone;
                }
                break;
            case ObjectType.PrototypeB:
                switch (existing)
                {
                    case ObjectType.PrototypeA:
                        return -BaseScoreBasic;
                    case ObjectType.PrototypeB:
                        return BaseScoreNone;
                    case ObjectType.PrototypeC:
                        return BaseScoreNone;
                }
                break;
            case ObjectType.PrototypeC:
                switch (existing)
                {
                    case ObjectType.PrototypeA:
                        return BaseScoreAdvanced;
                    case ObjectType.PrototypeB:
                        return -BaseScoreBasic;
                    case ObjectType.PrototypeC:
                        return BaseScoreNone;
                }
                break;
        }
        return 0;
    }
}

public enum ObjectType
{
    /// <summary>
    /// Gives 0 score, gives 10 score if in range of other ptA's
    /// </summary>
    PrototypeA,
    /// <summary>
    /// Gives 20 score, takes 10 away for all ptA's nearby
    /// </summary>
    PrototypeB,
    /// <summary>
    /// Gives 0 score, takes 10 away for any ptB's and gives 20 for any ptA's in range
    /// </summary>
    PrototypeC
}