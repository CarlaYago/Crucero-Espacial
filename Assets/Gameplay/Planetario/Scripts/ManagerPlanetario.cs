using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ManagerPlanetario : MonoBehaviour
{
    SQLQuery consulta;
    DatosJugador datos;

    public GameObject popUp;
    public TextMeshProUGUI energiaJugadorText;
    public Button viajarBot;
    public GameObject infoPlaneta, imgSeleccion;
    public desplegable desplegable;
    public int energiaJugador; //se accede desde PlanetPrefab
    public bool activado;
    string energiaText;
    public Camera cam;
    public Transform planetario;
    PlanetaPrefab tocado;
    public Toggle favToggle;
    Interfaz interfazScript;
    LeerPlanetas LectorPScript;

    bool doOnce;

    void Start()
    {
        interfazScript = FindObjectOfType<Interfaz>();
        LectorPScript = FindObjectOfType<LeerPlanetas>();
        datos = FindObjectOfType<DatosJugador>();
        energiaText = energiaJugadorText.text;

        CambioEnergia(datos.gasolina);

        infoPlaneta.SetActive(false);
        imgSeleccion.SetActive(false);

        consulta = new SQLQuery("Usuarios");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) //si pulsas L se abre la leyenda
        {
            popUp.SetActive(!activado);
            activado = !activado;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // si pulsas ESC sales 
        {
            LectorPScript.RondasGeneradas();
            interfazScript.CargarEscena("Nave");
        }

        if (Input.GetKeyDown(KeyCode.C)) // centrar nave en el 0,0 del planetario
        {
            cam.transform.position = new Vector3(planetario.position.x, planetario.position.y, -39);
            infoPlaneta.SetActive(false);
            imgSeleccion.SetActive(false);

        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit && !RaycastCanvas())
            {
                tocado = hit.transform.gameObject.GetComponent<PlanetaPrefab>();
                Transform capas = tocado.transform.GetChild(0);
                for (int i = 0; i < capas.childCount; i++)
                {
                    datos.colores[i] = capas.GetChild(i).GetComponent<SpriteRenderer>().color;
                }


                favToggle.isOn = tocado.favorito;
                viajarBot.onClick.RemoveAllListeners();
                viajarBot.onClick.AddListener(tocado.Viajar);

                if (doOnce == false && tocado != null)
                {
                    infoPlaneta.SetActive(true);
                    imgSeleccion.SetActive(true);

                    doOnce = true;
                }
            }
        }
    }

    bool RaycastCanvas()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem) { position = Input.mousePosition };
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count > 0) return true;
        else return false;
    }

    public void CambioEnergia(int energia)
    {
        energiaJugador = energia;
        energiaJugadorText.text = energiaText + energia;

        datos.ActualizarGasolina(energia);
    }
    public void MarcarFavorito(bool fav)
    {
        if (fav)
        {
            tocado.favorito = true;
            consulta.Query("UPDATE Planetario_" + datos.id + " SET Favorito = 1 WHERE Planeta_ID = " + tocado.id);
        }
        else
        {
            tocado.favorito = false;
            consulta.Query("UPDATE Planetario_" + datos.id + " SET Favorito = NULL WHERE Planeta_ID = " + tocado.id);
        }
    }
}