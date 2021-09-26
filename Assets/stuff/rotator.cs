using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    [SerializeField] float rotation;
    void Update()
    {
        transform.Rotate(0, 0, -rotation);
    }
}
