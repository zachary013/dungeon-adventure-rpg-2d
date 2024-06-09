using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 30; // Amount of health restored by this potion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collided with the health potion
        if (collision.CompareTag("Player"))
        {
            CollectPotion(collision.gameObject);
        }
    }

    public void CollectPotion(GameObject player)
    {
        // Here you can add code to increase the player's health
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }

        // Update the health potion count in the GameManager
        GameManager.instance.AddHealthPotion(1);

        // Destroy the health potion object
        Destroy(gameObject);
    }
}
