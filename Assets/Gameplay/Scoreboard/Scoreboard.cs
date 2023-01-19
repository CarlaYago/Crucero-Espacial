using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scoreboard : MonoBehaviour
{
    SQLQuery consultas;
    DatosJugador datos;
    string nombreJugador;
    List <string> NombreJugador = new List<string>();
    List <int> ExpJugador = new List<int>();
    public GameObject PrefabJugador, Scrollview;

    void Start()
    {
        consultas = new SQLQuery("Usuarios");
        consultas.Query("SELECT Nombre, Experiencia FROM Usuarios ORDER BY Experiencia, Nombre ASC");
        NombreJugador = consultas.StringReader(1); 
        ExpJugador = consultas.IntReader(2);

        datos = FindObjectOfType<DatosJugador>();
        nombreJugador = datos.nombre;

        for (int i = 0; i < NombreJugador.Count; i++)
        {
            GameObject JugadorDatos = Instantiate(PrefabJugador, Scrollview.transform);
            JugadorDatos.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = NombreJugador[i];
            JugadorDatos.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ExpJugador[i].ToString();

            if (NombreJugador[i] == nombreJugador)
            {
                Debug.Log(nombreJugador);
                JugadorDatos.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Vector4(0, 0, 0, 255);
                JugadorDatos.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Vector4(0, 0, 0, 255);
            }
        }
    }

    public void Salir()
    {
        SceneManager.LoadScene("SeleccionPaciente");
    }

    void Update()
    {
        
    }
}
