using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SonidoBotones : MonoBehaviour
{
    AudioSource reproductor;
    AudioClip PulsarBoton;

    Button boton;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        reproductor = audioManager.AudioSource2;
        reproductor.volume = 0.2f;
        PulsarBoton = audioManager.PulsarBoton;

        boton = GetComponent<Button>();
        boton.onClick.AddListener(sonidoBotonGenerico);
    }
 
    void sonidoBotonGenerico()
    {
        reproductor.clip = PulsarBoton;
        reproductor.Play();
    }
}
