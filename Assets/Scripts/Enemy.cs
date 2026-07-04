using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    public Slider hpBar; // drag slider dito sa inspector

    void Start()
    {
        currentHealth = maxHealth;

        hpBar.maxValue = maxHealth;
        hpBar.value = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        hpBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}