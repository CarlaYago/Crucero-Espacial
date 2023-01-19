using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgresoMinijuegos
{
    DatosJugador datos;
    readonly int index;

    public ProgresoMinijuegos()
    {
        datos = Object.FindObjectOfType<DatosJugador>();
        index = datos.minijuegoActualIndex;
    }

    /// <summary>
    /// Devuelve los valores de dificultad y rondas mediante un array de 2 elementos.
    /// </summary>
    /// <returns></returns>
    public int[] DificultadRondas()
    {
        int[] difRondas = new int[2];
        difRondas[0] = datos.minijuegoDificultad;
        difRondas[1] = datos.minijuegosRondas[index];

        return difRondas;
    }

    /// <summary>
    /// Automáticamente asigna los valores de dificultad y rondas a las variables que se pasen como referencia.
    /// </summary>
    public void DificultadRondasAuto(ref int dificultad, ref int rondas)
    {
        dificultad = datos.minijuegoDificultad;
        rondas = datos.minijuegosRondas[index];
    }
    
    public void DatosFinales(float porcentaje)
    {
        datos.minijuegosPorcentajes[index] = porcentaje;
        datos.minijuegoRecienCompletado[index] = true;
    }
}