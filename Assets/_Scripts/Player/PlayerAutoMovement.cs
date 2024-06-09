using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoMovement : MonoBehaviour
{
    public Pathfinding pathfinding;
    public Transform player;
    public float speed = 5f;
    public float enemyAvoidanceRadius = 5f;
    public Transform boss;

    private List<Node> path;
    private int currentPathIndex;
    private bool isAutoMoving = false;
    private HashSet<Node> visitedNodes = new HashSet<Node>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !isAutoMoving)
        {
            StartCoroutine(MoveToCollectiblesAndBoss());
        }
    }

    private IEnumerator MoveToCollectiblesAndBoss()
    {
        isAutoMoving = true;

        while (true)
        {
            Vector3 targetPosition = FindClosestCollectibleOrBox();

            if (targetPosition == Vector3.zero)
            {
                Debug.Log("No more collectibles or boxes. Moving to the boss.");
                targetPosition = boss.position;
            }

            yield return StartCoroutine(MoveToTarget(targetPosition));

            if (targetPosition == boss.position)
            {
                Debug.Log("Reached the boss.");
                isAutoMoving = false;
                yield break;
            }

            CollectAt(targetPosition);

            // Move away from the collected item or opened box
            yield return StartCoroutine(MoveAwayFrom(targetPosition, 1.0f)); // Move away by 1 unit
        }
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        path = pathfinding.FindPath(player.position, targetPosition);
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path found.");
            yield break;
        }

        currentPathIndex = 0;

        while (currentPathIndex < path.Count)
        {
            Vector3 nextPosition = path[currentPathIndex].worldPosition;
            while (Vector3.Distance(player.position, nextPosition) > 0.1f)
            {
                Vector3 direction = (nextPosition - player.position).normalized;
                player.position += direction * speed * Time.deltaTime;

                // Check for nearby enemies during movement
                if (IsEnemyNearby(player.position))
                {
                    Debug.Log("Enemy nearby, recalculating path.");
                    path = FindSafePath(targetPosition);
                    if (path == null || path.Count == 0)
                    {
                        Debug.LogWarning("No safe path found.");
                        yield break;
                    }
                    currentPathIndex = 0;
                    nextPosition = path[currentPathIndex].worldPosition;
                }

                // Check if the player is blocked and recalculate path
                if (IsBlocked())
                {
                    Debug.Log("Player blocked, recalculating path.");
                    path = pathfinding.FindPath(player.position, targetPosition);
                    if (path == null || path.Count == 0)
                    {
                        Debug.LogWarning("No path found.");
                        yield break;
                    }
                    currentPathIndex = 0;
                    nextPosition = path[currentPathIndex].worldPosition;
                }

                yield return null;
            }

            visitedNodes.Add(path[currentPathIndex]);
            currentPathIndex++;
        }
    }

    private IEnumerator MoveAwayFrom(Vector3 position, float distance)
    {
        Vector3 direction = (player.position - position).normalized;
        Vector3 targetPosition = player.position + direction * distance;
        while (Vector3.Distance(player.position, targetPosition) > 0.1f)
        {
            player.position = Vector3.MoveTowards(player.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private Vector3 FindClosestCollectibleOrBox()
    {
        Coin[] coins = FindObjectsOfType<Coin>();
        HealthPotion[] healthPotions = FindObjectsOfType<HealthPotion>();
        MysteryBox[] mysteryBoxes = FindObjectsOfType<MysteryBox>();

        if (coins.Length == 0 && healthPotions.Length == 0 && mysteryBoxes.Length == 0)
        {
            return Vector3.zero;
        }

        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Coin coin in coins)
        {
            float distance = Vector3.Distance(player.position, coin.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = coin.transform;
            }
        }

        foreach (HealthPotion potion in healthPotions)
        {
            float distance = Vector3.Distance(player.position, potion.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = potion.transform;
            }
        }

        foreach (MysteryBox box in mysteryBoxes)
        {
            float distance = Vector3.Distance(player.position, box.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = box.transform;
            }
        }

        return closest != null ? closest.position : Vector3.zero;
    }

    private void CollectAt(Vector3 position)
    {
        Coin[] coins = FindObjectsOfType<Coin>();
        foreach (Coin coin in coins)
        {
            if (Vector3.Distance(coin.transform.position, position) < 0.1f)
            {
                coin.CollectCoin();
                return;
            }
        }

        HealthPotion[] healthPotions = FindObjectsOfType<HealthPotion>();
        foreach (HealthPotion potion in healthPotions)
        {
            if (Vector3.Distance(potion.transform.position, position) < 0.1f)
            {
                potion.CollectPotion(player.gameObject);
                return;
            }
        }

        MysteryBox[] mysteryBoxes = FindObjectsOfType<MysteryBox>();
        foreach (MysteryBox box in mysteryBoxes)
        {
            if (Vector3.Distance(box.transform.position, position) < 0.1f && !box.hasOpened)
            {
                box.OpenBox();
                return;
            }
        }
    }

    private bool IsEnemyNearby(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, enemyAvoidanceRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    private List<Node> FindSafePath(Vector3 targetPosition)
    {
        List<Node> newPath = pathfinding.FindPath(player.position, targetPosition);
        while (newPath != null && newPath.Count > 0 && IsEnemyNearby(newPath[0].worldPosition))
        {
            if (visitedNodes.Contains(newPath[0]))
            {
                newPath.RemoveAt(0);
            }
            else
            {
                break;
            }
        }
        return newPath;
    }

    private bool IsBlocked()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
        }
    }
}
