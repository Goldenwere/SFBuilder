using System.Collections.Generic;
using UnityEngine;

public class ProtoObjectRanger : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private MeshRenderer       rangerMeshRenderer;
    [SerializeField] private SphereCollider     rangerSphereCollider;
#pragma warning restore 0649
    /**************/ private int                objectBaseWorth;
    /**************/ private int                objectWorth;
    /**************/ private List<ProtoObject>  othersCollided;
    /**************/ private ProtoObject        parent;

    private void Start()
    {
        othersCollided = new List<ProtoObject>();
        parent = transform.parent.GetComponent<ProtoObject>();
        objectBaseWorth = ProtoObject.ScoreOfSingleType(parent.Type);
        objectWorth = objectBaseWorth;
        GameScoring.Instance.Potential = GameScoring.Instance.Score + objectWorth;
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
            GameScoring.Instance.Score += objectWorth;
            GameScoring.Instance.Potential = GameScoring.Instance.Score;
        }

        else
        {
            rangerMeshRenderer.enabled = true;
            rangerSphereCollider.enabled = true;
            GameScoring.Instance.Score -= objectWorth;
        }
    }

    private void Calculate()
    {
        int val = objectBaseWorth;
        foreach (ProtoObject po in othersCollided)
            val += ProtoObject.ScoreOfTwoTypes(parent.Type, po.Type);
        objectWorth = val;
        GameScoring.Instance.Potential = GameScoring.Instance.Score + objectWorth;
    }
}
