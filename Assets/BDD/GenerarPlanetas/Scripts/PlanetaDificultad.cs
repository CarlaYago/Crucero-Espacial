using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaDificultad : MonoBehaviour
{
    SQLQuery consultas;
    PlanetaPrefab datosPlaneta;

    [Range(0, 1)] public float distanciaProporcional;
    float longitud;
    public int dificultad;
    public int[] rondas;

    DatosJugador config;

    void Start()
    {
        Transform planetario = FindObjectOfType<GenerarPlanetas>().espacio;

        longitud = planetario.GetComponent<SpriteRenderer>().bounds.extents.x;
        distanciaProporcional = Vector2.Distance(transform.position, planetario.position) / longitud;

        config = FindObjectOfType<DatosJugador>();
        int indexDificultad = -1;

        for (int i = 0; i < config.distancias.Count; i++)
        {
            //float dist = config.distancias[i] * longitud;
            float dist = config.distancias[i];

            if (distanciaProporcional < dist)
            {
                indexDificultad = i;
                break;
            }
        }

        dificultad = config.dificultades[indexDificultad];

        GenerarRondas();
        //CalcularPorcentajes();
    }

    void GenerarRondas()
    {
        datosPlaneta = GetComponent<PlanetaPrefab>();
        consultas = new SQLQuery("Usuarios");
        rondas = new int[3];
        Debug.Log("rondas:"+config.regenerarRondas);

        if (config.regenerarRondas)
        {
            string rondasElegidas = "";

            for (int i = 0; i < 3; i++)
            {
                int idMinijuego = datosPlaneta.minijuegosElegidos[i];

                consultas.Query("SELECT Rondas FROM Minijuegos WHERE Minijuego = " + idMinijuego + " AND Dificultad = " + dificultad);
                int[] rondasMinMax = consultas.IntArrayReader(1, '-')[0];

                if (rondasMinMax.Length < 2)
                    rondas[i] = rondasMinMax[0];
                else
                    rondas[i] = Random.Range(rondasMinMax[0], rondasMinMax[1] + 1);

                if (i == 0)
                    rondasElegidas += rondas[i];
                else
                    rondasElegidas += "/" + rondas[i];
            }

            ActualizarBDD(rondasElegidas);
        }
        else
        {
            LeerBDD();
        }
    }

    void ActualizarBDD(string rondas)
    {
        consultas.Query("SELECT * FROM Planetario_" + config.id + " WHERE Planeta_ID = " + datosPlaneta.id);

        if (consultas.Count() != 0)
        {
            consultas.Query("UPDATE Planetario_" + config.id + " SET Rondas = '" + rondas + "' WHERE Planeta_ID = " + datosPlaneta.id);
        }
        else
        {
            consultas.Query("INSERT INTO Planetario_" + config.id + " (Planeta_ID, Rondas) VALUES (" + datosPlaneta.id + ",'" + rondas + "')");
        }
    }

    void LeerBDD()
    {
        consultas.Query("SELECT Rondas FROM Planetario_" + config.id + " WHERE Planeta_ID = " + datosPlaneta.id);
        rondas = consultas.IntArrayReader(1)[0];
    }
}