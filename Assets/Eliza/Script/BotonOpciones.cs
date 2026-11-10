using UnityEngine;

public class BotonOpciones : MonoBehaviour
{
    public GameObject panelOpciones;

    void Start()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
    }

    public void Abrir()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (panelOpciones != null) panelOpciones.SetActive(true);
    }

    public void Cerrar()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        Time.timeScale = 1f;
    }
}
