using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Minijuego2 : MonoBehaviour
{
    [Header("Listas")]
    public List<Texture2D> listaImagenes;
    public List<string> listaPalabras;
    List<int> listaNumeros = new List<int>();
    public List<string> PalabrasRecompensa;

    [Header("Textos")]
    public Text texto;
    public Text countdownText;

    [Header("Variables")]
    public int dificultad;
    public int palabrasAcertadas = 0;
    public float ColumnCount;
    int contador;

    public float currentTime = 1f;
    public float normalTimer;
    public float difficultTimer;
    int startingTime;
    public float porcentaje;

    public bool corregirUnaVez = false;
    public bool corregirDesdeTimeOut = false;

    int numeroImagenes;

    [Header("Extra")]
    public GameObject panel;
    public RawImage rawImage;
    public Transform imgParentTransform, textParentTransform;
    public GameObject imgParent;
    public float porcentajeBarra;
    public float progreso_Jugador;
    public float max_Progreso;

    [Header("Interfaz")]
    public GameObject interfazGeneral;
    public GameObject RecompensasPrefab;
    public GameObject scrollbarVertical;
    public GameObject menuConfirmar;

    UI_ClickDragDrop[] arrayCDD;
    SQLQuery consultas;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    [Header("Rondas")]
    public int rondas;
    public int rondaActual = 1;

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

    void Awake()
    {
        panel.SetActive(false);
        RecompensasPrefab.SetActive(false);
    }


    void Start()
    {
        menuConfirmar.SetActive(false);

        CargarNivel();
        consultas = new SQLQuery("BaseLogopeda");

        consultas.Query("SELECT Palabra, Imagen" + " FROM Palabras WHERE Imagen IS NOT NULL");

        listaPalabras = consultas.StringReader(1);
        listaImagenes = consultas.ImageReader(2, 150);

        for (int i = 0; i < listaImagenes.Count; i++)
        {
            listaNumeros.Add(i);
        }

        Dificultad();

        rondaActual = 1;
        interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas = rondaActual;
        interfazGeneral.GetComponent<InterfazGeneralManager>().maxRondas = rondas;
        interfazGeneral.GetComponent<InterfazGeneralManager>().maxProgreso = numeroImagenes * rondas;

        //Debug.Log("Rondas: " + rondas);

        arrayCDD = FindObjectsOfType<UI_ClickDragDrop>();

        scrollbarVertical.SetActive(true);

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


    void Update()
    {
        Debug.Log("Rondas: " + rondas);
        Debug.Log("RondaActual: " + rondaActual);

        if (Input.GetKeyDown(KeyCode.Return) && rondaActual <= rondas)
        {
            Corregir();
        }

        if (interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime <= 0)
        {
            if (rondaActual < rondas)
            {
                rondaActual++;
                ResetMinijuego();
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = startingTime;
            }
            else
            {
                progresoManager.DatosFinales(porcentaje);

                RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
                RecompensasPrefab.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
                interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);
                RecompensasPrefab.SetActive(true);
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = false;
            }
        }
    }

    public void Dificultad()
    {
        switch (dificultad)
        {
            case 1:
                numDeImagenes(2);
                numeroImagenes = 2;
                recompensasManager = new RarezaPalabras(2, 0);
                interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                break;

            case 2:
                numDeImagenes(3);
                numeroImagenes = 3;
                recompensasManager = new RarezaPalabras(5, 0);
                interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                break;

            case 3:
                numDeImagenes(4);
                numeroImagenes = 4;
                recompensasManager = new RarezaPalabras(8, 0);
                interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                break;

            case 4:
                numDeImagenes(6);
                numeroImagenes = 6;
                recompensasManager = new RarezaPalabras(7, 0);
                interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                break;

            case 5:
                numDeImagenes(10);
                numeroImagenes = 10;
                recompensasManager = new RarezaPalabras(11, 0);
                interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60f;
                startingTime = 60;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                break;

            case 6:
                numDeImagenes(6);
                numeroImagenes = 6;
                recompensasManager = new RarezaPalabras(20, 0);
                interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60f;
                startingTime = 60;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                break;

            case 7:
                numDeImagenes(9);
                numeroImagenes = 9;
                recompensasManager = new RarezaPalabras(17, 1);
                interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60f;
                startingTime = 60;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                break;

            case 8:
                numDeImagenes(12);
                numeroImagenes = 12;
                recompensasManager = new RarezaPalabras(15, 1);
                interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 75f;
                startingTime = 75;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                break;

            case 9:
                numDeImagenes(16);
                numeroImagenes = 16;
                recompensasManager = new RarezaPalabras(15, 2);
                interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 75f;
                startingTime = 60;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                break;

            case 10:
                numDeImagenes(20);
                numeroImagenes = 20;
                recompensasManager = new RarezaPalabras(20, 4);
                interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 75f;
                startingTime = 45;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                break;

            case 11:
                numDeImagenes(20);
                numeroImagenes = 20;
                recompensasManager = new RarezaPalabras(20, 5);
                interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60;
                startingTime = 30;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                break;
        }


        #region Dificultad mal
        /*if (difficulty == 1)
        {
            numDeImagenes(2);
            numeroImagenes = 2;
            recompensasManager = new RarezaPalabras(2, 0);
        }

        else if (difficulty == 2)
        {
            numDeImagenes(3);
            numeroImagenes = 3;
            recompensasManager = new RarezaPalabras(5, 0);
        }

        else if (difficulty == 3)
        {
            numDeImagenes(4);
            numeroImagenes = 4;
            recompensasManager = new RarezaPalabras(8, 0);
        }

        else if (difficulty == 4)
        {
            numDeImagenes(6);
            numeroImagenes = 6;
            recompensasManager = new RarezaPalabras(7, 0);
        }

        else if (difficulty == 5)
        {
            numDeImagenes(10);
            numeroImagenes = 10;
            recompensasManager = new RarezaPalabras(11, 0);
            interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60f;
            startingTime = 60;
            interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
        }

        else if (difficulty == 6)
        {
            numDeImagenes(6);
            numeroImagenes = 6;
            recompensasManager = new RarezaPalabras(20, 0);
            interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60f;
            startingTime = 60;
            interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
        }

        else if (difficulty == 7)
        {
            numDeImagenes(9);
            numeroImagenes = 9;
            recompensasManager = new RarezaPalabras(17, 1);
            interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60f;
            startingTime = 60;
            interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
        }

        else if (difficulty == 8)
        {
            numDeImagenes(12);
            numeroImagenes = 12;
            recompensasManager = new RarezaPalabras(15, 1);
            interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 75f;
            startingTime = 75;
            interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
        }

        else if (difficulty == 9)
        {
            numDeImagenes(16);
            numeroImagenes = 16;
            recompensasManager = new RarezaPalabras(15, 2);
            interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 60f;
            startingTime = 60;
            interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
        }

        else if (difficulty == 10)
        {
            numDeImagenes(20);
            numeroImagenes = 20;
            recompensasManager = new RarezaPalabras(20, 4);
            interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 45f;
            startingTime = 45;
            interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
        }

        else if (difficulty == 11)
        {
            numDeImagenes(20);
            numeroImagenes = 20;
            recompensasManager = new RarezaPalabras(20, 5);
            interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 30f;
            startingTime = 30;
            interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
        }*/
        #endregion Dificultad mal
    }


    public void numDeImagenes(int numImagenes)
    {
        contador = numImagenes;
        List<int> listaNums2 = new List<int>();

        for (int i = contador; i < 107; i++)
        {
            float num = Mathf.Sqrt(i);

            if (num - Mathf.Round(num) == 0)
            {
                ColumnCount = Mathf.Sqrt(i);
                imgParent.GetComponent<GridLayoutGroup>().constraintCount = (int)ColumnCount;
                break;
            }
        }

        for (int i = 0; i < numImagenes; i++)
        {
            int n = listaNumeros[Random.Range(0, listaNumeros.Count)];

            RawImage img = Instantiate(rawImage, imgParentTransform);
            img.texture = listaImagenes[n];

            listaNumeros.Remove(n);
            listaNums2.Add(n);
        }

        for (int i = 0; i < numImagenes; i++)
        {
            int n = listaNums2[Random.Range(0, listaNums2.Count)];

            Text text = Instantiate(texto, textParentTransform);
            text.text = listaPalabras[n];

            listaNums2.Remove(n);
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
        if (interfazGeneral.GetComponent<InterfazGeneralManager>().error == true)
        {
            playSound(fallo);
        }
        else
        {
            playSound(acierto);
        }

    }

    #region Corregir
    public void Corregir()
    {
        for (int i = 0; i < imgParentTransform.childCount; i++)
        {
            for (int a = 0; a < listaImagenes.Count; a++)
            {
                if (imgParentTransform.GetChild(i).GetComponent<RawImage>().texture == listaImagenes[a])
                {
                    int x = a;
                    for (int b = 0; b < listaPalabras.Count; b++)
                    {
                        if (imgParentTransform.GetChild(i).childCount > 1)
                        {
                            if (imgParentTransform.GetChild(i).GetChild(0).GetComponent<Text>().text == listaPalabras[b] && imgParentTransform.GetChild(i).GetComponent<ScriptsImagenes>().correccion == false)
                            {
                                int y = b;

                                if (x == y)
                                {
                                    //Debug.Log("Palabra " + i + " correcta :D");
                                    imgParentTransform.GetChild(i).GetComponent<ScriptsImagenes>().correccion = true;
                                    comprobarsonidobienmal();
                                    interfazGeneral.GetComponent<InterfazGeneralManager>().progresoJugador++;
                                    palabrasAcertadas++;
                                    PalabraRecompensada(listaPalabras[b]);
                                }
                                else if (x != y)
                                {
                                    //Debug.Log("Palabra " + i + " INCORRECTA :(");
                                    interfazGeneral.GetComponent<InterfazGeneralManager>().error = true;
                                    comprobarsonidobienmal();
                                    Transform child = imgParentTransform.GetChild(i).GetChild(0);
                                    child.SetParent(textParentTransform);
                                    imgParentTransform.GetChild(i).GetComponent<ScriptsImagenes>().contadorFallos++;
                                }
                                if (imgParentTransform.GetChild(i).GetComponent<ScriptsImagenes>().correccion == true)
                                {
                                    if (imgParentTransform.GetChild(i).GetComponent<ScriptsImagenes>().contadorFallos == 0)
                                    {
                                        porcentaje += 1f / (numeroImagenes * rondas);
                                        imgParentTransform.GetChild(i).GetComponent<ScriptsImagenes>().contadorFallos = 0;
                                    }
                                    else if (imgParentTransform.GetChild(i).GetComponent<ScriptsImagenes>().contadorFallos > 0)
                                    {
                                        porcentaje += (1f / (numeroImagenes * rondas)) / 2f;
                                    }
                                }

                                if (palabrasAcertadas == imgParentTransform.childCount)
                                {
                                    if (rondaActual < rondas)
                                    {
                                        Debug.Log("AAAAAAAAAAA");
                                        rondaActual++;
                                        interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
                                        ResetMinijuego();
                                    }
                                    else if (rondaActual >= rondas)
                                    {
                                        Debug.Log("BBBBBBBBBB");
                                        progresoManager.DatosFinales(porcentaje);
                                        RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
                                        RecompensasPrefab.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
                                        RecompensasPrefab.SetActive(true);
                                        interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);
                                        interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        interfazGeneral.GetComponent<InterfazGeneralManager>().barraProgreso.GetComponent<Slider>().value = porcentaje;
    }
    #endregion Corregir

    void PalabraRecompensada(string palabraElegida)
    {
        consultas.Query("SELECT Palabra FROM Palabras WHERE Palabra = '" + palabraElegida + "' AND ID_Categoria IS NOT NULL");
        if (consultas.Count() > 0)
        {
            PalabrasRecompensa.Add(palabraElegida);
            recompensasManager.RarezaObtenida(palabraElegida);
        }
    }

    void ResetMinijuego()
    {
        foreach (Transform hijo in imgParentTransform)
        {
            Destroy(hijo.gameObject);
        }
        foreach (Transform hijoPalabro in textParentTransform)
        {
            Destroy(hijoPalabro.gameObject);
        }
        numDeImagenes(numeroImagenes);
        palabrasAcertadas = 0;
    }

    public void botonPasar()
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
        porcentaje = porcentaje + 0;
        if (rondaActual <= rondas)
        {
            rondaActual++;
            interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
            ResetMinijuego();
        }
        if (rondaActual >= rondas)
        {
            progresoManager.DatosFinales(porcentaje);
            RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
            RecompensasPrefab.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
            RecompensasPrefab.SetActive(true);
            interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);
            interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = false;
        }
    }
}