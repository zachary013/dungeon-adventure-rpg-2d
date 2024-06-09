using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int damageAmount = 30; // Amount of damage inflicted to enemies

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an enemy or boss
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            // Damage the enemy or boss
            IDamageable target = collision.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damageAmount);
            }
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an enemy or boss
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            // Damage the enemy or boss
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
        }
    }*/
}
