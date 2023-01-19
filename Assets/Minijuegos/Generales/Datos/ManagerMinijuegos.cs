using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMinijuegos
{
    ProgresoMinijuegos progresoManager;
    Minijuego17 MJ17;
    InterfazGeneralManager interfazGeneral;
    int archivosTotales, archivosPorRonda, archivosCorrectosPorRonda;

    bool finMinijuego;
    public InterfazPanelManager interfazRecompensas; //no le quiteis el public primero preguntar a Vicente o Carla exdi 
    //carla no recuerda que hacia esto así que podeis tocarlo :D
    List<string> palabrasRecompensadas = new List<string>();

    Action funcionMinijuego;

    int rondaActual = 1;
    int rondas, tiempo;

    public ManagerMinijuegos(int rondas, int archivosPorRonda, Action funcionMinijuego)
    {
         interfazGeneral = UnityEngine.Object.FindObjectOfType<InterfazGeneralManager>();
        //interfazRecompensas = UnityEngine.Object.FindObjectOfType<InterfazPanelManager>();
        //interfazRecompensas.gameObject.SetActive(false);

        this.rondas = rondas;
        this.archivosPorRonda = archivosPorRonda;
        this.funcionMinijuego = funcionMinijuego;

        archivosTotales = rondas * archivosPorRonda;

        IniciarInterfaz();
    }

    public void InterfazRecompensas(InterfazPanelManager interfaz)
    {
        interfazRecompensas = interfaz;
    }

    void IniciarInterfaz()
    {
        // Si el número de rondas es mayor a 1, mostrar la información de rondas
        if (rondas > 1)
        {
            interfazGeneral.progresoRondas = rondaActual;
            interfazGeneral.maxRondas = rondas;
        }
        else
        {
            interfazGeneral.textoRondas.SetActive(false);
        }

        // Mostrar el número de archivos 
        interfazGeneral.progresoJugador = 0;
        interfazGeneral.maxProgreso = archivosTotales;
    }

    public void SiguienteRonda()
    {
        if (rondaActual < rondas)
        {
            rondaActual++;
            interfazGeneral.progresoRondas = rondaActual;
            archivosCorrectosPorRonda = 0;

            funcionMinijuego?.Invoke(); // Llama a la función si no es null
            interfazGeneral.GameTime = tiempo;
        }
        else
        {
            FinMinijuego();
        }
    }

    void FinMinijuego()
    {
        
        finMinijuego = true;
        float porcentajeTotal = interfazGeneral.progresoJugador / archivosTotales;
        interfazGeneral.panel.SetActive(true);
        interfazRecompensas.nadaOExtra = MJ17.longitudPalabrasRespondidas;
        interfazRecompensas.num = porcentajeTotal;
        interfazRecompensas.numPalabras = palabrasRecompensadas.Count;
        interfazRecompensas.exManager = new ExperienciaManager(porcentajeTotal);
        interfazRecompensas.numEXP = interfazRecompensas.exManager.SumaExp(MJ17.longitudPalabrasRespondidas);

        interfazRecompensas.AsignarPorcentaje();

        interfazRecompensas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Tiempo hasta la ronda siguiente o el final del juego. Debe ir en el Update().
    /// </summary>
    /// <param name="tiempo"> La duración de una ronda, en segundos. </param>
    public void Temporizador(int tiempo)
    {
        if (!finMinijuego)
        {
            if (interfazGeneral.empezarTemporizador == false)
            {
                if (this.tiempo == 0) this.tiempo = tiempo;
                interfazGeneral.GameTime = tiempo;
                interfazGeneral.empezarTemporizador = true;
            }
            else
            {
                if (interfazGeneral.GameTime <= 0)
                {
                    SiguienteRonda();
                    interfazGeneral.empezarTemporizador = false;
                }
            }
        }
        else if (interfazGeneral.empezarTemporizador)
        {
            interfazGeneral.empezarTemporizador = false;
        }
    }

    public void EnviarArchivos(List<string> palabrasRecompensadas, int archivosCorrectos = 0, bool fallo = false)
    {
        MJ17 = UnityEngine.Object.FindObjectOfType<Minijuego17>();
        if (archivosCorrectos > 0)
        {
            archivosCorrectosPorRonda += archivosCorrectos;
            interfazGeneral.progresoJugador += archivosCorrectos;

            interfazGeneral.barraProgreso.value = interfazGeneral.progresoJugador / archivosTotales;

            if (archivosCorrectosPorRonda == archivosPorRonda)
            {
                SiguienteRonda();
            }
        }

        if (fallo)
        {
            interfazGeneral.error = true;
        }

        this.palabrasRecompensadas = palabrasRecompensadas;
    }
}