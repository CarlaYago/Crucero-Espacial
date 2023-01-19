using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollHeader : MonoBehaviour
{
    public Scrollbar scrollHeader, scrollTable;

    void Update()
    {
        scrollHeader.value = scrollTable.value;
    }
}
