using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ManagerAlmacen : MonoBehaviour
{
    SQLQuery consultas;
    DatosJugador datos;

    public GameObject palabraPrefab;
    public Transform padre;
    public Scrollbar scroll;

    ManagerCartas cartas, cartas2;
    public bool parar;
    public List<bool> activados = new List<bool>(5);//que nadie quite esto de publico si no explota
    public List<Toggle> filtrosToggle = new List<Toggle>();
    //public Dropdown orden;

    List<GameObject> todasLasCartas = new List<GameObject>()/*, cartasordenadas*/;
    List<string> pruebas = new List<string>() { "MANZANA", "PERA", "COCHE", "MOTO" };
    List<string> abcdario = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", " " };
    int pos = 0;
    int numg = 0;

    void Start()
    {
        scroll.value = 1;
        Todo();
        LeerPalabras();
    }

    void LeerPalabras()
    {
        consultas = new SQLQuery("BaseLogopeda");
        datos = FindObjectOfType<DatosJugador>();

        List<int> idPalabras = datos.palabraID_BDD;

        for (int i = 0; i < idPalabras.Count; i++)
        {
            consultas.Query("SELECT Palabra FROM Palabras WHERE ID_Palabra = " + idPalabras[i]);

            string palabra = consultas.StringReader(1)[0];
            int nivel = datos.palRareza_BDD[i];
            int cantidad = datos.palCantidad_BDD[i];
            bool nueva = datos.palNueva_BDD[i];

            Instanciar(nivel, idPalabras[i], palabra, cantidad, nueva);
        }
    }

    public void Instanciar(int nivel, int id, string palabra, int cantidad, bool nueva)
    {
        for (int i = 0; i < padre.childCount; i++)
        {
            cartas = padre.GetChild(i).GetComponent<ManagerCartas>();

            if (cartas.nivel == nivel && cartas.palabra == palabra)
            {
                cartas.cantidad += cantidad;
                parar = true;
                cartas.ActuBarra();
            }
        }

        if (parar == false)
        {
            GameObject prefab = Instantiate(palabraPrefab, padre);
            todasLasCartas.Add(prefab);
            cartas = prefab.transform.GetComponent<ManagerCartas>();
            cartas.nivel = nivel;
            cartas.palabra = palabra;
            cartas.id = id;
            cartas.cantidad = cantidad;

            if (cartas.nivel >= 6)
            {
                cartas.levelear.interactable = false;
            }
        }
        cartas.nueva = nueva;
    }

    public void Filtrar(int rareza)
    {
        int n = 0;
        for (int i = 0; i < padre.childCount; i++)
        {
            cartas2 = padre.GetChild(i).GetComponent<ManagerCartas>();
            if (cartas2.nivel == rareza && filtrosToggle[rareza].isOn == true)
            {
                cartas2.gameObject.SetActive(true);

            }
            else if (filtrosToggle[cartas2.nivel].isOn == false)
            {
                cartas2.gameObject.SetActive(false);

            }
        }
        for (int e = 0; e < filtrosToggle.Count; e++)
        {
            if (filtrosToggle[e].isOn == true)
            {
                n++;
            }
        }
        if (n == 0)
        {
            QuitFiltro();
        }
    }

    public void QuitFiltro()
    {
        for (int e = 0; e < filtrosToggle.Count; e++)
        {
            filtrosToggle[e].isOn = false;
        }
        Debug.Log(padre.childCount);
        for (int i = 0; i < padre.childCount; i++)
        {
            cartas2 = padre.GetChild(i).GetComponent<ManagerCartas>();
            cartas2.gameObject.SetActive(true);
        }
    }

    public void Todo()
    {
        for (int e = 0; e < activados.Count; e++)
        {
            activados[e] = false;
        }
    }

    public void Buscar()
    {
        int z = 0;
        for (int i = 0; i < filtrosToggle.Count; i++)
        {
            if (filtrosToggle[i].isOn == true)
            {
                z++;
            }
        }
        if (z == filtrosToggle.Count)
        {
            QuitFiltro();
        }
    }

    public void DropdownValueChanged(int val)
    {
        if (val == 1)
        {
            for (int i = abcdario.Count - 1; i >= 0; i--)
            {
                Abcdario(i);
            }
            valoresa0();
        }
        if (val == 2)
        {
            for (int i = 0; i < abcdario.Count; i++)
            {
                Abcdario(i);
            }
            valoresa0();
        }
        if (val == 4)// rareza descendente
        {
            cambiolevel(0);
            cambiolevel(1);
            cambiolevel(2);
            cambiolevel(3);
            cambiolevel(4);
        }
        if (val == 3)//rareza ascendente
        {
            cambiolevel(4);
            cambiolevel(3);
            cambiolevel(2);
            cambiolevel(1);
            cambiolevel(0);
        }
    }

    void cambiolevel(int n)
    {
        int numg = 0;
        for (int i = 0; i < padre.childCount; i++)
        {
            if (padre.GetChild(i).GetComponent<ManagerCartas>().nivel == n)
            {
                padre.GetChild(i).GetComponent<ManagerCartas>().gameObject.transform.SetSiblingIndex(numg);
                numg++;
            }
        }
    }

    void Abcdario(int abc)
    {
        for (int i = 0; i < padre.childCount; i++)
        {
            if (padre.GetChild(i).GetComponent<ManagerCartas>().palabra.Substring(pos, pos + 1) == abcdario[abc])
            {

                padre.GetChild(i).GetComponent<ManagerCartas>().gameObject.transform.SetSiblingIndex(numg);
                numg++;
            }
        }
    }

    void valoresa0()
    {
        numg = 0;
    }
}