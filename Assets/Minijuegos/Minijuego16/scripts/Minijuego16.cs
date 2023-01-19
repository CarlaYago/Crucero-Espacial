using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Minijuego16 : MonoBehaviour
{
    SQLQuery consultas;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    [Range(1, 11)] public int dificultad = 1;

    string[] posiblesSoluciones;

    public TextMeshProUGUI palabraUI, tituloUI;

    string palabraElegida;
    public List<int> palabrasYaRespondidas;

    public GameObject RespuestaObj;
    public int rondas;
    public GameObject rondasText;
    public List<Sprite> bien_mal;
    public TMP_InputField InputText;
    InterfazGeneralManager interfazScript;
    InterfazPanelManager recompensasScript;
    public GameObject recompensasPrefab;

    public int exp;
    public float porcentaje;
    bool SinOAnt, inicio;
    int time, fallados = 0, cierto, contadorFraseRondas, frasesPorRonda, fraseActual, frasesTotales, rondaActual = 1, random;
    public List<string> palabrasConseguidas = new List<string>();
    string inputUpperCase;
    public GameObject menuConfirmar;

    // Nuevas variables de base de datos para que no puedan repetirse palabras:
    public List<string> palabrasDisponibles;
    public List<string[]> sinonimosDisponibles;
    // (Usad estas listas para elegir de ellas las palabras que saldrán y poder eliminarlas para que no vuelvan a salir)
    //BDD
    List<int> idPalabras = new List<int>();

    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip acierto, fallo, BandaSonora1, BandaSonora2, BandaSonora3;

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
        time = 60;
        interfazScript = FindObjectOfType<InterfazGeneralManager>();
        //llamar dificultad

        Dificultades();

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

        Reiniciar();
        ActualizarInterfaz(SinOAnt);
        ReiniciarContador();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && rondaActual<=rondas)
        {
            Comprobar();
        }

        if (inputUpperCase != InputText.text)
        {
            InputText.text = inputUpperCase;
        }
        if (interfazScript.GameTime <= 0)
        {
            if (rondaActual <= rondas)
            {
                interfazScript.error = true;
                comprobarsonidobienmal();

                ReiniciarContador();
                //fraseActual = (frasesPorRonda * rondaActual);
                contadorFraseRondas = frasesPorRonda -1;
                AvanzarRondas();
                SiguienteEnunciado();
            }
        }
    }
    public void UpperCase()
    {
        inputUpperCase = InputText.text.ToUpper();
    }
    void ReiniciarContador()
    {
        // Reiniciar el tiempo
        interfazScript.GameTime = time;
        interfazScript.empezarTemporizador = true;
    }

    void ActualizarInterfaz(bool usandoSinonimos)
    {
        InputText.image.sprite = bien_mal[2];

        // Mostrar la palabra elegida por interfaz
        palabraUI.text = palabraElegida;

        // Cambiar el encabezado de la interfaz adecuadamente
        if (usandoSinonimos) tituloUI.text = "Escribe su sinónimo";
        else tituloUI.text = "Escribe su antónimo";

        // Rellenar los botones con sinónimos érroneos aleatorios --> Hay que añadir una función que instancie los botones por script

        // Elegir un botón para sustituirlo por la palabra correcta
    }

    void Dificultades() // Completar el switch con las especificaciones de de dificultad del logopeda <---------------- (facil = 1, medio = 2, dificil = 3)
    {
        switch (dificultad)
        {
            case 1:
                {
                    frasesPorRonda = 1;
                    AccesoABDDSinonimos(false); // Como aquí se piden antónimos ponemos la bool de sinónimo a false

                    recompensasManager = new RarezaPalabras(5);
                    break;
                }
            case 2:
                {
                    frasesPorRonda = 2;
                    AccesoABDDSinonimos(false);

                    recompensasManager = new RarezaPalabras(7);
                    break;
                }
            case 3:
                {
                    frasesPorRonda = 3;
                    AccesoABDDSinonimos(false);

                    recompensasManager = new RarezaPalabras(11);
                    break;
                }
            case 4:
                {
                    frasesPorRonda = 4;
                    AccesoABDDSinonimos(false);

                    recompensasManager = new RarezaPalabras(12);
                    break;
                }
            case 5:
                {
                    frasesPorRonda = 5;
                    AccesoABDDSinonimos(false);

                    recompensasManager = new RarezaPalabras(13);
                    break;
                }
            case 6:
                {
                    frasesPorRonda = 1;
                    AccesoABDDSinonimos(true);

                    recompensasManager = new RarezaPalabras(50, 1);
                    break;
                }
            case 7:
                {
                    frasesPorRonda = 2;
                    AccesoABDDSinonimos(true);

                    recompensasManager = new RarezaPalabras(40, 2);
                    break;
                }
            case 8:
                {
                    frasesPorRonda = 3;
                    AccesoABDDSinonimos(true);

                    recompensasManager = new RarezaPalabras(67, 4);
                    break;
                }
            case 9:
                {
                    frasesPorRonda = 4;
                    AccesoABDDSinonimos(true);

                    recompensasManager = new RarezaPalabras(75, 10);
                    break;
                }
            case 10:
                {
                    frasesPorRonda = 5;
                    AccesoABDDSinonimos(true);

                    recompensasManager = new RarezaPalabras(80, 16);
                    break;
                }
            case 11:
                {
                    frasesPorRonda = 6;
                    AccesoABDDSinonimos(true);

                    recompensasManager = new RarezaPalabras(67, 17);
                    break;
                }
        }
    }

    void AccesoABDDSinonimos(bool sinonimo)
    {
        int buscarSinonimo = 0;
        if (sinonimo) buscarSinonimo = 1;

        consultas = new SQLQuery("BaseLogopeda");

        // Seleccionar todas las IDs de palabras con sinónimos (/ antónimos)
        consultas.Query("SELECT Palabra, Palabras FROM SinonimosAntonimos INNER JOIN Palabras ON SinonimosAntonimos.ID_Palabra = Palabras.ID_Palabra WHERE Sinonimo = " + buscarSinonimo);
        palabrasDisponibles = consultas.StringReader(1);
        sinonimosDisponibles = consultas.StringArrayReader(2);

        frasesTotales = frasesPorRonda * rondas;
        SinOAnt = sinonimo;

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
        for (int i = 0; i < posiblesSoluciones.Length; i++)
        {

            if (InputText.text == posiblesSoluciones[i])
            {
                cierto++;
                comprobarsonidobienmal();
            }
        }
        if (cierto >= 1)
        {
            cierto = 0;
            PalabraRecompensada(palabraElegida);

            Debug.Log("ole");
            InputText.image.sprite = bien_mal[0];
            if (fallados == 0)
            {
                porcentaje += (1f / frasesTotales);
                exp += 5;// ----- pendiente de revisión -----//
            }
            else if (fallados > 0)
            {
                porcentaje += (1f / (frasesTotales * 2f));
                exp += 1;// ----- pendiente de revisión -----//
            }
            // Contabilizar la ronda actual
            AvanzarRondas();
            // Avanzar al responder verdadero o falso
            fraseActual++;

            SiguienteEnunciado();

        }
        else
        {
            if (InputText.text != "")
            {
                cierto = 0;

                Debug.Log("mal");
                InputText.image.sprite = bien_mal[1];
                interfazScript.error = true;
                comprobarsonidobienmal();
                fallados++;
            }
        }

        interfazScript.barraProgreso.GetComponent<Slider>().value = porcentaje;
        InputText.ActivateInputField();
    }

    void PalabraRecompensada(string palabraElegida)
    {
        consultas.Query("SELECT Palabra FROM Palabras WHERE Palabra = '" + palabraElegida + "' AND ID_Categoria IS NOT NULL");
        if (consultas.Count() > 0)
        {
            palabrasConseguidas.Add(palabraElegida);
            recompensasManager.RarezaObtenida(palabraElegida);
        }
    }

    void Reiniciar()
    {
        if (inicio)
        {
            palabrasDisponibles.RemoveAt(random);
            sinonimosDisponibles.RemoveAt(random);
        }
        interfazScript.progresoRondas = rondaActual;
        interfazScript.progresoJugador = fraseActual;

        InputText.image.sprite = bien_mal[2];
        InputText.text = "";

        random = Random.Range(0, palabrasDisponibles.Count);
        palabraElegida = palabrasDisponibles[random];
        posiblesSoluciones = sinonimosDisponibles[random];
        inicio = true;
    }

    void AvanzarRondas()
    {
        // Contar cuántas palabras se han hecho durante la ronda actual
        contadorFraseRondas++;

        fallados = 0;
        // Avanzar de ronda
        if (contadorFraseRondas == frasesPorRonda)
        {
            rondaActual++;
            ReiniciarContador();
            contadorFraseRondas = 0;
        }
    }

    IEnumerator ActivarRecompensas()
    {
        progresoManager.DatosFinales(porcentaje);

        interfazScript.progresoRondas = rondas;
        yield return new WaitForSeconds(0f);
        recompensasPrefab.SetActive(true);
        recompensasScript = FindObjectOfType<InterfazPanelManager>();
        recompensasScript.numEXP = exp;
        recompensasScript.num = porcentaje;
        recompensasScript.numPalabras = palabrasConseguidas.Count;
        interfazScript.panel.SetActive(true);
    }

    //public void TerminarRecompensas()
    //{
    //    interfazScript.panel.SetActive(false);
    //    recompensasPrefab.SetActive(false);
    //    Debug.Log("juego acabado");
    //    interfazScript.empezarTemporizador = false;
    //}

    void SiguienteEnunciado()
    {
        if (rondaActual <= rondas)
        {
            Reiniciar();
            ActualizarInterfaz(SinOAnt);
        }
        else
        {
            interfazScript.empezarTemporizador = false;
            fraseActual = frasesTotales; // no ver que las rondas se van sumando
            interfazScript.progresoRondas = rondaActual;
            interfazScript.progresoJugador = fraseActual;
            StartCoroutine("ActivarRecompensas");
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

        AvanzarRondas();
        SiguienteEnunciado();
    }
}