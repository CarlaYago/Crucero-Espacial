using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class boton7 : MonoBehaviour
{
    SQLQuery consultas;

    Button bton;
    Minijuego7 Mj7;
    InterfazGeneralManager interfazScript;
    public int marcador;


    AudioSource reproductor;
    AudioClip acierto, fallo;

    public List<Sprite> imagenmalybien; //si en arte hacen imagenes para el feedbak visual usar esta opción
    void Start()
    {
        Mj7 = FindObjectOfType<Minijuego7>();
        interfazScript = FindObjectOfType<InterfazGeneralManager>();
        bton = GetComponent<Button>();
        bton.onClick.AddListener(Click);

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        acierto = audioManager.acierto;
        fallo = audioManager.fallo;
    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void comprobarsonidobienmal()
    {
        if (interfazScript.GetComponent<InterfazGeneralManager>().error == true)
        {
            playSound(fallo);
        }
        else
        {
            playSound(acierto);
        }

    }
    public void Click()
    {
        if (CompareTag("Impostor"))
        {
            Debug.Log("correcto");
            comprobarsonidobienmal();
            Mj7.impostoresEnc++;
            marcador = Mj7.imp * (Mj7.rondaActual - 1) + Mj7.impostoresEnc;
            interfazScript.progresoJugador = marcador;
            if (Mj7.fallados == 0)
            {
                Mj7.porcentaje += (1f / (Mj7.imp * Mj7.rondas));
                Mj7.exp += 5;// ----- pendiente de revisión -----//
               
            }
            else if (Mj7.fallados > 0)
            {
                Mj7.porcentaje += (1f / (((float)Mj7.imp * (float)Mj7.rondas) * 2f));
                Mj7.exp += 1;// ----- pendiente de revisión -----//
            }
            Mj7.fallados = 0;
            //bton.image.color = Color.green;
            bton.image.sprite = imagenmalybien[0]; //si en arte hacen imagenes para el feedbak visual usar esta opción

            PalabraRecompensada(bton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);


            if (Mj7.rondaActual < Mj7.rondas && Mj7.impostoresEnc >= Mj7.imp)
            {
                Mj7.Comprobar();
                Mj7.Dificultades();
                Mj7.impostoresEnc = 0;
            }
            else if(Mj7.rondaActual >= Mj7.rondas && Mj7.impostoresEnc >= Mj7.imp)
            {
                Debug.Log("fin");
                interfazScript.progresoRondas = Mj7.rondas;
                Mj7.StartCoroutine("ActivarRecompensas");
                Mj7.impostoresEnc = 0;

            }
           
            interfazScript.barraProgreso.GetComponent<Slider>().value = Mj7.porcentaje;
        }
        else
        {
            //bton.image.color = Color.red; // sin necesidad de sprites
            bton.image.sprite = imagenmalybien[1]; //si en arte hacen imagenes para el feedbak visual usar esta opción
            Debug.Log("mal");
            interfazScript.error = true;
            comprobarsonidobienmal();
            Mj7.fallados++;
        }
        bton.enabled = false;
    }

    void PalabraRecompensada(string palabraElegida)
    {
        consultas = new SQLQuery("BaseLogopeda");

        consultas.Query("SELECT Palabra FROM Palabras WHERE Palabra = '" + palabraElegida + "' AND ID_Categoria IS NOT NULL");
        if (consultas.Count() > 0)
        {
            Mj7.palabrasConseguidas.Add(palabraElegida);
            Mj7.recompensasManager.RarezaObtenida(palabraElegida);
        }
    }
}