using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class CargarEscenas : MonoBehaviour
{
    [Header("Botones Minijuegos")]
    public Button[] botonesEscena;
    public Image[] iconosDesactivados;
    Image[] iconosActivados;
    //
    public Image[] filtrosDesactivado;
    Color tinteFiltro;
    //
    public TextMeshProUGUI[] porcentajes;

    [Header("Cables encendidos")]
    public Image[] cablesIzquierdaOn;
    public Image[] cableCentroOn, cablesDerechaOn;

    [Header("Cables apagados")]
    public Image[] cablesIzquierdaOff;
    public Image[] cableCentroOff, cablesDerechaOff;
    float contadorCableOn, contadorCableOff;
    float[] tiemposPorCable;

    [Header("Combustible")]
    public Image iconoCombustible;
    public TextMeshProUGUI porcentajeCombustible;
    float combustibleInicial, combustibleFinal, contadorCombustible, tiempoCombustible = -1;

    [Header("Botón Volver")]
    public Button volver;

    [Header("Recompensas")]
    public Transform contenidoRecompensas;
    public GameObject recompensaPrefab;
    public Sprite[] simbolosRarezas;

    [Header("Parámetros")]
    public float tiempoAparicionFiltro = 1;
    public float tiempoDePorcentaje = 1, velocidadCables = 1, delayCalculoCombustible;

    DatosJugador datos;
    float contadorMJ;
    int indexMJCompletado = -1;

    void Start()
    {
        datos = FindObjectOfType<DatosJugador>();
        CargarIconos();
        ActivarBotones();
        CargarRecompensas();
    }

    void Update()
    {
        if (indexMJCompletado != -1)
        {
            if (datos.minijuegosPorcentajes[indexMJCompletado] > 0)
            {
                VaciarImagen(indexMJCompletado, datos.minijuegosPorcentajes[indexMJCompletado]);
                ActivarCables(true);
                ActualizarIconoCombustible();
            }
        }
    }

    #region Funciones iniciales
    void CargarMinijuego(string escena, int index)
    {
        datos.minijuegoActualIndex = index;
        SceneManager.LoadScene(escena);
    }

    void ActivarBotones()
    {
        for (int i = 0; i < botonesEscena.Length; i++)
        {
            if (datos.minijuegosPorcentajes[i] == 0)
            {
                int id = datos.minijuegosElegidos[i];
                Minijuegos minijuego = datos.minijuegos.Find(x => x.id == id);

                string escena = minijuego.escena;
                int index = i;

                botonesEscena[i].onClick.AddListener(delegate { CargarMinijuego(escena, index); });
                //if (volver.interactable) volver.interactable = false;
            }
            else
            {
                botonesEscena[i].enabled = false;
            }
        }

        if (volver.interactable)
        {
            float porcentaje = 0;
            int numMinijuegos = datos.minijuegosPorcentajes.Length;

            for (int i = 0; i < numMinijuegos; i++)
            {
                porcentaje += datos.minijuegosPorcentajes[i];
            }

            porcentaje /= numMinijuegos;

            datos.minijuegoCombustible = (int)(datos.minijuegoCombustible * porcentaje);
            volver.onClick.AddListener(datos.ReclamarRecompensas);
            volver.onClick.AddListener(datos.ActualizarVisitaPlanetas);
        }
    }

    void CargarIconos() // Carga los iconos, sus porcentajes y el combustible en la interfaz
    {
        iconosActivados = new Image[botonesEscena.Length];
        tinteFiltro = filtrosDesactivado[0].color;

        for (int i = 0; i < botonesEscena.Length; i++)
        {
            iconosActivados[i] = botonesEscena[i].GetComponent<Image>();
        }

        float porcentajeGeneral = 0;

        for (int i = 0; i < iconosActivados.Length; i++)
        {
            int id = datos.minijuegosElegidos[i];
            Minijuegos minjuego = datos.minijuegos.Find(x => x.id == id);

            iconosActivados[i].sprite = minjuego.iconoActivado;
            iconosDesactivados[i].sprite = minjuego.iconoDesactivado;

            if (datos.minijuegoRecienCompletado[i] == false)
            {
                // Para los minijuegos ya completados, mostrar el filtro de desactivado y % de completado
                if (datos.minijuegosPorcentajes[i] != 0)
                {
                    iconosActivados[i].fillAmount = 1 - datos.minijuegosPorcentajes[i];
                    porcentajes[i].text = (datos.minijuegosPorcentajes[i] * 100).ToString("0") + "%";
                    filtrosDesactivado[i].color = new Color(tinteFiltro.r, tinteFiltro.g, tinteFiltro.b, 1);

                    porcentajeGeneral += datos.minijuegosPorcentajes[i];
                }
                // Para los minijuegos no completados, ocultar el filtro y %
                else
                {
                    porcentajes[i].enabled = false;
                    filtrosDesactivado[i].color = new Color(1, 1, 1, 0);
                }
            }
            else
            {
                // Para el minijuego recien completado, guardar su valor para representar el % adquirido
                indexMJCompletado = i;
                filtrosDesactivado[i].color = new Color(1, 1, 1, 0);
                datos.minijuegoRecienCompletado[i] = false;

                tiemposPorCable = TiemposPorCable();
            }
        }

        // Si algun minijuego acaba de ser completado, iniciar las variables para representar el combustible adquirido
        if (indexMJCompletado != -1)
        {
            combustibleInicial = porcentajeGeneral / 3;
            combustibleFinal = (porcentajeGeneral + datos.minijuegosPorcentajes[indexMJCompletado]) / 3;

            iconoCombustible.fillAmount = combustibleInicial;
            porcentajeCombustible.text = (combustibleInicial * 100).ToString("0") + "%";
        }
        else porcentajeCombustible.enabled = false;
    }

    void CargarRecompensas()
    {
        for (int i = 0; i < datos.recompensas.Count; i++)
        {
            GameObject palabraConseguida = Instantiate(recompensaPrefab, contenidoRecompensas);
            palabraConseguida.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = datos.recompensas[i];
            int rarezaIndex = datos.rarezasRecompensas[i];
            palabraConseguida.transform.GetChild(0).GetComponent<Image>().sprite = simbolosRarezas[rarezaIndex];
        }
    }
    #endregion Funciones iniciales

    #region Animacion iconos
    void VaciarImagen(int index, float porcentajeCompletado)
    {
        if (iconosActivados[index].fillAmount > (1 - porcentajeCompletado))
        {
            contadorMJ += Time.deltaTime;
            iconosActivados[index].fillAmount = Mathf.Lerp(1, (1 - porcentajeCompletado), contadorMJ / tiempoDePorcentaje);
            porcentajes[index].text = Mathf.Lerp(0, (porcentajeCompletado * 100), contadorMJ / tiempoDePorcentaje).ToString("0") + "%";
        }
        else
        {
            ActivarCables(false);

            if (filtrosDesactivado[index].color.a < 1)
            {
                contadorMJ += Time.deltaTime;
                float opacidad = Mathf.Lerp(0, 1, (contadorMJ - tiempoDePorcentaje) / tiempoAparicionFiltro);

                filtrosDesactivado[index].color = new Color(tinteFiltro.r, tinteFiltro.g, tinteFiltro.b, opacidad);
            }
            else contadorMJ = 0;
        }
    }

    void RellenarCombustible(Image[] cable)
    {
        if (cable[cable.Length - 1].fillAmount >= 1 && contadorCombustible < tiempoCombustible + delayCalculoCombustible)
        {
            contadorCombustible += Time.deltaTime;
            iconoCombustible.fillAmount = Mathf.Lerp(combustibleInicial, combustibleFinal, contadorCombustible / tiempoCombustible);
            float porcentaje = Mathf.Lerp(combustibleInicial, combustibleFinal, contadorCombustible / (tiempoCombustible + delayCalculoCombustible));
            porcentajeCombustible.text = (porcentaje * 100).ToString("0") + "%";
        }
    }

    void ActualizarIconoCombustible()
    {
        switch (indexMJCompletado)
        {
            case 0:
                RellenarCombustible(cablesIzquierdaOn);
                break;
            case 1:
                RellenarCombustible(cableCentroOn);
                break;
            case 2:
                RellenarCombustible(cablesDerechaOn);
                break;
        }
    }

    void TiempoCombustible(Image[] cable) // Calcula el tiempo que debe tardar el combustible en rellenarse para coincidir con el flujo de datos
    {
        if (cable[cable.Length - 1].fillAmount >= 1 && tiempoCombustible == -1)
        {
            if (contadorMJ < tiempoDePorcentaje)
            {
                tiempoCombustible = (tiempoDePorcentaje - contadorMJ) + tiemposPorCable.Sum();
            }
            else
            {
                tiempoCombustible = tiemposPorCable.Sum();
            }
        }
    }
    #endregion Animacion iconos

    #region Animacion cables
    void Cables(Image[] imagenes, float contador)
    {
        for (int i = 0; i < imagenes.Length; i++)
        {
            Image img = imagenes[i];

            if (i > 0)
            {
                if (imagenes[i - 1].fillAmount >= 1 && img.fillAmount < 1)
                {
                    img.fillAmount = Mathf.Lerp(0, 1, (contador - TiempoHastaAhora(i)) / tiemposPorCable[i]);
                }
            }
            else
            {
                if (img.fillAmount < 1)
                {
                    img.fillAmount = Mathf.Lerp(0, 1, contador / tiemposPorCable[i]);
                }
            }
        }
    }

    float LongitudCable(Image img)
    {
        RectTransform rectTransform = img.GetComponent<RectTransform>();

        if (img.fillMethod == Image.FillMethod.Vertical)
        {
            return rectTransform.rect.height;
        }
        else if (img.fillMethod == Image.FillMethod.Horizontal)
        {
            return rectTransform.rect.width;
        }

        return 0f;
    }

    float[] TiemposPorCable()
    {
        Image[] cable = new Image[0];

        switch (indexMJCompletado)
        {
            case 0:
                cable = cablesIzquierdaOn;
                break;
            case 1:
                cable = cableCentroOn;
                break;
            case 2:
                cable = cablesDerechaOn;
                break;
        }

        float[] tiempos = new float[cable.Length];

        for (int i = 0; i < cable.Length; i++)
        {
            tiempos[i] = LongitudCable(cable[i]) / velocidadCables;
        }

        return tiempos;
    }

    float TiempoHastaAhora(int index)
    {
        float tiempo = 0;

        for (int i = 0; i < index; i++)
        {
            tiempo += tiemposPorCable[i];
        }

        return tiempo;
    }

    void ActivarCables(bool activar)
    {
        if (activar) contadorCableOn += Time.deltaTime;
        else contadorCableOff += Time.deltaTime;

        switch (indexMJCompletado)
        {
            case 0:
                if (activar)
                {
                    Cables(cablesIzquierdaOn, contadorCableOn);
                    TiempoCombustible(cablesIzquierdaOn);
                }
                else Cables(cablesIzquierdaOff, contadorCableOff);

                break;
            case 1:
                if (activar)
                {
                    Cables(cableCentroOn, contadorCableOn);
                    TiempoCombustible(cableCentroOn);
                }
                else Cables(cableCentroOff, contadorCableOff);

                break;
            case 2:
                if (activar)
                {
                    Cables(cablesDerechaOn, contadorCableOn);
                    TiempoCombustible(cablesDerechaOn);
                }
                else Cables(cablesDerechaOff, contadorCableOff);

                break;
        }
    }
    #endregion Animacion cables
}