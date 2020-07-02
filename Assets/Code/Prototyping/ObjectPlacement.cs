using Goldenwere.Unity.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GodGameCamera              gameCam;
    [SerializeField] private GameObject[]               prefabs;
    [SerializeField] private int                        prefabUndoMaxCount;
    [SerializeField] private GameObject                 prototypeCanvas;
    [SerializeField] private float                      rotationAngleMagnitude;
#pragma warning restore 0649
    /**************/ private bool                       isPlacing;
    /**************/ private bool                       prefabHadFirstHit;
    /**************/ private ProtoObject                prefabInstance;
    /**************/ private LinkedList<ProtoObject>    prefabsPlaced;
    /**************/ private bool                       workingModifierMouseZoom;

    // Start is called before the first frame update
    void Start()
    {
        prefabsPlaced = new LinkedList<ProtoObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacing && Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 1000f))
        {
            if (prefabHadFirstHit)
                prefabInstance.transform.position = Vector3.Lerp(prefabInstance.transform.position, hit.point, Time.deltaTime * 25);

            else
            {
                prefabInstance.transform.position = hit.point;
                prefabHadFirstHit = true;
            }
        }
    }

    public void OnObjectSelected(int id)
    {
        if (!isPlacing)
        {
            prefabInstance = Instantiate(prefabs[id]).GetComponent<ProtoObject>();
            prefabHadFirstHit = false;
            prefabInstance.IsPlaced = false;
            isPlacing = true;
            prototypeCanvas.SetActive(false);
        }
    }

    public void OnObjectRotation(InputAction.CallbackContext context)
    {
        if (!workingModifierMouseZoom && context.performed && isPlacing)
            if (context.ReadValue<float>() > 0)
                prefabInstance.transform.Rotate(Vector3.up, -rotationAngleMagnitude);
            else
                prefabInstance.transform.Rotate(Vector3.up, rotationAngleMagnitude);
    }

    public void OnPlacement(InputAction.CallbackContext context)
    {
        if (context.performed && isPlacing && prefabInstance.IsValid)
        {
            prefabInstance.IsPlaced = true;
            prefabsPlaced.AddFirst(prefabInstance);
            if (prefabsPlaced.Count > prefabUndoMaxCount)
                prefabsPlaced.RemoveLast();
            prefabInstance = null;
            isPlacing = false;
            prefabHadFirstHit = false;
            prototypeCanvas.SetActive(true);
        }
    }

    public void OnUndo(InputAction.CallbackContext context)
    {
        if (context.performed && prefabsPlaced.Count > 0)
        {
            if (isPlacing)
                Destroy(prefabInstance.gameObject);
            isPlacing = true;
            prefabHadFirstHit = true;
            prefabInstance = prefabsPlaced.First.Value;
            prefabsPlaced.RemoveFirst();
            prefabInstance.IsPlaced = false;
            prototypeCanvas.SetActive(false);
        }
    }

    public void OnZoomMouseModifier(InputAction.CallbackContext context)
    {
        if (gameCam.settingModifiersAreToggled)
            workingModifierMouseZoom = !workingModifierMouseZoom;
        else
            workingModifierMouseZoom = context.performed;
    }
}
