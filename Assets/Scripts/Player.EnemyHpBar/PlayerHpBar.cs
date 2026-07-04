using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar: MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthSlider;
    

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
        void Update()
    {
        // TEST DAMAGE
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth < 0)
            currentHealth = 0;

        healthSlider.value = currentHealth;
    }
}