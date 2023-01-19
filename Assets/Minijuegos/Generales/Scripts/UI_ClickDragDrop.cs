using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class UI_ClickDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    GapScript gapscript;
    GameObject objetoOcupante;

    Transform parent, canvas;
    int siblingIndex;

    AudioManager audioManager;
    AudioSource reproductor;
    AudioClip arrastrarInicio, soltarEnCasilla, recolocar;

    InterfazGeneralManager interfazMinijuego;
    float contador;
    bool eliminarObjeto;

    UI_ClickDragDrop letraIntercambio;
    Transform parentAnterior;

    //FALTA DESACTIVAR ESTE SCRIPT CUANDO EL TIEMPO LLEGUE A CERO

    void Start()
    {
        parent = transform.parent;
        canvas = FindObjectOfType<Canvas>().transform;

        audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.AudioSource1;
        //
        arrastrarInicio = audioManager.arrastrarInicio;
        soltarEnCasilla = audioManager.soltarEnCasilla;
        recolocar = audioManager.recolocar;

        interfazMinijuego = FindObjectOfType<InterfazGeneralManager>();
    }

    void Update()
    {
        if (interfazMinijuego.empezarTemporizador)
        {
            if (interfazMinijuego.GameTime <= 1)
            {
                eliminarObjeto = true;
            }
        }

        if (eliminarObjeto)
        {
            contador += Time.deltaTime;
            if (contador >= 1)
            {
                Destroy(gameObject);
            }
        }
    }

    void letraIdentificada(List<RaycastResult> objetosCanvas)
    {
        for (int i = 0; i < objetosCanvas.Count; i++)
        {
            UI_ClickDragDrop letra = objetosCanvas[i].gameObject.GetComponent<UI_ClickDragDrop>();
            if (letra != null && letra.enabled)
            {
                if (objetosCanvas[i].gameObject != gameObject)
                {
                    letraIntercambio = letra;
                    break;
                }
            }
            else if (i == objetosCanvas.Count - 1)
            {
                letraIntercambio = null;
            }
        }
    }

    List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        parentAnterior = transform.parent;
        siblingIndex = transform.GetSiblingIndex();
        transform.SetParent(canvas);
        playSound(arrastrarInicio);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        letraIdentificada(GetEventSystemRaycastResults());
    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (gapscript != null)
        {
            if (gapscript.gapOcupado == false)
            {
                Vector3 gapPos = new Vector3(objetoOcupante.transform.position.x, objetoOcupante.transform.position.y, 0f);
                transform.position = gapPos;
                transform.SetParent(objetoOcupante.transform);
                transform.SetSiblingIndex(0);
                playSound(soltarEnCasilla);
            }
            else
            {
                GameObject letraIntercambio = gapscript.transform.GetChild(0).gameObject;
                UI_ClickDragDrop cddScript = letraIntercambio.GetComponent<UI_ClickDragDrop>();
                if (cddScript.enabled)
                {
                    transform.SetParent(gapscript.transform);
                    transform.position = gapscript.transform.position;
                    transform.SetSiblingIndex(letraIntercambio.transform.GetSiblingIndex());

                    letraIntercambio.transform.SetParent(parentAnterior);
                    //Si nuestro parent anterior era un espacio, setear la posición de la otra letra 
                    GapScript espaciosParentAnterior = parentAnterior.GetComponent<GapScript>();
                    if (espaciosParentAnterior != null)
                    {
                        letraIntercambio.transform.position = parentAnterior.position;
                    }
                    else cddScript.ActualizarCanvas();

                    letraIntercambio.transform.SetSiblingIndex(siblingIndex);

                    playSound(soltarEnCasilla);
                }
                else
                {
                    RecolocarElemento();
                }
            }
        }
        else
        {
            RecolocarElemento();
            playSound(recolocar);
        }

        GapScript espacioParent = transform.parent.GetComponent<GapScript>();
        if (letraIntercambio != null && espacioParent == null)
        {
            GapScript espaciosLetraIntercambio = letraIntercambio.GetComponentInParent<GapScript>();
            if (espaciosLetraIntercambio == null)
            {
                transform.SetParent(letraIntercambio.transform.parent);
                transform.SetSiblingIndex(letraIntercambio.transform.GetSiblingIndex());
                ActualizarCanvas();
            }

            GapScript espaciosParentAnterior = parentAnterior.GetComponent<GapScript>();
            if (espaciosParentAnterior == null)
            {
                letraIntercambio.transform.SetParent(parentAnterior);
                letraIntercambio.transform.SetSiblingIndex(siblingIndex);
                letraIntercambio.ActualizarCanvas();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        gapscript = col.GetComponent<GapScript>();
        if (col.CompareTag("AreaPermitida"))
        {
            objetoOcupante = col.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D exCol)
    {
        gapscript = null;
        objetoOcupante = null;
    }

    void RecolocarElemento()
    {
        transform.SetParent(parent);
        transform.SetSiblingIndex(siblingIndex);

        ActualizarCanvas();
    }

    void ActualizarCanvas()
    {
        Canvas.ForceUpdateCanvases();

        if (parent.TryGetComponent(out VerticalLayoutGroup layout1))
        {
            layout1.enabled = false;
            layout1.enabled = true;
        }
        else if (parent.TryGetComponent(out HorizontalLayoutGroup layout2))
        {
            layout2.enabled = false;
            layout2.enabled = true;
        }
        else if (parent.TryGetComponent(out GridLayoutGroup layout3))
        {
            layout3.enabled = false;
            layout3.enabled = true;
        }
    }
}