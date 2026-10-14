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
        if (!gameManager.isPlaying)
        {
            moveInput = Vector2.zero;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed)
            horizontal = -1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal = 1f;

        if (Keyboard.current.sKey.isPressed)
            vertical = -1f;

        if (Keyboard.current.wKey.isPressed)
            vertical = 1f;

        moveInput =
             new Vector2(horizontal, vertical);
    }


    void FixedUpdate()
    {
        Vector3 movement = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        rb.MovePosition(
             rb.position +
             movement *
             moveSpeed *
             Time.fixedDeltaTime
         );
    }

}