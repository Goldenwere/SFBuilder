using Goldenwere.Unity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Override dropdowns in order to make them keyboard navigable
/// </summary>
/// <remarks>Dropdowns are normally navigable, but in making other things navigable, it breaks this</remarks>
public class DropdownWithEvents : TMP_Dropdown
{
    /// <summary>
    /// Override for CreateDropdownList in order to select the itme
    /// </summary>
    /// <param name="template">The template to create the list from</param>
    /// <returns>The created dropdown list</returns>
    protected override GameObject CreateDropdownList(GameObject template)
    {
        GameObject created = base.CreateDropdownList(template);
        StartCoroutine(WaitBeforeSelection());
        return created;
    }

    /// <summary>
    /// Selects the first item in the list after waiting until end of frame (so that the list is properly instantiated)
    /// </summary>
    private IEnumerator WaitBeforeSelection()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(gameObject.FindChild("Dropdown List").FindChildRecursively("Content").transform.GetChild(1).gameObject);
    }
}
