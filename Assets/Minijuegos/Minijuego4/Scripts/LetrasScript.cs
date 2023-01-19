using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LetrasScript : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    //Minijuego
    Minijuego6 MinijuegoScriptV6;
    Minijuego4 MinijuegoScriptV4;

    //Espacio
    EspacioScript EspacioManager;
    Transform espacio, letrasParent;

    //Bools
    [HideInInspector] public bool colocar, moviendose;
    [HideInInspector] public int pos;
    bool noTocandoLetras = true;

    void Start()
    {
        MinijuegoScriptV4 = FindObjectOfType<Minijuego4>();
        MinijuegoScriptV6 = FindObjectOfType<Minijuego6>();

        EspacioManager = FindObjectOfType<EspacioScript>();
        espacio = EspacioManager.transform;
        letrasParent = transform.parent;

        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
    }

    public void OnPointerClick(PointerEventData eventData) // Al dejar de pulsar
    {
        if (moviendose) // Si la letra estaba siendo arrastrada
        {
            if (colocar) // Y está tocando el espacio donde debe ser colocada, cambiar su padre y posición según corresponda
            {
                transform.SetParent(espacio);
                transform.SetSiblingIndex(pos);
                ActualizarTextos();
            }

            moviendose = false;
        }
        else // Si la letra solo fue pulsada
        {
            if (transform.parent == espacio)
            {
                transform.SetParent(letrasParent);
            }
            else transform.SetParent(espacio);

            ActualizarTextos();
        }
    }

    public void OnPointerDown(PointerEventData eventData) // Al comenzar a pulsar
    {
        EspacioManager.letraPrefab = GetComponent<LetrasScript>();
        ActualizarTextos();
    }

    void Update()
    {
        if (noTocandoLetras)
        {
            if (transform.position.x > espacio.transform.position.x)
            {
                pos = espacio.childCount;
            }
            else
            {
                pos = 0;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LetrasScript letraScript))
        {
            if (letraScript.colocar)
            {
                letraScript.noTocandoLetras = false;

                if (collision.transform.position.x > transform.position.x)
                {
                    letraScript.pos = transform.GetSiblingIndex() + 1;
                }
                else
                {
                    letraScript.pos = transform.GetSiblingIndex();
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LetrasScript letraScript))
        {
            if (letraScript.colocar)
            {
                letraScript.noTocandoLetras = true;
            }
        }
    }

    void ActualizarTextos()
    {
        if (MinijuegoScriptV4 != null) MinijuegoScriptV4.ActualizarTextoDeEjemplo();
        else MinijuegoScriptV6.ActualizarTextoDeEjemplo();
    }
}
