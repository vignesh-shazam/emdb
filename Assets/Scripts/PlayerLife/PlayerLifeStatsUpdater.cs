using UnityEngine;

public class PlayerLifeStatsUpdater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLifeManager playerLifeManager;
    [SerializeField] private GameTimeManager gameTimeManager;

    [Header("Daily Drain")]
    [SerializeField] private float hungerLossPerGameHour = 5f;
    [SerializeField] private float energyLossPerGameHour = 3f;

    private int lastProcessedDay = -1;
    private int lastProcessedHour = -1;

    private void Start()
    {
        // Automatically find PlayerLifeManager on this Player.
        if (playerLifeManager == null)
        {
            playerLifeManager =
                GetComponent<PlayerLifeManager>();
        }

        // Automatically find GameTimeManager.
        if (gameTimeManager == null)
        {
            gameTimeManager =
                GameTimeManager.Instance;
        }

        if (playerLifeManager == null)
        {
            Debug.LogError(
                "PlayerLifeStatsUpdater: PlayerLifeManager not found."
            );

            return;
        }

        if (gameTimeManager == null)
        {
            Debug.LogError(
                "PlayerLifeStatsUpdater: GameTimeManager not found."
            );

            return;
        }

        lastProcessedDay = gameTimeManager.CurrentDay;
        lastProcessedHour = gameTimeManager.CurrentHour;
    }

    private void Update()
    {
        if (playerLifeManager == null ||
            gameTimeManager == null ||
            gameTimeManager.IsPaused)
        {
            return;
        }

        ProcessGameTime();
    }

    private void ProcessGameTime()
    {
        int currentDay = gameTimeManager.CurrentDay;
        int currentHour = gameTimeManager.CurrentHour;

        if (currentDay == lastProcessedDay &&
            currentHour == lastProcessedHour)
        {
            return;
        }

        int hoursPassed = CalculateHoursPassed(
            lastProcessedDay,
            lastProcessedHour,
            currentDay,
            currentHour
        );

        if (hoursPassed <= 0)
        {
            return;
        }

        for (int i = 0; i < hoursPassed; i++)
        {
            ApplyHourlyEffects();
        }

        lastProcessedDay = currentDay;
        lastProcessedHour = currentHour;
    }

    private int CalculateHoursPassed(
        int previousDay,
        int previousHour,
        int currentDay,
        int currentHour)
    {
        if (previousDay < 0 || previousHour < 0)
        {
            return 0;
        }

        int previousTotalHours =
            (previousDay * 24) + previousHour;

        int currentTotalHours =
            (currentDay * 24) + currentHour;

        return Mathf.Max(
            0,
            currentTotalHours - previousTotalHours
        );
    }

    private void ApplyHourlyEffects()
    {
        playerLifeManager.ReduceHunger(
            hungerLossPerGameHour
        );

        playerLifeManager.ReduceEnergy(
            energyLossPerGameHour
        );
    }
}