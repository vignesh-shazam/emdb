using UnityEngine;

public class CareerPromotionInteraction : MonoBehaviour, IInteractable
{
    [Header("Promotion")]
    [SerializeField] private JobType newJob = JobType.DeliveryDriver;
    [SerializeField] private int newSalary = 35000;

    public string InteractionPrompt =>
        $"Promotion: {newJob}";

    public void Interact()
    {
        if (CareerManager.Instance == null)
        {
            Debug.LogError(
                "CareerPromotionInteraction: CareerManager not found."
            );

            return;
        }

        bool promoted =
            CareerManager.Instance.Promote(
                newJob,
                newSalary
            );

        if (!promoted)
        {
            return;
        }

        Debug.Log(
            $"Career promotion accepted | " +
            $"Job: {CareerManager.Instance.CurrentJob} | " +
            $"Salary: Rs. {CareerManager.Instance.CurrentSalary:N0}"
        );
    }
}