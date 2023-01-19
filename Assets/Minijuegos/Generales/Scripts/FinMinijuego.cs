using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinMinijuego : MonoBehaviour
{
    DatosJugador datosJugador;
    public Button confirmarBoton;

    [Header("Nombres escenas")]
    public string modoLibre;
    public string lobbyMinijuegos;

    void Start()
    {
        datosJugador = FindObjectOfType<DatosJugador>();
        if (datosJugador.modoLibre == true)
        {
            confirmarBoton.onClick.AddListener(delegate { SceneManager.LoadScene(modoLibre); });
        }
        else
        {
            confirmarBoton.onClick.AddListener(delegate { SceneManager.LoadScene(lobbyMinijuegos); });
        }
    }
}
