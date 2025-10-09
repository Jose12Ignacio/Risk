using UnityEngine;
using UnityEngine.UI;
using CrazyRisk;

public class RegionButtonUI : MonoBehaviour
{
    private Button button;
    private TerritorioId id;
    public event System.Action<TerritorioId> OnClicked;

    void Awake()
    {
        button = GetComponent<Button>(); //  Se autoasigna
    }

    public void Init(TerritorioId territorio)
    {
        id = territorio;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => HandleClick());
        }
        else
        {
            Debug.LogError("[RegionButtonUI] No se encontró Button en el prefab.");
        }
    }

    private void HandleClick()
    {
        Debug.Log($"[RegionButtonUI] Botón presionado para {id}");
        OnClicked?.Invoke(id);
    }
}
