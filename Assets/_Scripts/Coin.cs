using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Amount of score to add when the coin is collected
    [SerializeField] private int scoreValue = 1;

    // Reference to the GameManager or any score tracking component
    private GameManager gameManager;

    private void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collided with the coin
        if (collision.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    public void CollectCoin()
    {
        // Add score to the GameManager
        if (gameManager != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }

        // Destroy the coin object
        Destroy(gameObject);
    }
}
