using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NuevaPalabra : MonoBehaviour
{
    [Header("Referencias interfaz")]
    public GameObject filaPrefab, contenidoTabla;
    public InputField palabraNueva;
    Button btn;

    [Header("Categorizacion palabras")]
    [Range(0, 2)] public int dificultad;
    Diccionario jugador;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(NuevaFila);
        jugador = FindObjectOfType<Diccionario>();
    }

    void NuevaFila()
    {
        if (PalabraRepetida() == false)
        {
            GameObject filaNueva = Instantiate(filaPrefab, contenidoTabla.transform);
            Text palabra = filaNueva.transform.GetChild(0).GetComponent<Text>();
            palabra.text = palabraNueva.text;

            // Enviar palabra al diccionario
            PalabrasDisponibles datos = new PalabrasDisponibles
            {
                palabra = palabra.text,
                dificultad = dificultad
            };

            jugador.diccionario.Add(datos);
        }
        else
        {
            // Mensaje de aviso
            Debug.Log("Palabra repetida");
        }
    }

    bool PalabraRepetida()
    {
        foreach (PalabrasDisponibles registro in jugador.diccionario)
        {
            if (registro.palabra == palabraNueva.text)
            {
                return true;
            }
        }

        return false;
    }
}
