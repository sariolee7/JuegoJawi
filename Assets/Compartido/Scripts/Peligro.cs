using UnityEngine;

// Ponlo en cualquier cosa que le quite un corazón a Jawi:
// espinas, agua turbia, invasores...
// Funciona con colliders normales y con colliders "Is Trigger".
public class Peligro : MonoBehaviour
{
    [Tooltip("Segundos que Jawi tiene que estar tocándolo para perder un corazón. 0 = al instante (espinas, agua). Para enemigos, unos 2")]
    public float tiempoParaDanar = 0f;

    float inicioToque = -1f; // -1 = Jawi no lo está tocando

    void OnCollisionEnter2D(Collision2D choque) { Tocando(choque.collider); }
    void OnCollisionStay2D(Collision2D choque) { Tocando(choque.collider); }
    void OnCollisionExit2D(Collision2D choque) { DejoDeTocar(choque.collider); }
    void OnTriggerEnter2D(Collider2D otro) { Tocando(otro); }
    void OnTriggerStay2D(Collider2D otro) { Tocando(otro); }
    void OnTriggerExit2D(Collider2D otro) { DejoDeTocar(otro); }

    void Tocando(Collider2D otro)
    {
        if (!enabled) return;
        VidaJawi vida = otro.GetComponentInParent<VidaJawi>();
        if (vida == null) return;

        if (inicioToque < 0f) inicioToque = Time.time;
        if (Time.time - inicioToque >= tiempoParaDanar)
        {
            vida.RecibirDano();
            inicioToque = -1f;
        }
    }

    void DejoDeTocar(Collider2D otro)
    {
        if (otro.GetComponentInParent<VidaJawi>() != null) inicioToque = -1f;
    }
}
