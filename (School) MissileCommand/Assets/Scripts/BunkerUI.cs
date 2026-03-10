using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BunkerUI : MonoBehaviour
{
    [SerializeField] private Bunker bunker;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text nameText;

    private void Start()
    {
        if (bunker != null)
        {
            healthSlider.maxValue = bunker.GetMaxHealth();
            nameText.text = bunker.name;
        }
    }

    private void Update()
    {
        if (bunker == null || bunker.IsDestroyed())
    {
        gameObject.SetActive(false);
        return;
    }

        healthSlider.value = bunker.GetCurrentHealth();
    }
}