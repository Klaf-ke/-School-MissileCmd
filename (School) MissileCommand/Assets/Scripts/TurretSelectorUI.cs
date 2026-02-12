using TMPro;
using UnityEngine;

public class TurretSelectorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turretText;

    private int currentIndex = 0;

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

        turretText.text = display;
    }
}