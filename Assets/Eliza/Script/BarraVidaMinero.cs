using UnityEngine;
using UnityEngine.UI;

public class BarraVidaMinero : MonoBehaviour
{
    public Minero minero;
    public string texto = "Barra de vida";
    public Font fuente;
    public int tamanoTexto = 32;
    public Color colorTexto = Color.white;
    public Color colorVida = new Color(0.85f, 0.15f, 0.15f);
    public Color colorFondo = new Color(0f, 0f, 0f, 0.6f);
    public Vector2 tamanoBarra = new Vector2(450f, 30f);
    public float distanciaDesdeArriba = 50f;
    public float distanciaDesdeDerecha = 50f;
    public float velocidadBarra = 2f;

    GameObject contenedor;
    PantallaFinal pantallaFinal;
    Image relleno;
    int vidaMaxima;

    void Start()
    {
        if (minero == null) minero = FindFirstObjectByType<Minero>();
        if (minero == null) return;
        vidaMaxima = Mathf.Max(1, minero.piedrasParaVencer);
        pantallaFinal = FindAnyObjectByType<PantallaFinal>();
        Crear();
    }

    void Update()
    {
        if (contenedor == null) return;

        bool ocultar = minero == null
            || Time.timeScale == 0f
            || (pantallaFinal != null && pantallaFinal.Termino)
            || (GameManager.Instancia != null && GameManager.Instancia.EsGameOver);
        if (contenedor.activeSelf == ocultar) contenedor.SetActive(!ocultar);
        if (ocultar) return;

        float vida = 1f - (float)minero.GolpesRecibidos / vidaMaxima;
        relleno.fillAmount = Mathf.MoveTowards(relleno.fillAmount, Mathf.Clamp01(vida), velocidadBarra * Time.deltaTime);
    }

    void Crear()
    {
        GameObject canvasGO = new GameObject("Canvas Vida Minero");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = -1;
        CanvasScaler escala = canvasGO.AddComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(1920f, 1080f);
        escala.matchWidthOrHeight = 0.5f;

        contenedor = new GameObject("Vida Minero", typeof(RectTransform));
        contenedor.transform.SetParent(canvasGO.transform, false);
        RectTransform rt = contenedor.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-distanciaDesdeDerecha, -distanciaDesdeArriba);
        rt.sizeDelta = new Vector2(tamanoBarra.x, tamanoBarra.y + tamanoTexto * 1.5f + 10f);

        GameObject textoGO = new GameObject("Texto", typeof(RectTransform));
        textoGO.transform.SetParent(contenedor.transform, false);
        Text t = textoGO.AddComponent<Text>();
        t.text = texto;
        t.font = fuente != null ? fuente : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = tamanoTexto;
        t.color = colorTexto;
        t.alignment = TextAnchor.LowerLeft;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        Outline borde = textoGO.AddComponent<Outline>();
        borde.effectColor = Color.black;
        borde.effectDistance = new Vector2(2f, -2f);
        RectTransform trt = textoGO.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0f, 1f);
        trt.anchorMax = new Vector2(1f, 1f);
        trt.pivot = new Vector2(0.5f, 1f);
        trt.anchoredPosition = Vector2.zero;
        trt.sizeDelta = new Vector2(0f, tamanoTexto * 1.5f);

        CrearImagen("Fondo", colorFondo);
        relleno = CrearImagen("Relleno", colorVida);
        relleno.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));
        relleno.type = Image.Type.Filled;
        relleno.fillMethod = Image.FillMethod.Horizontal;
        relleno.fillOrigin = (int)Image.OriginHorizontal.Left;
        relleno.fillAmount = 1f;
    }

    Image CrearImagen(string nombre, Color color)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(contenedor.transform, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0f, 0f);
        r.anchorMax = new Vector2(1f, 0f);
        r.pivot = new Vector2(0.5f, 0f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = new Vector2(0f, tamanoBarra.y);
        return img;
    }
}

