using UnityEngine;

using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour

{
    public GameManager gameManager;

    public float moveSpeed = 5f;



    private Rigidbody rb;

    private Vector2 moveInput;



    void Start()

    {

        rb = GetComponent<Rigidbody>();

    }



    void Update()
    {
        if (gameManager == null || !gameManager.isPlaying)
        {
            moveInput = Vector2.zero;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current?.aKey?.isPressed ?? false)
            horizontal = -1f;

        if (Keyboard.current?.dKey?.isPressed ?? false)
            horizontal = 1f;

        if (Keyboard.current?.sKey?.isPressed ?? false)
            vertical = -1f;

        if (Keyboard.current?.wKey?.isPressed ?? false)
            vertical = 1f;

        moveInput =
             new Vector2(horizontal, vertical);
    }



    void FixedUpdate()
    {
        if (gameManager == null || !gameManager.isPlaying)
            return;

        if (rb == null)
            return;

        Vector3 movement =
             new Vector3(
                 moveInput.x,
                 0f,
                 moveInput.y
             );

        movement = movement.normalized;

        rb.MovePosition(
             rb.position +
             movement *
             moveSpeed *
             Time.fixedDeltaTime
         );
    }

}