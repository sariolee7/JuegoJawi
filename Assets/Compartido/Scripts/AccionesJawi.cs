using System.Collections;
using UnityEngine;

// Cualquier objeto que reaccione al zarpazo (invasores, camión, jaula del bebé...)
public interface IGolpeable
{
    void RecibirZarpazo();
}

// Cualquier objeto que reaccione al canto (plantas marchitas, plantas acuáticas...)
public interface ICantable
{
    void RecibirCanto();
}

// Acciones de Jawi: zarpazo (Espacio / B) y canto (E / Y).
[RequireComponent(typeof(ControlesJawi))]
public class AccionesJawi : MonoBehaviour
{
    [Header("Zarpazo")]
    [Tooltip("Hasta dónde llega el zarpazo, medido desde el borde de Jawi")]
    public float alcanceZarpazo = 1f;
    public float esperaZarpazo = 0.3f;
    public Color colorZarpazo = Color.red;
    [Tooltip("Si está marcado, Jawi brilla rojo al atacar (útil con el cuadrado; con la animación no hace falta)")]
    public bool colorearAlAtacar = false;
    [Tooltip("Segundos desde que presionas hasta que el zarpazo golpea (cuando baja la garra en la animación)")]
    public float retrasoGolpe = 0.4f;
    [Tooltip("Tiempo MÁXIMO del zarpazo. Normalmente termina solo cuando acaba la animación")]
    public float duracionMaximaZarpazo = 2f;
    [Tooltip("Si está marcado, Jawi se queda quieto mientras ataca")]
    public bool quietoAlAtacar = true;

    [Header("Canto")]
    public float radioCanto = 3f;
    [Tooltip("Tiempo MÁXIMO del canto. Normalmente termina solo cuando acaba la animación")]
    public float duracionCanto = 6f;
    [Tooltip("Segundos que tarda en aparecer y en desaparecer el brillo (cambio lento de color)")]
    public float transicionCanto = 0.7f;
    [Tooltip("Segundos antes de poder volver a cantar (desde que empieza el canto)")]
    public float esperaCanto = 2.5f;
    public Color colorCanto = Color.cyan;
    [Tooltip("Si está marcado, Jawi cambia de color al cantar (era útil con el cuadrado; con la animación de rugido no hace falta)")]
    public bool colorearAlCantar = false;
    [Tooltip("Si está marcado, Jawi se queda quieto mientras canta")]
    public bool quietoAlCantar = true;
    [Tooltip("Si está marcado, Jawi SOLO puede cantar cuando está junto a una flor (FlorInteractiva)")]
    public bool soloCantarCercaDeFlor = true;

    ControlesJawi controles;
    MovimientoJawi movimiento;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Color colorOriginal;
    Coroutine destelloActual;
    float proximoZarpazo;
    float proximoCanto;
    float finCanto;

    // true mientras Jawi está cantando (lo usa AnimacionJawi).
    // Termina cuando acaba la animación de canto, o como máximo a los "duracionCanto" segundos.
    public bool Cantando { get; private set; }

    // true mientras Jawi está atacando (lo usa AnimacionJawi)
    public bool Atacando { get; private set; }
    float finAtaque;

    public void TerminarZarpazo()
    {
        Atacando = false;
    }

    public void TerminarCanto()
    {
        Cantando = false;
    }

    void Awake()
    {
        controles = GetComponent<ControlesJawi>();
        movimiento = GetComponent<MovimientoJawi>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) colorOriginal = sprite.color;
    }

    void Update()
    {
        // Seguro: si pasó el tiempo máximo, el canto termina sí o sí
        if (Cantando && Time.time >= finCanto) Cantando = false;

        if (Atacando && Time.time >= finAtaque) Atacando = false;

        // Al terminar el canto o el zarpazo, Jawi puede volver a moverse
        if (movimiento != null && !movimiento.enabled && !Cantando && !Atacando)
        {
            movimiento.enabled = true;
        }

        if (controles.ZarpazoPresionado && Time.time >= proximoZarpazo && !Atacando && !Cantando)
        {
            proximoZarpazo = Time.time + esperaZarpazo;
            Zarpazo();
        }

        bool puedeCantarAqui = !soloCantarCercaDeFlor || FlorInteractiva.HayFlorCerca;
        if (controles.CantoPresionado && Time.time >= proximoCanto && !Atacando && !Cantando && puedeCantarAqui)
        {
            proximoCanto = Time.time + esperaCanto;
            Canto();
        }
    }

    // 1 = mirando a la derecha, -1 = mirando a la izquierda
    float Direccion => Mathf.Sign(transform.localScale.x);

    // El área del zarpazo empieza un poco DENTRO de Jawi y sale hacia adelante,
    // así golpea aunque el objetivo esté totalmente pegado a él.
    void CalcularAreaZarpazo(out Vector2 centro, out Vector2 tamano)
    {
        Collider2D miCollider = GetComponent<Collider2D>();
        Bounds b = miCollider != null ? miCollider.bounds : new Bounds(transform.position, Vector3.one);

        float desdeDentro = 0.3f;
        float ancho = alcanceZarpazo + desdeDentro;
        float bordeDelantero = b.center.x + b.extents.x * Direccion;

        centro = new Vector2(bordeDelantero - desdeDentro * Direccion + ancho / 2f * Direccion, b.center.y);
        tamano = new Vector2(ancho, b.size.y * 1.2f);
    }

    void Zarpazo()
    {
        Debug.Log("¡Zarpazo!");
        Atacando = true;
        finAtaque = Time.time + duracionMaximaZarpazo;

        if (quietoAlAtacar && movimiento != null)
        {
            movimiento.enabled = false;
            if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        StartCoroutine(GolpearDespues(retrasoGolpe));
    }

    IEnumerator GolpearDespues(float espera)
    {
        yield return new WaitForSeconds(espera);
        if (colorearAlAtacar) Destello(colorZarpazo, 0.12f, 0f);

        CalcularAreaZarpazo(out Vector2 centro, out Vector2 tamano);
        Collider2D[] tocados = Physics2D.OverlapBoxAll(centro, tamano, 0f);
        foreach (Collider2D c in tocados)
        {
            if (c.transform.IsChildOf(transform)) continue; // no golpearse a sí mismo
            foreach (IGolpeable g in c.GetComponents<IGolpeable>())
            {
                g.RecibirZarpazo();
            }
        }
    }

    void Canto()
    {
        Debug.Log("¡Canto!");
        Cantando = true;
        finCanto = Time.time + duracionCanto;
        if (colorearAlCantar) Destello(colorCanto, duracionCanto, transicionCanto);

        if (quietoAlCantar && movimiento != null)
        {
            movimiento.enabled = false;
            if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // Las flores junto a Jawi cambian y le dan energía
        FlorInteractiva.CantarAFloresCercanas();

        Collider2D[] tocados = Physics2D.OverlapCircleAll(transform.position, radioCanto);
        foreach (Collider2D c in tocados)
        {
            if (c.transform.IsChildOf(transform)) continue;
            foreach (ICantable cantable in c.GetComponents<ICantable>())
            {
                cantable.RecibirCanto();
            }
        }
    }

    void Destello(Color color, float duracion, float transicion)
    {
        if (sprite == null) return;
        if (destelloActual != null) StopCoroutine(destelloActual);
        destelloActual = StartCoroutine(CambiarColor(color, duracion, transicion));
    }

    // Cambia el color de Jawi: aparece poco a poco, se mantiene y vuelve poco a poco.
    // Si transicion es 0, el cambio es inmediato (como en el zarpazo).
    IEnumerator CambiarColor(Color color, float duracion, float transicion)
    {
        transicion = Mathf.Min(transicion, duracion / 2f);
        Color desde = sprite.color;

        // 1. Aparece el color lentamente
        float t = 0f;
        while (t < transicion)
        {
            t += Time.deltaTime;
            sprite.color = Color.Lerp(desde, color, t / transicion);
            yield return null;
        }
        sprite.color = color;

        // 2. Se mantiene el color
        yield return new WaitForSeconds(duracion - transicion * 2f);

        // 3. Vuelve al color original lentamente
        t = 0f;
        while (t < transicion)
        {
            t += Time.deltaTime;
            sprite.color = Color.Lerp(color, colorOriginal, t / transicion);
            yield return null;
        }
        sprite.color = colorOriginal;
        destelloActual = null;
    }

    // Dibuja el alcance en la ventana Scene cuando Jawi está seleccionado
    void OnDrawGizmosSelected()
    {
        CalcularAreaZarpazo(out Vector2 centro, out Vector2 tamano);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centro, tamano);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioCanto);
    }
}
