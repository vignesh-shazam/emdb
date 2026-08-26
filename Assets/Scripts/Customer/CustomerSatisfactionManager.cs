using UnityEngine;

public class CustomerSatisfactionManager : MonoBehaviour
{
    public static CustomerSatisfactionManager Instance { get; private set; }

    [Header("Satisfaction")]
    [SerializeField]
    private int satisfaction = 0;

    [Header("Satisfaction Values")]
    [SerializeField]
    private int servedSatisfaction = 10;

    [SerializeField]
    private int leftSatisfaction = -10;

    public int Satisfaction =>
        satisfaction;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log(
            $"Customer Satisfaction Manager initialized | " +
            $"Satisfaction: {satisfaction}"
        );
    }

    // =========================
    // CUSTOMER SERVED
    // =========================

    public void CustomerServed(
        Customer customer)
    {
        if (customer == null)
        {
            Debug.LogWarning(
                "Satisfaction update failed: " +
                "Customer is null."
            );

            return;
        }

        AddSatisfaction(
            servedSatisfaction
        );

        Debug.Log(
            $"Customer served | " +
            $"Customer: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Satisfaction: +{servedSatisfaction} | " +
            $"Total: {satisfaction}"
        );
    }

    // =========================
    // CUSTOMER LEFT
    // =========================

    public void CustomerLeft(
        Customer customer)
    {
        if (customer == null)
        {
            Debug.LogWarning(
                "Satisfaction update failed: " +
                "Customer is null."
            );

            return;
        }

        AddSatisfaction(
            leftSatisfaction
        );

        Debug.Log(
            $"Customer left | " +
            $"Customer: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Satisfaction: {leftSatisfaction} | " +
            $"Total: {satisfaction}"
        );
    }

    // =========================
    // ADD SATISFACTION
    // =========================

    private void AddSatisfaction(
        int amount)
    {
        satisfaction += amount;
    }

    // =========================
    // RESET
    // =========================

    public void ResetSatisfaction()
    {
        satisfaction = 0;

        Debug.Log(
            "Customer satisfaction reset | " +
            "Total: 0"
        );
    }
}