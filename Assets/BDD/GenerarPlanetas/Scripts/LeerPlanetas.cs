using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeerPlanetas : MonoBehaviour
{
    // Acceso a BDD
    SQLQuery consulta;
    DatosJugador datos;

    [Header("Referencias")]
    public Transform planetaParent;
    GenerarPlanetas generador;
    public GameObject planetaPrefab;

    [Header("Funcionamiento")]
    public bool leerBDD;
    public bool debug;

    // Contenedores de datos
    List<string> nombres;
    List<int[]> capasBDD, minijuegos;
    List<float[]> colores, posiciones, escalas;
    List<bool> flipX;
    List<int> precios, recompensas, ids;

    void Start()
    {
        generador = FindObjectOfType<GenerarPlanetas>();

        if (leerBDD)
        {
            consulta = new SQLQuery("Usuarios");
            datos = FindObjectOfType<DatosJugador>();
            LecturaPlanetas();
            InstanciarPlanetas();
        }
    }

    void LecturaPlanetas()
    {
        consulta.Query("SELECT * FROM Planetas");
        ids = consulta.IntReader(1);
        nombres = consulta.StringReader(2);
        capasBDD = consulta.IntArrayReader(3);
        flipX = consulta.BoolReader(4);
        colores = consulta.FloatArrayReader(5);
        posiciones = consulta.FloatArrayReader(6);
        escalas = consulta.FloatArrayReader(7);
        minijuegos = consulta.IntArrayReader(8);
        precios = consulta.IntReader(9);
        recompensas = consulta.IntReader(10);
    }

    void InstanciarPlanetas()
    {
        int numPlanetas = consulta.Count();

        for (int i = 0; i < numPlanetas; i++)
        {
            GameObject planeta = Instantiate(planetaPrefab, planetaParent);

            DatosPlanetaPrefab(planeta.transform, i);
            DatosPlanetaScript(planeta.transform, i);
        }

        if (debug) Debug.Log("Lectura de planetas finalizada");
    }

    Vector2 ConversorVector2(float[] array)
    {
        float x = array[0];
        float y = array[1];

        return new Vector2(x, y);
    }

    Color ConversorColor(float[] colores, int index)
    {
        float[] rgb = new float[3];
        int indexInicial = index * 3;

        for (int i = indexInicial; i < indexInicial + 3; i++)
        {
            rgb[i - indexInicial] = colores[i];
        }

        return new Color(rgb[0], rgb[1], rgb[2]);
    }

    void DatosPlanetaPrefab(Transform planeta, int index)
    {
        planeta.name = "Planeta " + index;

        Transform capasParent = planeta.Find("Capas");
        SpriteRenderer[] capas = new SpriteRenderer[capasParent.childCount];

        for (int i = 0; i < capas.Length; i++)
        {
            capas[i] = capasParent.GetChild(i).GetComponent<SpriteRenderer>();
        }

        capas[0].sprite = generador.cielos[capasBDD[index][0]];
        capas[1].sprite = generador.bases[capasBDD[index][1]];
        capas[2].sprite = generador.detalles[capasBDD[index][2]];
        capas[3].sprite = generador.adicional[capasBDD[index][3]];

        if (flipX[index] == true)
        {
            for (int i = 0; i < capas.Length; i++)
            {
                capas[i].flipX = true;
            }
        }

        for (int i = 0; i < capas.Length; i++)
        {
            capas[i].color = ConversorColor(colores[index], i);
        }

        planeta.position = ConversorVector2(posiciones[index]);
        planeta.localScale = ConversorVector2(escalas[index]);
    }

    void DatosPlanetaScript(Transform planeta, int index)
    {
        PlanetaPrefab planetaScript = planeta.GetComponent<PlanetaPrefab>();
        int id = ids[index];

        // Datos generales
        planetaScript.id = id;
        planetaScript.nombre = nombres[index];
        planetaScript.minijuegosElegidos = minijuegos[index];
        planetaScript.precioViaje = precios[index];
        planetaScript.recompensaEnergia = recompensas[index];

        // Datos individuales
        if (datos.plaFavorito_BDD.ContainsKey(id))
            planetaScript.favorito = datos.plaFavorito_BDD[id];
        //
        if (datos.plaUltimaVisita_BDD.ContainsKey(id))
        {
            Debug.Log("si que lo detecto");
            DateTime tiempoInicial = datos.plaUltimaVisita_BDD[id];
            TimeSpan t = new TimeSpan((int)datos.tiempoDeEspera.x, (int)datos.tiempoDeEspera.y, (int)datos.tiempoDeEspera.z, (int)datos.tiempoDeEspera.w);

            DateTime tiempoFinal = tiempoInicial + t;

            planetaScript.tiempoLimite = tiempoFinal;
            planetaScript.visto = true;
        }
    }

    public void RondasGeneradas()
    {
        consulta = new SQLQuery("Usuarios");
        consulta.Query("UPDATE Usuarios SET DificultadCambiada = 0 WHERE ID_Usuario = " + datos.id);
        datos.regenerarRondas = false;
    }
}