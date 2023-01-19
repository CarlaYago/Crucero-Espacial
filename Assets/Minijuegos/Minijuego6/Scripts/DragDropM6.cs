using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDropM6 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    LetrasScript letraScript;
    EspacioScript EspacioScript;
    Minijuego6 MinijuegoScript;

    Transform parent;
    Transform parent2;



    AudioManager audioManager;
    AudioSource reproductor;
    AudioSource AudioSource3;

    AudioClip arrastrarInicio, soltarEnCasilla, recolocar, BandaSonora1, BandaSonora2, BandaSonora3;
    void Start()
    {
        letraScript = GetComponent<LetrasScript>();
        EspacioScript = FindObjectOfType<EspacioScript>();
        MinijuegoScript = FindObjectOfType<Minijuego6>();
        parent = GameObject.Find("LETRAS").transform.GetChild(0).GetChild(0);
        parent2 = GameObject.Find("ESPACIOS").transform.GetChild(0).GetChild(0);

        audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        AudioSource3 = audioManager.AudioSource3;
        BandaSonora1 = audioManager.BandaSonora1;
        BandaSonora2 = audioManager.BandaSonora2;
        BandaSonora3 = audioManager.BandaSonora3;
        arrastrarInicio = audioManager.arrastrarInicio;
        soltarEnCasilla = audioManager.soltarEnCasilla;
        recolocar = audioManager.recolocar;

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

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (transform.parent != parent)
        {
            transform.SetParent(parent);
            MinijuegoScript.ActualizarTextoDeEjemplo();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        letraScript.moviendose = true; // Informar al script de letra que está siendo arrastrada (distinguir entre clic y arrastrar)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (EspacioScript != null)
        {
            if (EspacioScript.gapOcupado == false)
            {
                transform.SetParent(parent);
                MinijuegoScript.ActualizarTextoDeEjemplo();
                Canvas.ForceUpdateCanvases();
                parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
                parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
                playSound(recolocar);       
            }
            else
            {
                playSound(soltarEnCasilla);
            }
        }

        if (letraScript != null)
        {
            if (transform.parent != parent && EspacioScript.gapOcupado == true)
            {
                transform.SetParent(parent2);
                Canvas.ForceUpdateCanvases();
                parent2.GetComponent<HorizontalLayoutGroup>().enabled = false;
                parent2.GetComponent<HorizontalLayoutGroup>().enabled = true;
                playSound(recolocar);
            }
        }

    }
}

