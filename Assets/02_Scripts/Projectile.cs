using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(0.1f)] private float lifetime = 3f;

    private Rigidbody2D projectileRigidbody;

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // 빗나간 탄환이 화면에 계속 남지 않도록 제거한다.
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 direction, float speed)
    {
        direction.Normalize();
        projectileRigidbody.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out EnemyHealth enemyHealth))
        {
            return;
        }

        enemyHealth.TakeDamage(damage);
        Destroy(gameObject);
    }
}
