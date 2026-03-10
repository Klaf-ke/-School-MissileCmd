using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LooseScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject looseScreenPanel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (looseScreenPanel != null)
            looseScreenPanel.SetActive(false);

       
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLevel);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitToMenu);
    }

   
    public void ShowLooseScreen()
    {
        if (looseScreenPanel != null)
        {
            looseScreenPanel.SetActive(true);
        }

        
        Time.timeScale = 0f;
    }

    private void RetryLevel()
    {
       
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu"); 
    }
}