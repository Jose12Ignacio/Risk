using UnityEngine;
using TMPro;

namespace CrazyRisk
{
    public class TerritoryNode : MonoBehaviour
    {
        [Header("Datos")]
        public TerritorioId id;         
        public string Nombre;           
        public int Tropas;              

        [Header("Refs UI")]
        public TMP_Text labelNombre;
        public TMP_Text labelTropas;
        public SpriteRenderer highlightCircle; 

        // Evento de click
        public event System.Action<TerritorioId> OnClicked;

        void OnMouseDown()
        {
            if (OnClicked != null)
                OnClicked(id);
        }

        // Inicializar datos
        public void SetData(string nombre, int tropas)
        {
            Nombre = nombre;
            Tropas = tropas;

            if (labelNombre != null)
                labelNombre.text = Nombre;

            if (labelTropas != null)
                labelTropas.text = Tropas.ToString();
        }

        // Actualizar tropas
        public void UpdateTroops(int nuevasTropas)
        {
            Tropas = nuevasTropas;
            if (labelTropas != null)
                labelTropas.text = Tropas.ToString();
        }

        // Resaltado visual
        public void SetHighlighted(bool activo)
        {
            if (highlightCircle != null)
                highlightCircle.enabled = activo;
        }
    }
}
