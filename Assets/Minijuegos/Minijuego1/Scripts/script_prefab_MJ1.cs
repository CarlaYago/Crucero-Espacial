using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;





public class script_prefab_MJ1 : MonoBehaviour
{
    //public RawImage Image;
    public Minijuego1 scriptMJ1;
    public TMP_InputField inputField;
    string inputUpperCase;
    public int posicion;
    public List<Sprite> bien_yMal;
    void Start()
    {
        inputField.image.sprite = bien_yMal[2];
        scriptMJ1 = FindObjectOfType<Minijuego1>();
        //Image.texture = scriptMJ1.Imagenes[posicion];
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
    private void Update()
    {
        if (inputUpperCase != inputField.text)
        {
            inputField.text = inputUpperCase;
        }
    }
    public void UpperCase()
    {
        inputUpperCase = inputField.text.ToUpper();
    }
  
}

