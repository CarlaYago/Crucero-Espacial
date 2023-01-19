using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoriasScript : MonoBehaviour
{
    public int contadorNivel = 0;
    public bool cambiarTexto;

    public string textoBase;
    public Sprite[] spritesFondo;
    public Sprite[] spritesSimboloMas;
    public Color[] colores; // Cambiar los colores de abajo por colores públicos

    void Start()
    {
        textoBase = gameObject.GetComponent<TextMeshProUGUI>().text;
        cambiarTexto = true;
    }

    void Update()
    {
        if (cambiarTexto)
        {
            cambiarTexto = false;
            if (contadorNivel < 5)
            {
                contadorNivel++;
                if (contadorNivel == 1)
                {
                    //Color colorGris = new Color(0.42f, 0.42f, 0.42f, 1f);
                    CambiarColor(colores[0], spritesFondo[0], spritesSimboloMas[0]);
                }

                if (contadorNivel == 2)
                {
                    //Color colorVerde = new Color(0.56f, 1f, 0f, 1f);
                    CambiarColor(colores[1], spritesFondo[1], spritesSimboloMas[1]);
                }

                if (contadorNivel == 3)
                {
                    //Color colorAzul = new Color(0f, 0.52f, 1f, 1f);
                    CambiarColor(colores[2], spritesFondo[2], spritesSimboloMas[2]);
                }

                if (contadorNivel == 4)
                {
                    //Color colorMorado = new Color(0.57f, 0f, 1f, 1f);
                    CambiarColor(colores[3], spritesFondo[3], spritesSimboloMas[3]);
                }

                if (contadorNivel >= 5)
                {
                    contadorNivel = 5;
                    //Color colorDorado = new Color(1f, 0.78f, 0f, 1f);
                    CambiarColor(colores[4], spritesFondo[4], spritesSimboloMas[4]);
                }
            }
            gameObject.GetComponent<TextMeshProUGUI>().text = textoBase + contadorNivel + ")";
        }
    }

    void CambiarColor(Color nuevoColor, Sprite spriteFondo, Sprite spriteMas)
    {
        GetComponent<TextMeshProUGUI>().color = nuevoColor; // Cambiar el color del nombre de la categoría
        transform.GetChild(0).GetComponent<Image>().sprite = spriteFondo; // Cambiar imagen del fondo
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spriteFondo; // Cambiar imagen del recuadro de recompensa
        transform.GetChild(7).GetComponent<TextMeshProUGUI>().color = nuevoColor; // Cambiar el color del texto de recompensa

        for (int i = 1; i < 6; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = spriteFondo; // Cambiar imagen de los huecos de la categoria

            // Cambiar simbolo + 
            if (transform.GetChild(i).childCount == 1)
            {
                if (transform.GetChild(i).GetChild(0).TryGetComponent(out BotonColeccion boton))
                {
                    //boton.transform.GetComponentInChildren<Text>().color = nuevoColor;
                    boton.transform.GetComponent<Image>().sprite = spriteFondo; // Cambiar el fondo del botón de +
                    boton.transform.GetChild(0).GetComponent<Image>().sprite = spriteMas; // Cambiar el símbolo de +
                }
            }
            else if (transform.GetChild(i).childCount == 2)
            {
                if (transform.GetChild(i).GetChild(1).TryGetComponent(out BotonColeccion boton))
                {
                    //boton.transform.GetComponentInChildren<Text>().color = nuevoColor;
                    boton.transform.GetComponent<Image>().sprite = spriteFondo; // ""
                    boton.transform.GetChild(0).GetComponent<Image>().sprite = spriteMas; // ""
                }
            }
        }
    }
}