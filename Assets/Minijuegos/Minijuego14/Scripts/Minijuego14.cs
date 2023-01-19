using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Minijuego14 : MonoBehaviour
{
    // Consultas SQL
    SQLQuery consultas;

    // Managers
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;
    ManagerMinijuegos minijuegoManager; // Le falta detectar fallos

    [Range(1, 11)] int dificultad;
    // escena
    public Transform padre, padreOpciones;
    // public TextMeshProUGUI palabraIncompleta;
    public GameObject palabraprefab, espacioprefab, opcion;
    //lista de pruebas
    List<string> probar = new List<string>();
    string eleccion;
    List<string> eleccionPrueba = new List<string>();
    string[] array;

    AudioSource reproductor;
    AudioClip acierto, fallo;

    void Start()
    {
        probar.Add("La /luna/ sale por la /noche/");
        probar.Add("El gato no puede hablar");
        int random = 0;
        if (random == 0)
        {
            eleccionPrueba.Add("luna");
            eleccionPrueba.Add("noche");
        }

        if (random == 1)
        {
            eleccionPrueba.Add("gato");
            eleccionPrueba.Add("hablar");
        }

        array = probar[random].Split(new string[] { "/" }, StringSplitOptions.None);
        for (int i = 0; i < array.Length; i++)
        {
            int n = 0;
            eleccion += array[i];
            Debug.Log(array[i]);
            for (int e = 0; e < eleccionPrueba.Count; e++)
            {
                if (array[i].Contains(eleccionPrueba[e]))
                {
                    n++;
                }


            }
            if (n > 0)
            {
                GameObject pos = Instantiate(espacioprefab, padre);
                UpdateCanvas();
            }
            else
            {
                GameObject pos = Instantiate(palabraprefab, padre);
                pos.GetComponentInChildren<TextMeshProUGUI>().text = array[i];
                UpdateCanvas();
            }
        }
        for (int i = 0; i < eleccionPrueba.Count; i++)
        {
            GameObject pos = Instantiate(opcion, padreOpciones);
            pos.GetComponentInChildren<Text>().text = eleccionPrueba[i];
            UpdateCanvas();
        }

        probar[random] = probar[random].Replace("/", "");
        // palabraIncompleta.text = probar[random];

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        acierto = audioManager.acierto;
        fallo = audioManager.fallo;
    }
    void UpdateCanvas()
    {
        Canvas.ForceUpdateCanvases();
       // padre.GetComponent<HorizontalLayoutGroup>().enabled = false;
        //padre.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    void Update()
    {
       
    }

    void GenerarFrases(int num)
    {

    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    recompensasManager = new RarezaPalabras(2);
                    GenerarFrases(3);
                    break;
                }
            case 2:
                {
                    recompensasManager = new RarezaPalabras(5);
                    GenerarFrases(4);
                    break;
                }
            case 3:
                {
                    recompensasManager = new RarezaPalabras(7);
                    GenerarFrases(5);
                    break;
                }
            case 4:
                {
                    recompensasManager = new RarezaPalabras(12);
                    GenerarFrases(6);
                    break;
                }
            case 5:
                {
                    recompensasManager = new RarezaPalabras(14);
                    GenerarFrases(7);
                    break;
                }
            case 6:
                {
                    recompensasManager = new RarezaPalabras(25);
                    GenerarFrases(8);
                    break;
                }
            case 7:
                {
                    recompensasManager = new RarezaPalabras(22);
                    GenerarFrases(9);
                    break;
                }
            case 8:
                {
                    recompensasManager = new RarezaPalabras(30);
                    GenerarFrases(10);
                    break;
                }
            case 9:
                {
                    recompensasManager = new RarezaPalabras(28);
                    GenerarFrases(11);
                    break;
                }
            case 10:
                {
                    recompensasManager = new RarezaPalabras(33);
                    GenerarFrases(12);
                    break;
                }
            case 11:
                {
                    recompensasManager = new RarezaPalabras(31);
                    GenerarFrases(13);
                    break;
                }
        }
    }
}