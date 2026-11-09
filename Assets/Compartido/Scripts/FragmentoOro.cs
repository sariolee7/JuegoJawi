using UnityEngine;

// Fragmento de oro: Jawi lo recoge al tocarlo.
// Necesita un Collider2D con "Is Trigger" activado.
public class FragmentoOro : MonoBehaviour
{
    [Header("Movimiento de flotar")]
    public float alturaFlotar = 0.15f;
    public float velocidadFlotar = 2f;

    [Header("Sonido (opcional)")]
    public AudioClip sonidoRecoger;

    Vector3 posicionInicial;
    bool recogido;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Sube y baja suavemente para que se note
        float y = Mathf.Sin(Time.time * velocidadFlotar) * alturaFlotar;
        transform.position = posicionInicial + new Vector3(0f, y, 0f);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (recogido) return;
        if (otro.GetComponentInParent<VidaJawi>() == null) return; // solo Jawi

        recogido = true;
        if (GameManager.Instancia != null) GameManager.Instancia.RecogerFragmento();
        if (sonidoRecoger != null) AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
        Destroy(gameObject);
    }
}
