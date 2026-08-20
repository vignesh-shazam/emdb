using UnityEngine;

public class BedInteraction : MonoBehaviour, IInteractable
{
    [Header("Player")]
    [SerializeField] private PlayerLifeManager playerLifeManager;

    [Header("Sleep Settings")]
    [SerializeField] private float energyRestorePerInteraction = 20f;

    private bool isSleeping;

    public string InteractionPrompt =>
        isSleeping ? "Wake Up" : "Sleep";

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
                "BedInteraction: PlayerLifeManager not found."
            );
        }
    }

    public void Interact()
    {
        if (playerLifeManager == null)
        {
            return;
        }

        if (!isSleeping)
        {
            StartSleeping();
        }
        else
        {
            WakeUp();
        }
    }

    private void StartSleeping()
    {
        isSleeping = true;

        playerLifeManager.SetActivity(
            PlayerActivity.Sleeping
        );

        playerLifeManager.RestoreEnergy(
            energyRestorePerInteraction
        );

        Debug.Log(
            $"Player is sleeping. Energy restored by " +
            $"{energyRestorePerInteraction}."
        );
    }

    private void WakeUp()
    {
        isSleeping = false;

        playerLifeManager.SetActivity(
            PlayerActivity.Idle
        );

        Debug.Log("Player woke up.");
    }
}