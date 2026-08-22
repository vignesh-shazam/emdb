using System;

[Serializable]
public class CareerData
{
    public JobType JobType;
    public int Salary;
    public bool IsEmployed;

    public CareerData(
        JobType jobType,
        int salary,
        bool isEmployed)
    {
        JobType = jobType;
        Salary = salary;
        IsEmployed = isEmployed;
    }
}