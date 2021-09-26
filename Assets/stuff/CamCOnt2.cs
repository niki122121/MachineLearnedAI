using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCOnt2 : MonoBehaviour
{

    [SerializeField] GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y,-10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10);
    }
}
