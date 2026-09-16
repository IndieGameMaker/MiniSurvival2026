using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 5;
    [SerializeField, Min(0f)] private float damageFlashDuration = 0.15f;
    [SerializeField] private Color damageColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine damageFlashCoroutine;
    private int currentHealth;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"Player HP: {currentHealth}/{maxHealth}", this);

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(ShowDamageColor());

        if (currentHealth <= 0)
        {
            Debug.Log("Player가 사망했습니다.", this);
        }
    }

    private IEnumerator ShowDamageColor()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor;
        damageFlashCoroutine = null;
    }

    private void OnDisable()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}
