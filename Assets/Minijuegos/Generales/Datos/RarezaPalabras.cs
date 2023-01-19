using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarezaPalabras
{
    float probGris, probVerde, probAzul;
    DatosJugador datos;

    public RarezaPalabras(int porcentajeVerde = 0, int porcentajeAzul = 0)
    {
        datos = Object.FindObjectOfType<DatosJugador>();

        probVerde = porcentajeVerde / 100f;
        probAzul = porcentajeAzul / 100f;
        probGris = 1 - (probVerde + probAzul);
    }

    public int RarezaObtenida(string palabra)
    {
        float random = Random.Range(0f, 1f);
        int rareza = 0;

        if (random <= probGris)
        {
            rareza = 0;
        }
        else if (random <= probGris + probVerde)
        {
            rareza = 1;
        }
        else if (random <= probGris + probVerde + probAzul)
        {
            rareza = 2;
        }

        if (datos != null)
        {
            datos.rarezasRecompensas.Add(rareza);
            datos.recompensas.Add(palabra);
        }

        return rareza;
    }
}