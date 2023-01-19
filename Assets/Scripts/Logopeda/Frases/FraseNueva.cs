using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FraseNueva : MonoBehaviour
{
    SQLQuery consultas;
    BorrarFrases eliminarScript;
    /// <summary>
    /// Detecta palabras que coincidan con la BDD y las pone en negrita
    /// </summary>
    DetectarIdentificadores identificadores;

    [Header("Referencias panel")]
    public TMP_InputField inputField;
    public Button botonConfirmar;
    public Text mensajeError;

    [Header("Referencias tabla")]
    public Transform contenido;

    [Header("Prefabs")]
    public GameObject filaPrefab;

    void Start()
    {
        consultas = new SQLQuery("BaseLogopeda");
        eliminarScript = FindObjectOfType<BorrarFrases>();
        identificadores = new DetectarIdentificadores("<b>", "</b>");

        botonConfirmar.onClick.AddListener(IntroducirPalabra);

        mensajeError.enabled = false;
        inputField.onValueChanged.AddListener(delegate { mensajeError.enabled = false; });
    }

    void IntroducirPalabra()
    {
        string frase = identificadores.AsignarIdentificadores(inputField.text.ToUpper());
        consultas.Query("SELECT Frase FROM FrasesTexto WHERE Frase = '" + frase + "'");

        if (consultas.Count() > 0)
        {
            mensajeError.enabled = true;
        }
        else
        {
            // Insertar la palabra nueva
            consultas.Query("INSERT INTO FrasesTexto (Frase) VALUES ('" + frase + "')");

            // Leer su ID por base de datos
            consultas.Query("SELECT ID_Frase FROM FrasesTexto WHERE Frase = '" + frase + "'");
            int id = consultas.IntReader(1)[0];

            // Insertar la ID nueva en la tabla dificultades
            consultas.Query("INSERT INTO Dificultad (ID_Entrada, Identificador, Minijuego6) VALUES (" + id + ", 'Frase', " + DifFormarFrases(frase) + ")");

            // Instanciar una fila nueva para la palabra nueva
            GameObject filaNueva = Instantiate(filaPrefab, contenido);

            // Mostrar la palabra en la fila creada
            Text fraseNueva = filaNueva.transform.GetChild(0).GetComponent<Text>();
            fraseNueva.text = frase;

            // Enviar su ID al identificador de la fila para que sus valores de dificultad se lean y actualizen correctamente
            Identificador identificador = filaNueva.transform.GetChild(0).GetComponent<Identificador>();
            identificador.id = id;

            // Asignar la función de eliminar a la fila nueva
            Button botonEliminar = filaNueva.transform.GetChild(3).GetComponentInChildren<Button>();
            botonEliminar.onClick.AddListener(delegate { eliminarScript.PanelEliminar(fraseNueva); });
        }
    }

    int DifFormarFrases(string frase)
    {
        // -> El minijuego con frases que se puede configurar automáticamente es el de ordenar palabras de frases (6) 

        int longitudFrase = frase.Split(' ').Length;
        int dificultadFormarFrases;

        if (longitudFrase < 4) dificultadFormarFrases = 0;
        else if (longitudFrase < 15) dificultadFormarFrases = longitudFrase - 3;
        else dificultadFormarFrases = 11;

        return dificultadFormarFrases;
    }
}