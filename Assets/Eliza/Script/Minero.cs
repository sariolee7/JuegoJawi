using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Minero : MonoBehaviour
{
    public int piedrasParaVencer = 5;
    public string triggerDano = "Dano";
    public string triggerMorir = "Morir";
    public float tiempoAntesDeDesaparecer = 1f;
    public AudioClip sonidoGolpe;
    public AudioClip sonidoVencido;
    public UnityEvent alVencer;

    public bool Vencido { get; private set; }
    public int GolpesRecibidos { get; private set; }

    SpriteRenderer sprite;
    Animator animator;
    Color colorOriginal;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        if (sprite != null) colorOriginal = sprite.color;
    }

    public void RecibirPiedra()
    {
        if (Vencido) return;

        GolpesRecibidos++;

        if (GolpesRecibidos >= piedrasParaVencer)
        {
            Vencido = true;
            if (sonidoVencido != null) AudioSource.PlayClipAtPoint(sonidoVencido, transform.position);
            Disparar(triggerMorir);
            alVencer?.Invoke();
            StartCoroutine(Desaparecer());
            return;
        }

        if (sonidoGolpe != null) AudioSource.PlayClipAtPoint(sonidoGolpe, transform.position);
        Disparar(triggerDano);
        StartCoroutine(DestelloRojo());
    }

    void Disparar(string nombre)
    {
        if (animator == null || string.IsNullOrEmpty(nombre)) return;
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == nombre && p.type == AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(nombre);
                return;
            }
        }
    }

    IEnumerator DestelloRojo()
    {
        if (sprite == null) yield break;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        if (!Vencido) sprite.color = colorOriginal;
    }

    IEnumerator Desaparecer()
    {
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        Peligro peligro = GetComponent<Peligro>();
        if (peligro != null) peligro.enabled = false;

        yield return new WaitForSeconds(tiempoAntesDeDesaparecer);

        float duracion = 0.8f;
        for (float t = 0f; t < duracion; t += Time.deltaTime)
        {
            if (sprite != null)
            {
                Color c = colorOriginal;
                c.a = 1f - t / duracion;
                sprite.color = c;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}

