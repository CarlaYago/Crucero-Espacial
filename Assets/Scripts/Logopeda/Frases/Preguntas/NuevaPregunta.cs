using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NuevaPregunta : MonoBehaviour
{
    SQLQuery query;

    [Header("Interfaz cambio preguntas")]
    public Transform contenidoParent;
    public GameObject preguntaPrefab;
    public TextMeshProUGUI fraseUI;
    public Color colorSeleccionado;
    Color colorPorDefecto;

    [Header("Interfaz pregunta nueva")]
    public Button botonNuevaPregunta;
    public TMP_InputField inputUsuario;

    // Variables registro
    int idFrase;
    Text numPreguntas;
    List<string> preguntas = new List<string>();
    List<string> respuestas = new List<string>();

    void Start()
    {
        query = new SQLQuery("BaseLogopeda");

        botonNuevaPregunta.onClick.AddListener(IntroducirPregunta);

        colorPorDefecto = preguntaPrefab.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;

        query.Query("SELECT Pregunta, Respuesta FROM Preguntas WHERE ID_Frase = " + idFrase);
        preguntas = query.StringReader(1);
        respuestas = query.StringReader(2);

        for (int i = 0; i < preguntas.Count; i++)
        {
            GameObject pregunta = Instantiate(preguntaPrefab, contenidoParent);
            pregunta.GetComponentInChildren<TMP_InputField>().text = preguntas[i];

            Transform buttons = pregunta.transform.GetChild(1);
            if (respuestas[i] == "SI") buttons.GetChild(0).GetComponent<Image>().color = colorSeleccionado;
            else buttons.GetChild(1).GetComponent<Image>().color = colorSeleccionado;
        }
    }

    public void ActualizarInterfaz(int id, Text txt)
    {
        idFrase = id;
        numPreguntas = txt;

        VaciarPreguntas();
        LeerPreguntas();
    }

    void LeerPreguntas()
    {
        query.Query("SELECT Frase FROM FrasesTexto WHERE ID_Frase = " + idFrase);
        string frase = query.StringReader(1)[0];
        fraseUI.text = frase;

        query.Query("SELECT Pregunta, Respuesta FROM Preguntas WHERE ID_Frase = " + idFrase);
        preguntas = query.StringReader(1);
        respuestas = query.StringReader(2);

        for (int i = 0; i < preguntas.Count; i++)
        {
            GameObject pregunta = Instantiate(preguntaPrefab, contenidoParent);
            pregunta.GetComponentInChildren<TMP_InputField>().text = preguntas[i];

            Transform buttons = pregunta.transform.GetChild(1);
            if (respuestas[i] == "SI") buttons.GetChild(0).GetComponent<Image>().color = colorSeleccionado;
            else buttons.GetChild(1).GetComponent<Image>().color = colorSeleccionado;

            for (int n = 0; n < buttons.childCount; n++)
            {
                Button btn = buttons.GetChild(n).GetComponent<Button>();
                Image img = buttons.GetChild(n).GetComponent<Image>();

                if (n == 0)
                {
                    btn.onClick.AddListener(delegate { CambiarRespuesta(img, "SI"); });
                }
                else
                {
                    btn.onClick.AddListener(delegate { CambiarRespuesta(img, "NO"); });
                }
            }
        }
    }

    void VaciarPreguntas()
    {
        foreach (Transform child in contenidoParent)
        {
            Destroy(child.gameObject);
        }
    }

    void IntroducirPregunta()
    {
        query.Query("INSERT INTO Preguntas (Pregunta, ID_Frase) VALUES ('" + inputUsuario.text.ToUpper() + "', " + idFrase + ")");
        query.Query("SELECT Respuesta FROM Preguntas WHERE Pregunta = '" + inputUsuario.text.ToUpper() + "'");
        string respuestaDefault = query.StringReader(1)[0];

        GameObject pregunta = Instantiate(preguntaPrefab, contenidoParent);
        pregunta.GetComponentInChildren<TMP_InputField>().text = inputUsuario.text.ToUpper();

        Transform buttons = pregunta.transform.GetChild(1);
        if (respuestaDefault == "SI") buttons.GetChild(0).GetComponent<Image>().color = colorSeleccionado;
        else buttons.GetChild(1).GetComponent<Image>().color = colorSeleccionado;

        for (int n = 0; n < buttons.childCount; n++)
        {
            Button btn = buttons.GetChild(n).GetComponent<Button>();
            Image img = buttons.GetChild(n).GetComponent<Image>();

            if (n == 0)
            {
                btn.onClick.AddListener(delegate { CambiarRespuesta(img, "SI"); });
            }
            else
            {
                btn.onClick.AddListener(delegate { CambiarRespuesta(img, "NO"); });
            }
        }

        int preguntasTotales = int.Parse(numPreguntas.text);
        numPreguntas.text = (preguntasTotales + 1).ToString();
    }

    void CambiarRespuesta(Image self, string respuestaNueva)
    {
        Transform parent = self.transform.parent;

        string pregunta = parent.parent.GetChild(0).GetComponent<TMP_InputField>().text;
        query.Query("UPDATE Preguntas SET Respuesta = '" + respuestaNueva + "' WHERE Pregunta = '" + pregunta + "'");

        foreach (Transform child in parent)
        {
            child.GetComponent<Image>().color = colorPorDefecto;
        }
        self.color = colorSeleccionado;
        Debug.Log(respuestaNueva);
    }
}