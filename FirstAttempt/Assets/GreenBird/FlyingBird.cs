using UnityEngine;
using UnityEngine.InputSystem;
public class FlyingBird : MonoBehaviour
{
    [SerializeField] private float speed =19.90f;
    [SerializeField] public float launchForce = 300f;
    private Vector3 startPosition;
    private SpriteRenderer sr;
   // private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D rb;

    // [SerializeField] private LineRenderer lr;
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private Vector3 dragEndPosition;
    private bool followCursor = false;
    private float timeLimit=5f;
    private bool cond = false;
    public bool firstTry = true; // To ensure the bird can only be launched once per drag
    void Start()
    {
        startPosition = transform.position;
        sr=GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        GoDown(keyboard);
        GoUp(keyboard);
        GoLeft(keyboard);
        GoRight(keyboard);
        CursorFollow(mouse);
        birdLaunch(mouse);
        checkBound();
        
        //renderLine();
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
        if (mouse.leftButton.isPressed&& isDragging)
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
        if (mouse.leftButton.isPressed && !isDragging && firstTry)
        {
            isDragging = true;
            firstTry= false;
            dragStartPosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            dragStartPosition.z = 0f;
            rb.gravityScale = 0f;
            cond = false;
        }
        
        else if (mouse.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            dragEndPosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            dragEndPosition.z = 0f;
            Vector3 launchDirection = (dragStartPosition - dragEndPosition).normalized;
            Debug.Log("Drag Start Position: " + dragStartPosition);
            Debug.Log("Drag End Position: " + dragEndPosition);
            Debug.Log("Launch Direction: " + launchDirection);
            rb.AddForce(launchDirection * launchForce);
            rb.gravityScale = 0.5f;
            cond = true;

            
        }
    }
    void checkBound()
    {
        if (cond)
        {
        timeLimit -= Time.deltaTime;
        }
        if (transform.position.y < -10 || transform.position.x < -30 || transform.position.x > 1000 || (timeLimit <= 0f && rb.linearVelocity.magnitude<=0))
        {
            rb.linearVelocity = Vector2.zero; // Stop the bird
            rb.gravityScale = 0f; // Disable gravity
            transform.position = startPosition; // Reset position
            timeLimit = 10f; // Reset time limit
            transform.rotation = Quaternion.identity;
            cond = false; // Reset condition
            isDragging = false; // Reset dragging state
            firstTry = true; // Allow the bird to be launched again
        }
            //Debug.Log(timeLimit);
    }
}
