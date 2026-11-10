using UnityEngine;

public class JawiDentroDePantalla : MonoBehaviour
{
    public float margen = 0.1f;
    public bool limitarArriba = false;

    Rigidbody2D rb;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector2 abajoIzq = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector2 arribaDer = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        Vector2 pos = rb != null ? rb.position : (Vector2)transform.position;
        float mitadAncho = 0f;
        float mitadAlto = 0f;
        Vector2 desfase = Vector2.zero;
        if (col != null)
        {
            mitadAncho = col.bounds.extents.x;
            mitadAlto = col.bounds.extents.y;
            desfase = (Vector2)col.bounds.center - pos;
        }

        float minX = abajoIzq.x + mitadAncho + margen - desfase.x;
        float maxX = arribaDer.x - mitadAncho - margen - desfase.x;
        float maxY = arribaDer.y - mitadAlto - margen - desfase.y;

        Vector2 nueva = pos;
        nueva.x = Mathf.Clamp(pos.x, minX, maxX);
        if (limitarArriba) nueva.y = Mathf.Min(pos.y, maxY);

        if (nueva == pos) return;

        if (rb != null)
        {
            Vector2 v = rb.linearVelocity;
            if ((nueva.x > pos.x && v.x < 0f) || (nueva.x < pos.x && v.x > 0f)) v.x = 0f;
            if (nueva.y < pos.y && v.y > 0f) v.y = 0f;
            rb.linearVelocity = v;
            rb.position = nueva;
        }
        else
        {
            transform.position = new Vector3(nueva.x, nueva.y, transform.position.z);
        }
    }
}

