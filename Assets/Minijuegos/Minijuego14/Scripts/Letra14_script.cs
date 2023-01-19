using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class Letra14_script : MonoBehaviour
{
    // Start is called before the first frame update
    Espacio14_Script EspacioManager;
    Transform espacio, letrasParent;

    //Bools
    [HideInInspector] public bool colocar, moviendose;
    [HideInInspector] public int pos;
    bool noTocandoLetras = true;

    void Start()
    {

        EspacioManager = FindObjectOfType<Espacio14_Script>();
        espacio = EspacioManager.transform;
        letrasParent = transform.parent;

        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
    }

    //public void OnPointerClick(PointerEventData eventData) // Al dejar de pulsar
    //{
    //    if (moviendose) // Si la letra estaba siendo arrastrada
    //    {
    //        if (colocar) // Y está tocando el espacio donde debe ser colocada, cambiar su padre y posición según corresponda
    //        {
    //            transform.SetParent(espacio);
    //            transform.SetSiblingIndex(pos);
    //        }

    //        moviendose = false;
    //    }
    //    else // Si la letra solo fue pulsada
    //    {
    //        if (transform.parent == espacio)
    //        {
    //            transform.SetParent(letrasParent);
    //        }
    //        else transform.SetParent(espacio);
    //    }
    //}


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
        if (collision.TryGetComponent(out Letra14_script letraScript))
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
        if (collision.TryGetComponent(out Letra14_script letraScript))
        {
            if (letraScript.colocar)
            {
                letraScript.noTocandoLetras = true;
            }
        }
    }
}
