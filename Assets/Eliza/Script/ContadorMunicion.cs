using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class ContadorMunicion : MonoBehaviour
{
    public TMP_Text texto;
    public Text textoSimple;
    public float energiaPorMunicion = 0.25f;

    EnergiaJawi energia;

    void Start()
    {
        energia = FindFirstObjectByType<EnergiaJawi>();
    }

    void LateUpdate()
    {
        if (energia == null) energia = FindFirstObjectByType<EnergiaJawi>();
        if (energia == null) return;

        int municion = Mathf.FloorToInt(energia.Energia / Mathf.Max(0.01f, energiaPorMunicion) + 0.001f);
        string valor = "x " + municion;
        if (texto != null) texto.text = valor;
        if (textoSimple != null) textoSimple.text = valor;
    }
}
