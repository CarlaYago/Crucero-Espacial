using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElegirCategoria : MonoBehaviour
{
    NuevaCategoria categoriaManager;
    Button btn;
    string categoria;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Elegir);

        categoria = GetComponentInChildren<Text>().text;
    }

    void Elegir()
    {
        if (categoriaManager == null) categoriaManager = FindObjectOfType<NuevaCategoria>();
        categoriaManager.CategoriaElegida(categoria, true);
    }
}
