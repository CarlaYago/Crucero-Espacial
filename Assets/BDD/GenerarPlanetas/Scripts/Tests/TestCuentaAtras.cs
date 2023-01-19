using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestCuentaAtras : MonoBehaviour
{
    public TextMeshProUGUI contadorUI;
    TimeSpan contador;

    [Header("Tiempo de espera")]
    public int dias;
    public int horas, minutos, segundos;

    DateTime tiempoInicial; // Valor de BDD
    DateTime tiempoFinal; // Valor de referencia

    bool acabado;

    void Start()
    {
        tiempoInicial = DateTime.Now;
        tiempoFinal = new DateTime(tiempoInicial.Year, tiempoInicial.Month, tiempoInicial.Day + dias, tiempoInicial.Hour + horas, tiempoInicial.Minute + minutos, tiempoInicial.Second + segundos);
    }

    private void Update()
    {
        if (DateTime.Now < tiempoFinal)
        {
            contador = tiempoFinal.Subtract(DateTime.Now);
            contadorUI.text = contador.ToString(@"d\:hh\:mm\:ss").TrimStart('0', ':');
        }
        else if (!acabado)
        {
            contadorUI.text = "Fin";
            acabado = true;
        }
    }
}