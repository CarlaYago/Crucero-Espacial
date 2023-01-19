using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImagen : MonoBehaviour
{
    NuevaImagen imagenManager;
    RawImage self;
    Button btn;

    int id;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Cambiar);

        id = transform.parent.parent.GetChild(0).GetComponent<Identificador>().id;
    }

    public void Cambiar()
    {
        if (imagenManager == null) imagenManager = FindObjectOfType<NuevaImagen>();
        if (self == null) self = GetComponent<RawImage>();

        imagenManager.ActualizarInterfaz(id, self);
    }
}
