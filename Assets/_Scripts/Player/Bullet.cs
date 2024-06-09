using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;
    public int damage = 30; // Damage value of the bullet
    public LayerMask enemyLayer; // Layer mask for enemy objects
    private float timeToDeactivate;

    void OnEnable()
    {
        Debug.Log("Bullet spawned, setting up destruction in " + lifeTime + " seconds.");
        timeToDeactivate = Time.time + lifeTime;
    }

    void Update()
    {
        if (Time.time >= timeToDeactivate)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log("Bullet hit: " + hitInfo.name);

        // Check if the bullet hits an enemy or boss using layer mask
        if (((1 << hitInfo.gameObject.layer) & enemyLayer) != 0)
        {
            IDamageable target = hitInfo.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log("Bullet hit damageable target: " + hitInfo.name);
                gameObject.SetActive(false); // Deactivate the bullet after hitting an enemy or boss
            }
        }
        else
        {
            Debug.Log("Bullet hit non-enemy target: " + hitInfo.name);
            // Do not destroy the bullet if it hits non-enemy objects
        }
    }
}

public interface IDamageable
{
    void TakeDamage(int damage);
}
