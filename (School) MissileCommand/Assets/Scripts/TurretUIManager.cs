using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TurretUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text enemiesText;
    public TMP_Text waveText;
    public TMP_Text nextWaveText;
    public Slider waveProgressBar;

    [Header("Next Wave Settings")]
    public float nextWaveDisplayTime = 2f;

    private int currentWave = 1;
    private int remainingEnemies = 0;
    private int totalEnemiesThisWave = 1;

    // -------------------

    public void SetWave(int waveNumber, int enemiesThisWave)
    {
        currentWave = waveNumber;
        waveText.text = "Wave: " + currentWave;

        totalEnemiesThisWave = enemiesThisWave;
        remainingEnemies = enemiesThisWave;

        UpdateEnemiesUI();
        ShowNextWaveMessage();
        UpdateProgressBar();
    }

    public void SetRemainingEnemies(int count)
    {
        remainingEnemies = count;
        UpdateEnemiesUI();
        UpdateProgressBar();
    }

    public void EnemyKilled()
    {
        remainingEnemies--;
        if (remainingEnemies < 0)
            remainingEnemies = 0;

        UpdateEnemiesUI();
        UpdateProgressBar();
    }

    private void UpdateEnemiesUI()
    {
        enemiesText.text = "Enemies: " + remainingEnemies;
    }

    private void UpdateProgressBar()
    {
        if (totalEnemiesThisWave > 0)
            waveProgressBar.value = (float)remainingEnemies / totalEnemiesThisWave;
        else
            waveProgressBar.value = 0;
    }

    private void ShowNextWaveMessage()
    {
        if (nextWaveText == null)
            return;

        StopAllCoroutines();
        nextWaveText.gameObject.SetActive(true);
        StartCoroutine(HideNextWaveAfterTime());
    }

    private IEnumerator HideNextWaveAfterTime()
    {
        yield return new WaitForSeconds(nextWaveDisplayTime);
        nextWaveText.gameObject.SetActive(false);
    }
}