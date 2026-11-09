using UnityEngine;

// Cambia la animación de Jawi según lo que está haciendo.
// Estados: Quieto, Caminar, Canto (el rugido), Zarpazo y Saltar.
// Los nombres deben ser IGUALES a los nombres de los estados en la ventana Animator.
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AnimacionJawi : MonoBehaviour
{
    [Header("Nombres de los estados en el Animator")]
    public string estadoQuieto = "Quieto";
    public string estadoCaminar = "Caminar";
    public string estadoCanto = "Canto";
    public string estadoZarpazo = "Zarpazo";
    public string estadoSaltar = "Saltar";

    [Tooltip("Segundos sin tocar el suelo antes de mostrar el salto al caer de un borde (evita parpadeos en bajaditas)")]
    public float margenAire = 0.08f;

    [Tooltip("Velocidad mínima para considerar que está caminando")]
    public float velocidadMinima = 0.1f;

    Animator anim;
    Rigidbody2D rb;
    AccionesJawi acciones;
    MovimientoJawi movimiento;
    string estadoActual;
    float tiempoEnAire;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        acciones = GetComponent<AccionesJawi>();
        movimiento = GetComponent<MovimientoJawi>();
    }

    void Update()
    {
        if (acciones != null && acciones.Cantando)
        {
            CambiarEstado(estadoCanto);

            // ¿Ya terminó la animación de canto? Entonces avisamos que acabó
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(estadoCanto) && info.normalizedTime >= 1f)
            {
                acciones.TerminarCanto();
            }
            else
            {
                return;
            }
        }

        if (acciones != null && acciones.Atacando)
        {
            CambiarEstado(estadoZarpazo);

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(estadoZarpazo) && info.normalizedTime >= 1f)
            {
                acciones.TerminarZarpazo();
            }
            else
            {
                return;
            }
        }

        bool caminando = Mathf.Abs(rb.linearVelocity.x) > velocidadMinima;
        bool enSuelo = movimiento == null || movimiento.EnSuelo;

        // --- En el aire: animación de salto ---
        tiempoEnAire = enSuelo ? 0f : tiempoEnAire + Time.deltaTime;
        bool subiendo = rb.linearVelocity.y > 0.1f;
        if (!enSuelo && (subiendo || tiempoEnAire > margenAire))
        {
            CambiarEstado(estadoSaltar);
            return;
        }

        // --- En el suelo: caminar o quieto ---
        CambiarEstado(caminando ? estadoCaminar : estadoQuieto);
    }

    public void CambiarEstado(string nuevo)
    {
        if (nuevo == estadoActual) return;
        estadoActual = nuevo;
        anim.Play(nuevo, 0, 0f); // empieza la animación desde el principio
    }
}
