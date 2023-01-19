// Colocar este script en los valores de una fila para configurar y abrir su panel de dificultad. //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDificultad : MonoBehaviour
{
    int registroID;
    string tipoDeRegistro;

    public string nombrePanelParent; // Referenciamos un parent activo ya que los objetos que llevan este script se instancian (no podemos asignar el panel desactivado por script) 
    LeerDificultad panelParent;
    List<CambiarDificultad> scriptsDificultad = new List<CambiarDificultad>();

    void Start()
    {
        LeerDificultad[] paneles = FindObjectsOfType<LeerDificultad>();

        for (int i = 0; i < paneles.Length; i++)
        {
            if (paneles[i].gameObject.name == nombrePanelParent)
            {
                // Acceder al panel de configuración de este tipo de registro
                panelParent = paneles[i];

                // Acceder a todos los scripts de dificultad asociados con su panel de configuración
                Text[] textosDificultad = panelParent.dificultadesUI;
                foreach (Text textoDificultad in textosDificultad)
                {
                    CambiarDificultad dificultadScript = textoDificultad.GetComponentsInParent<CambiarDificultad>(true)[0];
                    scriptsDificultad.Add(dificultadScript);
                }

                break;
            }
        }

        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(EnviarDificultad);
    }

    public void EnviarDificultad()
    {
        // --- Detectar ID propia --- //
        if (transform.parent.GetChild(0).TryGetComponent(out Identificador idScript))
        {
            registroID = idScript.id;
        }
        else
        {
            idScript = transform.parent.parent.GetChild(0).GetComponent<Identificador>();
            registroID = idScript.id;
        }

        // --- Detectar el tipo de registro propio (Palabra, Texto...) en base al script de cambio de interfaz --- //
        CambioInterfaz managerUI = FindObjectOfType<CambioInterfaz>();
        tipoDeRegistro = managerUI.identificadorActivo;

        // --- Enviar su ID a todos los scripts de cambio dificultad relevantes para poder actualizar sus valores en BDD --- //
        foreach (CambiarDificultad dificultadScript in scriptsDificultad)
        {
            dificultadScript.registroID = registroID;
        }

        // --- Mostrar los valores de dificultad del registro actual (desde BDD) en la interfaz --- //
        panelParent.Leer(registroID, tipoDeRegistro);

        // --- Cambiar encabezado --- //
        if (tipoDeRegistro == "Palabra")
        {
            string palabra = idScript.gameObject.GetComponent<Text>().text;
            panelParent.CambiarEncabezado(palabra);
        }
        else
        {
            int index = idScript.transform.parent.GetSiblingIndex();
            panelParent.CambiarEncabezado(index.ToString());
        }

        // --- Activar el panel --- //
        panelParent.transform.GetChild(0).gameObject.SetActive(true);
    }
}