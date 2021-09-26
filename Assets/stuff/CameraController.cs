using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] bool followX;
    [SerializeField] bool followY;
    
    void FixedUpdate()
    {
        if (target != null)
        {
            if(followX)
                transform.position = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
            if(followY)
                transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
        }
    }

    public void setTarget(GameObject trgt)
    {
        target = trgt;
    }

    public GameObject getTarget()
    {
        return target;
    }
}
