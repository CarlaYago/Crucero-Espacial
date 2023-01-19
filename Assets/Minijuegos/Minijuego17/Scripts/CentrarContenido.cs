using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CentrarContenido : MonoBehaviour
{
    public Scrollbar verticalScrollbar;
    public RectTransform scrollViewContent;
    float xPivot;

    void Start()
    {
        xPivot = scrollViewContent.pivot.x;
    }

    void Update()
    {
        if (verticalScrollbar.isActiveAndEnabled)
        {
            if (scrollViewContent.pivot.y != 1)
                scrollViewContent.pivot = new Vector2(xPivot, 1);
        }
        else
        {

            if (scrollViewContent.pivot.y != 0.5f)
                scrollViewContent.pivot = new Vector2(xPivot, 0.5f);
        }
    }
}