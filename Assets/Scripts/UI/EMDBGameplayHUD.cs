using UnityEngine;
using UnityEngine.UI;

public class EMDBGameplayHUD : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerLifeManager playerLifeManager;

    [Header("Hunger")]
    [SerializeField] private Image hungerBarFill;

    [Header("Energy")]
    [SerializeField] private Image energyBarFill;

    private void Update()
    {
        if (playerLifeManager == null)
            return;

        if (hungerBarFill != null)
        {
            hungerBarFill.fillAmount =
                Mathf.Clamp01(
                    (float)playerLifeManager.CurrentHunger /
                    playerLifeManager.MaxHunger
                );
        }

        if (energyBarFill != null)
        {
            energyBarFill.fillAmount =
                Mathf.Clamp01(
                    (float)playerLifeManager.CurrentEnergy /
                    playerLifeManager.MaxEnergy
                );
        }
    }
}