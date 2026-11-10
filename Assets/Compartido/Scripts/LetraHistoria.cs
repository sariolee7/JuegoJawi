using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LetraHistoria : MonoBehaviour
{
    public Font letra;
    public int tamano = 0;

    IEnumerator Start()
    {
        yield return null;
        if (letra == null) yield break;
        foreach (Text t in GetComponentsInChildren<Text>(true))
        {
            t.font = letra;
            if (tamano > 0) t.fontSize = tamano;
        }
    }
}
