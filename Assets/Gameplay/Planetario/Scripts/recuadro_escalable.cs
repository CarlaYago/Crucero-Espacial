using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recuadro_escalable : MonoBehaviour
{
    SpriteRenderer marco;

    Vector2 dimensiones = new Vector2(0, 0);
    void Start()
    {
        marco = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float altura = Camera.main.orthographicSize * 3;
        float anchura = altura * Screen.width / Screen.height;
        dimensiones = new Vector2(anchura, altura);
        marco.size = dimensiones;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                string tocado = hit.transform.gameObject.GetComponent<PlanetaPrefab>().nombre;
            }
        }
    }
}
