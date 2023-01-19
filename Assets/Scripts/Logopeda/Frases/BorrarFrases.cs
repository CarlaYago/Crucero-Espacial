using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BorrarFrases : MonoBehaviour
{
    SQLQuery consultas;

    [Header("Panel borrar frases")]
    public Text titulo;
    public TextMeshProUGUI fraseUI;
    public Button botonConfirmar;
    public GameObject panelEliminar;
    string tituloText;

    void Start()
    {
        consultas = new SQLQuery("BaseLogopeda");
        tituloText = titulo.text;
    }

    public void PanelEliminar(Text frase)
    {
        int id = frase.GetComponent<Identificador>().id;
        AbrirPanel(id, frase.text);
        botonConfirmar.onClick.AddListener(delegate { EliminarFrase(frase); });
    }

    void AbrirPanel(int id, string frase)
    {
        titulo.text = tituloText + id;
        fraseUI.text = frase;

        botonConfirmar.onClick.RemoveAllListeners();

        panelEliminar.SetActive(true);
    }

    void EliminarFrase(Text frase)
    {
        Destroy(frase.transform.parent.gameObject);

        consultas.Query("SELECT ID_Frase FROM FrasesTexto WHERE Frase = '" + frase.text + "'");
        int id = consultas.IntReader(1)[0];

        consultas.Query("DELETE FROM FrasesTexto WHERE ID_Frase = " + id + "");
        consultas.Query("DELETE FROM Dificultad WHERE ID_Entrada = " + id + " AND Identificador = 'Frase'");

        panelEliminar.SetActive(false);
    }
}