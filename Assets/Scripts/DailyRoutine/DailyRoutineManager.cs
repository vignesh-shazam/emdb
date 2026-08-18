using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineManager : MonoBehaviour
{
    public static DailyRoutineManager Instance { get; private set; }

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

    [System.Serializable]
    public class RoutineEntry
    {
        public int hour;
        public int minute;
        public RoutineActivity activity;
    }

    [Header("Daily Schedule")]
    [SerializeField]
    private RoutineEntry[] dailySchedule =
    {
        new RoutineEntry { hour = 7,  minute = 0,  activity = RoutineActivity.WakeUp },
        new RoutineEntry { hour = 7,  minute = 15, activity = RoutineActivity.Exercise },
        new RoutineEntry { hour = 7,  minute = 45, activity = RoutineActivity.FreshUp },
        new RoutineEntry { hour = 8,  minute = 15, activity = RoutineActivity.Breakfast },
        new RoutineEntry { hour = 8,  minute = 45, activity = RoutineActivity.TakeCycle },
        new RoutineEntry { hour = 9,  minute = 0,  activity = RoutineActivity.RideToShop },
        new RoutineEntry { hour = 9,  minute = 15, activity = RoutineActivity.ParkCycle },
        new RoutineEntry { hour = 9,  minute = 20, activity = RoutineActivity.OpenShop },
        new RoutineEntry { hour = 9,  minute = 20, activity = RoutineActivity.Work },
        new RoutineEntry { hour = 21, minute = 0,  activity = RoutineActivity.CloseShop },
        new RoutineEntry { hour = 21, minute = 5,  activity = RoutineActivity.TakeCycle },
        new RoutineEntry { hour = 21, minute = 10,  activity = RoutineActivity.RideHome },
        new RoutineEntry { hour = 21, minute = 30,  activity = RoutineActivity.ParkCycle },
        new RoutineEntry { hour = 21, minute = 35, activity = RoutineActivity.EnterHome },
        new RoutineEntry { hour = 22, minute = 0, activity = RoutineActivity.Sleep }
    };

    private readonly HashSet<RoutineActivity> completedActivities = new();

    private int lastProcessedDay = -1;
    private int lastProcessedMinute = -1;

    public RoutineActivity CurrentActivity { get; private set; }

    public IReadOnlyCollection<RoutineActivity> CompletedActivities =>
        completedActivities;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (GameTimeManager.Instance == null)
        {
            return;
        }

        CheckRoutine();
    }

    private void CheckRoutine()
    {
        int currentDay = GameTimeManager.Instance.CurrentDay;
        int currentHour = GameTimeManager.Instance.CurrentHour;
        int currentMinute = GameTimeManager.Instance.CurrentMinute;

        int currentTotalMinutes = currentHour * 60 + currentMinute;

        if (currentDay != lastProcessedDay)
        {
            StartNewDay(currentDay);
        }

        foreach (RoutineEntry entry in dailySchedule)
        {
            int entryTotalMinutes = entry.hour * 60 + entry.minute;

            if (entryTotalMinutes <= currentTotalMinutes &&
                entryTotalMinutes > lastProcessedMinute)
            {
                ProcessActivity(entry.activity);
                lastProcessedMinute = entryTotalMinutes;
            }
        }
    }

    private void StartNewDay(int day)
    {
        lastProcessedDay = day;
        lastProcessedMinute = -1;

        completedActivities.Clear();

        Debug.Log($"Daily Routine: New Day {day}");
    }

    private void ProcessActivity(RoutineActivity activity)
    {
        CurrentActivity = activity;

        completedActivities.Add(activity);

        Debug.Log($"Daily Routine: {activity} completed.");
    }

    public bool IsActivityCompleted(RoutineActivity activity)
    {
        return completedActivities.Contains(activity);
    }

    public int GetCompletedActivityCount()
    {
        return completedActivities.Count;
    }

    public int GetTotalActivityCount()
    {
        return dailySchedule.Length;
    }
}