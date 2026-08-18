using TMPro;
using UnityEngine;

public class DailyRoutineUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI routineText;

    private void Update()
    {
        UpdateRoutineDisplay();
    }

    private void UpdateRoutineDisplay()
    {
        if (DailyRoutineManager.Instance == null || routineText == null)
        {
            return;
        }

        routineText.text = BuildRoutineText();
    }

    private string BuildRoutineText()
    {
        DailyRoutineManager manager = DailyRoutineManager.Instance;

        string text = "DAILY ROUTINE\n\n";

        AddRoutineLine(
            ref text,
            manager,
            "Wake Up",
            DailyRoutineManager.RoutineActivity.WakeUp);

        AddRoutineLine(
            ref text,
            manager,
            "Exercise",
            DailyRoutineManager.RoutineActivity.Exercise);

        AddRoutineLine(
            ref text,
            manager,
            "Fresh Up",
            DailyRoutineManager.RoutineActivity.FreshUp);

        AddRoutineLine(
            ref text,
            manager,
            "Breakfast",
            DailyRoutineManager.RoutineActivity.Breakfast);

        AddRoutineLine(
            ref text,
            manager,
            "Take Cycle",
            DailyRoutineManager.RoutineActivity.TakeCycle);

        AddRoutineLine(
            ref text,
            manager,
            "Ride to Shop",
            DailyRoutineManager.RoutineActivity.RideToShop);

        AddRoutineLine(
            ref text,
            manager,
            "Park Cycle",
            DailyRoutineManager.RoutineActivity.ParkCycle);

        AddRoutineLine(
            ref text,
            manager,
            "Open Shop",
            DailyRoutineManager.RoutineActivity.OpenShop);

        return text;
    }

    private void AddRoutineLine(
        ref string text,
        DailyRoutineManager manager,
        string displayName,
        DailyRoutineManager.RoutineActivity activity)
    {
        if (manager.IsActivityCompleted(activity))
        {
            text += $"<color=#70D470>[DONE] {displayName}</color>\n";
        }
        else if (manager.CurrentActivity == activity)
        {
            text += $"<color=#FFD54F>[NOW] {displayName}</color>\n";
        }
        else
        {
            text += $"<color=#AAAAAA>[ ] {displayName}</color>\n";
        }
    }
}