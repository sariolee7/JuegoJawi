using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Planta marchita: SOLO reacciona al canto (el zarpazo no le hace nada).
// Revive poco a poco, cambia de color y, si le pones los dibujos,
// cambia del sprite marchito al sano.
public class PlantaMarchita : MonoBehaviour, ICantable
{
    [Header("Dibujos (opcional)")]
    [Tooltip("Dibujo de la planta sana. Si está vacío, solo cambia de color")]
    public Sprite spriteSana;

    [Header("Canto")]
    [Tooltip("Segundos que tarda en volverse una planta sana")]
    public float tiempoRestauracion = 5f;
    public Color colorRestaurado = Color.green;

    [Header("Aviso al terminar de revivir")]
    [Tooltip("Sonido opcional cuando termina de revivir")]
    public AudioClip sonidoRevivir;
    [Tooltip("Aquí puedes enganchar lo que quieras que pase al revivir (activar un objeto, abrir un camino...)")]
    public UnityEvent alRevivir;

    SpriteRenderer sprite;
    Vector3 escalaOriginal;
    bool restaurando = false;
    bool restaurado = false;

    public bool EstaRestaurado => restaurado;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        escalaOriginal = transform.localScale;
    }

    public void RecibirCanto()
    {
        // Si ya está reviviendo o ya revivió, cantar otra vez no hace nada
        if (restaurando || restaurado) return;
        StartCoroutine(Restaurar());
    }

    IEnumerator Restaurar()
    {
        restaurando = true;
        Debug.Log(name + " escuchó el canto y empieza a revivir...");

        Color colorInicial = sprite != null ? sprite.color : Color.gray;
        float tiempo = 0f;

        while (tiempo < tiempoRestauracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / tiempoRestauracion; // de 0 a 1
            if (sprite != null) sprite.color = Color.Lerp(colorInicial, colorRestaurado, progreso);
            yield return null; // esperar al siguiente frame
        }

        restaurando = false;
        restaurado = true;
        if (sprite != null && spriteSana != null)
        {
            sprite.sprite = spriteSana;
            sprite.color = Color.white; // con dibujo propio no hace falta teñirla
            colorRestaurado = Color.white;
        }
        Debug.Log(name + " ¡ya revivió! Es una planta sana");

        // --- Aviso: destello blanco + "salto" de tamaño + sonido ---
        if (sonidoRevivir != null) AudioSource.PlayClipAtPoint(sonidoRevivir, transform.position);
        alRevivir.Invoke();
        yield return StartCoroutine(AnimacionAviso());
    }

    IEnumerator AnimacionAviso()
    {
        float duracion = 0.5f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;            // de 0 a 1
            float salto = Mathf.Sin(t * Mathf.PI);  // sube y baja: 0 → 1 → 0

            transform.localScale = escalaOriginal * (1f + 0.4f * salto);
            if (sprite != null) sprite.color = Color.Lerp(colorRestaurado, Color.white, salto);
            yield return null;
        }

        transform.localScale = escalaOriginal;
        if (sprite != null) sprite.color = colorRestaurado;
    }
}
