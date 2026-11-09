using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Sistema COMPARTIDO de JAWI: los 5 corazones y el puntaje.
// Solo debe haber uno. Pasa de una escena a otra sin perder los corazones.
public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Corazones")]
    public int vidasMaximas = 5;
    public int puntosPorCorazon = 200;

    public int Vidas { get; private set; }
    public bool EsGameOver { get; private set; }

    // Puntaje final: 200 puntos por cada corazón que quede
    public int PuntajeFinal => Vidas * puntosPorCorazon;

    // Avisos para otros scripts (por ejemplo, los corazones en pantalla)
    public event Action<int> AlCambiarVidas;
    public event Action AlGameOver;

    [Header("Fragmentos de oro")]
    public int fragmentosTotales = 3;
    public int Fragmentos { get; private set; }
    public event Action<int> AlCambiarFragmentos;

    [Header("Game Over")]
    [Tooltip("Escena donde empieza el juego (la del Bosque). Si lo dejas vacío, se repite la escena actual (útil para probar).")]
    public string primeraEscena = "";

    void Awake()
    {
        // Si ya existe otro GameManager (de una escena anterior), este sobra
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
        Vidas = vidasMaximas;
    }

    public void RecogerFragmento()
    {
        Fragmentos = Mathf.Min(fragmentosTotales, Fragmentos + 1);
        Debug.Log("Fragmento de oro: " + Fragmentos + " de " + fragmentosTotales);
        AlCambiarFragmentos?.Invoke(Fragmentos);
    }

    public void PerderVida()
    {
        if (EsGameOver) return;

        Vidas = Mathf.Max(0, Vidas - 1);
        Debug.Log("Jawi perdió un corazón. Le quedan " + Vidas);
        AlCambiarVidas?.Invoke(Vidas);

        if (Vidas == 0)
        {
            EsGameOver = true;
            Debug.Log("GAME OVER");
            AlGameOver?.Invoke();
        }
    }

    // Game Over: se pierde todo y el juego empieza de cero
    public void Reiniciar()
    {
        Vidas = vidasMaximas;
        EsGameOver = false;
        Fragmentos = 0;
        AlCambiarVidas?.Invoke(Vidas);
        AlCambiarFragmentos?.Invoke(Fragmentos);
        string escena = string.IsNullOrEmpty(primeraEscena)
            ? SceneManager.GetActiveScene().name
            : primeraEscena;
        SceneManager.LoadScene(escena);
    }
}
