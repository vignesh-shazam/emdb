using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Bread")]
    [SerializeField] private TMP_Text breadNameText;
    [SerializeField] private TMP_Text breadPriceText;
    [SerializeField] private TMP_Text breadQuantityText;
    [SerializeField] private Button breadMinusButton;
    [SerializeField] private Button breadPlusButton;

    [Header("Milk")]
    [SerializeField] private TMP_Text milkNameText;
    [SerializeField] private TMP_Text milkPriceText;
    [SerializeField] private TMP_Text milkQuantityText;
    [SerializeField] private Button milkMinusButton;
    [SerializeField] private Button milkPlusButton;

    [Header("Tool Kit")]
    [SerializeField] private TMP_Text toolKitNameText;
    [SerializeField] private TMP_Text toolKitPriceText;
    [SerializeField] private TMP_Text toolKitQuantityText;
    [SerializeField] private Button toolKitMinusButton;
    [SerializeField] private Button toolKitPlusButton;

    private ShopItem breadItem;
    private ShopItem milkItem;
    private ShopItem toolKitItem;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged +=
            UpdateAllUI;

        CustomerManager.OnCustomerListChanged +=
            UpdateAllUI;
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -=
            UpdateAllUI;

        CustomerManager.OnCustomerListChanged -=
            UpdateAllUI;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        InitializeUI();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeUI()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUI: ShopManager not found."
            );

            return;
        }

        breadItem =
            ShopManager.Instance.GetItem(
                "food_001"
            );

        milkItem =
            ShopManager.Instance.GetItem(
                "food_002"
            );

        toolKitItem =
            ShopManager.Instance.GetItem(
                "tool_001"
            );

        SetupItemUI(
            breadItem,
            breadNameText,
            breadPriceText,
            breadQuantityText,
            breadMinusButton,
            breadPlusButton
        );

        SetupItemUI(
            milkItem,
            milkNameText,
            milkPriceText,
            milkQuantityText,
            milkMinusButton,
            milkPlusButton
        );

        SetupItemUI(
            toolKitItem,
            toolKitNameText,
            toolKitPriceText,
            toolKitQuantityText,
            toolKitMinusButton,
            toolKitPlusButton
        );

        UpdateAllUI();
    }

    // =========================
    // SETUP ITEM
    // =========================

    private void SetupItemUI(
        ShopItem item,
        TMP_Text nameText,
        TMP_Text priceText,
        TMP_Text quantityText,
        Button minusButton,
        Button plusButton)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "ShopUI: Shop item not found."
            );

            if (minusButton != null)
            {
                minusButton.interactable = false;
            }

            if (plusButton != null)
            {
                plusButton.interactable = false;
            }

            return;
        }

        if (nameText != null)
        {
            nameText.text =
                item.ItemName;
        }

        if (priceText != null)
        {
            priceText.text =
                $"Rs. {item.BuyPrice:N0}";
        }

        // =========================
        // MINUS
        // =========================

        if (minusButton != null)
        {
            minusButton.onClick.RemoveAllListeners();

            minusButton.onClick.AddListener(
                () => SellItem(item.ItemId)
            );
        }

        // =========================
        // PLUS
        // =========================

        if (plusButton != null)
        {
            plusButton.onClick.RemoveAllListeners();

            plusButton.onClick.AddListener(
                () => BuyItem(item.ItemId)
            );
        }
    }

    // =========================
    // BUY
    // =========================

    private void BuyItem(string itemId)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUI: ShopManager not found."
            );

            return;
        }

        // =========================
        // CHECK CURRENT CUSTOMER
        // =========================

        Customer customer =
            GetCurrentCustomer();

        if (customer == null)
        {
            Debug.LogWarning(
                "Buy blocked: No active customer."
            );

            UpdateAllUI();

            return;
        }

        // =========================
        // ONLY REQUESTED ITEM
        // =========================

        if (customer.RequestedItemId != itemId)
        {
            Debug.LogWarning(
                $"Buy blocked: Customer wants " +
                $"{customer.RequestedItemName}, " +
                $"not item ID {itemId}."
            );

            UpdateAllUI();

            return;
        }

        // =========================
        // CHECK REQUIRED QUANTITY
        // =========================

        int currentQuantity =
            InventoryManager.Instance != null
                ? InventoryManager.Instance.GetQuantity(
                    itemId
                )
                : 0;

        if (currentQuantity >=
            customer.RequestedQuantity)
        {
            Debug.Log(
                $"Buy blocked: Required quantity already reached. " +
                $"Item: {customer.RequestedItemName} | " +
                $"Required: {customer.RequestedQuantity} | " +
                $"Current: {currentQuantity}"
            );

            UpdateAllUI();

            return;
        }

        // =========================
        // BUY
        // =========================

        bool success =
            ShopManager.Instance.BuyItem(
                itemId
            );

        if (!success)
        {
            UpdateAllUI();

            return;
        }

        UpdateAllUI();

        ShopItem item =
            ShopManager.Instance.GetItem(
                itemId
            );

        if (item != null)
        {
            int quantity =
                InventoryManager.Instance != null
                    ? InventoryManager.Instance.GetQuantity(
                        itemId
                    )
                    : 0;

            Debug.Log(
                $"Shop UI purchase completed | " +
                $"Item: {item.ItemName} | " +
                $"Inventory Quantity: {quantity}"
            );
        }
    }

    // =========================
    // SELL
    // =========================

    private void SellItem(string itemId)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUI: ShopManager not found."
            );

            return;
        }

        bool success =
            ShopManager.Instance.SellItem(
                itemId
            );

        if (!success)
        {
            UpdateAllUI();

            return;
        }

        UpdateAllUI();

        ShopItem item =
            ShopManager.Instance.GetItem(
                itemId
            );

        if (item != null)
        {
            int quantity =
                InventoryManager.Instance != null
                    ? InventoryManager.Instance.GetQuantity(
                        itemId
                    )
                    : 0;

            Debug.Log(
                $"Shop UI sale completed | " +
                $"Item: {item.ItemName} | " +
                $"Inventory Quantity: {quantity}"
            );
        }
    }

    // =========================
    // UPDATE ALL UI
    // =========================

    public void UpdateAllUI()
    {
        UpdateItemUI(
            "food_001",
            breadQuantityText,
            breadMinusButton,
            breadPlusButton
        );

        UpdateItemUI(
            "food_002",
            milkQuantityText,
            milkMinusButton,
            milkPlusButton
        );

        UpdateItemUI(
            "tool_001",
            toolKitQuantityText,
            toolKitMinusButton,
            toolKitPlusButton
        );
    }

    // =========================
    // UPDATE ITEM UI
    // =========================

    private void UpdateItemUI(
        string itemId,
        TMP_Text quantityText,
        Button minusButton,
        Button plusButton)
    {
        int quantity = 0;

        if (InventoryManager.Instance != null)
        {
            quantity =
                InventoryManager.Instance.GetQuantity(
                    itemId
                );
        }

        // =========================
        // QUANTITY
        // =========================

        if (quantityText != null)
        {
            quantityText.text =
                $"Qty: {quantity}";
        }

        // =========================
        // MINUS
        // =========================

        if (minusButton != null)
        {
            minusButton.interactable =
                quantity > 0;
        }

        // =========================
        // PLUS
        // =========================

        if (plusButton != null)
        {
            plusButton.interactable =
                CanBuyForCurrentCustomer(
                    itemId,
                    quantity
                );
        }
    }

    // =========================
    // CAN BUY
    // =========================

    private bool CanBuyForCurrentCustomer(
        string itemId,
        int currentQuantity)
    {
        Customer customer =
            GetCurrentCustomer();

        // No customer = no buying
        if (customer == null)
        {
            return false;
        }

        // Only requested item
        if (customer.RequestedItemId != itemId)
        {
            return false;
        }

        // Required quantity reached
        if (currentQuantity >=
            customer.RequestedQuantity)
        {
            return false;
        }

        return true;
    }

    // =========================
    // GET CURRENT CUSTOMER
    // =========================

    private Customer GetCurrentCustomer()
    {
        if (CustomerManager.Instance == null)
        {
            return null;
        }

        if (CustomerManager.Instance.CustomerCount <= 0)
        {
            return null;
        }

        if (CustomerManager.Instance.ActiveCustomers == null)
        {
            return null;
        }

        if (CustomerManager.Instance.ActiveCustomers.Count <= 0)
        {
            return null;
        }

        return
            CustomerManager.Instance.ActiveCustomers[0];
    }
}