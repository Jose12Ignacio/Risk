using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CrazyRisk;

public class TerritoryUI : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text labelNombre;
    public TMP_Text labelTropas;
    public Button boton;
    public SpriteRenderer highlightCircle;

    private TerritorioId id;
    private string nombre;
    private int tropas;

    public delegate void ClickAction(TerritorioId id);
    public event ClickAction OnClicked;

    // Inicializa con datos
    public void Init(TerritorioId id, string nombre, int tropas)
    {
        this.id = id;
        this.nombre = nombre;
        this.tropas = tropas;

        if (labelNombre != null)
            labelNombre.text = nombre;

        if (labelTropas != null)
            labelTropas.text = tropas.ToString();

        if (boton != null)
        {
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(() => OnClicked?.Invoke(id));

            // Ajustar tamaño al texto
            AjustarBoton();
        }
    }

    private void AjustarBoton()
    {
        var rt = boton.GetComponent<RectTransform>();

        if (labelNombre != null && labelTropas != null)
        {
            float width = Mathf.Max(labelNombre.preferredWidth, labelTropas.preferredWidth) + 20f;
            float height = labelNombre.preferredHeight + labelTropas.preferredHeight + 20f;
            rt.sizeDelta = new Vector2(width, height);
        }
        else if (labelNombre != null)
        {
            float width = labelNombre.preferredWidth + 20f;
            float height = labelNombre.preferredHeight + 20f;
            rt.sizeDelta = new Vector2(width, height);
        }
    }

    public void UpdateTroops(int nuevasTropas)
    {
        tropas = nuevasTropas;
        if (labelTropas != null)
            labelTropas.text = tropas.ToString();
    }

    public TerritorioId GetId() => id;
    public string GetName() => nombre;
    public int GetTroops() => tropas;
}
