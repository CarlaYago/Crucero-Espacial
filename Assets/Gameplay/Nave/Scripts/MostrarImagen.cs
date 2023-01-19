using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MostrarImagen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image self;

    [Header("Audio")]
    AudioSource reproductor;
    AudioClip CapaPlanetario;

    void Start()
    {
        self = GetComponent<Image>();
        self.color = new Color(0,0,0,0);
        foreach (Transform child in transform) child.gameObject.SetActive(false);

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        CapaPlanetario = audioManager.CapaPlanetario;
    }
    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        self.color = Color.white;

        foreach (Transform child in transform) child.gameObject.SetActive(true);

        playSound(CapaPlanetario);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        self.color = new Color(0, 0, 0, 0);

        foreach (Transform child in transform) child.gameObject.SetActive(false);
    }
}
