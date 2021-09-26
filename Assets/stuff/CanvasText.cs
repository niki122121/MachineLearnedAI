using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasText : MonoBehaviour
{
    [SerializeField] Text gen;
    [SerializeField] Text timeRem;
    [SerializeField] Text alive;
    

    public void setTime(float t)
    {
        timeRem.text = "TIME: " + (int)t;
    }

    public void setAlive(int rem)
    {
        alive.text = "ALIVE: " + rem;
    }

    public void setGen(int n) {
        gen.text = "GENERATION: " + n;
    }
}
