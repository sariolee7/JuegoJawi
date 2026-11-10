using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FlorInteractiva : MonoBehaviour
{
    [SerializeField] private GameObject iconoE;
    [SerializeField] private string triggerCambio = "Cambiar";
    [SerializeField] private bool soloUnaVez = true;
    [Range(0f, 1f)]
    [SerializeField] private float energiaQueDa = 0.25f;
    [SerializeField] private float desaparecerDespues = 0f;

    static readonly List<FlorInteractiva> cercanas = new List<FlorInteractiva>();

    public static bool HayFlorCerca => cercanas.Count > 0;

    public static void CantarAFloresCercanas()
    {
        foreach (FlorInteractiva flor in cercanas.ToArray())
        {
            flor.Cambiar();
        }
    }

    private Animator animator;
    private EnergiaJawi energiaJawi;
    private bool yaCambiada;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (iconoE != null) iconoE.SetActive(false);
    }

    void OnDisable()
    {
        cercanas.Remove(this);
    }

    void Cambiar()
    {
        if (soloUnaVez && yaCambiada) return;

        animator.SetTrigger(triggerCambio);
        yaCambiada = true;
        if (energiaJawi != null) energiaJawi.GanarEnergia(energiaQueDa);

        if (soloUnaVez)
        {
            cercanas.Remove(this);
            if (iconoE != null) iconoE.SetActive(false);
        }

        if (desaparecerDespues > 0f)
        {
            cercanas.Remove(this);
            if (iconoE != null) iconoE.SetActive(false);
            Destroy(gameObject, desaparecerDespues);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        AccionesJawi jawi = other.GetComponentInParent<AccionesJawi>();
        if (jawi == null) return;
        if (soloUnaVez && yaCambiada) return;

        energiaJawi = jawi.GetComponent<EnergiaJawi>();
        if (!cercanas.Contains(this)) cercanas.Add(this);
        if (iconoE != null) iconoE.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<AccionesJawi>() == null) return;

        cercanas.Remove(this);
        if (iconoE != null) iconoE.SetActive(false);
    }
}
