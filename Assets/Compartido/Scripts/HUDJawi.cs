using UnityEngine;
using UnityEngine.UI;

// Muestra los corazones arriba a la izquierda, los fragmentos de oro
// arriba a la derecha y la pantalla de Game Over.
// Crea todo solo al darle Play: no hay que armar la interfaz a mano.
// Necesita que en la escena exista el GameManager.
public class HUDJawi : MonoBehaviour
{
    [Header("Corazones")]
    [Tooltip("Opcional: arrastra aquí un dibujo de corazón. Si está vacío, se ven cuadrados")]
    public Sprite spriteCorazon;
    public Color colorLleno = new Color(0.9f, 0.1f, 0.2f);
    public Color colorVacio = new Color(0.25f, 0.25f, 0.25f, 0.6f);
    public float tamanoCorazon = 60f;

    [Header("Fragmentos de oro")]
    [Tooltip("Opcional: arrastra aquí un dibujo del fragmento. Si está vacío, se ven rombos")]
    public Sprite spriteFragmento;
    public Color colorFragmentoTenido = new Color(1f, 0.8f, 0.15f);
    public Color colorFragmentoFalta = new Color(0.25f, 0.25f, 0.25f, 0.6f);
    public float tamanoFragmento = 55f;

    [Header("Game Over")]
    public string textoGameOver = "GAME OVER\n\nPresiona Espacio o W para reiniciar";

    Image[] corazones;
    Image[] fragmentos;
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
        CrearFragmentos(canvas, gm.fragmentosTotales);
        CrearPanelGameOver(canvas);

        controles = FindFirstObjectByType<ControlesJawi>();

        gm.AlCambiarVidas += ActualizarCorazones;
        gm.AlGameOver += MostrarGameOver;
        gm.AlCambiarFragmentos += ActualizarFragmentos;

        ActualizarCorazones(gm.Vidas);
        ActualizarFragmentos(gm.Fragmentos);
        panelGameOver.SetActive(gm.EsGameOver);
    }

    void OnDestroy()
    {
        GameManager gm = GameManager.Instancia;
        if (gm != null)
        {
            gm.AlCambiarVidas -= ActualizarCorazones;
            gm.AlGameOver -= MostrarGameOver;
            gm.AlCambiarFragmentos -= ActualizarFragmentos;
        }
    }

    void Update()
    {
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

    void ActualizarCorazones(int vidas)
    {
        if (corazones == null) return;
        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].color = i < vidas ? colorLleno : colorVacio;
        }
    }

    void ActualizarFragmentos(int cantidad)
    {
        if (fragmentos == null) return;
        for (int i = 0; i < fragmentos.Length; i++)
        {
            fragmentos[i].color = i < cantidad ? colorFragmentoTenido : colorFragmentoFalta;
        }
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
        rt.anchoredPosition = new Vector2(30f, -30f);
        rt.sizeDelta = new Vector2(cantidad * (tamanoCorazon + 12f), tamanoCorazon);

        HorizontalLayoutGroup layout = fila.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 12f;
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
    }

    void CrearFragmentos(Transform canvas, int cantidad)
    {
        GameObject fila = new GameObject("Fragmentos", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        fila.transform.SetParent(canvas, false);

        RectTransform rt = fila.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-30f, -30f);
        rt.sizeDelta = new Vector2(cantidad * (tamanoFragmento + 16f), tamanoFragmento);

        HorizontalLayoutGroup layout = fila.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.UpperRight;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        fragmentos = new Image[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            GameObject f = new GameObject("Fragmento" + (i + 1), typeof(RectTransform), typeof(Image));
            f.transform.SetParent(fila.transform, false);
            f.GetComponent<RectTransform>().sizeDelta = new Vector2(tamanoFragmento, tamanoFragmento);

            Image img = f.GetComponent<Image>();
            img.sprite = spriteFragmento;
            img.preserveAspect = true;
            // Sin dibujo: un cuadrado girado 45° parece un rombo
            if (spriteFragmento == null)
            {
                f.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
                f.transform.localScale = Vector3.one * 0.75f;
            }
            fragmentos[i] = img;
        }
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
