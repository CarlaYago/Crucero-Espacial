using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PalabraNueva : MonoBehaviour
{
    SQLQuery consultas;
    BorrarPalabras eliminarScript;

    [Header("Referencias panel")]
    public InputField inputField;
    public Button botonConfirmar;
    public Text mensajeError;

    [Header("Referencias tabla")]
    public Transform contenido;

    [Header("Prefabs")]
    public GameObject filaPrefab;

    void Start()
    {
        consultas = new SQLQuery("BaseLogopeda");
        eliminarScript = FindObjectOfType<BorrarPalabras>();

        botonConfirmar.onClick.AddListener(IntroducirPalabra);

        mensajeError.enabled = false;
        inputField.onValueChanged.AddListener(delegate { mensajeError.enabled = false; });
    }

    void IntroducirPalabra()
    {
        consultas.Query("SELECT Palabra FROM Palabras WHERE Palabra = '" + inputField.text + "'");

        if (consultas.Count() > 0)
        {
            mensajeError.enabled = true;
        }
        else
        {
            string palabra = inputField.text;

            // Insertar la palabra nueva
            consultas.Query("INSERT INTO Palabras (Palabra) VALUES ('" + palabra + "')");

            // Leer su ID por base de datos
            consultas.Query("SELECT ID_Palabra FROM Palabras WHERE Palabra = '" + palabra + "'");
            int id = consultas.IntReader(1)[0];

            // Insertar la ID nueva en la tabla dificultades
            consultas.Query("INSERT INTO Dificultad (ID_Entrada, Identificador, Minijuego3) VALUES (" + id + ", 'Palabra', " + DifAnagramas(palabra) + ")");

            // Instanciar una fila nueva para la palabra nueva
            GameObject filaNueva = Instantiate(filaPrefab, contenido);

            // Mostrar la palabra en la fila creada
            Text palabraNueva = filaNueva.transform.GetChild(0).GetComponent<Text>();
            palabraNueva.text = palabra;

            // Enviar su ID al identificador de la fila para que sus valores de dificultad se lean y actualizen correctamente
            Identificador identificador = filaNueva.transform.GetChild(0).GetComponent<Identificador>();
            identificador.id = id;

            // Asignar la función de eliminar a la fila nueva
            Button botonEliminar = filaNueva.transform.GetChild(6).GetComponentInChildren<Button>();
            botonEliminar.onClick.AddListener(delegate { eliminarScript.PanelEliminar(palabraNueva); });
        }
    }

    int DifAnagramas(string palabra)
    {
        // Minijuegos configurables de palabras: 1, 2, 3, 5, 7, 8, 16
        // (Minijuegos con imágenes no incluidos: 1, 2)
        // (Minijuegos con categorías no incluidos: 5, 7)
        // (Minijuegos con descripción no incluidos: 8)
        // (Minijuegos con sinónimos no incluidos: 16)

        // -> El único minijuego con palabras que se puede configurar automáticamente es anagramas (3)

        int longitudPalabra = palabra.Length;
        int dificultadAnagramas;

        if (longitudPalabra < 4) dificultadAnagramas = 0;
        else if (longitudPalabra == 4) dificultadAnagramas = 1; // Dif 1 = Palabras de 4 letras
        else if (longitudPalabra < 7) dificultadAnagramas = 2;  // Dif 2 = Palabras de 5 a 6 letras
        else dificultadAnagramas = 3;                           // Dif 3 = Palabras de 7+ letras

        return dificultadAnagramas;
    }
}