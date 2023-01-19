using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDesplegable : MonoBehaviour
{
    [Header("Prefabs")]

    public GameObject textoPalabras;
    public GameObject boton;
    public GameObject prefab;

    [Header("ScrollView")]

    public GameObject content;
    public GameObject scrollVertical;
    public GameObject textPalabra;
    public Transform pParent;

    bool cambio = true;
    int ran;
    List<GameObject> listaPrefabs = new List<GameObject>();


    void Start()
    {
        ran = Random.Range(5, 201);
        anyadirElementos();
        textoPalabras.transform.position = new Vector3(112.3f, 504.1f, -11.2f);
    }
    
    public void desplegar()
    {
       if (cambio == true)
        {
            gameObject.transform.position = new Vector3(114.7f, 271.9f, 0f);
            boton.transform.Rotate(0, 0, 180);
            content.SetActive(true);
            scrollVertical.SetActive(true);
            textPalabra.SetActive(false);
            cambio = false;
        }
        else
        {
            gameObject.transform.position = new Vector3(114.7f, 724.7f, 0f);
            boton.transform.Rotate(0, 0, 180);
            content.SetActive(false);
            scrollVertical.SetActive(false);
            textPalabra.SetActive(true);
            textPalabra.transform.position = new Vector3(114.7f, 503.2f, -11.2f);
            cambio = true;
        }
      
    }

    public void anyadirElementos()
    {
        for (int i = 0; i < ran; i++)
        {
            prefab.GetComponent<PuntoPrefab>().num = Random.Range(0, 5);
            Instantiate(prefab, pParent);
        }
    }


}
