using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DificultadMarcador : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI textoDificultad;
    public Button botonSubir, botonBajar;

    public Vector2 rangoDificultad;
    public bool ocultarBotones;

    HorizontalLayoutGroup layoutGroup;

    void Start()
    {
        botonSubir.onClick.AddListener(delegate { CambioDificultad(+1); });
        botonBajar.onClick.AddListener(delegate { CambioDificultad(-1); });

        ActivarBoton(botonSubir, false);
        ActivarBoton(botonBajar, false);

        int dificultad = int.Parse(textoDificultad.text);
        if (ocultarBotones) OcultarBotones(dificultad);

        layoutGroup = GetComponent<HorizontalLayoutGroup>();
    }

    void CambioDificultad(int cambio)
    {
        int dificultad = int.Parse(textoDificultad.text);

        if (dificultad + cambio >= rangoDificultad.x && dificultad + cambio <= rangoDificultad.y)
        {
            dificultad += cambio;
        }

        if (ocultarBotones) OcultarBotones(dificultad);

        textoDificultad.text = dificultad.ToString();

        if (layoutGroup != null) { Canvas.ForceUpdateCanvases(); layoutGroup.enabled = false; layoutGroup.enabled = true; }
    }

    void OcultarBotones(int dificultad)
    {
        // Deshabilitar / Habilitar botón -
        if (dificultad == rangoDificultad.x)
        {
            botonBajar.targetGraphic.enabled = false;
        }
        else if (!botonBajar.targetGraphic.enabled)
        {
            botonBajar.targetGraphic.enabled = true;
        }

        // Deshabilitar / Habilitar botón +
        if (dificultad == rangoDificultad.y)
        {
            botonSubir.targetGraphic.enabled = false;
        }
        else if (!botonSubir.targetGraphic.enabled)
        {
            botonSubir.targetGraphic.enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivarBoton(botonSubir, true);
        ActivarBoton(botonBajar, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ActivarBoton(botonSubir, false);
        ActivarBoton(botonBajar, false);
    }

    void ActivarBoton(Button btn, bool activar)
    {
        if (btn.targetGraphic.enabled != activar)
        {
            btn.targetGraphic.enabled = activar;
            btn.enabled = activar;
        }
    }
}
