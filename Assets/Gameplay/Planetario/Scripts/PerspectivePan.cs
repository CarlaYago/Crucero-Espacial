using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectivePan : MonoBehaviour
{
    private Vector3 touchStart;
    public Camera cam;
    public float groundZ = 0;
    public float maxX;
    public float maxY;
    public float minX;
    public float minY;
    public int minZoom = 1, maxZoom = 25;
    public int velZoom = 1;
    public desplegable desplegable;
    public GameObject imgSeleccion;


    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            desplegable.desplegableObject.SetActive(false);
            imgSeleccion.SetActive(false);
            if (GetComponent<Camera>().orthographicSize > minZoom)//antes 1
            {
                GetComponent<Camera>().orthographicSize -= velZoom;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            desplegable.desplegableObject.SetActive(false);
            imgSeleccion.SetActive(false);

            if (GetComponent<Camera>().orthographicSize < maxZoom)//antes 20
            {
                GetComponent<Camera>().orthographicSize += velZoom;
            }
        }

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            touchStart = GetWorldPosition(groundZ);
            if (touchStart.x > maxX)
            {
                touchStart.x = maxX;
            }

            if (touchStart.x < minX)
            {
                touchStart.x = minX;
            }

            if (touchStart.y > maxY)
            {
                touchStart.y = maxY;
            }

            if (touchStart.y < minY)
            {
                touchStart.y = minY;
            }
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 direction = touchStart - GetWorldPosition(groundZ);
            cam.transform.position += direction;
            desplegable.desplegableObject.SetActive(false);
            imgSeleccion.SetActive(false);
            if (touchStart.x > maxX)
            {
                touchStart.x = maxX;
            }

            if (touchStart.x < minX)
            {
                touchStart.x = minX;
            }

            if (touchStart.y > maxY)
            {
                touchStart.y = maxY;
            }

            if (touchStart.y < minY)
            {
                touchStart.y = minY;
            }
        }
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}