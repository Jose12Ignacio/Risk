using UnityEngine;
using CrazyRisk;
using CrazyRisk.Core;

public class MapView : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer worldMap;        // SpriteRenderer del mapa base 
    public TerritoryNode territoryPrefab;  // Prefab que representa visualmente un territorio 
    public Material lineMaterial;          // Material para las líneas que conectan territorios
    public float lineWidth = 0.02f;        // Grosor de las líneas de conexión

    private Mapa mapa;                     // Referencia lógica al mapa de territorios 

    private TerritorioId[] ids;            // Identificadores únicos de cada territorio
    private Vector2[] posiciones;          // Posiciones normalizadas para ubicar nodos en el mapa
    private TerritoryNode[] nodes;         // Instancias visuales (objetos TerritoryNode) en la escena

    void Awake()
    {
        // Se crea el mapa base antes del Start.
        // Awake() se ejecuta antes de que el objeto sea visible o interactivo.
        mapa = Mapa.CrearMapaBase();
    }

    void Start()
    {
        
        // Lista de todos los territorios existentes en el juego.
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

        // === 2. Posiciones normalizadas ===
        // Cada Vector2 indica la posición del territorio en relación con el mapa.
        posiciones = new Vector2[]
        {
            new Vector2(0.07f,0.62f), new Vector2(0.16f,0.65f), new Vector2(0.31f,0.74f),
            new Vector2(0.18f,0.56f), new Vector2(0.27f,0.57f), new Vector2(0.30f,0.56f),
            new Vector2(0.19f,0.46f), new Vector2(0.24f,0.46f), new Vector2(0.22f,0.35f),
            new Vector2(0.25f,0.34f), new Vector2(0.23f,0.22f), new Vector2(0.29f,0.25f), new Vector2(0.28f,0.13f),
            new Vector2(0.36f,0.67f), new Vector2(0.39f,0.58f), new Vector2(0.44f,0.66f),
            new Vector2(0.46f,0.59f), new Vector2(0.42f,0.57f), new Vector2(0.45f,0.51f), new Vector2(0.51f,0.59f),
            new Vector2(0.46f,0.43f), new Vector2(0.50f,0.38f), new Vector2(0.54f,0.31f),
            new Vector2(0.49f,0.28f), new Vector2(0.50f,0.17f), new Vector2(0.57f,0.19f),
            new Vector2(0.56f,0.60f), new Vector2(0.60f,0.66f), new Vector2(0.68f,0.67f), new Vector2(0.86f,0.62f),
            new Vector2(0.64f,0.61f), new Vector2(0.64f,0.53f), new Vector2(0.80f,0.50f), new Vector2(0.61f,0.49f),
            new Vector2(0.53f,0.43f), new Vector2(0.58f,0.39f), new Vector2(0.63f,0.42f), new Vector2(0.57f,0.52f),
            new Vector2(0.69f,0.37f), new Vector2(0.77f,0.36f), new Vector2(0.72f,0.23f), new Vector2(0.79f,0.24f)
        };

        // Arreglo para guardar referencias a los nodos instanciados.
        nodes = new TerritoryNode[ids.Length];

        // Crear visualmente los nodos y las líneas.
        InstanciarNodos();
        DibujarConexiones();
    }

    void InstanciarNodos()
    {
        // Se obtienen los límites del mapa (min y max del SpriteRenderer).
        var b = worldMap.bounds;

        for (int i = 0; i < ids.Length; i++)
        {
            var pos = posiciones[i];

            // Convierte coordenadas normalizadas (0–1) a posición real dentro del mapa.
            var worldPos = new Vector3(
                Mathf.Lerp(b.min.x, b.max.x, pos.x),
                Mathf.Lerp(b.min.y, b.max.y, pos.y),
                -0.01f // Se coloca ligeramente delante del mapa para que se vea visualmente.
            );

            // Instancia el prefab del territorio como hijo del objeto actual.
            var node = Instantiate(territoryPrefab, worldPos, Quaternion.identity, transform);

            // Asigna el ID y los datos lógicos (nombre, tropas).
            node.id = ids[i];
            node.SetData(mapa.GetName(ids[i]), mapa.GetTroops(ids[i]));

            // Guarda la referencia del nodo.
            nodes[i] = node;
        }

        
    }

    void DibujarConexiones()
    {
        
        var buffer = new TerritorioId[12];

        // Recorre todos los territorios y genera líneas hacia sus vecinos.
        for (int i = 0; i < ids.Length; i++)
        {
            var idA = ids[i];
            mapa.GetVecinos(idA, buffer, out int count);

            var nodeA = nodes[i];

            for (int j = 0; j < count; j++)
            {
                var idB = buffer[j];
                int idxB = GetIndex(idB);
                if (idxB < 0) continue; // Si el vecino no existe, se salta.

                var nodeB = nodes[idxB];

                // Crea la línea visual entre los dos nodos si ambos existen.
                if (nodeA != null && nodeB != null)
                    CrearLinea(nodeA.transform.position, nodeB.transform.position);
            }
        }

        
    }

    int GetIndex(TerritorioId id)
    {
        // Busca el índice correspondiente a un territorio en el arreglo de IDs.
        for (int i = 0; i < ids.Length; i++)
            if (ids[i] == id) return i;

        return -1; // Retorna -1 si no se encontró.
        
    }

    void CrearLinea(Vector3 a, Vector3 b)
    {
        // Crea un nuevo objeto para representar la línea entre dos nodos.
        var go = new GameObject("edge");
        go.transform.SetParent(transform, true);

        // Agrega un LineRenderer para dibujar visualmente la conexión.
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new[] { a, b });
        lr.widthMultiplier = lineWidth;
        lr.material = lineMaterial;

        
    }
}
