using UnityEngine;

// Va en el vehículo enemigo (junto con Enemigo.cs, sin cambiarlo).
// Cuando Jawi está dentro del rango, dispara balas hacia él cada cierto tiempo.
// Cuando el enemigo es vencido, deja de disparar.
public class TorretaDisparo : MonoBehaviour
{
    [Header("Bala")]
    [Tooltip("Arrastra aquí el prefab de la bala (el que tiene el script Bala)")]
    public Bala prefabBala;
    [Tooltip("Objeto hijo vacío puesto en la punta del cañón. Si lo dejas vacío, la bala sale del centro")]
    public Transform puntaCanon;

    [Header("Disparo")]
    [Tooltip("Distancia a la que el vehículo empieza a disparar a Jawi")]
    public float rango = 6f;
    public float tiempoEntreDisparos = 1.5f;
    public float velocidadBala = 7f;

    [Header("Sonido (opcional)")]
    public AudioClip sonidoDisparo;

    VidaJawi jawi;
    Collider2D jawiCollider;
    SpriteRenderer sprite;
    Collider2D[] misColliders;
    float siguienteDisparo;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        misColliders = GetComponentsInChildren<Collider2D>();
        jawi = FindFirstObjectByType<VidaJawi>();
        if (jawi != null) jawiCollider = jawi.GetComponent<Collider2D>();
        siguienteDisparo = Time.time + tiempoEntreDisparos;
    }

    void Update()
    {
        if (prefabBala == null || jawi == null) return;
        if (EstaVencido()) return;
        if (Time.time < siguienteDisparo) return;

        Vector2 objetivo = jawiCollider != null ? (Vector2)jawiCollider.bounds.center : (Vector2)jawi.transform.position;
        if (Vector2.Distance(transform.position, objetivo) > rango) return;

        Disparar(objetivo);
        siguienteDisparo = Time.time + tiempoEntreDisparos;
    }

    void Disparar(Vector2 objetivo)
    {
        Vector3 salida = transform.position;
        if (puntaCanon != null)
        {
            // Si el dibujo está volteado, la punta del cañón también se voltea
            Vector3 local = puntaCanon.localPosition;
            if (sprite != null && sprite.flipX) local.x = -local.x;
            salida = transform.TransformPoint(local);
        }

        Vector2 direccion = (objetivo - (Vector2)salida).normalized;
        Bala bala = Instantiate(prefabBala, salida, Quaternion.identity);

        // La bala no choca con el propio vehículo
        Collider2D colBala = bala.GetComponent<Collider2D>();
        if (colBala != null)
            foreach (Collider2D c in misColliders) if (c != null) Physics2D.IgnoreCollision(colBala, c);

        bala.Disparar(direccion, velocidadBala);
        if (sonidoDisparo != null) AudioSource.PlayClipAtPoint(sonidoDisparo, salida);
    }

    // Enemigo.cs apaga los colliders cuando lo vencen: así sabemos que ya no debe disparar
    bool EstaVencido()
    {
        foreach (Collider2D c in misColliders) if (c != null && c.enabled) return false;
        return misColliders.Length > 0;
    }

    // Dibuja el rango en la escena (solo en el editor)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
