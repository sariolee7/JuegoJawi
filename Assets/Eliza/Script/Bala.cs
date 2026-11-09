using UnityEngine;

// Va en el prefab de la bala.
// Vuela en línea recta; si toca a Jawi le quita un corazón.
// Si choca con el piso o una pared, desaparece.
// Necesita un Collider2D (por ejemplo CircleCollider2D). El Rigidbody2D se pone solo.
[RequireComponent(typeof(Rigidbody2D))]
public class Bala : MonoBehaviour
{
    [Tooltip("Segundos antes de desaparecer si no choca con nada")]
    public float duracion = 3f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        Destroy(gameObject, duracion);
    }

    public void Disparar(Vector2 direccion, float velocidad)
    {
        transform.right = direccion; // la bala mira hacia donde vuela
        rb.linearVelocity = direccion * velocidad;
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        VidaJawi vida = otro.GetComponentInParent<VidaJawi>();
        if (vida != null)
        {
            vida.RecibirDano();
            Destroy(gameObject);
            return;
        }

        if (otro.isTrigger) return;                            // flores, puntos seguros, etc.
        if (otro.GetComponentInParent<Enemigo>() != null) return; // no le pega a otros enemigos
        Destroy(gameObject);                                   // piso, paredes, rocas
    }
}

