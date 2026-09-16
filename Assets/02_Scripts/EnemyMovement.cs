using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 2f;
    [SerializeField] private Transform target;

    private Rigidbody2D enemyRigidbody;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (target != null)
        {
            return;
        }

        // 프리팹은 씬의 Player를 직접 참조할 수 없으므로 시작할 때 찾는다.
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            enemyRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        // 플레이어 방향만 사용해 일정한 속도로 직선 이동한다.
        Vector2 direction = ((Vector2)target.position - enemyRigidbody.position).normalized;
        enemyRigidbody.linearVelocity = direction * moveSpeed;
    }

    private void OnDisable()
    {
        if (enemyRigidbody != null)
        {
            enemyRigidbody.linearVelocity = Vector2.zero;
        }
    }
}
