using TMPro;
using UnityEngine;

public class AttendanceCardUI : MonoBehaviour
{
    [Header("Employee Information")]
    [SerializeField] private TMP_Text employeeIdText;
    [SerializeField] private TMP_Text employeeNameText;
    [SerializeField] private TMP_Text employeeRoleText;
    [SerializeField] private TMP_Text attendanceStatusText;

    [Header("Attendance Information")]
    [SerializeField] private TMP_Text checkInTimeText;
    [SerializeField] private TMP_Text checkOutTimeText;

    public void SetAttendance(
        EmployeeAttendance attendance)
    {
        if (attendance == null)
        {
            Debug.LogWarning(
                "AttendanceCardUI: Attendance data is null."
            );

            return;
        }

        if (employeeIdText != null)
        {
            employeeIdText.text =
                attendance.EmployeeId;
        }

        if (employeeNameText != null)
        {
            employeeNameText.text =
                attendance.EmployeeName;
        }

        if (employeeRoleText != null)
        {
            employeeRoleText.text =
                GetEmployeeRole(
                    attendance.EmployeeId
                );
        }

        if (attendanceStatusText != null)
        {
            attendanceStatusText.text =
                attendance.IsPresent
                    ? "Present"
                    : "Absent";
        }

        if (checkInTimeText != null)
        {
            checkInTimeText.text =
                string.IsNullOrWhiteSpace(
                    attendance.CheckInTime)
                    ? "--"
                    : attendance.CheckInTime;
        }

        if (checkOutTimeText != null)
        {
            checkOutTimeText.text =
                string.IsNullOrWhiteSpace(
                    attendance.CheckOutTime)
                    ? "--"
                    : attendance.CheckOutTime;
        }
    }

    private string GetEmployeeRole(
        string employeeId)
    {
        if (EmployeeManager.Instance == null)
        {
            return "--";
        }

        Employee employee =
            EmployeeManager.Instance.FindEmployee(
                employeeId
            );

        if (employee == null)
        {
            return "--";
        }

        return employee.Role.ToString();
    }
}