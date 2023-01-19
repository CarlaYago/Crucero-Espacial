using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GapScript : MonoBehaviour
{
    public bool gapOcupado = false;
    int childCount;

    void Start()
    {
        childCount = transform.childCount;
    }

    void Update()
    {
        if (transform.childCount > childCount)
        {
            gapOcupado = true;
        }
        else gapOcupado = false;
    }
}