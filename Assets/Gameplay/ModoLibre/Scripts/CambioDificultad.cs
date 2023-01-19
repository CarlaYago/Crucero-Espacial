using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CambioDificultad : MonoBehaviour
{
    //Base de datos limitar rondas
    int ActUID;
    SQLQuery BDD;
    int MaxRondas;


    int Dificultad, Rondas;
    public GameObject PanelPrefab;
    public Vector2 MinMaxDificultad;
    public Button UpDificultad, DownDificultad, UpRondas, DownRondas;
    TextMeshProUGUI NumeroDif, NumeroRondas;
    public TextMeshProUGUI TextoLimite;
    DatosJugador Datos;
    // Start is called before the first frame update
    void Start()
    {
        Datos = FindObjectOfType<DatosJugador>();
        Datos.minijuegoActualIndex = 0;
        BDD = new SQLQuery("Usuarios");

        NumeroDif = UpDificultad.GetComponentInParent<TextMeshProUGUI>();
        NumeroRondas = UpRondas.GetComponentInParent<TextMeshProUGUI>();
        Debug.Log("Start");

        UpDificultad.onClick.AddListener(delegate { CambiarDificultad(1);});
        DownDificultad.onClick.AddListener(delegate { CambiarDificultad(-1);});

        UpRondas.onClick.AddListener(delegate { CambiarRondas(1); });
        DownRondas.onClick.AddListener(delegate { CambiarRondas(-1); });
        PanelPrefab.SetActive(false);

        Dificultad = int.Parse(NumeroDif.text);
        Rondas = int.Parse(NumeroRondas.text);
    }

    public void CambiarDificultad(int modificador)
    {
        Dificultad = int.Parse(NumeroDif.text);

        if (Dificultad + modificador >= MinMaxDificultad.x && Dificultad + modificador <= MinMaxDificultad.y)
        {
            Dificultad += modificador;
            NumeroDif.text = Dificultad.ToString();
        }
        LimitarRondas();
    }

    public void CambiarRondas(int modificador)
    {
        Rondas = int.Parse(NumeroRondas.text);
        if (Rondas + modificador >= 1 && Rondas + modificador <= MaxRondas)
        {
            Rondas += modificador;
            NumeroRondas.text = Rondas.ToString();
        }
    }

    public void Confirmar()
    {
        Datos.minijuegoDificultad = Dificultad;
        Datos.minijuegosRondas = new int[] { Rondas };
        Datos.minijuegosPorcentajes = new float[1];
        Datos.minijuegoRecienCompletado = new List<bool>(new bool[1]);
    }

    public void GetUID(int UID)
    {
        ActUID = UID;
        Dificultad = int.Parse(NumeroDif.text);
        LimitarRondas();
    }

    public void LimitarRondas()
    {

        BDD.Query("select Rondas from Minijuegos where Minijuego = " + ActUID + " and Dificultad = " + Dificultad);
        int[] ArrayRondas = BDD.IntArrayReader(1, '-')[0];
        if (ArrayRondas.Length > 1)
        {
            MaxRondas = ArrayRondas[1];
        }
        else
        {
            MaxRondas = ArrayRondas[0];
        }

        if (Rondas > MaxRondas)
        {
            Rondas = MaxRondas;
            NumeroRondas.text = Rondas.ToString();
        }

        TextoLimite.text = "Max " + MaxRondas;
    }

}
