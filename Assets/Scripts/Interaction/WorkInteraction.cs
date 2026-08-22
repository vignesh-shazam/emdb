using UnityEngine;

public class WorkInteraction : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Work";

    [Header("Work Settings")]
    [SerializeField] private int workEnergyCost = 10;
    [SerializeField] private int workHealthGain = 0;

    [Header("References")]
    [SerializeField] private PlayerLifeManager playerLifeManager;

    private void Awake()
    {
        if (playerLifeManager == null)
        {
            playerLifeManager =
                FindFirstObjectByType<PlayerLifeManager>();
        }
    }

    public void Interact()
    {
        if (CareerManager.Instance == null)
        {
            Debug.LogError(
                "WorkInteraction: CareerManager not found."
            );

            return;
        }

        bool workSuccessful =
            CareerManager.Instance.Work();

        if (!workSuccessful)
        {
            return;
        }

        if (playerLifeManager == null)
        {
            Debug.LogError(
                "WorkInteraction: PlayerLifeManager not found."
            );

            return;
        }

        if (workEnergyCost > 0)
        {
            playerLifeManager.ReduceEnergy(
                workEnergyCost
            );
        }

        if (workHealthGain > 0)
        {
            playerLifeManager.RestoreHealth(
                workHealthGain
            );
        }

        Debug.Log(
            $"Work interaction completed | " +
            $"Job: {CareerManager.Instance.CurrentJob} | " +
            $"Energy Cost: {workEnergyCost} | " +
            $"Health Gain: {workHealthGain}"
        );
    }
}