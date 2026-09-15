using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;

    private Rigidbody2D playerRigidbody;
    private InputAction moveAction;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();

        PlayerInput playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move", true);
    }

    private void FixedUpdate()
    {
        // 대각선 이동 속도가 더 빨라지지 않도록 입력 크기를 1로 제한한다.
        Vector2 moveInput = Vector2.ClampMagnitude(
            moveAction.ReadValue<Vector2>(), 1f);

        playerRigidbody.linearVelocity = moveInput * moveSpeed;
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 남아 있는 이동 속도를 제거한다.
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
        }
    }
}
