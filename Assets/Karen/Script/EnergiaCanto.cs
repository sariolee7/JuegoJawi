using UnityEngine;
using UnityEngine.Events;

public class EnergiaCanto : MonoBehaviour
{
    public int plantasNecesarias = 8;
    public int plantasRestauradas;
    public bool tieneGranCanto;

    public UnityEvent alRestaurarPlanta;
    public UnityEvent alCompletar;

    EnergiaJawi energia;
    int floresAlEmpezar;

    void Start()
    {
        energia = GetComponent<EnergiaJawi>();
        if (energia != null) floresAlEmpezar = energia.FloresCantadas;
        plantasRestauradas = 0;
        tieneGranCanto = false;
    }

    void Update()
    {
        if (energia == null) return;

        int total = Mathf.Clamp(energia.FloresCantadas - floresAlEmpezar, 0, plantasNecesarias);
        if (total == plantasRestauradas) return;

        plantasRestauradas = total;
        alRestaurarPlanta.Invoke();

        if (!tieneGranCanto && plantasRestauradas >= plantasNecesarias)
        {
            tieneGranCanto = true;
            alCompletar.Invoke();
        }
    }
}
