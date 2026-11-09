using UnityEngine;
using UnityEngine.UI;

// Muestra los corazones y el contador de tunjos arriba a la izquierda,
// y la pantalla de Game Over.
// Crea todo solo al darle Play: no hay que armar la interfaz a mano.
// Necesita que en la escena exista el GameManager.
public class HUDJawi : MonoBehaviour
{
    [Header("Corazones")]
    [Tooltip("Opcional: arrastra aquí un dibujo de corazón. Si está vacío, se ven cuadrados")]
    public Sprite spriteCorazon;
    [Tooltip("Blanco = el corazón se ve con sus colores originales")]
    public Color colorLleno = Color.white;
    public Color colorVacio = new Color(0.25f, 0.25f, 0.25f, 0.6f);
    public float tamanoCorazon = 60f;
    [Tooltip("Espacio entre corazones")]
    public float espacioCorazones = 8f;
    [Tooltip("Dónde van los corazones, medido desde la esquina de arriba a la izquierda (X a la derecha, Y negativo hacia abajo). Se puede cambiar en Play y se ve al instante")]
    public Vector2 posicionCorazones = new Vector2(30f, -30f);

    [Header("Tunjos")]
    [Tooltip("Opcional: dibujo del tunjo que sale junto al contador")]
    public Sprite spriteTunjo;
    public float tamanoTunjo = 50f;
    [Tooltip("Si está marcado, el dibujo del tunjo se va llenando de abajo hacia arriba a medida que Jawi los recoge (como la barra de energía)")]
    public bool tunjoSeLlena = true;
    [Tooltip("Marcado: el dibujo del tunjo se llena cantándole a las flores (energía de Jawi). Desmarcado: se llena recogiendo tunjos")]
    public bool llenarConCanto = true;
    [Tooltip("Cuántos tunjos hacen falta para que el dibujo quede lleno (solo si Llenar Con Canto está desmarcado)")]
    public int tunjosParaLlenar = 3;

    [Header("Game Over")]
    public string textoGameOver = "GAME OVER\n\nPresiona Espacio o W para reiniciar";

    Image[] corazones;
    RectTransform filaCorazones;
    HorizontalLayoutGroup layoutCorazones;
    RectTransform iconoTunjoRT;
    RectTransform fondoTunjoRT;
    Image imagenTunjo;
    EnergiaJawi energia;
    RectTransform textoTunjosRT;
    Text textoTunjos;
    GameObject panelGameOver;
    ControlesJawi controles;

    void Start()
    {
        GameManager gm = GameManager.Instancia;
        if (gm == null)
        {
            Debug.LogWarning("HUDJawi: no hay GameManager en la escena.");
            return;
        }

        Transform canvas = CrearCanvas();
        CrearCorazones(canvas, gm.vidasMaximas);
        CrearTunjos(canvas);
        CrearPanelGameOver(canvas);

        controles = FindFirstObjectByType<ControlesJawi>();
        energia = FindFirstObjectByType<EnergiaJawi>();

        gm.AlCambiarVidas += ActualizarCorazones;
        gm.AlGameOver += MostrarGameOver;
        gm.AlCambiarTunjos += ActualizarTunjos;

        ActualizarCorazones(gm.Vidas);
        ActualizarTunjos(gm.Tunjos);
        panelGameOver.SetActive(gm.EsGameOver);
    }

    void OnDestroy()
    {
        GameManager gm = GameManager.Instancia;
        if (gm != null)
        {
            gm.AlCambiarVidas -= ActualizarCorazones;
            gm.AlGameOver -= MostrarGameOver;
            gm.AlCambiarTunjos -= ActualizarTunjos;
        }
    }

    void Update()
    {
        AcomodarHUD();

        // El dibujo del tunjo se llena con el canto a las flores
        if (llenarConCanto && tunjoSeLlena && imagenTunjo != null && energia != null)
        {
            imagenTunjo.fillAmount = Mathf.MoveTowards(imagenTunjo.fillAmount, energia.Energia, 1.5f * Time.deltaTime);
        }

        // Con el canto, el número cuenta las flores a las que Jawi les cantó
        if (llenarConCanto && energia != null && textoTunjos != null)
            textoTunjos.text = "x " + energia.FloresCantadas;

        // En Game Over: Espacio / W (o B / A en gamepad) reinicia
        if (panelGameOver != null && panelGameOver.activeSelf && controles != null)
        {
            if (controles.ZarpazoPresionado || controles.SaltarPresionado)
            {
                Time.timeScale = 1f;
                GameManager.Instancia.Reiniciar();
            }
        }
    }

    // Pone los corazones y el contador de tunjos según los valores del Inspector
    void AcomodarHUD()
    {
        if (filaCorazones == null) return;
        int cantidad = corazones.Length;
        filaCorazones.anchoredPosition = posicionCorazones;
        filaCorazones.sizeDelta = new Vector2(cantidad * (tamanoCorazon + espacioCorazones), tamanoCorazon);
        layoutCorazones.spacing = espacioCorazones;
        foreach (Image c in corazones)
            c.rectTransform.sizeDelta = new Vector2(tamanoCorazon, tamanoCorazon);

        float y = posicionCorazones.y - tamanoCorazon - 15f;
        if (iconoTunjoRT != null)
        {
            iconoTunjoRT.anchoredPosition = new Vector2(posicionCorazones.x, y);
            iconoTunjoRT.sizeDelta = new Vector2(tamanoTunjo, tamanoTunjo);
        }
        if (fondoTunjoRT != null)
        {
            fondoTunjoRT.anchoredPosition = new Vector2(posicionCorazones.x, y);
            fondoTunjoRT.sizeDelta = new Vector2(tamanoTunjo, tamanoTunjo);
        }
        if (textoTunjosRT != null)
            textoTunjosRT.anchoredPosition = new Vector2(posicionCorazones.x + tamanoTunjo + 12f, y);
    }

    void ActualizarCorazones(int vidas)
    {
        if (corazones == null) return;
        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].color = i < vidas ? colorLleno : colorVacio;
        }
    }

    void ActualizarTunjos(int cantidad)
    {
        if (textoTunjos != null && !(llenarConCanto && energia != null)) textoTunjos.text = "x " + cantidad;
        if (imagenTunjo != null && tunjoSeLlena && !llenarConCanto)
            imagenTunjo.fillAmount = Mathf.Clamp01((float)cantidad / Mathf.Max(1, tunjosParaLlenar));
    }

    void MostrarGameOver()
    {
        panelGameOver.SetActive(true);
        Time.timeScale = 0f; // congela el juego
    }

    // ---------- Construcción de la interfaz ----------

    Transform CrearCanvas()
    {
        GameObject go = new GameObject("CanvasHUD", typeof(Canvas), typeof(CanvasScaler));
        go.transform.SetParent(transform, false);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        return go.transform;
    }

    void CrearCorazones(Transform canvas, int cantidad)
    {
        GameObject fila = new GameObject("Corazones", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        fila.transform.SetParent(canvas, false);

        RectTransform rt = fila.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        filaCorazones = rt;

        HorizontalLayoutGroup layout = fila.GetComponent<HorizontalLayoutGroup>();
        layoutCorazones = layout;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        corazones = new Image[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            GameObject c = new GameObject("Corazon" + (i + 1), typeof(RectTransform), typeof(Image));
            c.transform.SetParent(fila.transform, false);
            c.GetComponent<RectTransform>().sizeDelta = new Vector2(tamanoCorazon, tamanoCorazon);

            Image img = c.GetComponent<Image>();
            img.sprite = spriteCorazon;
            img.preserveAspect = true;
            corazones[i] = img;
        }
        AcomodarHUD();
    }

    // Contador de tunjos debajo de los corazones: [dibujo] x 3
    void CrearTunjos(Transform canvas)
    {
        float y = posicionCorazones.y - tamanoCorazon - 15f;

        // Fondo: el mismo tunjo en gris, para que se vea la parte que falta por llenar
        if (tunjoSeLlena && spriteTunjo != null)
        {
            GameObject fondo = new GameObject("FondoTunjo", typeof(RectTransform), typeof(Image));
            fondo.transform.SetParent(canvas, false);
            fondoTunjoRT = fondo.GetComponent<RectTransform>();
            fondoTunjoRT.anchorMin = fondoTunjoRT.anchorMax = fondoTunjoRT.pivot = new Vector2(0f, 1f);
            Image imgFondo = fondo.GetComponent<Image>();
            imgFondo.sprite = spriteTunjo;
            imgFondo.preserveAspect = true;
            imgFondo.color = colorVacio;
        }

        GameObject icono = new GameObject("IconoTunjo", typeof(RectTransform), typeof(Image));
        icono.transform.SetParent(canvas, false);
        RectTransform ri = icono.GetComponent<RectTransform>();
        ri.anchorMin = ri.anchorMax = ri.pivot = new Vector2(0f, 1f);
        ri.anchoredPosition = new Vector2(posicionCorazones.x, y);
        iconoTunjoRT = ri;
        ri.sizeDelta = new Vector2(tamanoTunjo, tamanoTunjo);
        Image img = icono.GetComponent<Image>();
        img.sprite = spriteTunjo;
        img.preserveAspect = true;
        img.color = spriteTunjo != null ? Color.white : new Color(1f, 0.8f, 0.15f);
        if (tunjoSeLlena && spriteTunjo != null)
        {
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Vertical;
            img.fillOrigin = (int)Image.OriginVertical.Bottom;
            img.fillAmount = 0f;
        }
        imagenTunjo = img;

        GameObject textoGO = new GameObject("TextoTunjos", typeof(RectTransform), typeof(Text));
        textoGO.transform.SetParent(canvas, false);
        RectTransform rt = textoGO.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(posicionCorazones.x + tamanoTunjo + 12f, y);
        rt.sizeDelta = new Vector2(200f, tamanoTunjo);
        textoTunjosRT = rt;
        textoTunjos = textoGO.GetComponent<Text>();
        textoTunjos.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textoTunjos.fontSize = 44;
        textoTunjos.alignment = TextAnchor.MiddleLeft;
        textoTunjos.color = Color.white;
    }

    void CrearPanelGameOver(Transform canvas)
    {
        panelGameOver = new GameObject("PanelGameOver", typeof(RectTransform), typeof(Image));
        panelGameOver.transform.SetParent(canvas, false);
        Estirar(panelGameOver.GetComponent<RectTransform>());
        panelGameOver.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

        GameObject textoGO = new GameObject("TextoGameOver", typeof(RectTransform), typeof(Text));
        textoGO.transform.SetParent(panelGameOver.transform, false);
        Estirar(textoGO.GetComponent<RectTransform>());

        Text texto = textoGO.GetComponent<Text>();
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.text = textoGameOver;
        texto.fontSize = 64;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = Color.white;

        panelGameOver.SetActive(false);
    }

    void Estirar(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

