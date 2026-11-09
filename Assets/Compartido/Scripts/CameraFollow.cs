using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target (El personaje)")]
    public Transform target; // El personaje a seguir
    
    [Header("Límites de la cámara")]
    public float limiteIzquierdo = -10f;
    public float limiteDerecho = 10f;
    public float limiteInferior = -5f;
    public float limiteSuperior = 5f;
    
    [Header("Suavizado")]
    public float suavizado = 5f; // Qué tan suave sigue al personaje
    
    [Header("Offset (opcional)")]
    public Vector3 offset = new Vector3(0, 0, -10); // Distancia de la cámara al personaje
    
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Si no hay offset configurado, mantener la distancia actual
        if (offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        // Posición deseada (personaje + offset)
        Vector3 posicionDeseada = target.position + offset;
        
        // Aplicar límites
        float xLimitada = Mathf.Clamp(posicionDeseada.x, limiteIzquierdo, limiteDerecho);
        float yLimitada = Mathf.Clamp(posicionDeseada.y, limiteInferior, limiteSuperior);
        
        Vector3 posicionLimitada = new Vector3(xLimitada, yLimitada, posicionDeseada.z);
        
        // Movimiento suave (opcional, puedes usar suavizado = 0 para seguir exacto)
        transform.position = Vector3.Lerp(transform.position, posicionLimitada, suavizado * Time.deltaTime);
    }
}