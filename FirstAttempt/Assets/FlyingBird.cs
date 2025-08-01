using UnityEngine;
using UnityEngine.InputSystem;
public class FlyingBird : MonoBehaviour
{
    public float speed = 5f;
    public float height = 2f;
    public float frequency = 1f;
    [SerializeField] public float launchForce = 500f;
    private Vector3 startPosition;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private LineRenderer lr;
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private bool followCursor = false;
    private float timeLimit=5f;
    private bool cond = false;
    private bool isGrounded = true;
    void Start()
    {
        startPosition = transform.position;
        sr=GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        GoDown(keyboard);
        GoUp(keyboard);
        GoLeft(keyboard);
        GoRight(keyboard);
        CursorFollow(mouse);
        birdLaunch(mouse);
        checkBound();
        renderLine();
        //timeOut();
    }
    void GoDown(Keyboard keyboard)
    {
        if (keyboard.downArrowKey.isPressed)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }
    void GoUp(Keyboard keyboard)
    {
        if (keyboard.upArrowKey.isPressed)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }
    void GoLeft(Keyboard keyboard)
    {
        if (keyboard.leftArrowKey.isPressed)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (sr != null) sr.flipX = true;
        }
    }
    void GoRight(Keyboard keyboard)
    {
        if (keyboard.rightArrowKey.isPressed)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            if (sr != null) sr.flipX = false;
        }
    }
    void CursorFollow(Mouse mouse)
    {
         if (followCursor)
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
        if (mouse.leftButton.isPressed)
        {
            followCursor = true;
        }
        else if (mouse.leftButton.wasReleasedThisFrame)
        {
            followCursor = false;
        }
    }
    void birdLaunch(Mouse mouse)
    {
        if (mouse.leftButton.isPressed && !isDragging)
        {
            isDragging = true;
            dragStartPosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            dragStartPosition.z = 0f; // Ensure z is zero for 2D
            rb.gravityScale = 0f; // Disable gravity while dragging
            cond=false;
            isGrounded = true; // Reset grounded state
        }
        else if (mouse.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            Vector3 dragEndPosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            dragEndPosition.z = 0f; // Ensure z is zero for 2D
            Vector3 launchDirection = ( dragStartPosition - dragEndPosition  ).normalized;
            rb.AddForce(launchDirection * launchForce);
            rb.gravityScale=0.5f;
            cond = true; // Enable condition for checking bounds
            isGrounded = false; // Bird is no longer grounded after launch
        }
        
    }
    void checkBound()
    {
        if (cond)
        {
        timeLimit -= Time.deltaTime;
        }
        if (transform.position.y < -10 || transform.position.x < -30 || transform.position.x > 30 || (timeLimit <= 0f && rb.linearVelocity.magnitude<=0))
        {
            rb.linearVelocity = Vector2.zero; // Stop the bird
            rb.gravityScale = 0f; // Disable gravity
            transform.position = startPosition; // Reset position
            timeLimit = 5f; // Reset time limit
            transform.rotation = Quaternion.identity;
            cond = false; // Reset condition
            isDragging = false; // Reset dragging state
        }
            //Debug.Log(timeLimit);
    }
    void renderLine()
    {
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.positionCount = 2;
            }
        }
        if (lr != null)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = transform.position;
            positions[1] = startPosition;
            lr.SetPositions(positions);
        }
        if(!isGrounded)
        {
            lr.enabled = false; // Disable line renderer when bird is in the air
        }
        else
        {
            lr.enabled = true; // Enable line renderer when bird is grounded
        }
    }
}