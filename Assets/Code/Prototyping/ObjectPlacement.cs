using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject prefab;
#pragma warning restore
    /**************/ private GameObject prefabInstance;

    // Start is called before the first frame update
    void Start()
    {
        prefabInstance = Instantiate(prefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 1000f))
            prefabInstance.transform.position = hit.point;

        if (Mouse.current.leftButton.ReadValue() > 0)
            prefabInstance = Instantiate(prefab);
    }
}
