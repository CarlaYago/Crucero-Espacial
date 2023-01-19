using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Minijuego17 : MonoBehaviour
{
    List<char> abecedario = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'Z' }; // *
    readonly List<char> letrasNoRepetibles = new List<char> { 'H', 'Z' };
    // * No se incluyen las letras Ñ, W, X ni Y como indican las intrucciones del logopeda
    public List<int> longitudPalabrasRespondidas = new List<int>();

    // Consultas SQL
    SQLQuery corrector;
    public List<string> palabrasLogopeda = new List<string>();
    public List<string> palabrasDiccionario = new List<string>();

    // Managers
    RarezaPalabras recompensasManager;
    ProgresoMinijuegos progresoManager;
    ManagerMinijuegos minijuegoManager; // Le falta detectar fallos
    public InterfazPanelManager interfazRecompensas;

    // Funcionamiento minijuego
    [Range(1, 11)] int dificultad;
    int rondas, archivosPorRonda, numAciertos;
    List<string> palabrasCorrectas = new List<string>(); List<string> palabrasIncorrectas = new List<string>();
    int inputActual;

    [Header("Referencias Escena")]
    public GameObject letraPrefab;
    public Transform letrasParent;
    public Button botonEnviar;
    List<TMP_InputField> inputFields = new List<TMP_InputField>();
    public ScrollRect scrollView;

    [Header("Referencias Assets")]
    public Sprite inputCorrecto, inputIncorrecto;

    List<string> recompensas = new List<string>();
    public GameObject menuConfirmar;

    [Header("Referencias Assets")]
    AudioSource reproductor;
    AudioSource AudioSource3;
    AudioClip acierto, fallo, BandaSonora1, BandaSonora2, BandaSonora3;

    void ReferenciasCorrecion()
    {
        corrector = new SQLQuery("BaseLogopeda");

        corrector.Query("SELECT Palabra FROM Palabras WHERE ID_Categoria IS NOT NULL");
        palabrasLogopeda = corrector.StringReader(1);

        corrector = new SQLQuery("LexicoEsp");

        corrector.Query("SELECT Palabra FROM LexicoEsp");
        palabrasDiccionario = corrector.StringReader(1);
    }

    void Start()
    {
        menuConfirmar.SetActive(false);
        ReferenciasCorrecion();

        progresoManager = new ProgresoMinijuegos();
        progresoManager.DificultadRondasAuto(ref dificultad, ref rondas);

        Dificultades();

        minijuegoManager = new ManagerMinijuegos(rondas, archivosPorRonda, ReinicioMinijuego);
        minijuegoManager.InterfazRecompensas(interfazRecompensas);
        botonEnviar.onClick.AddListener(Correccion);

        //letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 1;

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

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void comprobarsonidobienmal()
    {
        
    }

    void Update()
    {
        /*if (scrollView.verticalScrollbar.enabled)
        {
            if (letrasParent.GetComponent<GridLayoutGroup>().constraintCount == 1)
            {
                letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 2;
            }
        }*/

        minijuegoManager.Temporizador(60);

        if (Input.GetKeyDown(KeyCode.Return) && !minijuegoManager.interfazRecompensas.gameObject.activeSelf)
        {
            Correccion();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CambiarDeInput();
        }
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

    void GenerarLetras(int numLetras)
    {
        archivosPorRonda = numLetras;

        for (int i = 0; i < numLetras; i++)
        {
            GameObject letraObject = Instantiate(letraPrefab, letrasParent);
            TextMeshProUGUI textoLetra = letraObject.GetComponentInChildren<TextMeshProUGUI>();

            char letra = abecedario[Random.Range(0, abecedario.Count)];
            if (letrasNoRepetibles.Contains(letra)) abecedario.Remove(letra);

            textoLetra.text = letra.ToString();

            TextMeshProUGUI textoTemp = letraObject.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            textoTemp.text = "Palabra que comienze por " + letra + "...";

            TMP_InputField inputField = letraObject.GetComponentInChildren<TMP_InputField>();
            int inputIndex = i;
            //
            inputField.onSelect.AddListener(delegate { inputActual = inputIndex; });
            if (i == 0) inputField.Select();
            //
            inputFields.Add(inputField);
        }
    }

    void ReinicioMinijuego()
    {
        foreach (Transform child in letrasParent) Destroy(child.gameObject);

        inputFields.Clear();
        //palabrasCorrectas.Clear();
        palabrasIncorrectas.Clear();

        Dificultades();
    }

    public void Correccion() // Activado por el botón de enviar datos
    {
        numAciertos = 0;
        bool fallo = false;

        foreach (Transform child in letrasParent)
        {
            TMP_InputField palabra = child.GetComponentInChildren<TMP_InputField>();
            string letra = child.GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, 1);

            if (palabra.text != "" && !palabrasCorrectas.Contains(palabra.text)) // Si la palabra no está vacía y no se ha usado ya correctamente, corregirla
            {
                if (palabra.text.StartsWith(letra)) // Si la palabra empieza por la letra correcta, compararla con las palabras de BDD
                {
                    if (palabrasLogopeda.Contains(palabra.text)) // Palabra correcta por parte de las palabras del logopeda
                    {
                        recompensas.Add(palabra.text);
                        longitudPalabrasRespondidas.Add(palabra.text.Length);

                        recompensasManager.RarezaObtenida(palabra.text);

                        PalabraCorrecta(palabra, fallo);
                    }
                    else if (palabrasDiccionario.Contains(palabra.text)) // Palabra correcta por comprobación diccionario
                    {
                        longitudPalabrasRespondidas.Add(palabra.text.Length);
                        PalabraCorrecta(palabra, fallo);
                    }
                    else // Si la palabra no ha sido determinada como correcta, marcar el fallo
                    {
                        PalabraIncorrecta(palabra, ref fallo);
                    }
                }
                else // Si no empieza por la letra correcta, marcar el fallo
                {
                    PalabraIncorrecta(palabra, ref fallo);
                }
            }
        }

        minijuegoManager.EnviarArchivos(recompensas, numAciertos, fallo);
    }

    void PalabraCorrecta(TMP_InputField palabra, bool error)
    {
        Image img = palabra.GetComponent<Image>();

        numAciertos++;
        palabra.interactable = false;
        img.sprite = inputCorrecto;
        playSound(acierto);

        palabrasCorrectas.Add(palabra.text);
        if (!error) CambiarDeInput();
    }

    void PalabraIncorrecta(TMP_InputField palabra, ref bool error)
    {
        Image img = palabra.GetComponent<Image>();
        int index = inputFields.IndexOf(palabra);

        img.sprite = inputIncorrecto;
        playSound(fallo);
        if (!palabrasIncorrectas.Contains(palabra.text)) error = true;

        palabrasIncorrectas.Add(palabra.text);
        inputFields[index].ActivateInputField();
    }

    void Dificultades()
    {
        switch (dificultad)
        {
            case 1:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 1;
                    recompensasManager = new RarezaPalabras(2);
                    GenerarLetras(3);
                    break;
                }
            case 2:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 1;
                    recompensasManager = new RarezaPalabras(5);
                    GenerarLetras(4);
                    break;
                }
            case 3:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 1;
                    recompensasManager = new RarezaPalabras(7);
                    GenerarLetras(5);
                    break;
                }
            case 4:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 1;
                    recompensasManager = new RarezaPalabras(12);
                    GenerarLetras(6);
                    break;
                }
            case 5:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 1;
                    recompensasManager = new RarezaPalabras(14);
                    GenerarLetras(7);
                    break;
                }
            case 6:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    recompensasManager = new RarezaPalabras(25);
                    GenerarLetras(8);
                    break;
                }
            case 7:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    recompensasManager = new RarezaPalabras(22);
                    GenerarLetras(9);
                    break;
                }
            case 8:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    recompensasManager = new RarezaPalabras(30);
                    GenerarLetras(10);
                    break;
                }
            case 9:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    recompensasManager = new RarezaPalabras(28);
                    GenerarLetras(11);
                    break;
                }
            case 10:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    recompensasManager = new RarezaPalabras(33);
                    GenerarLetras(12);
                    break;
                }
            case 11:
                {
                    letrasParent.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    recompensasManager = new RarezaPalabras(31);
                    GenerarLetras(13);
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

        ReinicioMinijuego();
    }
}