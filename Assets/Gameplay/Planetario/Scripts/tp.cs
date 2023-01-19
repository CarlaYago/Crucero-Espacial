using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class tp : MonoBehaviour, IPointerDownHandler
{
    public Camera cam;
    public GameObject mapa;
    Image thisObject;
    public GameObject desplegable;
    public Vector2 offset;
    void Start()
    {
        thisObject = gameObject.GetComponent<Image>();
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        desplegable.SetActive(false);
        float factorEscala = mapa.GetComponent<SpriteRenderer>().size.x * mapa.transform.localScale.x / thisObject.rectTransform.rect.width;
        Vector2 centroPequeno = new Vector2(transform.position.x, transform.position.y);
        
        Vector2 centroGrande = new Vector2(mapa.transform.position.x, mapa.transform.position.y);
        Vector2 distanciaPequena = pointerEventData.position - centroPequeno ;
        Vector2 distanciaLarga = distanciaPequena * factorEscala;
        Vector2 posFinal = distanciaLarga + centroGrande;
        float posx = posFinal.x + offset.x;
        float posy = posFinal.y + offset.y;
        float posz = cam.transform.position.z;
        cam.transform.position = new Vector3(posx, posy, posz);
    }

}