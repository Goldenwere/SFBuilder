using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject     prefab;
#pragma warning restore 0649
    /**************/ private ProtoObject    prefabInstance;
    /**************/ private bool           isPlacing;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacing && Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 1000f))
            if (hit.normal == Vector3.up)
                prefabInstance.transform.position = hit.point;

        if (Mouse.current.leftButton.ReadValue() > 0 && isPlacing && !prefabInstance.IsCollided && prefabInstance.IsGrounded)
        {
            prefabInstance = null;
            isPlacing = false;
        }

        if (Mouse.current.rightButton.ReadValue() > 0 && !isPlacing)
        {
            isPlacing = !isPlacing;
            if (isPlacing)
                prefabInstance = Instantiate(prefab).GetComponent<ProtoObject>();
        }
    }
}
