using UnityEngine;
using UnityEngine.UI;

public class BarraFloresCanto : MonoBehaviour
{
    public Image[] flores;
    public Sprite florApagada;
    public Sprite florEncendida;
    public Color colorApagada = new Color(0.25f, 0.25f, 0.3f, 0.8f);
    public Color colorEncendida = Color.white;
    public Text contador;
    public float crecerAlEncender = 1.3f;

    EnergiaCanto energia;
    int mostradas = -1;
    float[] pulso;

    void Start()
    {
        energia = FindAnyObjectByType<EnergiaCanto>();
        pulso = new float[flores.Length];
        Actualizar(0, false);
    }

    void Update()
    {
        if (energia == null) energia = FindAnyObjectByType<EnergiaCanto>();
        int actuales = energia != null ? energia.plantasRestauradas : 0;
        if (actuales != mostradas) Actualizar(actuales, true);

        for (int i = 0; i < flores.Length; i++)
        {
            if (flores[i] == null) continue;
            pulso[i] = Mathf.MoveTowards(pulso[i], 0f, Time.unscaledDeltaTime * 2f);
            flores[i].rectTransform.localScale = Vector3.one * (1f + (crecerAlEncender - 1f) * pulso[i]);
        }
    }

    void Actualizar(int actuales, bool animar)
    {
        for (int i = 0; i < flores.Length; i++)
        {
            if (flores[i] == null) continue;
            bool encendida = i < actuales;
            if (encendida && florEncendida != null) flores[i].sprite = florEncendida;
            if (!encendida && florApagada != null) flores[i].sprite = florApagada;
            flores[i].color = encendida ? colorEncendida : colorApagada;
            if (animar && encendida && i >= mostradas && mostradas >= 0) pulso[i] = 1f;
        }

        int total = energia != null ? energia.plantasNecesarias : flores.Length;
        if (contador != null) contador.text = actuales + "/" + total;
        mostradas = actuales;
    }
}
