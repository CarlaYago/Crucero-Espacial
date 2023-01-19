using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minijuego4vs2 : MonoBehaviour
{/*
    List<string> Palabras = new List<string>() { "COCHE", "GATO", "MANZANA", "ZAPATERIA", "ARBOL", "PLANTA", "CARAMELO", "GUAPO", "BOMBA" };

    string PalabraSeleccionada;
    
    public List<string> PalabrasOrdenadas = new List<string>();
    public List<string> PalabrasSeleccionadas = new List<string>();
    public List<int> LongitudPalabra = new List<int>();
    public List<Transform> EspaciosLista;

    public GameObject Espacio;
    public GameObject Letra;
    public Transform Espacios, Espacios2, Espacios3, Letras;

    bool FinMinijuego = true;
    public int PalabrasParaJugar;
    public int PalabrasPorRonda = 3;

    void Start()
    {
        //Espacios = GameObject.FindGameObjectWithTag("Espacios").transform; -> Si solo usáis los tags Espacios para esto se pueden quitar (ya son variables públicas)
        //Espacios2 = GameObject.FindGameObjectWithTag("Espacios2").transform;
        //Espacios3 = GameObject.FindGameObjectWithTag("Espacios3").transform;
    }

    void Update()
    {
        if (FinMinijuego == true && PalabrasParaJugar > 0)
        {
            FinMinijuego = false;
            InicioJuego();
        }
    }

    void InicioJuego()
    {

        SeleccionPalabra();
        DesglosePalabra();
        SpawnLetras();
    }

    void SeleccionPalabra()
    {
        for (int i = 0; i < PalabrasPorRonda; i++)
        {       
            PalabraSeleccionada = Palabras[Random.Range(0, Palabras.Count)];
            PalabrasSeleccionadas.Add(PalabraSeleccionada);
            LongitudPalabra.Add(PalabrasSeleccionadas[i].Length);
            //Debug.Log("Longitud de la palabra " + LongitudPalabra[i]);
            Palabras.Remove(PalabraSeleccionada);
            Debug.Log(PalabrasSeleccionadas[i]);
        }
            
    }

    void DesglosePalabra()
    {
        for (int i = 0; i < LongitudPalabra[0]; i++)
        {
            PalabrasOrdenadas.Add(PalabrasSeleccionadas[0].Substring(i, 1));
            Debug.Log(PalabrasOrdenadas[i]);
        }
        for (int i = 0; i < LongitudPalabra[1]; i++)
        {
            PalabrasOrdenadas.Add(PalabrasSeleccionadas[1].Substring(i, 1));
            Debug.Log(PalabrasOrdenadas[i]);
        }
        for (int i = 0; i < LongitudPalabra[2]; i++)
        {
            PalabrasOrdenadas.Add(PalabrasSeleccionadas[2].Substring(i, 1));
            Debug.Log(PalabrasOrdenadas[i]);
        }
    }

    void SpawnLetras()
    {
        for (int i = 0; i < LongitudPalabra[0]; i++)
        {
            GameObject letraNueva = Instantiate(Letra, Letras);
            string LetraEliminada = PalabrasOrdenadas[Random.Range(0, PalabrasOrdenadas.Count)];
            letraNueva.GetComponent<Text>().text = LetraEliminada;
            PalabrasOrdenadas.Remove(LetraEliminada);

            GameObject espacioBlanco = Instantiate(Espacio, Espacios);
            EspaciosLista.Add(espacioBlanco.transform);
        }
        
        for (int i = 0; i < LongitudPalabra[1]; i++)
        {
            GameObject letraNueva = Instantiate(Letra, Letras);
            string LetraEliminada = PalabrasOrdenadas[Random.Range(0, PalabrasOrdenadas.Count)];
            letraNueva.GetComponent<Text>().text = LetraEliminada;
            PalabrasOrdenadas.Remove(LetraEliminada);

            GameObject espacioBlanco = Instantiate(Espacio, Espacios2);
            EspaciosLista.Add(espacioBlanco.transform);
        }

        for (int i = 0; i < LongitudPalabra[2]; i++)
        {
            GameObject letraNueva = Instantiate(Letra, Letras);
            string LetraEliminada = PalabrasOrdenadas[Random.Range(0, PalabrasOrdenadas.Count)];
            letraNueva.GetComponent<Text>().text = LetraEliminada;
            PalabrasOrdenadas.Remove(LetraEliminada);

            GameObject espacioBlanco = Instantiate(Espacio, Espacios3);
            EspaciosLista.Add(espacioBlanco.transform);
        }
    }

    public void ComprobacionLetras()
    {
        string Palabra1 = "";
        string Palabra2 = "";
        string Palabra3 = "";

        for (int i = 0; i < Espacios.childCount; i++)
        {
            string LetraEspacio = Espacios.GetChild(i).GetChild(0).GetComponent<Text>().text;
            Palabra1 += LetraEspacio;
        }
        for (int i = 0; i < Espacios2.childCount; i++)
        {
            string LetraEspacio2 = Espacios2.GetChild(i).GetChild(0).GetComponent<Text>().text;
            Palabra2 += LetraEspacio2;
        }
        for (int i = 0; i < Espacios3.childCount; i++)
        {
            string LetraEspacio3 = Espacios3.GetChild(i).GetChild(0).GetComponent<Text>().text;
            Palabra3 += LetraEspacio3;
        }

        if (PalabrasSeleccionadas[0] == Palabra1 && PalabrasSeleccionadas[1] == Palabra2 && PalabrasSeleccionadas[2] == Palabra3)
        {
            Debug.Log("BIEEEEEN :D");
            PalabrasParaJugar--;
            ResetMinijuego();
            PalabrasSeleccionadas.Clear();
            PalabrasOrdenadas.Clear();
            LongitudPalabra.Clear();
            FinMinijuego = true;
        }
        else
        {
            Debug.Log("MAAAAAL D:");
        }
    }

    void ResetMinijuego()
    {     
        for (int i = 0; i < LongitudPalabra[0]; i++)
        {
            Destroy(Espacios.GetChild(i).gameObject);
        }
        for (int i = 0; i < LongitudPalabra[1]; i++)
        {
            Destroy(Espacios2.GetChild(i).gameObject);
        }
        for (int i = 0; i < LongitudPalabra[2]; i++)
        {
            Destroy(Espacios3.GetChild(i).gameObject);
        }
    }
    */
}