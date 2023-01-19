// Este script reorganiza todas las letras de las palabras fáciles en LexicoEsp en órden alfabético, para facilitar la busqueda de anagramas con los que poder formar el número mínimo requerido de
// palabras para el minijuego 4 (sacar X palabras de Y letras). Los anagramas se incluyen en la columna Anagrama.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnagramasLex : MonoBehaviour
{
    SQLQuery consulta;
    readonly char[] abecedario = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Ñ', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public List<string> palabrasFaciles, anagramas;
    public bool actualizarBDD, actualizarNoNulos;

    void Start()
    {
        consulta = new SQLQuery("LexicoEsp");
        consulta.Query("SELECT Palabra FROM LexicoEsp WHERE Facil = 1");

        ReorganizarLetras();
        if (actualizarBDD) IntroducirABDD();
    }

    void ReorganizarLetras()
    {
        palabrasFaciles = consulta.StringReader(1);

        // Recorrer todas las palabras
        for (int numPalabra = 0; numPalabra < palabrasFaciles.Count; numPalabra++)
        {
            string palabra = palabrasFaciles[numPalabra];

            // Recorrer las letras
            for (int numLetra = 0; numLetra < palabra.Length; numLetra++)
            {
                char letra = palabra[numLetra];
                int indiceActual = numLetra;

                // Comprarar cada letra con sus anteriores (para ordenarlas)
                for (int i = 1; i <= numLetra; i++)
                {
                    int ultimoIndice = numLetra - i;
                    char letraAnterior = palabra[ultimoIndice];

                    if (Indice(letra) < Indice(letraAnterior))
                    {
                        palabra = palabra.Remove(indiceActual, 1).Insert(indiceActual, letraAnterior.ToString());
                        palabra = palabra.Remove(ultimoIndice, 1).Insert(ultimoIndice, letra.ToString());
                        indiceActual--;
                    }
                    else break;
                }
            }

            anagramas.Add(palabra);
        }
    }

    int Indice(char letra)
    {
        return Array.IndexOf(abecedario, letra);
    }

    void IntroducirABDD()
    {
        for (int i = 0; i < anagramas.Count; i++)
        {
            consulta.Query("SELECT Anagrama FROM LexicoEsp WHERE Palabra = '" + palabrasFaciles[i] + "' AND Anagrama IS NOT NULL");

            if (consulta.Count() == 0)
            {
                consulta.Query("UPDATE LexicoEsp SET Anagrama = '" + anagramas[i] + "' WHERE Palabra = '" + palabrasFaciles[i] + "'");
            }
            else if (actualizarNoNulos)
            {
                consulta.Query("UPDATE LexicoEsp SET Anagrama = '" + anagramas[i] + "' WHERE Palabra = '" + palabrasFaciles[i] + "'");
            }
        }
    }
}