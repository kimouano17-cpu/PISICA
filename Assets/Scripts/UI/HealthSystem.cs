using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    public Animator animator;

    [Header("Potion Throw")]
    public GameObject thrownPotionPrefab;
    public Transform throwPoint;
    public float throwForce = 15f;

    [Header("Potion")]
    public GameObject potionObject;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaRegenSpeed = 25f;

    [Header("UI")]
    public BossHPBarUI bossUI;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        
        potionObject.SetActive(false);

        if (bossUI != null)
        {
            bossUI.SetHealth(currentHealth, maxHealth);
            bossUI.SetStamina(currentStamina, maxStamina);
        }
    }

  
// TEST HEAVY HIT

    public void HealPlayer()
{
     Heal(15);
     StartCoroutine(HidePotionDelay());
     
}
IEnumerator HidePotionDelay()
{
    yield return new WaitForSeconds(1.2f); // delay seconds
    potionObject.SetActive(false);
}

public void ThrowBottle()
{
    Debug.Log("ThrowBottle called");

    if (thrownPotionPrefab == null)
    {
        Debug.LogError("ThrownPotionPrefab is NOT assigned!");
        return;
    }

    if (throwPoint == null)
    {
        Debug.LogError("ThrowPoint is NOT assigned!");
        return;
    }

    GameObject bottle = Instantiate(thrownPotionPrefab, throwPoint.position, throwPoint.rotation);
    Debug.Log("Bottle spawned: " + bottle.name);

    Rigidbody rb = bottle.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
    }
}

    // HEALTH FUNCTIONS
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0f)
            currentHealth = 0f;

        UpdateHealthUI();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (bossUI != null)
            bossUI.SetHealth(currentHealth, maxHealth);
    }

    // STAMINA FUNCTIONS
    public bool UseStamina(float amount)
    {
        if (currentStamina < amount)
            return false;

        currentStamina -= amount;

        UpdateStaminaUI();
        return true;
    }

    void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenSpeed * Time.deltaTime;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            UpdateStaminaUI();
        }
    }

    void UpdateStaminaUI()
    {
        if (bossUI != null)
            bossUI.SetStamina(currentStamina, maxStamina);
    }
}