using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDefinicion : MonoBehaviour
{
    NuevaDefinicion definicionManager;
    RawImage self;
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
        if (definicionManager == null) definicionManager = FindObjectOfType<NuevaDefinicion>();
        if (self == null) self = GetComponentInChildren<RawImage>();

        definicionManager.ActualizarInterfaz(id, self);
    }
}
