using UnityEngine;

public class PlayerLifeManager : MonoBehaviour
{
    public static PlayerLifeManager Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = 100f;

    [Header("Hunger")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float currentHunger = 100f;

    public float CurrentHealth => currentHealth;
    public float CurrentEnergy => currentEnergy;
    public float CurrentHunger => currentHunger;

    public float MaxHealth => maxHealth;
    public float MaxEnergy => maxEnergy;
    public float MaxHunger => maxHunger;

    public PlayerActivity CurrentActivity { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        currentEnergy = Mathf.Clamp(
            currentEnergy,
            0f,
            maxEnergy
        );

        currentHunger = Mathf.Clamp(
            currentHunger,
            0f,
            maxHunger
        );

        CurrentActivity = PlayerActivity.Idle;
    }

    // =========================
    // HEALTH
    // =========================

    public void RestoreHealth(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Clamp(
            currentHealth + amount,
            0f,
            maxHealth
        );
    }

    public void ReduceHealth(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Clamp(
            currentHealth - amount,
            0f,
            maxHealth
        );
    }

    // =========================
    // ENERGY
    // =========================

    public void RestoreEnergy(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentEnergy = Mathf.Clamp(
            currentEnergy + amount,
            0f,
            maxEnergy
        );
    }

    public void ReduceEnergy(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentEnergy = Mathf.Clamp(
            currentEnergy - amount,
            0f,
            maxEnergy
        );
    }

    // =========================
    // HUNGER
    // =========================

    public void IncreaseHunger(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentHunger = Mathf.Clamp(
            currentHunger + amount,
            0f,
            maxHunger
        );
    }

    public void ReduceHunger(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentHunger = Mathf.Clamp(
            currentHunger - amount,
            0f,
            maxHunger
        );
    }

    // =========================
    // ACTIVITY
    // =========================

    public void SetActivity(PlayerActivity activity)
    {
        CurrentActivity = activity;
    }

    public bool IsDoing(PlayerActivity activity)
    {
        return CurrentActivity == activity;
    }
}