using UnityEngine;

public class ProtoObjectRanger : MonoBehaviour
{
    [SerializeField] private MeshRenderer   rangerMeshRenderer;
    [SerializeField] private SphereCollider rangerSphereCollider;

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
