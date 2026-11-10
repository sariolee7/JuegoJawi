using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class ReactivarUI : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
        EventSystem[] sistemas = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        for (int i = 1; i < sistemas.Length; i++) Destroy(sistemas[i].gameObject);
        if (sistemas.Length == 0) yield break;

        InputSystemUIInputModule modulo = sistemas[0].GetComponent<InputSystemUIInputModule>();
        if (modulo == null) yield break;
        modulo.enabled = false;
        yield return null;
        modulo.enabled = true;
        if (modulo.actionsAsset != null) modulo.actionsAsset.Enable();
    }
}
