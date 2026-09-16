using UnityEngine;

[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 1;
    [SerializeField] private GameObject experienceGemPrefab;

    private int currentHealth;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (IsDead)
        {
            // 적이 쓰러진 위치에 경험치 Gem을 하나 떨어뜨린다.
            if (experienceGemPrefab != null)
            {
                Instantiate(experienceGemPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
