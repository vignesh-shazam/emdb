using TMPro;
using UnityEngine;

public class GameTimeUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeText;

    private void Update()
    {
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (GameTimeManager.Instance == null)
        {
            return;
        }

        int day = GameTimeManager.Instance.CurrentDay;
        int hour = GameTimeManager.Instance.CurrentHour;
        int minute = GameTimeManager.Instance.CurrentMinute;

        string period = hour >= 12 ? "PM" : "AM";

        int displayHour = hour % 12;

        if (displayHour == 0)
        {
            displayHour = 12;
        }

        string formattedTime =
            $"{displayHour:00}:{minute:00} {period}";

        timeText.text = $"DAY {day}   {formattedTime}";
    }
}