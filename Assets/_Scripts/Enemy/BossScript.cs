using UnityEngine;

public class BossScript : MonoBehaviour, IDamageable
{
    public float speed = 5f;
    public Transform player;
    public int maxHealth = 500; // Boss's maximum health
    public GameObject healthBarPrefab; // Reference to the boss health bar prefab

    private int currentHealth;
    private HealthBarEnemy healthBarInstance;

    void Start()
    {
        currentHealth = maxHealth;
        // Instantiate and attach the health bar
        GameObject healthBarObject = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBarInstance = healthBarObject.GetComponentInChildren<HealthBarEnemy>();
    }

    void Update()
    {
        // Move towards the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        // Update health bar position to follow the boss
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2, 0)); // Adjust offset as needed
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBarInstance != null)
        {
            healthBarInstance.UpdateHealthBar(maxHealth, currentHealth); // Update boss health bar
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void StartBossBattle()
    {
        // Implement the logic to start the boss battle here
        Debug.Log("Boss battle started!");
    }

    private void Die()
{
    if (healthBarInstance != null)
    {
        Destroy(healthBarInstance.gameObject); // Destroy the health bar when the boss dies
    }

    // Show the victory menu using GameManager instance with a delay of 3 seconds
    if (GameManager.instance != null)
    {
        GameManager.instance.LoadVictoryScene(3f); // Wait for 3 seconds before loading the victory scene
    }

    Destroy(gameObject);
}

}
