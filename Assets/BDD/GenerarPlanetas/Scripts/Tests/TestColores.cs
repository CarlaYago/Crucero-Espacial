using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Paletas
{
    Monocromatica,
    Analoga,
    Complementaria,
    Triadica,
    Aleatoria
}

public class TestColores : MonoBehaviour
{
    Color[] colores = new Color[4];

    [Header("Referencia")]
    public SpriteRenderer[] coloresEscena;
    public Color colorPruebas;

    [Header("Límites de saturación (S) y brillo (V)")]
    public Color limitesSuperior;
    public Color limitesInferior;
    float saturacionMin, saturacionMax, brilloMin, brilloMax;

    //[Header("Valor de diferencia máxima entre colores")]
    //[Range(0, 0.5f)] public float rangoColores = 0.1f;

    [Header("Tipo de paleta a generar")]
    public Paletas paletas;
    public bool usarRYB;

    readonly RGB2RYB conversor = new RGB2RYB();

    // Variables de generación de paleta aleatoria
    bool generarPaleta = true;
    List<int> coloresRestantes = new List<int>() { 0, 1, 2, 3 };

    void Start()
    {
        Color.RGBToHSV(limitesInferior, out _, out float sMin, out float bMin);
        saturacionMin = sMin;
        brilloMin = bMin;

        Color.RGBToHSV(limitesSuperior, out _, out float sMax, out float bMax);
        saturacionMax = sMax;
        brilloMax = bMax;

        switch (paletas)
        {
            case Paletas.Monocromatica:
                {
                    colores = Paleta(ColorMonocromatico, colorPruebas);
                    break;
                }
            case Paletas.Analoga:
                {
                    colores = Paleta(ColorAnalogoPrueba, colorPruebas);
                    break;
                }
            case Paletas.Complementaria:
                {
                    PaletaComplementaria();
                    break;
                }
            case Paletas.Triadica:
                {
                    colores = Paleta(ColorTriadicoPrueba, colorPruebas);
                    break;
                }
            case Paletas.Aleatoria:
                {
                    PaletaAleatoria();
                    break;
                }
        }

        AplicarColores();
    }

    #region Generacion de paleta aleatoria

    // -------------------------------------------------------

    void PaletaAleatoria()
    {
        #region Generar paleta base (monocromática)

        #region Generar color base
        //Color.RGBToHSV(colorPruebas, out float H, out _, out _);

        float HRandom = UnityEngine.Random.Range(0f, 1f);
        float S = UnityEngine.Random.Range(saturacionMin, saturacionMax);
        float V = UnityEngine.Random.Range(brilloMin, brilloMax);

        Color colorBase = Color.HSVToRGB(HRandom, S, V);
        #endregion Generar color base

        Paleta(RandomizarSV, colorBase);

        #endregion Generar paleta base (monocromática)

        List<int> posibilidades = new List<int>() { 1, 2, 3, 4 }; // 1 = Parar, 2 = Colores complementarios, 3 = Colores triádicos, 4 = Colores análogos

        while (generarPaleta)
        {
            int cambio = posibilidades[UnityEngine.Random.Range(0, posibilidades.Count)];

            switch (cambio)
            {
                case 1: // Parar
                    Debug.Log("Terminado la paleta");
                    generarPaleta = false;
                    break;

                case 2: // Colores complementarios
                    ColoresComplementarios();
                    posibilidades.Remove(2);
                    break;

                case 3: // Colores triádicos
                    ColoresTriadicos();
                    posibilidades.Remove(3);
                    break;

                case 4: // Colores análogos
                    ColoresAnalogos();
                    posibilidades.Remove(4);
                    break;
            }

            if (coloresRestantes.Count == 0) generarPaleta = false;
        }
    }

    #region Funciones paleta monocromática

    Color RandomizarSV(float[] hsv) // Baja saturación y brillo con un número aleatorio entre los límites públicos
    {
        float cambioBrillo = UnityEngine.Random.Range(brilloMin, brilloMax);
        float cambioSaturacion = UnityEngine.Random.Range(brilloMin, brilloMax);

        float calculoBrillo = hsv[2] - cambioBrillo;
        float brilloNuevo = NumeroCiclico(calculoBrillo);

        float calculoSaturacion = hsv[1] - cambioSaturacion;
        float saturacionNueva = NumeroCiclico(calculoSaturacion);

        brilloNuevo = AjustarValor(brilloNuevo, hsv[2], brilloMin, brilloMax);
        saturacionNueva = AjustarValor(saturacionNueva, hsv[1], saturacionMin, saturacionMax);

        return Color.HSVToRGB(hsv[0], saturacionNueva, brilloNuevo);
    }

    float AjustarValor(float valorNuevo, float valorAntiguo, float valorMin, float valorMax) // Mantiene el valor entre los límites establecidos, y que difiera un mínimo del anterior
    {
        if (valorNuevo < valorMin) // Si el valor es demasiado bajo...
        {
            valorNuevo = valorMin;

            if (Mathf.Abs(valorNuevo - valorAntiguo) < 0.1) // ...Si los valores son demasiado parecidos
            {
                valorNuevo += 0.1f;
            }
        }
        else if (valorNuevo > valorMax) // Si el valor es demasiado alto...
        {
            valorNuevo = valorMax;

            if (Mathf.Abs(valorNuevo - valorAntiguo) < 0.1) // ...Si los valores son demasiado parecidos
            {
                valorNuevo -= 0.1f;
            }
        }

        if (Mathf.Abs(valorNuevo - valorAntiguo) < 0.1) // Si los valores son demasiado parecidos...
        {
            if (valorNuevo + 0.1f < valorMax)
            {
                valorNuevo += 0.1f;
            }
            else valorNuevo -= 0.1f;
        }

        return valorNuevo;
    }

    #endregion Funciones paleta monocromática

    #region Funciones paleta complementaria

    void ColoresComplementarios()
    {
        Debug.Log("Entrado en colores complementarios");

        // Determinar el máximo de colores complementarios posibles según los colores restantes
        int maxComplementarios;
        //
        if (coloresRestantes.Count > 1) maxComplementarios = 2;
        else maxComplementarios = 1;

        int numComplementarios = UnityEngine.Random.Range(1, maxComplementarios + 1);

        // Determinar los colores posibles a usar como referencia
        List<int> coloresRef = new List<int>() { 0, 1, 2, 3 };
        //
        if (coloresRestantes.Count <= numComplementarios)
        {
            for (int i = 0; i < coloresRestantes.Count; i++)
            {
                coloresRef.Remove(coloresRestantes[i]);
            }
        }

        // Aplicar los colores complementarios
        for (int i = 0; i < numComplementarios; i++)
        {
            int refIndex = coloresRef[UnityEngine.Random.Range(0, coloresRef.Count)];
            if (coloresRestantes.Contains(refIndex)) coloresRestantes.Remove(refIndex); // La referencia del color complementario forma parte de la paleta final, ya no debe ser un color disponible

            int complIndex = coloresRestantes[UnityEngine.Random.Range(0, coloresRestantes.Count)];

            Color colorRef = colores[refIndex];
            Color complementario = ColorComplementario(colorRef, colores[complIndex]);

            colores[complIndex] = complementario;
            Debug.Log("Color complementario en la posición " + (complIndex + 1) + " basado en el color " + (refIndex + 1));

            coloresRef.Remove(refIndex);
            coloresRestantes.Remove(complIndex); // El color complementario forma parte de la paleta final, ya no debe ser un color disponible
        }
    }

    Color ColorComplementario(Color original, Color nuevo) // Devuelve el color complementario, manteniendo el brillo y saturación del color nuevo
    {
        // Convertir color original (RGB) a RYB
        if (usarRYB) original = conversor.SetRYB(original);

        Color.RGBToHSV(original, out float H, out _, out _);
        Color.RGBToHSV(nuevo, out _, out float S, out float V);

        float calculoMatiz = H - 0.5f;
        float matizNuevo = NumeroCiclico(calculoMatiz);

        Color complementario = Color.HSVToRGB(matizNuevo, S, V);

        // Re-convertir el resultado final a RGB para poder leerlo
        if (usarRYB) complementario = conversor.SetRGB(complementario);

        return complementario;
    }

    #endregion Funciones paleta complementaria

    #region Funciones paleta triádica

    void ColoresTriadicos()
    {
        if (coloresRestantes.Count > 1)
        {
            Debug.Log("Entrado en colores triádicos");

            // Determinar los colores posibles a usar como referencia
            List<int> coloresRef = new List<int>() { 0, 1, 2, 3 };
            //
            if (coloresRestantes.Count == 2)
            {
                for (int i = 0; i < coloresRestantes.Count; i++)
                {
                    coloresRef.Remove(coloresRestantes[i]);
                }
            }

            // Escoger el color de referencia y quitarlo de los colores disponibles
            int refIndex = coloresRef[UnityEngine.Random.Range(0, coloresRef.Count)];
            Color colorRef = colores[refIndex];
            //
            if (coloresRestantes.Contains(refIndex)) coloresRestantes.Remove(refIndex);

            // Aplicar los colores triádicos
            for (int i = 0; i < 2; i++)
            {
                int triadIndex = coloresRestantes[UnityEngine.Random.Range(0, coloresRestantes.Count)];

                Color triadico = ColorTriadico(colorRef, i + 1, colores[triadIndex]);
                colores[triadIndex] = triadico;

                Debug.Log("Color triádico en la posición " + (triadIndex + 1) + " basado en el color " + (refIndex + 1));

                coloresRestantes.Remove(triadIndex); // El color triádico forma parte de la paleta final, ya no debe ser un color disponible
            }
        }
    }

    Color ColorTriadico(Color original, int i, Color nuevo) // Baja el matiz por i tercios, manteniendo el brillo y saturación del color nuevo
    {
        // Convertir color original (RGB) a RYB
        if (usarRYB) original = conversor.SetRYB(original);

        Color.RGBToHSV(original, out float H, out _, out _);
        Color.RGBToHSV(nuevo, out _, out float S, out float V);

        float calculoMatiz = H - (i / 3f);
        float matizNuevo = NumeroCiclico(calculoMatiz);

        Color triadico = Color.HSVToRGB(matizNuevo, S, V);

        // Re-convertir el resultado final a RGB para poder leerlo
        if (usarRYB) triadico = conversor.SetRGB(triadico);

        return triadico;
    }

    #endregion Funciones paleta triádica

    #region Funciones paleta análoga

    void ColoresAnalogos()
    {
        Debug.Log("Entrado en colores análogos");

        // Determinar los colores posibles a usar como referencia
        List<int> coloresRef = new List<int>() { 0, 1, 2, 3 };

        int numRefs = 1;
        // En el caso de que hayan 4 colores disponibles, determinar cuántos de ellos se usarán como referencia (máximo 2)
        if (coloresRestantes.Count == 4) numRefs = UnityEngine.Random.Range(1, 3);

        if (numRefs == 1)
        {
            int numAnalogos = UnityEngine.Random.Range(1, coloresRestantes.Count);

            if (coloresRestantes.Count == numAnalogos)
            {
                for (int i = 0; i < coloresRestantes.Count; i++)
                {
                    coloresRef.Remove(coloresRestantes[i]);
                }
            }

            // Escoger el color de referencia y quitarlo de los colores disponibles
            int refIndex = coloresRef[UnityEngine.Random.Range(0, coloresRef.Count)];
            Color colorRef = colores[refIndex];
            //
            if (coloresRestantes.Contains(refIndex)) coloresRestantes.Remove(refIndex);

            for (int i = 0; i < numAnalogos; i++)
            {
                int analogoIndex = coloresRestantes[UnityEngine.Random.Range(0, coloresRestantes.Count)];

                Color analogo = ColorAnalogo(colorRef, i + 1, colores[analogoIndex]);
                colores[analogoIndex] = analogo;

                Debug.Log("Color análogo en la posición " + (analogoIndex + 1) + " basado en el color " + (refIndex + 1));

                coloresRestantes.Remove(analogoIndex); // El color análogo forma parte de la paleta final, ya no debe ser un color disponible
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                int refIndex = coloresRef[UnityEngine.Random.Range(0, coloresRef.Count)];
                if (coloresRestantes.Contains(refIndex)) coloresRestantes.Remove(refIndex); // La referencia del color análogo forma parte de la paleta final, ya no debe ser un color disponible

                int analogoIndex = coloresRestantes[UnityEngine.Random.Range(0, coloresRestantes.Count)];

                Color colorRef = colores[refIndex];
                Color analogo = ColorAnalogo(colorRef, i + 1, colores[analogoIndex]);

                colores[analogoIndex] = analogo;
                Debug.Log("Color análogo en la posición " + (analogoIndex + 1) + " basado en el color " + (refIndex + 1));

                coloresRef.Remove(refIndex);
                coloresRestantes.Remove(analogoIndex); // El color análogo forma parte de la paleta final, ya no debe ser un color disponible
            }
        }
    }

    Color ColorAnalogo(Color original, int i, Color nuevo) // Baja el matiz de 0.1 a 0.2, multiplicado por i, de forma cíclica, manteniendo el brillo y saturación del color nuevo
    {
        // Convertir color original (RGB) a RYB
        if (usarRYB) original = conversor.SetRYB(original);

        Color.RGBToHSV(original, out float H, out _, out _);
        Color.RGBToHSV(nuevo, out _, out float S, out float V);

        float resta = UnityEngine.Random.Range(0.1f, 0.2f);

        float calculoMatiz = H - resta * i;
        float matizNuevo = NumeroCiclico(calculoMatiz);

        Color analogo = Color.HSVToRGB(matizNuevo, S, V);

        // Re-convertir el resultado final a RGB para poder leerlo
        if (usarRYB) analogo = conversor.SetRGB(analogo);

        return analogo;
    }

    #endregion Funciones paleta análoga

    // -------------------------------------------------------

    #endregion Generacion de paleta aleatoria

    Color ColorMonocromatico(float[] hsv) // Baja el brillo por 0.1 de forma cíclica 
    {
        float calculoBrillo = hsv[2] - 0.1f;
        float brilloNuevo = NumeroCiclico(calculoBrillo);

        return Color.HSVToRGB(hsv[0], hsv[1], brilloNuevo);
    }

    Color ColorAnalogoPrueba(float[] hsv) // Baja el matiz por 0.1 de forma cíclica 
    {
        float calculoMatiz = hsv[0] - 0.1f;
        float matizNuevo = NumeroCiclico(calculoMatiz);

        return Color.HSVToRGB(matizNuevo, hsv[1], hsv[2]);
    }

    void PaletaComplementaria()
    {
        colores = new Color[2];
        colores[0] = colorPruebas;

        // Convertir color original (RGB) a RYB
        if (usarRYB) ConversionARYB(colores);

        // Calcular color complenetario
        colores[1] = Color.white - colores[0];

        // Re-convertir el resultado final a RGB para poder leerlo
        if (usarRYB) ConversionARGB(colores);
    }

    Color ColorTriadicoPrueba(float[] hsv) // Baja el matiz por 0.333 
    {
        float calculoMatiz = hsv[0] - (1f / 3f);
        float matizNuevo = NumeroCiclico(calculoMatiz);

        return Color.HSVToRGB(matizNuevo, hsv[1], hsv[2]);
    }

    void AplicarColores() // Muestra los colores en pantalla 
    {
        for (int i = 0; i < colores.Length; i++)
        {
            Color colorAlpha1 = new Color(colores[i].r, colores[i].g, colores[i].b, 1);
            coloresEscena[i].color = colorAlpha1;
        }
    }

    Color[] Paleta(Func<float[], Color> calculoColor, Color colorRef) // Usa las funciones de Color para generar 3 colores nuevos en base a un color original 
    {
        colores[0] = colorRef;

        // Convertir color original (RGB) a RYB
        if (usarRYB) ConversionARYB(colores);

        // Formatear el color RYB para obtener la paleta
        for (int i = 1; i < colores.Length; i++)
        {
            Color.RGBToHSV(colores[i - 1], out float h, out float s, out float v);
            float[] colorHSV = { h, s, v };

            colores[i] = calculoColor(colorHSV);
        }

        // Re-convertir el resultado final a RGB para poder leerlo
        if (usarRYB) ConversionARGB(colores);

        return colores;
    }

    float NumeroCiclico(float valor) // Hace que una resta siempre se quede entre 1 y 0 de manera cíclica 
    {
        if (valor >= 0)
        {
            return valor;
        }
        else // Mantiene el valor entre 0 y 1 de forma cíclica
        {
            int numEnteroInferior = (int)Math.Truncate(valor) - 1;
            return Mathf.Abs(numEnteroInferior) + valor;
        }
    }

    Color[] ConversionARYB(Color[] colores)
    {
        for (int i = 0; i < colores.Length; i++)
        {
            colores[i] = conversor.SetRYB(colores[i]);
        }

        return colores;
    }

    Color[] ConversionARGB(Color[] colores)
    {
        for (int i = 0; i < colores.Length; i++)
        {
            colores[i] = conversor.SetRGB(colores[i]);
        }

        return colores;
    }
}
