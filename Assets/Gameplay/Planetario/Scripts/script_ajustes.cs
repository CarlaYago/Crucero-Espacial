using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class script_ajustes : MonoBehaviour
{
    public GameObject popUp;
    public ManagerPlanetario scriptPopUp;
    public void x()
    {
        popUp.SetActive(false);
        scriptPopUp.activado = false;
    }
}
