using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarIdentificadores
{
    string idPrincipio, idFinal;
    SQLQuery consultas;

    public DetectarIdentificadores(string idPrincipio, string idFinal)
    {
        this.idPrincipio = idPrincipio;
        this.idFinal = idFinal;

        consultas = new SQLQuery("BaseLogopeda");
    }

    public List<string> PalabrasDevueltas(string texto)
    {
        List<string> lista = new List<string> ();
        string[] array = texto.Split(new string[] { idPrincipio }, StringSplitOptions.None);
        

        for (int i = 0; i < array.Length; i++)
        {
            string palabra = array[i];
            
            if(palabra.Contains(idFinal))
            {
                int index = palabra.IndexOf(idFinal);
                int lenght = palabra.Length - index;
                palabra = palabra.Remove(index, lenght);
                lista.Add(palabra);
            }
        }
        return lista;
    }

    public List<string> PalabrasDevueltasSecuencias (List<string> secuencia)
    {
        List<string> lista = new List<string>();

        for (int i = 0; i < secuencia.Count; i++)
        {
            lista.AddRange(PalabrasDevueltas(secuencia[i]));
        }
        return lista;
    }

    public string QuitarIdentificadores(string texto)
    {
        texto = texto.Replace(idPrincipio, "");
        texto = texto.Replace(idFinal, "");
        return texto;
    }

    public List<string> QuitarIdentificadoresSecuencias(List<string> secuencia)
    {
        for (int i = 0; i < secuencia.Count; i++)
        {
            secuencia[i] = secuencia[i].Replace(idPrincipio, "");
            secuencia[i] = secuencia[i].Replace(idFinal, "");
        }  
        return secuencia;
    }

    public string AsignarIdentificadores(string texto)
    {
        string salida = "";

        string[] palabras = texto.Split(' ');

        for (int i = 0; i < palabras.Length; i++)
        {
            consultas.Query("SELECT Palabra FROM Palabras WHERE Palabra = '" + palabras[i] + "'");

            if (i > 0) salida += " ";

            if (consultas.Count() > 0)
            {
                string palabraIdentificada = "<b>" + palabras[i] + "</b>";
                salida += palabraIdentificada;
            }
            else
            {
                salida += palabras[i];
            }
        }

        return salida;
    }
}