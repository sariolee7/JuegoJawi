using UnityEngine;
using UnityEngine.SceneManagement;

public class ParedJefe : MonoBehaviour
{
    public string escena = "JEFE";

    bool cambiando;

    void OnCollisionEnter2D(Collision2D choque)
    {
        Revisar(choque.collider);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        Revisar(otro);
    }

    void Revisar(Collider2D otro)
    {
        if (cambiando) return;
        if (otro.GetComponentInParent<VidaJawi>() == null) return;

        cambiando = true;
        SceneManager.LoadScene(escena);
    }
}