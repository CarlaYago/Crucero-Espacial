using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NuevaDefinicion : MonoBehaviour
{
    SQLQuery query;

    //Interfaz cambio definición
    public Button[] botonesConfirmar;
    public TMP_InputField inputUsuario;

    Texture simboloValorNoNulo, simboloValorNulo;

    // Variables registro
    int id;
    RawImage simbolo;

    void Start()
    {
        query = new SQLQuery("BaseLogopeda");

        for (int i = 0; i < botonesConfirmar.Length; i++)
        {
            botonesConfirmar[i].onClick.AddListener(ActualizarDefinicion);
        }

        RellenarTabla tablasManager = FindObjectOfType<RellenarTabla>();
        simboloValorNoNulo = tablasManager.simboloValorNoNulo;
        simboloValorNulo = tablasManager.simboloValorNulo;
    }

    public void ActualizarInterfaz(int id, RawImage tex)
    {
        query.Query("SELECT Definicion FROM Palabras WHERE ID_Palabra =" + id);
        List<string> reader = query.StringReader(1);

        inputUsuario.text = reader[0];

        this.id = id;
        simbolo = tex;
    }

    void ActualizarDefinicion()
    {
        if (inputUsuario.text != "") // Si hay algo escrito
        {
            // Modificar la definición en la BDD
            query.Query("UPDATE Palabras SET Definicion = '" + inputUsuario.text + "' WHERE ID_Palabra =" + id);

            if (simbolo.texture == simboloValorNulo) // Y no había antes una definición
            {
                simbolo.texture = simboloValorNoNulo;
                simbolo.GetComponentInParent<Text>().text = "";
            }
        }
        else if (simbolo.texture == simboloValorNoNulo) // Si no hay nada escrito y antes había definición
        {
            // Borrar la definición de la BDD
            query.Query("UPDATE Palabras SET Definicion = NULL WHERE ID_Palabra =" + id);

            simbolo.texture = simboloValorNulo;
            simbolo.GetComponentInParent<Text>().text = "--";
        }
    }
}