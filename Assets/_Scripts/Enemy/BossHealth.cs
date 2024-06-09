using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 200; // Maximum health of the boss
    private int currentHealth; // Current health of the boss

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to maximum health
    }

    // Method to reduce boss health
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Decrease current health by the amount of damage

        // Check if the boss's health has dropped to or below zero
        if (currentHealth <= 0)
        {
            Die(); // Call the Die method if the boss has no health left
        }
    }

    // Method called when the boss's health reaches zero
    void Die()
    {
        // Death logic for the boss
        Debug.Log("Boss defeated.");
        Destroy(gameObject); // Destroy the boss object
    }
}
