using UnityEngine;

public class LluviaRocas : MonoBehaviour
{
    public GameObject rocaPrefab;
    public float tiempoMinimo = 1.5f;
    public float tiempoMaximo = 3.5f;
    public float distanciaHorizontal = 6f;
    public float alturaSobreJawi = 8f;
    public bool soloDentroDeZona = false;
    public Transform limiteIzquierdo;
    public Transform limiteDerecho;
    public Minero minero;
    public bool velocidadProgresiva = true;
    public float gravedadInicial = 0.3f;
    public float aumentoGravedad = 0.2f;
    public float cadaCuantosSegundos = 5f;
    public float gravedadMaxima = 4f;

    Transform jawi;
    bool hayMinero;
    float inicio;
    float siguiente;

    void Start()
    {
        VidaJawi vida = FindFirstObjectByType<VidaJawi>();
        if (vida != null) jawi = vida.transform;
        hayMinero = minero != null;
        inicio = Time.time;
        siguiente = Time.time + Random.Range(tiempoMinimo, tiempoMaximo);
    }

    void Update()
    {
        if (rocaPrefab == null || jawi == null) return;
        if (hayMinero && (minero == null || minero.Vencido)) return;
        if (Time.time < siguiente) return;
        siguiente = Time.time + Random.Range(tiempoMinimo, tiempoMaximo);

        if (soloDentroDeZona && !JawiEnZona()) return;

        float x = jawi.position.x + Random.Range(-distanciaHorizontal, distanciaHorizontal);
        if (limiteIzquierdo != null) x = Mathf.Max(x, limiteIzquierdo.position.x);
        if (limiteDerecho != null) x = Mathf.Min(x, limiteDerecho.position.x);

        Vector3 pos = new Vector3(x, jawi.position.y + alturaSobreJawi, 0f);
        GameObject nueva = Instantiate(rocaPrefab, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        if (velocidadProgresiva)
        {
            Roca roca = nueva.GetComponent<Roca>();
            if (roca != null) roca.CambiarGravedad(GravedadActual());
        }
    }

    public float GravedadActual()
    {
        int pasos = Mathf.FloorToInt((Time.time - inicio) / Mathf.Max(0.1f, cadaCuantosSegundos));
        return Mathf.Min(gravedadInicial + aumentoGravedad * pasos, gravedadMaxima);
    }

    bool JawiEnZona()
    {
        float x = jawi.position.x;
        if (limiteIzquierdo != null && x < limiteIzquierdo.position.x) return false;
        if (limiteDerecho != null && x > limiteDerecho.position.x) return false;
        return true;
    }
}


