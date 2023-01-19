////////using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class desplegable : MonoBehaviour
{
    // script editado 29/4
    public GameObject desplegableObject;
    public RectTransform rectTransform;
    public Vector2 Pos;
    public bool iniciar;
    public int offset; //new
    //public Vector2 planeta = new Vector2(0,0);//new
    // float speed = 2f;
    //public float max, min;
    //public TextMeshProUGUI botontext;

    void Update()
    {
        /* if (iniciar == true)//old
         {

             if (Pos.x < max) //old
              {
                  Pos = rectTransform.localPosition;
                  Pos.x += Pos.x * Time.deltaTime * speed;
                  rectTransform.localPosition = Pos;
              }
    
        if (iniciar == true)
        {
            if (Pos.x > min)
            {
                Pos = rectTransform.localPosition;
                Pos.x += Pos.x * Time.deltaTime * -speed;
                rectTransform.localPosition = Pos;
            }
        }*/
    }
   /* public void desplegar()
    {
        Pos = new Vector2(planeta.x + offset, planeta.y);//new
        rectTransform.position = Pos;//new
        //old
        // iniciar = !iniciar;
        /* if (iniciar == false)
         {
             botontext.text = "<";
         }
         if (iniciar == true)
         {
             botontext.text = ">";
         }
    }*/
}
