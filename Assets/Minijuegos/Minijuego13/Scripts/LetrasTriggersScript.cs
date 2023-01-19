using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LetrasTriggersScript : MonoBehaviour, IPointerClickHandler
{
    [Header("GameObjects")]
    public GameObject espacioPrefab;
    public GameObject iconoCursor;

    [Header("Transforms")]
    Transform parent;

    [Header("Ints")]
    int posLetra;

    [Header("Bools")]
    bool espacioActivo;

    [Header("Interfaz")]
    Canvas CanvasMinijuego13;
    Collider2D col;

    public void OnPointerClick(PointerEventData eventData)
    {
        posLetra = transform.GetSiblingIndex();
        if (eventData.position.x < transform.position.x)
        {
            if (posLetra > 0)
            {
                if (parent.GetChild(posLetra - 1).GetComponent<Text>().text != " ")
                {
                    GameObject espacioInstanciado = Instantiate(espacioPrefab, parent);
                    espacioInstanciado.transform.SetSiblingIndex(posLetra);
                }
            }
        }
        else if (eventData.position.x > transform.position.x)
        {
            if (posLetra < parent.childCount - 1)
            {
                if (parent.GetChild(posLetra + 1).GetComponent<Text>().text != " ")
                {
                    GameObject espacioInstanciado = Instantiate(espacioPrefab, parent);
                    espacioInstanciado.transform.SetSiblingIndex(posLetra + 1);
                }
            }
        }
    }

    void Start()
    {
        CanvasMinijuego13 = FindObjectOfType<Canvas>();

        parent = transform.parent;
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector2 posRaton = Input.mousePosition;
        Vector2 pos = transform.position;
        float anchura = col.bounds.extents.x;
        float altura = col.bounds.extents.y;


        if (posRaton.x <= pos.x + anchura && posRaton.x > pos.x - anchura)
        {
            if (posRaton.y <= pos.y + altura && posRaton.y > pos.y - altura)
            {
                if (posRaton.x < pos.x && gameObject.GetComponent<Text>().text != " ")
                {
                    gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);      
                }
                if (posRaton.x > pos.x && gameObject.GetComponent<Text>().text != " ")
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    gameObject.transform.GetChild(1).gameObject.SetActive(true);      
                }
            }            
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }        
    }
}