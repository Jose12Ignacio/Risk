using UnityEngine;
using TMPro;

namespace CrazyRisk
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class TerritoryNode : MonoBehaviour
    {
        [Header("Datos")]
        public TerritorioId id;

        [Header("Labels (TMP)")]
        public TMP_Text labelNombre;
        public TMP_Text labelTropas;

        // === Selección global con arrays ===
        private static string[] territoriosSeleccionados = new string[0];

        private SpriteRenderer sr;
        private Color originalColor;
        private bool seleccionado = false;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            // 👀 Forzar Sorting Layer de nodos
            if (sr != null)
            {
                sr.sortingLayerName = "UI";  // o "Nodos" si la creaste
                sr.sortingOrder = 10;
                originalColor = sr.color;
            }

            // 👀 Forzar Sorting Layer en labels
            if (labelNombre != null)
            {
                var labelRenderer = labelNombre.GetComponent<MeshRenderer>();
                if (labelRenderer != null)
                {
                    labelRenderer.sortingLayerName = "UI";
                    labelRenderer.sortingOrder = 11; // encima del nodo
                }
            }

            if (labelTropas != null)
            {
                var tropasRenderer = labelTropas.GetComponent<MeshRenderer>();
                if (tropasRenderer != null)
                {
                    tropasRenderer.sortingLayerName = "UI";
                    tropasRenderer.sortingOrder = 11; // encima del nodo
                }
            }
        }

        void OnMouseDown()
        {
            string nombre = id.ToString();
            territoriosSeleccionados = ToggleTerritorio(territoriosSeleccionados, nombre);

            seleccionado = !seleccionado;
            SetHighlighted(seleccionado);

            Debug.Log("Click en: " + nombre);
            Debug.Log("Lista actual: " + string.Join(", ", territoriosSeleccionados));
        }

        void OnMouseEnter()
        {
            if (!seleccionado)
                SetHighlighted(true);
        }

        void OnMouseExit()
        {
            if (!seleccionado)
                SetHighlighted(false);
        }

        // === Mostrar datos en labels ===
        public void SetData(string nombre, int tropas)
        {
            if (labelNombre != null)
                labelNombre.text = nombre;

            if (labelTropas != null)
                labelTropas.text = tropas.ToString();
        }

        // === Resaltado visual ===
        public void SetHighlighted(bool estado)
        {
            if (sr == null) return;
            sr.color = estado ? Color.yellow : originalColor;
        }

        // === Toggle selección en arrays ===
        private static string[] ToggleTerritorio(string[] arreglo, string valor)
        {
            int index = -1;
            for (int i = 0; i < arreglo.Length; i++)
            {
                if (arreglo[i] == valor)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                string[] nuevoArray = new string[arreglo.Length - 1];
                int j = 0;
                for (int i = 0; i < arreglo.Length; i++)
                {
                    if (i != index)
                        nuevoArray[j++] = arreglo[i];
                }
                return nuevoArray;
            }

            string[] agregado = new string[arreglo.Length + 1];
            for (int i = 0; i < arreglo.Length; i++)
                agregado[i] = arreglo[i];
            agregado[agregado.Length - 1] = valor;
            return agregado;
        }

        public static string[] ObtenerSeleccionados()
        {
            return territoriosSeleccionados;
        }

        public static void LimpiarSeleccion()
        {
            territoriosSeleccionados = new string[0];
        }
    }
}

