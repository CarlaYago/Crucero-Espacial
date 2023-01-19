using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotacion : MonoBehaviour
{
    public float angulosPorSegundo = 1;

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * angulosPorSegundo);
    }
}