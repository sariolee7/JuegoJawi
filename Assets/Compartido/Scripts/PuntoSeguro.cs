using UnityEngine;

// Cuando Jawi pasa por aquí, este se vuelve su punto de reaparición.
// El collider de este objeto debe tener marcado "Is Trigger".
public class PuntoSeguro : MonoBehaviour
{
    public Color colorActivado = Color.green;
    bool activado = false;

    void OnTriggerEnter2D(Collider2D otro)
    {
        VidaJawi vida = otro.GetComponentInParent<VidaJawi>();
        if (vida == null) return;

        vida.GuardarPuntoSeguro(transform.position);

        if (!activado)
        {
            activado = true;
            Debug.Log("Punto seguro guardado");
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.color = colorActivado;
        }
    }
}
