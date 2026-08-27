using System;
using UnityEngine;

public class CustomerReputationManager : MonoBehaviour
{
    public static CustomerReputationManager Instance { get; private set; }

    public static event Action OnReputationChanged;

    [Header("Reputation")]
    [SerializeField]
    private int reputationLevel = 1;

    [SerializeField]
    private string reputationName = "Very Poor";

    [Header("Reputation Levels")]
    [SerializeField]
    private int veryPoorLevel = 1;

    [SerializeField]
    private int poorLevel = 2;

    [SerializeField]
    private int averageLevel = 3;

    [SerializeField]
    private int goodLevel = 4;

    [SerializeField]
    private int excellentLevel = 5;

    public int ReputationLevel =>
        reputationLevel;

    public string ReputationName =>
        reputationName;

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
            "Customer Reputation Manager initialized."
        );
    }

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomerSatisfactionManager.OnSatisfactionChanged +=
            UpdateReputation;
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerSatisfactionManager.OnSatisfactionChanged -=
            UpdateReputation;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        UpdateReputation();
    }

    // =========================
    // UPDATE REPUTATION
    // =========================

    private void UpdateReputation()
    {
        if (CustomerSatisfactionManager.Instance == null)
        {
            return;
        }

        int satisfaction =
            CustomerSatisfactionManager.Instance
                .GetSatisfaction();

        int oldLevel =
            reputationLevel;

        string oldName =
            reputationName;

        if (satisfaction >= 100)
        {
            reputationLevel =
                excellentLevel;

            reputationName =
                "Excellent";
        }
        else if (satisfaction >= 75)
        {
            reputationLevel =
                goodLevel;

            reputationName =
                "Good";
        }
        else if (satisfaction >= 50)
        {
            reputationLevel =
                averageLevel;

            reputationName =
                "Average";
        }
        else if (satisfaction >= 25)
        {
            reputationLevel =
                poorLevel;

            reputationName =
                "Poor";
        }
        else
        {
            reputationLevel =
                veryPoorLevel;

            reputationName =
                "Very Poor";
        }

        if (oldLevel != reputationLevel ||
            oldName != reputationName)
        {
            Debug.Log(
                $"Customer reputation changed | " +
                $"Satisfaction: {satisfaction}/100 | " +
                $"Level: {reputationLevel} | " +
                $"Reputation: {reputationName}"
            );

            OnReputationChanged?.Invoke();
        }
    }

    // =========================
    // GET REPUTATION LEVEL
    // =========================

    public int GetReputationLevel()
    {
        return reputationLevel;
    }

    // =========================
    // GET REPUTATION NAME
    // =========================

    public string GetReputationName()
    {
        return reputationName;
    }
}