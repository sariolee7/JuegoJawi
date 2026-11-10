using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BarreraOscura : MonoBehaviour
{
    public float distanciaParaCantar = 3f;
    public Sprite[] cuadrosDesvanecer;
    public float cuadrosPorSegundo = 10f;
    public float esperaAntesDeRomper = 0.4f;

    public GameObject iconoE;
    public GameObject avisoSinEnergia;
    public GameObject efectoGranCanto;

    public AudioClip sonidoRomper;
    public UnityEvent alRomper;

    SpriteRenderer sprite;
    Collider2D[] colisiones;
    EnergiaCanto energia;
    ControlesJawi controles;
    AccionesJawi acciones;
    bool soloFlorOriginal;
    bool rota;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        colisiones = GetComponents<Collider2D>();
        energia = FindAnyObjectByType<EnergiaCanto>();
        if (energia != null)
        {
            controles = energia.GetComponent<ControlesJawi>();
            acciones = energia.GetComponent<AccionesJawi>();
            if (acciones != null) soloFlorOriginal = acciones.soloCantarCercaDeFlor;
        }
        if (iconoE != null) iconoE.SetActive(false);
        if (avisoSinEnergia != null) avisoSinEnergia.SetActive(false);
    }

    void Update()
    {
        if (rota || energia == null) return;

        bool cerca = Distancia() <= distanciaParaCantar;

        if (acciones != null)
            acciones.soloCantarCercaDeFlor = (cerca && energia.tieneGranCanto) ? false : soloFlorOriginal;

        if (iconoE != null) iconoE.SetActive(cerca && energia.tieneGranCanto);
        if (avisoSinEnergia != null) avisoSinEnergia.SetActive(cerca && !energia.tieneGranCanto);

        if (cerca && energia.tieneGranCanto && controles != null && controles.CantoPresionado)
            StartCoroutine(Romper());
    }

    float Distancia()
    {
        Vector2 jawi = energia.transform.position;
        float menor = Vector2.Distance(jawi, transform.position);
        foreach (Collider2D col in colisiones)
        {
            if (!col.enabled) continue;
            float d = Vector2.Distance(jawi, col.ClosestPoint(jawi));
            if (d < menor) menor = d;
        }
        return menor;
    }

    IEnumerator Romper()
    {
        rota = true;
        if (iconoE != null) iconoE.SetActive(false);
        if (avisoSinEnergia != null) avisoSinEnergia.SetActive(false);

        if (efectoGranCanto != null)
            Instantiate(efectoGranCanto, energia.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(esperaAntesDeRomper);

        if (sonidoRomper != null) AudioSource.PlayClipAtPoint(sonidoRomper, transform.position);

        if (sprite != null && cuadrosDesvanecer != null && cuadrosDesvanecer.Length > 0)
        {
            float espera = 1f / Mathf.Max(1f, cuadrosPorSegundo);
            foreach (Sprite cuadro in cuadrosDesvanecer)
            {
                sprite.sprite = cuadro;
                yield return new WaitForSeconds(espera);
            }
        }
        else if (sprite != null)
        {
            Color c = sprite.color;
            for (float t = 0f; t < 1f; t += Time.deltaTime)
            {
                sprite.color = new Color(c.r, c.g, c.b, 1f - t);
                yield return null;
            }
            sprite.enabled = false;
        }

        foreach (Collider2D col in colisiones) col.enabled = false;
        if (acciones != null) acciones.soloCantarCercaDeFlor = soloFlorOriginal;
        alRomper.Invoke();
    }
}
