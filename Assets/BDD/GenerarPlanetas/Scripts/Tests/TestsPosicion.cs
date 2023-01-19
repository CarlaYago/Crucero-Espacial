using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestsPosicion : MonoBehaviour
{
    [Range(0, 1)] public float posX, posY = 0.5f;
    public Transform fondo;

    float limitX, limitY;

    void Update()
    {
        limitX = fondo.localScale.x / 2 - transform.localScale.x / 2;
        limitY = fondo.localScale.y / 2 - transform.localScale.y / 2;
        Vector3 offset = fondo.transform.position;

        float porcentajeX = (posX - 0.5f) / 0.5f;
        float porcentajeY = (posY - 0.5f) / 0.5f;

        float posicionX = (limitX * porcentajeX) + offset.x;
        float posicionY = (limitY * porcentajeY) + offset.y;

        transform.position = new Vector3(posicionX, posicionY, fondo.transform.position.z);
    }
}
