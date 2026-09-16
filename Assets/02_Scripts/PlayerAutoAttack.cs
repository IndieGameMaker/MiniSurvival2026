using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerAutoAttack : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float attackInterval = 0.8f;
    [SerializeField, Min(0f)] private float projectileSpeed = 10f;
    [SerializeField, Min(0f)] private float detectionRange = 8f;
    [SerializeField] private Projectile projectilePrefab;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("PlayerAutoAttack의 Projectile Prefab을 확인해주세요.", this);
            enabled = false;
            return;
        }

        StartCoroutine(AttackAutomatically());
    }

    private IEnumerator AttackAutomatically()
    {
        WaitForSeconds attackDelay = new WaitForSeconds(attackInterval);

        while (!playerHealth.IsDead)
        {
            yield return attackDelay;

            if (playerHealth.IsDead)
            {
                yield break;
            }

            EnemyHealth closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                FireProjectile(closestEnemy.transform.position);
            }
        }
    }

    private EnemyHealth FindClosestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        EnemyHealth closestEnemy = null;
        float closestDistance = detectionRange * detectionRange;

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            float distance = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (distance <= closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void FireProjectile(Vector3 targetPosition)
    {
        Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Initialize(direction, projectileSpeed);
    }
}
