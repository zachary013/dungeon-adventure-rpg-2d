using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour, IDamageable
{
    public float speed = 3f;
    public int maxHealth = 100;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    public GameObject deathEffect;
    public GameObject healthBarPrefab;
    public float pathUpdateInterval = 0.5f; // Interval to update path

    private int currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private float knockbackEndTime;
    private HealthBarEnemy healthBarInstance;
    private Pathfinding pathfinding;
    private GridMap gridMap;
    private List<Node> path;
    private int currentPathIndex = 0;
    private float nextPathUpdateTime = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        pathfinding = FindObjectOfType<Pathfinding>();
        gridMap = FindObjectOfType<GridMap>();
        healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity).GetComponent<HealthBarEnemy>();
        healthBarInstance.UpdateHealthBar(maxHealth, currentHealth);
    }

    private void Update()
    {
        if (isKnockedBack)
        {
            if (Time.time >= knockbackEndTime)
            {
                isKnockedBack = false;
            }
            return;
        }

        if (player != null && Time.time >= nextPathUpdateTime)
        {
            UpdatePath();
            nextPathUpdateTime = Time.time + pathUpdateInterval;
        }

        MoveAlongPath();
    }

    private void UpdatePath()
    {
        path = pathfinding.FindPath(transform.position, player.position);
        currentPathIndex = 0;
    }

    private void MoveAlongPath()
    {
        if (path == null || path.Count == 0) return;

        Vector3 targetPosition = path[currentPathIndex].worldPosition;
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * speed;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= path.Count)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBarInstance.UpdateHealthBar(maxHealth, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;
        rb.velocity = direction * knockbackForce;
    }
}