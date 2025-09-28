using UnityEngine;
using System;
using TMPro;

public class TerritoryNode : MonoBehaviour
{
    public CrazyRisk.TerritorioId id;
    public SpriteRenderer spriteRenderer;
    public TextMeshPro textLabel;

    public event Action<CrazyRisk.TerritorioId> OnClicked;

    void OnMouseDown() => OnClicked?.Invoke(id);

    public void SetHighlighted(bool on)
    {
        if (!spriteRenderer) return;
        var c = spriteRenderer.color;
        spriteRenderer.color = on ? new Color(c.r, c.g, c.b, 1f)
                                  : new Color(c.r, c.g, c.b, 0.5f);
    }

    public void SetText(string t)
    {
        if (textLabel) textLabel.text = t;
    }
}
