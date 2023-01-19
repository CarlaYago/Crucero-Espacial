using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minijuego4 : MonoBehaviour
{
    //Constructores
    SQLQuery Constructor;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    //Recompensas
    List<string> recompensas = new List<string>();
    List<string> palabrasUsadas = new List<string>();
    List<int> longitudPalabrasRespondidas = new List<int>();

    [Range(1, 11)] public int dificultad;
    int numVocales, numConsonantes;

    readonly char[] abecedarioConsonantes = { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'Ñ', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z' };
    readonly char[] abecedarioVocales = { 'A', 'E', 'I', 'O', 'U' };

    public int numRondas;
    int NumPalabras, palabrasPorRonda, contadorPalabrasRonda = 1, rondaActual = 1;
    public GameObject Letra;
    public Transform Letras, Espacios, Palabras;
    public string PalabraFinal;
    public GameObject PalabraAcertada, TextoDeEjemplo;

    int tiempo;
    public InterfazPanelManager PanelManager;
    public InterfazGeneralManager InterfazManager;
    public float NumAciertos, NumFallos;

    public GameObject menuConfirmar;


    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip acierto, fallo,BandaSonora1, BandaSonora2, BandaSonora3;
    void CargarNivel()
    {
        progresoManager = new ProgresoMinijuegos();
        int[] difRondas = progresoManager.DificultadRondas();

        dificultad = difRondas[0];
        numRondas = difRondas[1];
    }

    void Start()
    {
        menuConfirmar.SetActive(false);
        CargarNivel();

        Dificultades();
        InterfazManager.GameTime = tiempo;
        InterfazManager.empezarTemporizador = true;
        InterfazManager.progresoJugador = NumAciertos;
        InterfazManager.maxProgreso = palabrasPorRonda * numRondas;
        InterfazManager.maxRondas = numRondas;
        InterfazManager.progresoRondas = rondaActual;

        SpawnLetras();

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        AudioSource3 = audioManager.AudioSource3;

        BandaSonora1 = audioManager.BandaSonora1;
        BandaSonora2 = audioManager.BandaSonora2;
        BandaSonora3 = audioManager.BandaSonora3;
        acierto = audioManager.acierto;
        fallo = audioManager.fallo;

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

    public void ost(AudioClip musica)
    {
        AudioSource3.clip = musica;
        AudioSource3.Play();
    }
    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void comprobarsonidobienmal()
    {
        if (InterfazManager.GetComponent<InterfazGeneralManager>().error == true)
        {
            playSound(fallo);
        }
        else
        {
            playSound(acierto);
        }

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !PanelManager.gameObject.activeSelf)
        {
            Correccion();
        }

        if (InterfazManager.GameTime <= 0)
        {
            contadorPalabrasRonda = rondaActual * palabrasPorRonda;

            if (rondaActual < numRondas)
            {
                rondaActual++;
                SpawnLetras();
                Resetear(true);
                NumPalabras = 0;
            }
            else
            {
                progresoManager.DatosFinales(InterfazManager.porcentajeBarra);
                InterfazManager.empezarTemporizador = false;
                InterfazManager.panel.SetActive(true);
                PanelManager.nadaOExtra = longitudPalabrasRespondidas;
                PanelManager.numPalabras = recompensas.Count;
                PanelManager.num = InterfazManager.porcentajeBarra;
                PanelManager.gameObject.SetActive(true);
            }

            InterfazManager.progresoRondas = rondaActual;
            InterfazManager.GameTime = tiempo;
            contadorPalabrasRonda = 1;
        }
    }

    void SpawnLetras()
    {
        foreach (Transform child in Letras)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numConsonantes; i++)
        {
            GameObject letraNueva = Instantiate(Letra, Letras);
            letraNueva.GetComponent<Text>().text = abecedarioConsonantes[Random.Range(0, 22)].ToString();
        }

        for (int i = 0; i < numVocales; i++)
        {
            GameObject letraNueva = Instantiate(Letra, Letras);
            letraNueva.GetComponent<Text>().text = abecedarioVocales[Random.Range(0, 5)].ToString();
        }
    }

    public void Correccion()
    {
        for (int i = 0; i < Espacios.childCount; i++)
        {
            PalabraFinal += Espacios.GetChild(i).GetComponent<Text>().text;
        }

        if (NumPalabras < palabrasPorRonda)
        {
            ComprobarDiccionario();
        }
    }

    void SpawnPalabraAcertada()
    {
        GameObject PalabraInstanciada = Instantiate(PalabraAcertada, Palabras);
        PalabraInstanciada.GetComponent<Text>().text = PalabraFinal;
        PalabraInstanciada.transform.SetParent(Palabras);
        NumPalabras++;

        Canvas.ForceUpdateCanvases();
        Palabras.GetComponent<HorizontalLayoutGroup>().enabled = false;
        Palabras.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    void ComprobarDiccionario() // Cambiar para usar el constructor de busca de identificadores (+ quitar identificadores antes de instanciar frase)
    {
        if (palabrasUsadas.Contains(PalabraFinal) == false)
        {
            Constructor = new SQLQuery("BaseLogopeda");
            Constructor.Query("Select Palabra From Palabras WHERE ID_Categoria IS NOT NULL");
            List<string> palabrasLogopeda = Constructor.StringReader(1);

            Debug.Log("palabrasLogopeda");

            if (palabrasLogopeda.Contains(PalabraFinal))
            {
                recompensas.Add(PalabraFinal);
                palabrasUsadas.Add(PalabraFinal);
                longitudPalabrasRespondidas.Add(PalabraFinal.Length);
                recompensasManager.RarezaObtenida(PalabraFinal);

                SpawnPalabraAcertada();
                AvanceProgreso();
                SumaRondas();
            }
            else
            {
                Constructor = new SQLQuery("LexicoEsp");
                Constructor.Query("Select Palabra From LexicoEsp");
                List<string> palabrasDiccionario = Constructor.StringReader(1);

                Debug.Log("palabrasDiccionario");

                if (palabrasDiccionario.Contains(PalabraFinal))
                {
                    palabrasUsadas.Add(PalabraFinal);
                    longitudPalabrasRespondidas.Add(PalabraFinal.Length);
                    SpawnPalabraAcertada();
                    AvanceProgreso();
                    SumaRondas();
                    comprobarsonidobienmal();
                }
                else
                {
                    Debug.Log("Error");
                    Resetear(false);
                    InterfazManager.error = true;
                    comprobarsonidobienmal();
                }
            }
        }

        PalabraFinal = "";
    }

    void SumaRondas()
    {
        if (contadorPalabrasRonda < palabrasPorRonda)
        {
            contadorPalabrasRonda++;
            Resetear(false);
        }
        else
        {
            if (rondaActual < numRondas)
            {
                InterfazManager.GameTime = tiempo;

                contadorPalabrasRonda = 1;
                rondaActual++;
                SpawnLetras();
                Resetear(true);
                NumPalabras = 0;
                Debug.Log("Ronda nueva, palabras 0");
            }
            else
            {
                progresoManager.DatosFinales(InterfazManager.porcentajeBarra);
                InterfazManager.empezarTemporizador = false;
                InterfazManager.panel.SetActive(true);
                PanelManager.nadaOExtra = longitudPalabrasRespondidas;
                PanelManager.numPalabras = recompensas.Count;
                PanelManager.num = InterfazManager.porcentajeBarra;
                PanelManager.gameObject.SetActive(true);
            }
        }

        InterfazManager.progresoRondas = rondaActual;
    }

    void AvanceProgreso()
    {
        NumAciertos++;
        InterfazManager.progresoJugador = NumAciertos;
        InterfazManager.porcentajeBarra = InterfazManager.progresoJugador / InterfazManager.maxProgreso;
        InterfazManager.barraProgreso.GetComponent<Slider>().value = InterfazManager.porcentajeBarra;
    }

    public void Resetear(bool rondaNueva)
    {
        int childCount = Espacios.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (!rondaNueva) Espacios.GetChild(0).SetParent(Letras);
            else Destroy(Espacios.GetChild(i).gameObject);
        }

        ActualizarTextoDeEjemplo();
    }

    public void ActualizarTextoDeEjemplo()
    {
        if (Espacios.childCount != 0)
        {
            TextoDeEjemplo.GetComponent<Text>().text = "";
        }
        else
        {
            TextoDeEjemplo.GetComponent<Text>().text = "Arrastra las letras aquí...";
        }
    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    numVocales = 4;
                    numConsonantes = 5;
                    palabrasPorRonda = 1;
                    tiempo = 60;

                    recompensasManager = new RarezaPalabras(4);
                    break;
                }
            case 2:
                {
                    numVocales = 4;
                    numConsonantes = 5;
                    palabrasPorRonda = 2;
                    tiempo = 60;

                    recompensasManager = new RarezaPalabras(5);
                    break;
                }
            case 3:
                {
                    numVocales = 4;
                    numConsonantes = 5;
                    palabrasPorRonda = 3;
                    tiempo = 60;

                    recompensasManager = new RarezaPalabras(5);
                    break;
                }
            case 4:
                {
                    numVocales = 4;
                    numConsonantes = 5;
                    palabrasPorRonda = 4;
                    tiempo = 60;

                    recompensasManager = new RarezaPalabras(9);
                    break;
                }
            case 5:
                {
                    numVocales = 5;
                    numConsonantes = 6;
                    palabrasPorRonda = 5;
                    tiempo = 60;

                    recompensasManager = new RarezaPalabras(13);
                    break;
                }
            case 6:
                {
                    numVocales = 5;
                    numConsonantes = 6;
                    palabrasPorRonda = 6;
                    tiempo = 75;

                    recompensasManager = new RarezaPalabras(22);
                    break;
                }
            case 7:
                {
                    numVocales = 5;
                    numConsonantes = 6;
                    palabrasPorRonda = 7;
                    tiempo = 75;

                    recompensasManager = new RarezaPalabras(19, 1);
                    break;
                }
            case 8:
                {
                    numVocales = 5;
                    numConsonantes = 6;
                    palabrasPorRonda = 8;
                    tiempo = 90;


                    recompensasManager = new RarezaPalabras(16, 2);
                    break;
                }
            case 9:
                {
                    numVocales = 5;
                    numConsonantes = 6;
                    palabrasPorRonda = 9;
                    tiempo = 90;

                    recompensasManager = new RarezaPalabras(23, 3);
                    break;
                }
            case 10:
                {
                    numVocales = 6;
                    numConsonantes = 7;
                    palabrasPorRonda = 10;
                    tiempo = 100;

                    recompensasManager = new RarezaPalabras(27, 5);
                    break;
                }
            case 11:
                {
                    numVocales = 6;
                    numConsonantes = 7;
                    palabrasPorRonda = 11;
                    tiempo = 120;

                    recompensasManager = new RarezaPalabras(25, 6);
                    break;
                }
        }
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

        if (rondaActual < numRondas)
        {
            InterfazManager.GameTime = tiempo;

            contadorPalabrasRonda = 1;
            rondaActual++;
            SpawnLetras();
            Resetear(true);
            NumPalabras = 0;
        }
        else
        {
            progresoManager.DatosFinales(InterfazManager.porcentajeBarra);
            InterfazManager.empezarTemporizador = false;
            InterfazManager.panel.SetActive(true);

            PanelManager.numPalabras = recompensas.Count;
            PanelManager.num = InterfazManager.porcentajeBarra;
            PanelManager.numEXP = 5; //exp provisional
            PanelManager.gameObject.SetActive(true);
        }

        InterfazManager.progresoRondas = rondaActual;

    }
}