using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interfaz : MonoBehaviour
{
    public void Cerrar(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void Abrir(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void CargarEscena(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }

    public void CerrarAplicacion()
    {
        Application.Quit();
    }
}
