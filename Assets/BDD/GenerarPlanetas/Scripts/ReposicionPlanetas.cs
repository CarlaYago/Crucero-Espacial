using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class ReposicionPlanetas : MonoBehaviour
{
    // Base de datos
    SQLQuery tablaPlanetas;
    PlanetaPrefab datosPlaneta;
    Transform capas;
    // Variable para enviar separadores decimales como puntos (por defecto, un ordenador español usa comas, lo que da problemas de lectura)
    readonly CultureInfo puntoDecimal = CultureInfo.InvariantCulture;

    // Generador y lector de planetas
    GenerarPlanetas generacion;
    LeerPlanetas lectura;

    // Número de intentos para buscar una nueva posición
    public int numIntentosReposicion;
    int intentoActual;

    // Decidir si mostrar el número de intentos por consola
    public bool debug;

    int idPropia, idExterna; // IDs para detectar colision propia vs externa
    float radio; // Tamaño del collider para detectar superposición entre planetas
    Transform parent;

    private void Start()
    {
        radio = GetComponent<Collider2D>().bounds.extents.x;
        idPropia = gameObject.GetInstanceID();

        generacion = FindObjectOfType<GenerarPlanetas>();
        lectura = FindObjectOfType<LeerPlanetas>();

        parent = GetComponent<PlanetaPrefab>().transform;

        if (generacion.rellenarBDD && (!lectura.leerBDD || !lectura.enabled))
        {
            datosPlaneta = GetComponent<PlanetaPrefab>();
            capas = transform.Find("Capas");
            tablaPlanetas = new SQLQuery("Usuarios");
            IntroducirEnBDD();
        }
    }

    private void Update()
    {
        if (!generacion.terminado)
        {
            Collider2D collision = Physics2D.OverlapCircle(transform.position, radio, LayerMask.GetMask("Planetas"));
            idExterna = collision.gameObject.GetInstanceID();

            if (collision == true && idExterna != idPropia) // Si el planeta choca con otro...
            {
                if (intentoActual < numIntentosReposicion) //... durante N veces
                {
                    Reposicionar();
                }
                else // Si el planeta se sigue chocando después de N veces... 
                {
                    if (debug) Debug.Log("Planeta eliminado (intentos superados)");
                    if (generacion.rellenarBDD) EliminarDeBDD();

                    generacion.planetasCancelados++;
                    Destroy(gameObject);
                }
            }
        }
    }

    void Reposicionar()
    {
        generacion.planetasGenerados--;

        if (debug) Debug.Log("Reposición intento " + (intentoActual + 1));

        parent.position = generacion.CalcularPosicion(transform);
        generacion.CosteViaje(transform);

        if (generacion.rellenarBDD) ActualizarValoresBDD();

        intentoActual++;
        generacion.planetasGenerados++;
    }

    #region Base de datos

    void IntroducirEnBDD()
    {
        int id = PlanetaID();
        string infoCapas = InformacionCapas();
        int flip = FlipX();
        string colores = ColoresRGB();
        string[] posEscala = PosicionEscala();
        string minijuegos = Minijuegos();

        tablaPlanetas.Query("SELECT * FROM Planetas WHERE ID_Planeta = " + id);

        if (tablaPlanetas.Count() > 0) tablaPlanetas.Query("DELETE FROM Planetas WHERE ID_Planeta = " + id);

        tablaPlanetas.Query("INSERT INTO Planetas VALUES (" +
                            // ID:
                            PlanetaID() + ", '" +
                            // Nombre:
                            datosPlaneta.nombre + "' , '" +
                            // Capas:
                            infoCapas + "' ," + flip + ", '" + colores + "' , '" +
                            // Posicion y Escala:
                            posEscala[0] + "' , '" + posEscala[1] + "' , '" +
                            // Minijuegos:
                            minijuegos + "' ," +
                            // Coste:
                            datosPlaneta.precioViaje + "," +
                            // Recompensa Energía:
                            datosPlaneta.recompensaEnergia + ")");
    }

    void ActualizarValoresBDD()
    {
        int id = PlanetaID();
        string[] posEscala = PosicionEscala();
        int precio = datosPlaneta.precioViaje;
        int recompensa = datosPlaneta.recompensaEnergia;

        tablaPlanetas.Query("UPDATE Planetas SET " +
                            // Posicion y Escala Nuevos:
                            "Posicion = '" + posEscala[0] + "' , Escala = '" + posEscala[1] + "' ," +
                            // Coste y Recompensa Nuevos:
                            "Coste = " + precio + ", Recompensa = " + recompensa + " WHERE ID_Planeta = " + id);
    }

    void EliminarDeBDD()
    {
        int id = PlanetaID();
        tablaPlanetas.Query("DELETE FROM Planetas WHERE ID_Planeta = " + id);
    }

    int PlanetaID()
    {
        string nombre = gameObject.name;
        int id = int.Parse(nombre.Replace("Planeta ", ""));

        return id;
    }

    string InformacionCapas()
    {
        int numCapas = capas.childCount;
        string infoCapas = "";

        for (int i = 0; i < numCapas; i++)
        {
            infoCapas += capas.GetChild(i).name;
            if (i != numCapas - 1) infoCapas += "/";
        }

        return infoCapas;
    }

    int FlipX()
    {
        int flipX = 0;

        SpriteRenderer sprite = capas.GetChild(0).GetComponent<SpriteRenderer>();
        if (sprite.flipX) flipX = 1;

        return flipX;
    }

    string ColoresRGB()
    {
        int numCapas = capas.childCount;
        string coloresRGB = "";

        for (int i = 0; i < numCapas; i++)
        {
            Color color = capas.GetChild(i).GetComponent<SpriteRenderer>().color;

            coloresRGB += color.r.ToString("0.####", puntoDecimal) + "/" + color.g.ToString("0.####", puntoDecimal) + "/" + color.b.ToString("0.####", puntoDecimal);
            if (i != numCapas - 1) coloresRGB += "/";
        }

        return coloresRGB;
    }

    string[] PosicionEscala()
    {
        string[] posEscala = new string[2];

        posEscala[0] = transform.position.x.ToString("0.####", puntoDecimal) + "/" + transform.position.y.ToString("0.####", puntoDecimal);
        posEscala[1] = transform.localScale.x.ToString("0.####", puntoDecimal) + "/" + transform.localScale.y.ToString("0.####", puntoDecimal);

        return posEscala;
    }

    string Minijuegos()
    {
        string minijuegos = "";

        for (int i = 0; i < 3; i++)
        {
            minijuegos += datosPlaneta.minijuegosElegidos[i];
            if (i != 2) minijuegos += "/";
        }

        return minijuegos;
    }

    #endregion Base de datos
}
