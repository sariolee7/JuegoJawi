using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Enemigo (invasores): camina de un lado a otro y se vence con zarpazos.
// El canto NO le hace nada.
// Para que le quite un corazón a Jawi al tocarlo, ponle también el script "Peligro".
// Es SÓLIDO: Jawi choca con él y no lo puede atravesar.
// Necesita Rigidbody2D (se pone solo en Kinematic) y un Collider2D SIN "Is Trigger".
[RequireComponent(typeof(Rigidbody2D))]
public class Enemigo : MonoBehaviour, IGolpeable
{
    [Header("Vida")]
    public int zarpazosParaVencer = 2;

    [Header("Patrulla")]
    [Tooltip("Cuánto camina hacia cada lado desde donde lo pusiste. 0 = se queda quieto")]
    public float distanciaPatrulla = 2f;
    public float velocidad = 1.5f;
    [Tooltip("Marca si el dibujo mira a la izquierda en vez de a la derecha")]
    public bool dibujoMiraIzquierda = false;
    [Tooltip("Desmárcalo para que el dibujo NO se voltee al cambiar de dirección (por ejemplo el jeep: avanza y retrocede sin girar)")]
    public bool voltearAlCaminar = true;

    [Header("Al recibir zarpazo")]
    public float empujon = 0.4f;
    public float tiempoAturdido = 0.4f;

    [Header("Sonidos (opcional)")]
    public AudioClip sonidoGolpe;
    public AudioClip sonidoVencido;

    [Tooltip("Para agregar cosas extra al vencerlo (soltar un objeto, abrir un camino...)")]
    public UnityEvent alVencer;

    SpriteRenderer sprite;
    Rigidbody2D rb;
    Color colorOriginal;
    float inicioX;
    float direccion = 1f;
    float aturdidoHasta;
    float bloqueadoHasta;
    bool vencido;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) colorOriginal = sprite.color;
        inicioX = transform.position.x;

        // Kinematic: no lo empuja Jawi ni la gravedad, pero sí choca con él
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void FixedUpdate()
    {
        if (vencido || distanciaPatrulla <= 0f || Time.time < aturdidoHasta) return;
        if (Time.time < bloqueadoHasta) return; // Jawi le cierra el paso

        Vector2 p = rb.position;
        p.x += direccion * velocidad * Time.fixedDeltaTime;

        // Da la vuelta al llegar al borde de su patrulla
        if (p.x > inicioX + distanciaPatrulla) { p.x = inicioX + distanciaPatrulla; direccion = -1f; }
        if (p.x < inicioX - distanciaPatrulla) { p.x = inicioX - distanciaPatrulla; direccion = 1f; }
        rb.MovePosition(p);

        if (sprite != null && voltearAlCaminar) sprite.flipX = dibujoMiraIzquierda ? direccion > 0f : direccion < 0f;
    }

    // Si Jawi está justo enfrente, el enemigo se detiene (no lo empuja por todo el nivel).
    // Así se quedan frente a frente: ninguno puede pasar.
    void OnCollisionStay2D(Collision2D choque)
    {
        if (vencido || choque.collider.GetComponentInParent<VidaJawi>() == null) return;
        float ladoJawi = choque.transform.position.x - transform.position.x;
        if (Mathf.Sign(ladoJawi) == direccion) bloqueadoHasta = Time.time + 0.1f;
    }

    public void RecibirZarpazo()
    {
        if (vencido) return;

        zarpazosParaVencer--;
        Debug.Log(name + " recibió un zarpazo. Faltan " + zarpazosParaVencer);

        // Empujón hacia el lado contrario a Jawi
        VidaJawi jawi = FindFirstObjectByType<VidaJawi>();
        float lado = (jawi != null && jawi.transform.position.x > transform.position.x) ? -1f : 1f;
        rb.position += new Vector2(lado * empujon, 0f);

        if (zarpazosParaVencer <= 0)
        {
            vencido = true;
            if (sonidoVencido != null) AudioSource.PlayClipAtPoint(sonidoVencido, transform.position);
            alVencer?.Invoke();
            StartCoroutine(Desaparecer());
        }
        else
        {
            if (sonidoGolpe != null) AudioSource.PlayClipAtPoint(sonidoGolpe, transform.position);
            aturdidoHasta = Time.time + tiempoAturdido;
            StartCoroutine(DestelloRojo());
        }
    }

    IEnumerator DestelloRojo()
    {
        if (sprite == null) yield break;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        if (!vencido) sprite.color = colorOriginal;
    }

    IEnumerator Desaparecer()
    {
        Debug.Log(name + " fue vencido");

        // Ya no hace daño ni estorba
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        Peligro peligro = GetComponent<Peligro>();
        if (peligro != null) peligro.enabled = false;

        // Parpadea y se desvanece
        float duracion = 0.6f;
        for (float t = 0f; t < duracion; t += Time.deltaTime)
        {
            if (sprite != null)
            {
                Color c = colorOriginal;
                c.a = 1f - t / duracion;
                sprite.color = c;
                sprite.enabled = Mathf.FloorToInt(t * 20f) % 2 == 0;
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    // Dibuja la zona de patrulla en la escena (solo en el editor)
    void OnDrawGizmosSelected()
    {
        float x = Application.isPlaying ? inicioX : transform.position.x;
        Vector3 a = new Vector3(x - distanciaPatrulla, transform.position.y, 0f);
        Vector3 b = new Vector3(x + distanciaPatrulla, transform.position.y, 0f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawWireSphere(a, 0.1f);
        Gizmos.DrawWireSphere(b, 0.1f);
    }
}

