using TMPro;
using UnityEngine;

public class PlayerLifeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLifeManager playerLifeManager;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private TextMeshProUGUI activityText;

    private void Start()
    {
        if (playerLifeManager == null)
        {
            playerLifeManager =
                FindFirstObjectByType<PlayerLifeManager>();
        }

        if (playerLifeManager == null)
        {
            Debug.LogError(
                "PlayerLifeUI: PlayerLifeManager not found."
            );

            return;
        }

        UpdateUI();
    }

    private void Update()
    {
        if (playerLifeManager == null)
        {
            return;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text =
                $"Health: {playerLifeManager.CurrentHealth:0} / " +
                $"{playerLifeManager.MaxHealth:0}";
        }

        if (energyText != null)
        {
            energyText.text =
                $"Energy: {playerLifeManager.CurrentEnergy:0} / " +
                $"{playerLifeManager.MaxEnergy:0}";
        }

        if (hungerText != null)
        {
            hungerText.text =
                $"Hunger: {playerLifeManager.CurrentHunger:0} / " +
                $"{playerLifeManager.MaxHunger:0}";
        }

        if (activityText != null)
        {
            activityText.text =
                $"Activity: {playerLifeManager.CurrentActivity}";
        }
    }
}