using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using SimpleFileBrowser;
using System.IO;
using System;

public class NuevaImagen : MonoBehaviour
{
    // Interfaz cambio imagen
    public Button boton;
    /// <summary>
    /// Vista previa de la imagen que se quiere cambiar
    /// </summary>
    public RawImage imagen;
    /// <summary>
    /// Imagen en el registro de datos asociada a una palabra 
    /// </summary>
    RawImage imagenACambiar;

    // Variables registro
    int id;

    void Start()
    {
        boton.onClick.AddListener(ElegirImagen);
        imagen.GetComponent<Button>().onClick.AddListener(ElegirImagen);
    }

    void ElegirImagen() // Se llama la pulsar sobre el botón de añadir imagen
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("", ".jpg", ".png"));
        FileBrowser.SetDefaultFilter(".jpg");

        StartCoroutine(AbrirExplorador());
    }

    IEnumerator AbrirExplorador()
    {
        // Mostrar el explorador de archivos y esperar una respuesta del usuario
        // Cargar archivos/carpetas: ambos, Permitir selección multiple: falso
        // Directorio inicial: Imágenes (Pictures), Nombre de archivo inicial: vacío
        // Nombre: "Explorador de archivos (.png, .jpg)", Texto botón enviar: "Cargar"
        string path = "C:\\Users\\" + Environment.UserName + "\\Pictures";
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, path, null, "Explorador de archivos (.png, .jpg)", "Cargar");

        if (FileBrowser.Success)
        {
            // Leer los bytes del archivo seleccionado
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            // Cargar los datos de imagen en una textura
            RectTransform imgSize = imagen.GetComponent<RectTransform>();
            Texture2D tex = new Texture2D((int)imgSize.rect.width, (int)imgSize.rect.height);
            tex.LoadImage(bytes);

            // Mostrar la textura en la interfaz
            imagen.texture = tex;
            if (!imagen.IsActive()) imagen.gameObject.SetActive(true);
            imagenACambiar.texture = tex;

            // Introducir en BDD
            SQLQuery query = new SQLQuery("BaseLogopeda");
            query.InsertImage("Palabras", "Imagen", bytes, id);
        }
    }

    // ActualizarInterfaz carga la imagen de la palabra seleccionada al panel de cambiar imagen
    // Se llama desde el script SetImagen, mediante el botón de imágen por cada palabra de la tabla
    public void ActualizarInterfaz(int id, RawImage img)
    {
        imagenACambiar = img;

        if (imagenACambiar.texture != null)
        {
            imagen.texture = imagenACambiar.texture;
            imagen.gameObject.SetActive(true);
        }
        else
        {
            imagen.gameObject.SetActive(false);
        }

        this.id = id;
    }
}