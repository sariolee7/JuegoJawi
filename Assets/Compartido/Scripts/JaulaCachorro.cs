using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Jaula con un cachorro atrapado. Jawi la abre con Espacio (el zarpazo).
// Estructura: este objeto es la JAULA (con su Collider2D) y tiene
// como HIJO al cachorro (otro objeto con su sprite).
public class JaulaCachorro : MonoBehaviour, IGolpeable
{
    [Tooltip("Zarpazos necesarios para romper la jaula")]
    public int golpesParaAbrir = 1;

    [Tooltip("Arrastra aquí el objeto hijo del cachorro")]
    public Transform cachorro;

    [Header("Cuando el cachorro sale")]
    [Tooltip("Altura de los saltitos mientras corre")]
    public float alturaSaltitos = 0.15f;
    [Tooltip("Cuántos saltitos da mientras se aleja")]
    public int cantidadSaltitos = 4;
    public float distanciaHuida = 3f;
    public float duracionHuida = 1.2f;

    [Header("Sonido (opcional)")]
    public AudioClip sonidoLiberar;

    [Tooltip("Para agregar cosas extra al liberar (sumar puntos, abrir una puerta...)")]
    public UnityEvent alLiberar;

    public bool Liberado { get; private set; }

    int golpes;
    SpriteRenderer spriteJaula;

    void Awake()
    {
        spriteJaula = GetComponent<SpriteRenderer>();
    }

    public void RecibirZarpazo()
    {
        if (Liberado) return;

        golpes++;
        if (golpes < golpesParaAbrir)
        {
            StartCoroutine(Temblar());
            return;
        }

        Liberado = true;
        Debug.Log("¡Cachorro liberado!");
        if (sonidoLiberar != null) AudioSource.PlayClipAtPoint(sonidoLiberar, transform.position);
        alLiberar?.Invoke();

        // Guardamos dónde está el piso (la base de la jaula) antes de apagarla
        float piso = transform.position.y;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) piso = col.bounds.min.y;

        // La jaula desaparece y ya no estorba
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        if (spriteJaula != null) spriteJaula.enabled = false;

        if (cachorro != null) StartCoroutine(CachorroHuye(piso));
    }

    IEnumerator Temblar()
    {
        Vector3 inicio = transform.position;
        for (float t = 0f; t < 0.2f; t += Time.deltaTime)
        {
            transform.position = inicio + new Vector3(Random.Range(-0.05f, 0.05f), 0f, 0f);
            yield return null;
        }
        transform.position = inicio;
    }

    IEnumerator CachorroHuye(float piso)
    {
        // Lo separamos de la jaula para que se mueva solo
        cachorro.SetParent(null);
        SpriteRenderer sr = cachorro.GetComponent<SpriteRenderer>();

        // Lo bajamos al piso para que corra por el suelo
        Vector3 inicio = cachorro.position;
        if (sr != null) inicio.y += piso - sr.bounds.min.y;

        // Corre en la dirección contraria a Jawi... o a la derecha si no lo encuentra
        float direccion = 1f;
        VidaJawi jawi = FindFirstObjectByType<VidaJawi>();
        if (jawi != null && jawi.transform.position.x > inicio.x) direccion = -1f;
        if (sr != null) sr.flipX = direccion < 0f;

        for (float t = 0f; t < duracionHuida; t += Time.deltaTime)
        {
            float p = t / duracionHuida;
            float x = direccion * distanciaHuida * p;
            float y = Mathf.Abs(Mathf.Sin(p * Mathf.PI * cantidadSaltitos)) * alturaSaltitos; // saltitos de alegría
            cachorro.position = inicio + new Vector3(x, y, 0f);

            // Se desvanece al final
            if (sr != null)
            {
                Color c = sr.color;
                c.a = p < 0.6f ? 1f : 1f - (p - 0.6f) / 0.4f;
                sr.color = c;
            }
            yield return null;
        }

        Destroy(cachorro.gameObject);
        Destroy(gameObject);
    }
}
