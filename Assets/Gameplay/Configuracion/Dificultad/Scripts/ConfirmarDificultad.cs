using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System;

public class ConfirmarDificultad : MonoBehaviour
{
    SQLQuery usuarios;
    DatosJugador datosJugador;

    [Header("Referencias")]
    public Button confirmar;

    [Header("Debug")]
    public float[] posiciones;
    public int[] dificultades;

    Marcador[] marcadores;

    void Start()
    {
        usuarios = new SQLQuery("Usuarios");
        datosJugador = FindObjectOfType<DatosJugador>();

        confirmar.onClick.AddListener(LeerConfiguracion);
    }

    void LeerConfiguracion()
    {
        marcadores = FindObjectsOfType<Marcador>();
        int numMarcadores = marcadores.Length;

        IComparer ordenarMarcadores = new OrdenAscendente();

        dificultades = new int[numMarcadores];
        posiciones = new float[numMarcadores];

        for (int i = 0; i < marcadores.Length; i++)
        {
            Marcador marcador = marcadores[i];

            dificultades[i] = int.Parse(marcador.dificultad.text);

            string distanciaText = marcador.distancia.text.Replace("%", "");
            posiciones[i] = float.Parse(distanciaText, CultureInfo.InvariantCulture) / 100;
        }

        Array.Sort(posiciones, dificultades, 0, numMarcadores, ordenarMarcadores);

        EnviarABDD();
    }

    void EnviarABDD()
    {
        string datos = "";
        int idUsuario = datosJugador.id;

        for (int i = 0; i < posiciones.Length; i++)
        {
            datos += posiciones[i].ToString("0.###", CultureInfo.InvariantCulture) + "/" + dificultades[i];
            if (i != posiciones.Length - 1) datos += "/";
        }

        datosJugador.distancias.Clear();
        datosJugador.distancias.AddRange(posiciones);
        //
        datosJugador.dificultades.Clear();
        datosJugador.dificultades.AddRange(dificultades);
        //
        datosJugador.regenerarRondas = true;

        usuarios.Query("UPDATE Usuarios SET Dificultad = '" + datos + "', DificultadCambiada = 1 WHERE ID_Usuario = " + idUsuario);
    }

    class OrdenAscendente : IComparer
    {
        int IComparer.Compare(object a, object b)
        {
            float f1 = (float)a;
            float f2 = (float)b;

            if (f1 > f2)
                return 1; // 1 = Mayor
            if (f1 < f2)
                return -1; // -1 = Menor
            else
                return 0; // 0 = Igual
        }
    }
}
