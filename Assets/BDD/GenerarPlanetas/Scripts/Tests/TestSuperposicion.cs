using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSuperposicion : MonoBehaviour
{
    // Generador de planetas
    TestGeneracion generacion;

    // Número de intentos para buscar una nueva posición
    public int numIntentosReposicion;
    int intentoActual;

    // IDs para detectar colision propia vs externa
    int idPropia, idExterna;

    // Tamaño del collider para detectar superposición entre planetas
    float radio;

    private void Start()
    {
        radio = GetComponent<Collider2D>().bounds.extents.x;
        idPropia = gameObject.GetInstanceID();

        generacion = FindObjectOfType<TestGeneracion>();
    }

    private void Update()
    {
        if (!generacion.terminado)
        {
            Collider2D collision = Physics2D.OverlapCircle(transform.position, radio, LayerMask.GetMask("Planetas"));
            idExterna = collision.gameObject.GetInstanceID();

            if (collision == true && idExterna != idPropia) // Si el planeta choca con otro...
            {
                if (intentoActual < numIntentosReposicion)
                {
                    Reposicionar();
                }
                else // Si el planeta se sigue chocando... 
                {
                    Debug.Log("Planeta eliminado (intentos superados)");
                    generacion.planetasCancelados++;
                    Destroy(gameObject);
                }
            }
        }
    }

    void Reposicionar()
    {
        generacion.planetasGenerados--;

        Debug.Log("Reposición intento " + (intentoActual + 1));
        transform.position = generacion.CalcularPosicion(transform);
        intentoActual++;

        generacion.planetasGenerados++;
    }
}
