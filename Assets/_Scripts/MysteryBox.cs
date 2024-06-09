using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class MysteryBox : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component controlling the box animation
    public bool hasOpened = false; // Flag to track whether the box has been opened

    [SerializeField] private GameObject coinPrefab; // Prefab for the coin
    [SerializeField] private GameObject healthPotionPrefab; // Prefab for the health potion
    [SerializeField] private int coinAmount = 5; // Number of coins to spawn
    [SerializeField] private int healthPotionAmount = 1; // Number of health potions to spawn
    [SerializeField] private float spawnRadius = 5.0f; // Maximum radius around the box to spawn items
    [SerializeField] private float minSpawnDistance = 2.0f; // Minimum distance from the box to spawn items

    private BoxCollider2D boxCollider; // Reference to the physical collider
    private BoxCollider2D triggerCollider; // Reference to the trigger collider

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator reference is not set for " + gameObject.name);
        }

        // Get references to the colliders
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        if (colliders.Length < 2)
        {
            Debug.LogError("Two BoxCollider2D components are required.");
            return;
        }

        boxCollider = colliders[0];
        triggerCollider = colliders[1];

        // Ensure the trigger collider is set as a trigger
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasOpened)
        {
            OpenBox();
        }
    }

    public void OpenBox()
    {
        Debug.Log("Player entered the trigger of " + gameObject.name);
        // Play animation to open the box
        if (animator != null)
        {
            animator.SetTrigger("Open");
            hasOpened = true; // Set the flag to true after opening the box

            // Disable the physical collider to prevent blocking the player
            boxCollider.enabled = false;

            // Call method to spawn items
            StartCoroutine(SpawnItems());
        }
        else
        {
            Debug.LogError("Animator component is missing for " + gameObject.name);
        }
    }

    private IEnumerator SpawnItems()
    {
        // Wait for the duration of the opening animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Vector3 spawnPosition = transform.position;

        // Spawn coins
        for (int i = 0; i < coinAmount; i++)
        {
            Vector3 randomOffset = GetRandomOffset();
            Instantiate(coinPrefab, spawnPosition + randomOffset, Quaternion.identity);
        }

        // Spawn health potions
        for (int i = 0; i < healthPotionAmount; i++)
        {
            Vector3 randomOffset = GetRandomOffset();
            Instantiate(healthPotionPrefab, spawnPosition + randomOffset, Quaternion.identity);
        }

        // Wait for 1 second before making the box disappear
        yield return new WaitForSeconds(1f);

        // Make the box disappear
        gameObject.SetActive(false);
    }

    private Vector3 GetRandomOffset()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, spawnRadius);
        return new Vector3(randomDirection.x * randomDistance, randomDirection.y * randomDistance, 0);
    }
}
