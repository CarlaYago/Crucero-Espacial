using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NuevaFrase : MonoBehaviour
{
    SQLQuery query;
    DetectarIdentificadores detector;

    //Interfaz cambio definición
    public Button[] botonesConfirmar;
    public TMP_InputField inputUsuario;

    public Text mensajeError;

    // Variables registro
    int id;
    Text frase;

    void Start()
    {
        query = new SQLQuery("BaseLogopeda");
        detector = new DetectarIdentificadores("<b>", "</b>");

        mensajeError.enabled = false;

        for (int i = 0; i < botonesConfirmar.Length; i++)
        {
            botonesConfirmar[i].onClick.AddListener(ActualizarDefinicion);
        }

        RellenarTabla tablasManager = FindObjectOfType<RellenarTabla>();
    }

    public void ActualizarInterfaz(int id, Text fraseText)
    {
        query.Query("SELECT Frase FROM FrasesTexto WHERE ID_Frase =" + id);
        string fraseBDD = query.StringReader(1)[0];
        string frase = detector.QuitarIdentificadores(fraseBDD);

        inputUsuario.text = frase;

        this.id = id;
        this.frase = fraseText;
    }

    void ActualizarDefinicion()
    {
        if (inputUsuario.text != "") // Si hay algo escrito
        {
            string textoActualizado = detector.AsignarIdentificadores(inputUsuario.text.ToUpper());

            // Modificar la frase en la BDD
            query.Query("UPDATE FrasesTexto SET Frase = '" + textoActualizado + "' WHERE ID_Frase =" + id);

            // Mostrarla en la interfaz
            frase.text = textoActualizado;
        }
        else // Si no hay nada escrito
        {
            // Mostrar el mensaje de error
            mensajeError.enabled = true;
        }
    }
}