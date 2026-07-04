using UnityEngine;
using System.Collections;

public class EnemyDebuff : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    float currentHealth;

    [Header("Armor")]
    public float armor = 0.3f; // 30% damage reduction
    bool armorBroken = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!armorBroken)
        {
            damage *= (1 - armor);
        }

        currentHealth -= damage;

        Debug.Log("Enemy took damage: " + damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ArmorBreak(float duration)
    {
        StartCoroutine(ArmorBreakRoutine(duration));
    }

    IEnumerator ArmorBreakRoutine(float duration)
    {
        armorBroken = true;

        Debug.Log("ARMOR BROKEN");

        yield return new WaitForSeconds(duration);

        armorBroken = false;

        Debug.Log("ARMOR RESTORED");
    }

    void Die()
    {
        Debug.Log("Enemy Died");

        Destroy(gameObject);
    }
}