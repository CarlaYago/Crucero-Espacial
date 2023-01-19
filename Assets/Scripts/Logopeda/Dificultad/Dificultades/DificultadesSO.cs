using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TiposDificultad", menuName = "ScriptableObjects/Dificultades", order = 1)]
public class DificultadesSO : ScriptableObject
{
    public int numeroMinijuego;
    public List<string> dificultades = new List<string> { "No incluido" };
}