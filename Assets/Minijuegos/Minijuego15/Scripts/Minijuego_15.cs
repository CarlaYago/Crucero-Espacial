using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minijuego_15 : MonoBehaviour
{
    SQLQuery consultas;
    ProgresoMinijuegos progresoManager;

    [Header("Dificultad")]
    [Range(1, 11)] public int dificultad = 1;
    int tiempo;

    List<string[]> ListaSecuencias = new List<string[]>();

    [Header("Referencias")]
    public GameObject EspacioBlanco, Frases;
    public Transform Scroll_FrasesOrdenadas, Scroll_FrasesDesordenadas;

    [Header("Funcionamiento")]
    int longitudLista, ContadorFrasesCorrectas;
    List<string> SecuenciaSeleccionada = new List<string>();
    List<string> SecuenciaOrdenadaComprobacion = new List<string>();

    [Header("Rondas")]
    public int numRondas;
    public int Progreso = 0;
    bool FinMinijuego = true;

    [Header("Interfaz")]

    InterfazGeneralManager interfazGeneral;
    public InterfazPanelManager interfazRecompensas;
    public GameObject scrollbarVertical;
    public GameObject scrollbarHorizontal;
    public GameObject menuConfirmar;

    [Header("Recompensas")]
    public List<string> palabrasNegrita, palabrasRecompensa;
    public int contadorFallos;
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
        numRondas = difRondas[1];
    }

    private void Start()
    {
        menuConfirmar.SetActive(false);
        CargarNivel();
        Dificultades(); // Completar esta función con los detalles de dificultad del logopeda

        interfazGeneral = FindObjectOfType<InterfazGeneralManager>();

        interfazRecompensas.gameObject.SetActive(false);
        interfazGeneral.progresoRondas = 1;
        interfazGeneral.maxProgreso = numRondas;
        interfazGeneral.maxRondas = numRondas;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && Progreso < numRondas)
        {
            ComprobacionFrases();
        }

        if (FinMinijuego == true && numRondas > 0)
        {
            SeleccionarSecuencia(); // Antes aquí se llamaba a AccesoABDD(), eso sobrescribe la lista de secuencias, haciendo que puedan repetirse
            Spawn();
            FinMinijuego = false;
        }

        // Pasar de ronda / terminar la partida al quedarse sin tiempo
        if (interfazGeneral.GameTime <= 0 && Progreso < numRondas)
        {
            ResetMinijuego();
            Progreso++;

            interfazGeneral.error = true;

            if (Progreso < numRondas)
            {
                interfazGeneral.progresoRondas++;
                FinMinijuego = true;
            }
            else
            {
                TerminarJuego();
            }
        }

        if (numRondas == 0)
        {
            Debug.Log("FIN DEL MINIJUEGO");
        }

        if (interfazRecompensas.isActiveAndEnabled && interfazGeneral.empezarTemporizador)
        {
            interfazGeneral.empezarTemporizador = false;
        }
    }

    void AccesoABDD(int dificultad) // Esta función devuelve secuencias de base de datos. Sólo debe llamarse 1 vez (para que no se repitan).
    {
        consultas = new SQLQuery("BaseLogopeda");

        // Acceder a todas las secuencias disponibles
        consultas.Query("SELECT Frase FROM Secuencias INNER JOIN Dificultad ON ID_Secuencia == Dificultad.ID_Entrada WHERE Dificultad.Minijuego14 == " + dificultad);
        ListaSecuencias = consultas.StringArrayReader(1, '|');
    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    tiempo = 75;
                    AccesoABDD(1);
                    recompensasManager = new RarezaPalabras(7, 0);
                    break;
                }
            case 2:
                {
                    tiempo = 60;
                    AccesoABDD(1);
                    recompensasManager = new RarezaPalabras(15, 0);
                    break;
                }
            case 3:
                {
                    tiempo = 75;
                    AccesoABDD(2);
                    recompensasManager = new RarezaPalabras(25, 0);
                    break;
                }
            case 4:
                {
                    tiempo = 60;
                    AccesoABDD(2);
                    recompensasManager = new RarezaPalabras(35, 0);
                    break;
                }
            case 5:
                {
                    tiempo = 75;
                    AccesoABDD(3);
                    recompensasManager = new RarezaPalabras(50, 0);
                    break;
                }
            case 6:
                {
                    tiempo = 60;
                    AccesoABDD(3);
                    recompensasManager = new RarezaPalabras(90, 2);
                    break;
                }
            case 7:
                {
                    tiempo = 75;
                    AccesoABDD(4);
                    recompensasManager = new RarezaPalabras(90, 5);
                    break;
                }
            case 8:
                {
                    tiempo = 60;
                    AccesoABDD(4);
                    recompensasManager = new RarezaPalabras(90, 10);
                    break;
                }
            case 9:
                {
                    tiempo = 75;
                    AccesoABDD(5);
                    recompensasManager = new RarezaPalabras(80, 20);
                    break;
                }
            case 10:
                {
                    tiempo = 60;
                    AccesoABDD(5);
                    recompensasManager = new RarezaPalabras(60, 40);
                    break;
                }
            case 11:
                {
                    tiempo = 45;
                    AccesoABDD(5);
                    recompensasManager = new RarezaPalabras(50, 50);
                    break;
                }
        }
    }

    void SeleccionarSecuencia()
    {
        interfazGeneral.GameTime = tiempo;
        interfazGeneral.empezarTemporizador = true;

        // Seleccionar una secuencia aleatoria
        int fraseRandom = Random.Range(0, ListaSecuencias.Count);

        // Pasar la secuencia a la lista FraseSeleccionada
        SecuenciaSeleccionada.AddRange(ListaSecuencias[fraseRandom]);
        longitudLista = SecuenciaSeleccionada.Count;

        // Eliminar la secuencia seleccionada de la lista de seleccionadas para no repetirla
        ListaSecuencias.RemoveAt(fraseRandom);

        // Detectas las palabras en negrita y quitar sus identificadores antes de spawnear las frases
        DetectarIdentificadores negritas = new DetectarIdentificadores("<b>", "</b>");
        palabrasNegrita = negritas.PalabrasDevueltasSecuencias(SecuenciaSeleccionada);
        SecuenciaSeleccionada = negritas.QuitarIdentificadoresSecuencias(SecuenciaSeleccionada);

        // Usar la secuencia *sin* identificadores como comprobación
        SecuenciaOrdenadaComprobacion.AddRange(SecuenciaSeleccionada);
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

    public void Terminar()
    {
        ComprobacionFrases();
    }

    public void Spawn()
    {
        for (int i = 0; i < longitudLista; i++)
        {
            Instantiate(EspacioBlanco, Scroll_FrasesOrdenadas);

            int fraseAleatoria = Random.Range(0, SecuenciaSeleccionada.Count);

            GameObject NewFrases = Instantiate(Frases, Scroll_FrasesDesordenadas);
            NewFrases.GetComponent<Text>().text = SecuenciaSeleccionada[fraseAleatoria];

            SecuenciaSeleccionada.RemoveAt(fraseAleatoria);
        }
    }

    public void ComprobacionFrases()
    {
        int contadorEspacios = 0;

        for (int i = 0; i < Scroll_FrasesOrdenadas.childCount; i++)
        {
            if (Scroll_FrasesOrdenadas.GetChild(i).childCount != 0)
            {
                if (Scroll_FrasesOrdenadas.GetChild(i).GetChild(0).GetComponent<Text>().text == SecuenciaOrdenadaComprobacion[i] && Scroll_FrasesOrdenadas.GetChild(i).GetChild(0).GetComponent<ScriptFrases>().correccion == false)
                {
                    ContadorFrasesCorrectas++;
                    Scroll_FrasesOrdenadas.GetChild(i).GetChild(0).GetComponent<ScriptFrases>().correccion = true;

                    if (contadorFallos == 0)
                    {
                        porcentaje += 1f / (longitudLista * numRondas);
                        contadorFallos = 0;
                    }
                    else if (contadorFallos > 0)
                    {
                        porcentaje += (1f / (longitudLista * numRondas)) / 2f;
                    }
                }
            }
            else
            {
                contadorEspacios++;
                Debug.Log("Está en blanco >:C");
            }
        }

        if (ContadorFrasesCorrectas == longitudLista)
        {
            Progreso++;
            interfazGeneral.progresoJugador++;
            comprobarsonidobienmal();

            if (Progreso < numRondas)
            {
                interfazGeneral.progresoRondas++;

                if (palabrasNegrita.Count > 0)
                {
                    string palabraRecompensada = palabrasNegrita[Random.Range(0, palabrasNegrita.Count)];
                    palabrasRecompensa.Add(palabraRecompensada);
                    recompensasManager.RarezaObtenida(palabraRecompensada);
                }

                FinMinijuego = true;
                Debug.Log("BIEEEEEN :D");
            }
            else
            {
                palabrasRecompensa.AddRange(palabrasNegrita);

                TerminarJuego();
                Debug.Log("Juego Completado");
            }
            ResetMinijuego();
        }
        else if (contadorEspacios == 0)
        {
            interfazGeneral.error = true;
            comprobarsonidobienmal();
            contadorFallos++;
            Debug.Log("MAAAAAAL D:");
        }

        interfazRecompensas.num = porcentaje;
        interfazGeneral.barraProgreso.GetComponent<Slider>().value = porcentaje;
    }

    void TerminarJuego()
    {
        progresoManager.DatosFinales(porcentaje);
        interfazGeneral.panel.SetActive(true);
        interfazRecompensas.num = porcentaje;
        interfazRecompensas.numPalabras = palabrasRecompensa.Count;
        interfazRecompensas.gameObject.SetActive(true);
    }

    public void ResetMinijuego()
    {
        for (int i = 0; i < Scroll_FrasesOrdenadas.childCount; i++)
        {
            Destroy(Scroll_FrasesOrdenadas.GetChild(i).gameObject);
        }
        for (int i = 0; i < Scroll_FrasesDesordenadas.childCount; i++)
        {
            Destroy(Scroll_FrasesDesordenadas.GetChild(i).gameObject);
        }
        ContadorFrasesCorrectas = 0;
        contadorFallos = 0;
        scrollbarVertical.SetActive(true);
        scrollbarHorizontal.SetActive(true);
        //ListaSecuencias.Clear(); ---> Esta es la lista de todas las secuencias posibles, no hay que eliminarla
        SecuenciaSeleccionada.Clear();
        SecuenciaOrdenadaComprobacion.Clear();
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
        Progreso++;

        if (Progreso < numRondas)
          {
              interfazGeneral.progresoRondas++;

                FinMinijuego = true;
             
          }
        else
          {
            TerminarJuego();
               
          }
        ResetMinijuego();

        interfazRecompensas.num = porcentaje;
        interfazGeneral.barraProgreso.GetComponent<Slider>().value = porcentaje;
    }


}
