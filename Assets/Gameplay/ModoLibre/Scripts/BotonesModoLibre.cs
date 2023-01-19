using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonesModoLibre : MonoBehaviour
{
    DatosJugador datosJugador;

    private void Start()
    {
        datosJugador = FindObjectOfType<DatosJugador>();
    }

    public void EntrarMLDesdeMenu(bool desdeMenuPrincipal)
    {
        datosJugador.modoLibre = true;
        datosJugador.menuPrincipal = desdeMenuPrincipal;
    }

    public void SalirML()
    {
        datosJugador.modoLibre = false;

        if (datosJugador.menuPrincipal) // Si se accede desde el menú principal...
        {
            SceneManager.LoadScene("SeleccionPaciente");
        }
        else // Si se accede desde la nave...
        {
            SceneManager.LoadScene("Nave");
        }
    }
}