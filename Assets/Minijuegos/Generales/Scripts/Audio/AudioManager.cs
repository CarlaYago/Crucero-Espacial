using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource AudioSource1;
    public AudioSource AudioSource2;
    public AudioSource AudioSource3;
    public AudioClip arrastrarInicio, soltarEnCasilla, recolocar, EnviarDatos;
    public AudioClip fallo, acierto;
    public AudioClip RecompensaSonido, RecompensaSonido_mal;
    public AudioClip PulsarBoton;
    public AudioClip PulsarTiempoEspera;
    public AudioClip SubirNivel, PorcentajeSubiendo, Experiencia;
    public AudioClip ClickPlaneta;
    public AudioClip SubidaCarta;
    public AudioClip CapaPlanetario;
    public AudioClip BandaSonora1, BandaSonora2, BandaSonora3;
    public List<AudioClip> audioCinematica;




    void Awake() // DontDestroy 
    {
        AudioManager[] objs = FindObjectsOfType<AudioManager>();

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (GetComponent<AudioSource>() == null)
        {
            Debug.LogError("El objeto con el script AudioDragDrop necesita un AudioSource para poder reproducir sonidos.");
        }
    }
}