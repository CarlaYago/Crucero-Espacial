using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Minijuego8 : MonoBehaviour
{
    //Constructor
    SQLQuery consultas;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    //Recompensas
    List<string> recompensas = new List<string>();

    int Tiempo = 60;

    [Range(1, 11)] public int dificultad;

    public List<string> listaPalabrasMJ8;
    public List<string> listaDefinicionesMJ8;

    public Text definicionText;
    Text instantiatedText;

    public Transform espacioParaDefinicion;

    public TMP_InputField inputField;
    string inputUpperCase;

    int definicionAleatoria;

    public int numRondas, NumArchivos;
    int NumPalabras = 1, palabrasPorRonda, rondaActual = 1;
    public float NumAciertos, NumFallos;
    bool Acierto1;
    public bool FinMinijuego = true;
    public InterfazGeneralManager InterfazManager;
    public InterfazPanelManager PanelManager;

    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip acierto, fallo, BandaSonora1, BandaSonora2, BandaSonora3;
    void CargarNivel()
    {
        progresoManager = new ProgresoMinijuegos();
        int[] difRondas = progresoManager.DificultadRondas();

        dificultad = difRondas[0];
        numRondas = difRondas[1];
    }

    void Start()
    {
        CargarNivel();
        Dificultades();
        Acierto1 = true;
        InterfazManager.GameTime = Tiempo;
        InterfazManager.empezarTemporizador = true;
        InterfazManager.progresoJugador = NumArchivos;
        InterfazManager.maxProgreso = palabrasPorRonda * numRondas;
        InterfazManager.maxRondas = numRondas;
        InterfazManager.progresoRondas = rondaActual;
        //InterfazManager.panel.SetActive(true);

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

        if (InterfazManager.GameTime <= 0)
        {
            if (rondaActual < numRondas)
            {
                rondaActual++;
                NumPalabras = 1;
                FinMinijuego = true;
                InterfazManager.progresoRondas = rondaActual;
                InterfazManager.GameTime = Tiempo;
            }
            else
            {
                progresoManager.DatosFinales(InterfazManager.porcentajeBarra);
                InterfazManager.panel.SetActive(true);

                PanelManager.numPalabras = recompensas.Count;
                PanelManager.num = InterfazManager.porcentajeBarra;
                PanelManager.numEXP = 5; //exp provisional
                PanelManager.gameObject.SetActive(true);
                InterfazManager.empezarTemporizador = false;
            }
        }

        if (FinMinijuego == true && rondaActual <= numRondas)
        {
            Debug.Log("Encima del If");
            if (listaPalabrasMJ8.Count > 0)
            {
                GenerarDefiniciones();
                Debug.Log("Ronda " + rondaActual + " generada");
            }
            else
            {
                Debug.LogError("No hay más palabras disponibles para seguir con el minijuego.");
            }

            FinMinijuego = false;
        }

        if (rondaActual > numRondas)
        {
            Debug.Log("FIN MINIJUEGO");
        }

        if (inputUpperCase != inputField.text)
        {
            inputField.text = inputUpperCase;
        }

        if (Input.GetKeyDown(KeyCode.Return) && !PanelManager.gameObject.activeSelf)
        {
            Corregir();
        }

    }

    void AccesoABDD(int dificultad)
    {
        consultas = new SQLQuery("BaseLogopeda");

        consultas.Query("SELECT Palabra, Definicion FROM Palabras INNER JOIN Dificultad ON Palabras.ID_Palabra == Dificultad.ID_Entrada WHERE Definicion IS NOT NULL AND Dificultad.Minijuego8 == " + dificultad);
        listaPalabrasMJ8 = consultas.StringReader(1);
        listaDefinicionesMJ8 = consultas.StringReader(2);
    }

    List<string> PalabrasErroneas(string definicion) // Usar para otorgar palabras incorrectas dentro de la pista
    {
        consultas.Query("SELECT Palabra FROM Palabras WHERE Definicion IS NOT NULL AND Definicion IS NOT '" + definicion + "'");
        List<string> palabrasIncorrectas = consultas.StringReader(1);
        return palabrasIncorrectas;
    }

    void GenerarDefiniciones()
    {
        definicionAleatoria = Random.Range(0, listaDefinicionesMJ8.Count);

        foreach (Transform Child in espacioParaDefinicion)
        {
            Destroy(Child.gameObject);
        }

        instantiatedText = Instantiate(definicionText, espacioParaDefinicion);
        instantiatedText.text = listaDefinicionesMJ8[definicionAleatoria];

    }

    public void Corregir() // Asignado al botón de terminar
    {
        Debug.Log("entro a corregir");
        PalabraCorrecta();

        if (NumPalabras > palabrasPorRonda)
        {
            if (rondaActual < numRondas)
            {
                rondaActual++;
                InterfazManager.GameTime = Tiempo;
                InterfazManager.progresoRondas = rondaActual;
                NumPalabras = 1;
            }
            else
            {
                progresoManager.DatosFinales(InterfazManager.porcentajeBarra);
                InterfazManager.panel.SetActive(true);

                PanelManager.numPalabras = recompensas.Count;
                PanelManager.num = InterfazManager.porcentajeBarra;
                PanelManager.numEXP = 5; //exp provisional
                PanelManager.gameObject.SetActive(true);
                InterfazManager.empezarTemporizador = false;
            }
        }

        inputField.ActivateInputField();
    }

    void PalabraCorrecta()
    {
        if (inputField.text == listaPalabrasMJ8[definicionAleatoria])
        {
            Debug.Log("correcto");
            comprobarsonidobienmal();
            PalabraRecompensada(inputField.text);

            inputField.text = null;
            Destroy(instantiatedText.gameObject);

            listaDefinicionesMJ8.RemoveAt(definicionAleatoria);
            listaPalabrasMJ8.RemoveAt(definicionAleatoria);
            FinMinijuego = true;

            NumPalabras++;

            if (Acierto1 == false)
            {
                NumAciertos += 0.5f;
            }
            else
            {
                NumAciertos++;
            }
            InterfazManager.porcentajeBarra = NumAciertos / InterfazManager.maxProgreso;
            InterfazManager.barraProgreso.GetComponent<Slider>().value = InterfazManager.porcentajeBarra;

            NumArchivos++;

            Acierto1 = true;
            InterfazManager.progresoJugador = NumArchivos;
        }
        else if(inputField.text != "")
        {
            InterfazManager.error = true;
            comprobarsonidobienmal();
            Debug.Log("Incorrecto");
            inputField.text = null;
            Acierto1 = false;
        }
    }

    void PalabraRecompensada(string palabraElegida)
    {
        consultas.Query("SELECT Palabra FROM Palabras WHERE Palabra = '" + palabraElegida + "' AND ID_Categoria IS NOT NULL");
        if (consultas.Count() > 0)
        {
            recompensas.Add(palabraElegida);
            recompensasManager.RarezaObtenida(palabraElegida);
        }
    }

    public void UpperCase()
    {
        inputUpperCase = inputField.text.ToUpper();
    }

    void Dificultades() // Completar el switch con las especificaciones de de dificultad del logopeda <---------------- (facil = 1, medio = 2, dificil = 3)
    {
        switch (dificultad)
        {
            case 1:
                {
                    AccesoABDD(1); // En este caso, como se piden descripciones fáciles, ponemos un 1
                    palabrasPorRonda = 1;
                    recompensasManager = new RarezaPalabras(7);
                    Tiempo = 60;
                    break;
                }
            case 2:
                {
                    AccesoABDD(2);
                    palabrasPorRonda = 1;
                    recompensasManager = new RarezaPalabras(15);
                    Tiempo = 60;
                    break;
                }
            case 3:
                {
                    AccesoABDD(1);
                    palabrasPorRonda = 2;
                    recompensasManager = new RarezaPalabras(17);
                    Tiempo = 60;
                    break;
                }
            case 4:
                {
                    AccesoABDD(1);
                    palabrasPorRonda = 3;
                    recompensasManager = new RarezaPalabras(23);
                    Tiempo = 60;
                    break;
                }
            case 5:
                {
                    AccesoABDD(2);
                    palabrasPorRonda = 2;
                    recompensasManager = new RarezaPalabras(33);
                    Tiempo = 60;
                    break;
                }
            case 6:
                {
                    AccesoABDD(2);
                    palabrasPorRonda = 2;
                    recompensasManager = new RarezaPalabras(67, 2);
                    Tiempo = 45;
                    break;
                }
            case 7:
                {
                    AccesoABDD(2);
                    palabrasPorRonda = 3;
                    recompensasManager = new RarezaPalabras(67, 3);
                    Tiempo = 60;
                    break;
                }
            case 8:
                {
                    AccesoABDD(2);
                    palabrasPorRonda = 3;
                    recompensasManager = new RarezaPalabras(93, 7);
                    Tiempo = 45;
                    break;
                }
            case 9:
                {
                    AccesoABDD(3);
                    palabrasPorRonda = 2;
                    recompensasManager = new RarezaPalabras(87, 13);
                    Tiempo = 60;
                    break;
                }
            case 10:
                {
                    AccesoABDD(3);
                    palabrasPorRonda = 3;
                    recompensasManager = new RarezaPalabras(73, 27);
                    Tiempo = 60;
                    break;
                }
            case 11:
                {
                    AccesoABDD(3);
                    palabrasPorRonda = 3;
                    recompensasManager = new RarezaPalabras(67, 33);
                    Tiempo = 45;
                    break;
                }
        }
    }
}
