using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{

    public GameObject obj;

    [SerializeField]
    private LayerMask layerMask;

    private RaycastHit hit;
    private Vector3 pos;

    [SerializeField]
    private Collider collider;

    private Material material;

    
    private void OnEnable()
    {
        collider.isTrigger = true;
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            pos = hit.point;
        }
    }

    void Update()
    {
        if(obj != null)
        {
            obj.transform.position = pos;

            
            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();
            }
        }
    }

    void PlaceObject()
    {
        Vector3 originPos;
        originPos = obj.transform.position;
        originPos.y += 1f;
        obj.transform.position = originPos;
        obj = null;
        collider.isTrigger = false;
    }
}
