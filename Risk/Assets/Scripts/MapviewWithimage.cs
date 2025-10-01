using UnityEngine;
using CrazyRisk;        // TerritorioId, Continente
using CrazyRisk.Core;  // Mapa, Territorio

public class MapViewWithImageLinked : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer worldMap;
    public TerritoryNode territoryPrefab;
    public Material lineMaterial;
    public float nodeScale = 1f;

    // Posiciones normalizadas
    struct PosEntry { public TerritorioId id; public Vector2 pos; }
    PosEntry[] pos01 = new PosEntry[]
    {
        new PosEntry{ id = TerritorioId.Alaska, pos = new Vector2(0.07f,0.62f)},
        new PosEntry{ id = TerritorioId.NWTerritory, pos = new Vector2(0.16f,0.65f)},
        new PosEntry{ id = TerritorioId.Groenlandia, pos = new Vector2(0.31f,0.74f)},
        // ... agregar los demás territorios
    };

    // Arrays paralelos simulando diccionario
    private TerritorioId[] nodeIds;
    private TerritoryNode[] nodeValues;
    private int nodeCount;

    private Mapa mapa;

    void Start()
    {
        if (worldMap == null || territoryPrefab == null || lineMaterial == null)
        {
            Debug.LogError("Faltan referencias (worldMap / territoryPrefab / lineMaterial).");
            return;
        }

        mapa = Mapa.CrearMapaBase();
        Territorio[] allTerritorios = mapa.GetAllTerritorios();

        nodeIds = new TerritorioId[allTerritorios.Length];
        nodeValues = new TerritoryNode[allTerritorios.Length];
        nodeCount = 0;

        PlaceNodes(allTerritorios);
        DrawEdges(allTerritorios);
    }

    void PlaceNodes(Territorio[] allTerritorios)
    {
        var b = worldMap.bounds;

        for (int i = 0; i < allTerritorios.Length; i++)
        {
            Territorio t = allTerritorios[i];
            TerritorioId id = t.Id;

            Vector2? pos = null;
            for (int j = 0; j < pos01.Length; j++)
                if (pos01[j].id == id) { pos = pos01[j].pos; break; }

            if (pos == null) continue;

            Vector3 worldPos = new Vector3(
                Mathf.Lerp(b.min.x, b.max.x, pos.Value.x),
                Mathf.Lerp(b.min.y, b.max.y, pos.Value.y),
                -0.01f
            );

            var node = Instantiate(territoryPrefab, worldPos, Quaternion.identity, transform);
            node.transform.localScale = Vector3.one * nodeScale;
            node.id = id;
            node.SetText(t.Nombre + "\n" + t.Tropas);
            node.OnClicked += HandleClicked;

            nodeIds[nodeCount] = id;
            nodeValues[nodeCount] = node;
            nodeCount++;
        }
    }

    void DrawEdges(Territorio[] allTerritorios)
    {
        for (int i = 0; i < allTerritorios.Length; i++)
        {
            Territorio t = allTerritorios[i];
            TerritoryNode aNode = GetNode(t.Id);
            if (aNode == null) continue;

            // Recorremos la lista enlazada de vecinos
            var vecino = t.Vecinos.head;
            while (vecino != null)
            {
                TerritorioId neighborId = vecino.data;
                TerritoryNode bNode = GetNode(neighborId);

                if (bNode != null && (int)t.Id < (int)neighborId)
                    DrawLine(aNode.transform.position, bNode.transform.position);

                vecino = vecino.next;
            }
        }
    }

    TerritoryNode GetNode(TerritorioId id)
    {
        for (int i = 0; i < nodeCount; i++)
            if (nodeIds[i] == id) return nodeValues[i];
        return null;
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
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 0;
    }

    void HandleClicked(TerritorioId id)
    {
        // Desmarcar todos
        for (int i = 0; i < nodeCount; i++)
            nodeValues[i].SetHighlighted(false);

        var node = GetNode(id);
        if (node == null) return;

        node.SetHighlighted(true);

        Territorio t = mapa.Get(id);
        var vecino = t.Vecinos.head;
        while (vecino != null)
        {
            TerritoryNode n = GetNode(vecino.data);
            if (n != null) n.SetHighlighted(true);
            vecino = vecino.next;
        }

        Debug.Log($"Click: {id}  Vecinos: {GetVecinosString(t.Vecinos)}");
    }

    string GetVecinosString(LinkedList<TerritorioId> vecinos)
    {
        string s = "";
        var curr = vecinos.head;
        while (curr != null)
        {
            s += curr.data.ToString();
            if (curr.next != null) s += ", ";
            curr = curr.next;
        }
        return s;
    }
}
