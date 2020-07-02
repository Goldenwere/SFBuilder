using System.Collections.Generic;
using UnityEngine;

public class ProtoObjectRanger : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private MeshRenderer       rangerMeshRenderer;
    [SerializeField] private SphereCollider     rangerSphereCollider;
#pragma warning restore 0649
    /**************/ private List<ProtoObject>  othersCollided;
    /**************/ private ProtoObject        parent;
    /**************/ private int                objectHappiness;
    /**************/ private int                objectPower;
    /**************/ private int                objectSustenance;
    /**************/ private int                placedHappiness;
    /**************/ private int                placedPower;
    /**************/ private int                placedSustenance;

    private void Start()
    {
        othersCollided = new List<ProtoObject>();
        parent = transform.parent.GetComponent<ProtoObject>();
        ProtoObject.ScoreOfSingleType(parent.Type, out int hp, out int pp, out int sp);
        objectHappiness = hp;
        objectPower = pp;
        objectSustenance = sp;
        GameScoring.Instance.PotentialPower = pp;
        GameScoring.Instance.PotentialSustenance = sp;
        GameScoring.Instance.PotentialHappiness = hp;
    }

    private void OnTriggerEnter(Collider other)
    {
        ProtoObject otherProto = other.GetComponent<ProtoObject>();
        if (otherProto != null && otherProto != parent)
        {
            othersCollided.Add(otherProto);
            Calculate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ProtoObject otherProto = other.GetComponent<ProtoObject>();
        if (otherProto != null && otherProto != parent)
        {
            othersCollided.Remove(otherProto);
            Calculate();
        }
    }

    public void SetPlaced(bool placed)
    {
        if (placed)
        {
            rangerMeshRenderer.enabled = false;
            rangerSphereCollider.enabled = false;
            GameScoring.Instance.ApplyScore();
        }

        else
        {
            rangerMeshRenderer.enabled = true;
            rangerSphereCollider.enabled = true;
            GameScoring.Instance.RevokeScore(placedHappiness, placedPower, placedSustenance);
        }
    }

    private void Calculate()
    {
        placedHappiness = objectHappiness;
        placedPower = objectPower;
        placedSustenance = objectSustenance;

        foreach (ProtoObject po in othersCollided)
        {
            ProtoObject.ScoreOfTwoTypes(parent.Type, po.Type, out int workingHappiness, out int workingPower, out int workingSustenance);
            placedHappiness += workingHappiness;
            placedPower += workingPower;
            placedSustenance += workingSustenance;
        }

        GameScoring.Instance.PotentialHappiness = placedHappiness;
        GameScoring.Instance.PotentialPower = placedPower;
        GameScoring.Instance.PotentialSustenance = placedSustenance;
    }
}
