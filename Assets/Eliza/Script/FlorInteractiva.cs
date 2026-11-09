using System.Collections.Generic;
using UnityEngine;

// Flor con la que Jawi interactúa CANTANDO (E / botón Y).
// Jawi solo puede cantar cuando está dentro del trigger de una flor.
// Al cantarle: la flor cambia (animación), se esconde el ícono E
// y la barra de energía de Jawi sube.
// Necesita un Collider2D con "Is Trigger" activado.
[RequireComponent(typeof(Animator))]
public class FlorInteractiva : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject iconoE;          // Hijo con la imagen del botón E

    [Header("Animación")]
    [SerializeField] private string triggerCambio = "Cambiar";
    [SerializeField] private bool soloUnaVez = true;

    [Header("Energía")]
    [Tooltip("Cuánto llena la barra esta flor (0.25 = 25%)")]
    [Range(0f, 1f)]
    [SerializeField] private float energiaQueDa = 0.25f;

    // Flores que tienen a Jawi dentro de su trigger y todavía se pueden cantar
    static readonly List<FlorInteractiva> cercanas = new List<FlorInteractiva>();

    // Lo usa AccionesJawi: true si Jawi está junto a una flor
    public static bool HayFlorCerca => cercanas.Count > 0;

    // Lo llama AccionesJawi cuando Jawi empieza a cantar
    public static void CantarAFloresCercanas()
    {
        // Copia, porque al cambiar las flores se quitan de la lista
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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        AccionesJawi jawi = other.GetComponentInParent<AccionesJawi>();
        if (jawi == null) return; // solo Jawi
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
