using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 300;
    int currentHealth;

    public Slider bossHpSlider;
    public GameObject bossUI;
    

    void Start()
    {
        currentHealth = maxHealth;
        bossHpSlider.maxValue = maxHealth;
        bossHpSlider.value = maxHealth;

        bossUI.SetActive(false); // hidden muna
    }
        void Update()
    {
        // TEST DAMAGE
    if (Input.GetKeyDown(KeyCode.H))
    {
        TakeDamage(25);
    }
    }
    

    public void TakeDamage(int damage)
    {
        // show boss UI kapag tinamaan
        bossUI.SetActive(true);

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        bossHpSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // mawala UI
        bossUI.SetActive(false);

        // destroy boss
        Destroy(gameObject);
    }
    
        

}