// Asignar este script en cualquier parte y dar a Play para que todas las frases de BDD detecten las palabras recompensables y las pongan en negrita

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PalabrasNegritaBDD : MonoBehaviour
{
    SQLQuery consultas;
    DetectarIdentificadores identificadores;

    void Start()
    {
        consultas = new SQLQuery("BaseLogopeda");
        identificadores = new DetectarIdentificadores("<b>", "</b>");

        consultas.Query("SELECT ID_Frase, Frase FROM FrasesTexto");
        Dictionary<int, string> frases = consultas.StringReaderID(1, 2);

        for (int i = 0; i < frases.Count; i++)
        {
            string frase = frases.ElementAt(i).Value;
            string fraseNueva = identificadores.AsignarIdentificadores(frase);

            int id = frases.ElementAt(i).Key;

            consultas.Query("UPDATE FrasesTexto SET Frase = '" + fraseNueva + "' WHERE ID_Frase = " + id);
        }
    }
}