using Goldenwere.Unity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Override dropdowns in order to make them keyboard navigable
/// </summary>
/// <remarks>Dropdowns are normally navigable, but in making other things navigable, it breaks this</remarks>
public class KeyboardNavigableDropdown : TMP_Dropdown
{
    /// <summary>
    /// Override for CreateDropdownList in order to select the item
    /// </summary>
    /// <param name="template">The template to create the list from</param>
    /// <returns>The created dropdown list</returns>
    protected override GameObject CreateDropdownList(GameObject template)
    {
        GameObject created = base.CreateDropdownList(template);
        StartCoroutine(WaitBeforeSelection(false));
        return created;
    }

    /// <summary>
    /// Override for DestroyDropdownList in order to select the dropdown
    /// </summary>
    /// <param name="dropdownList">The dropdown list to destroy</param>
    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        base.DestroyDropdownList(dropdownList);
        StartCoroutine(WaitBeforeSelection(true));
    }

    /// <summary>
    /// Selects the first item in the list after waiting until end of frame (so that the list is properly instantiated)
    /// </summary>
    /// <param name="selectList"></param>
    private IEnumerator WaitBeforeSelection(bool selectList)
    {
        yield return new WaitForEndOfFrame();
        if (selectList)
            EventSystem.current.SetSelectedGameObject(gameObject);
        else
            EventSystem.current.SetSelectedGameObject(gameObject.FindChild("Dropdown List").FindChildRecursively("Content").transform.GetChild(1).gameObject);
    }
}
