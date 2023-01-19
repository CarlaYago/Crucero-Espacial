using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class SonidoController : MonoBehaviour
{
    [Header("Variables Generales")]
    public float volumenSonido;
    public GameObject barraSeleccionada;

    [Header("Sprites de opciones")]
    public Image sonido_on;
    public Image sonido_off;
    public Sprite volume_on;
    public Sprite volume_off;
    public List<Image> sonido_image;

    [Header ("Sonidos")]
    public AudioMixer audioMixer;

    int opcionOpciones, opcionOpcionesAnt;

    private void Awake()
    {
        //opcionOpciones = opcionOpcionesAnt = 1;
    }


    void Update()
    {
        barraSeleccionada = EventSystem.current.currentSelectedGameObject;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }


    public void cambioSprite()
    {        
        for (int i = 0; i < sonido_image.Count; i++)
        {
            sonido_image[i].GetComponent<Image>().sprite = volume_off;
        }

        for (int i = 0; i < sonido_image.Count; i++)
        {
            sonido_image[i].GetComponent<Image>().sprite = volume_on;

            if (sonido_image[i].gameObject == barraSeleccionada)
            {
                switch (i)
                {
                    case 0:
                        SetVolume(-72f);
                        Debug.Log("a1");
                        break;
                    case 1:
                        SetVolume(-65f);
                        Debug.Log("a2");
                        break;
                    case 2:
                        SetVolume(-56f);
                        Debug.Log("a3");
                        break;
                    case 3:
                        SetVolume(-48f);
                        break;
                    case 4:
                        SetVolume(-40f);
                        break;
                    case 5:
                        SetVolume(-32f);
                        break;
                    case 6:
                        SetVolume(-24f);
                        break;
                    case 7:
                        SetVolume(-16f);
                        break;
                    case 8:
                        SetVolume(-8f);
                        break;
                    case 9:
                        SetVolume(0f);
                        break;
                }
                break;
            }
        }
    }
}
