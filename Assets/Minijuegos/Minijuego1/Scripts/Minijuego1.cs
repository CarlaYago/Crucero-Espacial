using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Minijuego1 : MonoBehaviour
{
    SQLQuery consultas;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    // public List<int> Posicion = new List<int>();
    public List<script_prefab_MJ1> prefabScript;
    public List<string> Elegidas = new List<string>();
    List<TMP_InputField> inputFields = new List<TMP_InputField>();

    public int dificultad;
    public GameObject ImText;
    public Transform listOrden;
    public List<string> Palabras = new List<string>();
    public List<string> Dificultad = new List<string>();
    public List<Texture2D> Imagenes = new List<Texture2D>();
    public GameObject recompensasPrefab;
    public GameObject rondasText;
    
    int inputActual;
    int cantidad = 1;
    int numCorrectas; // se resetea para el funcionamiento del script
    int numCorrectas2; // igual que el anterior pero no se resetea para la barra de progreso
    public int rondas;
    int rondaActual = 1;
    InterfazGeneralManager interfazScript;
    InterfazPanelManager recompensasScript;
    int totalImj;
    List<string> palabrasConseguidas = new List<string>();
    public int exp;
    public float porcentaje;
    bool letraInicial, booltiempo = true;
    int time;
    int saltar;//en desuso ?
    public GameObject menuConfirmar;
   
    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip EnviarDatos, acierto, fallo,BandaSonora1, BandaSonora2, BandaSonora3;
    void CargarNivel()
    {
        progresoManager = new ProgresoMinijuegos();
        int[] difRondas = progresoManager.DificultadRondas();

        dificultad = difRondas[0];
        rondas = difRondas[1];
    }
    void CambiarDeInput()
    {
        bool inputEncontrado = false;

        for (int i = inputActual + 1; i < inputFields.Count; i++)
        {
            if (inputFields[i].interactable)
            {
                inputFields[i].Select();
                inputEncontrado = true;
                break;
            }
        }

        if (!inputEncontrado)
        {
            for (int i = 0; i < inputActual; i++)
            {
                if (inputFields[i].interactable)
                {
                    inputFields[i].Select();
                    break;
                }
            }
        }
    }
    void Start()
    {
        menuConfirmar.SetActive(false);

        CargarNivel();

        // Si la ronda es = 1 se desactiva --------
        rondasText.SetActive(true);
        if (rondas == 1)
        {
            rondasText.SetActive(false);
        }
        //-----------------------------------------
        porcentaje = 0;
        exp = 0;
        interfazScript = FindObjectOfType<InterfazGeneralManager>();
        consultas = new SQLQuery("BaseLogopeda");
        Dificultades();
        Iniciar();
        interfazScript.maxRondas = rondas;
        interfazScript.progresoRondas = rondaActual;
        totalImj = cantidad * rondas;
        interfazScript.progresoJugador = numCorrectas2;
        interfazScript.maxProgreso = totalImj;


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
            case 1: ost(BandaSonora1);
                Debug.Log("1");
                break;

            case 2: ost(BandaSonora2);
                Debug.Log("2");
                break;

            case 3: ost(BandaSonora3);
                Debug.Log("3");
                break;
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && rondaActual<=rondas)
        {
            terminar();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CambiarDeInput();
        }

        //if (Input.GetKeyDown(KeyCode.Space)) ferran cabron que es un  minijuego de escribir, la tecla espacio se usa 
        //{
        //    pasarRonda();
        //}

            if (interfazScript.GameTime <= 0)
        {
            if (booltiempo)
            {
                inputFields.Clear();
                //numCorrectas2 = cantidad * rondaActual;
                for (int i = 0; i < cantidad; i++)
                {
                    Destroy(listOrden.GetChild(i).gameObject);
                }
                rondaActual++;
                interfazScript.progresoRondas = rondaActual;
                interfazScript.error = true;
                playSound(fallo);

                interfazScript.progresoJugador = numCorrectas2;
                booltiempo = false;
                if (rondaActual <= rondas)
                {
                    Iniciar();
                }
                else
                {
                    interfazScript.progresoRondas = rondas;
                    StartCoroutine("activarRecompensas");
                }
            }
        }
    }

    void RellenarPalabras(int dificultad)
    {
        consultas.Query("SELECT Palabras.Palabra, Palabras.Imagen" + " FROM Palabras INNER JOIN Dificultad ON Palabras.ID_Palabra == Dificultad.ID_Entrada  WHERE Imagen IS NOT NULL AND Dificultad.Minijuego1 == " + dificultad);
        Palabras = consultas.StringReader(1);
        Imagenes = consultas.ImageReader(2, 150);
    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    RellenarPalabras(1);
                    cantidad = 1;
                    letraInicial = true;
                    time = 60;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(4);
                    break;
                }
            case 2:
                {
                    RellenarPalabras(1);
                    cantidad = 2;
                    letraInicial = true;
                    time = 60;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(2);
                    break;
                }
            case 3:
                {
                    RellenarPalabras(1);
                    cantidad = 1;
                    letraInicial = false;
                    time = 60;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(12);
                    break;
                }
            case 4:
                {
                    RellenarPalabras(1);
                    cantidad = 2;
                    letraInicial = false;
                    time = 60;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(6);
                    break;
                };
            case 5:
                {
                    RellenarPalabras(1);
                    cantidad = 1;
                    letraInicial = true;
                    time = 60;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(25);
                    break;
                };
            case 6:
                {
                    RellenarPalabras(1);
                    cantidad = 1;
                    letraInicial = true;
                    time = 60;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(50, 1);
                    break;
                };
            case 7:
                {
                    RellenarPalabras(2);
                    cantidad = 2;
                    letraInicial = true;
                    time = 60;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(17, 1);
                    break;
                };
            case 8:
                {
                    RellenarPalabras(2);
                    cantidad = 1;
                    letraInicial = false;
                    time = 45;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(75, 5);
                    break;
                };
            case 9:
                {
                    RellenarPalabras(2);
                    cantidad = 2;
                    letraInicial = false;
                    time = 45;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(25, 3);
                    break;
                };
            case 10:
                {
                    RellenarPalabras(2);
                    cantidad = 3;
                    letraInicial = false;
                    time = 45;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(15, 3);
                    break;
                };
            case 11:
                {
                    RellenarPalabras(2);
                    cantidad = 3;
                    letraInicial = false;
                    time = 30;
                    interfazScript.empezarTemporizador = true;
                    recompensasManager = new RarezaPalabras(15, 4);
                    break;
                };
            default:
                break;
        }
    }

    public void Iniciar()
    {
        interfazScript.GameTime = time;
        for (int i = 0; i < cantidad; i++)
        {
            int random = Random.Range(0, Palabras.Count);
            Elegidas.Add(Palabras[random]);
            GameObject prefab = Instantiate(ImText, new Vector3(0f, 0f, 0f), Quaternion.identity);
            prefab.transform.parent = listOrden;
            prefab.transform.GetChild(0).GetComponent<RawImage>().texture = Imagenes[random];
            prefabScript[i] = prefab.GetComponent<script_prefab_MJ1>();
            TMP_InputField inputField = prefab.transform.GetChild(1).GetComponent<TMP_InputField>();
            int inputIndex = i;
            //
            inputField.onSelect.AddListener(delegate { inputActual = inputIndex; });
            if (i == 0) inputField.Select();
            inputFields.Add(inputField);

            // prefabScript[i].posicion = random;
            if (letraInicial == true)
            {
                prefab.transform.GetChild(1).GetComponent<TMP_InputField>().text = Palabras[random].Substring(0, 1);
            }
            Imagenes.RemoveAt(random);
            Palabras.RemoveAt(random);
        }
        booltiempo = true;
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
   

    public void terminar()
    {
        for (int i = 0; i < cantidad; i++)
        {
            TMP_InputField inputField = listOrden.GetChild(i).GetChild(1).GetComponent<TMP_InputField>();
            if (inputField.text == Elegidas[i + (cantidad * (rondaActual - 1))])
            {
                if (inputField.enabled == true)
                {
                    numCorrectas2++;
                    PalabraRecompensada(inputField.text); 
                    comprobarsonidobienmal();
                }

                Debug.Log("correcto1");

                if (inputField.image.sprite == prefabScript[i].bien_yMal[2])
                {
                    porcentaje += (1f / (cantidad * (float)rondas));
                }
                else if (inputField.image.sprite == prefabScript[i].bien_yMal[1])
                {
                    porcentaje += (1f / ((cantidad * (float)rondas) * 2f));
                }

                inputField.image.sprite = prefabScript[i].bien_yMal[0];
                 if (inputField.enabled == true) CambiarDeInput();
                inputFields[i].enabled = false;
                numCorrectas++;
            }
            else
            {
                if ((inputField.text != "" && inputField.text != Elegidas[i + (cantidad * (rondaActual - 1))].Substring(0, 1)) || (inputField.text != "" && letraInicial == false))
                {
                    Debug.Log("Incorrecto 1");
                    inputField.image.sprite = prefabScript[i].bien_yMal[1];
                    interfazScript.error = true;
                    comprobarsonidobienmal();
                }
            }

        }

        interfazScript.barraProgreso.GetComponent<Slider>().value = porcentaje;
        interfazScript.progresoJugador = numCorrectas2;

        if (numCorrectas == cantidad)
        {
            inputFields.Clear();
            for (int i = 0; i < cantidad; i++)
            {
                Destroy(listOrden.GetChild(i).gameObject);
            }
            rondaActual++;
            interfazScript.progresoRondas = rondaActual;

            if (rondaActual <= rondas)
            {
                Iniciar();
            }
            else
            {
                interfazScript.progresoRondas = rondas;
                StartCoroutine("activarRecompensas");
            }

        }
        numCorrectas = 0;
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

    IEnumerator activarRecompensas()
    {
        progresoManager.DatosFinales(porcentaje);

        yield return new WaitForSeconds(0f);
        recompensasPrefab.SetActive(true);
        recompensasScript = FindObjectOfType<InterfazPanelManager>();
        recompensasScript.numEXP = exp;
        recompensasScript.num = porcentaje;
        recompensasScript.numPalabras = palabrasConseguidas.Count;
        interfazScript.panel.SetActive(true);
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
        porcentaje = porcentaje + 0;
        inputFields.Clear();
        for (int i = 0; i < cantidad; i++)
        {
            Destroy(listOrden.GetChild(i).gameObject);
        }
        rondaActual++;
        interfazScript.progresoRondas = rondaActual;

        if (rondaActual <= rondas)
        {
            Iniciar();
        }
        if (rondaActual > rondas)
        {
            progresoManager.DatosFinales(porcentaje);
            recompensasPrefab.SetActive(true);
            recompensasScript = FindObjectOfType<InterfazPanelManager>();
            recompensasScript.num = porcentaje;
            recompensasScript.numPalabras = palabrasConseguidas.Count;
            interfazScript.panel.SetActive(true);
            
         }
    }

    //public void terminarRecompensas()
    //{
    //    interfazScript.panel.SetActive(false);
    //    recompensasPrefab.SetActive(false);
    //    Debug.Log("juego acabado");
    //    interfazScript.empezarTemporizador = false;
    //}
}