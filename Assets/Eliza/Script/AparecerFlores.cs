using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AparecerFlores : MonoBehaviour
{
    public GameObject florPrefab;
    public Tilemap[] suelos;
    public float cadaCuantosSegundos = 5f;
    public int maximoFlores = 4;
    public float distanciaEntreFlores = 1.5f;
    public bool soloDentroDeCamara = true;
    public float ajusteAltura = 0f;

    readonly List<GameObject> flores = new List<GameObject>();
    float siguiente;

    void Start()
    {
        siguiente = Time.time + cadaCuantosSegundos;
    }

    void Update()
    {
        if (Time.time < siguiente) return;
        siguiente = Time.time + cadaCuantosSegundos;

        flores.RemoveAll(f => f == null);
        if (florPrefab == null || suelos == null || suelos.Length == 0 || flores.Count >= maximoFlores) return;

        List<Vector3> lugares = BuscarLugares();
        if (lugares.Count == 0) return;

        Vector3 pos = lugares[Random.Range(0, lugares.Count)];
        flores.Add(Instantiate(florPrefab, pos, Quaternion.identity));
    }

    List<Vector3> BuscarLugares()
    {
        List<Vector3> lugares = new List<Vector3>();
        Camera cam = Camera.main;

        foreach (Tilemap suelo in suelos)
        {
            if (suelo == null) continue;
            BoundsInt limites = suelo.cellBounds;
            float alto = suelo.cellSize.y * suelo.transform.lossyScale.y;

            for (int x = limites.xMin; x < limites.xMax; x++)
            {
                for (int y = limites.yMin; y < limites.yMax; y++)
                {
                    Vector3Int celda = new Vector3Int(x, y, 0);
                    if (!suelo.HasTile(celda)) continue;

                    Vector3 centro = suelo.GetCellCenterWorld(celda);
                    if (HayTileEn(centro + Vector3.up * alto)) continue;

                    Vector3 pos = new Vector3(centro.x, centro.y + alto / 2f + ajusteAltura, 0f);

                    if (soloDentroDeCamara && cam != null)
                    {
                        Vector3 v = cam.WorldToViewportPoint(pos);
                        if (v.x < 0.05f || v.x > 0.95f || v.y < 0.05f || v.y > 0.95f) continue;
                    }

                    if (MuyCerca(pos)) continue;
                    lugares.Add(pos);
                }
            }
        }
        return lugares;
    }

    bool HayTileEn(Vector3 posicion)
    {
        foreach (Tilemap t in suelos)
        {
            if (t != null && t.HasTile(t.WorldToCell(posicion))) return true;
        }
        return false;
    }

    bool MuyCerca(Vector3 pos)
    {
        foreach (GameObject f in flores)
        {
            if (f != null && Vector2.Distance(f.transform.position, pos) < distanciaEntreFlores) return true;
        }
        return false;
    }
}

