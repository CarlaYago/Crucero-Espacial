using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public struct Secuencia
{
    public string Nombre;
    public Cinematica imagen;
    public List<Dialogo> dialogo;
}

[Serializable]
public struct Cinematica
{
    public Texture imagen;
    public VideoClip videoIntro;
    public VideoClip videoLoop;
}


[Serializable]
public struct Dialogo
{
    public Personajes portavoz;
    public string narracion;
    public Texture imagen;
}

public enum Personajes
{
    Ninguno,
    IA,
    Jugador
}

public class cinematicasScript : MonoBehaviour
{
    SQLQuery consultas;

    public string nombreClaveJugador;
    DatosJugador datos;
    public RawImage imagenes;
    VideoPlayer reproductor;
    Texture imagenVideo;
    public List<Secuencia> cinematica;
    public TextMeshProUGUI texto;
    public Text textoPortavoz;
    int vinetaActual;
    int dialogoActual;
    public GameObject boton;
    AudioSource reprod;
    AudioSource AudioSource3;
    AudioClip audioC;
    AudioManager audioManager;
    int audioActual;

    void Start()
    {
        consultas = new SQLQuery("Usuarios");

        audioManager = FindObjectOfType<AudioManager>();
        //
        reprod = audioManager.GetComponent<AudioSource>();
        //
        reproductor = imagenes.GetComponent<VideoPlayer>();
        imagenVideo = imagenes.texture;
        datos = FindObjectOfType<DatosJugador>();
        vinetaActual = 0;
        dialogoActual = 0;
        audioActual = 0;
        boton.SetActive(false);
        pasarPrueba();
      
    }

   
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return) && boton.active)
        {
            pasarPrueba();
        }
    }

    IEnumerator Reloj()
    {
        textoPortavoz.text = portavoz(cinematica[vinetaActual].dialogo[dialogoActual].portavoz);
        if (cinematica[vinetaActual].dialogo[dialogoActual].imagen != null) imagenes.texture = cinematica[vinetaActual].dialogo[dialogoActual].imagen;

        string dialogo = cinematica[vinetaActual].dialogo[dialogoActual].narracion;
        dialogo = dialogo.Replace(nombreClaveJugador, datos.nombre);

        //audioC = audioManager.audioCinematica[audioActual];
        //ost(audioC);
       
        foreach (char caracter in dialogo)
        {
           texto.GetComponent<TextMeshProUGUI>().text += caracter;
           yield return new WaitForSeconds(0.05f);
        }
        boton.SetActive(true);
        
    }

    IEnumerator video(VideoClip intro, VideoClip loop)
    {
        float duracion = 0;
        imagenes.texture = imagenVideo;
        reproductor.isLooping = false;

        if (intro != null)
        {
            reproductor.clip = intro;
            duracion = (float)intro.length;
        } 
        yield return new WaitForSeconds(duracion);
        if (loop != null)
        {
            reproductor.clip = loop;
            reproductor.isLooping = true;
        }      
    } 

    void asignarCinematica(Cinematica cinematica)
    {
        if(cinematica.imagen != null)
        {
            imagenes.texture = cinematica.imagen;
        }
        else if (cinematica.videoIntro != null || cinematica.videoLoop != null)
        {
            StartCoroutine(video(cinematica.videoIntro, cinematica.videoLoop));
        }
        else
        {
            Debug.LogError("Eres un puto subnormal que no sabes asignar cosas. Hijo de puta");
        }
    }

    public void pasarPrueba()
    {
        if (vinetaActual == 10 && dialogoActual > cinematica[vinetaActual].dialogo.Count - 1)
        {
            consultas.Query("UPDATE Usuarios SET CinematicaVista = 1 WHERE ID_Usuario = " + datos.id);
            datos.cinematicaVista = true;
            SceneManager.LoadScene("Nave");
        }
        else
        {

            if (dialogoActual <= cinematica[vinetaActual].dialogo.Count - 1)
            {
                boton.SetActive(false);
                if (dialogoActual == 0)
                {
                    asignarCinematica(cinematica[vinetaActual].imagen);
                }
                texto.text = " ";
                textoPortavoz.text = " ";
                StartCoroutine(Reloj());
                dialogoActual++;
            }
            else
            {
                boton.SetActive(false);
                dialogoActual = 0;
                vinetaActual++;
                texto.text = " ";
                pasarPrueba();
            }
            audioActual++;
        }

    }

    string portavoz(Personajes personaje)
    {
        switch (personaje)
        {
            case Personajes.Ninguno:
                return "";
                break;
            case Personajes.IA:
                texto.GetComponent<TextMeshProUGUI>().color = Color.yellow;
                textoPortavoz.color = Color.yellow;
                return "IA";
                break;
            case Personajes.Jugador:
                texto.GetComponent<TextMeshProUGUI>().color = Color.green;
                textoPortavoz.color = Color.green;
                return datos.nombre;
                break;
            default:
                return "";
                break;
        }
    }

    /*public void ost(AudioClip musica)
    {
        reprod.clip = musica;
        reprod.Play();
    }*/
}