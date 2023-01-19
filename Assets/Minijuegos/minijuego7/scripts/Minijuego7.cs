using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Minijuego7 : MonoBehaviour
{
    SQLQuery consultas;
    public RarezaPalabras recompensasManager;
    List<string> recompensas = new List<string>();
    ProgresoMinijuegos progresoManager;

    [Header("Listas")]
    public List<string> listaPalabrasCorrectas = new List<string>();
    public List<string> listaImpostores = new List<string>();

    [Header("Referencias")]
    public Transform palabrasParent;
    public Button palabrasBotonPrefab;
    InterfazGeneralManager interfazScript;
    public GameObject recompensasPrefab;
    InterfazPanelManager recompensasScript;

    [Header("Interfaz")]
    public TextMeshProUGUI enunciado;
    public GameObject rondasText;
    public GameObject menuConfirmar;

    [Header("Parámetros")]
    [Range(1, 11)] public int dificultad;
    public int rondas;
    public int rondaActual = 1; // es publico para poder acceder desde el boton, si fuese necesario se cambia todo a este script
    public int imp;
    public int impostoresEnc;
    public bool booltiempo;
    int time;
    public int fallados = 0;
    public int exp;
    public float porcentaje;
    public List<string> palabrasConseguidas = new List<string>();

    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip acierto, fallo,BandaSonora1, BandaSonora2, BandaSonora3;

    void CargarNivel()
    {
        progresoManager = new ProgresoMinijuegos();
        int[] difRondas = progresoManager.DificultadRondas();

        dificultad = difRondas[0];
        rondas = difRondas[1];
    }

    private void Start()
    {
        menuConfirmar.SetActive(false);
        CargarNivel();

        // si la ronda es = 1 se desactiva---------
        rondasText.SetActive(true);
        if (rondas == 1)
        {
            rondasText.SetActive(false);
        }
        //-----------------------------------------
        interfazScript = FindObjectOfType<InterfazGeneralManager>();

        AccesoABDD();
        Dificultades();
        IniciarInterfaz();

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        acierto = audioManager.acierto;
        fallo = audioManager.fallo;
        AudioSource3 = audioManager.AudioSource3;
        BandaSonora1 = audioManager.BandaSonora1;
        BandaSonora2 = audioManager.BandaSonora2;
        BandaSonora3 = audioManager.BandaSonora3;

        float MusicaRandom = Random.Range(1, 3);
        switch (MusicaRandom)
        {
            case 1:
                ost(BandaSonora1);
                Debug.Log("1");
                break;

            case 2:
                ost(BandaSonora2);
                Debug.Log("2");
                break;

            case 3:
                ost(BandaSonora3);
                Debug.Log("3");
                break;
        }
    }

    void IniciarInterfaz()
    {
        // (Ronda actual = 1)
        interfazScript.progresoRondas = rondaActual;

        // Mostrar el número de rondas / archivos totales
        interfazScript.maxRondas = rondas;
        interfazScript.maxProgreso = imp * rondas;

        // Cambiar el encabezado según el número de intrusos
        if (imp > 1)
        {
            enunciado.text = "Encuentra los " + imp + " intrusos";
        }
        else
        {
            enunciado.text = "Encuentra el intruso";
        }
    }

    void Update()
    {
        if (interfazScript.GameTime <= 0)
        {
            if (booltiempo)
            {
                // int marcador = imp * (rondaActual);
                //nterfazScript.progresoJugador = marcador;

                interfazScript.progresoRondas = rondaActual;
                interfazScript.error = true;
                booltiempo = false;
                comprobarsonidobienmal();

                if (rondaActual < rondas)
                {
                    Comprobar();
                    Dificultades();
                    impostoresEnc = 0;
                }
                else if (rondaActual >= rondas)
                {
                    interfazScript.progresoRondas = rondas;
                    StartCoroutine("ActivarRecompensas");
                    impostoresEnc = 0;

                }
            }
        }
    }

    void AccesoABDD()
    {
        consultas = new SQLQuery("BaseLogopeda");

        consultas.Query("SELECT ID_Categoria FROM Categorias"); // Acceder a las IDs de todas las categorías para elegir una aleatoriamente
        List<int> idCategorias = consultas.IntReader(1);
        int categoriaElegida = idCategorias[Random.Range(0, idCategorias.Count)];

        idCategorias.Remove(categoriaElegida); // Quitar la categoría elegida para usar las demás al encontrar intrusos

        consultas.Query("SELECT Palabra FROM Palabras WHERE ID_Categoria = " + categoriaElegida);
        listaPalabrasCorrectas = consultas.StringReader(1);

        consultas.Query("SELECT Palabra FROM Palabras WHERE ID_Categoria != " + categoriaElegida);
        listaImpostores = consultas.StringReader(1);
    }

    void Instanciar(int numPalabras, int numImpostores)
    {
        interfazScript.GameTime = time;
        booltiempo = true;

        imp = numImpostores;

        int contImp = 0, contPal = 0;
        int palabrasTotales = numPalabras + numImpostores;

        for (int i = 0; i < palabrasTotales; i++)
        {
            float probImp = (numImpostores - contImp) / (float)(palabrasTotales - i);
            float prob = Random.Range(0f, 1f);

            if (prob <= probImp)
            {
                int n = Random.Range(0, listaImpostores.Count); // Obtener un impostor aleatorio

                Button btn = Instantiate(palabrasBotonPrefab, palabrasParent); // Instanciar la palabra
                btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = listaImpostores[n];
                btn.gameObject.tag = "Impostor";

                listaImpostores.RemoveAt(n); // Quitar la palabra de la lista de impostores para que no se repita
                contImp++;
            }
            else
            {
                int n = Random.Range(0, listaPalabrasCorrectas.Count); // Obtener una palabra aleatoria

                Button btn = Instantiate(palabrasBotonPrefab, palabrasParent); // Instanciar la palabra
                btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = listaPalabrasCorrectas[n];

                listaPalabrasCorrectas.RemoveAt(n); // Quitar la palabra de la lista de palabras para que no se repita
                contPal++;
            }
        }

        interfazScript.empezarTemporizador = true;
    }

    public void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    time = 60;
                    Instanciar(3, 1);

                    recompensasManager = new RarezaPalabras(4);
                    break;
                }
            case 2:
                {
                    time = 60;
                    Instanciar(5, 1);

                    recompensasManager = new RarezaPalabras(7);
                    break;
                }
            case 3:
                {
                    time = 60;
                    Instanciar(6, 2);

                    recompensasManager = new RarezaPalabras(8);
                    break;
                }
            case 4:
                {
                    time = 60;
                    Instanciar(8, 2);

                    recompensasManager = new RarezaPalabras(12);
                    break;
                };
            case 5:
                {
                    time = 60;
                    Instanciar(9, 3);

                    recompensasManager = new RarezaPalabras(13);
                    break;
                };
            case 6:
                {
                    time = 45;
                    Instanciar(9, 3);

                    recompensasManager = new RarezaPalabras(27, 1);
                    break;
                };
            case 7:
                {
                    time = 45;
                    Instanciar(10, 4);

                    recompensasManager = new RarezaPalabras(25, 1);
                    break;
                };
            case 8:
                {
                    time = 30;
                    Instanciar(10, 4);

                    recompensasManager = new RarezaPalabras(38, 3);
                    break;
                };
            case 9:
                {
                    time = 30;
                    Instanciar(11, 5);

                    recompensasManager = new RarezaPalabras(40, 5);
                    break;
                };
            case 10:
                {
                    time = 30;
                    Instanciar(12, 6);

                    recompensasManager = new RarezaPalabras(45, 9);
                    break;
                };
            case 11:
                {
                    time = 30;
                    Instanciar(13, 7);

                    recompensasManager = new RarezaPalabras(39, 10);
                    break;
                };
            default:
                break;
        }
    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void ost(AudioClip musica)
    {
        AudioSource3.clip = musica;
        AudioSource3.loop = true;
        AudioSource3.Play();
    }

    public void comprobarsonidobienmal()
    {
        if (interfazScript.error == true)
        {
            playSound(fallo);
        }
        else
        {
            playSound(acierto);
        }

    }

    public void Comprobar()
    {
        AccesoABDD();
        rondaActual++;
        comprobarsonidobienmal();

        interfazScript.progresoRondas = rondaActual;
        for (int i = 0; i < palabrasParent.childCount; i++)
        {
            Destroy(palabrasParent.GetChild(i).gameObject);
        }
    }

    //public void TerminarRecompensas()
    //{
    //    interfazScript.panel.SetActive(false);
    //    recompensasPrefab.SetActive(false);
    //    Debug.Log("juego acabado");
    //    interfazScript.empezarTemporizador = false;
    //}

    public IEnumerator ActivarRecompensas()
    {
        interfazScript.empezarTemporizador = false;
        progresoManager.DatosFinales(porcentaje);
        interfazScript.panel.SetActive(true);

        yield return new WaitForSeconds(0f);
        recompensasPrefab.SetActive(true);
        recompensasScript = FindObjectOfType<InterfazPanelManager>();
        recompensasScript.numEXP = exp;
        recompensasScript.num = porcentaje;
        recompensasScript.numPalabras = palabrasConseguidas.Count;
    }

    public void pasarRonda()
    {
        menuConfirmar.SetActive(true);
    }

    public void cancelar()
    {
        menuConfirmar.SetActive(false);
    }

    public void confirmar()
    {
        menuConfirmar.SetActive(false);

        if (rondaActual < rondas)
        {
            Comprobar();
            Dificultades();
            impostoresEnc = 0;
        }
        else if (rondaActual >= rondas)
        {
            interfazScript.progresoRondas = rondas;
            StartCoroutine("ActivarRecompensas");
            impostoresEnc = 0;

        }
    }
}