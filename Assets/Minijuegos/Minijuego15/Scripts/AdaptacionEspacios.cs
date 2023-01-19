using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptacionEspacios : MonoBehaviour
{
    public Vector2 AnchuraInicial;
    bool DoOnce;
    public bool refrescarVLayoutGroup;

    public void Start()
    {
        AnchuraInicial = transform.localScale;
    }

    public void Update()
    {
        AdaptarEspacios();
    }
    
    public void AdaptarEspacios()
    {
        if (transform.childCount > 0 && DoOnce == true)
        {
            gameObject.GetComponent<ContentSizeFitter>().enabled = true;
            
            if (refrescarVLayoutGroup)
            {
                ArreglarEspacios();
            }

            DoOnce = false;
            Debug.Log("Estoy en el if :D");
        }
        else if(transform.childCount == 0 && DoOnce == false)
        {
            gameObject.GetComponent<ContentSizeFitter>().enabled = false;

            if (refrescarVLayoutGroup)
            {
                ArreglarEspacios();
            }

            gameObject.transform.localScale = AnchuraInicial;
            DoOnce = true;
            Debug.Log("Estoy en el else if :D");
        }
    }

    void ArreglarEspacios()
    {
        Canvas.ForceUpdateCanvases();
        transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
        transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        Debug.Log("hola estoy entrando :D");
    }
}
