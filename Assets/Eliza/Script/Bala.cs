using UnityEngine;

// Va en el prefab de la bala.
// Vuela en línea recta; si toca a Jawi le quita un corazón.
// Si Jawi le da un zarpazo, la bala se devuelve y le hace daño al enemigo
// (cuenta como un zarpazo para vencerlo).
// Si choca con el piso o una pared, desaparece.
// Necesita un Collider2D (por ejemplo CircleCollider2D). El Rigidbody2D se pone solo.
[RequireComponent(typeof(Rigidbody2D))]
public class Bala : MonoBehaviour, IGolpeable
{
    [Tooltip("Segundos antes de desaparecer si no choca con nada")]
    public float duracion = 4f;
    [Tooltip("Qué tan rápido vuelve cuando Jawi la rasguña (1 = igual de rápido)")]
    public float multiplicadorDevuelta = 1.5f;
    [Tooltip("Si Jawi está dando un zarpazo y la bala está a esta distancia (o menos) delante de él, la bala se devuelve")]
    public float distanciaParaDevolver = 1.5f;

    public bool Devuelta { get; private set; }

    Rigidbody2D rb;
    Transform tirador;
    AccionesJawi jawi;
    Collider2D jawiCollider;
    float velocidad;
    float tiempoFin;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        tiempoFin = Time.time + duracion;

        jawi = FindFirstObjectByType<AccionesJawi>();
        if (jawi != null) jawiCollider = jawi.GetComponent<Collider2D>();
    }

    public void Disparar(Vector2 direccion, float vel, Transform quienDispara = null)
    {
        tirador = quienDispara;
        velocidad = vel;
        Mover(direccion, velocidad);
    }

    void Mover(Vector2 direccion, float vel)
    {
        transform.right = direccion; // la bala mira hacia donde vuela
        rb.linearVelocity = direccion * vel;
    }

    void Update()
    {
        if (Time.time > tiempoFin) { Destroy(gameObject); return; }

        // Si Jawi está dando un zarpazo y la bala está cerca y delante de él, se devuelve
        if (!Devuelta && JawiLaPuedeRasgunar()) RecibirZarpazo();
    }

    bool JawiLaPuedeRasgunar()
    {
        if (jawi == null || !jawi.Atacando) return false;

        Vector2 pos = transform.position;
        Vector2 cercano = jawiCollider != null ? jawiCollider.ClosestPoint(pos) : (Vector2)jawi.transform.position;
        if (Vector2.Distance(pos, cercano) > distanciaParaDevolver) return false;

        float mira = Mathf.Sign(jawi.transform.localScale.x);              // 1 derecha, -1 izquierda
        float lado = Mathf.Sign(pos.x - jawi.transform.position.x);        // dónde está la bala
        return mira == lado;
    }

    // Jawi le da un zarpazo: la bala vuelve hacia el que la disparó
    public void RecibirZarpazo()
    {
        if (Devuelta) return;
        Devuelta = true;

        Vector2 direccion = -rb.linearVelocity.normalized;
        if (tirador != null)
        {
            Collider2D colTirador = tirador.GetComponent<Collider2D>();
            Vector2 objetivo = colTirador != null ? (Vector2)colTirador.bounds.center : (Vector2)tirador.position;
            direccion = (objetivo - (Vector2)transform.position).normalized;
        }
        Mover(direccion, velocidad * multiplicadorDevuelta);
        tiempoFin = Time.time + duracion;
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        Enemigo enemigo = otro.GetComponentInParent<Enemigo>();

        if (!Devuelta)
        {
            VidaJawi vida = otro.GetComponentInParent<VidaJawi>();
            if (vida != null)
            {
                if (JawiLaPuedeRasgunar()) { RecibirZarpazo(); return; } // la rasguñó justo a tiempo
                vida.RecibirDano();
                Destroy(gameObject);
                return;
            }
            if (enemigo != null) return; // al salir no choca con el vehículo
        }
        else
        {
            if (otro.GetComponentInParent<VidaJawi>() != null) return; // ya no le hace daño a Jawi
            if (enemigo != null)
            {
                enemigo.RecibirZarpazo(); // cuenta como un ataque
                Destroy(gameObject);
                return;
            }
        }

        if (otro.isTrigger) return; // flores, puntos seguros, etc.
        Destroy(gameObject);        // piso, paredes, rocas
    }
}


