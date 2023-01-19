using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGeneracion : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject planetaPrefab;
    public Transform espacio;

    [Header("Parámetros")]
    public int numPlanetas;
    public float distanciaBorde;
    public Vector2 limitesEscala;

    GameObject planetaParent;
    public int planetasCancelados, planetasGenerados;
    public bool terminado;

    void Start()
    {
        planetaParent = new GameObject("Planetas");
        planetaParent.transform.parent = espacio.parent;

        if (limitesEscala == Vector2.zero)
            limitesEscala = new Vector2(planetaPrefab.transform.localScale.x, planetaPrefab.transform.localScale.y);

        for (int i = 0; i < numPlanetas; i++)
        {
            GameObject planeta = Instantiate(planetaPrefab, planetaParent.transform);
            planeta.transform.localScale = EscalarPlanetas();
            planeta.transform.position = CalcularPosicion(planeta.transform);
            planeta.name = "Planeta " + i;
        }
    }

    private void Update()
    {
        Terminado();
    }

    Vector2 EscalarPlanetas()
    {
        float escala = Random.Range(limitesEscala.x, limitesEscala.y);
        return new Vector2(escala, escala);
    }

    public Vector2 CalcularPosicion(Transform planeta)
    {
        float limites = espacio.localScale.x / 2 - planeta.localScale.x / 2 - distanciaBorde;
        Vector2 offset = espacio.transform.position;

        float posX = Random.Range(-limites, limites);
        float posY = Random.Range(-limites, limites);

        float posicionX = posX + offset.x;
        float posicionY = posY + offset.y;

        return new Vector2(posicionX, posicionY);
    }

    void Terminado()
    {
        if (!terminado)
        {
            if (planetaParent.transform.childCount == numPlanetas - planetasCancelados)
            {
                for (int i = 0; i < planetaParent.transform.childCount; i++)
                {
                    Transform planeta = planetaParent.transform.GetChild(i);
                    int idPlaneta = planeta.GetInstanceID();

                    float radio = planeta.GetComponent<Collider2D>().bounds.extents.x;
                    Collider2D collision = Physics2D.OverlapCircle(planeta.position, radio, LayerMask.GetMask("Planetas"));
                    int idColision = collision.transform.GetInstanceID();

                    if (idPlaneta == idColision)
                        planetasGenerados++;
                    else
                    {
                        planetasGenerados = 0;
                        break;
                    }
                }

                if (planetasGenerados == numPlanetas - planetasCancelados)
                {
                    terminado = true;
                    Debug.Log("Generación de planetas finalizada: " + planetasGenerados + " / " + numPlanetas);
                }
            }
        }
    }
}

