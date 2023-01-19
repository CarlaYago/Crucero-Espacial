using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EspacioScript : MonoBehaviour
{
    //Letra seleccionada
    [Header("Datos")]
    public LetrasScript letraPrefab;
    public bool gapOcupado;

    Scrollbar scroll;
    float scrollSize;

    BoxCollider2D col;
    float offsetInicial;

    [Header("Adaptación para ScrollViews")]
    public bool adaptarAScrollView;

    AudioManager audioManager;
    AudioSource reproductor;
    AudioClip arrastrarInicio, soltarEnCasilla, recolocar;

    void Start()
    {
        if (adaptarAScrollView)
        {
            scroll = transform.parent.parent.GetChild(1).GetComponent<Scrollbar>();

            scrollSize = scroll.size;
            scroll.onValueChanged.AddListener(PosicionCollider);

            col = GetComponent<BoxCollider2D>();
            offsetInicial = col.offset.x;
        }

     
    }
   

    void Update()
    {
        if (scroll != null && !Mathf.Approximately(scrollSize, scroll.size))
        {
            scrollSize = scroll.size;
            PosicionCollider(0);
        }
    }

    void PosicionCollider(float a)
    {
        col.offset = new Vector2(offsetInicial, col.offset.y);
        float offsetFinal = offsetInicial - col.bounds.center.x;

        col.offset = new Vector2(offsetInicial + offsetFinal, col.offset.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LetrasScript letraScript))
        {
            letraScript.colocar = true;
            gapOcupado = true;
          
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LetrasScript letraScript))
        {
            letraScript.colocar = false;
            gapOcupado = false;
          
        }

    }

}