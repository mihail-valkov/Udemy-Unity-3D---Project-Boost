using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    //slider for the health

    [SerializeField] Slider healthSlider;
    [SerializeField] Slider fuelSlider;
    [SerializeField] TMPro.TextMeshProUGUI scoreText;
    [SerializeField] GameObject levelCompletePanel;
    [SerializeField] GameObject levelFailedPanel;

    Health playerHealth;


    void Awake()
    {
        playerHealth = FindObjectOfType<Health>();
    }

    void Start()
    {
        ResetScoreDisplay();
    }

    public void ResetScoreDisplay()
    {
        UpdateScoreText(GameManager.Instance.ScoreKeeper.Score);
        UpdateHealth(GameManager.Instance.PlayerSettings.MaxHealthValue);
        UpdateFuel(GameManager.Instance.PlayerSettings.MaxFuelValue);

        levelCompletePanel.SetActive(false);
        levelFailedPanel.SetActive(false);
    }

    public void ShowLevelCompletedUI()
    {
        levelCompletePanel.SetActive(true);
    }

    public void ShowLevelFailedUI()
    {
        levelFailedPanel.SetActive(true);
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void OnRestartLevel()
    {
        GameManager.Instance.RestartLevel();
    }

    public void OnNextLevel()
    {
        GameManager.Instance.NextLevel();
    }

    private void UpdateHealth(float health)
    {
        //display life in % of maxLifeValue on the health slider
        healthSlider.value = health / GameManager.Instance.PlayerSettings.MaxHealthValue;
    }

    public void UpdateFuel(float fuel)
    {
        //display fuel in % of maxFuelValue on the fuel slider
        fuelSlider.value = fuel / GameManager.Instance.PlayerSettings.MaxFuelValue;
    }

    void Update()
    {
        UpdateScoreText(GameManager.Instance.ScoreKeeper.Score);
        UpdateHealth(playerHealth.HealthValue);
        UpdateFuel(playerHealth.FuelValue);
    }
}
