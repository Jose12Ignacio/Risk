using UnityEngine;
using System.Collections.Generic;
using CrazyRisk;        // TerritorioId, Continente
using CrazyRisk.Core;  // Mapa, Territorio

public class MapViewWithImage : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer worldMap;       // Asigna el SpriteRenderer del objeto Worldmap
    public TerritoryNode territoryPrefab; // Prefab con SpriteRenderer + TextMeshPro (Label)
    public Material lineMaterial;         // Material Unlit/Color para las aristas
    public float nodeScale = 1f;

    // Posiciones NORMALIZADAS (0..1) para todos los territorios (aprox. para mapamundi equirectangular).
    // Ajusta lo que haga falta según tu imagen concreta.
    Dictionary<TerritorioId, Vector2> pos01 = new Dictionary<TerritorioId, Vector2>
    {
        // América del Norte (9)
        { TerritorioId.Alaska,         new Vector2(0.07f, 0.62f) },
        { TerritorioId.NWTerritory,    new Vector2(0.16f, 0.65f) },
        { TerritorioId.Groenlandia,    new Vector2(0.31f, 0.74f) },
        { TerritorioId.Alberta,        new Vector2(0.18f, 0.56f) },
        { TerritorioId.Ontario,        new Vector2(0.27f, 0.57f) },
        { TerritorioId.Quebec,         new Vector2(0.30f, 0.56f) },
        { TerritorioId.OesteEEUU,      new Vector2(0.19f, 0.46f) },
        { TerritorioId.EsteEEUU,       new Vector2(0.24f, 0.46f) },
        { TerritorioId.CentroAmerica,  new Vector2(0.22f, 0.35f) },

        // Sudamérica (4)
        { TerritorioId.Venezuela,      new Vector2(0.25f, 0.34f) },
        { TerritorioId.Peru,           new Vector2(0.23f, 0.22f) },
        { TerritorioId.Brasil,         new Vector2(0.29f, 0.25f) },
        { TerritorioId.Argentina,      new Vector2(0.28f, 0.13f) },

        // Europa (7)
        { TerritorioId.Islandia,       new Vector2(0.36f, 0.67f) },
        { TerritorioId.GranBretana,    new Vector2(0.39f, 0.58f) },
        { TerritorioId.Escandinavia,   new Vector2(0.44f, 0.66f) },
        { TerritorioId.EuropaNorte,    new Vector2(0.46f, 0.59f) },
        { TerritorioId.EuropaOccidental,new Vector2(0.42f, 0.57f) },
        { TerritorioId.EuropaSur,      new Vector2(0.45f, 0.51f) },
        { TerritorioId.Ucrania,        new Vector2(0.51f, 0.59f) },

        // África (6)
        { TerritorioId.AfricaNorte,    new Vector2(0.46f, 0.43f) },
        { TerritorioId.Egipto,         new Vector2(0.50f, 0.38f) },
        { TerritorioId.AfricaEste,     new Vector2(0.54f, 0.31f) },
        { TerritorioId.Congo,          new Vector2(0.49f, 0.28f) },
        { TerritorioId.AfricaSur,      new Vector2(0.50f, 0.17f) },
        { TerritorioId.Madagascar,     new Vector2(0.57f, 0.19f) },

        // Asia (12)
        { TerritorioId.Ural,           new Vector2(0.56f, 0.60f) },
        { TerritorioId.Siberia,        new Vector2(0.60f, 0.66f) },
        { TerritorioId.Yakutsk,        new Vector2(0.68f, 0.67f) },
        { TerritorioId.Kamchatka,      new Vector2(0.86f, 0.62f) },
        { TerritorioId.Irkutsk,        new Vector2(0.64f, 0.61f) },
        { TerritorioId.Mongolia,       new Vector2(0.64f, 0.53f) },
        { TerritorioId.Japon,          new Vector2(0.80f, 0.50f) },
        { TerritorioId.China,          new Vector2(0.61f, 0.49f) },
        { TerritorioId.MedioOriente,   new Vector2(0.53f, 0.43f) },
        { TerritorioId.India,          new Vector2(0.58f, 0.39f) },
        { TerritorioId.Siam,           new Vector2(0.63f, 0.42f) },
        { TerritorioId.Afganistan,     new Vector2(0.57f, 0.52f) },

        // Oceanía (4)
        { TerritorioId.Indonesia,          new Vector2(0.69f, 0.37f) },
        { TerritorioId.NuevaGuinea,        new Vector2(0.77f, 0.36f) },
        { TerritorioId.AustraliaOccidental,new Vector2(0.72f, 0.23f) },
        { TerritorioId.AustraliaOriental,  new Vector2(0.79f, 0.24f) },
    };

    Mapa mapa;
    readonly Dictionary<TerritorioId, TerritoryNode> nodes = new Dictionary<TerritorioId, TerritoryNode>();

    void Start()
    {
        if (worldMap == null || territoryPrefab == null || lineMaterial == null)
        {
            Debug.LogError("MapViewWithImage: faltan referencias (worldMap / territoryPrefab / lineMaterial).");
            return;
        }

        mapa = Mapa.CrearMapaBase();   // Crea el grafo con Territorios y Vecinos
        PlaceNodes();
        DrawEdges();
    }

    void PlaceNodes()
    {
        var b = worldMap.bounds; // mundo (coordenadas) del sprite del mapa
        foreach (var kv in mapa.Territorios)
        {
            var id = kv.Key;
            if (!pos01.TryGetValue(id, out var p)) continue;

            Vector3 worldPos = new Vector3(
                Mathf.Lerp(b.min.x, b.max.x, p.x),
                Mathf.Lerp(b.min.y, b.max.y, p.y),
                -0.01f // un poco delante del fondo
            );

            var node = Instantiate(territoryPrefab, worldPos, Quaternion.identity, transform);
            node.transform.localScale = Vector3.one * nodeScale;
            node.id = id;
            node.SetText(kv.Value.Nombre + "\n" + kv.Value.Tropas); // usa tus datos de Territorio
            node.OnClicked += HandleClicked;
            nodes[id] = node;
        }
    }

    void DrawEdges()
    {
        foreach (var kv in mapa.Territorios)
        {
            var aId = kv.Key;
            if (!nodes.ContainsKey(aId)) continue;

            var t = kv.Value; // lista de vecinos del territorio
            for (int i = 0; i < t.Vecinos.Count; i++)
            {
                var bId = t.Vecinos[i];
                if (!nodes.ContainsKey(bId)) continue;

                // para no duplicar líneas, solo dibujo si aId < bId
                if ((int)aId < (int)bId)
                    DrawLine(nodes[aId].transform.position, nodes[bId].transform.position);
            }
        }
    }

    void DrawLine(Vector3 a, Vector3 b)
    {
        var go = new GameObject("edge");
        go.transform.SetParent(transform, true);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new[] { a, b });
        lr.widthMultiplier = 0.04f;
        lr.material = lineMaterial;
        lr.numCapVertices = 4;
        // que se vea sobre el fondo (si tu fondo está en SortingLayer Background / order -10, esto va bien)
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 0;
    }

    void HandleClicked(TerritorioId id)
    {
        foreach (var n in nodes.Values) n.SetHighlighted(false);
        if (!nodes.ContainsKey(id)) return;

        nodes[id].SetHighlighted(true);
        var t = mapa.Get(id);
        for (int i = 0; i < t.Vecinos.Count; i++)
        {
            var v = t.Vecinos[i];
            if (nodes.ContainsKey(v)) nodes[v].SetHighlighted(true);
        }
        Debug.Log($"Click: {id}  Vecinos: {string.Join(", ", t.Vecinos)}");
    }
}
