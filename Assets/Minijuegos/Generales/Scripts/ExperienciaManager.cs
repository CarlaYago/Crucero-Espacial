using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienciaManager
{
    float porcentaje;
    float temp, multDificultad;
    //List<int> tamañoPalabrasExtra = new List<int>();

    public ExperienciaManager(float porcentaje, float temp = 1f ,float multDificultad = 1f)
    {
        this.porcentaje = porcentaje;
        this.temp = temp;
        this.multDificultad = multDificultad;
    }

    public int SumaExp(List<int> tamanoPalabrasExtra)
    {
        int Exp = Mathf.RoundToInt(porcentaje * 400);
        Exp = Mathf.RoundToInt(Exp * temp);
        int extraExp =0;
        if(tamanoPalabrasExtra.Count > 0)
        {
            for (int i = 0; i < tamanoPalabrasExtra.Count; i++)
            {
                if (tamanoPalabrasExtra[i] > 4)
                {
                    extraExp += (20 * Mathf.RoundToInt(Mathf.Pow(2, tamanoPalabrasExtra[i]-5)));
                }
            }
            if (extraExp > 1000)
            {
                extraExp = 1000;
            }
            Exp += extraExp;
        }
        return Exp;
    }

}
