using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NuevaCategoria : MonoBehaviour
{
    SQLQuery query;

    [Header("Interfaz cambio categoría")]
    public Button botonNuevaCategoria;
    public Transform contenidoParent;
    public GameObject categoriaPrefab;
    public InputField inputUsuario;
    public Color colorSeleccionada;
    Color colorPorDefecto;

    // Variables registro
    int idPalabra;
    Text categoria, categoriaUI;
    string textoCategoria;
    List<string> categorias;

    void Start()
    {
        query = new SQLQuery("BaseLogopeda");

        botonNuevaCategoria.onClick.AddListener(IntroducirCategoria);

        colorPorDefecto = categoriaPrefab.GetComponent<Image>().color;

        query.Query("SELECT Categoria FROM Categorias");
        categorias = query.StringReader(1);

        for (int i = 0; i < categorias.Count; i++)
        {
            GameObject categoria = Instantiate(categoriaPrefab, contenidoParent);
            categoria.GetComponentInChildren<Text>().text = categorias[i];
        }

        categoriaUI = transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();
        textoCategoria = categoriaUI.text;
    }

    public void ActualizarInterfaz(int id, Text txt)
    {
        idPalabra = id;
        categoria = txt;

        CategoriaElegida(categoria.text, false);
    }

    void IntroducirCategoria()
    {
        if (inputUsuario.text != "")
        {
            GameObject categoriaNueva = Instantiate(categoriaPrefab, contenidoParent);
            categoriaNueva.GetComponentInChildren<Text>().text = inputUsuario.text;

            query.Query("INSERT INTO Categorias (Categoria) VALUES ('" + inputUsuario.text + "')");
        }
    }

    public void CategoriaElegida(string categoria, bool actualizarBDD)
    {
        categoriaUI.text = textoCategoria + "<b>" + categoria + "</b>";
        this.categoria.text = categoria;

        for (int i = 0; i < contenidoParent.childCount; i++)
        {
            string categoriaListada = contenidoParent.GetChild(i).GetComponentInChildren<Text>().text;
            Image categoriaRect = contenidoParent.GetChild(i).GetComponentInChildren<Image>();

            if (categoriaListada == categoria)
            {
                categoriaRect.color = colorSeleccionada;
            }
            else if (categoriaRect.color == colorSeleccionada)
            {
                categoriaRect.color = colorPorDefecto;
            }
        }

        if (actualizarBDD)
        {
            query.Query("SELECT ID_Categoria FROM Categorias WHERE Categoria = '" + categoria + "'");
            List<int> id_Categoria = query.IntReader(1);

            query.Query("UPDATE Palabras SET ID_Categoria = " + id_Categoria[0] + " WHERE ID_Palabra = " + idPalabra);
        }
    }
}
