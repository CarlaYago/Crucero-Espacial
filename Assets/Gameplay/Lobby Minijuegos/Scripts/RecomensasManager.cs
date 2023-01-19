using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RecomensasManager : MonoBehaviour
{
    [Header("Barra Recompensa Adicional")]

    [Range(0, 100)] public float porcentajeRecompensaAdicional;
    public Slider barraRecompensaAdicional;

    [Header("Minijuegos")]

    [Range(0, 1)] public float porcentajeBarra;
    public GameObject Gasolina;
    public GameObject botonIzq;
    public GameObject botonCen;
    public GameObject botonDer;
    public Sprite gasGris;
    public Sprite gasVerde;
    public Sprite gasRojo;

    [Header("Menú Información")]

    public GameObject inf;

    [Header("Menú Recompensa")]

    public GameObject puntGasolina;
    public GameObject textPuntGasolina;

    [Header("Menú Recompensa Adicional")]

    public GameObject menuRecompensaAdicional;

    [Header("Menú Información Recompensa Adicional")]

    public GameObject infoRecompensaAdicional;

    [Header("Recompensa Adicional")]

    public string palabraRecompensaAdicional;
    public Text contrasenya;
    public InputField inputF;

    string palabraRAOculta;
    int letras;
    float porcentaje;
    List<int> indexLetras = new List<int>();
    int numeroLetras;
    string ordenDeAparacion;
    bool cambioGasolina = false;

    public int pGasolina;





    private void Start()
    {
        for (int i = 0; i < palabraRecompensaAdicional.Length; i++)
        {
            indexLetras.Add(i);
            palabraRAOculta += "#";
        }

        for (int i = 0; i < palabraRecompensaAdicional.Length; i++)
        {
            int rand = indexLetras[UnityEngine.Random.Range(0, indexLetras.Count)];
            ordenDeAparacion += rand;
            indexLetras.Remove(rand);
        }

        letras = palabraRAOculta.Length;
        porcentaje = porcentajeRecompensaAdicional / 100;
        numeroLetras = (int)(porcentaje * letras);
        recompensaAdicional(numeroLetras);
    }

    void Update()
    {
        barraRecompensaAdicional.GetComponent<Slider>().value = porcentajeBarra;

        if (botonIzq.GetComponent<SeleccionMinijuegos>().porcentaje > 49 && botonCen.GetComponent<SeleccionMinijuegos>().porcentaje > 49 && botonDer.GetComponent<SeleccionMinijuegos>().porcentaje > 49)
        {
            Gasolina.GetComponent<Image>().sprite = gasVerde;
            cambioGasolina = true;
        }

        if (botonIzq.GetComponent<SeleccionMinijuegos>().porcentaje < 50 || botonCen.GetComponent<SeleccionMinijuegos>().porcentaje < 50 || botonDer.GetComponent<SeleccionMinijuegos>().porcentaje < 50)
        {
            Gasolina.GetComponent<Image>().sprite = gasRojo;
        }

        if (botonIzq.GetComponent<SeleccionMinijuegos>().porcentaje == 0 && botonCen.GetComponent<SeleccionMinijuegos>().porcentaje == 0 && botonDer.GetComponent<SeleccionMinijuegos>().porcentaje == 0)
        {
            Gasolina.GetComponent<Image>().sprite = gasGris;
        }

    }

    public void Salir()
    {

    }

    public void Info()
    {
        inf.SetActive(true);
    }

    public void cerrarInfo()
    {
        inf.SetActive(false);
    }

    public void Confirmar()
    {

    }

    public void gasolinaRecompensa()
    {
        if (cambioGasolina == true)
        {
            puntGasolina.SetActive(true);
            textPuntGasolina.GetComponent<Text>().text = "+" + pGasolina;
        }
    }

    public void abirMenuRecompensaAdicional()
    {
        menuRecompensaAdicional.SetActive(true);
    }

    public void cerrarMenuRecompensaAdicional()
    {
        menuRecompensaAdicional.SetActive(false);
    }

    public void infRecompensaAdicional()
    {
        infoRecompensaAdicional.SetActive(true);
    }

    public void cerrarInfoMenuAdicional()
    {
        infoRecompensaAdicional.SetActive(false);
    }

    public void recompensaAdicional(float referencia)
    {
        for (int i = 0; i < referencia; i++)
        {
            int pos = (int)char.GetNumericValue(ordenDeAparacion[i]);
            palabraRAOculta = palabraRAOculta.Remove(pos, 1).Insert(pos, palabraRecompensaAdicional[pos].ToString());
            contrasenya.GetComponent<Text>().text = (palabraRAOculta);
        }
    }

    public void ReadStringInput(string Input)
    {
        if (Input != "")
        {
            if (Input == palabraRecompensaAdicional)
            {
                Debug.Log("Correcto");
                inputF.text = null;
            }
        }
    }

    public void confirmarInputField()
    {
        ReadStringInput(inputF.text);
    }
}
