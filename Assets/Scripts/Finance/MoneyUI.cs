using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (moneyText == null)
        {
            return;
        }

        if (MoneyManager.Instance == null)
        {
            moneyText.text = "Money: Rs. 0";
            return;
        }

        moneyText.text =
            $"Money: Rs. {MoneyManager.Instance.CurrentMoney:N0}";
    }
}