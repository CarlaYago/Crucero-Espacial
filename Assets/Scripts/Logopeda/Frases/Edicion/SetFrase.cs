using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetFrase : MonoBehaviour
{
    NuevaFrase fraseManager;
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
        if (fraseManager == null) fraseManager = FindObjectOfType<NuevaFrase>();
        if (self == null) self = GetComponent<Text>();

        fraseManager.ActualizarInterfaz(id, self);
    }
}