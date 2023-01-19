using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Imagen : MonoBehaviour
{
    // Variables imagen
    RawImage img;
    RectTransform rect;
    Button btn;
    string palabra;

    // Variables cambio imagen
    public string refPanelImagen, rutaBotonAplicar;
    GameObject ventana;
    Button cambiarImagen;

    Diccionario jugador;

    void Start()
    {
        img = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();
        palabra = transform.parent.parent.GetChild(0).GetComponent<Text>().text;

        btn = GetComponent<Button>();
        btn.onClick.AddListener(CambiarImagen);

        jugador = FindObjectOfType<Diccionario>();
    }

    public void CambiarImagen() // Se le llama al pulsar sobre la imagen
    {
        if (ventana == null) ventana = GameObject.Find(refPanelImagen).transform.GetChild(0).gameObject;
        ventana.SetActive(true);
        if (cambiarImagen == null) cambiarImagen = ventana.transform.Find(rutaBotonAplicar).GetComponent<Button>();
        cambiarImagen.onClick.AddListener(AplicarImagen);
    }

    void AplicarImagen() // Se le llama al pulsar en el boton de aplicar imagen, una vez elegida la imagen nueva
    {
        // Aplicar imagen del PC usuario a la fila de la interfaz
        string filePath = ventana.GetComponentInChildren<InputField>().text;
        Vector2 size = new Vector2(rect.rect.width, rect.rect.height);
        img.texture = CargarImagen(size, filePath);
        // img.texture = CargarImagen(size, Path.GetFullPath(fileName)); -> manera de forzar ruta concreta

        cambiarImagen.onClick.RemoveListener(AplicarImagen);
        ventana.SetActive(false);

        // Guardar imagen en el diccionario
        List<PalabrasDisponibles> dicc = jugador.diccionario;

        for (int i = 0; i < dicc.Count; i++)
        {
            if (dicc[i].palabra == palabra)
            {
                PalabrasDisponibles registro = new PalabrasDisponibles
                {
                    palabra = dicc[i].palabra,
                    imagen = (Texture2D)img.texture,
                    dificultad = dicc[i].dificultad
                };

                dicc[i] = registro;
            }
        }
    }

    private static Texture2D CargarImagen(Vector2 size, string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;
        texture.LoadImage(bytes);

        return texture;
    }
}