using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TrancisionesScript : MonoBehaviour
{
    DatosJugador datos;
    public List<SpriteRenderer> Escenario;
    public List<Material> EscenarioMat;
    // Start is called before the first frame update
    private void Awake()
    {
        datos = FindObjectOfType<DatosJugador>();


    }
    void Start()
    {
       // EscenarioMat[1].SetColor("Color_12", Color.blue);

        EscenarioMat[1].SetColor("Color_12", datos.colores[1]);

        EscenarioMat[1].SetColor("Color_9753af98497e459d9c399d252f7f80fe", datos.colores[1]);
        //EscenarioMat[1].SetTexture("_SampleTexture2D_bdd453e3f45c4180a622589de800fa9f_Texture_1",)
        EscenarioMat[0].SetColor("Color_12", datos.colores[0]);
        EscenarioMat[0].SetColor("Color_9753af98497e459d9c399d252f7f80fe", datos.colores[0]);

        EscenarioMat[2].SetColor("Color_12", datos.colores[2]);
        EscenarioMat[2].SetColor("Color_9753af98497e459d9c399d252f7f80fe", datos.colores[2]);

        EscenarioMat[3].SetColor("Color_12", datos.colores[2]);
        EscenarioMat[3].SetColor("Color_9753af98497e459d9c399d252f7f80fe", datos.colores[2]);

        StartCoroutine("escena");
    }
    IEnumerator escena()
    {
        yield return new WaitForSeconds(12f);
        SceneManager.LoadScene("InterfazPrincipalRecompensa");
    }
    public void Saltar()
    {
        SceneManager.LoadScene("InterfazPrincipalRecompensa");
    }
}
