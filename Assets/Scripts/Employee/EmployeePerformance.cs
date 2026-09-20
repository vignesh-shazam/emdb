using System;
using UnityEngine;

[Serializable]
public class EmployeePerformance
{
    [Header("Employee")]
    [SerializeField] private string employeeId;
    [SerializeField] private string employeeName;

    [Header("Performance")]
    [SerializeField] private float performanceScore = 100f;
    [SerializeField] private float productivityScore = 100f;

    [Header("Performance Statistics")]
    [SerializeField] private int tasksCompleted;
    [SerializeField] private int tasksAssigned;
    [SerializeField] private int daysWorked;

    [Header("Performance Status")]
    [SerializeField] private EmployeePerformanceRating rating;

    public string EmployeeId =>
        employeeId;

    public string EmployeeName =>
        employeeName;

    public float PerformanceScore =>
        performanceScore;

    public float ProductivityScore =>
        productivityScore;

    public int TasksCompleted =>
        tasksCompleted;

    public int TasksAssigned =>
        tasksAssigned;

    public int DaysWorked =>
        daysWorked;

    public EmployeePerformanceRating Rating =>
        rating;

    public EmployeePerformance(
        string id,
        string name)
    {
        employeeId = id;
        employeeName = name;

        performanceScore = 100f;
        productivityScore = 100f;

        tasksCompleted = 0;
        tasksAssigned = 0;
        daysWorked = 0;

        rating =
            EmployeePerformanceRating.Excellent;
    }

    public void SetPerformanceScore(
        float score)
    {
        performanceScore =
            Mathf.Clamp(score, 0f, 100f);

        UpdateRating();
    }

    public void SetProductivityScore(
        float score)
    {
        productivityScore =
            Mathf.Clamp(score, 0f, 100f);
    }

    public void AddTaskAssigned()
    {
        tasksAssigned++;
        RecalculatePerformance();
    }

    public void AddTaskCompleted()
    {
        tasksCompleted++;
        RecalculatePerformance();
    }

    public void AddWorkedDay()
    {
        daysWorked++;
        RecalculatePerformance();
    }

    private void RecalculatePerformance()
    {
        float taskScore = 100f;

        if (tasksAssigned > 0)
        {
            taskScore =
                ((float)tasksCompleted /
                 tasksAssigned) * 100f;
        }

        performanceScore =
            Mathf.Clamp(
                taskScore,
                0f,
                100f
            );

        UpdateRating();
    }

    private void UpdateRating()
    {
        if (performanceScore >= 90f)
        {
            rating =
                EmployeePerformanceRating.Excellent;
        }
        else if (performanceScore >= 75f)
        {
            rating =
                EmployeePerformanceRating.Good;
        }
        else if (performanceScore >= 60f)
        {
            rating =
                EmployeePerformanceRating.Average;
        }
        else if (performanceScore >= 40f)
        {
            rating =
                EmployeePerformanceRating.Poor;
        }
        else
        {
            rating =
                EmployeePerformanceRating.Critical;
        }
    }

    public void ResetPerformance()
    {
        performanceScore = 100f;
        productivityScore = 100f;

        tasksCompleted = 0;
        tasksAssigned = 0;
        daysWorked = 0;

        rating =
            EmployeePerformanceRating.Excellent;
    }
}

public enum EmployeePerformanceRating
{
    Excellent,
    Good,
    Average,
    Poor,
    Critical
}