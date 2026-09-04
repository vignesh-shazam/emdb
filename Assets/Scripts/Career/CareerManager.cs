using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CareerJobRequirement
{
    [Tooltip("Job that this requirement applies to.")]
    public JobType job;

    [Tooltip("Required previous/current job.")]
    public JobType requiredJob;

    [Tooltip("Display message when the requirement is not satisfied.")]
    public string requirementMessage =
        "Required career experience not met.";
}

public class CareerManager : MonoBehaviour
{
    public static CareerManager Instance { get; private set; }

    [Header("Starting Career")]
    [SerializeField]
    private JobType startingJob =
        JobType.Unemployed;

    [SerializeField]
    private int startingSalary = 0;

    [Header("Career Requirements")]
    [SerializeField]
    private List<CareerJobRequirement> jobRequirements =
        new List<CareerJobRequirement>();

    [Header("Career Experience")]
    [SerializeField]
    private int startingExperience = 0;

    [SerializeField]
    private int startingCareerLevel = 1;

    [SerializeField]
    private int experiencePerWork = 25;

    [SerializeField]
    private int baseExperienceRequired = 100;

    private CareerData careerData;

    private int lastWorkedDay = -1;

    private int currentExperience;

    private int careerLevel;

    // =========================
    // SALARY TRACKING
    // =========================

    private int lastSalaryPaidMonth = -1;

    // =========================
    // PUBLIC PROPERTIES
    // =========================

    public JobType CurrentJob =>
        careerData != null
            ? careerData.JobType
            : JobType.Unemployed;

    public int CurrentSalary =>
        careerData != null
            ? careerData.Salary
            : 0;

    public bool IsEmployed =>
        careerData != null &&
        careerData.IsEmployed;

    public int LastWorkedDay =>
        lastWorkedDay;

    public int CurrentExperience =>
        currentExperience;

    public int CareerLevel =>
        careerLevel;

    public int ExperienceRequiredForNextLevel =>
        CalculateExperienceRequired(
            careerLevel
        );

    public float ExperienceProgress =>
        ExperienceRequiredForNextLevel <= 0
            ? 0f
            : (float)currentExperience /
              ExperienceRequiredForNextLevel;

    public int LastSalaryPaidMonth =>
        lastSalaryPaidMonth;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeCareer();
    }

    private void Update()
    {
        ProcessMonthlySalary();
    }

    // =========================
    // INITIALIZATION
    // =========================

    private void InitializeCareer()
    {
        bool employed =
            startingJob != JobType.Unemployed;

        careerData = new CareerData(
            startingJob,
            Mathf.Max(0, startingSalary),
            employed
        );

        lastWorkedDay = -1;

        lastSalaryPaidMonth = -1;

        currentExperience =
            Mathf.Max(
                0,
                startingExperience
            );

        careerLevel =
            Mathf.Max(
                1,
                startingCareerLevel
            );

        Debug.Log(
            $"Career initialized | " +
            $"Job: {careerData.JobType} | " +
            $"Salary: Rs. {careerData.Salary:N0} | " +
            $"Employed: {careerData.IsEmployed} | " +
            $"Level: {careerLevel} | " +
            $"XP: {currentExperience}/" +
            $"{ExperienceRequiredForNextLevel}"
        );
    }

    private bool EnsureCareerInitialized()
    {
        if (careerData != null)
        {
            return true;
        }

        Debug.LogWarning(
            "CareerManager: Career data was not initialized. " +
            "Initializing now."
        );

        InitializeCareer();

        return careerData != null;
    }

    // =========================
    // MONTHLY SALARY
    // =========================

    private void ProcessMonthlySalary()
    {
        if (!IsEmployed)
        {
            return;
        }

        if (CurrentSalary <= 0)
        {
            return;
        }

        if (GameTimeManager.Instance == null)
        {
            return;
        }

        int currentMonth =
            GameTimeManager.Instance.CurrentMonth;

        if (!GameTimeManager.Instance.IsLastDayOfMonth)
        {
            return;
        }

        if (lastSalaryPaidMonth ==
            currentMonth)
        {
            return;
        }

        if (IncomeManager.Instance == null)
        {
            Debug.LogError(
                "Salary payment failed: " +
                "IncomeManager not found."
            );

            return;
        }

        bool salaryCredited =
            IncomeManager.Instance.AddIncome(
                CurrentSalary,
                "Salary Credited",
                FinanceAccountType.Savings
            );

        if (!salaryCredited)
        {
            Debug.LogWarning(
                $"Salary payment failed | " +
                $"Month: {currentMonth} | " +
                $"Salary: Rs. {CurrentSalary:N0}"
            );

            return;
        }

        lastSalaryPaidMonth =
            currentMonth;

        Debug.Log(
            $"Monthly salary credited | " +
            $"Month: {currentMonth} | " +
            $"Day: {GameTimeManager.Instance.CurrentDay} | " +
            $"Salary: Rs. {CurrentSalary:N0}"
        );
    }

    // =========================
    // CAREER REQUIREMENTS
    // =========================

    public bool CanTakeJob(
        JobType newJob)
    {
        if (!EnsureCareerInitialized())
        {
            return false;
        }

        if (newJob == JobType.Unemployed)
        {
            Debug.LogWarning(
                "Career requirement failed: " +
                "Unemployed cannot be selected as a career."
            );

            return false;
        }

        if (newJob == CurrentJob)
        {
            Debug.LogWarning(
                $"Career requirement failed: " +
                $"Player is already a {CurrentJob}."
            );

            return false;
        }

        CareerJobRequirement requirement =
            FindRequirement(newJob);

        if (requirement == null)
        {
            return true;
        }

        if (CurrentJob != requirement.requiredJob)
        {
            string message =
                string.IsNullOrWhiteSpace(
                    requirement.requirementMessage)
                    ? $"Job requires {requirement.requiredJob}."
                    : requirement.requirementMessage;

            Debug.LogWarning(
                $"Career requirement failed | " +
                $"Job: {newJob} | " +
                $"Required: {requirement.requiredJob} | " +
                $"Current: {CurrentJob} | " +
                $"{message}"
            );

            return false;
        }

        return true;
    }

    private CareerJobRequirement FindRequirement(
        JobType job)
    {
        if (jobRequirements == null)
        {
            return null;
        }

        foreach (
            CareerJobRequirement requirement
            in jobRequirements)
        {
            if (requirement == null)
            {
                continue;
            }

            if (requirement.job == job)
            {
                return requirement;
            }
        }

        return null;
    }

    // =========================
    // CAREER
    // =========================

    public void SetCareer(
        JobType jobType,
        int salary)
    {
        if (!EnsureCareerInitialized())
        {
            return;
        }

        bool employed =
            jobType != JobType.Unemployed;

        careerData = new CareerData(
            jobType,
            Mathf.Max(0, salary),
            employed
        );

        lastWorkedDay = -1;

        Debug.Log(
            $"Career updated | " +
            $"Job: {careerData.JobType} | " +
            $"Salary: Rs. {careerData.Salary:N0} | " +
            $"Employed: {careerData.IsEmployed}"
        );
    }

    public void QuitJob()
    {
        if (!EnsureCareerInitialized())
        {
            return;
        }

        careerData = new CareerData(
            JobType.Unemployed,
            0,
            false
        );

        lastWorkedDay = -1;

        Debug.Log(
            "Player is now unemployed."
        );
    }

    // =========================
    // WORK
    // =========================

    public bool Work()
    {
        if (!EnsureCareerInitialized())
        {
            return false;
        }

        if (!IsEmployed)
        {
            Debug.LogWarning(
                "Work failed: Player is unemployed."
            );

            return false;
        }

        if (GameTimeManager.Instance == null)
        {
            Debug.LogError(
                "Work failed: GameTimeManager not found."
            );

            return false;
        }

        int currentDay =
            GameTimeManager.Instance.CurrentDay;

        if (lastWorkedDay == currentDay)
        {
            Debug.LogWarning(
                $"Work failed: Already worked on Day {currentDay}."
            );

            return false;
        }

        // =========================
        // WORK
        // =========================

        lastWorkedDay =
            currentDay;

        AddExperience(
            experiencePerWork
        );

        Debug.Log(
            $"Work completed | " +
            $"Day: {currentDay} | " +
            $"Job: {CurrentJob} | " +
            $"Salary pending until month end: " +
            $"Rs. {CurrentSalary:N0}"
        );

        return true;
    }

    // =========================
    // CAREER EXPERIENCE
    // =========================

    private void AddExperience(
        int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentExperience += amount;

        Debug.Log(
            $"Career XP gained | " +
            $"+{amount} XP | " +
            $"Current XP: {currentExperience}/" +
            $"{ExperienceRequiredForNextLevel} | " +
            $"Level: {careerLevel}"
        );

        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        int requiredXP =
            ExperienceRequiredForNextLevel;

        while (currentExperience >= requiredXP)
        {
            currentExperience -=
                requiredXP;

            careerLevel++;

            Debug.Log(
                $"Career level up! | " +
                $"New Level: {careerLevel}"
            );

            requiredXP =
                ExperienceRequiredForNextLevel;
        }
    }

    private int CalculateExperienceRequired(
        int level)
    {
        if (level <= 1)
        {
            return Mathf.Max(
                1,
                baseExperienceRequired
            );
        }

        return Mathf.Max(
            1,
            baseExperienceRequired +
            ((level - 1) * 50)
        );
    }

    // =========================
    // PROMOTION
    // =========================

    public bool Promote(
        JobType newJob,
        int newSalary)
    {
        if (!EnsureCareerInitialized())
        {
            return false;
        }

        if (!IsEmployed)
        {
            Debug.LogWarning(
                "Promotion failed: Player is unemployed."
            );

            return false;
        }

        if (newJob == JobType.Unemployed)
        {
            Debug.LogWarning(
                "Promotion failed: New job cannot be Unemployed."
            );

            return false;
        }

        if (newSalary <= 0)
        {
            Debug.LogWarning(
                "Promotion failed: Salary must be greater than zero."
            );

            return false;
        }

        if (newJob == CurrentJob)
        {
            Debug.LogWarning(
                $"Promotion failed: Player is already a {CurrentJob}."
            );

            return false;
        }

        if (newSalary <= CurrentSalary)
        {
            Debug.LogWarning(
                "Promotion failed: New salary must be higher " +
                "than the current salary."
            );

            return false;
        }

        if (!CanTakeJob(newJob))
        {
            Debug.LogWarning(
                $"Promotion failed: " +
                $"Requirements not met for {newJob}."
            );

            return false;
        }

        JobType previousJob =
            CurrentJob;

        int previousSalary =
            CurrentSalary;

        careerData = new CareerData(
            newJob,
            newSalary,
            true
        );

        Debug.Log(
            $"Promotion completed | " +
            $"Previous Job: {previousJob} | " +
            $"Previous Salary: Rs. {previousSalary:N0} | " +
            $"New Job: {CurrentJob} | " +
            $"New Salary: Rs. {CurrentSalary:N0}"
        );

        return true;
    }

    // =========================
    // JOB SWITCHING
    // =========================

    public bool SwitchJob(
        JobType newJob,
        int newSalary)
    {
        if (!EnsureCareerInitialized())
        {
            return false;
        }

        if (newJob == JobType.Unemployed)
        {
            Debug.LogWarning(
                "Job switch failed: " +
                "New job cannot be Unemployed."
            );

            return false;
        }

        if (newSalary <= 0)
        {
            Debug.LogWarning(
                "Job switch failed: " +
                "Salary must be greater than zero."
            );

            return false;
        }

        if (newJob == CurrentJob)
        {
            Debug.LogWarning(
                $"Job switch failed: " +
                $"Player is already a {CurrentJob}."
            );

            return false;
        }

        if (!CanTakeJob(newJob))
        {
            Debug.LogWarning(
                $"Job switch failed: " +
                $"Requirements not met for {newJob}."
            );

            return false;
        }

        JobType previousJob =
            CurrentJob;

        int previousSalary =
            CurrentSalary;

        careerData = new CareerData(
            newJob,
            newSalary,
            true
        );

        Debug.Log(
            $"Job switch completed | " +
            $"Previous Job: {previousJob} | " +
            $"Previous Salary: Rs. {previousSalary:N0} | " +
            $"New Job: {CurrentJob} | " +
            $"New Salary: Rs. {CurrentSalary:N0}"
        );

        return true;
    }
}