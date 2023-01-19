using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class salirEscenaLobbyMinijuegos : MonoBehaviour
{
    public bool todosLosMJsTerminados = false;
    public GameObject menuConfirmar;
    SeleccionMinijuegos selecMJsScript;

    void Start()
    {
        menuConfirmar.SetActive(false);
    }

    void Update()
    {
        
    }

    public void CargarEscena()
    {
        if (todosLosMJsTerminados == false)
        {
            menuConfirmar.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Planetario_interfaz");
        }
    }

    public void confirmarSalirEscena()
    {
        SceneManager.LoadScene("Planetario_interfaz");
    }

    public void cancelarSalirEscena()
    {
        menuConfirmar.SetActive(false);
    }
}