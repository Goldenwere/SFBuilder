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
        /*
        objectBaseWorth = ProtoObject.ScoreOfSingleType(parent.Type);
        objectWorth = objectBaseWorth;
        GameScoring.Instance.Potential = GameScoring.Instance.Score + objectWorth;
        */
        ProtoObject.ScoreOfSingleType(parent.Type, out int pp, out int sp, out int hp);
        objectHappiness = hp;
        objectPower = pp;
        objectSustenance = sp;
        GameScoring.Instance.PowerPotential = pp;
        GameScoring.Instance.SustenancePotential = sp;
        GameScoring.Instance.HappinessPotential = hp;
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
            GameScoring.Instance.RevokeScore(placedPower, placedSustenance, placedHappiness);
        }
    }

    private void Calculate()
    {
        placedHappiness = objectHappiness;
        placedPower = objectPower;
        placedSustenance = objectSustenance;

        foreach (ProtoObject po in othersCollided)
        {
            ProtoObject.ScoreOfTwoTypes(parent.Type, po.Type, out int workingPower, out int workingSustenance, out int workingHappiness);
            placedHappiness += workingHappiness;
            placedPower += workingPower;
            placedSustenance += workingSustenance;
        }

        GameScoring.Instance.HappinessPotential = placedHappiness;
        GameScoring.Instance.PowerPotential = placedPower;
        GameScoring.Instance.SustenancePotential = placedSustenance;
    }
}
