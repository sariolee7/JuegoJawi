using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Roca : MonoBehaviour, IGolpeable
{
    public float gravedadMinima = 1f;
    public float gravedadMaxima = 3f;
    public float empujeInicialMinimo = 0f;
    public float empujeInicialMaximo = 2f;
    public float giro = 180f;
    public float duracion = 8f;
    public Color colorGolpeable = new Color(0.3f, 0.6f, 1f);
    public float velocidadDevuelta = 14f;
    public float distanciaParaGolpear = 1.5f;
    public GameObject efectoRomper;
    public float energiaPorGolpe = 0.25f;

    Rigidbody2D rb;
    SpriteRenderer sprite;
    Color colorOriginal;
    AccionesJawi jawi;
    Collider2D jawiCollider;
    EnergiaJawi energia;
    bool devuelta;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = Random.Range(gravedadMinima, gravedadMaxima);
        rb.linearVelocity = Vector2.down * Random.Range(empujeInicialMinimo, empujeInicialMaximo);
        rb.angularVelocity = Random.Range(-giro, giro);
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) colorOriginal = sprite.color;

        jawi = FindFirstObjectByType<AccionesJawi>();
        if (jawi != null)
        {
            jawiCollider = jawi.GetComponent<Collider2D>();
            energia = jawi.GetComponent<EnergiaJawi>();
        }

        Destroy(gameObject, duracion);
    }

    public void CambiarGravedad(float valor)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (devuelta) return;
        rb.gravityScale = valor;
        rb.linearVelocity = Vector2.zero;
    }

    bool SePuedeGolpear => !devuelta && energia != null && energia.Energia >= energiaPorGolpe - 0.001f;

    void Update()
    {
        if (devuelta)
        {
            if (FueraDeCamara()) Destroy(gameObject);
            return;
        }

        if (DebajoDeCamara()) { Destroy(gameObject); return; }

        if (sprite != null) sprite.color = SePuedeGolpear ? colorGolpeable : colorOriginal;

        if (SePuedeGolpear && JawiLaAlcanza()) Devolver();
    }

    bool JawiLaAlcanza()
    {
        if (jawi == null || !jawi.Atacando) return false;

        Vector2 pos = transform.position;
        Vector2 cercano = jawiCollider != null ? jawiCollider.ClosestPoint(pos) : (Vector2)jawi.transform.position;
        if (Vector2.Distance(pos, cercano) > distanciaParaGolpear) return false;

        float mira = Mathf.Sign(jawi.transform.localScale.x);
        float lado = Mathf.Sign(pos.x - jawi.transform.position.x);
        return mira == lado || Mathf.Abs(pos.x - jawi.transform.position.x) < 0.5f;
    }

    bool FueraDeCamara()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;
        Vector3 v = cam.WorldToViewportPoint(transform.position);
        return v.x < -0.1f || v.x > 1.1f || v.y < -0.1f || v.y > 1.1f;
    }

    bool DebajoDeCamara()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;
        return cam.WorldToViewportPoint(transform.position).y < -0.1f;
    }

    public void RecibirZarpazo()
    {
        if (SePuedeGolpear) Devolver();
    }

    void Devolver()
    {
        if (!energia.GastarEnergia(Mathf.Min(energiaPorGolpe, energia.Energia))) return;
        devuelta = true;
        if (sprite != null) sprite.color = colorGolpeable;

        Vector2 direccion = Vector2.up;
        Minero minero = FindFirstObjectByType<Minero>();
        if (minero != null && !minero.Vencido)
        {
            Collider2D colMinero = minero.GetComponent<Collider2D>();
            Vector2 objetivo = colMinero != null ? (Vector2)colMinero.bounds.center : (Vector2)minero.transform.position;
            direccion = (objetivo - (Vector2)transform.position).normalized;
            if (direccion.y < 0.3f) direccion = new Vector2(direccion.x, 0.3f).normalized;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = direccion * velocidadDevuelta;
        rb.angularVelocity = giro * 3f;
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (devuelta)
        {
            Minero minero = otro.GetComponentInParent<Minero>();
            if (minero != null)
            {
                minero.RecibirPiedra();
                Romper();
            }
            return;
        }

        VidaJawi vida = otro.GetComponentInParent<VidaJawi>();
        if (vida != null)
        {
            if (SePuedeGolpear && jawi != null && jawi.Atacando) { Devolver(); return; }
            vida.RecibirDano();
            Romper();
        }
    }

    void Romper()
    {
        if (efectoRomper != null) Instantiate(efectoRomper, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

