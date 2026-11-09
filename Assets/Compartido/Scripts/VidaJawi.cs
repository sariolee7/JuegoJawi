using System.Collections;
using UnityEngine;

// Va en Jawi. Cuando recibe daño: pierde un corazón, vuelve al último
// punto seguro y queda 1 segundo invulnerable (parpadeando).
[RequireComponent(typeof(Rigidbody2D))]
public class VidaJawi : MonoBehaviour
{
    public float tiempoInvulnerable = 1f;

    public bool EsInvulnerable { get; private set; }

    Vector3 puntoSeguro;
    Rigidbody2D rb;
    SpriteRenderer sprite;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        puntoSeguro = transform.position; // al empezar, el punto seguro es donde aparece
    }

    public void GuardarPuntoSeguro(Vector3 posicion)
    {
        puntoSeguro = posicion;
    }

    public void RecibirDano()
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

        // Volver al último punto seguro
        rb.linearVelocity = Vector2.zero;
        rb.position = puntoSeguro;
        transform.position = puntoSeguro;

        StartCoroutine(Invulnerable());
    }

    IEnumerator Invulnerable()
    {
        EsInvulnerable = true;
        float tiempo = 0f;
        while (tiempo < tiempoInvulnerable)
        {
            if (sprite != null) sprite.enabled = !sprite.enabled; // parpadeo
            yield return new WaitForSeconds(0.1f);
            tiempo += 0.1f;
        }
        if (sprite != null) sprite.enabled = true;
        EsInvulnerable = false;
    }
}
