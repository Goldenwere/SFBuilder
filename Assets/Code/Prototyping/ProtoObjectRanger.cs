using UnityEngine;

public class ProtoObjectRanger : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private MeshRenderer   rangerMeshRenderer;
    [SerializeField] private SphereCollider rangerSphereCollider;
#pragma warning restore 0649

    public void SetPlaced(bool placed)
    {
        if (placed)
        {
            rangerMeshRenderer.enabled = false;
            rangerSphereCollider.enabled = false;
            
        }

        else
        {
            rangerMeshRenderer.enabled = true;
            rangerSphereCollider.enabled = true;
        }
    }
}
