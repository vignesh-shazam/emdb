using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineManager : MonoBehaviour
{
    public static DailyRoutineManager Instance { get; private set; }

    // =========================
    // ROUTINE ACTIVITIES
    // =========================

    public enum RoutineActivity
    {
        WakeUp,
        Exercise,
        FreshUp,
        Breakfast,
        TakeCycle,
        RideToShop,
        ParkCycle,
        OpenShop,
        Work,
        CloseShop,
        RideHome,
        EnterHome,
        Sleep
    }

    // =========================
    // ROUTINE ENTRY
    // =========================

    [System.Serializable]
    public class RoutineEntry
    {
        public int hour;
        public int minute;
        public RoutineActivity activity;
    }

    // =========================
    // DAILY SCHEDULE
    // =========================

    [Header("Daily Schedule")]
    [SerializeField]
    private RoutineEntry[] dailySchedule =
    {
        new RoutineEntry
        {
            hour = 7,
            minute = 0,
            activity = RoutineActivity.WakeUp
        },

        new RoutineEntry
        {
            hour = 7,
            minute = 15,
            activity = RoutineActivity.Exercise
        },

        new RoutineEntry
        {
            hour = 7,
            minute = 45,
            activity = RoutineActivity.FreshUp
        },

        new RoutineEntry
        {
            hour = 8,
            minute = 15,
            activity = RoutineActivity.Breakfast
        },

        new RoutineEntry
        {
            hour = 8,
            minute = 45,
            activity = RoutineActivity.TakeCycle
        },

        new RoutineEntry
        {
            hour = 9,
            minute = 0,
            activity = RoutineActivity.RideToShop
        },

        new RoutineEntry
        {
            hour = 9,
            minute = 15,
            activity = RoutineActivity.ParkCycle
        },

        new RoutineEntry
        {
            hour = 9,
            minute = 20,
            activity = RoutineActivity.OpenShop
        },

        new RoutineEntry
        {
            hour = 9,
            minute = 20,
            activity = RoutineActivity.Work
        },

        new RoutineEntry
        {
            hour = 21,
            minute = 0,
            activity = RoutineActivity.CloseShop
        },

        new RoutineEntry
        {
            hour = 21,
            minute = 5,
            activity = RoutineActivity.TakeCycle
        },

        new RoutineEntry
        {
            hour = 21,
            minute = 10,
            activity = RoutineActivity.RideHome
        },

        new RoutineEntry
        {
            hour = 21,
            minute = 30,
            activity = RoutineActivity.ParkCycle
        },

        new RoutineEntry
        {
            hour = 21,
            minute = 35,
            activity = RoutineActivity.EnterHome
        },

        new RoutineEntry
        {
            hour = 22,
            minute = 0,
            activity = RoutineActivity.Sleep
        }
    };

    // =========================
    // COMPLETED ACTIVITIES
    // =========================

    private readonly HashSet<RoutineActivity>
        completedActivities =
        new HashSet<RoutineActivity>();

    // =========================
    // PROCESS TRACKING
    // =========================

    private int lastProcessedDay = -1;

    private int lastProcessedMinute = -1;

    // =========================
    // CURRENT ACTIVITY
    // =========================

    public RoutineActivity CurrentActivity
    {
        get;
        private set;
    }

    // =========================
    // COMPLETED ACTIVITIES
    // =========================

    public IReadOnlyCollection<RoutineActivity>
        CompletedActivities =>
        completedActivities;

    // =========================
    // AWAKE
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

        DontDestroyOnLoad(gameObject);
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (GameTimeManager.Instance == null)
        {
            return;
        }

        CheckRoutine();
    }

    // =========================
    // CHECK ROUTINE
    // =========================

    private void CheckRoutine()
    {
        int currentDay =
            GameTimeManager.Instance.CurrentDay;

        int currentHour =
            GameTimeManager.Instance.CurrentHour;

        int currentMinute =
            GameTimeManager.Instance.CurrentMinute;

        int currentTotalMinutes =
            currentHour * 60 +
            currentMinute;

        if (currentDay != lastProcessedDay)
        {
            StartNewDay(currentDay);
        }

        foreach (RoutineEntry entry
                 in dailySchedule)
        {
            int entryTotalMinutes =
                entry.hour * 60 +
                entry.minute;

            if (entryTotalMinutes <=
                    currentTotalMinutes &&
                entryTotalMinutes >
                    lastProcessedMinute)
            {
                ProcessActivity(
                    entry.activity
                );

                lastProcessedMinute =
                    entryTotalMinutes;
            }
        }
    }

    // =========================
    // START NEW DAY
    // =========================

    private void StartNewDay(int day)
    {
        lastProcessedDay = day;

        lastProcessedMinute = -1;

        completedActivities.Clear();

        Debug.Log(
            $"Daily Routine: New Day {day}"
        );
    }

    // =========================
    // PROCESS ACTIVITY
    // =========================

    private void ProcessActivity(
        RoutineActivity activity)
    {
        CurrentActivity = activity;

        completedActivities.Add(
            activity
        );

        Debug.Log(
            $"Daily Routine: {activity} completed."
        );

        ProcessFinanceForActivity(
            activity
        );
    }

    // =========================
    // FINANCE INTEGRATION
    // =========================

    private void ProcessFinanceForActivity(
        RoutineActivity activity)
    {
        switch (activity)
        {
            case RoutineActivity.Breakfast:

                ProcessExpense(
                    100,
                    ExpenseCategory.Food,
                    "Food"
                );

                break;

            case RoutineActivity.RideToShop:

                ProcessExpense(
                    50,
                    ExpenseCategory.Transport,
                    "Transport"
                );

                break;

            case RoutineActivity.RideHome:

                ProcessExpense(
                    50,
                    ExpenseCategory.Transport,
                    "Transport"
                );

                break;
        }
    }

    // =========================
    // PROCESS EXPENSE
    // =========================

    private void ProcessExpense(
        int amount,
        ExpenseCategory category,
        string description)
    {
        if (ExpenseManager.Instance == null)
        {
            Debug.LogWarning(
                "DailyRoutineManager: " +
                "ExpenseManager not found. " +
                "Finance expense skipped."
            );

            return;
        }

        bool success =
            ExpenseManager.Instance.Spend(
                amount,
                category,
                FinanceAccountType.Savings,
                description
            );

        if (success)
        {
            Debug.Log(
                $"Daily Routine Finance | " +
                $"Account: Employee | " +
                $"Expense: {description} | " +
                $"Amount: Rs. {amount:N0}"
            );
        }
        else
        {
            Debug.LogWarning(
                $"Daily Routine Finance | " +
                $"Expense failed | " +
                $"Account: Employee | " +
                $"Description: {description} | " +
                $"Amount: Rs. {amount:N0}"
            );
        }
    }

    // =========================
    // ACTIVITY STATUS
    // =========================

    public bool IsActivityCompleted(
        RoutineActivity activity)
    {
        return completedActivities.Contains(
            activity
        );
    }

    // =========================
    // COMPLETED COUNT
    // =========================

    public int GetCompletedActivityCount()
    {
        return completedActivities.Count;
    }

    // =========================
    // TOTAL COUNT
    // =========================

    public int GetTotalActivityCount()
    {
        return dailySchedule.Length;
    }
}