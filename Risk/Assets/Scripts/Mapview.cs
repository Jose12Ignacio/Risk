using UnityEngine;
using CrazyRisk;       // TerritorioId, Continente
using CrazyRisk.Core;  // Mapa, Territorio

public class MapView : MonoBehaviour
{
    public TerritoryNode territoryPrefab;
    public Material lineMaterial;
    public float nodeScale = 1f;

    // Arrays paralelos para posiciones
    TerritorioId[] posKeys = new TerritorioId[]
    {
        TerritorioId.Alaska,
        TerritorioId.NWTerritory,
        TerritorioId.Alberta,
        TerritorioId.Ontario,
        TerritorioId.OesteEEUU,
        TerritorioId.Kamchatka
    };

    Vector2[] posValues = new Vector2[]
    {
        new Vector2(-8f, 3f),
        new Vector2(-6f, 3.2f),
        new Vector2(-6f, 1.8f),
        new Vector2(-4f, 2.1f),
        new Vector2(-5f, 0.6f),
        new Vector2( 6f, 3.5f)
    };

    Mapa mapa;

    // Arrays paralelos para nodos instanciados
    TerritorioId[] nodeKeys;
    TerritoryNode[] nodeValues;
    int nodeCount = 0;

    void Start()
    {
        mapa = Mapa.CrearMapaBase(); // crea grafo con territorios y vecinos propios

        // Inicializa arrays con tamaño fijo igual al número de territorios
        int totalTerritorios = mapa.GetAllTerritorios().Length;
        nodeKeys = new TerritorioId[totalTerritorios];
        nodeValues = new TerritoryNode[totalTerritorios];

        // Instanciar nodos
        var territorios = mapa.GetAllTerritorios();
        for (int i = 0; i < territorios.Length; i++)
        {
            var territorio = territorios[i];
            var id = territorio.Id;

            Vector2? pos = GetPos(id);
            if (pos == null) continue;

            var node = Instantiate(territoryPrefab, pos.Value, Quaternion.identity, transform);
            node.transform.localScale = Vector3.one * nodeScale;
            node.id = id;
            node.spriteRenderer.color = new Color(0.2f, 0.6f, 1f, 0.5f);
            node.SetText(territorio.Nombre + "\n" + territorio.Tropas);
            node.OnClicked += HandleClicked;

            nodeKeys[nodeCount] = id;
            nodeValues[nodeCount] = node;
            nodeCount++;
        }

        // Dibujar aristas
        for (int i = 0; i < territorios.Length; i++)
        {
            var a = territorios[i];
            var aNode = GetNode(a.Id);
            if (aNode == null) continue;

            for (int j = 0; j < a.Vecinos.Count(); j++)
            {
                var bId = a.Vecinos.Get(j);
                var bNode = GetNode(bId);
                if (bNode != null && (int)a.Id < (int)bId)
                {
                    DrawEdge(aNode.transform.position, bNode.transform.position);
                }
            }
        }
    }

    void HandleClicked(TerritorioId id)
    {
        // Desmarcar todos los nodos
        for (int i = 0; i < nodeCount; i++)
            nodeValues[i].SetHighlighted(false);

        var node = GetNode(id);
        if (node == null) return;

        node.SetHighlighted(true);

        var t = mapa.Get(id);
        for (int i = 0; i < t.Vecinos.Count(); i++)
        {
            var vId = t.Vecinos.Get(i);
            var vNode = GetNode(vId);
            if (vNode != null) vNode.SetHighlighted(true);
        }
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

    // 🔹 Helpers
    Vector2? GetPos(TerritorioId id)
    {
        for (int i = 0; i < posKeys.Length; i++)
            if (posKeys[i] == id) return posValues[i];
        return null;
    }

    TerritoryNode GetNode(TerritorioId id)
    {
        for (int i = 0; i < nodeCount; i++)
            if (nodeKeys[i] == id) return nodeValues[i];
        return null;
    }
}
