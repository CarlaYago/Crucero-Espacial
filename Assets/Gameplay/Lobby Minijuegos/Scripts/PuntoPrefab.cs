using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuntoPrefab : MonoBehaviour
{
    public List<Sprite> listaSprites = new List<Sprite>();

    public int num;

    void Update()
    {
        gameObject.GetComponent<Image>().sprite = listaSprites[num];
    }
}
