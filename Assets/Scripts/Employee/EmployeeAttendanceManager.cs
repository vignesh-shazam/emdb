using System;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeAttendanceManager : MonoBehaviour
{
    public static EmployeeAttendanceManager Instance { get; private set; }

    [Header("Attendance Records")]
    [SerializeField]
    private List<EmployeeAttendance> attendanceRecords =
        new List<EmployeeAttendance>();

    [Header("Attendance UI")]
    [SerializeField] private Transform attendanceList;
    [SerializeField] private AttendanceCardUI attendanceCardPrefab;

    public IReadOnlyList<EmployeeAttendance> AttendanceRecords =>
        attendanceRecords;

    public int AttendanceRecordCount =>
        attendanceRecords.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "Duplicate EmployeeAttendanceManager found. " +
                "Destroying duplicate."
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log(
            "EmployeeAttendanceManager initialized successfully."
        );
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool CreateAttendanceRecord(
        Employee employee)
    {
        if (employee == null)
        {
            Debug.LogWarning(
                "Cannot create attendance record. " +
                "Employee is null."
            );

            return false;
        }

        string today =
            DateTime.Now.ToString("dd-MM-yyyy");

        if (HasAttendanceRecord(
            employee.EmployeeId,
            today))
        {
            return false;
        }

        EmployeeAttendance attendance =
            new EmployeeAttendance(
                employee.EmployeeId,
                employee.EmployeeName,
                today
            );

        attendanceRecords.Add(
            attendance
        );

        Debug.Log(
            $"Attendance record created: " +
            $"{employee.EmployeeName} | " +
            $"ID: {employee.EmployeeId} | " +
            $"Date: {today}"
        );

        return true;
    }

    public EmployeeAttendance GetAttendanceRecord(
        string employeeId,
        string date)
    {
        if (string.IsNullOrWhiteSpace(employeeId) ||
            string.IsNullOrWhiteSpace(date))
        {
            return null;
        }

        return attendanceRecords.Find(
            attendance =>
                attendance != null &&
                attendance.EmployeeId == employeeId &&
                attendance.AttendanceDate == date
        );
    }

    public bool HasAttendanceRecord(
        string employeeId,
        string date)
    {
        return GetAttendanceRecord(
            employeeId,
            date
        ) != null;
    }

    public bool MarkEmployeePresent(
        string employeeId)
    {
        Employee employee =
            GetEmployee(employeeId);

        if (employee == null)
        {
            return false;
        }

        if (!employee.IsHired)
        {
            Debug.LogWarning(
                $"Cannot mark fired employee present: " +
                $"{employee.EmployeeName}"
            );

            return false;
        }

        string today =
            DateTime.Now.ToString("dd-MM-yyyy");

        EmployeeAttendance attendance =
            GetAttendanceRecord(
                employeeId,
                today
            );

        if (attendance == null)
        {
            CreateAttendanceRecord(employee);

            attendance =
                GetAttendanceRecord(
                    employeeId,
                    today
                );
        }

        if (attendance == null)
        {
            return false;
        }

        if (attendance.IsPresent)
        {
            Debug.LogWarning(
                $"Employee is already present: " +
                $"{employee.EmployeeName}"
            );

            return false;
        }

        attendance.MarkPresent();

        string checkInTime =
            DateTime.Now.ToString("hh:mm tt");

        attendance.SetCheckInTime(
            checkInTime
        );

        Debug.Log(
            $"Employee checked in: " +
            $"{employee.EmployeeName} | " +
            $"Time: {checkInTime}"
        );

        RefreshAttendanceUI();

        return true;
    }

    public bool MarkEmployeeAbsent(
        string employeeId)
    {
        Employee employee =
            GetEmployee(employeeId);

        if (employee == null)
        {
            return false;
        }

        string today =
            DateTime.Now.ToString("dd-MM-yyyy");

        EmployeeAttendance attendance =
            GetAttendanceRecord(
                employeeId,
                today
            );

        if (attendance == null)
        {
            CreateAttendanceRecord(employee);

            attendance =
                GetAttendanceRecord(
                    employeeId,
                    today
                );
        }

        if (attendance == null)
        {
            return false;
        }

        attendance.MarkAbsent();

        Debug.Log(
            $"Employee marked absent: " +
            $"{employee.EmployeeName}"
        );

        RefreshAttendanceUI();

        return true;
    }

    public bool CheckOutEmployee(
        string employeeId)
    {
        Employee employee =
            GetEmployee(employeeId);

        if (employee == null)
        {
            return false;
        }

        string today =
            DateTime.Now.ToString("dd-MM-yyyy");

        EmployeeAttendance attendance =
            GetAttendanceRecord(
                employeeId,
                today
            );

        if (attendance == null)
        {
            Debug.LogWarning(
                $"No attendance record found for: " +
                $"{employee.EmployeeName}"
            );

            return false;
        }

        if (!attendance.IsPresent)
        {
            Debug.LogWarning(
                $"Employee is not marked present: " +
                $"{employee.EmployeeName}"
            );

            return false;
        }

        if (!string.IsNullOrWhiteSpace(
            attendance.CheckOutTime))
        {
            Debug.LogWarning(
                $"Employee already checked out: " +
                $"{employee.EmployeeName}"
            );

            return false;
        }

        string checkOutTime =
            DateTime.Now.ToString("hh:mm tt");

        attendance.SetCheckOutTime(
            checkOutTime
        );

        Debug.Log(
            $"Employee checked out: " +
            $"{employee.EmployeeName} | " +
            $"Time: {checkOutTime}"
        );

        RefreshAttendanceUI();

        return true;
    }

    public int GetPresentDays(
        string employeeId)
    {
        int presentDays = 0;

        foreach (
            EmployeeAttendance attendance
            in attendanceRecords)
        {
            if (attendance == null)
            {
                continue;
            }

            if (attendance.EmployeeId != employeeId)
            {
                continue;
            }

            if (attendance.IsPresent)
            {
                presentDays++;
            }
        }

        return presentDays;
    }

    public int GetAbsentDays(
        string employeeId)
    {
        int absentDays = 0;

        foreach (
            EmployeeAttendance attendance
            in attendanceRecords)
        {
            if (attendance == null)
            {
                continue;
            }

            if (attendance.EmployeeId != employeeId)
            {
                continue;
            }

            if (!attendance.IsPresent)
            {
                absentDays++;
            }
        }

        return absentDays;
    }

    private Employee GetEmployee(
        string employeeId)
    {
        if (EmployeeManager.Instance == null)
        {
            Debug.LogWarning(
                "EmployeeManager instance not found."
            );

            return null;
        }

        return EmployeeManager.Instance.FindEmployee(
            employeeId
        );
    }

    public void RefreshAttendanceUI()
    {
        if (attendanceList == null)
        {
            Debug.LogWarning(
                "EmployeeAttendanceManager: " +
                "Attendance List reference is not assigned."
            );

            return;
        }

        if (attendanceCardPrefab == null)
        {
            Debug.LogWarning(
                "EmployeeAttendanceManager: " +
                "Attendance Card Prefab reference is not assigned."
            );

            return;
        }

        ClearAttendanceCards();

        string today =
            DateTime.Now.ToString("dd-MM-yyyy");

        if (EmployeeManager.Instance == null)
        {
            return;
        }

        foreach (
            Employee employee
            in EmployeeManager.Instance.Employees)
        {
            if (employee == null)
            {
                continue;
            }

            EmployeeAttendance attendance =
                GetAttendanceRecord(
                    employee.EmployeeId,
                    today
                );

            if (attendance == null)
            {
                CreateAttendanceRecord(employee);

                attendance =
                    GetAttendanceRecord(
                        employee.EmployeeId,
                        today
                    );
            }

            if (attendance == null)
            {
                continue;
            }

            AttendanceCardUI card =
                Instantiate(
                    attendanceCardPrefab,
                    attendanceList
                );

            card.SetAttendance(
                attendance
            );
        }

        Debug.Log(
            "Attendance UI refreshed."
        );
    }

    private void ClearAttendanceCards()
    {
        if (attendanceList == null)
        {
            return;
        }

        for (
            int i = attendanceList.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                attendanceList.GetChild(i).gameObject
            );
        }
    }

    public void DisplayAllAttendance()
    {
        if (attendanceRecords.Count == 0)
        {
            Debug.Log(
                "No attendance records available."
            );

            return;
        }

        Debug.Log(
            $"Total Attendance Records: " +
            $"{attendanceRecords.Count}"
        );

        foreach (
            EmployeeAttendance attendance
            in attendanceRecords)
        {
            if (attendance == null)
            {
                continue;
            }

            Debug.Log(
                $"Employee: {attendance.EmployeeName} | " +
                $"ID: {attendance.EmployeeId} | " +
                $"Date: {attendance.AttendanceDate} | " +
                $"Status: " +
                $"{(attendance.IsPresent ? "Present" : "Absent")} | " +
                $"Check-In: {attendance.CheckInTime} | " +
                $"Check-Out: {attendance.CheckOutTime}"
            );
        }
    }

    [ContextMenu("TEST - Refresh Attendance UI")]
    private void TestRefreshAttendanceUI()
    {
        RefreshAttendanceUI();
    }

    [ContextMenu("TEST - Display Attendance")]
    private void TestDisplayAttendance()
    {
        DisplayAllAttendance();
    }

    [ContextMenu("TEST - Check In EMP001")]
    private void TestCheckInEmployee()
    {
        MarkEmployeePresent("EMP001");
    }

    [ContextMenu("TEST - Check Out EMP001")]
    private void TestCheckOutEmployee()
    {
        CheckOutEmployee("EMP001");
    }

    [ContextMenu("TEST - Mark EMP001 Absent")]
    private void TestMarkEmployeeAbsent()
    {
        MarkEmployeeAbsent("EMP001");
    }
}