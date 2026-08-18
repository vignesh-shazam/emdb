using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    [Header("Game Time")]
    [SerializeField] private int currentDay = 1;
    [SerializeField, Range(0, 23)] private int currentHour = 7;
    [SerializeField, Range(0, 59)] private int currentMinute = 0;

    [Header("Time Speed")]
    [SerializeField] private float gameMinutesPerRealSecond = 1f;

    public int CurrentDay => currentDay;
    public int CurrentHour => currentHour;
    public int CurrentMinute => currentMinute;

    private float minuteAccumulator;

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
        AdvanceTime();
    }

    private void AdvanceTime()
    {
        minuteAccumulator += gameMinutesPerRealSecond * Time.deltaTime;

        if (minuteAccumulator < 1f)
        {
            return;
        }

        int minutesToAdd = Mathf.FloorToInt(minuteAccumulator);

        minuteAccumulator -= minutesToAdd;

        AddMinutes(minutesToAdd);
    }

    private void AddMinutes(int minutes)
    {
        currentMinute += minutes;

        while (currentMinute >= 60)
        {
            currentMinute -= 60;
            currentHour++;

            if (currentHour >= 24)
            {
                currentHour = 0;
                currentDay++;
            }
        }
    }
}