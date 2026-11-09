using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Escena de historia al inicio: muestra las imágenes en orden, con fundido a negro.
// Avanza con Espacio / W / Enter, clic o toque, o A / B en el gamepad.
// Esc o Start (gamepad) salta toda la historia.
// Crea toda la interfaz sola al darle Play: solo pon este script en un objeto vacío.
public class IntroHistoria : MonoBehaviour
{
    [Tooltip("Las imágenes de la historia, en orden")]
    public Sprite[] imagenes;

    [Tooltip("Nombre EXACTO de la escena que carga al terminar")]
    public string siguienteEscena = "Etapa1_Bosque";

    public float duracionFundido = 0.5f;

    Image imagen;
    Image negro;
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

        if ((k != null && k.escapeKey.wasPressedThisFrame) || (g != null && g.startButton.wasPressedThisFrame))
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

            yield return Fundido(1f, 0f); // aparece

            // Espera a que el jugador presione para seguir
            avanzar = false;
            while (!avanzar && !saltar) yield return null;
            avanzar = false;

            yield return Fundido(0f, 1f); // se oscurece
        }

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

        // Ayuda pequeña abajo a la derecha
        GameObject ayudaGO = new GameObject("Ayuda", typeof(RectTransform), typeof(Text));
        ayudaGO.transform.SetParent(canvasGO.transform, false);
        RectTransform ra = ayudaGO.GetComponent<RectTransform>();
        ra.anchorMin = new Vector2(0f, 0f);
        ra.anchorMax = new Vector2(1f, 0f);
        ra.pivot = new Vector2(0.5f, 0f);
        ra.sizeDelta = new Vector2(-60f, 40f);
        ra.anchoredPosition = new Vector2(0f, 15f);
        Text ayuda = ayudaGO.GetComponent<Text>();
        ayuda.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ayuda.fontSize = 24;
        ayuda.alignment = TextAnchor.LowerRight;
        ayuda.color = new Color(1f, 1f, 1f, 0.6f);
        ayuda.text = "Espacio / A: continuar     Esc / Start: saltar";

        // Pantalla negra para los fundidos (encima de todo)
        negro = CrearImagen(canvasGO.transform, "Negro", Color.black);
        Estirar(negro.rectTransform);
        negro.raycastTarget = false;
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
