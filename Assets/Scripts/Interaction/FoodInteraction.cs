using UnityEngine;

public class FoodInteraction : MonoBehaviour, IInteractable
{
    [Header("Player")]
    [SerializeField] private PlayerLifeManager playerLifeManager;

    [Header("Food Settings")]
    [SerializeField] private float hungerRestoreAmount = 25f;

    public string InteractionPrompt => "Eat";

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
                "FoodInteraction: PlayerLifeManager not found."
            );
        }
    }

    public void Interact()
    {
        if (playerLifeManager == null)
        {
            return;
        }

        playerLifeManager.SetActivity(
            PlayerActivity.Eating
        );

        playerLifeManager.IncreaseHunger(
            hungerRestoreAmount
        );

        Debug.Log(
            $"Player ate food. Hunger restored by " +
            $"{hungerRestoreAmount}."
        );

        playerLifeManager.SetActivity(
            PlayerActivity.Idle
        );
    }
}