using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int lightDamage = 25;
    public int heavyDamage = 50;

    public float attackRadius = 0.5f;

    public Vector3 hitboxOffset; // pwede iusog sa inspector

    public LayerMask enemyLayer;

    public void DealLightDamage()
    {
        DealDamage(lightDamage);
    }

    public void DealHeavyDamage()
    {
        DealDamage(heavyDamage);
    }
    
    

    void DealDamage(int damage)
    {
        Vector3 hitPoint = transform.position + transform.forward * hitboxOffset.z + transform.right * hitboxOffset.x + transform.up * hitboxOffset.y;

        Collider[] enemies = Physics.OverlapSphere(hitPoint, attackRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();

            if (e != null)
            {
                e.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 hitPoint = transform.position + transform.forward * hitboxOffset.z + transform.right * hitboxOffset.x + transform.up * hitboxOffset.y;

        Gizmos.DrawWireSphere(hitPoint, attackRadius);
    }
}