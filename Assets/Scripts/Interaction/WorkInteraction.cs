using UnityEngine;

public class WorkInteraction : MonoBehaviour, IInteractable
{
    [Header("Player")]
    [SerializeField] private PlayerLifeManager playerLifeManager;

    [Header("Work Settings")]
    [SerializeField] private float energyCost = 15f;
    [SerializeField] private float hungerCost = 10f;

    public string InteractionPrompt => "Work";

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
                "WorkInteraction: PlayerLifeManager not found."
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
            PlayerActivity.Working
        );

        playerLifeManager.ReduceEnergy(
            energyCost
        );

        playerLifeManager.ReduceHunger(
            hungerCost
        );

        Debug.Log(
            $"Player worked. " +
            $"Energy -{energyCost}, " +
            $"Hunger -{hungerCost}."
        );

        playerLifeManager.SetActivity(
            PlayerActivity.Idle
        );
    }
}