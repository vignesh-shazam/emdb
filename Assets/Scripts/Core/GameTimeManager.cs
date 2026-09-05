using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    // =========================
    // GAME CALENDAR
    // =========================

    [Header("Game Calendar")]
    [SerializeField]
    private int currentMonth = 1;

    [SerializeField]
    private int currentDay = 1;

    [Header("Game Time")]
    [SerializeField, Range(0, 23)]
    private int currentHour = 7;

    [SerializeField, Range(0, 59)]
    private int currentMinute = 0;

    // =========================
    // TIME SPEED
    // =========================

    [Header("Time Speed")]
    [SerializeField, Range(0.1f, 10f)]
    private float gameMinutesPerRealSecond = 1f;

    private float minuteAccumulator;

    // =========================
    // PUBLIC PROPERTIES
    // =========================

    public int CurrentMonth =>
        currentMonth;

    public int CurrentDay =>
        currentDay;

    public int CurrentHour =>
        currentHour;

    public int CurrentMinute =>
        currentMinute;

    public int DaysInCurrentMonth =>
        GetDaysInMonth(currentMonth);

    public bool IsLastDayOfMonth =>
        currentDay >= DaysInCurrentMonth;

    public bool IsFirstDayOfMonth =>
        currentDay == 1;

    public bool IsPaused { get; private set; }

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

        ValidateCalendar();
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (IsPaused)
        {
            return;
        }

        AdvanceTime();
    }

    // =========================
    // ADVANCE TIME
    // =========================

    private void AdvanceTime()
    {
        minuteAccumulator +=
            gameMinutesPerRealSecond *
            Time.deltaTime;

        if (minuteAccumulator < 1f)
        {
            return;
        }

        int minutesToAdd =
            Mathf.FloorToInt(
                minuteAccumulator
            );

        minuteAccumulator -=
            minutesToAdd;

        AddMinutes(
            minutesToAdd
        );
    }

    // =========================
    // ADD MINUTES
    // =========================

    private void AddMinutes(
        int minutes)
    {
        if (minutes <= 0)
        {
            return;
        }

        currentMinute += minutes;

        while (currentMinute >= 60)
        {
            currentMinute -= 60;

            currentHour++;

            if (currentHour >= 24)
            {
                currentHour = 0;

                AdvanceDay();
            }
        }
    }

    // =========================
    // ADVANCE DAY
    // =========================

    private void AdvanceDay()
    {
        currentDay++;

        if (currentDay >
            DaysInCurrentMonth)
        {
            currentDay = 1;

            AdvanceMonth();
        }
    }

    // =========================
    // ADVANCE MONTH
    // =========================

    private void AdvanceMonth()
    {
        currentMonth++;

        if (currentMonth > 12)
        {
            currentMonth = 1;
        }

        Debug.Log(
            $"Calendar: New month started | " +
            $"Month: {currentMonth} | " +
            $"Days: {DaysInCurrentMonth}"
        );
    }

    // =========================
    // DAYS IN MONTH
    // =========================

    public int GetDaysInMonth(
        int month)
    {
        switch (month)
        {
            case 1:
                return 31;

            case 2:
                return 28;

            case 3:
                return 31;

            case 4:
                return 30;

            case 5:
                return 31;

            case 6:
                return 30;

            case 7:
                return 31;

            case 8:
                return 31;

            case 9:
                return 30;

            case 10:
                return 31;

            case 11:
                return 30;

            case 12:
                return 31;

            default:
                return 30;
        }
    }

    // =========================
    // CALENDAR VALIDATION
    // =========================

    private void ValidateCalendar()
    {
        if (currentMonth < 1)
        {
            currentMonth = 1;
        }

        if (currentMonth > 12)
        {
            currentMonth = 12;
        }

        if (currentDay < 1)
        {
            currentDay = 1;
        }

        int daysInMonth =
            GetDaysInMonth(
                currentMonth
            );

        if (currentDay >
            daysInMonth)
        {
            currentDay =
                daysInMonth;
        }
    }

    // =========================
    // PAUSE
    // =========================

    [ContextMenu("Pause Time")]
    public void PauseTime()
    {
        IsPaused = true;
    }

    // =========================
    // RESUME
    // =========================

    [ContextMenu("Resume Time")]
    public void ResumeTime()
    {
        IsPaused = false;
    }
}