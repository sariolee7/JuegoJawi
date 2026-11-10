using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PanelInstrucciones : MonoBehaviour
{
    public GameObject panel;
    public Button botonSiguiente;

    public bool Mostrando { get; private set; }

    ControlesJawi controles;
    MenuPausa pausa;

    void Start()
    {
        if (panel == null) return;

        if (FindAnyObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        controles = FindAnyObjectByType<ControlesJawi>();
        pausa = FindAnyObjectByType<MenuPausa>();

        if (botonSiguiente != null) botonSiguiente.onClick.AddListener(Siguiente);

        Mostrar();
    }

    void Mostrar()
    {
        Mostrando = true;
        panel.SetActive(true);
        Time.timeScale = 0f;
        if (controles != null) controles.enabled = false;
        if (pausa != null) pausa.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Seleccionar();
    }

    void Update()
    {
        if (!Mostrando || EventSystem.current == null) return;
        if (EventSystem.current.currentSelectedGameObject == null) Seleccionar();
    }

    void Seleccionar()
    {
        if (botonSiguiente == null || EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonSiguiente.gameObject);
    }

    public void Siguiente()
    {
        if (!Mostrando) return;
        Mostrando = false;
        panel.SetActive(false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1f;
        StartCoroutine(Activar());
    }

    IEnumerator Activar()
    {
        yield return null;
        yield return null;
        if (controles != null) controles.enabled = true;
        if (pausa != null) pausa.enabled = true;
    }

    void OnDestroy()
    {
        if (botonSiguiente != null) botonSiguiente.onClick.RemoveListener(Siguiente);
    }
}
