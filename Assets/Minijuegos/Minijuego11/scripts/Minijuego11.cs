using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Minijuego11 : MonoBehaviour
{
    SQLQuery consultas;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;
    ExperienciaManager ExManager;

    [Header("Dificultad")]
    [Range(1, 11)] public int dificultad = 1;

    [Header("Referencias")]
    public GameObject recompensasPrefab;
    public TextMeshProUGUI pregunta;
    public GameObject rondasText;

    [Header("Rondas")]
    public int rondas;
    int time, frasesTotales, fraseActual, frasesPorRonda;
    int rondaActual = 1, contadorFraseRondas;

    // Referencias Interfaz General
    InterfazGeneralManager interfazScript;
    InterfazPanelManager recompensasScript;

    [Header("Stats")]
    public int exp;
    public float porcentaje;
    //
    public List<string> preguntas = new List<string>();
    public List<string> respuestas = new List<string>();
    int random;

    [Header("Recompensas")]
    // ---- temporalmente publicas para ver cuales son --------- //
    public List<string> palabrasParaConseguir = new List<string>();
    public List<string> palabrasConseguidas = new List<string>();

    [Header("Audio")]
    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip acierto, fallo, BandaSonora1, BandaSonora2, BandaSonora3;

    public GameObject menuConfirmar; 

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
        interfazScript = FindObjectOfType<InterfazGeneralManager>();

        porcentaje = 0;
        exp = 0;

        Dificultades(); // Completar esta función con los detalles de dificultad del logopeda

        interfazScript.progresoRondas = rondaActual;
        interfazScript.progresoJugador = fraseActual - 1;
        interfazScript.maxRondas = rondas;
        interfazScript.maxProgreso = frasesTotales;
        
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

        InicioMinijuego11();
        ReiniciarContador();
    }

    void Update()
    {

        // Avanzar si te quedas sin tiempo
        if (interfazScript.GameTime <= 0)
        {
            if (rondaActual <= rondas)
            {
                interfazScript.error = true;
                comprobarsonidobienmal();
                ReiniciarContador();
                //fraseActual = (frasesPorRonda * rondaActual);
                contadorFraseRondas = frasesPorRonda - 1;
                AvanzarRondas();
                SiguienteEnunciado();
            }
        }
    }

    void InicioMinijuego11()
    {
        // Subir el número de archivos
        interfazScript.progresoRondas = rondaActual;
        interfazScript.progresoJugador = fraseActual;

        // Elegir una frase aleatoria
        random = Random.Range(0, preguntas.Count);
        pregunta.text = preguntas[random];

        // Detectar posibles palabras como recompensa
        DetectarIdentificadores negritas = new DetectarIdentificadores("<b>", "</b>");
        palabrasParaConseguir = negritas.PalabrasDevueltas(pregunta.text);
        pregunta.text = negritas.QuitarIdentificadores(pregunta.text);
    }

    void ReiniciarContador()
    {
        // Reiniciar el tiempo
        interfazScript.GameTime = time;
        interfazScript.empezarTemporizador = true;
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
    public void Eleccion(string decision)
    {
        if (decision == respuestas[random])
        {
            Debug.Log("correcto");
            comprobarsonidobienmal();
            porcentaje += (1f / frasesTotales);
            exp += 5;// ----- pendiente de revisión -----//

            if (palabrasParaConseguir.Count > 0)
            {
                int recompensa = Random.Range(0, palabrasParaConseguir.Count);
                PalabraRecompensada(palabrasParaConseguir[recompensa]);
            }
        }
        else
        {
            interfazScript.error = true;
            comprobarsonidobienmal();
            exp += 1;// ----- pendiente de revisión -----//
            Debug.Log("mal");
        }
        fraseActual++; // lo sumo aquí porque no tiene sentido darle otra oportunidad si falla la pregunta
        interfazScript.barraProgreso.GetComponent<Slider>().value = porcentaje;

        // Contabilizar la ronda actual
        AvanzarRondas();
        // Avanzar al responder verdadero o falso
        SiguienteEnunciado();
    }

    void PalabraRecompensada(string palabraElegida)
    {
        palabrasConseguidas.Add(palabraElegida);
        recompensasManager.RarezaObtenida(palabraElegida);
    }

    void AvanzarRondas()
    {
        // Contar cuántas palabras se han hecho durante la ronda actual
        contadorFraseRondas++;

        // Avanzar de ronda
        if (contadorFraseRondas == frasesPorRonda)
        {
            rondaActual++;
            ReiniciarContador();
            contadorFraseRondas = 0;
        }
    }

    void SiguienteEnunciado()
    {
        if (rondaActual <= rondas)
        {
            preguntas.RemoveAt(random);
            respuestas.RemoveAt(random);
            InicioMinijuego11();
        }
        else
        {
            fraseActual = frasesTotales; // no ver que las rondas se van sumando
            interfazScript.progresoRondas = rondaActual;
            interfazScript.progresoJugador = fraseActual;
            StartCoroutine("ActivarRecompensas");
        }
    }

    void AccesoABDD(int dificultadFrase, int frasesPorRonda)
    {
        consultas = new SQLQuery("BaseLogopeda");

        // Leer todas las frases que tengan una respuesta asociada
        consultas.Query(
            // Acceder a las IDs de frases (para acceder más tarde a sus preguntas), frases y el número de preguntas por frase
            "SELECT Frase, Verdadero " +
            "FROM FrasesTexto " +

            // Relacionar la tabla de dificultades
            "INNER JOIN Dificultad " +
            "ON FrasesTexto.ID_Frase = Dificultad.ID_Entrada " +

            // Filtrar las frases que no cumplan con la dificultad especificada
            "WHERE Dificultad.Minijuego11 = " + dificultadFrase + " " +
            // Filtrar las frases que no tengan respuestas verdadero / falso
            "AND Verdadero IS NOT NULL");

        List<string> preguntasTotales = consultas.StringReader(1);
        List<bool> respuestasBool = consultas.BoolReader(2);
        List<string> respuestasTotales = new List<string>();

        for (int i = 0; i < respuestasBool.Count; i++)
        {
            if (respuestasBool[i] == false)
            {
                respuestasTotales.Add("FALSO");
            }
            else
            {
                respuestasTotales.Add("VERDADERO");
            }
        }

        for (int i = 0; i < frasesPorRonda * rondas; i++)
        {
            int elegida = Random.Range(0, preguntasTotales.Count);

            preguntas.Add(preguntasTotales[elegida]);
            respuestas.Add(respuestasTotales[elegida]);

            preguntasTotales.RemoveAt(elegida);
            respuestasTotales.RemoveAt(elegida);
        }

        this.frasesPorRonda = frasesPorRonda;
        frasesTotales = frasesPorRonda * rondas;
    }

    void Dificultades() // Esta función devuelve frases de base de datos para que no se repitan entre sí. Sólo debe llamarse 1 vez.
    {
        switch (dificultad)
        {
            case 1:
                {
                    time = 45;
                    recompensasManager = new RarezaPalabras(4);
                    AccesoABDD(1, 1);
                    break;
                }
            case 2:
                {
                    time = 30;
                    recompensasManager = new RarezaPalabras(7);
                    AccesoABDD(1, 1);
                    break;
                }
            case 3:
                {
                    time = 45;
                    recompensasManager = new RarezaPalabras(8);
                    AccesoABDD(1, 2);
                    break;
                }
            case 4:
                {
                    time = 30;
                    recompensasManager = new RarezaPalabras(12);
                    AccesoABDD(1, 2);
                    break;
                }
            case 5:
                {
                    time = 45;
                    recompensasManager = new RarezaPalabras(17);
                    AccesoABDD(1, 3);
                    break;
                }
            case 6:
                {
                    time = 45;
                    recompensasManager = new RarezaPalabras(67, 2);
                    AccesoABDD(2, 1);
                    break;
                }
            case 7:
                {
                    time = 45;
                    recompensasManager = new RarezaPalabras(40, 2);
                    AccesoABDD(2, 2);
                    break;
                }
            case 8:
                {
                    time = 30;
                    recompensasManager = new RarezaPalabras(60, 4);
                    AccesoABDD(2, 2);
                    break;
                }
            case 9:
                {
                    time = 30;
                    recompensasManager = new RarezaPalabras(67, 9);
                    AccesoABDD(2, 3);
                    break;
                }
            case 10:
                {
                    time = 30;
                    recompensasManager = new RarezaPalabras(67, 13);
                    AccesoABDD(2, 4);
                    break;
                }
            case 11:
                {
                    time = 30;
                    recompensasManager = new RarezaPalabras(80, 20);
                    AccesoABDD(2, 5);
                    break;
                }
        }
    }

    IEnumerator ActivarRecompensas()
    {
        progresoManager.DatosFinales(porcentaje);
        interfazScript.panel.SetActive(true);

        interfazScript.progresoRondas = rondas;
        yield return new WaitForSeconds(0f);
        recompensasPrefab.SetActive(true);
        recompensasScript = FindObjectOfType<InterfazPanelManager>();
        recompensasScript.numEXP = exp;
        recompensasScript.num = porcentaje;
        recompensasScript.numPalabras = palabrasConseguidas.Count;
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
        rondaActual++;
        ReiniciarContador();
        contadorFraseRondas = 0;
        SiguienteEnunciado();
           
    }
}
