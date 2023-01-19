using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaSQLQ : MonoBehaviour
{
    public List<string> palabrasFiltradas;

    void Start()
    {
        SQLQuery consulta = new SQLQuery("LexicoEsp");
        consulta.Query("SELECT Palabra FROM LexicoEsp WHERE Palabra LIKE 'AMB%'");

        palabrasFiltradas = consulta.StringReader(1);
    }
}
