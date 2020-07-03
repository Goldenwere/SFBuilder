using UnityEngine;
using UnityEngine.UI;

public class ProtoButton : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Button             button;
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
        {
            ProtoObject spawned = objPlacer.OnObjectSelected(associatedID);
            if (spawned != null)
            {
                spawned.objectPlaced += OnObjectPlaced;
                spawned.objectRecalled += OnObjectRevoked;
            }
        }
    }

    private void OnObjectPlaced(ProtoObject obj)
    {
        associatedCount--;
        indicatorCount.text = associatedCount.ToString();
        if (associatedCount <= 0)
            button.interactable = false;
    }

    private void OnObjectRevoked(ProtoObject obj)
    {
        associatedCount++;
        indicatorCount.text = associatedCount.ToString();
        if (associatedCount > 0 && !button.interactable)
            button.interactable = true;
    }
}