using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnagramaTest : MonoBehaviour
{
    readonly char[] abecedario = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Ñ', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public string palabra, anagrama;
    public bool refrescar, debug;

    void Start()
    {
        anagrama = palabra;
        ReorganizarLetras();
    }

    private void Update()
    {
        if (refrescar)
        {
            anagrama = palabra;
            ReorganizarLetras();
            refrescar = false;
        }
    }

    void ReorganizarLetras()
    {
        // Recorrer las letras
        for (int numLetra = 0; numLetra < anagrama.Length; numLetra++)
        {
            char letra = anagrama[numLetra];
            int indiceActual = numLetra;

            // Comprarar cada letra con sus anteriores (para ordenarlas)
            for (int i = 1; i <= numLetra; i++)
            {
                int ultimoIndice = numLetra - i;
                char letraAnterior = anagrama[ultimoIndice];

                if (debug) Debug.Log(numLetra + ": ¿Es " + letra + " menor que " + letraAnterior + "?");

                if (Indice(letra) < Indice(letraAnterior))
                {
                    anagrama = anagrama.Remove(indiceActual, 1).Insert(indiceActual, letraAnterior.ToString());
                    anagrama = anagrama.Remove(ultimoIndice, 1).Insert(ultimoIndice, letra.ToString());
                    indiceActual--;
                }
                else break;
            }
        }
    }

    int Indice(char letra)
    {
        return Array.IndexOf(abecedario, letra);
    }
}
