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
                if (palabra.Contains("�")) palabra = palabra.Replace("�", "A");
                if (palabra.Contains("�")) palabra = palabra.Replace("�", "E");
                if (palabra.Contains("�")) palabra = palabra.Replace("�", "I");
                if (palabra.Contains("�")) palabra = palabra.Replace("�", "O");
                if (palabra.Contains("�")) palabra = palabra.Replace("�", "U");

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