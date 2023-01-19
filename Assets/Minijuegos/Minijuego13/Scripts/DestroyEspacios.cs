using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DestroyEspacios : MonoBehaviour, IPointerClickHandler
{
    public bool moverEspacio;
    Canvas CanvasMinijuego13;
    Collider2D col;

    void Start()
    {
        CanvasMinijuego13 = FindObjectOfType<Canvas>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector2 posRaton = Input.mousePosition;
        Vector2 pos = transform.position;
        float anchura = col.bounds.extents.x;
        float altura = col.bounds.extents.y;

        if (posRaton.x <= pos.x + anchura && posRaton.x > pos.x - anchura)
        {
            if (posRaton.y <= pos.y + altura && posRaton.y > pos.y - altura)
            {

                if (gameObject.GetComponent<Text>().text == " ")
                {
                    if (moverEspacio) gameObject.transform.GetChild(0).transform.position = posRaton;
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
}
