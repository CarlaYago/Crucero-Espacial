using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public struct Minijuegos
{
    public int id;
    public string nombre;
    public Sprite iconoActivado, iconoDesactivado;
    public string escena;
}

public class InspectorPlanetas : MonoBehaviour
{
    [Header("Encabezado")]
    public TextMeshProUGUI nombrePlaneta;
    public TextMeshProUGUI nivelPlaneta;

    [Header("Imagen")]
    public Image[] fotoPlaneta = new Image[4];

    [Header("Minijuegos")]
    public List<TextMeshProUGUI> textosMinijuegos;
    public List<Image> imagenesMinijuegos;
    public List<TextMeshProUGUI> textosRondas;
    string rondasText;

    [Header("Recompensas")]
    public List<TextMeshProUGUI> textosRecompensas;

    [Header("Coste")]
    public TextMeshProUGUI costeViaje;
    string costeText, nivelText;

    // Listado de minijuegos
    List<Minijuegos> minijuegos;

    [Header("Audio")]
    AudioSource reproductor;
    AudioClip ClickPlaneta;

    void Start()
    {
        costeText = costeViaje.text;
        nivelText = nivelPlaneta.text;
        rondasText = textosRondas[0].text;

        minijuegos = FindObjectOfType<DatosJugador>().minijuegos;


        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor = audioManager.GetComponent<AudioSource>();
        //
        ClickPlaneta = audioManager.ClickPlaneta;

    }

    public void playSound(AudioClip sonido)
    {
        reproductor.clip = sonido;
        reproductor.Play();
    }
    public void CargarMinijuegos(int[] minijuegosElegidos, int[] rondas)
    {
        for (int i = 0; i < 3; i++)
        {
            int minijuegoID = minijuegosElegidos[i];
            Minijuegos minijuego = minijuegos.Find(minijuegoX => minijuegoX.id == minijuegoID);

            textosMinijuegos[i].text = minijuego.nombre;
            imagenesMinijuegos[i].sprite = minijuego.iconoActivado;

            textosRondas[i].text = rondas[i] + rondasText;
        }
    }

    public void CargarCoste(int coste)
    {
        costeViaje.text = costeText + coste;
    }

    public void CargarNivel(int nivel)
    {
        nivelPlaneta.text = nivelText + nivel;
    }

    public void ImagenPlaneta(Transform planeta)
    {
        for (int i = 0; i < 4; i++)
        {
            Transform capasPlaneta = planeta.Find("Capas");
            SpriteRenderer capa = capasPlaneta.GetChild(i).GetComponent<SpriteRenderer>();

            fotoPlaneta[i].sprite = capa.sprite;

            if (capa.flipX) fotoPlaneta[i].transform.localScale = new Vector2(-1, 1);
            else fotoPlaneta[i].transform.localScale = Vector2.one;

            fotoPlaneta[i].color = capa.color;
            playSound(ClickPlaneta);

        }
    }
}