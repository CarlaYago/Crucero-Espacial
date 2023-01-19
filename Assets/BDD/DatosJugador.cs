using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatosJugador : MonoBehaviour
{
    SQLQuery consultas;

    [Header("Modo testeo")]
    public bool modoDebug;

    [Header("Información usuario")]
    public int id;
    public string nombre;
    public int gasolina;
    public float experiencia;

    [Header("Configuración planetario")]
    public List<float> distancias;
    public List<int> dificultades;
    public Vector4 tiempoDeEspera = new Vector4(0, 0, 0, 30);
    public bool regenerarRondas;

    [Header("Información almacén")]
    #region Datos 
    public List<int> palabraID_BDD;
    public List<int> palRareza_BDD, palCantidad_BDD;
    public List<bool> palNueva_BDD;
    #endregion Datos

    [Header("Información colección")]
    #region Datos 
    public Dictionary<int, int> catNivel_BDD;
    public Dictionary<int, int[]> catPalabrasID_BDD = new Dictionary<int, int[]>();
    #endregion Datos

    [Header("Información planetas")]
    #region Datos 
    public Dictionary<int, DateTime> plaUltimaVisita_BDD;
    public Dictionary<int, bool> plaFavorito_BDD;
    public int idPlanetaActual;
    #endregion Datos

    [Header("Información minijuegos")]
    public List<Minijuegos> minijuegos;
    #region Datos 
    public int[] minijuegosElegidos, minijuegosRondas;
    public int minijuegoDificultad, minijuegoActualIndex;
    #endregion Datos

    [Header("Progreso minijuegos")]
    public float[] minijuegosPorcentajes;
    public List<bool> minijuegoRecienCompletado;

    [Header("Información recompensas")]
    public List<string> recompensas;
    public List<int> rarezasRecompensas;
    public int minijuegoCombustible;

    [Header("Modo libre")]
    public bool modoLibre;
    public bool menuPrincipal;

    [Header("Transicion Color")]
    public List<Color> colores;

    [Header("Cinemática")]
    public bool cinematicaVista;

    void Awake() // DontDestroy 
    {
        DatosJugador[] objs = FindObjectsOfType<DatosJugador>();

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        consultas = new SQLQuery("Usuarios");
    }

    #region Acceso a PlayerPrefs

    /// <param name="ddhhmmss"> Nombre de las variables en PlayerPrefs para días, horas, minutos y segundos en ese órden. </param>
    /// /// <param name="valoresPorDefecto"> Variable donde guardar la información del tiempo de espera, y referencia para el valor inicial </param>
    public void AccederTiempoEspera(string[] ddhhmmss, ref Vector4 valoresPorDefecto)
    {
        int dias = PlayerPrefs.GetInt(ddhhmmss[0], Mathf.RoundToInt(valoresPorDefecto.x));
        int horas = PlayerPrefs.GetInt(ddhhmmss[1], Mathf.RoundToInt(valoresPorDefecto.y));
        int minutos = PlayerPrefs.GetInt(ddhhmmss[2], Mathf.RoundToInt(valoresPorDefecto.z));
        int segundos = PlayerPrefs.GetInt(ddhhmmss[3], Mathf.RoundToInt(valoresPorDefecto.w));

        valoresPorDefecto = new Vector4(dias, horas, minutos, segundos);
    }

    public int RecibirValor(string nombre)
    {
        return PlayerPrefs.GetInt(nombre);
    }

    #endregion Acceso a PlayerPrefs

    #region Acceso a BDD

    public void AccederInfoAlmacen()
    {
        consultas.Query("SELECT * FROM Almacen_" + id);
        palabraID_BDD = consultas.IntReader(1);
        palRareza_BDD = consultas.IntReader(2);
        palCantidad_BDD = consultas.IntReader(3);
        palNueva_BDD = consultas.BoolReader(4);
    }

    public void AccederInfoColeccion()
    {
        consultas.Query("SELECT * FROM Coleccion_" + id + " WHERE Rareza IS NOT NULL");
        catNivel_BDD = consultas.IntReaderID(1, 2);
        consultas.Query("SELECT * FROM Coleccion_" + id + " WHERE ID_Palabras IS NOT NULL");
        catPalabrasID_BDD = consultas.IntArrayReaderID(1, 3);
    }

    public void AccederInfoPlanetas()
    {
        consultas.Query("SELECT * FROM Planetario_" + id + " WHERE UltimaVisita IS NOT NULL");
        plaUltimaVisita_BDD = consultas.DateTimeReaderID(1, 2);
        consultas.Query("SELECT * FROM Planetario_" + id + " WHERE Favorito IS NOT NULL");
        plaFavorito_BDD = consultas.BoolReaderID(1, 3);
    }

    #endregion Acceso a BDD

    #region Rellenar BDD

    public void ReclamarRecompensas() // (Actualizar Almacen)
    {
        if (recompensas.Count > 0)
        {
            consultas = new SQLQuery("BaseLogopeda");
            List<int> idPalabras = new List<int>();

            for (int i = 0; i < recompensas.Count; i++)
            {
                consultas.Query("SELECT ID_Palabra FROM Palabras WHERE Palabra = '" + recompensas[i] + "'");
                int id = consultas.IntReader(1)[0];
                idPalabras.Add(id);
            }

            consultas = new SQLQuery("Usuarios");

            for (int i = 0; i < recompensas.Count; i++)
            {
                consultas.Query("SELECT Cantidad FROM Almacen_" + id + " WHERE ID_Palabra = " + idPalabras[i] + " AND Rareza = " + rarezasRecompensas[i]);
                if (consultas.Count() > 0)
                {
                    int cantidad = consultas.IntReader(1)[0];
                    consultas.Query("UPDATE Almacen_" + id + " SET Cantidad = " + (cantidad + 1) + ", Nueva = 1 WHERE ID_Palabra = " + idPalabras[i] + " AND Rareza = " + rarezasRecompensas[i]);
                }
                else
                {
                    consultas.Query("INSERT INTO Almacen_" + id + " VALUES(" + idPalabras[i] + "," + rarezasRecompensas[i] + ", 1, 1)");
                }
            }

            recompensas.Clear();
            rarezasRecompensas.Clear();
        }

        gasolina += minijuegoCombustible;
        consultas.Query("UPDATE Usuarios SET Gasolina = " + gasolina + " WHERE ID_Usuario = " + id);

        AccederInfoAlmacen();
    }

    public void ActualizarColeccion(int idCategoria, int idPalabraNueva)
    {
        consultas.Query("SELECT ID_Palabras FROM Coleccion_" + id + " WHERE ID_Categoria = " + idCategoria);

        if (consultas.Count() > 0) // Si la categoría aparece en BDD (ha sido rellenada antes)
        {
            // Acceder a las palabras que tiene la categoría en ese momento
            string idPalabras = consultas.StringReader(1)[0];

            // Añadir la ID nueva a las otras IDs (en el formato ID1/ID2/...)
            if (idPalabras != null) idPalabras += "/";
            idPalabras += idPalabraNueva;

            // Actualizar BDD
            consultas.Query("UPDATE Coleccion_" + id + " SET ID_Palabras = '" + idPalabras + "' WHERE ID_Categoria = " + idCategoria);

            // Actualizar DatosJugador
            string[] idsString = idPalabras.Split('/');
            int[] ids = Array.ConvertAll(idsString, s => int.Parse(s));
            catPalabrasID_BDD[idCategoria] = ids;

        }
        else // Si la categoria no aparece en BDD (estaba vacía)
        {
            // Introducir en BDD
            consultas.Query("INSERT INTO Coleccion_" + id + " VALUES (" + idCategoria + "," + 0 + "," + idPalabraNueva + ")");
            int[] ids = new int[] { idPalabraNueva };

            // Introducir a DatosJugador
            catPalabrasID_BDD.Add(idCategoria, ids);
            catNivel_BDD.Add(idCategoria, 0);
        }
    }

    public void SubirNivelCategoria(int idCategoria)
    {
        consultas.Query("SELECT Rareza FROM Coleccion_" + id + " WHERE ID_Categoria = " + idCategoria);

        int rareza = consultas.IntReader(1)[0];
        if (rareza < 4) rareza++;

        // Actualizar BDD
        consultas.Query("UPDATE Coleccion_" + id + " SET Rareza = " + rareza + ", ID_Palabras = NULL WHERE ID_Categoria = " + idCategoria);

        // Actualizar DatosJugador
        catNivel_BDD[idCategoria] = rareza;
    }

    public void ActualizarVisitaPlanetas()
    {
        DateTime fecha = DateTime.Now;
        var culture = new CultureInfo("en-GB");
        string fechaTexto = fecha.ToString(culture);

        plaUltimaVisita_BDD[idPlanetaActual] = fecha;
        consultas.Query("UPDATE Planetario_" + id + " SET UltimaVisita = '" + fechaTexto + "' WHERE Planeta_ID = " + idPlanetaActual);
    }

    public void ActualizarGasolina(int gasolina)
    {
        this.gasolina = gasolina;
        consultas.Query("UPDATE Usuarios SET Gasolina = " + gasolina + " WHERE ID_Usuario = " + id);
    }

    #endregion Rellenar BDD
}