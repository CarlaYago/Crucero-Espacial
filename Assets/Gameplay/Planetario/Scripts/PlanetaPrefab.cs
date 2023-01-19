using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlanetaPrefab : MonoBehaviour
{
    SQLQuery consultas;

    [Header("Referencias")]
    public TextMeshPro contadorUI;
    public Color colorDeshabilitado = Color.white;
    Color[] colores;
    ManagerPlanetario managerScript;
    SpriteRenderer favoritoIm, vistoIm, cronometroIm, grisIm;
    desplegable desplegableScript;
    GameObject imgSeleccion;

[Header("Stats")]
    public InspectorPlanetas inspectorScript;

    [Header("Datos generales")]
    public int id;
    public string nombre;
    public int dificultad, precioViaje;
    public int recompensaEnergia;
    public int[] minijuegosElegidos;

    [Header("Recompensas")]
    public int[] porcentajeRareza; // Indica la rareza de palabras adquiribles

    [Header("Datos individuales")]
    public bool favorito;
    public bool visto, cronometro;
    public DateTime tiempoLimite;
    TimeSpan contador;

    void Start()
    {
        Transform simbolos = transform.Find("Simbolos");

        grisIm = simbolos.GetChild(0).GetComponent<SpriteRenderer>();
        cronometroIm = simbolos.GetChild(1).GetComponent<SpriteRenderer>();
        favoritoIm = simbolos.GetChild(2).GetComponent<SpriteRenderer>();
        vistoIm = simbolos.GetChild(3).GetComponent<SpriteRenderer>();

        if (FindObjectOfType<ManagerPlanetario>() != null) managerScript = FindObjectOfType<ManagerPlanetario>();
        if (FindObjectOfType<InspectorPlanetas>() != null) inspectorScript = FindObjectOfType<InspectorPlanetas>();
        desplegableScript = managerScript.desplegable;
        imgSeleccion = managerScript.imgSeleccion;
        cronometroIm.enabled = false;
        if (DateTime.Now < tiempoLimite) cronometro = true;
        //funcion marcar favorito
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit && hit.transform.gameObject == gameObject && !RaycastCanvas())
            {
                managerScript.cam.transform.position = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -39);//new 29/4
                desplegableScript.desplegableObject.SetActive(true); //new 29/4
                imgSeleccion.SetActive(true);
                InfoPlanetaDificultad();
                Textos();
                //desplegableScript.desplegar();//new 29/4
                if (cronometro || managerScript.energiaJugador < precioViaje) managerScript.viajarBot.interactable = false;
                else managerScript.viajarBot.interactable = true;
            }
        }


        if (managerScript != null)
        {
            if (managerScript.energiaJugador < precioViaje)
            {
                if (!grisIm.enabled) grisIm.enabled = true;
                ColorGris();
            }
            else
            {
                if (grisIm.enabled) grisIm.enabled = false;
            }
        }

        Contador();
        //para estos iconos recordar que el script planeta_padre accede a ellos según su posición como hijos, si se edita su jerarquía leugo se deve arreglar el script
        ActivarObjeto(favoritoIm.gameObject, favorito); // Favorito
        ActivarObjeto(vistoIm.gameObject, visto); // Visto
        ActivarObjeto(contadorUI.gameObject, cronometro); // Cronometro
    }

    bool RaycastCanvas()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem) { position = Input.mousePosition };
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count > 0) return true;
        else return false;
    }

    void ColorGris()
    {
        Transform capas = transform.Find("Capas");
        colores = new Color[capas.childCount];

        for (int i = 0; i < capas.childCount; i++)
        {
            SpriteRenderer capa = capas.GetChild(i).GetComponent<SpriteRenderer>();
            colores[i] = capa.color;
            capa.color = colorDeshabilitado;
        }
    }

    void ColorOriginal()
    {
        Transform capas = transform.Find("Capas");

        for (int i = 0; i < capas.childCount; i++)
        {
            SpriteRenderer capa = capas.GetChild(i).GetComponent<SpriteRenderer>();
            capa.color = colores[i];
        }
    }

    void Contador()
    {
        if (cronometro)
        {
            if (!cronometroIm.enabled)
            {
                ColorGris();
                visto = false;
                cronometroIm.enabled = true;
            }

            if (DateTime.Now < tiempoLimite)
            {
                contador = tiempoLimite.Subtract(DateTime.Now);
                contadorUI.text = contador.ToString(@"d\:hh\:mm\:ss").TrimStart('0', ':');
            }
            else
            {
                ColorOriginal();
                visto = true;
                inspectorScript.ImagenPlaneta(transform);
                managerScript.viajarBot.interactable = true;
                cronometro = false;
            }
        }
        else
        {
            contadorUI.text = "";
            cronometroIm.enabled = false;
        }
    }

    public void Viajar() // boton del viaje acción
    {
        DatosJugador datos = FindObjectOfType<DatosJugador>();
        datos.idPlanetaActual = id;
        datos.minijuegosElegidos = minijuegosElegidos;
        datos.minijuegosRondas = GetComponent<PlanetaDificultad>().rondas;
        datos.minijuegoDificultad = GetComponent<PlanetaDificultad>().dificultad;
        datos.minijuegoCombustible = recompensaEnergia;

        datos.minijuegosPorcentajes = new float[3];
        datos.minijuegoRecienCompletado = new List<bool>(new bool[3]);

        int energiaNueva = managerScript.energiaJugador - precioViaje;
        managerScript.CambioEnergia(energiaNueva);

        Debug.Log("viajas a" + nombre);
    }

    void Textos()
    {
        if (inspectorScript != null)
        {
            // Encabezado
            int dificultad = GetComponent<PlanetaDificultad>().dificultad;
            inspectorScript.nombrePlaneta.text = nombre;
            inspectorScript.CargarNivel(dificultad);

            // Imagen
            inspectorScript.ImagenPlaneta(transform);

            // Minijuegos
            int[] rondas = GetComponent<PlanetaDificultad>().rondas;
            inspectorScript.CargarMinijuegos(minijuegosElegidos, rondas);

            // Recompensas
            for (int i = 0; i < 3; i++)
            {
                inspectorScript.textosRecompensas[i].text = porcentajeRareza[i] + "%";
            }
            inspectorScript.textosRecompensas[3].text = recompensaEnergia.ToString();

            // Precio
            inspectorScript.CargarCoste(precioViaje);
        }
    }

    void ActivarObjeto(GameObject objeto, bool activar)
    {
        if (activar)
        {
            if (!objeto.activeInHierarchy) objeto.SetActive(true);
        }
        else
        {
            if (objeto.activeInHierarchy) objeto.SetActive(false);
        }
    }

    void InfoPlanetaDificultad()
    {
        consultas = new SQLQuery("Usuarios");

        int[] porcentajesTotales = new int[3];
        porcentajeRareza = new int[3];

        for (int i = 0; i < minijuegosElegidos.Length; i++)
        {
            int id = minijuegosElegidos[i];
            int dificultad = GetComponent<PlanetaDificultad>().dificultad;

            consultas.Query("SELECT Probabilidades FROM Minijuegos WHERE Minijuego = " + id + " AND Dificultad = " + dificultad);
            int[] porcentajes = consultas.IntArrayReader(1)[0];

            for (int n = 0; n < 3; n++)
            {
                porcentajesTotales[n] += porcentajes[n];
            }
        }

        for (int i = 0; i < 3; i++)
        {
            float porcentajeDecimal = porcentajesTotales[i] / 3f;
            porcentajeRareza[i] = Mathf.RoundToInt(porcentajeDecimal);
        }
    }


}