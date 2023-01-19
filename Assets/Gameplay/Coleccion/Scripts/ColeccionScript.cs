using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class ColeccionScript : MonoBehaviour
{
    SQLQuery consultas;
    DatosJugador datos;

    [Header("Referencias Escena")]
    public GameObject canvasColeccion;
    public GameObject canvasInfo, canvasMinijuegoCanvas;
    public Transform categoriaParent, minijuegoParent;

    [Header("Referencias Assets")]
    public Sprite[] cartasPalabras;

    [Header("Stats")]
    public GameObject botonSeleccionado;
    public GameObject botonRecompensa;

    [Header("Prefabs")]
    public GameObject categoriaPrefab;
    public GameObject palabraPrefab; // tendra que cambiarse por gameobject, al cual le cambiaremos la palabra 
    public GameObject palabraMinijuego;

    [Header("Funcionamiento")]
    public bool palabrasDisponibles = true;
    public Sprite[] assets;

    [Header("Audio")]
    AudioSource reproductor;
    AudioClip SubidaCarta;

    void Start()
    {
        canvasColeccion.SetActive(true);
        canvasInfo.SetActive(false);
        canvasMinijuegoCanvas.SetActive(false);

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        SubidaCarta = audioManager.SubidaCarta;

        InstanciarCategorias();
    }
    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    void InstanciarCategorias()
    {
        consultas = new SQLQuery("BaseLogopeda");
        datos = FindObjectOfType<DatosJugador>();

        consultas.Query("SELECT * FROM Categorias");
        Dictionary<int, string> categorias = consultas.StringReaderID(1, 2);

        for (int i = 0; i < categorias.Count; i++)
        {
            int idCategoria = categorias.ElementAt(i).Key;
            string nombreCategoria = categorias.ElementAt(i).Value;

            GameObject cat = Instantiate(categoriaPrefab, categoriaParent);

            // Mostrar el nombre de la categoría en el prefab
            TextMeshProUGUI nombre = cat.GetComponent<TextMeshProUGUI>();
            nombre.text = nombreCategoria + " (NIVEL ";

            // Cambiar el nivel de la categoría
            int nivel = datos.catNivel_BDD[idCategoria];
            CategoriasScript catScript = cat.GetComponent<CategoriasScript>();
            catScript.contadorNivel = nivel;


            // Asociar la función del minijuego de la colección al botón de palabra nueva
            Button btnPalabra = cat.transform.GetChild(1).GetComponentInChildren<Button>();
            btnPalabra.onClick.AddListener(delegate { MinijuegoColeccion(idCategoria, datos.catNivel_BDD[idCategoria]); });

            // Asociar la función de recompensa de la colección al botón de recompensa
            Button btnRecompensa = cat.transform.GetChild(6).GetChild(0).GetComponent<Button>();
            btnRecompensa.onClick.AddListener(delegate { Recompensa(idCategoria); });


            // Leer cuántas palabras hay en la categoría actual y mover el botón las veces que corresponda
            botonSeleccionado = btnPalabra.gameObject;
            if (datos.catPalabrasID_BDD.ContainsKey(idCategoria))
            {
                int[] idPalabras = datos.catPalabrasID_BDD[idCategoria];
                int numPalabras = idPalabras.Length;

                for (int n = 0; n < numPalabras; n++)
                {
                    consultas.Query("SELECT Palabra FROM Palabras WHERE ID_Palabra = " + idPalabras[n]);
                    string palabra = consultas.StringReader(1)[0];

                    MoverBoton(palabra, nivel);
                    ComprobarSiHayCartas(nivel);
                }
            }
            // Si la categoría actual no tiene palabras, comprobar si hay palabras disponibles para colocar
            else ComprobarSiHayCartas(nivel);
        }
    }

    public void MinijuegoColeccion(int idCategoria, int rarezaCategoria)
    {
        consultas.Query("SELECT Palabra FROM Palabras WHERE ID_Categoria != " + idCategoria);
        List<string> palabrasErroneas = consultas.StringReader(1);

        int numPalabrasMinijuego = (rarezaCategoria + 1) * 3;

        // Elegir ID de la palabra correcta
        List<int> idPalabras = IDPalabrasDisponibles(idCategoria, rarezaCategoria);
        int idPalabraCorrecta = idPalabras[Random.Range(0, idPalabras.Count)];
        // Leer la palabra a partir de su ID
        consultas.Query("SELECT Palabra FROM Palabras WHERE ID_Palabra = " + idPalabraCorrecta);
        string palabraCorrecta = consultas.StringReader(1)[0];
        // Elegir una posición donde instanciar la palabra correcta
        int palabraCorrectaIndex = Random.Range(0, numPalabrasMinijuego);

        // Instanciar palabras
        for (int i = 0; i < numPalabrasMinijuego; i++)
        {
            GameObject palabra = Instantiate(palabraMinijuego, minijuegoParent);
            palabra.GetComponent<Image>().sprite = assets[rarezaCategoria];

            if (i != palabraCorrectaIndex)
            {
                string palabraIncorrecta = palabrasErroneas[Random.Range(0, palabrasErroneas.Count)];
                palabra.GetComponentInChildren<TextMeshProUGUI>().text = palabraIncorrecta;
                palabra.GetComponentInChildren<Button>().onClick.AddListener(delegate { CerrarMinijuego(); });
                palabrasErroneas.Remove(palabraIncorrecta);
            }
            else
            {
                palabra.GetComponentInChildren<TextMeshProUGUI>().text = palabraCorrecta;
                palabra.GetComponentInChildren<Button>().onClick.AddListener(delegate { datos.ActualizarColeccion(idCategoria, idPalabraCorrecta); });
                palabra.GetComponentInChildren<Button>().onClick.AddListener(delegate { MoverBoton(palabraCorrecta, rarezaCategoria); ComprobarSiHayCartas(rarezaCategoria); CerrarMinijuego(); });
            }
        }

        canvasColeccion.SetActive(false);
        canvasMinijuegoCanvas.SetActive(true);

        botonSeleccionado = EventSystem.current.currentSelectedGameObject;
    }

    public void MoverBoton(string palabraNueva, int rareza)
    {
        canvasMinijuegoCanvas.SetActive(false);
        canvasColeccion.SetActive(true);
        Transform categoriaTransform = botonSeleccionado.transform.parent.parent;

        GameObject carta = Instantiate(palabraPrefab, botonSeleccionado.transform.parent);
        carta.GetComponentInChildren<TextMeshProUGUI>().text = palabraNueva;
        carta.GetComponent<Image>().sprite = cartasPalabras[rareza];

        if (botonSeleccionado.GetComponent<BotonColeccion>().contador == 4)
        {
            botonSeleccionado.SetActive(false);
            categoriaTransform.GetChild(6).GetChild(0).gameObject.SetActive(true); // Activar el botón de recompensa
        }
        else
        {
            int i = botonSeleccionado.transform.parent.GetSiblingIndex();
            botonSeleccionado.transform.SetParent(categoriaTransform.GetChild(i + 1));
            botonSeleccionado.transform.position = categoriaTransform.GetChild(i + 1).transform.position;
            botonSeleccionado.GetComponent<BotonColeccion>().contador++;
        }
    }

    public void Recompensa(int idCategoria)
    {
        botonRecompensa = EventSystem.current.currentSelectedGameObject;
        Transform categoriaTransform = botonRecompensa.transform.parent.parent;

        categoriaTransform.GetChild(5).GetChild(0).gameObject.SetActive(true);
        categoriaTransform.GetChild(5).GetChild(0).SetParent(categoriaTransform.GetChild(1));
        categoriaTransform.GetChild(1).GetChild(1).transform.position = categoriaTransform.GetChild(1).transform.position;
        categoriaTransform.GetChild(1).GetChild(1).GetComponent<BotonColeccion>().contador = 0;
        categoriaTransform.GetComponent<CategoriasScript>().cambiarTexto = true;

        for (int i = 1; i < 6; i++)
        {
            Destroy(categoriaTransform.GetChild(i).GetChild(0).gameObject);
        }

        botonRecompensa.SetActive(false);

        datos.SubirNivelCategoria(idCategoria);
        playSound(SubidaCarta);
        int rarezaCategoria = categoriaTransform.GetComponent<CategoriasScript>().contadorNivel;
        ComprobarSiHayCartas(rarezaCategoria);
    }

    public void ComprobarSiHayCartas(int nivelCartas)
    {
        palabrasDisponibles = false;

        // Acceder al nombre de la categoría actual
        string[] textoCategoria = botonSeleccionado.transform.parent.GetComponentInParent<TextMeshProUGUI>().text.Split(' ');
        string categoriaActual = textoCategoria[0];

        // Conseguir su ID
        consultas.Query("SELECT ID_Categoria FROM Categorias WHERE Categoria = '" + categoriaActual + "'");
        int idCategoria = consultas.IntReader(1)[0];

        if (IDPalabrasDisponibles(idCategoria, nivelCartas).Count > 0) palabrasDisponibles = true;

        // Si no hay ninguna, el botón de añadir palabras para la categoría actual se deshabilita
        if (!palabrasDisponibles)
        {
            Button btn = botonSeleccionado.GetComponent<Button>();
            btn.enabled = false;
            btn.image.color = new Color(0f, 0f, 0f, 0f);

            botonSeleccionado.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    List<int> IDPalabrasDisponibles(int idCategoria, int nivelCartas)
    {
        // Acceder a la ID de todas las palabras de la categoría actual
        consultas.Query("SELECT ID_Palabra FROM Palabras WHERE ID_Categoria = " + idCategoria);
        List<int> palabrasRestantes = consultas.IntReader(1);

        // Quitar, de la lista de IDs de palabras, las palabras ya añadidas a la categoría
        if (datos.catPalabrasID_BDD.ContainsKey(idCategoria))
        {
            int[] palabrasUsadas = datos.catPalabrasID_BDD[idCategoria];

            for (int i = 0; i < palabrasUsadas.Length; i++)
            {
                palabrasRestantes.Remove(palabrasUsadas[i]);
            }
        }

        List<int> palabrasDisponibles = new List<int>();

        // Comprobar si alguna palabra de las necesarias está en el almacén del usuario
        for (int i = 0; i < palabrasRestantes.Count; i++)
        {
            int idPalabra = palabrasRestantes[i];
            if (datos.palabraID_BDD.Contains(idPalabra)) // ¿Existe la palabra en el almacén?
            {
                int indexAlmacen = datos.palabraID_BDD.IndexOf(idPalabra);
                if (datos.palRareza_BDD[indexAlmacen] == nivelCartas) // ¿Es de la rareza necesaria?
                {
                    palabrasDisponibles.Add(idPalabra);
                }
            }
        }

        return palabrasDisponibles;
    }

    void CerrarMinijuego()
    {
        canvasMinijuegoCanvas.SetActive(false);
        canvasColeccion.SetActive(true);

        foreach (Transform child in minijuegoParent)
        {
            Destroy(child.gameObject);
        }
    }
}