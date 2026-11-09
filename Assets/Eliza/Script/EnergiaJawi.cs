using UnityEngine;
using UnityEngine.UI;

// Barra de energía de Jawi.
// Va en Jawi. Cada flor (FlorInteractiva) a la que Jawi le canta
// llena la barra (25% por flor). Usa tu propia imagen de barra (Image Type = Filled).
public class EnergiaJawi : MonoBehaviour
{
    [Header("Tu barra (Image con Image Type = Filled)")]
    [Tooltip("Arrastra aquí la imagen verde de tu barra")]
    public Image barraRelleno;

    [Header("Energía")]
    [Tooltip("Qué tan rápido sube la barra en pantalla")]
    public float velocidadBarra = 1.5f;

    [Header("Sonido (opcional)")]
    public AudioClip sonidoEnergia;

    // Energía de 0 (vacía) a 1 (llena)
    public float Energia { get; private set; }
    public bool EstaLlena => Energia >= 1f;

    // Cuántas flores le ha cantado Jawi en esta etapa (lo muestra el HUD)
    public int FloresCantadas { get; private set; }

    float mostrada; // lo que se ve en la barra, sube suave hasta Energia

    void Start()
    {
        // La barra es opcional: el HUD ya muestra la energía en el dibujo del tunjo
        if (barraRelleno == null) return;
        barraRelleno.type = Image.Type.Filled; // por si acaso
        barraRelleno.fillAmount = 0f;          // empieza vacía
    }

    void Update()
    {
        // La barra sube suave hasta la energía real
        mostrada = Mathf.MoveTowards(mostrada, Energia, velocidadBarra * Time.deltaTime);
        if (barraRelleno != null) barraRelleno.fillAmount = mostrada;
    }

    public void GanarEnergia(float cantidad)
    {
        Energia = Mathf.Clamp01(Energia + cantidad);
        FloresCantadas++;
        Debug.Log("Jawi ganó energía: " + Mathf.RoundToInt(Energia * 100f) + "%");
        if (sonidoEnergia != null) AudioSource.PlayClipAtPoint(sonidoEnergia, transform.position);
    }

    // Por si más adelante algo debe GASTAR energía (cantidad de 0 a 1)
    public bool GastarEnergia(float cantidad)
    {
        if (Energia < cantidad) return false;
        Energia -= cantidad;
        return true;
    }
}