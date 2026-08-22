using UnityEngine;

public class CareerJobSwitchInteraction : MonoBehaviour, IInteractable
{
    [Header("Job Switch")]
    [SerializeField] private JobType newJob = JobType.Mechanic;
    [SerializeField] private int newSalary = 30000;

    public string InteractionPrompt =>
        $"Switch Career: {newJob}";

    public void Interact()
    {
        if (CareerManager.Instance == null)
        {
            Debug.LogError(
                "CareerJobSwitchInteraction: CareerManager not found."
            );

            return;
        }

        bool switched =
            CareerManager.Instance.SwitchJob(
                newJob,
                newSalary
            );

        if (!switched)
        {
            return;
        }

        Debug.Log(
            $"Career switch accepted | " +
            $"Job: {CareerManager.Instance.CurrentJob} | " +
            $"Salary: Rs. {CareerManager.Instance.CurrentSalary:N0}"
        );
    }
}