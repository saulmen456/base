using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 dir = Vector2.zero;
    [SerializeField] private Joystick stick;
    private Animator animator;
    public bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stick ??= FindObjectOfType<Joystick>();

        #if !UNITY_STANDALONE
        stick?.gameObject.SetActive(true);
        #else
        stick?.gameObject.SetActive(false);
        #endif 
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            dir = Vector2.zero;
            animator.SetBool("isRunning", false);
            return;
        }

        #if !UNITY_STANDALONE
        dir = new Vector2(stick.Horizontal, stick.Vertical);
        #endif

        rb.MovePosition(rb.position + 5 * Time.fixedDeltaTime * dir);
        bool isRunning = dir.magnitude > 0;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isRunning2", dir.y < 0);
        transform.localScale = new Vector3(dir.x < 0 ? -1 : 1, 1, 1);
        if (dir.y < 0) animator.SetBool("isRunning", false);
    }

    public void OnMove(InputValue inputValue)
    {
        #if UNITY_STANDALONE
        if (!canMove) return;
        dir = inputValue.Get<Vector2>();
        #endif
    }

    public void EnableMovement() => canMove = true;
    public void DisableMovement() => canMove = false;
}