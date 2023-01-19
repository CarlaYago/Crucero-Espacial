// Colocar este script por cada texto donde se muestre la dificultad de un minijuego. //
// Utilizar las funciones públicas con los botones de aumentar / subir dificultad relacionados en todos los paneles de dificultad. //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Identificadores
{
    Palabra,
    Frase,
}

public class CambiarDificultad : MonoBehaviour
{
    SQLQuery query;
    public DificultadesSO nombresDificultad;

    Text dificultadUI;
    int dificultad;

    [HideInInspector] public int registroID;
    public Identificadores tipoDeRegistro; // Palabras, Textos, Frases, etc.

    void Start()
    {
        query = new SQLQuery("BaseLogopeda");
        dificultadUI = GetComponentInChildren<Text>();
    }

    public void CambioDificultad(int cambio) // Se le llama al hacer clic sobre el símbolo "+" o "-" en cualquier panel de Dificultad
    {
        dificultad = nombresDificultad.dificultades.IndexOf(dificultadUI.text);

        if (dificultad + cambio >= 0 && dificultad + cambio <= nombresDificultad.dificultades.Count - 1)
        {
            dificultad += cambio;
        }

        dificultadUI.text = nombresDificultad.dificultades[dificultad];

        int numMinijuego = nombresDificultad.numeroMinijuego;
        query.Query("UPDATE Dificultad SET Minijuego" + numMinijuego + "=" + dificultad + " WHERE ID_Entrada = " + registroID + " AND Identificador = '" + tipoDeRegistro + "'");
    }
}