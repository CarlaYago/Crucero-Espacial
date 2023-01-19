using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Transferencia de imagenes para poder convertirse en JSON
[Serializable]
public struct PalabrasJSON
{
    public List<string> palabras;
    public List<Texture2DData> imagenes;
    public List<int> dificultades;
}

public class Diccionario : MonoBehaviour
{
    [Header("Palabras por defecto")]
    public Predefinidas predefinidas;

    [Header("Contenedor de datos")]
    public List<PalabrasDisponibles> diccionario;
    PalabrasJSON diccionarioJSON, lecturaJSON;
    string json;

    [Header("Relleno de datos (temporal)")]
    public List<string> palabras = new List<string>();
    public List<Texture2D> imagenes = new List<Texture2D>();
    public List<int> dificultad = new List<int>();

    [Header("Muestra de datos")]
    int id;

    void Start()
    {
        diccionarioJSON.palabras = new List<string>();
        diccionarioJSON.imagenes = new List<Texture2DData>();
        diccionarioJSON.dificultades = new List<int>();

        //id = IDJugador.id;
        CargarDatos();
    }

    void CargarDatos()
    {
        if (PlayerPrefs.HasKey(id.ToString()))
        {
            Debug.Log("Se han encontrado datos para el jugador " + id + ". Cargando datos...");

            // Encontrar y leer JSON
            json = PlayerPrefs.GetString(id.ToString());
            lecturaJSON = JsonUtility.FromJson<PalabrasJSON>(json);

            // Pasar datos JSON a listas públicas (temporal, para comprobar si funciona)
            palabras = lecturaJSON.palabras;
            for (int i = 0; i < lecturaJSON.imagenes.Count; i++)
            {
                imagenes.Add(lecturaJSON.imagenes[i].Recreate());
            }
            dificultad = lecturaJSON.dificultades;

            Debug.Log("Palabras del jugador  " + id + " cargados.");
        }
        else
        {
            Debug.Log("No se han encontrado datos para el jugador " + id + ". Cargando predefinidos...");
            diccionario = predefinidas.diccionario;
            Debug.Log("Palabras predefinidas cargadas.");
        }
    }

    public void GuardarDatos()
    {
        // Rellenar struct con datos de palabras, imagenes y dificultad
        for (int i = 0; i < diccionario.Count; i++)
        {
            diccionarioJSON.palabras.Add(diccionario[i].palabra);
            diccionarioJSON.imagenes.Add(new Texture2DData(diccionario[i].imagen));
            diccionarioJSON.dificultades.Add(diccionario[i].dificultad);
        }

        // Convertir a JSON
        json = JsonUtility.ToJson(diccionarioJSON);

        // Guardar con la id del jugador
        PlayerPrefs.SetString(id.ToString(), json);
        PlayerPrefs.Save();
    }

    void MostrarDatos()
    {

    }
}
