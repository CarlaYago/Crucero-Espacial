using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionMinijuegos : MonoBehaviour
{
    [Header("Porcentaje")]

    [Range(0,100)] public int porcentaje;
    public GameObject texto;

    [Header("Imágenes")]

    public GameObject candado;
    public Sprite gris;
    public Sprite rojo;
    public Sprite verde;
    public Sprite candAbierto;
    public Sprite candCerrado;

    void Update()
    {
        if (porcentaje > 0 && porcentaje <= 49)
        {
            gameObject.GetComponent<Image>().sprite = rojo;
            texto.GetComponent<Text>().text = porcentaje + "%";
            texto.GetComponent<Text>().color = new Color(0.66f, 0f, 0f, 1f);
            candado.GetComponent<Image>().sprite = candCerrado;

        }
        if (porcentaje >= 50 && porcentaje <= 101)
        {
            gameObject.GetComponent<Image>().sprite = verde;
            texto.GetComponent<Text>().text = porcentaje + "%";
            texto.GetComponent<Text>().color = new Color(0f, 0.65f, 0.1f, 1f);
            candado.GetComponent<Image>().sprite = candAbierto;
        }
        if (porcentaje == 0)
        {
            gameObject.GetComponent<Image>().sprite = gris;
            texto.GetComponent<Text>().text = "";
            texto.GetComponent<Text>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            candado.GetComponent<Image>().sprite = candCerrado;
        }
    }
}
