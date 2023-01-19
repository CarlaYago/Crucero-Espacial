using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDropM14 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Letra14_script letraScript;
    Espacio14_Script espacioScript;
    Minijuego14 minijuegoScript;
    Transform parent;

    void Start()
    {
        letraScript = GetComponent<Letra14_script>();
        espacioScript = FindObjectOfType<Espacio14_Script>();
        minijuegoScript = FindObjectOfType<Minijuego14>();
        parent = transform.parent;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (transform.parent != parent)
        {
            transform.SetParent(parent);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        letraScript.moviendose = true; // Informar al script de letra que está siendo arrastrada (distinguir entre clic y arrastrar)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (espacioScript != null)
        {
            if (espacioScript.gapOcupado == false)
            {
                transform.SetParent(parent);
                Canvas.ForceUpdateCanvases();
                parent.GetComponent<GridLayoutGroup>().enabled = false;
                parent.GetComponent<GridLayoutGroup>().enabled = true;
            }
        }
    }
}
