using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaPerdiste : MonoBehaviour
{
    public GameObject panelPerdiste;
    public float esperaAlPerder = 1f;
    public string escenaMenu = "Menu";
    public GameObject[] ocultarAlTerminar;

    public bool Termino { get; private set; }

    void Start()
    {
        if (panelPerdiste != null) panelPerdiste.SetActive(false);

        if (GameManager.Instancia != null) GameManager.Instancia.AlGameOver += Perder;

        if (FindAnyObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    void OnDestroy()
    {
        if (GameManager.Instancia != null) GameManager.Instancia.AlGameOver -= Perder;
    }

    public void Perder()
    {
        if (Termino) return;
        Termino = true;
        StartCoroutine(Mostrar());
    }

    IEnumerator Mostrar()
    {
        MenuPausa pausa = FindAnyObjectByType<MenuPausa>();
        if (pausa != null)
        {
            if (pausa.EstaPausado) pausa.Reanudar();
            pausa.enabled = false;
        }

        yield return new WaitForSecondsRealtime(esperaAlPerder);

        ControlesJawi controles = FindAnyObjectByType<ControlesJawi>();
        if (controles != null) controles.enabled = false;

        if (ocultarAlTerminar != null)
            foreach (GameObject go in ocultarAlTerminar)
                if (go != null) go.SetActive(false);

        if (panelPerdiste != null) panelPerdiste.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Seleccionar();
    }

    void Update()
    {
        if (!Termino || EventSystem.current == null) return;
        if (EventSystem.current.currentSelectedGameObject != null) return;

        Seleccionar();
    }

    void Seleccionar()
    {
        if (EventSystem.current == null) return;
        if (panelPerdiste == null || !panelPerdiste.activeSelf) return;
        Button boton = panelPerdiste.GetComponentInChildren<Button>();
        if (boton == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(boton.gameObject);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        Termino = false;
        if (panelPerdiste != null) panelPerdiste.SetActive(false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        if (GameManager.Instancia != null) GameManager.Instancia.Reiniciar();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instancia != null) Destroy(GameManager.Instancia.gameObject);
        SceneManager.LoadScene(escenaMenu);
    }

    public void Salir()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        IrAlMenu();
#else
        Application.Quit();
#endif
    }
}
