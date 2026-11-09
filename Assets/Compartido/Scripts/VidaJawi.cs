using System.Collections;
using UnityEngine;

// Va en Jawi. Cuando recibe daño: pierde un corazón y queda 1 segundo
// invulnerable (parpadeando), en el mismo lugar donde estaba.
// Solo vuelve al último punto seguro si el peligro lo pide (agua, abismos).
// Cuando pierde todos los corazones, el GameManager reinicia el juego.
// Mientras parpadea, no puede moverse ni atacar ni cantar.
[RequireComponent(typeof(Rigidbody2D))]
public class VidaJawi : MonoBehaviour
{
    public float tiempoInvulnerable = 1f;
    [Tooltip("Si está marcado, Jawi no se puede mover mientras parpadea")]
    public bool quietoMientrasParpadea = false;

    public bool EsInvulnerable { get; private set; }

    Vector3 puntoSeguro;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    MovimientoJawi movimiento;
    AccionesJawi acciones;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        movimiento = GetComponent<MovimientoJawi>();
        acciones = GetComponent<AccionesJawi>();
        puntoSeguro = transform.position; // al empezar, el punto seguro es donde aparece
    }

    public void GuardarPuntoSeguro(Vector3 posicion)
    {
        puntoSeguro = posicion;
    }

    public void RecibirDano() { RecibirDano(false); }

    public void RecibirDano(bool volverAlPuntoSeguro)
    {
        if (EsInvulnerable) return;

        GameManager gm = GameManager.Instancia;
        if (gm == null)
        {
            Debug.LogWarning("No hay GameManager en la escena. Agrega el objeto GameManager.");
            return;
        }
        if (gm.EsGameOver) return;

        gm.PerderVida();

        // Solo vuelve al último punto seguro si el peligro lo pide (agua, abismos)
        if (volverAlPuntoSeguro)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = puntoSeguro;
            transform.position = puntoSeguro;
        }

        StartCoroutine(Invulnerable());
    }

    IEnumerator Invulnerable()
    {
        EsInvulnerable = true;
        PuedeControlarse(false);

        float tiempo = 0f;
        while (tiempo < tiempoInvulnerable)
        {
            if (sprite != null) sprite.enabled = !sprite.enabled; // parpadeo
            yield return new WaitForSeconds(0.1f);
            tiempo += 0.1f;
        }
        if (sprite != null) sprite.enabled = true;
        EsInvulnerable = false;
        PuedeControlarse(true);
    }

    void PuedeControlarse(bool si)
    {
        if (!quietoMientrasParpadea && !si) return;

        if (!si) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        if (movimiento != null) movimiento.enabled = si;
        if (acciones != null) acciones.enabled = si;
    }
}

