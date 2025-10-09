using UnityEngine;
using TMPro;

namespace CrazyRisk
{
    // Este componente representa visualmente un territorio en el mapa.
    // Cada territorio tiene un identificador, nombre, cantidad de tropas y puede seleccionarse con el mouse.
    [RequireComponent(typeof(Collider2D))]     // Obliga a que el objeto tenga un collider 2D (para detectar clics).
    [RequireComponent(typeof(SpriteRenderer))] // Obliga a que tenga un SpriteRenderer (para mostrar el territorio).
    public class TerritoryNode : MonoBehaviour
    {
        [Header("Datos")]
        public TerritorioId id; // Identificador único del territorio.

        [Header("Labels (TMP)")]
        public TMP_Text labelNombre; // Referencia al texto con el nombre del territorio.
        public TMP_Text labelTropas; // Referencia al texto con la cantidad de tropas.

        // === Selección global ===
        // Almacena los nombres de los territorios seleccionados globalmente.
        // Se usa un arreglo en lugar de listas para mantener compatibilidad con el estilo del proyecto.
        private static string[] territoriosSeleccionados = new string[0];

        // Referencias internas.
        private SpriteRenderer sr;  // Controla el color y sprite del territorio.
        private Color originalColor; // Guarda el color original del territorio.
        private bool seleccionado = false; // Indica si el territorio está actualmente seleccionado.

        void Awake()
        {
            // Obtiene y guarda la referencia al SpriteRenderer.
            sr = GetComponent<SpriteRenderer>();
            originalColor = sr.color;

            // Asegura que el nodo esté en la capa correcta y con orden de dibujo sobre el mapa.
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 10;
        }


        // Evento: clic del mouse sobre el territorio.

        void OnMouseDown()
        {
            string nombre = id.ToString();

            // Alterna la selección del territorio en el arreglo global.
            territoriosSeleccionados = ToggleTerritorio(territoriosSeleccionados, nombre);

            // Cambia el estado de selección actual (true/false).
            seleccionado = !seleccionado;

            // Cambia visualmente el color del territorio seleccionado.
            SetHighlighted(seleccionado);

            // Muestra en consola el nombre y la lista actual de seleccionados.
            Debug.Log($" Click detectado en nodo: {nombre}");
            Debug.Log($"Lista actual: {string.Join(", ", territoriosSeleccionados)}");
        }


        // Eventos de entrada y salida del cursor del mouse.

        void OnMouseEnter()
        {
            // Si el territorio no está seleccionado, se resalta temporalmente al pasar el cursor.
            if (!seleccionado)
                SetHighlighted(true);
        }

        void OnMouseExit()
        {
            // Si el territorio no está seleccionado, se restaura su color original al salir el cursor.
            if (!seleccionado)
                SetHighlighted(false);
        }


        // Asigna los datos visuales (nombre y tropas) en los textos del territorio.

        public void SetData(string nombre, int tropas)
        {
            if (labelNombre != null)
                labelNombre.text = nombre; // Muestra el nombre del territorio.

            if (labelTropas != null)
                labelTropas.text = tropas.ToString(); // Muestra la cantidad de tropas.
        }


        // Controla el color de resaltado del territorio.

        public void SetHighlighted(bool estado)
        {
            if (sr == null) return;

            // Si está resaltado, usa color amarillo; de lo contrario, el color original.
            sr.color = estado ? Color.yellow : originalColor;
        }


        // Agrega o elimina un territorio del arreglo de seleccionados.

        private static string[] ToggleTerritorio(string[] arreglo, string valor)
        {
            // Busca si el territorio ya está en el arreglo.
            int index = -1;
            for (int i = 0; i < arreglo.Length; i++)
            {
                if (arreglo[i] == valor)
                {
                    index = i;
                    break;
                }
            }

            // Si el territorio ya estaba seleccionado, se elimina del arreglo.
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

            // Si el territorio no estaba seleccionado, se agrega al arreglo.
            string[] agregado = new string[arreglo.Length + 1];
            for (int i = 0; i < arreglo.Length; i++)
                agregado[i] = arreglo[i];

            agregado[agregado.Length - 1] = valor;
            return agregado;
        }


        // Devuelve el arreglo actual de territorios seleccionados.

        public static string[] ObtenerSeleccionados()
        {
            return territoriosSeleccionados;
        }


        // Limpia todas las selecciones globales.

        public static void LimpiarSeleccion()
        {
            territoriosSeleccionados = new string[0];
        }
    }
}
