using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimFlotante : MonoBehaviour
{
    public float velocidad, altura;
    float posInicial, contador;

    private void Start()
    {
        posInicial = transform.position.y;
    }

    void Update()
    {
        contador += Time.deltaTime * velocidad;
        float posY = posInicial + Mathf.Sin(contador) * altura;

        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
    }
}