using UnityEngine;

public class ExerciseInteraction : MonoBehaviour, IInteractable
{
    [Header("Player")]
    [SerializeField] private PlayerLifeManager playerLifeManager;

    [Header("Exercise Settings")]
    [SerializeField] private float energyCost = 10f;
    [SerializeField] private float healthRestore = 2f;

    public string InteractionPrompt => "Exercise";

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
                "ExerciseInteraction: PlayerLifeManager not found."
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
            PlayerActivity.Exercising
        );

        playerLifeManager.ReduceEnergy(
            energyCost
        );

        playerLifeManager.RestoreHealth(
            healthRestore
        );

        Debug.Log(
            $"Player exercised. " +
            $"Energy -{energyCost}, " +
            $"Health +{healthRestore}."
        );

        playerLifeManager.SetActivity(
            PlayerActivity.Idle
        );
    }
}