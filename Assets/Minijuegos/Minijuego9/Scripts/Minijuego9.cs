using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Minijuego9 : MonoBehaviour
{
    SQLQuery consultas;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    public List<string> preguntas = new List<string>();
    public List<string> respuestas = new List<string>(); // (SI / NO)
    Dictionary<int, string> frases = new Dictionary<int, string>();
    InterfazGeneralManager interfazScript;

    [Header("Referencias")]
    public TextMeshProUGUI preguntaUI;
    public TextMeshProUGUI fraseUI;
    public Button botonSi, botonNo;
    InterfazPanelManager recompensasScript;
    public GameObject recompensasPrefab;
    public GameObject rondasText;

    [Header("Variables")]
    [Range(1, 11)] public int dificultad;
    int preguntasxRonda;
    public int rondas;
    int rondaActual = 1, rondaActual2 = 1;
    int rondaActReset = 1;
    int fraseAleatoria, preguntaAleatoria;
    KeyValuePair<int, string> fraseElegida;

    // ---- temporalmente publicas para ver cuales son --------- //
    public List<string> palabrasParaConseguir = new List<string>();
    public List<string> palabrasConseguidas = new List<string>();
    public int exp;
    public float porcentaje;
    bool booltiempo = true;
    int time, fallos;
    public GameObject menuConfirmar;

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

        // si la ronda es = 1 se desactiva---------
        rondasText.SetActive(true);
        if (rondas == 1)
        {
            rondasText.SetActive(false);
        }
        //-----------------------------------------
        porcentaje = 0;
        exp = 0;
        time = 100;
        interfazScript = FindObjectOfType<InterfazGeneralManager>();
        interfazScript.progresoRondas = rondaActual2;
        interfazScript.progresoJugador = rondaActual - 1;
        Dificultades();

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

        if (interfazScript.GameTime <= 0)
        {
            if (booltiempo)
            {
                interfazScript.error = true;
                comprobarsonidobienmal();
                booltiempo = false;
                // rondaActual = (rondaActual2 * preguntasxRonda) + 1;
                rondaActual2++;
                interfazScript.progresoRondas = rondaActual2;
                //interfazScript.progresoJugador = rondaActual;

                if (rondaActual2 > rondas)
                {
                    interfazScript.progresoRondas = rondaActual2;
                    interfazScript.progresoJugador = rondas * preguntasxRonda;
                    StartCoroutine("ActivarRecompensas");
                }
                else
                {
                    rondaActReset = 1;
                    ElegirFrase(); // <--- Aquí antes ponia Dificultades(), no sé qué se supone que es esto pero Dificultades() va a volver a buscar en BDD, lo que hará que puedan repetirse textos
                }
            }
        }
    }

    void AccesoABDD(bool isFrase, int dificultadFrase, int numPreguntas)
    {
        preguntasxRonda = numPreguntas;

        consultas = new SQLQuery("BaseLogopeda");

        // Buscar frases o textos dependiendo del valor bool de entrada, isFrase
        string identificador;
        if (isFrase) identificador = "Frase";
        else identificador = "Texto";


        // Leer todas las frases posibles que tengan el mínimo de preguntas requerido
        consultas.Query(
            // Acceder a las IDs de frases (para acceder más tarde a sus preguntas), frases y el número de preguntas por frase
            "SELECT FrasesTexto.ID_Frase, Frase, count(*) AS NumPreguntas " +
            "FROM FrasesTexto " +

            // Relacionar la tabla de preguntas
            "INNER JOIN Preguntas " +
            "ON FrasesTexto.ID_Frase = Preguntas.ID_Frase " +

            // Relacionar la tabla de dificultades
            "INNER JOIN Dificultad " +
            "ON FrasesTexto.ID_Frase = Dificultad.ID_Entrada " +

            // Filtrar las frases que no cumplan con la dificultad especificada
            "WHERE Dificultad.Identificador = '" + identificador + "' AND Dificultad.Minijuego9 = " + dificultadFrase + " " +
            // Filtrar las frases que no tengan respuesta si / no
            "AND (Preguntas.Respuesta = 'SI' OR Preguntas.Respuesta = 'NO') " +

            // Agrupar por la ID de la frase para contabilizar el número de preguntas por frase, e incluir sólo las que tengan suficientes
            "GROUP BY FrasesTexto.ID_Frase " +
            "HAVING NumPreguntas >= " + numPreguntas);

        frases = consultas.StringReaderID(1, 2);

        ElegirFrase();
    }

    void ElegirFrase()
    {
        // ---- para que funcione bien hay que eliminar las frases que ya han salido -----// 
        booltiempo = true;
        //rondas = numPreguntas;
        interfazScript.maxRondas = rondas;
        interfazScript.maxProgreso = preguntasxRonda * rondas;
        // *Esto estaba antes en AccesoABDD(), esa función siempre va a sobreescribir la lista de frases disponibles con las que existen, por lo que sólo se le llama en el Start()

        interfazScript.GameTime = time;

        // Elegir frase aleatoria
        fraseAleatoria = Random.Range(0, frases.Count);
        fraseElegida = frases.ElementAt(fraseAleatoria);
        fraseUI.text = fraseElegida.Value;

        // Acceder a las preguntas y respuestas de la frase elegida
        consultas.Query("SELECT Pregunta, Respuesta" + " FROM Preguntas" + " WHERE ID_Frase = " + fraseElegida.Key);
        preguntas = consultas.StringReader(1);
        respuestas = consultas.StringReader(2);

        // Escoger la primera pregunta aleatoriamente
        preguntaAleatoria = Random.Range(0, preguntas.Count);
        preguntaUI.text = preguntas[preguntaAleatoria];

        DetectarIdentificadores negritas = new DetectarIdentificadores("<b>", "</b>");
        palabrasParaConseguir = negritas.PalabrasDevueltas(fraseUI.text);
        fraseUI.text = negritas.QuitarIdentificadores(fraseUI.text);
    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
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

    public void TaskOnClick(string sn)
    {
        if (respuestas[preguntaAleatoria] == sn)
        {
            porcentaje += (1f / (preguntasxRonda * rondas));
            exp += 5;// ----- pendiente de revisión -----//
            Debug.Log("correcto");
            comprobarsonidobienmal();
            if (palabrasParaConseguir.Count > 0)
            {
                int recompensa = Random.Range(0, palabrasParaConseguir.Count);
                PalabraRecompensada(palabrasParaConseguir[recompensa]);
            }

            if (rondaActReset >= preguntasxRonda)
            {
                rondaActual2++;
            }
        }
        else
        {
            exp += 1;// ----- pendiente de revisión -----//
            interfazScript.error = true;
            comprobarsonidobienmal();
            interfazScript.progresoRondas = rondaActual2;
            interfazScript.progresoJugador = rondaActual - 1;
            Debug.Log("Incorrecto");
            fallos++;
            if (rondaActReset >= preguntasxRonda)//añadido para solucionar un bug
            {
                rondaActual2++;
            }
        }
        interfazScript.barraProgreso.GetComponent<Slider>().value = porcentaje;
        rondaActReset++;
        rondaActual++;

        if (rondaActReset <= preguntasxRonda)
        {
            interfazScript.progresoRondas = rondaActual2;
            interfazScript.progresoJugador = rondaActual - 1;
            Debug.Log("reroll");
            Reroll();
        }
        else if (rondaActReset > preguntasxRonda && rondaActual2 > rondas)
        {
            interfazScript.progresoRondas = rondaActual2;
            interfazScript.progresoJugador = rondas * preguntasxRonda;
            StartCoroutine("ActivarRecompensas");
        }
        else
        {
            Debug.Log("dificultades");
            rondaActReset = 1;

            interfazScript.progresoRondas = rondaActual2;
            interfazScript.progresoJugador = rondaActual - 1;
            fallos = 0;

            frases.Remove(fraseElegida.Key); // Eliminamos la frase elegida por ID
            ElegirFrase(); // Llamamos a ElegirFrase() (No a Dificultades() por que si no no tenemos en cuenta la frase eliminada)
        }
    }

    void PalabraRecompensada(string palabraElegida)
    {
        palabrasConseguidas.Add(palabraElegida);
        recompensasManager.RarezaObtenida(palabraElegida);
    }

    void Reroll()
    {
        preguntas.RemoveAt(preguntaAleatoria);
        respuestas.RemoveAt(preguntaAleatoria);

        preguntaAleatoria = Random.Range(0, preguntas.Count);
        preguntaUI.text = preguntas[preguntaAleatoria];
    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    time = 60;
                    recompensasManager = new RarezaPalabras(7);
                    AccesoABDD(false, 1, 1);
                    break;
                }
            case 2:
                {
                    time = 60;
                    AccesoABDD(false, 1, 2);
                    recompensasManager = new RarezaPalabras(10);
                    break;
                }
            case 3:
                {
                    time = 60;
                    AccesoABDD(false, 2, 1);
                    recompensasManager = new RarezaPalabras(25);
                    break;
                }
            case 4:
                {
                    time = 60;
                    AccesoABDD(false, 2, 2);
                    recompensasManager = new RarezaPalabras(23);
                    break;
                };
            case 5:
                {
                    time = 60;
                    AccesoABDD(false, 2, 3);
                    recompensasManager = new RarezaPalabras(22);
                    break;
                };
            case 6:
                {
                    time = 60;
                    AccesoABDD(false, 3, 1);
                    recompensasManager = new RarezaPalabras(90, 2);
                    break;
                };
            case 7:
                {
                    time = 60;
                    AccesoABDD(false, 3, 2);
                    recompensasManager = new RarezaPalabras(50, 3);
                    break;
                };
            case 8:
                {
                    time = 45;
                    AccesoABDD(false, 2, 3);
                    recompensasManager = new RarezaPalabras(50, 3);
                    break;
                };
            case 9:
                {
                    time = 60;
                    AccesoABDD(false, 3, 3);
                    recompensasManager = new RarezaPalabras(87, 13);
                    break;
                };
            case 10:
                {
                    time = 45;
                    AccesoABDD(false, 3, 3);
                    recompensasManager = new RarezaPalabras(73, 27);
                    break;
                };
            case 11:
                {
                    time = 30;
                    AccesoABDD(false, 3, 3);
                    recompensasManager = new RarezaPalabras(67, 33);
                    break;
                };
            default:
                break;
        }
    }

    IEnumerator ActivarRecompensas()
    {
        progresoManager.DatosFinales(porcentaje);

        interfazScript.panel.SetActive(true);// soluciona un bug, para no poder clicar más de una vez el boton cuando termina el juego
        interfazScript.progresoRondas = rondas;
        yield return new WaitForSeconds(0f);
        recompensasPrefab.SetActive(true);
        recompensasScript = FindObjectOfType<InterfazPanelManager>();
        recompensasScript.numEXP = exp;
        recompensasScript.num = porcentaje;
        recompensasScript.numPalabras = palabrasConseguidas.Count;
        interfazScript.empezarTemporizador = false;
    }

    //public void TerminarRecompensas()
    //{

    //    interfazScript.panel.SetActive(false);
    //    recompensasPrefab.SetActive(false);
    //    Debug.Log("juego acabado");
    //    interfazScript.empezarTemporizador = false;
    //}

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

        if (rondaActReset >= preguntasxRonda)
        {
            rondaActual2++;
        }
        rondaActReset++;
        rondaActual++;

        if (rondaActReset <= preguntasxRonda)
        {
            interfazScript.progresoRondas = rondaActual2;
            interfazScript.progresoJugador = rondaActual - 1;
            Reroll();
        }
        else if (rondaActReset > preguntasxRonda && rondaActual2 > rondas)
        {
            interfazScript.progresoRondas = rondaActual2;
            interfazScript.progresoJugador = rondas * preguntasxRonda;
            StartCoroutine("ActivarRecompensas");
        }
        else
        {
            rondaActReset = 1;

            interfazScript.progresoRondas = rondaActual2;
            interfazScript.progresoJugador = rondaActual - 1;
            fallos = 0;

            frases.Remove(fraseElegida.Key);
            ElegirFrase();
        }
    }
}
