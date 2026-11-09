using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Escena de historia al inicio: muestra las imágenes en orden con su texto abajo, y fundido a negro.
// Avanza con Espacio / W / Enter, clic o toque, o A / B en el gamepad.
// X (teclado o gamepad), Esc o Start salta toda la historia.
// Crea toda la interfaz sola al darle Play: solo pon este script en un objeto vacío.
public class IntroHistoria : MonoBehaviour
{
    [Tooltip("Las imágenes de la historia, en orden")]
    public Sprite[] imagenes;

    [Tooltip("El texto de cada imagen, en el mismo orden")]
    [TextArea(2, 5)]
    public string[] textosHistoria =
    {
        "En el corazón de la selva existe una laguna sagrada, cuidada por dos guardianes: una madre jaguar y su pequeño cachorro.",
        "Son el espíritu de dos tunjos de oro que descansan bajo la cascada. Mientras permanezcan juntos, la laguna vivirá.",
        "La Madre Jaguar es la ninfa de las aguas. Su luz mantiene la laguna clara y llena de vida.",
        "El pequeño es el corazón de la selva. A su paso, las flores se abren y los árboles crecen.",
        "Pero entre los hombres corría una leyenda: en la Laguna del Jaguar, el oro era abundante y mágico.",
        "Movidos por la avaricia, cruzaron la selva, destruyendo todo a su paso.",
        "Al llegar al lugar sagrado, encontraron el tunjo del cachorro. Sin importarles su valor, se lo llevaron junto con todo el oro de la laguna.",
        "La Madre Jaguar despertó y vio cómo todo se apagaba: las aguas se oscurecían, las flores se marchitaban y los árboles perdían su color.",
        "Lejos de su hogar, el cachorro se debilita. Si no regresa pronto, morirá, y con él toda la vida de la laguna.",
        "Con el último brillo del agua en sus marcas, la Madre Jaguar emprende su camino. Debe recuperar el tunjo, salvar a su pequeño y devolverle la vida a la laguna."
    };

    [Tooltip("Nombre EXACTO de la escena que carga al terminar")]
    public string siguienteEscena = "Nivel";

    [Header("Ayudas de botones (opcional)")]
    [Tooltip("Imagen tipo 'Con W o A Siguiente'. Sale abajo a la derecha")]
    public Sprite spriteSiguiente;
    [Tooltip("Imagen tipo 'Con X Omitir'. Sale abajo a la izquierda. Si está vacía, sale como texto")]
    public Sprite spriteOmitir;
    public float altoAyuda = 70f;

    public float letrasPorSegundo = 40f;
    public float duracionFundido = 0.5f;

    Image imagen;
    Image negro;
    Text texto;
    bool avanzar;
    bool saltar;

    void Start()
    {
        CrearInterfaz();
        StartCoroutine(MostrarHistoria());
    }

    void Update()
    {
        Keyboard k = Keyboard.current;
        Gamepad g = Gamepad.current;
        Pointer p = Pointer.current;

        if ((k != null && (k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame)) ||
            (g != null && (g.buttonSouth.wasPressedThisFrame || g.buttonEast.wasPressedThisFrame)) ||
            (p != null && p.press.wasPressedThisFrame))
        {
            avanzar = true;
        }

        if ((k != null && (k.xKey.wasPressedThisFrame || k.escapeKey.wasPressedThisFrame)) ||
            (g != null && (g.buttonWest.wasPressedThisFrame || g.startButton.wasPressedThisFrame)))
        {
            saltar = true;
        }
    }

    IEnumerator MostrarHistoria()
    {
        int total = imagenes != null ? imagenes.Length : 0;
        for (int i = 0; i < total && !saltar; i++)
        {
            // Cambia la imagen detrás de la pantalla negra
            imagen.sprite = imagenes[i];
            imagen.color = imagenes[i] != null ? Color.white : Color.black;
            string completo = (textosHistoria != null && i < textosHistoria.Length) ? textosHistoria[i] : "";
            texto.text = "";

            yield return Fundido(1f, 0f); // aparece

            // Texto letra por letra (si presionas, se completa de una)
            avanzar = false;
            float letras = 0f;
            while (letras < completo.Length && !saltar)
            {
                if (avanzar) { letras = completo.Length; avanzar = false; }
                letras += letrasPorSegundo * Time.deltaTime;
                texto.text = completo.Substring(0, Mathf.Min(completo.Length, (int)letras));
                yield return null;
            }
            texto.text = completo;

            // Espera a que el jugador presione para seguir
            avanzar = false;
            while (!avanzar && !saltar) yield return null;
            avanzar = false;

            yield return Fundido(0f, 1f); // se oscurece
        }

        Debug.Log("IntroHistoria: cargando la escena \"" + siguienteEscena + "\"");
        SceneManager.LoadScene(siguienteEscena);
    }

    IEnumerator Fundido(float desde, float hasta)
    {
        for (float t = 0f; t < duracionFundido; t += Time.deltaTime)
        {
            negro.color = new Color(0f, 0f, 0f, Mathf.Lerp(desde, hasta, t / duracionFundido));
            yield return null;
        }
        negro.color = new Color(0f, 0f, 0f, hasta);
    }

    // ---------- Interfaz ----------

    void CrearInterfaz()
    {
        if (Camera.main != null) Camera.main.backgroundColor = Color.black;

        GameObject canvasGO = new GameObject("CanvasHistoria", typeof(Canvas), typeof(CanvasScaler));
        canvasGO.transform.SetParent(transform, false);
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Fondo negro
        Image fondo = CrearImagen(canvasGO.transform, "Fondo", Color.black);
        Estirar(fondo.rectTransform);

        // La imagen (ocupa la pantalla, sin deformarse)
        imagen = CrearImagen(canvasGO.transform, "Imagen", Color.black);
        Estirar(imagen.rectTransform);
        imagen.preserveAspect = true;

        // Panel oscuro abajo con el texto de la historia
        Image panel = CrearImagen(canvasGO.transform, "PanelTexto", new Color(0f, 0f, 0f, 0.7f));
        RectTransform rp = panel.rectTransform;
        rp.anchorMin = new Vector2(0f, 0f);
        rp.anchorMax = new Vector2(1f, 0f);
        rp.pivot = new Vector2(0.5f, 0f);
        rp.sizeDelta = new Vector2(0f, 220f + altoAyuda);
        rp.anchoredPosition = Vector2.zero;
        panel.raycastTarget = false;

        GameObject textoGO = new GameObject("Texto", typeof(RectTransform), typeof(Text));
        textoGO.transform.SetParent(panel.transform, false);
        RectTransform rt = textoGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(120f, altoAyuda + 25f);
        rt.offsetMax = new Vector2(-120f, -25f);
        texto = textoGO.GetComponent<Text>();
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = 42;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = Color.white;

        // Ayudas de botones abajo: Siguiente a la derecha, Omitir a la izquierda
        CrearAyuda(canvasGO.transform, "AyudaSiguiente", spriteSiguiente, "W / A: siguiente", false);
        CrearAyuda(canvasGO.transform, "AyudaOmitir", spriteOmitir, "X: omitir historia", true);

        // Pantalla negra para los fundidos (encima de todo)
        negro = CrearImagen(canvasGO.transform, "Negro", Color.black);
        Estirar(negro.rectTransform);
        negro.raycastTarget = false;
    }

    // Pone una imagen de ayuda en una esquina de abajo; si no hay imagen, pone texto
    void CrearAyuda(Transform padre, string nombre, Sprite sprite, string textoAlterno, bool izquierda)
    {
        Vector2 esquina = new Vector2(izquierda ? 0f : 1f, 0f);
        Vector2 posicion = new Vector2(izquierda ? 30f : -30f, 15f);

        if (sprite != null)
        {
            Image img = CrearImagen(padre, nombre, Color.white);
            img.sprite = sprite;
            img.preserveAspect = true;
            img.raycastTarget = false;
            RectTransform r = img.rectTransform;
            r.anchorMin = r.anchorMax = r.pivot = esquina;
            float ancho = altoAyuda * sprite.rect.width / sprite.rect.height;
            r.sizeDelta = new Vector2(ancho, altoAyuda);
            r.anchoredPosition = posicion;
        }
        else
        {
            GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(padre, false);
            RectTransform r = go.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = r.pivot = esquina;
            r.sizeDelta = new Vector2(500f, altoAyuda);
            r.anchoredPosition = posicion;
            Text t = go.GetComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 30;
            t.alignment = izquierda ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
            t.color = new Color(1f, 1f, 1f, 0.8f);
            t.text = textoAlterno;
        }
    }

    Image CrearImagen(Transform padre, string nombre, Color color)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(padre, false);
        Image img = go.GetComponent<Image>();
        img.color = color;
        return img;
    }

    void Estirar(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

