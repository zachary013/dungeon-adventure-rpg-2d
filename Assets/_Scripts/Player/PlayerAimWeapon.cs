using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimWeapon : MonoBehaviour
{
    public Transform gunTransform; // Assign this to the Shooter GameObject
    public GameObject bulletPrefab; // Bullet prefab to be instantiated
    public Transform firePoint; // The point from where bullets are fired
    public float bulletSpeed = 20f; // Speed of the bullet
    public float fireRate = 0.5f; // Rate of fire (bullets per second)
    private float nextFireTime = 0f; // Time when the player can fire next
    private ObjectPool bulletPool;

    void Start()
    {
        // Log the initial bulletPrefab assignment
        if (bulletPrefab != null)
        {
            Debug.Log("Bullet prefab assigned successfully in Start.");
        }
        else
        {
            Debug.LogError("Bullet prefab is missing in Start.");
        }

        bulletPool = new ObjectPool(bulletPrefab, 20); // Initial pool size
    }

    void Update()
    {
        AimGun();

        // Allow continuous shooting while Fire1 button is held down
        if (Input.GetButton("Fire1") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + (1 / fireRate);
            Shoot();
        }
    }

    void AimGun()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gunTransform.position = transform.position; // Ensure gun is always at the player's position
        gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is missing when trying to shoot.");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned.");
            return;
        }

        GameObject bullet = bulletPool.GetObject();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.right * bulletSpeed;
        }
    }
}
