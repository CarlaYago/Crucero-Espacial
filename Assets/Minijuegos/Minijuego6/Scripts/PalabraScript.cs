using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PalabraScript : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    //Minijuego
    Minijuego6 MinijuegoScript;

    //Espacio
    EspacioScriptM6 EspacioManager;
    Transform espacio;

    //Bools
    [HideInInspector] public bool colocar;
    [HideInInspector] public int pos;
    bool noTocandoLetras;

    void Start()
    {
        MinijuegoScript = FindObjectOfType<Minijuego6>();
        EspacioManager = FindObjectOfType<EspacioScriptM6>();
        espacio = EspacioManager.transform;
    }

    public void OnPointerClick(PointerEventData eventData) // Al dejar de pulsar
    {
        noTocandoLetras = true;

        if (colocar)
        {
            transform.SetParent(espacio);
            transform.SetSiblingIndex(pos);
            MinijuegoScript.ActualizarTextoDeEjemplo();
        }
    }

    public void OnPointerDown(PointerEventData eventData) // Al comenzar a pulsar
    {
        EspacioManager.letraPrefab = GetComponent<PalabraScript>();
        MinijuegoScript.ActualizarTextoDeEjemplo();
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
        if (collision.TryGetComponent(out PalabraScript letraScript))
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
        if (collision.TryGetComponent(out PalabraScript letraScript))
        {
            if (letraScript.colocar)
            {
                letraScript.noTocandoLetras = true;
            }
        }
    }
}
