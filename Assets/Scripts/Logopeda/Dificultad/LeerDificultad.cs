// Colocar este script en los padres activos de los paneles de dificultad, referenciando en "dificultadesUI" los valores númericos de dificultad por minijuego. //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeerDificultad : MonoBehaviour
{
    public Text[] dificultadesUI;
    SQLQuery lectura;

    Text encabezado;
    string encabezadoUI;

    void Start()
    {
        lectura = new SQLQuery("BaseLogopeda");
        encabezado = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        encabezadoUI = encabezado.text;
    }

    public void Leer(int registroID, string tipoDeRegistro)
    {
        // Insertar una referencia al registro en la tabla de dificultades si todavía no hay
        lectura.Query("SELECT * FROM Dificultad WHERE ID_Entrada = " + registroID + " AND Identificador = '" + tipoDeRegistro + "'");
        if (lectura.Count() == 0)
        {
            lectura.Query("INSERT INTO Dificultad (ID_Entrada, Identificador)" + " VALUES (" + registroID + ", '" + tipoDeRegistro + "')");
        }

        // Leer los valores de la tabla de dificultades 
        for (int i = 0; i < dificultadesUI.Length; i++)
        {
            DificultadesSO infoDificultad = dificultadesUI[i].GetComponentsInParent<CambiarDificultad>(true)[0].nombresDificultad;
            int minijuego = infoDificultad.numeroMinijuego;

            lectura.Query("SELECT Minijuego" + minijuego + " FROM Dificultad WHERE ID_Entrada = " + registroID + " AND Identificador = '" + tipoDeRegistro + "'");
            int dificultad = lectura.IntReader(1)[0];

            List<string> dificultades = infoDificultad.dificultades;
            dificultadesUI[i].text = dificultades[dificultad];
        }
    }

    public void CambiarEncabezado(string texto)
    {
        encabezado.text = encabezadoUI + texto;
    }
}
