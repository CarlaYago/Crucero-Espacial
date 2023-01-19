using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public struct Palabras
{
    public string palabra;
    public string categoria;
}

public class Diccionario_Minijuego5: MonoBehaviour
{
    public List<Palabras> diccionario;
}