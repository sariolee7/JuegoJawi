using UnityEngine;

// Movimiento de Jawi: caminar y saltar.
// Necesita ControlesJawi en el mismo objeto (Unity lo agrega solo).
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ControlesJawi))]
public class MovimientoJawi : MonoBehaviour
{
    [Header("Caminar")]
    public float velocidad = 6f;

    [Header("Saltar")]
    public float fuerzaSalto = 13f;
    public float gravedad = 3f;

    // Hacia dónde mira Jawi (lo usará el zarpazo)
    public bool MirandoDerecha { get; private set; } = true;
    public bool EnSuelo => rb.IsTouching(filtroSuelo);

    Rigidbody2D rb;
    ControlesJawi controles;
    ContactFilter2D filtroSuelo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controles = GetComponent<ControlesJawi>();

        // Ajustes del cuerpo físico para un plataformas
        rb.gravityScale = gravedad;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Sin fricción, para que Jawi no se quede pegado a las paredes
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.sharedMaterial = new PhysicsMaterial2D("SinFriccion") { friction = 0f, bounciness = 0f };
        }

        // "Suelo" = algo que Jawi toca por debajo
        filtroSuelo = new ContactFilter2D();
        filtroSuelo.SetNormalAngle(45f, 135f);
    }

    void Update()
    {
        // Saltar solo si está tocando el suelo
        if (controles.SaltarPresionado && EnSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // Salto más corto si suelta el botón antes
        if (!controles.SaltarSostenido && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Voltear a Jawi según hacia dónde camina
        float h = controles.Horizontal;
        if (h > 0.1f && !MirandoDerecha) Voltear();
        else if (h < -0.1f && MirandoDerecha) Voltear();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(controles.Horizontal * velocidad, rb.linearVelocity.y);
    }

    void Voltear()
    {
        MirandoDerecha = !MirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1f;
        transform.localScale = escala;
    }
}
