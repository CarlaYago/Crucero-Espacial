using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeta_padre : MonoBehaviour
{
    public Transform planetasparent;

    public void quitAndImagevisto(bool toggle)
    {
        desactivado(toggle, 2);
    }

    public void quitAndImagefavorito(bool toggle)
    {
        desactivado(toggle, 1);
    }

    public void quitAndImagecronometro(bool toggle)
    {
        desactivado(toggle, 0);
    }

    void desactivado(bool toggle, int childIndex)
    {
        for (int i = 0; i < planetasparent.childCount; i++)
        {
            SpriteRenderer imagen = planetasparent.GetChild(i).GetChild(1).GetChild(childIndex+1).gameObject.GetComponent<SpriteRenderer>();
            imagen.enabled = toggle;
        }
    }
}
