using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minjuego4vs0 : MonoBehaviour
{
    List<string> Palabras = new List<string>() { "COCHE", "GATO", "MANZANA", "ZAPATERIA", "ARBOL" };

    string PalabraSeleccionada;
    List <int> LongitudPalabra = new List<int>();

    public List<List <string>> PalabraOrdenada = new List<List<string>>();
    public List<string> PalabrasSeleccionadas = new List<string>();
    public List<Transform> EspaciosLista;

    public GameObject Espacio;
    public GameObject Letra;
    public Transform Espacios;
    public Transform Letras;

    int contadorLetras;
    bool DoOnce;
    bool FinMinijuego = true;
    public int PalabrasParaJugar;
    public int PalabrasPorRonda = 3;

    // Start is called before the first frame update
    void Start()
    {
        Espacios = GameObject.FindGameObjectWithTag("Espacios").transform;
        Letras = GameObject.FindGameObjectWithTag("Letras").transform;

        InicioJuego();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (FinMinijuego == true && PalabrasParaJugar > 0)
        {
            FinMinijuego = false;
            InicioJuego();
        }

        for (int i = 0; i < Espacios.childCount; i++)
        {
            if (Espacios.GetChild(i).childCount > 0)
            {
                contadorLetras++;
            }

            if (i == Espacios.childCount - 1)
            {
                if (contadorLetras == LongitudPalabra)
                {
                    if (DoOnce == false)
                    {
                        ComprobacionLetras();
                        DoOnce = true;
                    }
                }
                else
                {
                    DoOnce = false;
                }

                contadorLetras = 0;
            }
        }*/
    }

    void InicioJuego()
    {
        //ResetMinijuego();
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
            Debug.Log("Longitud de la palabra " + LongitudPalabra[i]);
            Palabras.Remove(PalabraSeleccionada);
            Debug.Log(PalabrasSeleccionadas[i]);
        }      
    }

    void DesglosePalabra()
    {
        for (int i = 0; i < LongitudPalabra.Count; i++)
        {
            for (int j = 0; j < LongitudPalabra[i]; j++)
            {
                PalabraOrdenada[i].Add(PalabrasSeleccionadas[i].Substring(j, 1));
                Debug.Log(PalabraOrdenada[i]);
            }
        }
        
    }

    void SpawnLetras()
    {
        for (int i = 0; i < LongitudPalabra.Count; i++)
        {
            for (int j = 0; j < LongitudPalabra[i]; j++)
            {
                GameObject letraNueva = Instantiate(Letra, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity);
                string LetraEliminada = PalabraOrdenada[j][Random.Range(0, PalabraOrdenada.Count)];
                letraNueva.GetComponent<Text>().text = LetraEliminada;
                PalabraOrdenada[j].Remove(LetraEliminada);
                letraNueva.transform.parent = Letras;

                GameObject espacioBlanco = Instantiate(Espacio, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity);
                espacioBlanco.transform.parent = Espacios;
                EspaciosLista.Add(espacioBlanco.transform);
            }
        }
            
    }

    public void ComprobacionLetras()
    {
        string NuevaPalabra = "";

        for (int i = 0; i < Espacios.childCount; i++)
        {
            string LetraEspacio = Espacios.GetChild(i).GetChild(0).GetComponent<Text>().text;
            NuevaPalabra += LetraEspacio;
        }

        if (PalabraSeleccionada == NuevaPalabra)
        {
            Debug.Log("BIEEEEEN :D");
            FinMinijuego = true;
            PalabrasParaJugar--;
        }
        else
        {
            Debug.Log("MAAAAAL D:");
        }
    }

    void ResetMinijuego()
    {
        for (int i = 0; i < LongitudPalabra.Count; i++)
        {
            for (int j = 0; j < LongitudPalabra[i]; j++)
            {
                Destroy(Espacios.GetChild(i).gameObject);
            }
        }
    }
}

