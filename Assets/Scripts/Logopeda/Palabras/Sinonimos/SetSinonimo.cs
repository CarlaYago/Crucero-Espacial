using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSinonimo : MonoBehaviour
{
    [Header("Cambiar script para referenciar antónimos")]
    public bool isAntonimo;

    NuevoSinonimo sinonimoManager;
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
        if (sinonimoManager == null)
        {
            NuevoSinonimo[] array = FindObjectsOfType<NuevoSinonimo>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].isAntonimo == isAntonimo) sinonimoManager = array[i];
            }
        }

        if (self == null) self = GetComponent<Text>();

        sinonimoManager.ActualizarInterfaz(id, self);
    }
}
