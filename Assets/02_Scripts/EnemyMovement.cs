using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyMovement : MonoBehaviour
{
    private static readonly int IsRun = Animator.StringToHash("IsRun");
    private static readonly int IsAttack = Animator.StringToHash("IsAttack");

    [SerializeField, Min(0f)] private float moveSpeed = 2f;
    [SerializeField, Min(0f)] private float attackRange = 2f;
    [SerializeField, Min(1)] private int attackDamage = 1;
    [SerializeField] private Transform target;

    private Rigidbody2D enemyRigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerHealth targetHealth;
    private bool isAttacking;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (target == null)
        {
            // 프리팹은 씬의 Player를 직접 참조할 수 없으므로 시작할 때 찾는다.
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target != null)
        {
            targetHealth = target.GetComponent<PlayerHealth>();
        }
    }

    private void FixedUpdate()
    {
        if (target == null || PlayerIsDead())
        {
            enemyRigidbody.linearVelocity = Vector2.zero;
            isAttacking = false;
            return;
        }

        Vector2 offset = (Vector2)target.position - enemyRigidbody.position;
        Vector2 direction = offset.normalized;

        // 상하로 이동할 때는 마지막으로 바라보던 좌우 방향을 유지한다.
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            // 플레이어가 왼쪽에 있을 때 스프라이트를 좌우 반전한다.
            spriteRenderer.flipX = direction.x < 0f;
        }

        isAttacking = offset.sqrMagnitude <= attackRange * attackRange;
        if (isAttacking)
        {
            // 공격 사거리 안에서는 이동을 멈추고 공격한다.
            enemyRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        // 공격 사거리 밖에서는 플레이어를 향해 직선 이동한다.
        enemyRigidbody.linearVelocity = direction * moveSpeed;
    }

    private void Update()
    {
        if (PlayerIsDead())
        {
            // 플레이어가 사망하면 즉시 멈추고 Idle 애니메이션으로 전환한다.
            enemyRigidbody.linearVelocity = Vector2.zero;
            isAttacking = false;
            animator.SetBool(IsRun, false);
            animator.SetBool(IsAttack, false);
            return;
        }

        // 실제 이동 속도에 따라 Idle과 Run 애니메이션을 전환한다.
        bool isRunning = !isAttacking && enemyRigidbody.linearVelocity.sqrMagnitude > 0.01f;
        animator.SetBool(IsRun, isRunning);
        animator.SetBool(IsAttack, isAttacking);
    }

    // 공격 애니메이션의 타격 프레임에서 Animation Event로 호출한다.
    public void ApplyAttackDamage()
    {
        if (!isAttacking || target == null || PlayerIsDead())
        {
            return;
        }

        Vector2 offset = (Vector2)target.position - enemyRigidbody.position;
        if (offset.sqrMagnitude > attackRange * attackRange)
        {
            return;
        }

        if (target.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private bool PlayerIsDead()
    {
        return targetHealth != null && targetHealth.IsDead;
    }

    private void OnDisable()
    {
        if (enemyRigidbody != null)
        {
            enemyRigidbody.linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool(IsRun, false);
            animator.SetBool(IsAttack, false);
        }

        isAttacking = false;
    }
}
