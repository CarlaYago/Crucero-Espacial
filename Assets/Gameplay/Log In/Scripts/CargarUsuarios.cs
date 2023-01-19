using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CargarUsuarios : MonoBehaviour
{
    SQLQuery consultasUsuarios, consultasBaseLogopeda;

    [Header("Referencias generales")]
    public Transform usuariosParent;
    public GameObject usuarioPrefab;
    public GameObject panelBotones;

    DatosJugador datos;

    [Header("Creación de jugador nuevo")]
    public GameObject panelCrear;
    public Button crearJugador;
    public TMP_InputField nombreJugadorNuevo;
    public TextMeshProUGUI errorUsuarioExistente;
    string errorText;


    [Header("Eliminación de jugador existente")]
    TextMeshProUGUI nombreJugadorAEliminar;
    public GameObject panelEliminar;
    public Button eliminarJugador;
    public TMP_InputField nombreJugadorEliminado;
    public TextMeshProUGUI errorTextoIncorrecto;
    string errorText2;

    [Header("Inicio de juego")]
    public Button modoHistoria;

    void Start()
    {
        consultasUsuarios = new SQLQuery("Usuarios");
        consultasBaseLogopeda = new SQLQuery("BaseLogopeda");
        panelBotones.SetActive(false);
        datos = FindObjectOfType<DatosJugador>();

        string[] clavesEsperaPlanetas = { "Dias", "Horas", "Minutos", "Segundos" };
        datos.AccederTiempoEspera(clavesEsperaPlanetas, ref datos.tiempoDeEspera);

        errorText = errorUsuarioExistente.text;
        errorUsuarioExistente.text = "";
        //
        LeerJugadores();
        crearJugador.onClick.AddListener(JugadorNuevo);
        nombreJugadorNuevo.onValueChanged.AddListener(delegate { errorUsuarioExistente.text = ""; });

        errorText2 = errorTextoIncorrecto.text;
        errorTextoIncorrecto.text = "";
        //
        nombreJugadorEliminado.onValueChanged.AddListener(delegate { errorTextoIncorrecto.text = ""; });

        CargarJugadorActual();

        modoHistoria.onClick.AddListener(CargarModoHistoria);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (panelCrear.activeInHierarchy)
            {
                JugadorNuevo();
            }

            if (panelEliminar.activeInHierarchy)
            {
                EliminarJugador(nombreJugadorAEliminar);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panelCrear.SetActive(false);
            panelEliminar.SetActive(false);
        }
    }

    void LeerJugadores()
    {
        consultasUsuarios.Query("SELECT Nombre FROM Usuarios");
        List<string> usuarios = consultasUsuarios.StringReader(1);

        for (int i = 0; i < usuarios.Count; i++)
        {
            UsuarioNuevo(usuarios[i]);
        }
    }

    void CargarModoHistoria()
    {
        if (!datos.cinematicaVista)
        {
            SceneManager.LoadScene("cinematica");
        }
        else
        {
            SceneManager.LoadScene("Nave");
        }
    }

    void CargarJugadorActual()
    {
        if (datos.nombre != "")
        {
            ActualizarInterfaz(datos.nombre);
        }
    }

    void ActualizarInterfaz(string nombre)
    {
        if (!panelBotones.activeInHierarchy)
        {
            panelBotones.SetActive(true);
        }

        TextMeshProUGUI nombreUI = panelBotones.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        nombreUI.text = nombre;
    }

    void PanelEliminar(TextMeshProUGUI nombre)
    {
        panelEliminar.SetActive(true);
        eliminarJugador.onClick.RemoveAllListeners();
        eliminarJugador.onClick.AddListener(delegate { EliminarJugador(nombre); });
        nombreJugadorAEliminar = nombre;
    }

    void EliminarJugador(TextMeshProUGUI nombre)
    {
        if (nombreJugadorEliminado.text == nombre.text)
        {
            consultasUsuarios.Query("SELECT ID_Usuario FROM Usuarios WHERE Nombre = '" + nombre.text + "'");
            int id = consultasUsuarios.IntReader(1)[0];

            consultasUsuarios.Query("DROP TABLE Almacen_" + id);
            consultasUsuarios.Query("DROP TABLE Coleccion_" + id);
            consultasUsuarios.Query("DROP TABLE Planetario_" + id);

            consultasUsuarios.Query("DELETE FROM Usuarios WHERE Nombre = '" + nombre.text + "'");
            Destroy(nombre.transform.parent.parent.gameObject);

            panelBotones.SetActive(false);
            panelEliminar.SetActive(false);
        }
        else
        {
            errorTextoIncorrecto.text = errorText2;
        }
    }

    void JugadorNuevo()
    {
        string nombre = nombreJugadorNuevo.text;
        consultasUsuarios.Query("SELECT * FROM Usuarios WHERE Nombre = '" + nombre + "'");

        if (consultasUsuarios.Count() == 0)
        {
            consultasUsuarios.Query("INSERT INTO Usuarios (Nombre, Gasolina) VALUES ('" + nombre + "', " + 50000 + ")");

            consultasUsuarios.Query("SELECT ID_Usuario FROM Usuarios WHERE Nombre = '" + nombre + "'");
            int id = consultasUsuarios.IntReader(1)[0];

            consultasUsuarios.Query("CREATE TABLE Almacen_" + id + " AS SELECT * FROM Almacen_0");
            consultasUsuarios.Query("CREATE TABLE Coleccion_" + id + " AS SELECT * FROM Coleccion_0");
            RellenarColeccion(id);
            consultasUsuarios.Query("CREATE TABLE Planetario_" + id + " AS SELECT * FROM Planetario_0");

            UsuarioNuevo(nombreJugadorNuevo.text);
        }
        else
        {
            errorUsuarioExistente.text = errorText;
        }
    }

    void RellenarColeccion(int idJugador)
    {
        consultasBaseLogopeda.Query("SELECT ID_Categoria FROM Categorias");
        List<int> idCategorias = consultasBaseLogopeda.IntReader(1);

        for (int i = 0; i < idCategorias.Count; i++)
        {
            int idCategoria = idCategorias[i];
            consultasUsuarios.Query("INSERT INTO Coleccion_" + idJugador + " VALUES (" + idCategoria + ", 0, NULL)");
            Debug.Log(idCategoria);
        }
    }

    void UsuarioNuevo(string nombreUsuario)
    {
        GameObject usuario = Instantiate(usuarioPrefab, usuariosParent);

        TextMeshProUGUI nombre = usuario.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        nombre.text = nombreUsuario;

        Button boton1 = usuario.transform.GetChild(0).GetComponent<Button>();
        boton1.onClick.AddListener(delegate { ActualizarInterfaz(nombreUsuario); });
        boton1.onClick.AddListener(delegate { CargarDatos(nombreUsuario); });

        Button boton2 = usuario.transform.GetChild(1).GetComponent<Button>();
        boton2.onClick.AddListener(delegate { PanelEliminar(nombre); });
    }

    void CargarDatos(string nombreUsuario)
    {
        consultasUsuarios.Query("SELECT * FROM Usuarios WHERE Nombre = '" + nombreUsuario + "'");

        datos.id = consultasUsuarios.IntReader(1)[0];
        datos.nombre = nombreUsuario;
        datos.gasolina = consultasUsuarios.IntReader(3)[0];
        datos.experiencia = consultasUsuarios.IntReader(4)[0];

        float[] configDificultad = consultasUsuarios.FloatArrayReader(5)[0];
        ConfiguracionDificultad(configDificultad);

        datos.regenerarRondas = consultasUsuarios.BoolReader(6)[0];

        // ¿Ha visto el usuario la cinemática?
        datos.cinematicaVista = consultasUsuarios.BoolReader(7)[0];
    }

    void ConfiguracionDificultad(float[] config)
    {
        datos.distancias.Clear();
        datos.dificultades.Clear();

        for (int i = 0; i < config.Length; i++)
        {
            if ((i + 1) % 2 != 0)
            {
                datos.distancias.Add(config[i]);
            }
            else
            {
                datos.dificultades.Add((int)config[i]);
            }
        }
    }
}
