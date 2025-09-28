using UnityEngine;
using System.Collections.Generic;
using CrazyRisk;       // Enums
using CrazyRisk.Core; // Mapa, Territorio

public class MapView : MonoBehaviour
{
    public TerritoryNode territoryPrefab;
    public Material lineMaterial;
    public float nodeScale = 1f;

    // posiciones para los territorios ya definidos en Mapa.CrearMapaBase()
    Dictionary<TerritorioId, Vector2> pos = new()
    {
        { TerritorioId.Alaska,      new Vector2(-8f, 3f) },
        { TerritorioId.NWTerritory, new Vector2(-6f, 3.2f) },
        { TerritorioId.Alberta,     new Vector2(-6f, 1.8f) },
        { TerritorioId.Ontario,     new Vector2(-4f, 2.1f) },
        { TerritorioId.OesteEEUU,   new Vector2(-5f, 0.6f) },
        { TerritorioId.Kamchatka,   new Vector2( 6f, 3.5f) },
    };

    Mapa mapa;
    readonly Dictionary<TerritorioId, TerritoryNode> nodes = new();

    void Start()
    {
        mapa = Mapa.CrearMapaBase(); // crea grafo con vecinos:contentReference[oaicite:2]{index=2}

        // instanciar nodos
        foreach (var kv in mapa.Territorios)
        {
            var id = kv.Key;
            if (!pos.ContainsKey(id)) continue;

            var node = Instantiate(territoryPrefab, pos[id], Quaternion.identity, transform);
            node.transform.localScale = Vector3.one * nodeScale;
            node.id = id;
            node.spriteRenderer.color = new Color(0.2f, 0.6f, 1f, 0.5f);
            node.SetText(kv.Value.Nombre + "\n" + kv.Value.Tropas); // Nombre/Tropas:contentReference[oaicite:3]{index=3}
            node.OnClicked += HandleClicked;
            nodes[id] = node;
        }

        // dibujar aristas (evitar duplicados)
        foreach (var kv in mapa.Territorios)
        {
            var a = kv.Key;
            if (!nodes.ContainsKey(a)) continue;
            var ta = kv.Value;

            for (int i = 0; i < ta.Vecinos.Count; i++)
            {
                var b = ta.Vecinos[i];
                if (!nodes.ContainsKey(b)) continue;
                if ((int)a < (int)b) DrawEdge(nodes[a].transform.position, nodes[b].transform.position);
            }
        }
    }

    void HandleClicked(TerritorioId id)
    {
        foreach (var n in nodes.Values) n.SetHighlighted(false);

        if (!nodes.ContainsKey(id)) return;
        nodes[id].SetHighlighted(true);

        var t = mapa.Get(id); // busca territorio por Id:contentReference[oaicite:4]{index=4}
        for (int i = 0; i < t.Vecinos.Count; i++)
        {
            var v = t.Vecinos[i];
            if (nodes.ContainsKey(v)) nodes[v].SetHighlighted(true);
        }
        Debug.Log($"Clicked: {id} — vecinos: {string.Join(", ", t.Vecinos)}");
    }

    void DrawEdge(Vector3 a, Vector3 b)
    {
        var go = new GameObject("edge");
        go.transform.SetParent(transform, true);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new[] { a, b });
        lr.widthMultiplier = 0.05f;
        lr.material = lineMaterial;
        lr.numCapVertices = 4;
        lr.useWorldSpace = true;
    }
}

