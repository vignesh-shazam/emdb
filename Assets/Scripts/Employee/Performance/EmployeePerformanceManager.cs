using System.Collections.Generic;
using UnityEngine;

public class EmployeePerformanceManager : MonoBehaviour
{
    public static EmployeePerformanceManager Instance { get; private set; }

    [Header("Performance Records")]
    [SerializeField]
    private List<EmployeePerformance> performanceRecords =
        new List<EmployeePerformance>();

    [Header("Performance UI")]
    [SerializeField] private Transform performanceList;
    [SerializeField] private GameObject performanceCardPrefab;

    public int PerformanceRecordCount =>
        performanceRecords.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ==================================================
    // CREATE PERFORMANCE RECORD
    // ==================================================

    public EmployeePerformance CreatePerformanceRecord(
        Employee employee)
    {
        if (employee == null)
        {
            Debug.LogWarning(
                "EmployeePerformanceManager: Employee is null."
            );

            return null;
        }

        if (HasPerformanceRecord(employee.EmployeeId))
        {
            Debug.LogWarning(
                $"Performance record already exists for {employee.EmployeeId}."
            );

            return GetPerformanceRecord(employee.EmployeeId);
        }

        EmployeePerformance performance =
            new EmployeePerformance(
                employee.EmployeeId,
                employee.EmployeeName
            );

        performanceRecords.Add(performance);

        Debug.Log(
            $"Performance record created for " +
            $"{employee.EmployeeId} - " +
            $"{employee.EmployeeName}"
        );

        RefreshPerformanceUI();

        return performance;
    }

    // ==================================================
    // GET PERFORMANCE RECORD
    // ==================================================

    public EmployeePerformance GetPerformanceRecord(
        string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            return null;
        }

        foreach (EmployeePerformance performance
                 in performanceRecords)
        {
            if (performance.EmployeeId == employeeId)
            {
                return performance;
            }
        }

        return null;
    }

    // ==================================================
    // CHECK RECORD
    // ==================================================

    public bool HasPerformanceRecord(
        string employeeId)
    {
        return GetPerformanceRecord(employeeId) != null;
    }

    // ==================================================
    // TASK MANAGEMENT
    // ==================================================

    public void AddTaskAssigned(
        string employeeId)
    {
        EmployeePerformance performance =
            GetPerformanceRecord(employeeId);

        if (performance == null)
        {
            Debug.LogWarning(
                $"No performance record found for {employeeId}."
            );

            return;
        }

        performance.AddTaskAssigned();

        RefreshPerformanceUI();

        Debug.Log(
            $"Task assigned to {employeeId}."
        );
    }

    public void AddTaskCompleted(
        string employeeId)
    {
        EmployeePerformance performance =
            GetPerformanceRecord(employeeId);

        if (performance == null)
        {
            Debug.LogWarning(
                $"No performance record found for {employeeId}."
            );

            return;
        }

        performance.AddTaskCompleted();

        RefreshPerformanceUI();

        Debug.Log(
            $"Task completed by {employeeId}."
        );
    }

    // ==================================================
    // WORKED DAY
    // ==================================================

    public void AddWorkedDay(
        string employeeId)
    {
        EmployeePerformance performance =
            GetPerformanceRecord(employeeId);

        if (performance == null)
        {
            Debug.LogWarning(
                $"No performance record found for {employeeId}."
            );

            return;
        }

        performance.AddWorkedDay();

        RefreshPerformanceUI();

        Debug.Log(
            $"Worked day added for {employeeId}."
        );
    }

    // ==================================================
    // PERFORMANCE SCORE
    // ==================================================

    public void SetPerformanceScore(
        string employeeId,
        float score)
    {
        EmployeePerformance performance =
            GetPerformanceRecord(employeeId);

        if (performance == null)
        {
            Debug.LogWarning(
                $"No performance record found for {employeeId}."
            );

            return;
        }

        performance.SetPerformanceScore(score);

        RefreshPerformanceUI();

        Debug.Log(
            $"Performance score for {employeeId}: {score}"
        );
    }

    // ==================================================
    // PRODUCTIVITY SCORE
    // ==================================================

    public void SetProductivityScore(
        string employeeId,
        float score)
    {
        EmployeePerformance performance =
            GetPerformanceRecord(employeeId);

        if (performance == null)
        {
            Debug.LogWarning(
                $"No performance record found for {employeeId}."
            );

            return;
        }

        performance.SetProductivityScore(score);

        RefreshPerformanceUI();

        Debug.Log(
            $"Productivity score for {employeeId}: {score}"
        );
    }

    // ==================================================
    // REFRESH PERFORMANCE UI
    // ==================================================

    public void RefreshPerformanceUI()
    {
        if (performanceList == null ||
            performanceCardPrefab == null)
        {
            return;
        }

        // Remove existing cards
        for (int i = performanceList.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                performanceList.GetChild(i).gameObject
            );
        }

        // Create performance cards
        foreach (EmployeePerformance performance
                 in performanceRecords)
        {
            if (performance == null)
            {
                continue;
            }

            GameObject card =
                Instantiate(
                    performanceCardPrefab,
                    performanceList
                );

            PerformanceCardUI cardUI =
                card.GetComponent<PerformanceCardUI>();

            if (cardUI != null)
            {
                cardUI.SetPerformance(performance);
            }
        }
    }

    // ==================================================
    // DISPLAY ALL PERFORMANCE
    // ==================================================

    [ContextMenu("Display All Performance")]
    public void DisplayAllPerformance()
    {
        if (performanceRecords.Count == 0)
        {
            Debug.Log(
                "EmployeePerformanceManager: " +
                "No performance records found."
            );

            return;
        }

        Debug.Log(
            $"========== EMPLOYEE PERFORMANCE " +
            $"({performanceRecords.Count}) =========="
        );

        foreach (EmployeePerformance performance
                 in performanceRecords)
        {
            Debug.Log(
                $"ID: {performance.EmployeeId} | " +
                $"Name: {performance.EmployeeName} | " +
                $"Performance: {performance.PerformanceScore:F1}% | " +
                $"Productivity: {performance.ProductivityScore:F1}% | " +
                $"Tasks: {performance.TasksCompleted}/" +
                $"{performance.TasksAssigned} | " +
                $"Days Worked: {performance.DaysWorked} | " +
                $"Rating: {performance.Rating}"
            );
        }

        Debug.Log(
            "================================================"
        );
    }

    // ==================================================
    // TEST - CREATE EMP001 PERFORMANCE
    // ==================================================

    [ContextMenu("Test Create EMP001 Performance")]
    private void TestCreateEMP001Performance()
    {
        if (EmployeeManager.Instance == null)
        {
            Debug.LogWarning(
                "EmployeePerformanceManager: " +
                "EmployeeManager is not available."
            );

            return;
        }

        Employee employee =
            EmployeeManager.Instance
                .FindEmployee("EMP001");

        if (employee == null)
        {
            Debug.LogWarning(
                "Employee EMP001 was not found."
            );

            return;
        }

        CreatePerformanceRecord(employee);
    }

    // ==================================================
    // TEST - ASSIGN TASK
    // ==================================================

    [ContextMenu("Test Assign Task EMP001")]
    private void TestAssignTaskEMP001()
    {
        AddTaskAssigned("EMP001");
    }

    // ==================================================
    // TEST - COMPLETE TASK
    // ==================================================

    [ContextMenu("Test Complete Task EMP001")]
    private void TestCompleteTaskEMP001()
    {
        AddTaskCompleted("EMP001");
    }

    // ==================================================
    // TEST - ADD WORKED DAY
    // ==================================================

    [ContextMenu("Test Add Worked Day EMP001")]
    private void TestAddWorkedDayEMP001()
    {
        AddWorkedDay("EMP001");
    }

    // ==================================================
    // TEST - SET PRODUCTIVITY
    // ==================================================

    [ContextMenu("Test Set Productivity 80 EMP001")]
    private void TestSetProductivityEMP001()
    {
        SetProductivityScore(
            "EMP001",
            80f
        );
    }

    // ==================================================
    // TEST - REFRESH UI
    // ==================================================

    [ContextMenu("Refresh Performance UI")]
    private void TestRefreshPerformanceUI()
    {
        RefreshPerformanceUI();
    }

    // ==================================================
    // TEST - DISPLAY EMP001
    // ==================================================

    [ContextMenu("Test Display EMP001 Performance")]
    private void TestDisplayEMP001Performance()
    {
        EmployeePerformance performance =
            GetPerformanceRecord("EMP001");

        if (performance == null)
        {
            Debug.LogWarning(
                "No performance record found for EMP001."
            );

            return;
        }

        Debug.Log(
            $"EMP001 | " +
            $"Performance: {performance.PerformanceScore:F1}% | " +
            $"Productivity: {performance.ProductivityScore:F1}% | " +
            $"Rating: {performance.Rating}"
        );
    }
}