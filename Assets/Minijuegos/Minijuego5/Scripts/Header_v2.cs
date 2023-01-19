using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Header_v2 : MonoBehaviour
{
    public GameObject header;
    public bool congelarEjes;

    void Update()
    {
        if (congelarEjes) transform.position = new Vector3(header.transform.position.x, header.transform.position.y, transform.position.z);
        else transform.position = new Vector3(transform.position.x, header.transform.position.y, transform.position.z);
    }
}