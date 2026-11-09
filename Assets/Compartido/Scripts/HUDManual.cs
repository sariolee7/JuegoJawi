using UnityEngine;
using UnityEngine.UI;
using TMPro;

// HUD hecho A MANO: tú pones los corazones, el tunjo y el texto en tu Canvas
// (dentro de tu marco), y este script solo los actualiza.
// Reemplaza a HUDJawi: NO uses los dos al mismo tiempo.
// Ponlo en cualquier objeto de la escena (por ejemplo en tu Canvas).
public class HUDManual : MonoBehaviour
{
    [Header("Corazones (arrastra tus imágenes en orden: 1, 2, 3...)")]
    public Image[] corazones;
    [Tooltip("Opcional: dibujo del corazón vacío. Si está vacío, el corazón perdido se pone gris")]
    public Sprite corazonVacio;
    public Color colorVacio = new Color(0.25f, 0.25f, 0.25f, 0.6f);

    [Header("Tunjo que se llena con el canto")]
    [Tooltip("Tu imagen del tunjo (se pone sola en Image Type = Filled)")]
    public Image tunjoRelleno;
    [Tooltip("Texto del número (TextMeshPro). Muestra las flores cantadas: x 0, x 1...")]
    public TMP_Text textoContador;

    [Header("Game Over (opcional)")]
    [Tooltip("Un panel tuyo que diga GAME OVER. Debe empezar apagado")]
    public GameObject panelGameOver;

    Sprite[] corazonLleno;
    EnergiaJawi energia;
    ControlesJawi controles;

    void Start()
    {
        energia = FindFirstObjectByType<EnergiaJawi>();
        controles = FindFirstObjectByType<ControlesJawi>();

        // Guarda el dibujo lleno de cada corazón para poder volver a ponerlo
        corazonLleno = new Sprite[corazones.Length];
        for (int i = 0; i < corazones.Length; i++)
            if (corazones[i] != null) corazonLleno[i] = corazones[i].sprite;

        if (tunjoRelleno != null)
        {
            tunjoRelleno.type = Image.Type.Filled;
            tunjoRelleno.fillMethod = Image.FillMethod.Vertical;
            tunjoRelleno.fillOrigin = (int)Image.OriginVertical.Bottom;
            tunjoRelleno.fillAmount = 0f;
        }
        if (panelGameOver != null) panelGameOver.SetActive(false);

        GameManager gm = GameManager.Instancia;
        if (gm == null)
        {
            Debug.LogWarning("HUDManual: no hay GameManager en la escena.");
            return;
        }
        gm.AlCambiarVidas += ActualizarCorazones;
        gm.AlGameOver += MostrarGameOver;
        ActualizarCorazones(gm.Vidas);
        if (gm.EsGameOver) MostrarGameOver();
    }

    void OnDestroy()
    {
        GameManager gm = GameManager.Instancia;
        if (gm != null)
        {
            gm.AlCambiarVidas -= ActualizarCorazones;
            gm.AlGameOver -= MostrarGameOver;
        }
    }

    void Update()
    {
        if (energia != null)
        {
            if (tunjoRelleno != null)
                tunjoRelleno.fillAmount = Mathf.MoveTowards(tunjoRelleno.fillAmount, energia.Energia, 1.5f * Time.deltaTime);
            if (textoContador != null)
                textoContador.text = "x " + energia.FloresCantadas;
        }

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
        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] == null) continue;
            bool lleno = i < vidas;
            if (corazonVacio != null)
            {
                corazones[i].sprite = lleno ? corazonLleno[i] : corazonVacio;
                corazones[i].color = Color.white;
            }
            else
            {
                corazones[i].color = lleno ? Color.white : colorVacio;
            }
        }
    }

    void MostrarGameOver()
    {
        if (panelGameOver == null) return;
        panelGameOver.SetActive(true);
        Time.timeScale = 0f; // congela el juego
    }
}
