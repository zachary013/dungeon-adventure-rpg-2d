using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int damageFromEnemy = 10;
    [SerializeField] private int bossAttackDamage = 20;
    [SerializeField] private GameOverMenu gameOverMenu; // Reference to the Game Over menu script

    private int currentHealth;
    private Animator animator;

    private void Start()
    {
        InitializeHealth();
        animator = GetComponent<Animator>();
    }

    private void InitializeHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Debug.Log("Player died.");
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        // Allow some time for the death animation to play before showing the Game Over menu
        yield return new WaitForSeconds(0.5f); // Adjust this duration based on your animation length

        if (gameOverMenu != null)
        {
            gameOverMenu.ShowGameOverMenu(); // Activate Game Over menu
        }
        else
        {
            Debug.LogWarning("Game Over menu is not assigned or found.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentHealth <= 0) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(damageFromEnemy);
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            TakeDamage(bossAttackDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentHealth <= 0) return;

        if (other.gameObject.CompareTag("HealthPotion"))
        {
            HealthPotion healthPotion = other.gameObject.GetComponent<HealthPotion>();
            if (healthPotion != null)
            {
                Heal(healthPotion.healAmount);
                Destroy(other.gameObject); // Destroy the health potion after use
            }
        }
    }

    public void Heal(int healAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        healthBar.SetHealth(currentHealth);
    }
}
