using System.Collections;
using UnityEngine;

public class Customerspawner : MonoBehaviour
{
    [Header("Customer")]
    [SerializeField] private GameObject customerPrefab;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("Customer Names")]
    [SerializeField]
    private string[] customerNames =
    {
        "Shanthi",
        "Duraisamy",
        "Divya",
        "Rithick Ram",
        "Rithanya",
        "Vignesh",
        "Arun",
        "Priya",
        "Rahul",
        "Karthik",
        "Meena",
        "Vijay",
        "Anitha"
    };

    [Header("Customer ID")]
    [SerializeField] private int nextCustomerNumber = 1;

    [Header("Patience")]
    [SerializeField] private float patience = 100f;

    [Header("Next Customer")]
    [SerializeField] private float nextCustomerDelay = 2f;

    private GameObject currentCustomerObject;

    private bool waitingForNextCustomer;

    private string currentCustomerId;

    private string lastCustomerName;

    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        CustomerManager.OnCustomerListChanged +=
            HandleCustomerListChanged;

        ShopManager.OnShopUpdated +=
            HandleShopUpdated;
    }

    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        CustomerManager.OnCustomerListChanged -=
            HandleCustomerListChanged;

        ShopManager.OnShopUpdated -=
            HandleShopUpdated;
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (IsShopClosed())
        {
            Debug.Log(
                "Customer spawning skipped: Shop is closed."
            );

            return;
        }

        SpawnCustomer();
    }

    // =========================================================
    // SHOP STATUS
    // =========================================================

    private bool IsShopClosed()
    {
        return ShopManager.Instance != null &&
               !ShopManager.Instance.IsShopOpen;
    }

    private void HandleShopUpdated()
    {
        if (IsShopClosed())
        {
            StopNextCustomerTimer();

            Debug.Log(
                "Customerspawner: Shop closed. " +
                "New customer spawning stopped."
            );

            return;
        }

        if (currentCustomerObject == null &&
            !waitingForNextCustomer &&
            CustomerManager.Instance != null &&
            CustomerManager.Instance.CustomerCount == 0)
        {
            StartNextCustomerTimer();
        }
    }

    // =========================================================
    // SPAWN CUSTOMER
    // =========================================================

    public void SpawnCustomer()
    {
        if (IsShopClosed())
        {
            Debug.LogWarning(
                "Customer spawn skipped: Shop is closed."
            );

            return;
        }

        if (currentCustomerObject != null)
        {
            Debug.LogWarning(
                "Customer spawn skipped: " +
                "A customer is already active."
            );

            return;
        }

        if (CustomerManager.Instance == null)
        {
            Debug.LogError(
                "Customer spawn failed: " +
                "CustomerManager not found."
            );

            return;
        }

        if (customerPrefab == null)
        {
            Debug.LogError(
                "Customer spawn failed: " +
                "Customer Prefab is not assigned."
            );

            return;
        }

        if (CustomerRequestPool.Instance == null)
        {
            Debug.LogError(
                "Customer spawn failed: " +
                "CustomerRequestPool not found."
            );

            return;
        }

        currentCustomerId =
            CreateCustomerId();

        string newCustomerName =
            GetNextCustomerName();

        CustomerRequest request =
            CustomerRequestPool.Instance
                .GetRandomRequest();

        if (request == null)
        {
            Debug.LogError(
                "Customer spawn failed: " +
                "Could not create customer request."
            );

            return;
        }

        Vector3 spawnPosition =
            transform.position;

        Quaternion spawnRotation =
            transform.rotation;

        if (spawnPoint != null)
        {
            spawnPosition =
                spawnPoint.position;

            spawnRotation =
                spawnPoint.rotation;
        }

        currentCustomerObject =
            Instantiate(
                customerPrefab,
                spawnPosition,
                spawnRotation
            );

        Customer customer =
            CustomerManager.Instance.CreateCustomer(
                currentCustomerId,
                newCustomerName,
                request.ItemId,
                request.ItemName,
                request.Quantity,
                patience
            );

        if (customer == null)
        {
            Destroy(
                currentCustomerObject
            );

            currentCustomerObject = null;

            Debug.LogError(
                "Customer spawn failed: " +
                "Could not create customer data."
            );

            return;
        }

        Debug.Log(
            $"Customer spawned | " +
            $"Name: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Request: {customer.RequestedItemName} x" +
            $"{customer.RequestedQuantity}"
        );
    }

    // =========================================================
    // CUSTOMER LIST CHANGED
    // =========================================================

    private void HandleCustomerListChanged()
    {
        if (CustomerManager.Instance == null)
        {
            return;
        }

        if (currentCustomerObject != null)
        {
            bool CustomerstillExists =
                CustomerManager.Instance.HasCustomer(
                    currentCustomerId
                );

            if (!CustomerstillExists)
            {
                DestroyCurrentCustomer();

                if (!IsShopClosed())
                {
                    StartNextCustomerTimer();
                }
            }

            return;
        }

        if (!IsShopClosed() &&
            !waitingForNextCustomer &&
            CustomerManager.Instance.CustomerCount == 0)
        {
            StartNextCustomerTimer();
        }
    }

    // =========================================================
    // START NEXT CUSTOMER TIMER
    // =========================================================

    private void StartNextCustomerTimer()
    {
        if (IsShopClosed())
        {
            return;
        }

        if (waitingForNextCustomer)
        {
            return;
        }

        StartCoroutine(
            SpawnNextCustomer()
        );
    }

    // =========================================================
    // STOP NEXT CUSTOMER TIMER
    // =========================================================

    private void StopNextCustomerTimer()
    {
        if (waitingForNextCustomer)
        {
            StopAllCoroutines();

            waitingForNextCustomer = false;

            Debug.Log(
                "Customerspawner: Next customer timer stopped."
            );
        }
    }

    // =========================================================
    // NEXT CUSTOMER
    // =========================================================

    private IEnumerator SpawnNextCustomer()
    {
        waitingForNextCustomer = true;

        Debug.Log(
            $"Waiting {nextCustomerDelay} seconds " +
            $"before spawning next customer."
        );

        yield return new WaitForSeconds(
            nextCustomerDelay
        );

        waitingForNextCustomer = false;

        if (IsShopClosed())
        {
            Debug.Log(
                "Next customer spawn cancelled: " +
                "Shop is closed."
            );

            yield break;
        }

        SpawnCustomer();
    }

    // =========================================================
    // CREATE CUSTOMER ID
    // =========================================================

    private string CreateCustomerId()
    {
        string id =
            $"customer_{nextCustomerNumber:000}";

        nextCustomerNumber++;

        return id;
    }

    // =========================================================
    // GET CUSTOMER NAME
    // =========================================================

    private string GetNextCustomerName()
    {
        if (customerNames == null ||
            customerNames.Length == 0)
        {
            Debug.LogWarning(
                "No customer names configured. " +
                "Using default name."
            );

            return "Customer";
        }

        if (customerNames.Length == 1)
        {
            lastCustomerName =
                customerNames[0];

            return customerNames[0];
        }

        string selectedName;

        int attempts = 0;

        do
        {
            int index =
                Random.Range(
                    0,
                    customerNames.Length
                );

            selectedName =
                customerNames[index];

            attempts++;

        }
        while (
            selectedName == lastCustomerName &&
            attempts < 20
        );

        lastCustomerName =
            selectedName;

        return selectedName;
    }

    // =========================================================
    // DESTROY CUSTOMER
    // =========================================================

    private void DestroyCurrentCustomer()
    {
        if (currentCustomerObject == null)
        {
            return;
        }

        Destroy(
            currentCustomerObject
        );

        currentCustomerObject = null;

        Debug.Log(
            $"Customer GameObject destroyed | " +
            $"ID: {currentCustomerId}"
        );
    }

    // =========================================================
    // MANUAL DESPAWN
    // =========================================================

    public void DespawnCustomer()
    {
        if (CustomerManager.Instance != null &&
            !string.IsNullOrWhiteSpace(currentCustomerId))
        {
            CustomerManager.Instance.RemoveCustomer(
                currentCustomerId
            );
        }
        else
        {
            DestroyCurrentCustomer();
        }
    }

    // =========================================================
    // DEVELOPMENT TESTS
    // =========================================================

    [ContextMenu("TEST - Spawn Customer")]
    private void TestSpawnCustomer()
    {
        SpawnCustomer();
    }

    [ContextMenu("TEST - Stop Customer Timer")]
    private void TestStopCustomerTimer()
    {
        StopNextCustomerTimer();
    }
}