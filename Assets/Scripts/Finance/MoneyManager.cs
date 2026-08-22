using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Money")]
    [SerializeField] private int startingMoney = 10000;
    [SerializeField] private int currentMoney;

    public int CurrentMoney => currentMoney;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentMoney = Mathf.Max(0, startingMoney);
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentMoney += amount;
    }

    public bool CanAfford(int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        return currentMoney >= amount;
    }

    public bool RemoveMoney(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (!CanAfford(amount))
        {
            return false;
        }

        currentMoney -= amount;

        return true;
    }
}