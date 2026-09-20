using System;
using UnityEngine;

[Serializable]
public class EmployeeAttendance
{
    [Header("Employee")]
    [SerializeField] private string employeeId;
    [SerializeField] private string employeeName;

    [Header("Attendance")]
    [SerializeField] private string attendanceDate;
    [SerializeField] private bool isPresent;

    [Header("Working Time")]
    [SerializeField] private string checkInTime;
    [SerializeField] private string checkOutTime;

    public string EmployeeId =>
        employeeId;

    public string EmployeeName =>
        employeeName;

    public string AttendanceDate =>
        attendanceDate;

    public bool IsPresent =>
        isPresent;

    public string CheckInTime =>
        checkInTime;

    public string CheckOutTime =>
        checkOutTime;

    public EmployeeAttendance(
        string id,
        string name,
        string date)
    {
        employeeId = id;
        employeeName = name;
        attendanceDate = date;

        isPresent = false;

        checkInTime = string.Empty;
        checkOutTime = string.Empty;
    }

    public void MarkPresent()
    {
        isPresent = true;
    }

    public void MarkAbsent()
    {
        isPresent = false;

        checkInTime = string.Empty;
        checkOutTime = string.Empty;
    }

    public void SetCheckInTime(
        string time)
    {
        checkInTime = time;
    }

    public void SetCheckOutTime(
        string time)
    {
        checkOutTime = time;
    }
}