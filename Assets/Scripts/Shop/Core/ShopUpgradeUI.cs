using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgradeUI : MonoBehaviour
{
    [Header("Shop Level")]
    [SerializeField]
    private TMP_Text shopLevelText;

    [SerializeField]
    private TMP_Text nextLevelText;

    [Header("Upgrade")]
    [SerializeField]
    private TMP_Text upgradeCostText;

    [SerializeField]
    private Button upgradeButton;

    [Header("Benefits")]
    [SerializeField]
    private TMP_Text revenueMultiplierText;

    [SerializeField]
    private TMP_Text customerGrowthText;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        ShopManager.OnShopUpdated += RefreshUI;
        ShopManager.OnShopUpgrade += RefreshUI;

        RefreshUI();
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        ShopManager.OnShopUpdated -= RefreshUI;
        ShopManager.OnShopUpgrade -= RefreshUI;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        InitializeUI();
        RefreshUI();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeUI()
    {
        if (upgradeButton == null)
        {
            Debug.LogWarning(
                "ShopUpgradeUI: " +
                "Upgrade button is not assigned."
            );

            return;
        }

        upgradeButton.onClick.RemoveAllListeners();

        upgradeButton.onClick.AddListener(
            UpgradeShop
        );
    }

    // =========================
    // REFRESH UI
    // =========================

    public void RefreshUI()
    {
        if (ShopManager.Instance == null)
        {
            SetDefaultUI();
            return;
        }

        int currentLevel =
            ShopManager.Instance.ShopUpgradeLevel;

        int maximumLevel =
            ShopManager.Instance.MaximumUpgradeLevel;

        bool canUpgrade =
            ShopManager.Instance.CanUpgradeShop;

        int upgradeCost =
            ShopManager.Instance.GetUpgradeCost();

        float revenueMultiplier =
            ShopManager.Instance.GetRevenueMultiplier();

        float customerGrowthMultiplier =
            ShopManager.Instance.GetCustomerGrowthMultiplier();

        // =========================
        // SHOP LEVEL
        // =========================

        if (shopLevelText != null)
        {
            shopLevelText.text =
                $"Shop Level: {currentLevel}";
        }

        // =========================
        // NEXT LEVEL
        // =========================

        if (nextLevelText != null)
        {
            if (canUpgrade)
            {
                nextLevelText.text =
                    $"Next Level: {currentLevel + 1}";
            }
            else
            {
                nextLevelText.text =
                    "Maximum Level";
            }
        }

        // =========================
        // UPGRADE COST
        // =========================

        if (upgradeCostText != null)
        {
            if (canUpgrade)
            {
                upgradeCostText.text =
                    $"Upgrade Cost: Rs. {upgradeCost:N0}";
            }
            else
            {
                upgradeCostText.text =
                    "Upgrade Complete";
            }
        }

        // =========================
        // REVENUE
        // =========================

        if (revenueMultiplierText != null)
        {
            revenueMultiplierText.text =
                $"Revenue: {revenueMultiplier:F2}x";
        }

        // =========================
        // CUSTOMER GROWTH
        // =========================

        if (customerGrowthText != null)
        {
            float customerGrowthPercent =
                (customerGrowthMultiplier - 1f) * 100f;

            customerGrowthText.text =
                $"Customer: +{customerGrowthPercent:F0}%";
        }

        // =========================
        // BUTTON
        // =========================

        if (upgradeButton != null)
        {
            upgradeButton.interactable =
                canUpgrade &&
                CanAffordUpgrade(upgradeCost);
        }
    }

    // =========================
    // DEFAULT UI
    // =========================

    private void SetDefaultUI()
    {
        if (shopLevelText != null)
        {
            shopLevelText.text =
                "Shop Level: 1";
        }

        if (nextLevelText != null)
        {
            nextLevelText.text =
                "Next Level: 2";
        }

        if (upgradeCostText != null)
        {
            upgradeCostText.text =
                "Upgrade Cost: Rs. 1,000";
        }

        if (revenueMultiplierText != null)
        {
            revenueMultiplierText.text =
                "Revenue: 1.00x";
        }

        if (customerGrowthText != null)
        {
            customerGrowthText.text =
            "Customer: +0%";
        }

        if (upgradeButton != null)
        {
            upgradeButton.interactable = false;
        }
    }

    // =========================
    // CHECK MONEY
    // =========================

    private bool CanAffordUpgrade(
        int upgradeCost)
    {
        if (MoneyManager.Instance == null)
        {
            return false;
        }

        return MoneyManager.Instance.CanAfford(
            upgradeCost
        );
    }

    // =========================
    // UPGRADE SHOP
    // =========================

    private void UpgradeShop()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUpgradeUI: " +
                "ShopManager not found."
            );

            return;
        }

        bool success =
            ShopManager.Instance.UpgradeShop();

        if (!success)
        {
            RefreshUI();
            return;
        }

        RefreshUI();

        Debug.Log(
            "ShopUpgradeUI: " +
            "Shop upgraded successfully."
        );
    }
}