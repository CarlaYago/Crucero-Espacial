using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Espacio14_Script : MonoBehaviour
{
    public bool gapOcupado;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LetrasScript letraScript))
        {
            letraScript.colocar = true;
            gapOcupado = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LetrasScript letraScript))
        {
            letraScript.colocar = false;
            gapOcupado = false;
        }
    }
}
