using Goldenwere.Unity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownWithEvents : TMP_Dropdown, IPointerClickHandler, ISubmitHandler
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        StartCoroutine(WaitForOptionsEnabled());
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        StartCoroutine(WaitForOptionsEnabled());
    }

    private IEnumerator WaitForOptionsEnabled()
    {
        while (!template.gameObject.activeInHierarchy)
            yield return new WaitForFixedUpdate();
        EventSystem.current.SetSelectedGameObject(template.gameObject.FindChildRecursively("Content").transform.GetChild(0).gameObject);
    }
}
