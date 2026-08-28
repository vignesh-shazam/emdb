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

    [Header("Satisfaction Thresholds")]
    [SerializeField]
    private int poorThreshold = 25;

    [SerializeField]
    private int averageThreshold = 50;

    [SerializeField]
    private int goodThreshold = 75;

    [SerializeField]
    private int excellentThreshold = 100;

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

        ValidateSettings();

        Debug.Log(
            "Customer Reputation Manager initialized | " +
            $"Level: {reputationLevel} | " +
            $"Reputation: {reputationName}"
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
    // VALIDATE SETTINGS
    // =========================

    private void ValidateSettings()
    {
        poorThreshold =
            Mathf.Clamp(poorThreshold, 1, 100);

        averageThreshold =
            Mathf.Clamp(averageThreshold, 1, 100);

        goodThreshold =
            Mathf.Clamp(goodThreshold, 1, 100);

        excellentThreshold =
            Mathf.Clamp(excellentThreshold, 1, 100);

        veryPoorLevel =
            Mathf.Max(1, veryPoorLevel);

        poorLevel =
            Mathf.Max(1, poorLevel);

        averageLevel =
            Mathf.Max(1, averageLevel);

        goodLevel =
            Mathf.Max(1, goodLevel);

        excellentLevel =
            Mathf.Max(1, excellentLevel);
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
                .Satisfaction;

        int oldLevel =
            reputationLevel;

        string oldName =
            reputationName;

        // =========================
        // EXCELLENT
        // =========================

        if (satisfaction >= excellentThreshold)
        {
            reputationLevel =
                excellentLevel;

            reputationName =
                "Excellent";
        }

        // =========================
        // GOOD
        // =========================

        else if (satisfaction >= goodThreshold)
        {
            reputationLevel =
                goodLevel;

            reputationName =
                "Good";
        }

        // =========================
        // AVERAGE
        // =========================

        else if (satisfaction >= averageThreshold)
        {
            reputationLevel =
                averageLevel;

            reputationName =
                "Average";
        }

        // =========================
        // POOR
        // =========================

        else if (satisfaction >= poorThreshold)
        {
            reputationLevel =
                poorLevel;

            reputationName =
                "Poor";
        }

        // =========================
        // VERY POOR
        // =========================

        else
        {
            reputationLevel =
                veryPoorLevel;

            reputationName =
                "Very Poor";
        }

        // =========================
        // CHANGE DETECTION
        // =========================

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