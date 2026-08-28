using System;
using UnityEngine;

public class CustomerSatisfactionManager : MonoBehaviour
{
    public static CustomerSatisfactionManager Instance { get; private set; }

    public static event Action OnSatisfactionChanged;
    public static event Action OnMaximumSatisfactionReached;

    [Header("Satisfaction")]
    [SerializeField]
    private int satisfaction = 0;

    [Header("Satisfaction Limit")]
    [SerializeField]
    private int maximumSatisfaction = 100;

    [Header("Satisfaction Values")]
    [SerializeField]
    private int servedSatisfaction = 10;

    [SerializeField]
    private int leftSatisfaction = -10;

    // =========================
    // PUBLIC PROPERTIES
    // =========================

    public int Satisfaction =>
        satisfaction;

    public int MaximumSatisfaction =>
        maximumSatisfaction;

    public int ServedSatisfaction =>
        servedSatisfaction;

    public int LeftSatisfaction =>
        leftSatisfaction;

    public bool IsMaximumSatisfaction =>
        satisfaction >= maximumSatisfaction;

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

        ValidateSettings();

        satisfaction = Mathf.Clamp(
            satisfaction,
            0,
            maximumSatisfaction
        );

        Debug.Log(
            $"Customer Satisfaction Manager initialized | " +
            $"Satisfaction: {satisfaction}/{maximumSatisfaction}"
        );

        OnSatisfactionChanged?.Invoke();
    }

    // =========================
    // VALIDATE SETTINGS
    // =========================

    private void ValidateSettings()
    {
        if (maximumSatisfaction < 1)
        {
            maximumSatisfaction = 100;

            Debug.LogWarning(
                "Maximum Satisfaction was invalid. " +
                "Reset to 100."
            );
        }

        if (servedSatisfaction < 0)
        {
            servedSatisfaction = 0;

            Debug.LogWarning(
                "Served Satisfaction cannot be negative. " +
                "Reset to 0."
            );
        }

        if (leftSatisfaction > 0)
        {
            leftSatisfaction = -10;

            Debug.LogWarning(
                "Left Satisfaction should be negative. " +
                "Reset to -10."
            );
        }
    }

    // =========================
    // CUSTOMER SERVED
    // =========================

    public void CustomerServed(Customer customer)
    {
        if (customer == null)
        {
            Debug.LogWarning(
                "Satisfaction update failed: " +
                "Customer is null."
            );

            return;
        }

        AddSatisfaction(servedSatisfaction);

        Debug.Log(
            $"Customer served | " +
            $"Customer: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Satisfaction: +{servedSatisfaction} | " +
            $"Total: {satisfaction}/{maximumSatisfaction}"
        );
    }

    // =========================
    // CUSTOMER LEFT
    // =========================

    public void CustomerLeft(Customer customer)
    {
        if (customer == null)
        {
            Debug.LogWarning(
                "Satisfaction update failed: " +
                "Customer is null."
            );

            return;
        }

        AddSatisfaction(leftSatisfaction);

        Debug.Log(
            $"Customer left | " +
            $"Customer: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Satisfaction: {leftSatisfaction} | " +
            $"Total: {satisfaction}/{maximumSatisfaction}"
        );
    }

    // =========================
    // ADD SATISFACTION
    // =========================

    private void AddSatisfaction(int amount)
    {
        bool wasMaximum =
            satisfaction >= maximumSatisfaction;

        int previousSatisfaction =
            satisfaction;

        satisfaction += amount;

        satisfaction = Mathf.Clamp(
            satisfaction,
            0,
            maximumSatisfaction
        );

        if (previousSatisfaction != satisfaction)
        {
            OnSatisfactionChanged?.Invoke();
        }

        if (!wasMaximum &&
            satisfaction >= maximumSatisfaction)
        {
            OnMaximumSatisfactionReached?.Invoke();

            Debug.Log(
                "⭐ MAXIMUM CUSTOMER SATISFACTION REACHED! " +
                $"({maximumSatisfaction})"
            );
        }
    }

    // =========================
    // GET SATISFACTION
    // =========================

    public int GetSatisfaction()
    {
        return satisfaction;
    }

    // =========================
    // SET SATISFACTION
    // =========================

    public void SetSatisfaction(int value)
    {
        int previousSatisfaction =
            satisfaction;

        satisfaction = Mathf.Clamp(
            value,
            0,
            maximumSatisfaction
        );

        if (previousSatisfaction != satisfaction)
        {
            OnSatisfactionChanged?.Invoke();
        }

        if (previousSatisfaction < maximumSatisfaction &&
            satisfaction >= maximumSatisfaction)
        {
            OnMaximumSatisfactionReached?.Invoke();

            Debug.Log(
                "⭐ MAXIMUM CUSTOMER SATISFACTION REACHED! " +
                $"({maximumSatisfaction})"
            );
        }
    }

    // =========================
    // RESET
    // =========================

    public void ResetSatisfaction()
    {
        int previousSatisfaction =
            satisfaction;

        satisfaction = 0;

        if (previousSatisfaction != satisfaction)
        {
            OnSatisfactionChanged?.Invoke();
        }

        Debug.Log(
            "Customer satisfaction reset | " +
            $"Total: 0/{maximumSatisfaction}"
        );
    }
}