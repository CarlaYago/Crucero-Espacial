using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfazGeneralManager : MonoBehaviour
{
    [Header("Menú Información")]

    public GameObject menuInfo;

    [Header("Barra de Progreso")]

    public GameObject textoBarra;
    public GameObject fillBarra;
    public Slider barraProgreso;
    public float porcentajeBarra;
    public float progresoJugador;
    public float maxProgreso;

    [Header("Temporizador")]

    public GameObject textoTemporizador;
    public GameObject panel;
    public float CountDownTime;
    public bool empezarTemporizador = false;

    [Header("Rondas")]

    public GameObject textoRondas;
   // public GameObject textoMaxRondas;
    public int progresoRondas;
    public int maxRondas;

    [Header("Error")]
    public GameObject imagenError;
    public bool error = false;

    public float GameTime;
    private float timer = 0;
    

    void Start()
    {
        //GameTime = CountDownTime;
    }

    void Update()
    {
        if(error == true)
        {
            StartCoroutine("Error");
        }
        else
        {
            textoBarra.GetComponent<TextMeshProUGUI>().text = "Subiendo archivos (" + progresoJugador.ToString("0") + " de " + maxProgreso.ToString("0") + ")";
        }

        // se debe de indicar desde el propio minijuego ( este valor es igual al porcentaje del minijuego en sí)
       // porcentajeBarra = progresoJugador / maxProgreso;
       // barraProgreso.GetComponent<Slider>().value = porcentajeBarra; 
        

        if (empezarTemporizador == true)
        {
            //Debug.Log("Ha empezado el temporizador");
            int M = (int)(GameTime / 60);
            float S = GameTime % 60;
            textoTemporizador.GetComponent<TextMeshProUGUI>().text = M + ":" + string.Format("{0:00}", S);

            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0;
                GameTime--;
                if (M <= 0 && S <= 0)
                {
                    GameTime = 0;
                    //panel.SetActive(true); -- activar desde los scripts de cada minijuego

                }
            }
        }
        
        textoRondas.GetComponent<TextMeshProUGUI>().text = "Ronda " + progresoRondas + "/" + maxRondas;
        //textoMaxRondas.GetComponent<TextMeshProUGUI>().text = ;
     }

    public void abirMenuInfo()
    {
        menuInfo.SetActive(true);
    }

    public void cerrarMenuInfo()
    {
        menuInfo.SetActive(false);
    }

    public void salir()
    {

    }

    IEnumerator Error()
    {
        
        imagenError.SetActive(true);
        fillBarra.GetComponent<Image>().color = new Color(0.89f, 0.04f, 0f, 1f);
        textoBarra.GetComponent<TextMeshProUGUI>().text = "Error en archivo";
        yield return new WaitForSeconds(1f);
        error = false;
        imagenError.SetActive(false);
        fillBarra.GetComponent<Image>().color = new Color(0.10f, 1f, 0f, 1f);
        textoBarra.GetComponent<TextMeshProUGUI>().text = "Subiendo archivos (" + progresoJugador.ToString("0") + " de " + maxProgreso.ToString("0") + ")";


    }
}
