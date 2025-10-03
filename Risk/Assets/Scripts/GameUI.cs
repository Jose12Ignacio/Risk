using UnityEngine;
using UnityEngine.UI;
using TerritoryNode = CrazyRisk.TerritoryNode;

public class GameUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;      
    public Button[] botones;      // [0]=Atacar, [1]=Mover, [2]=Reforzar

    public delegate void AttackAction(TerritoryNode atq, TerritoryNode def);
    public delegate void MoveAction(TerritoryNode atq, TerritoryNode def);
    public delegate void ReinforceAction(TerritoryNode nodo);

    public event AttackAction AttackRequested;
    public event MoveAction MoveRequested;
    public event ReinforceAction ReinforceRequested;

    private TerritoryNode atacante;
    private TerritoryNode defensor;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);

        if (botones != null && botones.Length >= 3)
        {
            botones[0].onClick.AddListener(OnAttackClicked);
            botones[1].onClick.AddListener(OnMoveClicked);
            botones[2].onClick.AddListener(OnReinforceClicked);
        }
    }

    public void ShowActions(TerritoryNode atq)
    {
        atacante = atq;
        defensor = null;
        if (panel != null) panel.SetActive(true);

        botones[0].gameObject.SetActive(false);
        botones[1].gameObject.SetActive(false);
        botones[2].gameObject.SetActive(true);
    }

    public void ShowActions(TerritoryNode atq, TerritoryNode def)
    {
        atacante = atq;
        defensor = def;
        if (panel != null) panel.SetActive(true);

        botones[0].gameObject.SetActive(true);
        botones[1].gameObject.SetActive(true);
        botones[2].gameObject.SetActive(false);
    }

    public void Clear()
    {
        if (panel != null) panel.SetActive(false);
        atacante = null;
        defensor = null;
    }

    void OnAttackClicked()
    {
        if (AttackRequested != null && atacante != null && defensor != null)
            AttackRequested(atacante, defensor);
        Clear();
    }

    void OnMoveClicked()
    {
        if (MoveRequested != null && atacante != null && defensor != null)
            MoveRequested(atacante, defensor);
        Clear();
    }

    void OnReinforceClicked()
    {
        if (ReinforceRequested != null && atacante != null)
            ReinforceRequested(atacante);
        Clear();
    }
}
