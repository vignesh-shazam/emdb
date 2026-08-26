using System.Collections.Generic;
using UnityEngine;

public class CustomerRequestPool : MonoBehaviour
{
    public static CustomerRequestPool Instance { get; private set; }

    [System.Serializable]
    public class RequestItem
    {
        public string itemId;
        public string itemName;
        public int minQuantity = 1;
        public int maxQuantity = 1;
    }

    [Header("Available Customer Items")]
    [SerializeField]
    private List<RequestItem> availableItems =
        new List<RequestItem>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =========================
    // GET RANDOM REQUEST
    // =========================

    public CustomerRequest GetRandomRequest()
    {
        if (availableItems == null ||
            availableItems.Count == 0)
        {
            Debug.LogWarning(
                "Customer request pool is empty."
            );

            return null;
        }

        RequestItem selectedItem =
            availableItems[
                Random.Range(
                    0,
                    availableItems.Count
                )
            ];

        int quantity =
            Random.Range(
                selectedItem.minQuantity,
                selectedItem.maxQuantity + 1
            );

        CustomerRequest request =
            new CustomerRequest(
                selectedItem.itemId,
                selectedItem.itemName,
                quantity
            );

        if (!request.IsValid())
        {
            Debug.LogWarning(
                "Generated customer request is invalid."
            );

            return null;
        }

        return request;
    }
}