using UnityEngine;

public class TurretPanelManager : MonoBehaviour
{
    [Header("Turret Panels")]
    public GameObject turret1Panel;
    public GameObject turret2Panel;
    public GameObject turret3Panel;

    private int currentTurret = 1;

    void Start()
    {
       
        ShowTurretPanel(currentTurret);
    }

   
    public void ShowTurretPanel(int turretIndex)
    {
        currentTurret = turretIndex;

       
        if (turret1Panel != null) turret1Panel.SetActive(false);
        if (turret2Panel != null) turret2Panel.SetActive(false);
        if (turret3Panel != null) turret3Panel.SetActive(false);

        
        switch (turretIndex)
        {
            case 1:
                if (turret1Panel != null) turret1Panel.SetActive(true);
                break;
            case 2:
                if (turret2Panel != null) turret2Panel.SetActive(true);
                break;
            case 3:
                if (turret3Panel != null) turret3Panel.SetActive(true);
                break;
            default:
                Debug.LogWarning("Invalid turret index: " + turretIndex);
                break;
        }
    }
}