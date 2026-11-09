using UnityEngine;

// Pon este script en cada llanta (hija del carro).
// La llanta gira sola según cuánto se mueva el carro a la izquierda o derecha.
// Si el carro se voltea con flipX (por ejemplo con el script Enemigo), la llanta se cambia de lado sola.
public class RuedaGira : MonoBehaviour
{
    [Tooltip("Radio de la llanta en unidades de Unity (con PPU 250 es 0.66)")]
    public float radio = 0.66f;

    Vector3 ultimaPosicion;
    SpriteRenderer spriteCarro;
    float xOriginal;

    void Start()
    {
        ultimaPosicion = transform.position;
        xOriginal = transform.localPosition.x;
        if (transform.parent != null)
            spriteCarro = transform.parent.GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Si el carro está volteado, la llanta pasa al otro lado
        if (spriteCarro != null)
        {
            Vector3 p = transform.localPosition;
            p.x = spriteCarro.flipX ? -xOriginal : xOriginal;
            transform.localPosition = p;
        }

        float avance = transform.position.x - ultimaPosicion.x;
        ultimaPosicion = transform.position;

        float radioReal = radio * Mathf.Abs(transform.lossyScale.x); // si agrandas el carro, la llanta también
        float grados = -avance / radioReal * Mathf.Rad2Deg;
        transform.Rotate(0f, 0f, grados);
    }
}
