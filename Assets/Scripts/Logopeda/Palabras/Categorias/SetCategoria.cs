using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCategoria : MonoBehaviour
{
    NuevaCategoria categoriaManager;
    Text self;
    Button btn;

    int id;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Cambiar);

        id = transform.parent.GetChild(0).GetComponent<Identificador>().id;
    }

    public void Cambiar()
    {
        if (categoriaManager == null) categoriaManager = FindObjectOfType<NuevaCategoria>();
        if (self == null) self = GetComponent<Text>();

        categoriaManager.ActualizarInterfaz(id, self);
    }
}
