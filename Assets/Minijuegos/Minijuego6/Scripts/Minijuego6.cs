using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minijuego6 : MonoBehaviour
{
    //Constructor
    SQLQuery Constructor;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    //Recompensas
    List<string> recompensas = new List<string>();

    [Range(1, 11)] public int dificultad;

    int Tiempo = 35, rondaActual = 1;
    public int numRondas;
    public string FraseElegida;
    public List<string> ListaDeFrases;
    public Sprite Base, Fallo, Acierto;
    List<string> FraseDesordenada = new List<string>();
    List<string> listaFrase = new List<string>();
    List<string> listaFraseFinal = new List<string>();
    public GameObject FraseInstanciada;
    int ListaFraseCount;

    public Transform Palabras, Espacios;
    public string FraseFinal;
    public GameObject TextoDeEjemplo;
    
    //Interfaz
    public InterfazPanelManager PanelManager;
    public InterfazGeneralManager InterfazManager;
    public float NumAciertos, NumFallos;
    bool Acierto1;
    public GameObject menuConfirmar;

    //Correccion
    List<int> palabrasCorrectasPrimera = new List<int>(), palabrasIncorrectas = new List<int>();
    int numAciertosPrimera, numAciertosSegunda, numNoRespondidas;


    AudioSource reproductor;
    AudioClip acierto, fallo;

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

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        acierto = audioManager.acierto;
        fallo = audioManager.fallo;

        InterfazManager.GameTime = Tiempo;
        InterfazManager.empezarTemporizador = true;

        Acierto1 = true;
        InterfazManager.progresoJugador = NumAciertos;
        InterfazManager.maxProgreso = numRondas;
        InterfazManager.maxRondas = numRondas;
        InterfazManager.progresoRondas = rondaActual;
        //InterfazManager.panel.SetActive(true);
        ResetRonda();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !PanelManager.gameObject.activeSelf && !menuConfirmar.active)
        {
            Correccion();
        }

        if (Input.GetKeyDown(KeyCode.Return) && menuConfirmar.active)
        {
            confirmar();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cancelar();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            pasarRonda();
        }

            if (InterfazManager.GameTime <= 0)
        {
            numNoRespondidas += listaFrase.Count;
            SumaRondas();
        }
    }

    void ResetRonda()
    {
        VaciarListas();
        EliminarGameobjects();
        ElegirFrase();
        RellenarFrases();
        SpawnPalabras();
    }

    void SpawnPalabras()
    {
        for (int i = 0; i < FraseDesordenada.Count; i++)
        {
            GameObject PalabraNueva = Instantiate(FraseInstanciada, Palabras);
            PalabraNueva.GetComponentInChildren<Text>().text = FraseDesordenada[i].ToString();
            UpdateCanvas();
        }
        UpdateCanvas();
    }

    void UpdateCanvas()
    {
        Canvas.ForceUpdateCanvases();
        Palabras.GetComponent<HorizontalLayoutGroup>().enabled = false;
        Palabras.GetComponent<HorizontalLayoutGroup>().enabled = true;
        Debug.Log("ª");
    }

    void RellenarFrases()
    {

        ListaFraseCount = listaFrase.Count;
        for (int i = 0; i < ListaFraseCount; i++)
        {
            int x = Random.Range(0, listaFrase.Count);
            FraseDesordenada.Add(listaFrase[x]);
            listaFrase.Remove(listaFrase[x]);
        }
        listaFrase.AddRange(FraseElegida.Split(' '));
    }

    void ElegirFrase()
    {
        if (ListaDeFrases.Count > 0)
        {
            FraseElegida = ListaDeFrases[Random.Range(0, ListaDeFrases.Count)];
            ListaDeFrases.Remove(FraseElegida);
            listaFrase.AddRange(FraseElegida.Split(' '));
        }
    }

    void VaciarListas()
    {
        palabrasCorrectasPrimera.Clear();
        palabrasIncorrectas.Clear();

        listaFrase.Clear();
        FraseDesordenada.Clear();
        listaFraseFinal.Clear();
    }

    void EliminarGameobjects()
    {
        foreach (Transform child in Espacios)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in Palabras)
        {
            Destroy(child.gameObject);
        }
    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void comprobarsonidobienmal()
    {
        if (InterfazManager.error == true)
        {
            playSound(fallo);
        }
        else
        {
            playSound(acierto);
        }
    }

    public void Correccion()
    {
        listaFraseFinal.Clear();

        for (int i = 0; i < Espacios.childCount; i++)
        {
            if (i == Espacios.childCount - 1)
            {
                FraseFinal += Espacios.GetChild(i).GetComponentInChildren<Text>().text;
            }
            else
            {
                FraseFinal = FraseFinal + Espacios.GetChild(i).GetComponentInChildren<Text>().text + " ";
            }
        }
        listaFraseFinal.AddRange(FraseFinal.Split(' '));


        for (int i = 0; i < Espacios.childCount; i++)
        {
            if (listaFrase[i] == listaFraseFinal[i])
            {
                Espacios.GetChild(i).GetComponent<Image>().sprite = Acierto;
                comprobarsonidobienmal();
                if (!palabrasIncorrectas.Contains(i) && !palabrasCorrectasPrimera.Contains(i)) palabrasCorrectasPrimera.Add(i);
            }
            else
            {
                Espacios.GetChild(i).GetComponent<Image>().sprite = Fallo;
                Acierto1 = false;
                InterfazManager.error = true;
                Debug.Log("MAL");
                comprobarsonidobienmal();
                if (!palabrasIncorrectas.Contains(i)) palabrasIncorrectas.Add(i);
            }
        }

        if (FraseElegida == FraseFinal)
        {
            Debug.Log("HAS GANADO");
            ActualizarProgreso();

            ComprobarDiccionario();
            //AvanceProgreso();
            SumaRondas();
        }
        FraseFinal = "";
    }

    void ActualizarProgreso()
    {
        numAciertosPrimera += palabrasCorrectasPrimera.Count;
        numAciertosSegunda += palabrasIncorrectas.Count;

        InterfazManager.progresoJugador++;
        InterfazManager.porcentajeBarra = InterfazManager.progresoJugador / InterfazManager.maxProgreso;
        InterfazManager.barraProgreso.GetComponent<Slider>().value = InterfazManager.porcentajeBarra;
    }

    private void ComprobarDiccionario() // Cambiar para usar el constructor de busca de identificadores (+ quitar identificadores antes de instanciar frase)
    {
        Constructor = new SQLQuery("BaseLogopeda");
        Constructor.Query("Select Palabra From Palabras WHERE ID_Categoria IS NOT NULL");
        List<string> palabrasLogopeda = Constructor.StringReader(1);

        Debug.Log("palabrasLogopeda");

        for (int i = 0; i < listaFraseFinal.Count; i++)
        {
            if (palabrasLogopeda.Contains(listaFraseFinal[i]))
            {
                recompensas.Add(listaFraseFinal[i]);
                recompensasManager.RarezaObtenida(listaFraseFinal[i]);
            }
        }
    }


    float RendimientoFinal()

    {
        float porcentaje = 0;
        float palabrasTotales = numAciertosPrimera + numAciertosSegunda + numNoRespondidas;
        if(palabrasTotales > 0)
        {
            porcentaje = numAciertosPrimera / palabrasTotales + numAciertosSegunda / (palabrasTotales * 2);
        } 
        return porcentaje;
    }

    void SumaRondas()
    {
        InterfazManager.GameTime = Tiempo;

        if (rondaActual < numRondas)
        {
            rondaActual++;
            InterfazManager.progresoRondas = rondaActual;
            ResetRonda();
        }
        else
        {
            float porcentaje = RendimientoFinal();
            progresoManager.DatosFinales(porcentaje);

            InterfazManager.empezarTemporizador = false;
            InterfazManager.panel.SetActive(true);

            PanelManager.numPalabras = recompensas.Count;
            PanelManager.num = porcentaje;
            PanelManager.numEXP = 5; //exp provisional
            PanelManager.gameObject.SetActive(true);
        }
    }

    public void Reset()
    {
        int childCount = Espacios.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Espacios.GetChild(0).SetParent(Palabras);
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
            TextoDeEjemplo.GetComponent<Text>().text = "Arrastra las palabras aquí...";
        }
    }

    void AccesoABDD(int Dificultad)
    {
        Constructor = new SQLQuery("BaseLogopeda");
        Constructor.Query("select Frase from FrasesTexto Inner Join Dificultad on FrasesTexto.ID_Frase " +
            "= Dificultad.ID_Entrada where Dificultad.Minijuego6 = " + Dificultad);
        ListaDeFrases = Constructor.StringReader(1);
    }

    void Dificultades() // Completar el switch con las especificaciones de de dificultad del logopeda <---------------- (facil = 1, medio = 2, dificil = 3)
    {

        switch (dificultad)
        {
            case 1:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 120;
                    recompensasManager = new RarezaPalabras(7);
                    break;
                }
            case 2:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 120;
                    recompensasManager = new RarezaPalabras(15);
                    break;
                }
            case 3:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 120;
                    recompensasManager = new RarezaPalabras(25);
                    break;
                }
            case 4:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 120;
                    recompensasManager = new RarezaPalabras(35);
                    break;
                }
            case 5:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 120;
                    recompensasManager = new RarezaPalabras(50);
                    break;
                }
            case 6:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 180;
                    recompensasManager = new RarezaPalabras(90, 2);
                    break;
                }
            case 7:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 180;
                    recompensasManager = new RarezaPalabras(90, 5);
                    break;
                }
            case 8:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 180;
                    recompensasManager = new RarezaPalabras(90, 10);
                    break;
                }
            case 9:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 180;
                    recompensasManager = new RarezaPalabras(80, 20);
                    break;
                }
            case 10:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 180;
                    recompensasManager = new RarezaPalabras(60, 40);
                    break;
                }
            case 11:
                {
                    AccesoABDD(dificultad);
                    Tiempo = 180;
                    recompensasManager = new RarezaPalabras(50, 50);
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

        numAciertosPrimera = numAciertosPrimera + 0;

        if (rondaActual < numRondas)
        {
            rondaActual++;
            InterfazManager.progresoRondas = rondaActual;
            ResetRonda();
        }
        else
        {
            
            float porcentaje = RendimientoFinal();
            Debug.Log(porcentaje);
            progresoManager.DatosFinales(porcentaje);

            InterfazManager.empezarTemporizador = false;
            InterfazManager.panel.SetActive(true);

            PanelManager.numPalabras = recompensas.Count;
            PanelManager.num = porcentaje;
            PanelManager.gameObject.SetActive(true);
        }


    }
}