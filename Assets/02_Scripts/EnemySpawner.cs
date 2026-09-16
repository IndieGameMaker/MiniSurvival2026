using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Min(0.1f)] private float spawnInterval = 2f;
    [SerializeField, Min(0f)] private float viewportPadding = 0.05f;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }

        if (playerHealth == null)
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        }
    }

    private void Start()
    {
        if (enemyPrefab == null || gameCamera == null || playerHealth == null)
        {
            Debug.LogWarning("EnemySpawner 설정을 확인해주세요.", this);
            enabled = false;
            return;
        }

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds spawnDelay = new WaitForSeconds(spawnInterval);

        while (!playerHealth.IsDead)
        {
            yield return spawnDelay;

            if (playerHealth.IsDead)
            {
                yield break;
            }

            Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomPosition = Random.value;
        Vector2 viewportPosition;

        // 카메라의 왼쪽, 오른쪽, 아래, 위 중 한 곳을 무작위로 선택한다.
        switch (Random.Range(0, 4))
        {
            case 0:
                viewportPosition = new Vector2(-viewportPadding, randomPosition);
                break;
            case 1:
                viewportPosition = new Vector2(1f + viewportPadding, randomPosition);
                break;
            case 2:
                viewportPosition = new Vector2(randomPosition, -viewportPadding);
                break;
            default:
                viewportPosition = new Vector2(randomPosition, 1f + viewportPadding);
                break;
        }

        float distanceFromCamera = Mathf.Abs(gameCamera.transform.position.z);
        Vector3 spawnPosition = gameCamera.ViewportToWorldPoint(
            new Vector3(viewportPosition.x, viewportPosition.y, distanceFromCamera));
        spawnPosition.z = 0f;
        return spawnPosition;
    }
}
