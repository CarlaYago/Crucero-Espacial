using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Minijuego3 : MonoBehaviour
{
    SQLQuery consultas, comprobacionDiccionario;
    List<string> palabrasPosibles;
    public GameObject botonPista;

    List<string> Palabras;
    List<int> longitudPalabrasRespondidas = new List<int>();
    string PalabraSeleccionada;
    int LongitudPalabra;

    public List<string> PalabraOrdenada;
    public List<string> PalabraOrdenadaPista;
    public List<string> PalabrasRecompensa;
    public List<Transform> EspaciosLista;
    public Transform espacioPista;

    [Header("Referencias")]
    public GameObject espacioPrefab;
    public GameObject letraPrefab;
    public Transform espaciosParent, letrasParent;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    [Header("Parámetros")]
    [Range(1, 11)] public int dificultad;
    public int NumPistas;
    public string CategoriaPista;
    public int rondas;
    bool FinMinijuego = true;
    public int palabrasAcertadas = 0;
    public int Progreso;
    public int contadorFallos;
    public float porcentaje;
    int tiempo;
    bool empezarMinijuegoSinTiempo;

    [Header("Interfaz")]
    public GameObject interfazGeneral;
    public GameObject RecompensasPrefab;
    public Button enviarDatos;
    public GameObject texCatPistaxDDDDD;
    public GameObject menuConfirmar;

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

        RecompensasPrefab.SetActive(false);
        interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas = 1;
        Progreso = 1;

        consultas = new SQLQuery("BaseLogopeda");
        comprobacionDiccionario = new SQLQuery("LexicoEsp");

        Dificultades();
        InicioJuego();

        if (dificultad <= 7)
        {
            empezarMinijuegoSinTiempo = true;
        }

        //espaciosParent = GameObject.FindGameObjectWithTag("Espacios").transform; -> Si solo usáis los tags Espacios / Letras para esto se pueden quitar (ya son variables públicas)
        //letrasParent = GameObject.FindGameObjectWithTag("Letras").transform;

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.AudioSource1;
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
        //Debug.Log("porcentaje " + porcentaje);
        //Debug.Log("porcentajeTexto " + RecompensasPrefab.GetComponent<InterfazPanelManager>().num);

        if (Input.GetKeyDown(KeyCode.Return) && enviarDatos.enabled == true)
        {
            ComprobacionLetras();
        }

        if (interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime <= 0 && dificultad >= 8)
        {
            Debug.Log("Dificultad mayor o igual a 8");

            if (Progreso < rondas)
            {

                FinMinijuego = false;

                interfazGeneral.GetComponent<InterfazGeneralManager>().error = true;
                interfazGeneral.GetComponent<InterfazGeneralManager>().progresoJugador++;
                interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
                ResetMinijuego();
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = tiempo;
                Progreso++;
                palabrasAcertadas++;
                InicioJuego();

            }
            else
            {
                enviarDatos.enabled = false;
                progresoManager.DatosFinales(porcentaje);

                RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
                RecompensasPrefab.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
                RecompensasPrefab.GetComponent<InterfazPanelManager>().nadaOExtra = longitudPalabrasRespondidas;
                interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);
                RecompensasPrefab.SetActive(true);
                interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = false;
            }
        }

    }

    void InicioJuego()
    {
        ResetMinijuego();
        SeleccionPalabra();
        DesglosePalabra();
        SpawnLetras();
    }

    void AccesoABDD(int maxLetras)
    {
        consultas.Query("SELECT ID_Palabra, Palabra, Dificultad.ID_Entrada " +
                        "FROM Palabras INNER JOIN Dificultad ON Palabras.ID_Palabra == Dificultad.ID_Entrada " +
                        "WHERE Dificultad.Identificador = 'Palabra' AND Dificultad.Minijuego3 > 0 AND LENGTH(Palabra) > 3 AND LENGTH(Palabra) <= " + maxLetras + " AND ID_Categoria IS NOT NULL");
        Palabras = consultas.StringReader(2);
    }

    string Categoria(string palabra)
    {
        consultas.Query("SELECT Palabras.ID_Categoria FROM Palabras WHERE Palabra = '" + palabra + "'");
        int ID = consultas.IntReader(1)[0];
        consultas.Query("SELECT Categoria FROM Categorias WHERE ID_Categoria == " + ID);
        string categoria = consultas.StringReader(1)[0];
        return categoria;
    }

    void Dificultades()
    {
        interfazGeneral.GetComponent<InterfazGeneralManager>().maxProgreso = rondas;
        interfazGeneral.GetComponent<InterfazGeneralManager>().maxRondas = rondas;

        switch (dificultad)
        {
            case 1:
                {
                    AccesoABDD(4);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                    recompensasManager = new RarezaPalabras(5, 0);
                    break;
                }
            case 2:
                {
                    AccesoABDD(6);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                    recompensasManager = new RarezaPalabras(10, 0);
                    break;
                }
            case 3:
                {
                    AccesoABDD(100);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                    recompensasManager = new RarezaPalabras(25, 0);
                    break;
                }
            case 4:
                {
                    AccesoABDD(6);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                    recompensasManager = new RarezaPalabras(28, 0);
                    break;
                };
            case 5:
                {
                    AccesoABDD(100);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                    recompensasManager = new RarezaPalabras(40, 0);
                    break;
                };
            case 6:
                {
                    AccesoABDD(100);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                    recompensasManager = new RarezaPalabras(60, 2);
                    break;
                };
            case 7:
                {
                    AccesoABDD(100);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(false);
                    recompensasManager = new RarezaPalabras(57, 3);
                    break;
                };
            case 8:
                {
                    AccesoABDD(100);
                    recompensasManager = new RarezaPalabras(67, 4);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(true);
                    tiempo = 60;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = tiempo;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                    break;
                };
            case 9:
                {
                    AccesoABDD(100);
                    recompensasManager = new RarezaPalabras(67, 9);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(true);
                    tiempo = 45;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = tiempo;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                    break;
                };
            case 10:
                {
                    AccesoABDD(100);
                    recompensasManager = new RarezaPalabras(67, 13);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(true);
                    tiempo = 45;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = tiempo;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                    botonPista.SetActive(false);
                    break;
                };
            case 11:
                {
                    AccesoABDD(100);
                    recompensasManager = new RarezaPalabras(57, 14);
                    interfazGeneral.GetComponent<InterfazGeneralManager>().textoTemporizador.SetActive(true);
                    tiempo = 30;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = tiempo;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                    botonPista.SetActive(false);
                    break;
                };
            default:
                break;
        }
    }

    void SeleccionPalabra()
    {
        int posicionPalabra = Random.Range(0, Palabras.Count);
        PalabraSeleccionada = Palabras[posicionPalabra];

        LongitudPalabra = PalabraSeleccionada.Length;
        PalabraOrdenada = new List<string>();
    }

    void DesglosePalabra()
    {
        PalabraOrdenadaPista.Clear();
        for (int i = 0; i < LongitudPalabra; i++)
        {
            Debug.Log(PalabraSeleccionada);
            PalabraOrdenada.Add(PalabraSeleccionada.Substring(i, 1));
            PalabraOrdenadaPista.Add(PalabraSeleccionada.Substring(i, 1));
        }
    }

    void SpawnLetras()
    {
        interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = tiempo;

        for (int i = 0; i < LongitudPalabra; i++)
        {
            GameObject letraNueva = Instantiate(letraPrefab, letrasParent);
            string letraAleatoria = PalabraOrdenada[Random.Range(0, PalabraOrdenada.Count)];
            letraNueva.GetComponent<Text>().text = letraAleatoria;
            PalabraOrdenada.Remove(letraAleatoria);

            GameObject espacioBlanco = Instantiate(espacioPrefab, espaciosParent);
            EspaciosLista.Add(espacioBlanco.transform);
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

    public void ComprobacionLetras() // Asignado al botón de terminar
    {
        string NuevaPalabra = "";
        bool palabraCompletada = false;

        // Leer la palabra formada
        for (int i = 0; i < espaciosParent.childCount; i++)
        {
            if (espaciosParent.GetChild(i).childCount == 0)
            {
                break;
            }

            string LetraEspacio = espaciosParent.GetChild(i).GetChild(0).GetComponent<Text>().text;
            NuevaPalabra += LetraEspacio;

            if (i == espaciosParent.childCount - 1)
            {
                palabraCompletada = true;
            }
        }

        if (palabraCompletada) // Si todos los espacios han sido rellenados
        {
            if (PalabraSeleccionada == NuevaPalabra) // Y la palabra es la esparada, terminar el minijuego con respuesta correcta
            {
                longitudPalabrasRespondidas.Add(PalabraSeleccionada.Length);
                comprobarsonidobienmal();

                if (contadorFallos == 0)
                {
                    porcentaje += 1f / rondas;
                    contadorFallos = 0;
                }
                else if (contadorFallos > 0)
                {
                    porcentaje += (1f / rondas) / 2f;
                }

                if (Progreso <= rondas)
                {
                    //Debug.Log("BIEEEEEN :D");
                    RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
                    interfazGeneral.GetComponent<InterfazGeneralManager>().progresoJugador++;


                    //Debug.Log("He entrado en el if");
                    palabrasAcertadas++;

                    if (Progreso < rondas)
                    {
                        interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
                        Progreso++;
                        InicioJuego();
                    }

                    PalabraRecompensada(NuevaPalabra);

                    FinMinijuego = true;
                    empezarMinijuegoSinTiempo = true;
                    Palabras.Remove(PalabraSeleccionada);
                    texCatPistaxDDDDD.SetActive(false);
                    contadorFallos = 0;
                    if (dificultad != 10 || dificultad != 11)
                    {
                        botonPista.SetActive(true);
                        //PalabraOrdenadaPista.Clear();
                        EspaciosLista.Clear();
                    }
                }
            }
            else // Si la palabra no es la esperada, necesitamos comprobar si forma parte del diccionario español
            {
                comprobacionDiccionario.Query("SELECT Palabra FROM LexicoEsp");
                if (palabrasPosibles == null) palabrasPosibles = comprobacionDiccionario.StringReader(1); // (Leer todas las palabras del español)

                for (int i = 0; i < palabrasPosibles.Count; i++)
                {
                    if (palabrasPosibles[i] == NuevaPalabra) // Si la palabra forma parte del diccionario español, terminar el minijuego con respuesta correcta
                    {
                        longitudPalabrasRespondidas.Add(PalabraSeleccionada.Length);
                        comprobarsonidobienmal();

                        if (contadorFallos == 0)
                        {
                            porcentaje += 1f / rondas;
                            contadorFallos = 0;
                        }
                        else if (contadorFallos > 0)
                        {
                            porcentaje += (1f / rondas) / 2f;
                        }

                        if (Progreso <= rondas)
                        {
                            //Debug.Log("La palabra existe, aunque no sea la esperada");
                            interfazGeneral.GetComponent<InterfazGeneralManager>().progresoJugador++;
                            RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;

                            Debug.Log("He entrado en el segundo if");
                            palabrasAcertadas++;

                            if (Progreso < rondas)
                            {
                                interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
                                Progreso++;
                                InicioJuego();
                            }

                            PalabrasRecompensa.Add(NuevaPalabra);
                            FinMinijuego = true;
                            empezarMinijuegoSinTiempo = true;
                            Palabras.Remove(PalabraSeleccionada);
                            texCatPistaxDDDDD.SetActive(false);
                            contadorFallos = 0;
                            if (dificultad != 10 || dificultad != 11)
                            {
                                botonPista.SetActive(true);
                                //PalabraOrdenadaPista.Clear();
                                EspaciosLista.Clear();
                            }
                            break;
                        }

                    }
                    else if (i == palabrasPosibles.Count - 1) // Si la palabra es incorrecta y no existe, informar del fallo
                    {
                        interfazGeneral.GetComponent<InterfazGeneralManager>().error = true;
                        comprobarsonidobienmal();
                        contadorFallos++;
                        Debug.Log("MAAAAAL D:");
                    }
                }
            }
            //Debug.Log("palabrasAcertadas: " + palabrasAcertadas);
            //Debug.Log("rondas: " + rondas);
            RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
            interfazGeneral.GetComponent<InterfazGeneralManager>().barraProgreso.GetComponent<Slider>().value = porcentaje;

            if (palabrasAcertadas == rondas) //fin minijuego
            {
                enviarDatos.enabled = false;
                progresoManager.DatosFinales(porcentaje);

                RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
                RecompensasPrefab.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
                interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);
                RecompensasPrefab.GetComponent<InterfazPanelManager>().nadaOExtra = longitudPalabrasRespondidas;
                RecompensasPrefab.SetActive(true);
            }
        }
        else
        {
            contadorFallos = 0;
            playSound(fallo);
            Debug.Log("no has hecho nada >:c");
        }
    }

    void PalabraRecompensada(string palabraElegida)
    {
        consultas.Query("SELECT Palabra FROM Palabras WHERE Palabra = '" + palabraElegida + "' AND ID_Categoria IS NOT NULL");
        if (consultas.Count() > 0)
        {
            PalabrasRecompensa.Add(palabraElegida);
            recompensasManager.RarezaObtenida(palabraElegida);
        }
    }

    public void piztaChachiguay()
    {
        PistasSegunCategoria();

        RevelarLetras(NumPistas);
    }

    void RevelarLetras(int numLetras)
    {
        for (int n = 0; n < numLetras; n++)
        {
            for (int i = 0; i < letrasParent.childCount; i++)
            {
                if (letrasParent.GetChild(i).GetComponent<Text>().text == PalabraOrdenadaPista[n])
                {
                    letrasParent.GetChild(i).transform.position = espaciosParent.GetChild(n).transform.position;
                    letrasParent.GetChild(i).SetParent(espaciosParent.GetChild(n));
                    //espacioPista.GetChild(n).GetChild(0).transform.position = espacioPista.GetChild(n).transform.position;
                    break;
                }
            }
        }

        botonPista.SetActive(false);
    }

    public void PistasSegunCategoria()
    {
        if (dificultad <= 3)
        {
            NumPistas = 2;
            texCatPistaxDDDDD.SetActive(true);
            texCatPistaxDDDDD.GetComponent<Text>().text = "Categoría: " + Categoria(PalabraSeleccionada);
        }

        if (dificultad == 4 || dificultad == 5)
        {
            NumPistas = 1;
            texCatPistaxDDDDD.SetActive(true);
            texCatPistaxDDDDD.GetComponent<Text>().text = "Categoría: " + Categoria(PalabraSeleccionada);
        }

        if (dificultad == 6)
        {
            NumPistas = 0;
            texCatPistaxDDDDD.SetActive(true);
            texCatPistaxDDDDD.GetComponent<Text>().text = "Categoría: " + Categoria(PalabraSeleccionada);
        }

        if (dificultad == 7 || dificultad == 8 || dificultad == 9)
        {
            NumPistas = 1;
        }

    }

    void ResetMinijuego()
    {
        for (int i = 0; i < LongitudPalabra; i++)
        {
            Destroy(espaciosParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < letrasParent.childCount; i++)
        {
            Destroy(letrasParent.GetChild(i).gameObject);
        }
    }

    public void PasarRonda()
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


        if (Progreso < rondas)
        {
            interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
            palabrasAcertadas++;
            empezarMinijuegoSinTiempo = true;
            Palabras.Remove(PalabraSeleccionada);
            texCatPistaxDDDDD.SetActive(false);
            contadorFallos = 0;
            InicioJuego();

            if (dificultad != 10 || dificultad != 11)
            {

                botonPista.SetActive(true);
                EspaciosLista.Clear();

            }

            if (dificultad == 10 || dificultad == 11)
            {
                botonPista.SetActive(false);
            }



        }
        if (palabrasAcertadas == rondas || Progreso >= rondas)
        {
            enviarDatos.enabled = false;
            progresoManager.DatosFinales(porcentaje);
            interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas = rondas;
            RecompensasPrefab.GetComponent<InterfazPanelManager>().num = porcentaje;
            RecompensasPrefab.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
            interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);
            RecompensasPrefab.SetActive(true);
        }
    }
}
