using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EliminarPalabra : MonoBehaviour
{
    public GameObject registro;
    Button btn;
    string palabra;

    // Referencias al panel de confirmación para eliminar palabras
    public string refPanelConfirmacion, rutaBotonEliminar, rutaBotonCancelar1, rutaBotonCancelar2;
    GameObject panelConfirmacion;
    Button eliminar, cancelar1, cancelar2;

    Diccionario jugador;

    void Start()
    {
        palabra = transform.parent.parent.GetChild(0).GetComponent<Text>().text;

        btn = GetComponent<Button>();
        btn.onClick.AddListener(Confirmar);

        jugador = FindObjectOfType<Diccionario>();
    }

    void Destruir() // se le llama al confirmar la eliminación de un registro, borrándolo
    {
        eliminar.onClick.RemoveListener(Destruir);
        panelConfirmacion.SetActive(false);
        Destroy(registro);

        // Eliminar registro del diccionario
        List<PalabrasDisponibles> dicc = jugador.diccionario;

        for (int i = 0; i < dicc.Count; i++)
        {
            if (dicc[i].palabra == palabra)
            {
                dicc.RemoveAt(i);
            }
        }
    }

    void Confirmar() // se le llama al hacer click al botón de eliminar registro, cargando un panel de confirmación
    {
        if (panelConfirmacion == null) panelConfirmacion = GameObject.Find(refPanelConfirmacion).transform.GetChild(0).gameObject;
        panelConfirmacion.SetActive(true);

        if (eliminar == null) eliminar = panelConfirmacion.transform.Find(rutaBotonEliminar).GetComponent<Button>();
        eliminar.onClick.AddListener(Destruir);

        if (cancelar1 == null) cancelar1 = panelConfirmacion.transform.Find(rutaBotonCancelar1).GetComponent<Button>();
        if (cancelar2 == null) cancelar2 = panelConfirmacion.transform.Find(rutaBotonCancelar2).GetComponent<Button>();
        cancelar1.onClick.AddListener(Denegar);
        cancelar2.onClick.AddListener(Denegar);
    }

    void Denegar() // se le llama al cancelar la confirmación, evitando que este registro se destruya al borrar cualquier otro
    {
        eliminar.onClick.RemoveListener(Destruir);
        panelConfirmacion.SetActive(false);
    }
}
