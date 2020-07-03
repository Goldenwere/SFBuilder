using UnityEngine;

public class ProtoButton : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private TMPro.TMP_Text     indicatorCount;
    [SerializeField] private TMPro.TMP_Text     indicatorID;
#pragma warning restore 0649
    /**************/ private int                associatedCount;
    /**************/ private int                associatedID;
    /**************/ private bool               initialized;
    /**************/ private ObjectPlacement    objPlacer;

    public void Initialize(int id, int count)
    {
        if (!initialized)
        {
            associatedCount = count;
            associatedID = id;
            indicatorID.text = ((ObjectType)id).ToString();
            indicatorCount.text = count.ToString();
            objPlacer = FindObjectOfType<ObjectPlacement>();
            initialized = true;
        }
    }

    public void OnButtonPress()
    {
        if (associatedCount > 0)
            objPlacer.OnObjectSelected(associatedID);
    }
}