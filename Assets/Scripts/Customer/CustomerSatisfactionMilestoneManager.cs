using UnityEngine;

public class CustomerSatisfactionMilestoneManager : MonoBehaviour
{
    public static CustomerSatisfactionMilestoneManager Instance { get; private set; }

    [Header("Milestones")]
    [SerializeField]
    private int milestone25 = 25;

    [SerializeField]
    private int milestone50 = 50;

    [SerializeField]
    private int milestone75 = 75;

    [SerializeField]
    private int milestone100 = 100;

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

        ValidateMilestones();

        Debug.Log(
            "Customer Satisfaction Milestone Manager initialized | " +
            $"Milestones: {milestone25}, {milestone50}, " +
            $"{milestone75}, {milestone100}"
        );
    }

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomerSatisfactionManager.OnSatisfactionChanged +=
            CheckMilestones;
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerSatisfactionManager.OnSatisfactionChanged -=
            CheckMilestones;
    }

    // =========================
    // VALIDATE
    // =========================

    private void ValidateMilestones()
    {
        milestone25 = Mathf.Clamp(
            milestone25,
            1,
            100
        );

        milestone50 = Mathf.Clamp(
            milestone50,
            1,
            100
        );

        milestone75 = Mathf.Clamp(
            milestone75,
            1,
            100
        );

        milestone100 = Mathf.Clamp(
            milestone100,
            1,
            100
        );
    }

    // =========================
    // CHECK MILESTONES
    // =========================

    private void CheckMilestones()
    {
        if (CustomerSatisfactionManager.Instance == null)
        {
            return;
        }

        int satisfaction =
            CustomerSatisfactionManager.Instance
                .Satisfaction;

        CheckMilestone(
            milestone25,
            "Getting Better"
        );

        CheckMilestone(
            milestone50,
            "Good"
        );

        CheckMilestone(
            milestone75,
            "Very Good"
        );

        CheckMilestone(
            milestone100,
            "Excellent"
        );
    }

    // =========================
    // CHECK SINGLE MILESTONE
    // =========================

    private void CheckMilestone(
        int milestone,
        string milestoneName)
    {
        if (CustomerSatisfactionManager.Instance == null)
        {
            return;
        }

        int satisfaction =
            CustomerSatisfactionManager.Instance
                .Satisfaction;

        if (satisfaction == milestone)
        {
            Debug.Log(
                $"⭐ SATISFACTION MILESTONE REACHED | " +
                $"{milestone} | {milestoneName}"
            );
        }
    }

    // =========================
    // GET MILESTONE NAME
    // =========================

    public string GetMilestoneName()
    {
        if (CustomerSatisfactionManager.Instance == null)
        {
            return "Starting";
        }

        int satisfaction =
            CustomerSatisfactionManager.Instance
                .Satisfaction;

        if (satisfaction >= milestone100)
        {
            return "Excellent";
        }

        if (satisfaction >= milestone75)
        {
            return "Very Good";
        }

        if (satisfaction >= milestone50)
        {
            return "Good";
        }

        if (satisfaction >= milestone25)
        {
            return "Getting Better";
        }

        return "Starting";
    }

    // =========================
    // GET CURRENT MILESTONE
    // =========================

    public int GetCurrentMilestone()
    {
        if (CustomerSatisfactionManager.Instance == null)
        {
            return 0;
        }

        int satisfaction =
            CustomerSatisfactionManager.Instance
                .Satisfaction;

        if (satisfaction >= milestone100)
        {
            return milestone100;
        }

        if (satisfaction >= milestone75)
        {
            return milestone75;
        }

        if (satisfaction >= milestone50)
        {
            return milestone50;
        }

        if (satisfaction >= milestone25)
        {
            return milestone25;
        }

        return 0;
    }
}