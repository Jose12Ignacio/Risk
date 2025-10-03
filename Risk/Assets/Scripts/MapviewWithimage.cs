using UnityEngine;
using CrazyRisk;
using CrazyRisk.Core;
using UnityEngine.UI;

public class MapViewWithImageLinked : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer worldMap;       // Imagen del mapa
    public GameObject territoryPrefab;    // Prefab TerritoryNode
    public GameObject regionButtonPrefab; // Prefab TerritoryUI (botón invisible)
    public Transform canvasTransform;     // Canvas donde instanciar botones
    public Material lineMaterial;         // Material de líneas
    public float lineWidth = 0.02f;

    [Header("Offsets Globales")]
    public Vector2 globalNodeOffset = new Vector2(0.5f, 0.5f); // mover todos los nodos en mundo
    public Vector2 globalLabelOffset = new Vector2(40f, 20f);  // mover todos los labels en pantalla
    public Vector2 globalButtonOffset = new Vector2(50f, 20f); // mover todos los botones en pantalla

    private Mapa mapa;

    private TerritorioId[] ids;
    private Vector2[] posiciones;
    private Vector2[] offsetNombres;
    private Vector2[] offsetTropas;
    private Vector2[] offsetBotones;

    private TerritoryNode[] nodes; // Nodos con labels
    private TerritoryUI[] uis;     // Botones invisibles

    private TerritoryNode atacante;
    private TerritoryNode defensor;

    void Awake()
    {
        mapa = Mapa.CrearMapaBase();
    }

    void Start()
    {
        // === IDs ===
        ids = new TerritorioId[]
        {
            TerritorioId.Alaska, TerritorioId.NWTerritory, TerritorioId.Groenlandia,
            TerritorioId.Alberta, TerritorioId.Ontario, TerritorioId.Quebec,
            TerritorioId.OesteEEUU, TerritorioId.EsteEEUU, TerritorioId.CentroAmerica,
            TerritorioId.Venezuela, TerritorioId.Peru, TerritorioId.Brasil, TerritorioId.Argentina,
            TerritorioId.Islandia, TerritorioId.GranBretana, TerritorioId.Escandinavia,
            TerritorioId.EuropaNorte, TerritorioId.EuropaOccidental, TerritorioId.EuropaSur, TerritorioId.Ucrania,
            TerritorioId.AfricaNorte, TerritorioId.Egipto, TerritorioId.AfricaEste,
            TerritorioId.Congo, TerritorioId.AfricaSur, TerritorioId.Madagascar,
            TerritorioId.Ural, TerritorioId.Siberia, TerritorioId.Yakutsk, TerritorioId.Kamchatka,
            TerritorioId.Irkutsk, TerritorioId.Mongolia, TerritorioId.Japon, TerritorioId.China,
            TerritorioId.MedioOriente, TerritorioId.India, TerritorioId.Siam, TerritorioId.Afganistan,
            TerritorioId.Indonesia, TerritorioId.NuevaGuinea, TerritorioId.AustraliaOccidental, TerritorioId.AustraliaOriental
        };

        // === Posiciones base (normalizadas 0..1) ===
        posiciones = new Vector2[]
        {
            new Vector2(0.08f,0.62f), new Vector2(0.16f,0.65f), new Vector2(0.31f,0.74f),
            new Vector2(0.19f,0.56f), new Vector2(0.27f,0.57f), new Vector2(0.30f,0.56f),
            new Vector2(0.2f,0.46f), new Vector2(0.24f,0.46f), new Vector2(0.22f,0.35f),
            new Vector2(0.26f,0.34f), new Vector2(0.23f,0.22f), new Vector2(0.29f,0.25f), new Vector2(0.28f,0.13f),
            new Vector2(0.37f,0.67f), new Vector2(0.39f,0.58f), new Vector2(0.44f,0.66f),
            new Vector2(0.47f,0.59f), new Vector2(0.42f,0.57f), new Vector2(0.45f,0.51f), new Vector2(0.51f,0.59f),
            new Vector2(0.46f,0.43f), new Vector2(0.50f,0.38f), new Vector2(0.54f,0.31f),
            new Vector2(0.5f,0.28f), new Vector2(0.50f,0.17f), new Vector2(0.57f,0.19f),
            new Vector2(0.57f,0.60f), new Vector2(0.60f,0.66f), new Vector2(0.68f,0.67f), new Vector2(0.86f,0.62f),
            new Vector2(0.65f,0.61f), new Vector2(0.64f,0.53f), new Vector2(0.80f,0.50f), new Vector2(0.61f,0.49f),
            new Vector2(0.54f,0.43f), new Vector2(0.58f,0.39f), new Vector2(0.63f,0.42f), new Vector2(0.57f,0.52f),
            new Vector2(0.7f,0.37f), new Vector2(0.77f,0.36f), new Vector2(0.72f,0.23f), new Vector2(0.79f,0.24f)
        };

        // === Offsets iniciales ===
        offsetNombres = new Vector2[ids.Length];
        offsetTropas = new Vector2[ids.Length];
        offsetBotones = new Vector2[ids.Length];

        for (int i = 0; i < ids.Length; i++)
        {
            offsetNombres[i] = new Vector2(0, 25);   // nombre arriba
            offsetTropas[i]  = new Vector2(0, -25);  // tropas abajo
            offsetBotones[i] = new Vector2(0, 0);    // botón centrado
        }

        nodes = new TerritoryNode[ids.Length];
        uis = new TerritoryUI[ids.Length];

        InstanciarNodosYBotones();
        DibujarConexiones();
    }

    void Update()
    {
        // Reajusta posiciones en pantalla
        for (int i = 0; i < ids.Length; i++)
        {
            if (nodes[i] == null || uis[i] == null) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(nodes[i].transform.position);

            // Nombre
            if (nodes[i].labelNombre != null)
                nodes[i].labelNombre.transform.position =
                    screenPos + (Vector3)(offsetNombres[i] + globalLabelOffset);

            // Tropas
            if (nodes[i].labelTropas != null)
                nodes[i].labelTropas.transform.position =
                    screenPos + (Vector3)(offsetTropas[i] + globalLabelOffset);

            // Botón invisible
            RectTransform rt = uis[i].GetComponent<RectTransform>();
            rt.position = screenPos + (Vector3)(offsetBotones[i] + globalButtonOffset);
        }
    }

    void InstanciarNodosYBotones()
{
    var b = worldMap.bounds;

    for (int i = 0; i < ids.Length; i++)
    {
        var pos = posiciones[i];
        var worldPos = new Vector3(
            Mathf.Lerp(b.min.x, b.max.x, pos.x),
            Mathf.Lerp(b.min.y, b.max.y, pos.y),
            -0.01f
        );

        // aplicar offset global en coordenadas de mundo
        worldPos += (Vector3)globalNodeOffset;

        // === Nodo en el mapa ===
        var goNode = Instantiate(territoryPrefab, worldPos, Quaternion.identity, transform);
        var node = goNode.GetComponent<TerritoryNode>();
        node.id = ids[i];
        node.SetData(mapa.GetName(ids[i]), mapa.GetTroops(ids[i]));
        nodes[i] = node;

        // === Botón UI ===
        var goUI = Instantiate(regionButtonPrefab, canvasTransform);
        var ui = goUI.GetComponent<TerritoryUI>();
        ui.Init(ids[i], mapa.GetName(ids[i]), mapa.GetTroops(ids[i]));
        ui.OnClicked += HandleClicked;
        uis[i] = ui;

        // === Alinear botón con el nodo ===
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform as RectTransform,
            screenPos,
            Camera.main,
            out Vector2 localPos
        );

        goUI.GetComponent<RectTransform>().localPosition = localPos;
    }
}



    void DibujarConexiones()
    {
        var buffer = new TerritorioId[12];
        for (int i = 0; i < ids.Length; i++)
        {
            var idA = ids[i];
            mapa.GetVecinos(idA, buffer, out int count);

            var nodeA = nodes[i];
            for (int j = 0; j < count; j++)
            {
                var idB = buffer[j];
                int idxB = GetIndex(idB);
                if (idxB < 0) continue;

                var nodeB = nodes[idxB];
                if (nodeA != null && nodeB != null)
                    CrearLinea(nodeA.transform.position, nodeB.transform.position);
            }
        }
    }

    int GetIndex(TerritorioId id)
    {
        for (int i = 0; i < ids.Length; i++)
            if (ids[i] == id) return i;
        return -1;
    }

    void CrearLinea(Vector3 a, Vector3 b)
    {
        var go = new GameObject("edge");
        go.transform.SetParent(transform, true);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new[] { a, b });
        lr.widthMultiplier = lineWidth;
        lr.material = lineMaterial;
    }

    void HandleClicked(TerritorioId id)
    {
        var node = nodes[GetIndex(id)];

        if (atacante == null)
        {
            atacante = node;
            atacante.SetHighlighted(true);
            return;
        }

        defensor = node;
        if (defensor == atacante)
        {
            atacante.SetHighlighted(false);
            atacante = null;
            return;
        }

        if (mapa.SonVecinos(atacante.id, defensor.id))
            Debug.Log($"[Ataque] {atacante.Nombre} -> {defensor.Nombre}");

        atacante.SetHighlighted(false);
        atacante = null;
    }
}
