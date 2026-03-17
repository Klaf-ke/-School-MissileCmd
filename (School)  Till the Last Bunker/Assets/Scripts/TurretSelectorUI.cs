using TMPro;
using UnityEngine;

public class TurretSelectorUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI turretText;

    [Header("Panel Manager")]
    [SerializeField] private TurretPanelManager panelManager;

    private int currentIndex = 0; 
    public int totalTurrets = 3;

    void Start()
    {
        UpdateSelection(currentIndex, totalTurrets);
    }

    public void UpdateSelection(int index, int total)
    {
        currentIndex = index;

        
        string display = "";
        for (int i = 0; i < total; i++)
        {
            if (i == currentIndex)
                display += $"<color=#FFD700>[{i + 1}]</color>  ";
            else
                display += $"{i + 1}  ";
        }

        if (turretText != null)
            turretText.text = display;

       
        if (panelManager != null)
            panelManager.ShowTurretPanel(currentIndex + 1); 
    }

    public void NextTurret()
    {
        int next = currentIndex + 1;
        if (next >= totalTurrets) next = 0;
        UpdateSelection(next, totalTurrets);
    }

    public void PreviousTurret()
    {
        int prev = currentIndex - 1;
        if (prev < 0) prev = totalTurrets - 1;
        UpdateSelection(prev, totalTurrets);
    }
}