using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ManagerCartas : MonoBehaviour, IPointerEnterHandler
{
    SQLQuery consultas;
    DatosJugador datos;

    [Header("Stats")]
    public int nivel;
    public int cantidad;
    public string palabra;
    public int id;
    public int restarCartas;
    public bool nueva;

    [Header("Referencias")]
    public TextMeshProUGUI texto, texto2;
    public List<Sprite> rareza;
    public Image borde;
    public TextMeshProUGUI textoCantidad;
    public Button levelear;
    public Image slide, nuevaImg;

    public Sprite[] assets;

    //no publico
    bool LevelUP;
    ManagerAlmacen Almacen;

    [Header("Audio")]
    AudioSource reproductor;
    AudioClip SubidaCarta;

    void Start()
    {
        Restart();
        Almacen = FindObjectOfType<ManagerAlmacen>();

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        SubidaCarta = audioManager.SubidaCarta;
    }

    void Update()
    {
        if (cantidad >= restarCartas && nivel < 5)
        {
            LevelUP = true;
            levelear.interactable = true;
            levelear.image.enabled = true;
            levelear.image.sprite = assets[nivel];
        }
        else
        {
            LevelUP = false;
            levelear.interactable = false;
            levelear.image.enabled = false;
        }
    }
    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void SubirNivel()
    {
        if (LevelUP == true && nivel < 5)
        {
            cantidad -= restarCartas;
            Almacen.Instanciar(nivel + 1, id, palabra, 1, true);
            ActuBarra();
            Almacen.parar = false;

            SubirNivelBDD();
            playSound(SubidaCarta);
            if (cantidad == 0) Destroy(gameObject);
        }
    }

    public void Restart()
    {
        texto.text = palabra;
        texto2.text = palabra;
        borde.sprite = rareza[nivel];
        if (!nueva) nuevaImg.gameObject.SetActive(false);
        ActuBarra();
    }

    public void ActuBarra()
    {
        if (cantidad < restarCartas)
        {
            slide.fillAmount = (float)cantidad / restarCartas;
        }
        else
        {
            slide.fillAmount = 1;
        }
        textoCantidad.text = cantidad.ToString() + "/" + restarCartas.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (nueva)
        {
            nuevaImg.gameObject.SetActive(false);

            // Acceder a la ID de esta palabra
            consultas = new SQLQuery("BaseLogopeda");
            consultas.Query("SELECT ID_Palabra FROM Palabras WHERE Palabra = '" + palabra + "'");
            int id = consultas.IntReader(1)[0];

            // Actualizar la BDD para que ya no aparezca como nueva
            datos = FindObjectOfType<DatosJugador>();
            consultas = new SQLQuery("Usuarios");
            consultas.Query("UPDATE Almacen_" + datos.id + " SET Nueva = 0 WHERE ID_Palabra = " + id);
        }
    }

    void SubirNivelBDD()
    {
        consultas = new SQLQuery("Usuarios");
        datos = FindObjectOfType<DatosJugador>();

        // Comprobar si se tienen más de 10 cartas de este tipo
        consultas.Query("SELECT Cantidad FROM Almacen_" + datos.id + " WHERE ID_Palabra = " + id + " AND Rareza = " + nivel);
        int cantidadAntigua = consultas.IntReader(1)[0];
        //
        // En caso de tener más de 10, restarle 10 a su cantidad, si no, eliminar la carta de BDD
        if (cantidadAntigua > 10) consultas.Query("UPDATE Almacen_" + datos.id + " SET Cantidad = " + (cantidadAntigua - 10) + " WHERE ID_Palabra = " + id + " AND Rareza = " + nivel);
        else consultas.Query("DELETE FROM Almacen_" + datos.id + " WHERE ID_Palabra = " + id + " AND Rareza = " + nivel);

        // Comprobar si ya se tienen más cartas de este tipo con rareza superior
        consultas.Query("SELECT Cantidad FROM Almacen_" + datos.id + " WHERE ID_Palabra = " + id + " AND Rareza = " + nivel + 1);
        //
        // En caso de tenerlas, subir su cantidad, si no, añadir la carta a BDD
        if (consultas.Count() > 0)
        {
            int cantidadNueva = consultas.IntReader(1)[0];
            consultas.Query("UPDATE Almacen_" + datos.id + " SET Cantidad = " + (cantidadNueva + 1) + ", Nueva = 1 WHERE ID_Palabra = " + id + " AND Rareza = " + (nivel + 1));
        }
        else consultas.Query("INSERT INTO Almacen_" + datos.id + " VALUES (" + id + "," + nivel + 1 + ", 1, 1)");
    }
}