using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EspacioScriptM6 : MonoBehaviour // matar
{
    //Letra seleccionada
    public PalabraScript letraPrefab;
    public bool gapOcupado;

    Scrollbar scroll;
    BoxCollider2D col;
    float offsetInicial;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        offsetInicial = col.offset.x;
        scroll = transform.parent.parent.GetChild(1).GetComponent<Scrollbar>();
    }

    void Update()
    {
        PosicionCollider();
    }

    void PosicionCollider()
    {
        float compensacion = offsetInicial * (1 - scroll.size);
        float offsetFinal = compensacion - ((1 - scroll.value) * compensacion);

        col.offset = new Vector2(offsetFinal, col.offset.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PalabraScript letraScript))
        {
            letraScript.colocar = true;
            gapOcupado = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PalabraScript letraScript))
        {
            letraScript.colocar = false;
            gapOcupado = false;
        }
    }
}

