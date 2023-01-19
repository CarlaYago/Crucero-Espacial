using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Minijuego13 : MonoBehaviour
{
    SQLQuery consultas;
    ProgresoMinijuegos progresoManager;

    [Header("Dificultad")]
    [Range(1, 11)] public int dificultad = 1;

    [Header("GameObjects")]
    public GameObject canvasMinijuego13;
    public GameObject canvasInfoMJ13;
    public GameObject letraPrefab;

    [Header("Listas")]
    public List<string> listaFrases;

    [Header("Transforms")]
    public Transform parentLetra;
    public Transform imagenFrases;

    [Header("Rondas")]
    public int rondas;
    int rondaActual = 1, contadorFrasesRonda, frasesPorRonda;
    int numFrases, progreso, tiempo;

    [Header("Interfaz")]
    InterfazGeneralManager interfazGeneral;
    public InterfazPanelManager interfazRecompensas;
    public GameObject menuConfirmar;

    string fraseSeleccionada, fraseCreada;

    [Header("Recompensas")]
    public List<string> palabrasNegrita;
    public List<string> palabrasRecompensa;
    int contadorFallos;
    public float porcentaje;
    RarezaPalabras recompensasManager;

    [Header("Audio")]
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

    void Start()
    {
        menuConfirmar.SetActive(false);
        CargarNivel();
        Dificultades(); // Completar esta función con los detalles de dificultad del logopeda

        numFrases = rondas * frasesPorRonda;
        interfazGeneral = FindObjectOfType<InterfazGeneralManager>();

        interfazGeneral.progresoRondas = 1;
        interfazGeneral.maxProgreso = numFrases;
        interfazGeneral.maxRondas = rondas;

        interfazGeneral.GameTime = tiempo;
        interfazGeneral.empezarTemporizador = true;

        canvasMinijuego13.SetActive(true);
        canvasInfoMJ13.SetActive(false);

        InstanciarFrase();
        ArreglarEspacios();

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
        AudioSource3.loop = true;
        AudioSource3.Play();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && progreso < rondas)
        {
            CorregirMJ13();
        }

        if (interfazGeneral.GameTime <= 0 && progreso < rondas)
        {
            ResetMinijuego();
            progreso++;
            interfazGeneral.error = true;
            comprobarsonidobienmal();

            if (progreso < rondas)
            {
                interfazGeneral.GameTime = tiempo;
                interfazGeneral.empezarTemporizador = true;
                interfazGeneral.progresoRondas++;
                InstanciarFrase();
            }
            else
            {
                progresoManager.DatosFinales(porcentaje);
                interfazGeneral.panel.SetActive(true);
                interfazRecompensas.num = porcentaje;
                interfazRecompensas.numPalabras = palabrasRecompensa.Count;
                interfazRecompensas.gameObject.SetActive(true);
            }
        }

        if (interfazRecompensas.isActiveAndEnabled && interfazGeneral.empezarTemporizador)
        {
            interfazGeneral.empezarTemporizador = false;
        }
    }

    void AccesoABDD(int dificultad) // (Sólo llamar esta función una vez para que no se repitan frases durante el minijuego.)
    {
        consultas = new SQLQuery("BaseLogopeda");
        consultas.Query("SELECT Frase FROM FrasesTexto INNER JOIN Dificultad ON FrasesTexto.ID_Frase = Dificultad.ID_Entrada WHERE Dificultad.Identificador = 'Frase' AND Dificultad.Minijuego13 = " + dificultad);
        listaFrases = consultas.StringReader(1);
    }

    public void CorregirMJ13()
    {
        fraseCreada = "";
        for (int i = 0; i < parentLetra.childCount; i++)
        {
            string letra = parentLetra.GetChild(i).GetComponent<Text>().text;
            fraseCreada += letra;
        }

        if (fraseCreada == fraseSeleccionada)
        {
            progreso++;
            contadorFrasesRonda++;
            comprobarsonidobienmal();

            if (contadorFrasesRonda == frasesPorRonda && rondaActual < rondas)
            {
                interfazGeneral.GameTime = tiempo;
                interfazGeneral.empezarTemporizador = true;

                rondaActual++;
                interfazGeneral.progresoRondas++;
                contadorFrasesRonda = 0;
            }

            if (contadorFallos == 0)
            {
                porcentaje += 1f / numFrases;
                contadorFallos = 0;
            }
            else if (contadorFallos > 0)
            {
                porcentaje += (1f / numFrases) / 2f;
            }

            if (progreso < numFrases)
            {
                interfazGeneral.progresoJugador++;
                Debug.Log("ta nice");

                if (palabrasNegrita.Count > 0)
                {
                    string palabraRecompensada = palabrasNegrita[Random.Range(0, palabrasNegrita.Count)];
                    PalabraRecompensada(palabraRecompensada);
                }

                for (int i = 0; i < imagenFrases.childCount; i++)
                {
                    Destroy(imagenFrases.GetChild(i).gameObject);
                }

                InstanciarFrase();
                contadorFallos = 0;
            }
            else
            {
                progresoManager.DatosFinales(porcentaje);

                interfazGeneral.progresoJugador++;
                interfazRecompensas.num = porcentaje;
                interfazRecompensas.numPalabras = palabrasRecompensa.Count;
                interfazRecompensas.gameObject.SetActive(true);
                interfazGeneral.panel.SetActive(true);
                Debug.Log("juego completado");
            }
        }
        else
        {
            if (fraseCreada == fraseSeleccionada.Replace(" ", ""))
            {
                contadorFallos = 0;
                Debug.Log("no has hecho nada, cabron >:c");
            }
            else
            {
                contadorFallos++;
                interfazGeneral.error = true;
                comprobarsonidobienmal();
                Debug.Log("ta bad");
            }

        }

        interfazGeneral.barraProgreso.GetComponent<Slider>().value = porcentaje;

    }

    void PalabraRecompensada(string palabraElegida)
    {
        palabrasRecompensa.Add(palabraElegida);
        recompensasManager.RarezaObtenida(palabraElegida);
    }

    void ArreglarEspacios()
    {
        Canvas.ForceUpdateCanvases();
        parentLetra.GetComponent<HorizontalLayoutGroup>().enabled = false;
        parentLetra.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    public void InstanciarFrase()
    {
        int fraseInstanciadaRandom = Random.Range(0, listaFrases.Count);
        fraseSeleccionada = listaFrases[fraseInstanciadaRandom];

        DetectarIdentificadores negritas = new DetectarIdentificadores("<b>", "</b>");
        palabrasNegrita = negritas.PalabrasDevueltas(fraseSeleccionada);
        fraseSeleccionada = negritas.QuitarIdentificadores(fraseSeleccionada);

        string fraseSinEspacios = fraseSeleccionada.Replace(" ", "");
        for (int i = 0; i < fraseSinEspacios.Length; i++)
        {
            GameObject letraInstanciada = Instantiate(letraPrefab, parentLetra);
            letraInstanciada.GetComponent<Text>().text = fraseSinEspacios[i].ToString();
            letraInstanciada.name = fraseSinEspacios[i].ToString();
        }

        listaFrases.RemoveAt(fraseInstanciadaRandom);
    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    tiempo = 60;
                    frasesPorRonda = 1;
                    AccesoABDD(1);
                    recompensasManager = new RarezaPalabras(5, 0);
                    break;
                }
            case 2:
                {
                    tiempo = 75;
                    frasesPorRonda = 2;
                    AccesoABDD(1);
                    recompensasManager = new RarezaPalabras(7, 0);
                    break;
                }
            case 3:
                {
                    tiempo = 60;
                    frasesPorRonda = 1;
                    AccesoABDD(2);
                    recompensasManager = new RarezaPalabras(17, 0);
                    break;
                }
            case 4:
                {
                    tiempo = 60;
                    frasesPorRonda = 2;
                    AccesoABDD(1);
                    recompensasManager = new RarezaPalabras(14, 0);
                    break;
                }
            case 5:
                {
                    tiempo = 60;
                    frasesPorRonda = 2;
                    AccesoABDD(2);
                    recompensasManager = new RarezaPalabras(17, 0);
                    break;
                }
            case 6:
                {
                    tiempo = 60;
                    frasesPorRonda = 1;
                    AccesoABDD(3);
                    recompensasManager = new RarezaPalabras(67, 2);
                    break;
                }
            case 7:
                {
                    tiempo = 75;
                    frasesPorRonda = 3;
                    AccesoABDD(2);
                    recompensasManager = new RarezaPalabras(44, 2);
                    break;
                }
            case 8:
                {
                    tiempo = 75;
                    frasesPorRonda = 2;
                    AccesoABDD(3);
                    recompensasManager = new RarezaPalabras(50, 5);
                    break;
                }
            case 9:
                {
                    tiempo = 90;
                    frasesPorRonda = 3;
                    AccesoABDD(3);
                    recompensasManager = new RarezaPalabras(67, 9);
                    break;
                }
            case 10:
                {
                    tiempo = 75;
                    frasesPorRonda = 3;
                    AccesoABDD(3);
                    recompensasManager = new RarezaPalabras(82, 18);
                    break;
                }
            case 11:
                {
                    tiempo = 60;
                    frasesPorRonda = 3;
                    AccesoABDD(3);
                    recompensasManager = new RarezaPalabras(78, 22);
                    break;
                }
        }
    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void comprobarsonidobienmal()
    {
        if (interfazGeneral.error == true)
        {
            playSound(fallo);
        }
        else
        {
            playSound(acierto);
        }
    }

    public void ResetMinijuego()
    {
        for (int i = 0; i < parentLetra.childCount; i++)
        {
            Destroy(parentLetra.GetChild(i).gameObject);
        }
        contadorFallos = 0;
        fraseCreada = "";
    }

    public void InfoMJ13()
    {
        canvasMinijuego13.SetActive(false);
        canvasInfoMJ13.SetActive(true);
    }

    public void SalirInfoMJ13()
    {
        canvasMinijuego13.SetActive(true);
        canvasInfoMJ13.SetActive(false);
    }

    public void SalirMJ13()
    {
        //SceneManager.LoadScene("LoQueVayaAntes");
        Debug.Log("Salir del Minijuego 13");
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

            progreso++;
            contadorFrasesRonda++;

            if (contadorFrasesRonda == frasesPorRonda && rondaActual < rondas)
            {
                interfazGeneral.GameTime = tiempo;
                interfazGeneral.empezarTemporizador = true;

                rondaActual++;
                interfazGeneral.progresoRondas++;
                contadorFrasesRonda = 0;
            }

            if (progreso < numFrases)
            {
                interfazGeneral.progresoJugador++;

                for (int i = 0; i < imagenFrases.childCount; i++)
                {
                    Destroy(imagenFrases.GetChild(i).gameObject);
                }

                InstanciarFrase();
                contadorFallos = 0;
            }
            else
            {
                progresoManager.DatosFinales(porcentaje);

                interfazGeneral.progresoJugador++;
                interfazRecompensas.num = porcentaje;
                interfazRecompensas.numPalabras = palabrasRecompensa.Count;
                interfazRecompensas.gameObject.SetActive(true);
                interfazGeneral.panel.SetActive(true);
                Debug.Log("juego completado");
            }
    }
}