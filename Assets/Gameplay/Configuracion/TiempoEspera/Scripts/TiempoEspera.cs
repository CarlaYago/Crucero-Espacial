using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TiempoEspera : MonoBehaviour
{
    public TextMeshProUGUI[] ddhhmmss;
    public Button confirmar;
    DatosJugador datos;

    void Start()
    {
        datos = FindObjectOfType<DatosJugador>();
        CargarTiempoEspera();

        string[] clavesEsperaPlanetas = { "Dias", "Horas", "Minutos", "Segundos" };
        confirmar.onClick.AddListener(delegate { GuardarTiempoEspera(clavesEsperaPlanetas, ref datos.tiempoDeEspera); });
    }

    void CargarTiempoEspera()
    {
        ddhhmmss[0].text = datos.tiempoDeEspera.x.ToString();
        ddhhmmss[1].text = datos.tiempoDeEspera.y.ToString();
        ddhhmmss[2].text = datos.tiempoDeEspera.z.ToString();
        ddhhmmss[3].text = datos.tiempoDeEspera.w.ToString();
    }

    /// <param name="ddhhmmss"> Nombre de las variables en PlayerPrefs para días, horas, minutos y segundos en ese órden. </param>
    void GuardarTiempoEspera(string[] clavesTiempoEspera, ref Vector4 variableTiempoEspera)
    {
        for (int i = 0; i < ddhhmmss.Length; i++)
        {
            int valor = int.Parse(ddhhmmss[i].text);

            switch (i)
            {
                case 0:
                    GuardarValor(clavesTiempoEspera[0], valor);
                    break;
                case 1:
                    GuardarValor(clavesTiempoEspera[1], valor);
                    break;
                case 2:
                    GuardarValor(clavesTiempoEspera[2], valor);
                    break;
                case 3:
                    GuardarValor(clavesTiempoEspera[3], valor);
                    break;
            }
        }

        datos.AccederTiempoEspera(clavesTiempoEspera, ref variableTiempoEspera);
    }

    void GuardarValor(string nombre, int valor)
    {
        PlayerPrefs.SetInt(nombre, valor);
    }
}