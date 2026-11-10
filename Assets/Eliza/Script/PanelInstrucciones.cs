using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

        if (botonSiguiente != null)
        {
            botonSiguiente.interactable = true;
            botonSiguiente.onClick.AddListener(Siguiente);
        }

        Mostrar();
    }

    void Mostrar()
    {
        Mostrando = true;
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.GetComponent<GraphicRaycaster>() == null) canvas.gameObject.AddComponent<GraphicRaycaster>();
        Time.timeScale = 0f;
        if (controles != null) controles.enabled = false;
        if (pausa != null) pausa.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Seleccionar();
    }

    void Update()
    {
        if (!Mostrando) return;

        if (TeclaSiguiente())
        {
            Siguiente();
            return;
        }

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null) Seleccionar();
    }

    bool TeclaSiguiente()
    {
        Keyboard k = Keyboard.current;
        if (k != null && (k.enterKey.wasPressedThisFrame || k.numpadEnterKey.wasPressedThisFrame || k.spaceKey.wasPressedThisFrame)) return true;
        Gamepad g = Gamepad.current;
        if (g != null && (g.buttonSouth.wasPressedThisFrame || g.startButton.wasPressedThisFrame)) return true;
        return false;
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
        if (isActiveAndEnabled) StartCoroutine(Activar());
        else ActivarAhora();
    }

    IEnumerator Activar()
    {
        yield return null;
        yield return null;
        ActivarAhora();
    }

    void ActivarAhora()
    {
        if (controles != null) controles.enabled = true;
        if (pausa != null) pausa.enabled = true;
    }

    void OnDestroy()
    {
        if (botonSiguiente != null) botonSiguiente.onClick.RemoveListener(Siguiente);
    }
}

