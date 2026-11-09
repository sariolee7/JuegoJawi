using UnityEngine;

// Pon este script en el objeto del fondo.
// El fondo sigue a la cámara para que nunca se vea el azul detrás.
// No lo pongas como hijo de Jawi ni de la cámara: Jawi se voltea y el fondo se voltearía también.
public class FondoSigueCamara : MonoBehaviour
{
    [Tooltip("0 = el fondo queda pegado a la cámara (siempre cubre la pantalla). 0.1 a 0.3 = se mueve un poquito, da sensación de profundidad.")]
    [Range(0f, 0.5f)]
    public float efectoProfundidad = 0.1f;

    Transform camara;
    Vector3 inicioCamara;
    Vector3 inicioFondo;

    void Start()
    {
        camara = Camera.main.transform;
        inicioCamara = camara.position;
        inicioFondo = transform.position;
    }

    void LateUpdate()
    {
        Vector3 recorrido = camara.position - inicioCamara;
        Vector3 pos = inicioFondo + recorrido * (1f - efectoProfundidad);
        pos.z = transform.position.z;
        transform.position = pos;
    }
}
