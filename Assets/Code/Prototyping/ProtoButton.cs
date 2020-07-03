using System;
using System.Linq;
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
    /**************/ private bool               isRequired;
    /**************/ private ObjectPlacement    objPlacer;

    public void Initialize(int id, int count, bool required)
    {
        if (!initialized)
        {
            associatedCount = count;
            associatedID = id;
            isRequired = required;
            indicatorID.text = ProtoObject.NameOfType((ObjectType)id);
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

        if (isRequired) 
        {
            int i = Array.IndexOf(
                ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalRequirements, 
                ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalRequirements.First(g => g.goalStructureID == associatedID));
            ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalRequirements[i].goalStructureCount--;
        }
        else
        {
            if (isRequired)
            {
                int i = Array.IndexOf(
                    ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalExtras,
                    ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalExtras.First(g => g.goalStructureID == associatedID));
                ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalExtras[i].goalStructureCount--;
            }
        }
    }

    private void OnObjectRevoked(ProtoObject obj)
    {
        associatedCount++;
        indicatorCount.text = associatedCount.ToString();
        if (associatedCount > 0 && !button.interactable)
            button.interactable = true;

        if (isRequired)
        {
            int i = Array.IndexOf(
                ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalRequirements,
                ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalRequirements.First(g => g.goalStructureID == associatedID));
            ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalRequirements[i].goalStructureCount++;
        }
        else
        {
            if (isRequired)
            {
                int i = Array.IndexOf(
                    ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalExtras,
                    ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalExtras.First(g => g.goalStructureID == associatedID));
                ProtoGoalSystem.Instance.CurrentGoalWorkingSet.goalExtras[i].goalStructureCount++;
            }
        }
    }
}