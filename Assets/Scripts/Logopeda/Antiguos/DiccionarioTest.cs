using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Asignación de palabras a través del Inspector
[Serializable]
public struct PalabrasDisponibles0
{
    public string palabra;
    public Texture2D imagen;
    [Range(0, 2)] public int dificultad;
}

// Transferencia de palabras para poder convertirse en JSON
[Serializable]
public struct PalabrasJSON0
{
    public List<string> palabras;
    public List<Texture2DData> imagenes;
    public List<int> dificultades;
}

public class DiccionarioTest : MonoBehaviour
{
    public PalabrasDisponibles0[] diccionario;
    PalabrasJSON0 diccionarioJSON, lecturaJSON;

    public List<string> palabras = new List<string>();
    public List<Texture2D> imagenes = new List<Texture2D>();
    public List<int> dificultad = new List<int>();

    void Start()
    {
        diccionarioJSON.palabras = new List<string>();
        diccionarioJSON.imagenes = new List<Texture2DData>();
        diccionarioJSON.dificultades = new List<int>();

        #region Rellenar diccionarioJSON

        for (int i = 0; i < diccionario.Length; i++)
        {
            diccionarioJSON.palabras.Add(diccionario[i].palabra);
            diccionarioJSON.imagenes.Add(new Texture2DData(diccionario[i].imagen));
            diccionarioJSON.dificultades.Add(diccionario[i].dificultad);
        }

        #endregion Rellenar diccionarioJSON


        #region Convertir diccionarioJSON a JSON y leerlo en lecturaJSON

        string json = JsonUtility.ToJson(diccionarioJSON);
        lecturaJSON = JsonUtility.FromJson<PalabrasJSON0>(json);

        #endregion Convertir diccionarioJSON a JSON y leerlo en lecturaJSON


        #region Usar lecturas de JSON para rellenar tres listas públicas

        palabras = lecturaJSON.palabras;
        for (int i = 0; i < lecturaJSON.imagenes.Count; i++)
        {
            imagenes.Add(lecturaJSON.imagenes[i].Recreate());
        }
        dificultad = lecturaJSON.dificultades;

        #endregion Usar lecturas de JSON para rellenar tres listas públicas
    }
}
