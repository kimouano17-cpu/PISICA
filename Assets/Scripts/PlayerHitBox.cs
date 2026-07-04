using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public float radius = 0.4f;
    public LayerMask enemyLayer;

    public void Hit(int damage)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius, enemyLayer);

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
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}