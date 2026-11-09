using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menú de pausa de JAWI (teclado, mouse y gamepad).
// Abrir / cerrar: tecla Esc (o Start en el gamepad).
// Moverse entre botones: flechas / W-S, joystick izquierdo o cruceta.
// Aceptar: Enter / Espacio o A (Xbox) / X (PlayStation). B / Círculo también reanuda.
//
// Ponlo en el Canvas (o en un objeto vacío) y arrastra el panel y el botón Reanudar en el Inspector.
// Funciones para los botones (OnClick):
//   X y Reanudar -> Reanudar
//   Menu         -> IrAlMenu
//   Salir        -> Salir
public class MenuPausa : MonoBehaviour
{
    [Tooltip("El panel de PAUSE (el cuadro con los botones)")]
    public GameObject panelPausa;

    [Tooltip("Botón seleccionado al abrir la pausa (para el gamepad). Normalmente Reanudar")]
    public Button primerBoton;

    [Tooltip("Nombre EXACTO de la escena del menú principal")]
    public string escenaMenu = "MenuPrincipal";

    [Tooltip("Oculta el mouse mientras se juega y lo muestra en la pausa")]
    public bool ocultarMouseAlJugar = true;

    [Tooltip("Cosas que se esconden mientras está en pausa (por ejemplo tu HUD con el retrato de JAWI). Los corazones del HUD se esconden solos.")]
    public GameObject[] ocultarAlPausar;

    public bool EstaPausado { get; private set; }

    ControlesJawi controles;
    InputAction pausar, volver;

    void Awake()
    {
        // Esc abre/cierra la pausa (Start en el gamepad, porque la jam pide control)
        pausar = new InputAction("Pausar", InputActionType.Button);
        pausar.AddBinding("<Keyboard>/escape");
        pausar.AddBinding("<Gamepad>/start");

        // B / Círculo cierra la pausa
        volver = new InputAction("Volver", InputActionType.Button);
        volver.AddBinding("<Gamepad>/buttonEast");

        AsegurarEventSystem();
    }

    void OnEnable() { pausar.Enable(); volver.Enable(); }
    void OnDisable() { pausar.Disable(); volver.Disable(); }

    void OnDestroy()
    {
        pausar.Dispose();
        volver.Dispose();
        MostrarMouse(true); // al cambiar de escena, el mouse queda visible
    }

    void Start()
    {
        controles = FindAnyObjectByType<ControlesJawi>();
        if (panelPausa != null) panelPausa.SetActive(false);
        EstaPausado = false;
        MostrarMouse(false);
    }

    void Update()
    {
        if (pausar.WasPressedThisFrame())
        {
            if (EstaPausado) Reanudar();
            else Pausar();
        }
        else if (EstaPausado && volver.WasPressedThisFrame())
        {
            Reanudar();
        }

        // Si el mouse quita la selección, el gamepad la recupera
        if (EstaPausado && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null
            && Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            Seleccionar(primerBoton);
        }
    }

    // ---------- Funciones para los botones ----------

    public void Pausar()
    {
        // No se pausa encima del Game Over
        if (GameManager.Instancia != null && GameManager.Instancia.EsGameOver) return;

        EstaPausado = true;
        Time.timeScale = 0f;
        if (controles != null) controles.enabled = false; // Jawi no salta ni ataca con los botones del menú
        if (panelPausa != null) panelPausa.SetActive(true);
        MostrarMouse(true);
        MostrarHUD(false);
        Seleccionar(primerBoton);
    }

    public void Reanudar()
    {
        EstaPausado = false;
        Time.timeScale = 1f;
        if (panelPausa != null) panelPausa.SetActive(false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        MostrarMouse(false);
        MostrarHUD(true);
        StartCoroutine(ActivarControlesLuego());
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f; // si no, el menú principal queda congelado
        EstaPausado = false;
        MostrarMouse(true); // en el menú principal se usa el mouse
        SceneManager.LoadScene(escenaMenu);
    }

    public void Salir()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // en el editor, detiene el Play
#elif UNITY_WEBGL
        SceneManager.LoadScene(escenaMenu); // en la web no se puede cerrar la pestaña: vuelve al menú
#else
        Application.Quit();
#endif
    }

    // ---------- Ayudantes ----------

    // Muestra u oculta el mouse. No lo bloquea: en la web, bloquearlo le roba la tecla Esc al juego.
    void MostrarMouse(bool mostrar)
    {
        if (!ocultarMouseAlJugar) return;
        Cursor.visible = mostrar;
        Cursor.lockState = CursorLockMode.None;
    }

    // Esconde o muestra el HUD (corazones y tunjos) y lo que pongas en "Ocultar Al Pausar"
    void MostrarHUD(bool mostrar)
    {
        HUDJawi hud = FindAnyObjectByType<HUDJawi>();
        if (hud != null)
        {
            foreach (Canvas c in hud.GetComponentsInChildren<Canvas>(true))
                c.enabled = mostrar;
        }
        if (ocultarAlPausar != null)
        {
            foreach (GameObject go in ocultarAlPausar)
                if (go != null) go.SetActive(mostrar);
        }
    }

    void Seleccionar(Button boton)
    {
        if (boton == null || EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(boton.gameObject);
    }

    // Espera un cuadro para que el botón de "Reanudar" no haga saltar o atacar a Jawi
    IEnumerator ActivarControlesLuego()
    {
        yield return null;
        if (controles != null) controles.enabled = true;
    }

    // Sin EventSystem los botones no responden; lo crea si falta (con el Input System nuevo)
    void AsegurarEventSystem()
    {
        EventSystem es = FindAnyObjectByType<EventSystem>();
        if (es == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            return;
        }
        // Si tiene el módulo viejo, lo cambia por el del Input System
        StandaloneInputModule viejo = es.GetComponent<StandaloneInputModule>();
        if (viejo != null)
        {
            Destroy(viejo);
            if (es.GetComponent<InputSystemUIInputModule>() == null)
                es.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
