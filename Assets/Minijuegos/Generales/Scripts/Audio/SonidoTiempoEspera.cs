using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SonidoTiempoEspera : MonoBehaviour
{
    AudioSource reproductor;
    AudioClip PulsarTiempoEspera;

    Button boton;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        reproductor = audioManager.GetComponent<AudioSource>();
        PulsarTiempoEspera = audioManager.PulsarTiempoEspera;

        boton = GetComponent<Button>();
        boton.onClick.AddListener(sonidoBotonTiempoEspera);
    }

    public void sonidoBotonTiempoEspera()
    {
        reproductor.clip = PulsarTiempoEspera;
        reproductor.Play();
    }
}
