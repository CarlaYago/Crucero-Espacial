using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CambioInterfaz : MonoBehaviour
{
    public Text titulo;
    public Button panelNuevo, panelPrevio;
    public GameObject[] paneles;

    int panelActivo;
    [HideInInspector] public string identificadorActivo;

    void Start()
    {
        panelNuevo.onClick.AddListener(BotonDerecha);
        panelPrevio.onClick.AddListener(BotonIzquierda);

        identificadorActivo = ((Identificadores)panelActivo).ToString();

        for (int i = 0; i < paneles.Length; i++)
        {
            if (i != panelActivo) paneles[i].SetActive(false);
            else paneles[i].SetActive(true);
        }
    }

    void BotonDerecha()
    {
        paneles[panelActivo].SetActive(false);

        if (panelActivo < paneles.Length - 1)
        {
            panelActivo++;
        }
        else
        {
            panelActivo = 0;
        }

        paneles[panelActivo].SetActive(true);

        identificadorActivo = ((Identificadores)panelActivo).ToString();
        titulo.text = identificadorActivo + "s";
    }

    void BotonIzquierda()
    {
        paneles[panelActivo].SetActive(false);

        if (panelActivo > 0)
        {
            panelActivo--;
        }
        else
        {
            panelActivo = paneles.Length - 1;
        }

        paneles[panelActivo].SetActive(true);

        identificadorActivo = ((Identificadores)panelActivo).ToString();
        titulo.text = identificadorActivo + "s";
    }
}