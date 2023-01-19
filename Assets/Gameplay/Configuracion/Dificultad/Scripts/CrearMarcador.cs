using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrearMarcador : MonoBehaviour, IPointerClickHandler
{
    DatosJugador datos;

    [Header("Parámetros")]
    public float offsetY = 40;
    public float distanciaMinMarcadores;
    public int numDificultades = 11;

    [Header("Referencias")]
    public GameObject marcadorPrefab;
    public RectTransform lineaDistancia;

    [Header("Debug")]
    public bool posicionesDebug;
    public List<float> posiciones;
    public List<int> dificultades;
    public GameObject visorPosiciones;
    public Vector2 limites;
    GameObject parentPos;

    AudioSource reproductor;
    AudioClip arrastrarInicio;
    void Start()
    {
        for (int i = 1; i <= numDificultades; i++)
        {
            dificultades.Add(i);
        }

        limites = MedirDistancia(lineaDistancia);

        if (posicionesDebug)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            parentPos = new GameObject("PosicionesDebug");
            parentPos.transform.SetParent(canvas.transform);
        }

        datos = FindObjectOfType<DatosJugador>();
        if (datos != null) LeerDatos();

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        arrastrarInicio = audioManager.arrastrarInicio;
     
        
    }
    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }
    void LeerDatos()
    {
        for (int i = 0; i < datos.distancias.Count; i++)
        {
            GameObject marcador = Instantiate(marcadorPrefab, lineaDistancia);
            float anchura = lineaDistancia.sizeDelta.x;
            float posX = -anchura / 2 + datos.distancias[i] * anchura;

            RectTransform marcadorPos = marcador.GetComponent<RectTransform>();
            marcadorPos.localPosition = new Vector2(posX, offsetY);
            posiciones.Add(marcador.transform.position.x);

            Marcador marcadorScript = marcador.GetComponentInChildren<Marcador>();
            marcadorScript.dificultad.text = datos.dificultades[i] + "";
        }

        if (posicionesDebug) UpdatearPosiciones();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float posX = PosicionSinSolapar(eventData.position.x, Mathf.Infinity, -1);

        if (posX != Mathf.Infinity)
        {
            GameObject marcador = Instantiate(marcadorPrefab, lineaDistancia);

            marcador.transform.position = new Vector2(posX, 0);
            RectTransform marcadorPos = marcador.GetComponent<RectTransform>();
            marcadorPos.localPosition = new Vector2(marcadorPos.localPosition.x, offsetY);

            posiciones.Add(posX);
            if (posicionesDebug) UpdatearPosiciones();
                playSound(arrastrarInicio);
           
        }
        
    }

    public bool PosicionEnUso(float pos)
    {
        for (int i = 0; i < posiciones.Count; i++)
        {
            if (pos > posiciones[i] - distanciaMinMarcadores && pos < posiciones[i] + distanciaMinMarcadores)
            {
                return true;
            }
        }

        return false;
    }

    public float PosicionSinSolapar(float pos, float posPrevia, int index)
    {
        for (int i = 0; i < posiciones.Count; i++)
        {
            if (i != index)
            {
                if (pos > posiciones[i] - distanciaMinMarcadores && pos < posiciones[i] + distanciaMinMarcadores)
                {
                    float distanciaMin = posiciones[i] - distanciaMinMarcadores;
                    float distanciaMax = posiciones[i] + distanciaMinMarcadores;

                    if (Mathf.Abs(pos - distanciaMax) < Mathf.Abs(pos - distanciaMin))
                    {
                        float newPos = distanciaMax;

                        if (PosicionEnUso(newPos)) return posPrevia;
                        else
                        {
                            if (newPos >= limites.x && newPos <= limites.y) return newPos;
                            else return posPrevia;
                        }
                    }
                    else
                    {
                        float newPos = distanciaMin;

                        if (PosicionEnUso(newPos)) return posPrevia;
                        else
                        {
                            if (newPos >= limites.x && newPos <= limites.y) return newPos;
                            else return posPrevia;
                        }
                    }
                }
            }
        }

        return pos;
    }

    public void UpdatearPosiciones()
    {
        foreach (Transform child in parentPos.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < posiciones.Count; i++)
        {
            GameObject lol = Instantiate(visorPosiciones, parentPos.transform);
            float posY = lineaDistancia.GetChild(i).position.y;
            lol.transform.position = new Vector2(posiciones[i], posY);
        }
    }

    Vector2 MedirDistancia(RectTransform obj)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject temp = new GameObject("Regla", typeof(RectTransform));
        temp.transform.SetParent(canvas.transform);

        RectTransform regla = temp.GetComponent<RectTransform>();

        float minX = obj.rect.min.x;
        float maxX = obj.rect.max.x;

        regla.anchoredPosition = new Vector2(minX, 0);
        float minWorldPosX = regla.transform.position.x;

        regla.anchoredPosition = new Vector2(maxX, 0);
        float maxWorldPosX = regla.transform.position.x;

        Destroy(temp);


        return new Vector2(minWorldPosX, maxWorldPosX);
    }
}
