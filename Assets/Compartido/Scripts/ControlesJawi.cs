using UnityEngine;
using UnityEngine.InputSystem;

// Controles compartidos de JAWI (teclado + gamepad).
// Teclado: A/D o flechas = caminar, W o flecha arriba = saltar,
//          Espacio = zarpazo (y liberar al bebé), E = canto.
// Gamepad: joystick izquierdo o cruceta = caminar, A (Xbox) / X (PlayStation) = saltar,
//          B / Círculo = zarpazo (y liberar al bebé), Y / Triángulo = canto.
public class ControlesJawi : MonoBehaviour
{
    [Tooltip("Muestra en la Consola qué botón se presiona (solo para probar)")]
    public bool mostrarEnConsola = false;

    InputAction mover, saltar, zarpazo, canto;

    // Lo que leen los demás scripts (por ejemplo, el movimiento de Jawi)
    public float Horizontal => mover.ReadValue<float>();
    public bool SaltarPresionado => saltar.WasPressedThisFrame();
    public bool SaltarSostenido => saltar.IsPressed();
    public bool ZarpazoPresionado => zarpazo.WasPressedThisFrame();
    public bool CantoPresionado => canto.WasPressedThisFrame();
    // Liberar al bebé usa el mismo botón que el zarpazo
    public bool LiberarPresionado => zarpazo.WasPressedThisFrame();

    void Awake()
    {
        mover = new InputAction("Mover", InputActionType.Value);
        mover.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");
        mover.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");
        mover.AddBinding("<Gamepad>/leftStick/x");
        mover.AddBinding("<Gamepad>/dpad/x");

        saltar = new InputAction("Saltar", InputActionType.Button);
        saltar.AddBinding("<Keyboard>/w");
        saltar.AddBinding("<Keyboard>/upArrow");
        saltar.AddBinding("<Gamepad>/buttonSouth"); // A en Xbox, X en PlayStation

        zarpazo = new InputAction("Zarpazo", InputActionType.Button);
        zarpazo.AddBinding("<Keyboard>/space");
        zarpazo.AddBinding("<Gamepad>/buttonEast"); // B en Xbox, Círculo en PlayStation

        canto = new InputAction("Canto", InputActionType.Button);
        canto.AddBinding("<Keyboard>/e");
        canto.AddBinding("<Gamepad>/buttonNorth"); // Y en Xbox, Triángulo en PlayStation
    }

    void OnEnable()
    {
        mover.Enable();
        saltar.Enable();
        zarpazo.Enable();
        canto.Enable();
    }

    void OnDisable()
    {
        mover.Disable();
        saltar.Disable();
        zarpazo.Disable();
        canto.Disable();
    }

    void OnDestroy()
    {
        mover.Dispose();
        saltar.Dispose();
        zarpazo.Dispose();
        canto.Dispose();
    }

    void Update()
    {
        if (!mostrarEnConsola) return;

        if (mover.WasPressedThisFrame()) Debug.Log("Caminar: " + Horizontal);
        if (SaltarPresionado) Debug.Log("Saltar");
        if (ZarpazoPresionado) Debug.Log("Zarpazo");
        if (CantoPresionado) Debug.Log("Canto");
    }
}
