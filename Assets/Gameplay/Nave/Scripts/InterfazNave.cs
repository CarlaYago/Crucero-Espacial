using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfazNave : MonoBehaviour
{
    public TextMeshProUGUI nombre, experiencia, proximoRango,ExpText;
    DatosJugador datos;

    public Button[] botonesAlmacen, botonesColeccion;
    public Button botonSalir;
    public Image barraXP;

    int numNivel; // Nivel del jugador
    float valor, puntuacionBarra;//Valor máximo de la barra de experiencia
    
    public List<string> rangosText = new List<string>();
    public List<Sprite> rangosImgL = new List<Sprite>();
    public List<Sprite> rangosImgNoL = new List<Sprite>();
    public List<int> proximoNivel = new List<int>();
    public List<Image> logos = new List<Image>();
    void Start()
    {
        
        datos = FindObjectOfType<DatosJugador>();
        CalcularNivel();
        CalcularRango();
        experiencia.text += numNivel;
        LeerBDD();
        botonSalir.onClick.AddListener(VaciarDatos);
    }

    void LeerBDD()
    {
        if (datos.palabraID_BDD.Count == 0) // para que esto funcione al cambiar de paciente hay que hacer Clear() de todos los datos al salir de partida
        {
            datos.AccederInfoPlanetas();
            datos.AccederInfoAlmacen();
        }

        foreach(Button almacen in botonesAlmacen) almacen.onClick.AddListener(datos.AccederInfoAlmacen); // Actualizar información almacén (desde BDD)
        foreach (Button coleccion in botonesColeccion) coleccion.onClick.AddListener(datos.AccederInfoColeccion); // Actualizar información colección (desde BDD)
    }

    void VaciarDatos()
    {
        datos.palabraID_BDD.Clear();

        datos.plaFavorito_BDD.Clear();
        datos.plaUltimaVisita_BDD.Clear();
    }
    void CalcularNivel()
    {
        int suma12 = 12;
        int inicio = 3600;
        if (datos.experiencia < 1200)
        {
            valor = 600;
            numNivel = ((int)((datos.experiencia) / 600)) + 1;
            puntuacionBarra = datos.experiencia % 600;
        }
        else if (datos.experiencia >= 1200 && datos.experiencia < 3600)
        {
            valor = 1200;
            numNivel = ((int)((datos.experiencia - 1200) / 1200)) + 3;
            puntuacionBarra = (datos.experiencia - 1200) % 1200;
        }
        else if (datos.experiencia >= 3600 && datos.experiencia < 8400)
        {
            valor = 2400;
            numNivel = ((int)((datos.experiencia - 3600) / 2400)) + 5;
            puntuacionBarra = (datos.experiencia - 3600) % 2400;
        }
        else if (datos.experiencia >= 8400 && datos.experiencia < 350400)
        {
            valor = 3600;
            numNivel = ((int)((datos.experiencia - 8400) / 3600)) + 7;
            puntuacionBarra = (datos.experiencia - 8400) % 3600;
        }
        else if (datos.experiencia >= 350400 && datos.experiencia < 1311600)
        {
            float resta = datos.experiencia - 350400;
            int registroLv = 0;
            while (resta > inicio + suma12)
            {
                valor = 3600 + suma12;
                resta -= (inicio + suma12);
                suma12 += 12;
                registroLv++;
            }
            numNivel = registroLv + 102;
            puntuacionBarra = resta;
        }
        else
        {
            valor = 6000;
            numNivel = ((int)((datos.experiencia - 1311600) / 4000)) + 302;
            puntuacionBarra = (datos.experiencia - 1311600) % 4000;
        }
        barraXP.fillAmount = puntuacionBarra / valor;
        ExpText.text = puntuacionBarra.ToString() + ExpText.text;
    }
    void CalcularRango()
    {
        int numRango = 0;
        if (numNivel < 6)
        {
            numRango = (int)((float)numNivel / 3f);
        }
        else if (numNivel >= 6 && numNivel < 10)
        {
            numRango = 2;
        }
        else if (numNivel >= 10 && numNivel < 20)
        {
            numRango = ((int)(((float)numNivel  - 10f) / 5f)) + 3;
        }
        else if (numNivel >= 20 && numNivel < 100)
        {
            numRango = ((int)(((float)numNivel - 20f) / 10f)) + 5;
        }
        else if (numNivel >= 100 && numNivel < 120)
        {
            numRango = 13;
        }
        else if (numNivel >= 120 && numNivel < 150)
        {
            numRango = 14;
        }
        else if (numNivel >= 150 && numNivel < 200)
        {
            numRango = 15;
        }
        else if (numNivel >= 200 && numNivel < 260)
        {
            numRango = ((int)(((float)numNivel - 200f) / 30f)) + 16;
        }
        else if (numNivel >= 260 && numNivel < 300)
        {
            numRango = 18;
        }
        else
        {
            numRango = 30;
        }
        logos[0].sprite = rangosImgNoL[numRango];
        logos[1].sprite = rangosImgL[numRango];
        logos[2].sprite = rangosImgL[numRango];
        nombre.text = rangosText[numRango] + ": " + datos.nombre;
        if(numNivel < 300)
        {
            proximoRango.text += proximoNivel[numRango];
        }
        else
        {
            proximoRango.text = "ENHORABUENA YA POSEE EL MÁXIMO RANGO";
        }

    }
}