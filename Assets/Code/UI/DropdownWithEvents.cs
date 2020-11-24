using Goldenwere.Unity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownWithEvents : TMP_Dropdown
{
    protected override GameObject CreateDropdownList(GameObject template)
    {
        GameObject created = base.CreateDropdownList(template);
        StartCoroutine(WaitBeforeSelection());
        return created;
    }

    private IEnumerator WaitBeforeSelection()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(gameObject.FindChild("Dropdown List").FindChildRecursively("Content").transform.GetChild(1).gameObject);
    }
}
