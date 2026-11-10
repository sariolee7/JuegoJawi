using UnityEngine;

public class FlorDesaparece : MonoBehaviour
{
    public float segundosDespuesDeFlorecer = 2f;

    EnergiaJawi energia;
    bool jawiDentro;
    int floresAlEntrar;
    bool florecio;

    void Start()
    {
        energia = FindFirstObjectByType<EnergiaJawi>();
    }

    void Update()
    {
        if (florecio || !jawiDentro || energia == null) return;

        if (energia.FloresCantadas > floresAlEntrar)
        {
            florecio = true;
            Destroy(gameObject, segundosDespuesDeFlorecer);
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<AccionesJawi>() == null) return;
        if (energia == null) energia = FindFirstObjectByType<EnergiaJawi>();
        if (energia == null) return;
        jawiDentro = true;
        floresAlEntrar = energia.FloresCantadas;
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.GetComponentInParent<AccionesJawi>() == null) return;
        jawiDentro = false;
    }
}
