using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterMinijuego : MonoBehaviour
{
    Interfaz interfaz;
    public string Escenas;

    void Start()
    {
        interfaz = FindObjectOfType<Interfaz>();
    }

    public void IniciarMinijuego()
    {
        if (Escenas != null)
        {
            interfaz.CargarEscena(Escenas);
        }
    }

    public void SetearMinijuego(string Escena)
    {
        Escenas = Escena;
    }

    public void Cancelar()
    {
        Escenas = "";
    }

}
