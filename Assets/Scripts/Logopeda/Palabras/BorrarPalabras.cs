using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorrarPalabras : MonoBehaviour
{
    SQLQuery consultas;

    [Header("Panel borrar palabras")]
    public Text titulo;
    public Button botonConfirmar;
    public GameObject panelEliminar;
    string tituloText;

    void Start()
    {
        consultas = new SQLQuery("BaseLogopeda");
        tituloText = titulo.text;
    }

    public void PanelEliminar(Text palabra)
    {
        AbrirPanel(palabra.text);
        botonConfirmar.onClick.AddListener(delegate { EliminarPalabra(palabra); });
    }

    void AbrirPanel(string registro)
    {
        titulo.text = tituloText + registro;

        botonConfirmar.onClick.RemoveAllListeners();

        panelEliminar.SetActive(true);
    }

    void EliminarPalabra(Text palabra)
    {
        Destroy(palabra.transform.parent.gameObject);

        consultas.Query("SELECT ID_Palabra FROM Palabras WHERE Palabra = '" + palabra.text + "'");
        int id = consultas.IntReader(1)[0];

        consultas.Query("DELETE FROM Palabras WHERE ID_Palabra = " + id + "");
        consultas.Query("DELETE FROM Dificultad WHERE ID_Entrada = " + id + " AND Identificador = 'Palabra'");

        panelEliminar.SetActive(false);
    }
}