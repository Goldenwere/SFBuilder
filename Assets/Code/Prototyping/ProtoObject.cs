using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ProtoObjectDelegate(ProtoObject obj);

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
                objectPlaced?.Invoke(this);
            }
            else
            {
                objectBody.material = materialPlacing;
                grounder.enabled = true;
                ranger.SetPlaced(false);
                ranger.enabled = true;
                objectRecalled?.Invoke(this);
            }
        }
    }
    public bool         IsValid             { get { return IsGrounded && !IsCollided; } }
    public ObjectType   Type                { get { return type; } }

    public ProtoObjectDelegate objectPlaced;
    public ProtoObjectDelegate objectRecalled;

    private void Start()
    {
        collidedObjects = new List<GameObject>(16);
    }

    private void Update()
    {
        if (!IsPlaced)
        {
            for (int i = collidedObjects.Count - 1; i >= 0; i--)
                if (collidedObjects[i] == null)
                    collidedObjects.RemoveAt(i);
            if (collidedObjects.Count < 1)
                IsCollided = false;

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

    public static void ScoreOfSingleType(ObjectType type, out int happiness, out int power, out int sustenance)
    {
        happiness = 0;
        power = 0;
        sustenance = 0;

        switch (type)
        {
            case ObjectType.PrototypeA:
                happiness = 0;
                power = 3;
                sustenance = 0;
                break;
            case ObjectType.PrototypeB:
                happiness = 0;
                power = -5;
                sustenance = -5;
                break;
            case ObjectType.PrototypeC:
                happiness = 0;
                power = 0;
                sustenance = 5;
                break;
        }
    }

    public static void ScoreOfTwoTypes(ObjectType toBePlaced, ObjectType existing, out int happiness, out int power, out int sustenance)
    {
        happiness = 0;
        power = 0;
        sustenance = 0;

        switch (toBePlaced)
        {
            case ObjectType.PrototypeA:
                switch (existing)
                {
                    case ObjectType.PrototypeA:
                        happiness = 0;
                        power = 3;
                        sustenance = 0;
                        break;
                    case ObjectType.PrototypeB:
                        happiness = -5;
                        power = 0;
                        sustenance = 0;
                        break;
                    case ObjectType.PrototypeC:
                        happiness = 0;
                        power = 0;
                        sustenance = 0;
                        break;
                }
                break;
            case ObjectType.PrototypeB:
                switch (existing)
                {
                    case ObjectType.PrototypeA:
                        happiness = 0;
                        power = 0;
                        sustenance = 0;
                        break;
                    case ObjectType.PrototypeB:
                        happiness = 0;
                        power = 0;
                        sustenance = 0;
                        break;
                    case ObjectType.PrototypeC:
                        happiness = 0;
                        power = 0;
                        sustenance = 0;
                        break;
                }
                break;
            case ObjectType.PrototypeC:
                switch (existing)
                {
                    case ObjectType.PrototypeA:
                        happiness = 0;
                        power = 0;
                        sustenance = 0;
                        break;
                    case ObjectType.PrototypeB:
                        happiness = 3;
                        power = 0;
                        sustenance = 3;
                        break;
                    case ObjectType.PrototypeC:
                        happiness = 0;
                        power = 0;
                        sustenance = 0;
                        break;
                }
                break;
        }
    }
}

public enum ObjectType
{
    // Ranges defined/reserved:
    // 0-31: prototyping
    /// <summary>
    /// pA is a power source, giving 3 power; -5 happiness for each pB in range, +3 power for each pA in range
    /// </summary>
    PrototypeA = 0,
    /// <summary>
    /// pB is a residence, taking -5 sustenance and -5 power; no change for buildings in range
    /// </summary>
    PrototypeB = 1,
    /// <summary>
    /// pC is a community garden, giving 5 base sustenance; gives 3 sustenance and 3 happiness for each pB in range
    /// </summary>
    PrototypeC = 2
}