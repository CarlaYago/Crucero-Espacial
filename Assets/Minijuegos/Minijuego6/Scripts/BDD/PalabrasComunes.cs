using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalabrasComunes : MonoBehaviour
{
    SQLQuery bdd;

    public TextAsset datos;
    string[] palabrasArray;
    public List<string> palabrasComunes;

    public bool rellenarBDD;

    void Start()
    {
        RellenarListas();
        if (rellenarBDD) RellenarBDD();
    }

    void RellenarListas()
    {
        palabrasArray = datos.text.Split('\n', ' ');

        for (int i = 0; i < palabrasArray.Length; i++)
        {
            if ((i + 1) % 2 != 0)
            {
                string palabra = palabrasArray[i];
                if (palabra.Contains("Á")) palabra = palabra.Replace("Á", "A");
                if (palabra.Contains("É")) palabra = palabra.Replace("É", "E");
                if (palabra.Contains("Í")) palabra = palabra.Replace("Í", "I");
                if (palabra.Contains("Ó")) palabra = palabra.Replace("Ó", "O");
                if (palabra.Contains("Ú")) palabra = palabra.Replace("Ú", "U");

                if (!palabrasComunes.Contains(palabra))
                {
                    palabrasComunes.Add(palabra);
                }
            }
        }
    }

    void RellenarBDD()
    {
        bdd = new SQLQuery("LexicoEsp");

        for (int i = 0; i < palabrasComunes.Count; i++)
        {
            string palabra = palabrasComunes[i];
            bdd.Query("SELECT Palabra FROM LexicoEsp WHERE Palabra = '" + palabra + "'");

            if (bdd.Count() == 1)
            {
                bdd.Query("UPDATE LexicoEsp SET Facil = " + 1 + " WHERE Palabra = '" + palabra + "'");
            }
        }
    }
}