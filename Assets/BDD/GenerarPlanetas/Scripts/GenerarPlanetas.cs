using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerarPlanetas : MonoBehaviour
{
    SQLQuery tablaPlanetas;
    LeerPlanetas lectorPlanetas;

    [Header("Referencias")]
    public Transform espacio;
    public GameObject planetaPrefab;
    public Sprite[] cielos, bases, detalles, adicional;
    public string[] rarezasAdquiribles;

    [Header("Funcionamiento")]
    public bool generarPlanetas;
    public bool rellenarBDD;

    [Header("Datos Planetas")]
    public float multiplicadorPrecios = 100;
    [Min(1)] public Vector2 multiplicadorRecompensaMinMax = new Vector2(1.5f, 3f);
    DatosJugador datos;

    [Header("Parámetros generación de planetas")]
    public int numPlanetas;
    public float distanciaBorde;
    public Vector2 limitesEscala;
    public GameObject planetaParent;
    Transform planetaActual;

    [Header("Límites de saturación (S) y brillo (V)")]
    public Color limiteSuperior;
    public Color limiteInferior;
    float saturacionMin, saturacionMax, brilloMin, brilloMax;

    [Header("Valor de diferencia mínima entre colores")]
    [Range(0, 0.5f)] public float diferenciaMin = 0.1f;

    [Header("Tipo de paleta a generar")]
    public bool usarRYB;
    readonly RGB2RYB conversor = new RGB2RYB();

    // Variables de generación de paleta
    Color[] colores = new Color[4];
    bool generarPaleta = true;
    List<int> coloresRestantes;

    [HideInInspector] public bool terminado, reiniciar;
    public int planetasGenerados, planetasCancelados;

    void Start()
    {
        lectorPlanetas = FindObjectOfType<LeerPlanetas>();
        datos = FindObjectOfType<DatosJugador>();
    }

    void Update()
    {
        if (lectorPlanetas != null && lectorPlanetas.enabled && lectorPlanetas.leerBDD)
        {
            if (generarPlanetas)
            {
                Debug.LogWarning("No es posible generar planetas si el lector de planetas en '" + lectorPlanetas.gameObject.name + "' está activado.");
                generarPlanetas = false;
                rellenarBDD = false;
            }
        }
        else if (generarPlanetas)
        {
            // Si hay planetas... eliminarlos (para poder crear nuevos)
            if (planetaParent != null && planetaParent.transform.childCount > 0)
            {
                EliminarPlanetas();
            }
            // Si no, o ya han sido eliminados... crear planetas
            else if (planetaParent == null || planetaParent.transform.childCount == 0)
            {
                if (rellenarBDD)
                {
                    tablaPlanetas = new SQLQuery("Usuarios");
                    tablaPlanetas.Query("DELETE FROM Planetas");
                }

                ReiniciarVariables();

                LimitesBrilloSaturacion();
                Generador();

                generarPlanetas = false;
            }
        }

        GeneracionTerminada(); // Informa de cuando todos los planetas han sido generados éxitosamente (incluyendo reposiciones)
    }

    //----------------------------

    #region Generación de planetas

    void Generador()
    {
        // Crear un nuevo objeto para guardar todos los planetas
        if (planetaParent == null)
        {
            planetaParent = new GameObject("Planetas");
            planetaParent.transform.parent = espacio;
        }

        // Si no se han introducido límites de escalado, utilizar la escala del prefab
        if (limitesEscala == Vector2.zero)
            limitesEscala = new Vector2(planetaPrefab.transform.localScale.x, planetaPrefab.transform.localScale.y);

        // Guardar los valores máximos y mínimos de saturación y brillo en base a los colores públicos
        LimitesBrilloSaturacion();

        // Generación de planetas
        for (int i = 0; i < numPlanetas; i++)
        {
            GameObject planeta = Instantiate(planetaPrefab, planetaParent.transform);
            planeta.name = "Planeta " + i;

            planetaActual = planeta.transform;

            AleatorizarCapas(planetaActual);

            GenerarPaleta();
            ColorearPlaneta(planetaActual);

            planetaActual.localScale = EscalarPlaneta();
            planetaActual.position = CalcularPosicion(planetaActual);

            DatosPlaneta(planetaActual);
        }
    }

    void AleatorizarCapas(Transform planeta)
    {
        Sprite[] capas = new Sprite[4];
        int[] capasIndex = new int[4];

        capasIndex[0] = UnityEngine.Random.Range(0, cielos.Length);
        capasIndex[1] = UnityEngine.Random.Range(0, bases.Length);
        capasIndex[2] = UnityEngine.Random.Range(0, detalles.Length);
        capasIndex[3] = UnityEngine.Random.Range(0, adicional.Length);

        capas[0] = cielos[UnityEngine.Random.Range(0, cielos.Length)];
        capas[1] = bases[UnityEngine.Random.Range(0, bases.Length)];
        capas[2] = detalles[UnityEngine.Random.Range(0, detalles.Length)];
        capas[3] = adicional[UnityEngine.Random.Range(0, adicional.Length)];

        int flip = UnityEngine.Random.Range(0, 2);

        for (int i = 0; i < 4; i++)
        {
            Transform capasPlaneta = planeta.Find("Capas");
            SpriteRenderer sr = capasPlaneta.GetChild(i).GetComponent<SpriteRenderer>();

            sr.sprite = capas[i];
            if (flip > 0) sr.flipX = true;

            sr.gameObject.name = capasIndex[i].ToString();
        }
    }

    Vector2 EscalarPlaneta()
    {
        float escala = UnityEngine.Random.Range(limitesEscala.x, limitesEscala.y);
        return new Vector2(escala, escala);
    }

    public Vector2 CalcularPosicion(Transform planeta)
    {
        SpriteRenderer espacioSprite = espacio.GetComponent<SpriteRenderer>();
        SpriteRenderer planetaSprite = planeta.Find("Capas").GetComponentInChildren<SpriteRenderer>();

        float radio = espacioSprite.bounds.extents.x - planetaSprite.bounds.extents.x - distanciaBorde;
        Vector2 offset = espacio.transform.position;

        Vector2 pos = UnityEngine.Random.insideUnitCircle * radio;

        float posicionX = pos.x + offset.x;
        float posicionY = pos.y + offset.y;

        return new Vector2(posicionX, posicionY);
    }

    void DatosPlaneta(Transform planeta)
    {
        PlanetaPrefab planetaScript = planeta.GetComponent<PlanetaPrefab>();

        planetaScript.nombre = UnityEngine.Random.Range(1000, 10000) + "";

        int numMinijuegos = datos.minijuegos.Count;

        // Hacer una lista de IDs para que no se repitan minijuegos
        List<int> idMinijuegos = new List<int>();
        for (int i = 0; i < numMinijuegos; i++) 
        { 
            idMinijuegos.Add(datos.minijuegos[i].id); 
        }

        for (int i = 0; i < 3; i++)
        {
            int minijuegoRandom = UnityEngine.Random.Range(0, idMinijuegos.Count);

            planetaScript.minijuegosElegidos[i] = idMinijuegos[minijuegoRandom];

            idMinijuegos.RemoveAt(minijuegoRandom);
        }

        CosteViaje(planeta);
    }

    public void CosteViaje(Transform planeta)
    {
        PlanetaPrefab planetaScript = planeta.GetComponent<PlanetaPrefab>();

        int precio = (int)(Vector2.Distance(planeta.position, espacio.position) * multiplicadorPrecios);
        planetaScript.precioViaje = precio;

        float multiplicadorRecompensa = UnityEngine.Random.Range(multiplicadorRecompensaMinMax.x, multiplicadorRecompensaMinMax.y);
        planetaScript.recompensaEnergia = (int)(precio * multiplicadorRecompensa);
    }

    void GeneracionTerminada()
    {
        if (!terminado)
        {
            if (planetaParent.transform.childCount == numPlanetas - planetasCancelados)
            {
                for (int i = 0; i < planetaParent.transform.childCount; i++)
                {
                    Transform planeta = planetaParent.transform.GetChild(i);
                    int idPlaneta = planeta.GetInstanceID();

                    float radio = planeta.GetComponent<Collider2D>().bounds.extents.x;

                    Collider2D collision = Physics2D.OverlapCircle(planeta.position, radio, LayerMask.GetMask("Planetas"));
                    int idColision = 0;
                    if (collision != null) idColision = collision.transform.GetInstanceID();

                    if (idPlaneta == idColision)
                        planetasGenerados++;
                    else
                    {
                        planetasGenerados = 0;
                        break;
                    }
                }

                if (planetasGenerados == numPlanetas - planetasCancelados)
                {
                    reiniciar = true;
                    Debug.Log("Generación de planetas finalizada: " + planetasGenerados + " / " + numPlanetas);
                    terminado = true;
                }
            }
        }
    }

    void EliminarPlanetas()
    {
        Transform planetas = planetaParent.transform;

        for (int i = 0; i < planetas.childCount; i++)
        {
            Destroy(planetas.GetChild(0).gameObject);
        }
    }

    void ReiniciarVariables()
    {
        terminado = false;
        planetasGenerados = 0;
        planetasCancelados = 0;
    }

    #endregion Generación de planetas

    //----------------------------

    #region Paleta de color

    void LimitesBrilloSaturacion() // Establece los valores máximos y mínimos de saturación y brillo en base a los colores públicos 
    {
        Color.RGBToHSV(limiteInferior, out _, out float sMin, out float bMin);
        saturacionMin = sMin;
        brilloMin = bMin;

        Color.RGBToHSV(limiteSuperior, out _, out float sMax, out float bMax);
        saturacionMax = sMax;
        brilloMax = bMax;
    }

    void ColorearPlaneta(Transform planeta)
    {
        for (int i = 0; i < 4; i++)
        {
            Transform capasPlaneta = planeta.Find("Capas");
            SpriteRenderer sr = capasPlaneta.GetChild(i).GetComponent<SpriteRenderer>();
            sr.color = colores[i];
        }
    }

    void GenerarPaleta()
    {
        coloresRestantes = new List<int>() { 0, 1, 2, 3 };
        generarPaleta = true;

        // Generar paleta base (monocromática)
        //
        Color colorBase = GenerarColorBase();
        Paleta(RandomizarSV, colorBase);

        List<int> posibilidades = new List<int>() { 1, 2, 3, 4 }; // 1 = Parar, 2 = Colores complementarios, 3 = Colores triádicos, 4 = Colores análogos

        while (generarPaleta)
        {
            int cambio = posibilidades[UnityEngine.Random.Range(0, posibilidades.Count)];

            switch (cambio)
            {
                case 1: // Parar
                    DebugPaleta("Terminado la paleta");
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

            if (coloresRestantes.Count == 0) { DebugPaleta("Terminado la paleta"); generarPaleta = false; }
        }
    }

    void DebugPaleta(string texto) // Muestra las operaciones para conseguir los colores de un planeta en un componente texto del mismo
    {
        if (planetaActual.TryGetComponent(out DebugPaleta debug))
        {
            debug.texto.Add(texto);
        }
    }

    #region Funciones paleta monocromática

    Color GenerarColorBase() // Genera un primer color aleatorio entre los límites establecidos 
    {
        float H = UnityEngine.Random.Range(0f, 1f);
        float S = UnityEngine.Random.Range(saturacionMin, saturacionMax);
        float V = UnityEngine.Random.Range(brilloMin, brilloMax);

        return Color.HSVToRGB(H, S, V);
    }

    Color RandomizarSV(float[] hsv) // Baja saturación y brillo con un número aleatorio entre los límites establecidos 
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

            if (Mathf.Abs(valorNuevo - valorAntiguo) < diferenciaMin) // ...Si los valores son demasiado parecidos
            {
                valorNuevo += diferenciaMin;
            }
        }
        else if (valorNuevo > valorMax) // Si el valor es demasiado alto...
        {
            valorNuevo = valorMax;

            if (Mathf.Abs(valorNuevo - valorAntiguo) < diferenciaMin) // ...Si los valores son demasiado parecidos
            {
                valorNuevo -= diferenciaMin;
            }
        }

        if (Mathf.Abs(valorNuevo - valorAntiguo) < 0.1) // Si los valores son demasiado parecidos...
        {
            if (valorNuevo + diferenciaMin < valorMax)
            {
                valorNuevo += diferenciaMin;
            }
            else valorNuevo -= diferenciaMin;
        }

        return valorNuevo;
    }

    Color[] Paleta(Func<float[], Color> calculoColor, Color colorRef) // Usa la función RandomizarSV para generar 3 tonalidades nuevas de un color original 
    {
        colores[0] = colorRef;

        // Formatear el color RYB para obtener la paleta
        for (int i = 1; i < colores.Length; i++)
        {
            Color.RGBToHSV(colores[i - 1], out float h, out float s, out float v);
            float[] colorHSV = { h, s, v };

            colores[i] = calculoColor(colorHSV);
        }

        return colores;
    }

    #endregion Funciones paleta monocromática

    #region Funciones paleta complementaria

    void ColoresComplementarios()
    {
        DebugPaleta("Entrado en colores complementarios");

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
            DebugPaleta("Color complementario en la posición " + (complIndex + 1) + " basado en el color " + (refIndex + 1));

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
            DebugPaleta("Entrado en colores triádicos");

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

                DebugPaleta("Color triádico en la posición " + (triadIndex + 1) + " basado en el color " + (refIndex + 1));

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
        DebugPaleta("Entrado en colores análogos");

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

                DebugPaleta("Color análogo en la posición " + (analogoIndex + 1) + " basado en el color " + (refIndex + 1));

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
                DebugPaleta("Color análogo en la posición " + (analogoIndex + 1) + " basado en el color " + (refIndex + 1));

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

    #region Funciones comunes

    float NumeroCiclico(float valor) // Hace que una resta siempre se quede entre 1 y 0 de manera cíclica (sirve para que los matices se mantengan en la rueda de color) 
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

    #endregion Funcion común

    #endregion Paleta de color

    //----------------------------
}