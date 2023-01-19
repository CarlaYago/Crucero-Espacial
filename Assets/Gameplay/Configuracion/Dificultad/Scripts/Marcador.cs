using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class Marcador : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI dificultad, distancia;

    RectTransform parentTransform;
    CrearMarcador generador;

    float posY;
    Vector2 posPrevia;

    bool dontDestroy;

    AudioSource reproductor;
    AudioClip recolocar;
    void Start()
    {
        parentTransform = transform.parent.GetComponent<RectTransform>();

        generador = FindObjectOfType<CrearMarcador>();
        posY = parentTransform.position.y;

        posPrevia = parentTransform.position;
        ActualizarMarcador();

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        recolocar = audioManager.recolocar;
    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!dontDestroy)
        {
            int index = Index();
            generador.posiciones.RemoveAt(index);
            Destroy(parentTransform.gameObject);
            playSound(recolocar);
            generador.UpdatearPosiciones();
           
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posPrevia = parentTransform.position;
        dontDestroy = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float posX = eventData.position.x;
        float posXClamped = Mathf.Clamp(posX, generador.limites.x, generador.limites.y);
        parentTransform.position = new Vector2(posXClamped, posY);

        ActualizarMarcador();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dontDestroy = false;
        int index = Index();

        float nuevaPos = generador.PosicionSinSolapar(parentTransform.position.x, posPrevia.x, index);
        parentTransform.position = new Vector2(nuevaPos, posY);
        ActualizarMarcador();

        posPrevia.x = nuevaPos;

        generador.posiciones[index] = nuevaPos;
        generador.UpdatearPosiciones();
    }

    void ActualizarMarcador()
    {
        float dist = generador.limites.y - generador.limites.x;
        float pos = parentTransform.position.x - generador.limites.x;

        distancia.text = (pos / dist * 100).ToString("0.#", CultureInfo.InvariantCulture) + "%";
    }

    int Index()
    {
        int index = -1;

        for (int i = 0; i < generador.posiciones.Count; i++)
        {
            if (Mathf.Abs(posPrevia.x - generador.posiciones[i]) < 0.05f)
            {
                return i;
            }
        }

        Debug.Log(posPrevia.x);

        return index;
    }
}
