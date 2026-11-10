using UnityEngine;

public class DisparoVisual : MonoBehaviour
{
    public GameObject prefabBala;
    public Transform puntaArma;
    public float rango = 6f;
    public float tiempoEntreDisparos = 0.8f;
    public float velocidadBala = 8f;
    public float duracionBala = 1.5f;
    public bool haciaLaDerecha = true;

    Transform jawi;
    SpriteRenderer sprite;
    float siguienteDisparo;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        VidaJawi v = FindFirstObjectByType<VidaJawi>();
        if (v != null) jawi = v.transform;
    }

    void Update()
    {
        if (prefabBala == null || jawi == null) return;
        if (Time.time < siguienteDisparo) return;
        if (Vector2.Distance(transform.position, jawi.position) > rango) return;

        Disparar();
        siguienteDisparo = Time.time + tiempoEntreDisparos;
    }

    void Disparar()
    {
        Vector3 salida = transform.position;
        bool volteado = sprite != null && sprite.flipX;

        if (puntaArma != null)
        {
            Vector3 local = puntaArma.localPosition;
            if (volteado) local.x = -local.x;
            salida = transform.TransformPoint(local);
        }

        float lado = haciaLaDerecha ? 1f : -1f;
        if (volteado) lado = -lado;
        if (transform.lossyScale.x < 0) lado = -lado;
        Vector2 direccion = new Vector2(lado, 0f);

        GameObject b = Instantiate(prefabBala, salida, Quaternion.identity);
        b.transform.right = direccion;

        foreach (MonoBehaviour m in b.GetComponentsInChildren<MonoBehaviour>()) Destroy(m);
        foreach (Collider2D c in b.GetComponentsInChildren<Collider2D>()) Destroy(c);

        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        if (rb == null) rb = b.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = direccion * velocidadBala;

        Destroy(b, duracionBala);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
