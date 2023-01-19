using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPregunta : MonoBehaviour
{
    NuevaPregunta preguntaManager;
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
        if (preguntaManager == null) preguntaManager = FindObjectOfType<NuevaPregunta>();
        if (self == null) self = GetComponent<Text>();

        preguntaManager.ActualizarInterfaz(id, self);
    }
}