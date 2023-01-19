using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RellenarTabla : MonoBehaviour
{
    // Apariencia UI
    [Header("Contendores interfaz")]
    public Transform palabrasParent;
    public Transform textosParent, frasesParent, secuenciasParent;

    [Header("Prefabs filas")]
    public GameObject filaPalabras;
    public GameObject filaTextos, filaFrases, filaSecuencias;

    // Eliminación
    BorrarPalabras eliminacionPalabras;
    BorrarFrases eliminacionFrases;

    [Header("Otros")]
    public Texture simboloValorNoNulo;
    [HideInInspector] public Texture simboloValorNulo;

    // Palabras
    Dictionary<int, string> palabras, categoria;
    List<Texture2D> imagenes;
    List<string> definicion;
    Dictionary<int, string[]> sinonimos, antonimos;

    // Textos/Frases
    Dictionary<int, string> textos, frases, preguntas;
    List<int> verdadero; // 0 = false, 1 = true, -1 = null

    // Secuencias
    List<string[]> secuencias;

    // BDD
    SQLQuery consultas;

    void Awake()
    {
        simboloValorNulo = filaPalabras.transform.GetChild(3).GetChild(0).GetComponent<RawImage>().texture;
    }

    void Start()
    {
        consultas = new SQLQuery("BaseLogopeda");

        EliminacionRegistros();

        RellenarPalabras();
        RellenarTextos();
        RellenarFrases();
        RellenarSecuencias();
    }

    void RellenarPalabras()
    {
        #region Consultas SQL

        consultas.Query("SELECT ID_Palabra, Palabra" + " FROM Palabras");
        palabras = consultas.StringReaderID(1, 2);

        consultas.Query("SELECT Imagen, Definicion" + " FROM Palabras");
        imagenes = consultas.ImageReader(1, 150);
        definicion = consultas.StringReader(2);

        consultas.Query("SELECT Palabras.ID_Palabra, Categorias.Categoria" + " FROM Palabras" + " INNER JOIN Categorias ON Palabras.ID_Categoria = Categorias.ID_Categoria");
        categoria = consultas.StringReaderID(1, 2);

        string consultaSinonimosAntonimos = "SELECT Palabras.ID_Palabra, SinonimosAntonimos.Palabras" + " FROM Palabras" + " INNER JOIN SinonimosAntonimos ON Palabras.ID_Palabra = SinonimosAntonimos.ID_Palabra";

        consultas.Query(consultaSinonimosAntonimos + " WHERE SinonimosAntonimos.Sinonimo = 1");
        sinonimos = consultas.StringArrayReaderID(1, 2);

        consultas.Query(consultaSinonimosAntonimos + " WHERE SinonimosAntonimos.Sinonimo = 0");
        antonimos = consultas.StringArrayReaderID(1, 2);

        #endregion Consultas SQL

        #region Rellenar Interfaz

        for (int i = 0; i < palabras.Count; i++)
        {
            GameObject pal = Instantiate(filaPalabras, palabrasParent);

            // --- Palabras --- //
            pal.transform.GetChild(0).GetComponent<Text>().text = palabras.ElementAt(i).Value;
            pal.transform.GetChild(0).GetComponent<Identificador>().id = palabras.ElementAt(i).Key;

            // --- Imagenes --- //
            if (imagenes[i] != null) pal.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = imagenes[i];

            int palabraID = palabras.ElementAt(i).Key;

            // --- Categorias --- //
            if (categoria.ContainsKey(palabraID))
            {
                pal.transform.GetChild(2).GetComponent<Text>().text = categoria[palabraID];
            }

            // --- Definiciones --- //
            if (definicion[i] != null)
            {
                pal.transform.GetChild(3).GetComponent<Text>().text = "";
                pal.transform.GetChild(3).GetChild(0).GetComponent<RawImage>().texture = simboloValorNoNulo;
            }

            // --- Sinonimos --- //
            if (sinonimos.ContainsKey(palabraID))
            {
                pal.transform.GetChild(4).GetComponent<Text>().text = sinonimos[palabraID].Count().ToString();
            }

            // --- Antonimos --- //
            if (antonimos.ContainsKey(palabraID))
            {
                pal.transform.GetChild(5).GetComponent<Text>().text = antonimos[palabraID].Count().ToString();
            }

            // --- Botón eliminar --- //
            Text palabraText = pal.transform.GetChild(0).GetComponent<Text>();
            Button botonEliminar = pal.transform.GetChild(6).GetComponentInChildren<Button>();
            botonEliminar.onClick.AddListener(delegate { eliminacionPalabras.PanelEliminar(palabraText); });
        }

        #endregion Rellenar Interfaz
    }

    void RellenarTextos()
    {
        #region Consultas SQL

        consultas.Query("SELECT ID_Frase, Frase" + " FROM FrasesTexto" + " WHERE Texto = 1");
        textos = consultas.StringReaderID(1, 2);

        #endregion Consultas SQL

        #region Rellenar Interfaz

        for (int i = 0; i < textos.Count; i++)
        {
            GameObject tex = Instantiate(filaTextos, textosParent);
            int id = textos.ElementAt(i).Key;

            // --- Textos --- //
            tex.transform.GetChild(0).GetComponent<Text>().text = textos.ElementAt(i).Value;
            tex.transform.GetChild(0).GetComponent<Identificador>().id = id;

            // --- Preguntas --- //
            consultas.Query("SELECT Pregunta" + " FROM Preguntas" + " WHERE ID_Frase = " + id);
            int numPreguntas = consultas.Count();
            if (numPreguntas > 0) tex.transform.GetChild(1).GetComponent<Text>().text = numPreguntas.ToString();
        }

        #endregion Rellenar Interfaz
    }

    void RellenarFrases()
    {
        #region Consultas SQL

        consultas.Query("SELECT ID_Frase, Frase" + " FROM FrasesTexto" + " WHERE Texto = 0");
        frases = consultas.StringReaderID(1, 2);

        consultas.Query("SELECT Verdadero" + " FROM FrasesTexto" + " WHERE Texto = 0");
        verdadero = consultas.IntReader(1);

        #endregion Consultas SQL

        #region Rellenar Interfaz

        for (int i = 0; i < frases.Count; i++)
        {
            GameObject fra = Instantiate(filaFrases, frasesParent);
            int id = frases.ElementAt(i).Key;

            // --- Frases --- //
            fra.transform.GetChild(0).GetComponent<Text>().text = frases.ElementAt(i).Value;
            fra.transform.GetChild(0).GetComponent<Identificador>().id = id;

            // --- Preguntas --- //
            consultas.Query("SELECT Pregunta" + " FROM Preguntas" + " WHERE ID_Frase = " + id);
            int numPreguntas = consultas.Count();
            if (numPreguntas > 0) fra.transform.GetChild(1).GetComponent<Text>().text = numPreguntas.ToString();

            // --- Verdadero || Falso --- //
            if (verdadero[i] == 0) fra.transform.GetChild(2).GetComponent<Text>().text = "Falso";
            if (verdadero[i] == 1) fra.transform.GetChild(2).GetComponent<Text>().text = "Verdadero";

            // --- Botón eliminar --- //
            Text fraseText = fra.transform.GetChild(0).GetComponent<Text>();
            Button botonEliminar = fra.transform.GetChild(3).GetComponentInChildren<Button>();
            botonEliminar.onClick.AddListener(delegate { eliminacionFrases.PanelEliminar(fraseText); });
        }

        #endregion Rellenar Interfaz
    }

    void RellenarSecuencias()
    {

    }

    void EliminacionRegistros()
    {
        eliminacionPalabras = FindObjectOfType<BorrarPalabras>();
        if (eliminacionPalabras == null) Debug.LogError("Falta añadir el script BorrarPalabras a la escana");

        eliminacionFrases = FindObjectOfType<BorrarFrases>();
        if (eliminacionPalabras == null) Debug.LogError("Falta añadir el script BorrarFrases a la escana");
    }
}