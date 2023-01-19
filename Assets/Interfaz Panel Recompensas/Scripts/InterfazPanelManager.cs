using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InterfazPanelManager : MonoBehaviour
{
    SQLQuery consultas;

    [Header("Rueda Reconocimiento")]

    public GameObject circuloRendimiento;
    public GameObject textoPorcentaje;
    public GameObject textReconocimiento;

    [Range(0, 1)] public float num; //Porcentaje del minijuego superado

    public float velocidadRelleno = 1;
    public Color[] coloresRueda; //Colores para cambiar la rueda de reconocimiento según el porcentaje

    [Header("Palabras Desbloqueadas")]

    public GameObject textoPalabras;

    public int numPalabras;

    [Header("Barra Experiencia")]

    public ExperienciaManager exManager;

    public Slider barraEXP;

    public GameObject textoExp;

    public float numEXP; //Puntos de experiencia
    public float numExpTotal; //todos los puntos de experiencia que tiene el jugador

    public float puntuacionBarra; //Puntuación anterior de la barra de experiencia
    public float valor; //Valor máximo de la barra de experiencia

    public GameObject textoNivel;

    [Header("Audio")]
    AudioSource reproductor1;
    AudioSource reproductor2;
    AudioSource reproductor3;
    AudioClip RecompensaSonido, RecompensaSonido_mal;
    AudioClip levelUp, experienceSound, porcentajeSound;

    [Header("Mensaje Level Up")]

    public GameObject subirNivel;

    [Header("Botón")]

    public GameObject bot;

    int numNivel; // Nivel del jugador

    public List<int> nadaOExtra = new List<int>();

    float num3;
    float num4;
    float num5 = 0;
    float puntuacionBarra2 = 0;
    float diferencia;
    float num2EXP = 0;
    float diferencia2;//es para guardar cuanta experiencia as gastado para subir un nivel anterior
    float diferencia3 =0; //lo mismo que la 2 pero no se resetea al suvir de nivel

    bool corrutina = true;
    bool exp = false;
    bool siguienteNivel = true;
    bool vSiguienteNivel = true;
    bool vSig = true;
    bool doOnce = true;
    bool sonidoNoReproducido = true;
    DatosJugador datosScript;
    void Start()
    {

        datosScript = FindObjectOfType<DatosJugador>();
        CalcularNivel();
        AsignarPorcentaje();
        //Cambiar valor máximo de la barra de experiencia
        barraEXP.GetComponent<Slider>().maxValue = valor;
        exManager = new ExperienciaManager(num);// si aquí hay un + algo significa que me lo dejé sin querer, quitalo plis ;)
        numEXP = exManager.SumaExp(nadaOExtra);
        if (datosScript.modoLibre)
        {
            datosScript.recompensas.Clear();
            datosScript.rarezasRecompensas.Clear();
        }
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        //
        reproductor1 = audioManager.AudioSource1;
        reproductor2 = audioManager.AudioSource2;
        reproductor3 = audioManager.AudioSource3;
        //
        RecompensaSonido = audioManager.RecompensaSonido;
        RecompensaSonido_mal = audioManager.RecompensaSonido_mal;
        levelUp = audioManager.SubirNivel;
        experienceSound = audioManager.Experiencia;
        porcentajeSound = audioManager.PorcentajeSubiendo;

        RecompensaBuena_Mala_Porcentaje();
    }

    public void AsignarPorcentaje()
    {
        barraEXP.GetComponent<Slider>().value = puntuacionBarra;
        num4 = num * 100; //Porcentaje

    }
    public void playSound(AudioClip sonido)
    {
        reproductor1.clip = sonido;
        reproductor1.Play();
    }

    public void SonidoLevelUp(AudioClip sonido)
    {
        reproductor2.clip = sonido;
        reproductor2.Play();
    }

    //Esto es para otro posible sonido para el porcentaje. 
    /*public void SonidoPorcentaje(AudioClip sonido)
    {
        reproductor2.clip = sonido;
        reproductor2.Play();
    }*/

    public void SonidoExperiencia(AudioClip sonido)
    {
        reproductor3.clip = sonido;
        reproductor3.loop = false;
        reproductor3.Play();
    }

    //Si sale una puntuación a mejorar sonará el sonido de derrota
    public void RecompensaBuena_Mala_Porcentaje()
    {
        if (num4 <= 49.1f)
        {
            playSound(RecompensaSonido_mal);
        }
        else
        {
            playSound(RecompensaSonido);
        }

    }


    void Update()
    {
        if (doOnce)//se que parece que no sirva de nada esto, pero si lo quito no funciona bien XD
        {
            barraEXP.GetComponent<Slider>().value = puntuacionBarra;
            SonidoExperiencia(experienceSound);
            doOnce = false;
        }
        //Suma para que se vaya rellenando progresivamente el círculo de rendimiento
        num3 += Time.deltaTime / velocidadRelleno;

        //Suma para que se vaya subiendo progresivamente el porcentaje
        num5 += Time.deltaTime * 100 / velocidadRelleno;

        //Relleno del círculo de rendimiento
        circuloRendimiento.GetComponent<Image>().fillAmount = num3;
        


        //Cambio de color del círculo de rendimiento y fuente de porcentaje
        circuloRendimiento.GetComponent<Image>().color = CambiarColor(coloresRueda, num3);
        textoPorcentaje.GetComponent<TextMeshProUGUI>().color = CambiarColor(coloresRueda, num3);

        //Si la variable del porcentaje llega al valor correspondiente, la suma para
        if (num5 >= num4)
        {
            num5 = num4;

            ////Si el porcentaje es 100% cambia de color y tipo de fuente
            //if (num5 == 100)
            //{
            //    textoPorcentaje.GetComponent<TextMeshProUGUI>().color = darkGreen;
            //    //textoPorcentaje.GetComponent<TextMeshProUGUI>().fontStyle = (FontStyles)FontStyle.Bold;
            //}

            //Empieza la corrutina
            if (corrutina == true)
            {
                StartCoroutine("aparicion");
            }

        }

        //Texto del porcentaje
        textoPorcentaje.GetComponent<TextMeshProUGUI>().text = num5.ToString("0") + "%";

        //Si el valor de la imagen llega al valor correspondiente, la suma para
        if (num3 >= num)
        {
            num3 = num;
        }

        //Si la variable exp es true, empieza a sumar la experiencia de la barra de la experiencia
        if (exp == true)
        {
            if (sonidoNoReproducido)
            {         
                SonidoExperiencia(experienceSound);
                sonidoNoReproducido = false;
            }

            //Si el número de la experiencia llega al valor correspondiente, la suma para
            if (num2EXP >= numEXP)
            {
                num2EXP = numEXP;
            }

            //Texto de la experiencia
            textoExp.GetComponent<TextMeshProUGUI>().text = "Experiencia +" + num2EXP.ToString("0");

            //Suma para que vaya subiendo progresivamente el número de experiencia
            num2EXP += Time.deltaTime * valor/5;

            //Suma para que se vaya rellenando progresivamnete la barra de experiencia
            puntuacionBarra2 += Time.deltaTime * valor /5;

            //Si el valor de la barra llega al valor correspondiente la suma para (si el jugador no ha subido de nivel)
            if (puntuacionBarra2 >= numEXP && vSiguienteNivel == true)
            {
                num2EXP = numEXP;
                puntuacionBarra2 = numEXP;
                exp = false;
                StartCoroutine("boton"); //Empieza la corrutina
            }

            //Si el valor de la barra llega al valor correspondiente la suma para (si el jugador ha subido de nivel)
            if (puntuacionBarra2+diferencia3 >= numEXP && vSiguienteNivel == false)
            {
                num2EXP = numEXP;
                //puntuacionBarra2 = diferencia2;
                textoExp.GetComponent<TextMeshProUGUI>().text = "Experiencia +" + num2EXP.ToString("0");
                exp = false;
                StartCoroutine("boton"); //Empieza la corrutina
            }



            //Si la barra llega a su valor máximo, el valor máximo cambia, la barra se reinicia y empieza a contar desde 0 hasta llegar al vlaor correspondiente
            if (puntuacionBarra + puntuacionBarra2 >= valor /*barraEXP.GetComponent<Slider>().maxValue*/)
            {
                diferencia2 = numEXP + puntuacionBarra - valor;
                diferencia3 += valor - puntuacionBarra;
                //Multiplica el valor máximo de la barra
                if (numNivel > 2 && numNivel < 5)
                {
                    valor = 1200;
                }
                if (numNivel > 4 && numNivel < 7)
                {
                    valor = 2400;
                }
                if (numNivel > 6 && numNivel < 102)
                {
                    valor = 3600;
                }
                if (numNivel > 100 && numNivel < 302)
                {
                    valor += 12;
                }
                //Empieza la corrutina
                if (siguienteNivel)
                {
                    StartCoroutine("SubirNivel");
                    SonidoLevelUp(levelUp);
                    siguienteNivel = false;
                }
                //Cambiar valor máximo de la barra de experiencia
                barraEXP.GetComponent<Slider>().maxValue = valor;
                //Resta que calcula la experiencia que falta por sumar a la barra de experiencia
                //diferencia = valor - puntuacionBarra; //Resta para calcular la experiencia que se necesita para subir al siguiente nivel
                vSiguienteNivel = false;
            }
            //Valor de la barra de experiencia (sumando la puntuación que ya tenía el jugador + la puntuación que ha obtenido)
            //barraEXP.GetComponent<Slider>().value = puntuacionBarra + puntuacionBarra2;
        }
        barraEXP.GetComponent<Slider>().value = puntuacionBarra + puntuacionBarra2;
        //Texto del nivel
        textoNivel.GetComponent<TextMeshProUGUI>().text = "NIVEL " + numNivel;
    }

    //Una vez que la rueda de reconocimiento a terminado la "animación" aparecen el resto de cosas poco a poco
    IEnumerator aparicion()
    {
        corrutina = false;
        yield return new WaitForSeconds(0.3f);

        #region Animacion Antigua
        //Si el valor del porcentaje es 100 hace una "animación"
        /*if (num4 == 100)
        {
            textoPorcentaje.GetComponent<TextMeshProUGUI>().fontSize = 81;
            yield return new WaitForSeconds(0.15f);
            textoPorcentaje.GetComponent<TextMeshProUGUI>().fontSize = 44;
            yield return new WaitForSeconds(0.15f);
            textoPorcentaje.GetComponent<TextMeshProUGUI>().fontSize = 81;
            yield return new WaitForSeconds(0.15f);
            textoPorcentaje.GetComponent<TextMeshProUGUI>().fontSize = 44;
            yield return new WaitForSeconds(0.15f);
            textoPorcentaje.GetComponent<TextMeshProUGUI>().fontSize = 185;
            textoPorcentaje.transform.Rotate(0, 0, -18.007f);
            yield return new WaitForSeconds(0.5f);
            textoPorcentaje.GetComponent<TextMeshProUGUI>().fontSize = 111;
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }*/
        #endregion Animacion Antigua

        //Se activa el texto de reconocimiento
        textReconocimiento.SetActive(true);

        //Cambio del mensaje según el porcentaje obtenido
        if (num4 >= 0 && num4 < 49.5f)
        {
            textReconocimiento.GetComponent<TextMeshProUGUI>().text = "¡HAY QUE MEJORAR!";
            //textReconocimiento.GetComponent<TextMeshProUGUI>().color = red;
        }
        else if (num4 >= 49.5f && num4 < 59.5f)
        {
            textReconocimiento.GetComponent<TextMeshProUGUI>().text = "¡NO ESTÁ MAL!";
            //textReconocimiento.GetComponent<TextMeshProUGUI>().color = orange;
        }
        else if (num4 >= 59.5f && num4 < 79.5)
        {
            textReconocimiento.GetComponent<TextMeshProUGUI>().text = "¡BIEN!";
            //textReconocimiento.GetComponent<TextMeshProUGUI>().color = yellow;
        }
        else if (num4 >= 79.5 && num4 < 99.5)

        {
            textReconocimiento.GetComponent<TextMeshProUGUI>().text = "¡BUEN TRABAJO!";
            //textReconocimiento.GetComponent<TextMeshProUGUI>().color = green;
        }
        else if (num4 >= 99.5)
        {
            textReconocimiento.GetComponent<TextMeshProUGUI>().text = "¡EXCELENTE!";
            //textReconocimiento.GetComponent<TextMeshProUGUI>().color = darkGreen;
        }
        textReconocimiento.GetComponent<TextMeshProUGUI>().color = circuloRendimiento.GetComponent<Image>().color;

        //"Animación" del texto de reconocimiento
        textReconocimiento.GetComponent<TextMeshProUGUI>().fontSize = 66;
        yield return new WaitForSeconds(1f);
        textReconocimiento.GetComponent<TextMeshProUGUI>().fontSize = 30;
        yield return new WaitForSeconds(0.1f);

        //Se activa el texto de palabras desbloqueadas
        if (numPalabras > 0 && !datosScript.modoLibre)
        {
            textoPalabras.SetActive(true);

            if (numPalabras == 1)
            {
                textoPalabras.GetComponent<TextMeshProUGUI>().text = "¡DESBLOQUEADA " + numPalabras + " PALABRA!";
            }
            else
            {
                textoPalabras.GetComponent<TextMeshProUGUI>().text = "¡DESBLOQUEADAS " + numPalabras + " PALABRAS!";
            }
        }

        //"Animación" del texto de palabras desbloqueadas
        textoPalabras.GetComponent<TextMeshProUGUI>().fontSize = 36;
        yield return new WaitForSeconds(1f);
        textoPalabras.GetComponent<TextMeshProUGUI>().fontSize = 20;

        //Se activa el texto de la experiencia
        textoExp.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        exp = true; //Se activa la experiencia en el Update
    }

    //Si el jugador sube de nivel, el numNivel sube de valor, la barra de experiencia se reinicia y se activa un mensaje que informa al jugador de que ha subido de nivel
    IEnumerator SubirNivel()
    {
        puntuacionBarra2 = 0;
        puntuacionBarra = 0;
        numNivel++;
        subirNivel.SetActive(true); //Se activa el mensaje
        yield return new WaitForSeconds(4f);
        subirNivel.SetActive(false); //Se desactiva el mensaje
        siguienteNivel = true;
    }

    //Una vez que la barra de experienia ha termiando de sumar puntos, se activa el botón de "Confirmar"
    IEnumerator boton()
    {
        yield return new WaitForSeconds(1f);
        bot.SetActive(true);
    }

    //Función para cambiar de escena al hacer clic en el botón "Confirmar"
    public void cambioEcena()
    {
        SceneManager.LoadScene("InterfazPrincipalRecompensa");
    }

    Color CambiarColor(Color[] colores, float t)
    {
        float tiempoPorColor = 1f / (colores.Length - 1);

        for (int i = 0; i < colores.Length - 1; i++)
        {
            if (t >= tiempoPorColor * i && t <= tiempoPorColor * (i + 1))
            {
                float diferencia = Mathf.Abs(tiempoPorColor * i - tiempoPorColor * (i + 1));
                float interpolado = (t - (tiempoPorColor * i)) / diferencia;

                Color col = Color.Lerp(colores[i], colores[i + 1], interpolado);
                return col;
            }
        }

        if (colores.Length > 1) return colores[colores.Length - 1];
        else return Color.clear;
    }

    public void Actualizar_BDD_Exp()
    {
        consultas = new SQLQuery("Usuarios");
        consultas.Query("SELECT Experiencia FROM Usuarios WHERE ID_Usuario = " + datosScript.id);
        int experiencia = consultas.IntReader(1)[0];

        consultas.Query("UPDATE Usuarios SET Experiencia = " + (experiencia + numEXP) + " WHERE ID_Usuario = " + datosScript.id);
        datosScript.experiencia = experiencia + numEXP;
    }
    void CalcularNivel()
    {
        numExpTotal = datosScript.experiencia;
        int suma12 = 12;
        int inicio = 3600;
        if (numExpTotal < 1200)
        {
            valor = 600;
            numNivel = ((int)((numExpTotal) / 600)) + 1;
            puntuacionBarra = numExpTotal % 600;
        }
        else if (numExpTotal >= 1200 && numExpTotal < 3600)
        {
            valor = 1200;
            numNivel = ((int)((numExpTotal - 1200) / 1200)) + 3;
            puntuacionBarra = (numExpTotal - 1200) % 1200;
        }
        else if (numExpTotal >= 3600 && numExpTotal < 8400)
        {
            valor = 2400;
            numNivel = ((int)((numExpTotal - 3600) / 2400)) + 5;
            puntuacionBarra = (numExpTotal - 3600) % 2400;
        }
        else if (numExpTotal >= 8400 && numExpTotal < 350400)
        {
            valor = 3600;
            numNivel = ((int)((numExpTotal - 8400) / 3600)) + 7;
            puntuacionBarra = (numExpTotal - 8400) % 3600;
        }
        else if (numExpTotal >= 350400 && numExpTotal < 1311600)
        {
            float resta = datosScript.experiencia - 350400;
            int registroLv = 0;
            while (resta > inicio + suma12)
            {
                valor = 3600 + suma12;
                resta -= (inicio + suma12);
                suma12 += 12;
                registroLv++;
            }
            numNivel = registroLv + 102;
            puntuacionBarra = resta;
        }
        else
        {
            valor = 6000;
            numNivel = ((int)((numExpTotal - 1311600) / 4000)) + 302;
            puntuacionBarra = (numExpTotal - 1311600) % 4000;
        }
    }
}