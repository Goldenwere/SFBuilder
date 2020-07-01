using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject     prefab;
#pragma warning restore 0649
    /**************/ private bool           isPlacing;
    /**************/ private bool           prefabFirstHit;
    /**************/ private ProtoObject    prefabInstance;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacing && Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 1000f))
        {
            if (prefabFirstHit)
                prefabInstance.transform.position = Vector3.Lerp(prefabInstance.transform.position, hit.point, Time.deltaTime * 25);

            else
            {
                prefabInstance.transform.position = hit.point;
                prefabFirstHit = true;
            }

            if (Mouse.current.leftButton.ReadValue() > 0 && prefabInstance.IsValid && hit.normal == Vector3.up)
            {
                prefabInstance = null;
                isPlacing = false;
                prefabFirstHit = false;
            }
        }

        if (Mouse.current.rightButton.ReadValue() > 0 && !isPlacing)
        {
            isPlacing = !isPlacing;
            if (isPlacing)
            {
                prefabInstance = Instantiate(prefab).GetComponent<ProtoObject>();
                prefabFirstHit = false;
            }

            else
                Destroy(prefabInstance);
        }
    }
}
