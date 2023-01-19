using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDDDificultad : MonoBehaviour
{
    [Header("Numero de Registros")]
    public int indiceInicial;
    public int cantidad;

    [Header("Tipo de Registros")]
    [Range(0, 2)] public int dificultad;
    public Identificadores identificador;

    [Header("Eliminar Registros")]
    public bool vaciarTablaCompleta;
    public bool vaciarPorIdentificador;

    SQLQuery query;

    void Start()
    {
        query = new SQLQuery("BaseLogopeda");

        if (!vaciarTablaCompleta && !vaciarPorIdentificador)
        {
            ValoresDificultad(15);

            for (int i = indiceInicial; i <= cantidad; i++)
            {
                query.Query("INSERT INTO Dificultad VALUES (" + i + ", '" + identificador.ToString() + "' , " + ValoresDificultad(15) + ")");
            }
        }
        else
        {
            EliminarRegistros();
        }
    }

    string ValoresDificultad(int numMinijuegos)
    {
        string valoresDificultad = "";

        for (int n = 1; n <= numMinijuegos; n++)
        {
            if (n < numMinijuegos)
                valoresDificultad += dificultad + ", ";
            else
                valoresDificultad += dificultad;
        }

        return valoresDificultad;
    }

    void EliminarRegistros()
    {
        if (vaciarTablaCompleta)
        {
            query.Query("DELETE FROM Dificultad");
        }
        else if (vaciarPorIdentificador)
        {
            query.Query("DELETE FROM Dificultad WHERE Identificador = " + identificador.ToString());
        }
    }
}
