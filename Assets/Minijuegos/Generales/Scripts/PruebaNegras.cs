using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaNegras : MonoBehaviour
{
    public List<string> SecuenciaSeleccionada;
    public List<string> palabrasNegrita;


    // Start is called before the first frame update
    void Start()
    {
        DetectarIdentificadores negritas = new DetectarIdentificadores("<b>", "</b>");
        /*palabrasNegrita = negritas.PalabrasDevueltas(fraseSeleccionada);
        fraseSeleccionada = negritas.QuitarIdentificadores(fraseSeleccionada);*/

        palabrasNegrita = negritas.PalabrasDevueltasSecuencias(SecuenciaSeleccionada);
        SecuenciaSeleccionada = negritas.QuitarIdentificadoresSecuencias(SecuenciaSeleccionada);
    }

}
