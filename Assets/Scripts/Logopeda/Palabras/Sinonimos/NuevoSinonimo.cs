using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NuevoSinonimo : MonoBehaviour
{
    SQLQuery query;

    [Header("Interfaz cambio sinónimo")]
    public Button botonConfirmar;
    public Transform contenidoParent;
    public GameObject sinonimoPrefab;
    public InputField inputUsuario;
    Text cuentaSinonimos;

    [Header("Cambiar consulta para buscar antónimos")]
    public bool isAntonimo;
    int sinonimoInt;

    // Variables registro
    int id;
    List<string[]> sinonimosBDD = new List<string[]>();
    List<string> sinonimos = new List<string>();

    void Start()
    {
        query = new SQLQuery("BaseLogopeda");

        botonConfirmar.onClick.AddListener(IntroducirSinonimo);

        if (!isAntonimo) sinonimoInt = 1;
        else sinonimoInt = 0;
    }

    public void ActualizarInterfaz(int id, Text txt)
    {
        if (sinonimos.Count > 0) sinonimos.Clear();

        for (int i = 0; i < contenidoParent.childCount; i++)
        {
            Destroy(contenidoParent.GetChild(i).gameObject);
        }

        query.Query("SELECT Palabras FROM SinonimosAntonimos WHERE ID_Palabra = " + id + " AND Sinonimo = " + sinonimoInt);

        sinonimosBDD = query.StringArrayReader(1);
        if (sinonimosBDD.Count > 0) sinonimos.AddRange(sinonimosBDD[0]);

        for (int i = 0; i < sinonimos.Count; i++)
        {
            GameObject sinonimo = Instantiate(sinonimoPrefab, contenidoParent);
            sinonimo.GetComponentInChildren<Text>().text = sinonimos[i];
        }

        this.id = id;
        cuentaSinonimos = txt;
    }

    void IntroducirSinonimo()
    {
        GameObject sinonimo = Instantiate(sinonimoPrefab, contenidoParent);
        sinonimo.GetComponentInChildren<Text>().text = inputUsuario.text;

        sinonimos.Add(inputUsuario.text);
        cuentaSinonimos.text = sinonimos.Count.ToString();

        string sinonimosaBDD = "";

        for (int i = 0; i < sinonimos.Count; i++)
        {
            if (i < sinonimos.Count - 1) sinonimosaBDD += sinonimos[i] + "/";
            else sinonimosaBDD += sinonimos[i];
        }

        if (sinonimosBDD.Count > 0)
            query.Query("UPDATE SinonimosAntonimos SET Palabras = '" + sinonimosaBDD + "' WHERE ID_Palabra = " + id + " AND Sinonimo = " + sinonimoInt);
        else
            query.Query("INSERT INTO SinonimosAntonimos VALUES (" + id + ", '" + sinonimosaBDD + "' ," + sinonimoInt + ")");
    }
}
