using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Minijuego5 : MonoBehaviour
{
    SQLQuery consultas;

    [Header("Listas")]
    Dictionary<int, string> listaCategorias = new Dictionary<int, string>();
    public List<string> listaPalabras = new List<string>();
    public List<string> PalabrasRecompensa;

    List<List<Text>> palabrasCorreccion = new List<List<Text>>(); // guarda las palabras que ha colocado el usuario 
    List<int> categoriasCorreccion = new List<int>(); // guarda las IDs de las categorías en orden para comprobar si las categorías de las palabras colocadas coinciden
    List<string> palabrasCorrectas = new List<string>(); // necesario para que la misma palabra no cuente como correcta más de una vez

    [Header("Transforms")]
    public Transform palabrasParent;
    public Transform categoriasParent, espaciosParent;

    [Header("Texts")]
    public Text categoriasText;
    public Text palabrasText;

    [Header("GameObject")]
    public GameObject espParentPrefab;
    public GameObject espacio;

    [Header("Variables")]
    [Range(1, 11)] public int dificultad;
    int numCategorias;
    int palabrasPorCategoria;
    public int archivosPorRonda;
    bool FinMinijuego;
    public int contadorFallos;
    public float porcentaje;
    public int numRondas;
    public int rondaActual = 1;
    public float miTiempo;

    [Header("Interfaz")]
    public GameObject interfazGeneral;
    public Slider barraProgreso;
    public GameObject menuConfirmar;

    [Header("Recompensas")]
    public GameObject interfazRecompensas;
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;

    int contadorArchivos = 0;
    int Aciertos = 0;

    [Header("Audio")]
    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip EnviarDatos, acierto, fallo,BandaSonora1, BandaSonora2, BandaSonora3;

    void CargarNivel()
    {
        progresoManager = new ProgresoMinijuegos();
        int[] difRondas = progresoManager.DificultadRondas();

        dificultad = difRondas[0];
        numRondas = difRondas[1];
    }

    void Start()
    {
        Debug.Log("Rondas: " + numRondas);

        menuConfirmar.SetActive(false);
        CargarNivel();

        consultas = new SQLQuery("BaseLogopeda");
        Dificultades();
        centrarCamaraInicial();
        ArreglarPalabras(categoriasParent);
        interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas = 1;
        interfazGeneral.GetComponent<InterfazGeneralManager>().maxProgreso = archivosPorRonda * numRondas;
        interfazGeneral.GetComponent<InterfazGeneralManager>().maxRondas = numRondas;

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
   
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return) && rondaActual <= numRondas)
        {
            Correccion();
        }


            if (interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime <= 0)
        {
            if (rondaActual < numRondas)
            {
                rondaActual++;
                interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
                interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = miTiempo;
                interfazGeneral.GetComponent<InterfazGeneralManager>().error = true;
                comprobarsonidobienmal();
                ResetMinijuego();
            }
            else
            {
                interfazRecompensas.GetComponent<InterfazPanelManager>().num = porcentaje;
                interfazRecompensas.GetComponent<InterfazPanelManager>().numPalabras = palabrasCorrectas.Count;
                interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);

                progresoManager.DatosFinales(porcentaje);
                interfazRecompensas.SetActive(true);
            }
        }
    }

    void InicioJuego(int categorias, int palabras, float tiempo)
    {
        numCategorias = categorias;
        palabrasPorCategoria = palabras;

        for (int i = 0; i < numCategorias; i++)
        {
            palabrasCorreccion.Add(new List<Text>());
        }

        GenerarCategorias();
        GenerarPalabras();
        GenerarEspacios();

        InterfazGeneral(categorias * palabras, tiempo);
        miTiempo = tiempo;
    }

    void InterfazGeneral(int numArchivos, float tiempo)
    {
        InterfazGeneralManager manager = interfazGeneral.GetComponent<InterfazGeneralManager>();

        manager.CountDownTime = tiempo;
        manager.GameTime = tiempo;
        manager.empezarTemporizador = true;
        archivosPorRonda = numArchivos;
        //Debug.Log("archivosPorRonda: " + archivosPorRonda);

    }

    void ArreglarPalabras(Transform parent)
    {
        Canvas.ForceUpdateCanvases();
        parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    void GenerarCategorias()
    {
        consultas.Query("SELECT ID_Categoria, Categoria From Categorias");
        Dictionary<int, string> categoriasReader = consultas.StringReaderID(1, 2);

        for (int i = 0; i < numCategorias; i++)
        {
            int n = Random.Range(0, categoriasReader.Count);

            int key = categoriasReader.ElementAt(n).Key;
            string value = categoriasReader.ElementAt(n).Value;

            Text categoria = Instantiate(categoriasText, categoriasParent);
            categoria.text = value;

            categoriasCorreccion.Add(key);
            listaCategorias.Add(key, value);

            categoriasReader.Remove(key);
        }
    }

    void GenerarPalabras()
    {
        // -- Rellenamos la lista de palabras con X palabras (numPalabras) por categoría creada --
        for (int i = 0; i < numCategorias; i++)
        {
            // Seleccionamos la ID de una categoría aleatoria
            int n = Random.Range(0, listaCategorias.Count);
            int id = listaCategorias.ElementAt(n).Key;

            // Buscamos todas las palabras que formen parte de esa categoría en la BDD
            consultas.Query("SELECT Palabra FROM Palabras WHERE ID_Categoria = " + id);
            List<string> palabrasTemp = consultas.StringReader(1);

            // Añadimos, por categoría, el número de palabras especificado
            for (int y = 0; y < palabrasPorCategoria; y++)
            {
                // Seleccionamos una palabra aleatoria de esa categoría
                int indice = Random.Range(0, palabrasTemp.Count);
                string palabra = palabrasTemp[indice];

                // La añadimos a la lista de palabras
                listaPalabras.Add(palabra);

                // Quitamos la palabra seleccionada para que no se repita
                palabrasTemp.Remove(palabra);
            }

            // Quitamos la categoría de la lista
            listaCategorias.Remove(listaCategorias.ElementAt(n).Key);
        }

        int count = listaPalabras.Count;

        // -- Instanciamos las palabras en órden aleatorio (de forma que no aparezcan ordenadas por categoría) --
        for (int i = 0; i < count; i++)
        {
            int numElegido = Random.Range(0, listaPalabras.Count);

            Text text = Instantiate(palabrasText, palabrasParent);
            text.text = listaPalabras[numElegido];

            listaPalabras.RemoveAt(numElegido);
        }
    }

    void GenerarEspacios()
    {
        for (int i = 0; i < palabrasPorCategoria; i++)
        {
            GameObject espParent = Instantiate(espParentPrefab, espaciosParent);

            for (int a = 0; a < numCategorias; a++)
            {
                GameObject espacioEnBlanco = Instantiate(espacio, espParent.transform);
                //listaCorreccion[a].Add(espacioEnBlanco);
            }
        }

        // contentParent.GetChild(0).SetSiblingIndex(contentParent.childCount);
        // contentParent.GetChild(0).SetSiblingIndex(contentParent.childCount);
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

    public void Correccion()
    {
        int contadorEspacios = 0;

        foreach (List<Text> palabras in palabrasCorreccion)
        {
            palabras.Clear();
        }

        for (int i = 0; i < palabrasCorreccion.Count; i++)
        {
            for (int n = 0; n < palabrasPorCategoria; n++)
            {
                Text palabra = null;

                if (espaciosParent.GetChild(n).GetChild(i).GetComponentInChildren<Text>() != null)
                    palabra = espaciosParent.GetChild(n).GetChild(i).GetComponentInChildren<Text>();

                palabrasCorreccion[i].Add(palabra);
            }
        }

        int numAciertos = 0;
        //int palabrasPorCorregir = numCategorias * palabrasPorCategoria - palabrasCorrectas.Count;
        int palabrasPorCorregir = numCategorias * palabrasPorCategoria;
        //Debug.Log("palabrasPorCorregir: " + palabrasPorCorregir);
        //Debug.Log("numCategorias: " + numCategorias);
        //Debug.Log("palabrasPorCategoria: " + palabrasPorCategoria);
        //Debug.Log("palabrasCorrectas.Count: " + palabrasCorrectas.Count);

        for (int i = 0; i < palabrasCorreccion.Count; i++)
        {
            List<Text> palabras = palabrasCorreccion[i];

            for (int n = 0; n < palabrasPorCategoria; n++)
            {
                if (palabras[n] != null)
                {
                    if (!palabrasCorrectas.Contains(palabras[n].text))
                    {
                        consultas.Query("SELECT ID_Categoria FROM Palabras WHERE Palabra = '" + palabras[n].text + "'");
                        List<int> idCategoria = consultas.IntReader(1);

                        //Debug.Log("contar falloh" + contadorFallos);

                        if (idCategoria[0] == categoriasCorreccion[i])
                        {
                            contadorArchivos++;
                            comprobarsonidobienmal();

                            if (contadorFallos == 0 && espaciosParent.GetChild(n).GetChild(i).GetComponentInChildren<ScriptFrases>().correccion == false)
                            {
                                espaciosParent.GetChild(n).GetChild(i).GetComponentInChildren<ScriptFrases>().correccion = true;
                                porcentaje += 1f / (numCategorias * palabrasPorCategoria * numRondas);
                            }
                            else if (contadorFallos > 0 && espaciosParent.GetChild(n).GetChild(i).GetComponentInChildren<ScriptFrases>().correccion == false)
                            {
                                espaciosParent.GetChild(n).GetChild(i).GetComponentInChildren<ScriptFrases>().correccion = true;
                                porcentaje += 1f / (numCategorias * palabrasPorCategoria * numRondas * 2f);
                            }                            

                            if (contadorArchivos == archivosPorRonda)
                            {
                                //Debug.Log("contadorArchivos: " + contadorArchivos);
                                //Debug.Log("archivosPorRonda: " + archivosPorRonda);
                                //Debug.Log("rondaActual: " + rondaActual);

                                if (rondaActual == numRondas)
                                {
                                    progresoManager.DatosFinales(porcentaje);
                                    interfazRecompensas.SetActive(true);
                                    interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = false;
                                    interfazGeneral.GetComponent<InterfazGeneralManager>().panel.SetActive(true);
                                    rondaActual++;
                                }
                                else
                                {
                                    rondaActual++;
                                    interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
                                    ResetMinijuego();
                                }
                            }

                            // Evitar que una palabra correcta pueda moverse de sitio
                            UI_ClickDragDrop dragScript = palabras[n].GetComponent<UI_ClickDragDrop>();
                            dragScript.enabled = false;

                            // Suma un nuevo acierto y lo añade a las palabras correctas
                            interfazGeneral.GetComponent<InterfazGeneralManager>().progresoJugador++;
                            interfazRecompensas.GetComponent<InterfazPanelManager>().num = porcentaje;
                            interfazGeneral.GetComponent<InterfazGeneralManager>().barraProgreso.GetComponent<Slider>().value = porcentaje;
                            numAciertos++;

                            palabrasCorrectas.Add(palabras[n].text);
                            PalabraRecompensada(palabras[n].text);
                        }
                        else
                        {
                            interfazGeneral.GetComponent<InterfazGeneralManager>().error = true;
                            comprobarsonidobienmal();
                            contadorFallos++;
                            palabras[n].transform.SetParent(palabrasParent);
                            Debug.Log("La palabra " + palabras[n].text + " no pertenece a la categoria de ID " + idCategoria[0]);
                        }
                    }
                }
                else
                {
                    contadorEspacios++;
                    playSound(fallo);
                    Debug.Log("La palabra no está rellenada");
                }
            }
        }
        Aciertos += numAciertos;
        if (Aciertos == palabrasPorCorregir)
        {
            //Debug.Log("Entra en el if: " + numAciertos);
            interfazRecompensas.GetComponent<InterfazPanelManager>().num = porcentaje;
            interfazRecompensas.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
            interfazGeneral.GetComponent<InterfazGeneralManager>().barraProgreso.GetComponent<Slider>().value = porcentaje;
            Aciertos = 0;
            //Debug.Log("Bien");
        }
    }

    void PalabraRecompensada(string palabraElegida)
    {
        PalabrasRecompensa.Add(palabraElegida);
        recompensasManager.RarezaObtenida(palabraElegida);
    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    InicioJuego(2, 2, 60);
                    recompensasManager = new RarezaPalabras(2, 0);
                    break;
                }
            case 2:
                {
                    InicioJuego(2, 5, 120);
                    recompensasManager = new RarezaPalabras(3, 0);
                    break;
                }
            case 3:
                {
                    InicioJuego(2, 10, 120);
                    recompensasManager = new RarezaPalabras(2, 0);
                    break;
                }
            case 4:
                {
                    InicioJuego(3, 5, 180);
                    recompensasManager = new RarezaPalabras(3, 0);
                    break;
                };
            case 5:
                {
                    InicioJuego(3, 10, 180);
                    recompensasManager = new RarezaPalabras(3, 0);
                    break;
                };
            case 6:
                {
                    InicioJuego(4, 5, 180);
                    recompensasManager = new RarezaPalabras(7, 0);
                    break;
                };
            case 7:
                {
                    InicioJuego(4, 10, 180);
                    recompensasManager = new RarezaPalabras(5, 0);
                    break;
                };
            case 8:
                {
                    InicioJuego(5, 8, 180);
                    recompensasManager = new RarezaPalabras(7, 0);
                    break;
                };
            case 9:
                {
                    InicioJuego(4, 13, 180);
                    recompensasManager = new RarezaPalabras(6, 1);
                    break;
                };
            case 10:
                {
                    InicioJuego(5, 10, 180);
                    recompensasManager = new RarezaPalabras(8, 1);
                    break;
                };
            case 11:
                {
                    InicioJuego(5, 13, 180);
                    recompensasManager = new RarezaPalabras(6, 1);
                    //interfazGeneral.GetComponent<InterfazGeneralManager>().maxProgreso = 65;
                    //interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime = 180f;
                    //interfazGeneral.GetComponent<InterfazGeneralManager>().GameTime = interfazGeneral.GetComponent<InterfazGeneralManager>().CountDownTime;
                    //interfazGeneral.GetComponent<InterfazGeneralManager>().empezarTemporizador = true;
                    break;
                };
            default:
                break;
        }
    }

    //LLAMAR CUANDO EL MINIJUEGO SE RESUELVA CORRECTAMENTE
    public void ResetMinijuego()
    {
        contadorFallos = 0;
        contadorArchivos = 0;
        numCategorias = 0;
        listaCategorias.Clear();
        listaPalabras.Clear();
        palabrasCorreccion.Clear();
        categoriasCorreccion.Clear();
        palabrasCorrectas.Clear();

        foreach (Transform categorias in categoriasParent)
        {
            Destroy(categorias.gameObject);
        }
        foreach (Transform espacios in espaciosParent)
        {
            Destroy(espacios.gameObject);
        }
        foreach (Transform palabras in palabrasParent)
        {
            Destroy(palabras.gameObject);
        }

        Dificultades();
        ArreglarPalabras(categoriasParent);
    }


    void centrarCamaraInicial()
    {
        ScrollRect espaciosScrollView = espaciosParent.parent.GetComponentInParent<ScrollRect>();
        espaciosScrollView.verticalScrollbar.value = 1;
        espaciosScrollView.horizontalScrollbar.value = 0;
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

        if(rondaActual <= numRondas)
        {
            rondaActual++;
            interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas++;
            ResetMinijuego();
        }

        if (rondaActual > numRondas)
        {
            interfazRecompensas.SetActive(true);
            interfazGeneral.GetComponent<InterfazGeneralManager>().progresoRondas = numRondas;
            progresoManager.DatosFinales(porcentaje);
            interfazRecompensas.GetComponent<InterfazPanelManager>().num = porcentaje;
            interfazRecompensas.GetComponent<InterfazPanelManager>().numPalabras = PalabrasRecompensa.Count;
            interfazGeneral.GetComponent<InterfazGeneralManager>().barraProgreso.GetComponent<Slider>().value = porcentaje;
            Aciertos = 0;
        }
  
    }

}