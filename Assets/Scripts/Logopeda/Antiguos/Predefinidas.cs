using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct PalabrasDisponibles
{
    public string palabra;
    public Texture2D imagen;
    [Range(0, 2)] public int dificultad;
}

public class Predefinidas : MonoBehaviour
{
    // Asignaci�n de palabras a trav�s del Inspector
    public List<PalabrasDisponibles> diccionario;
}
