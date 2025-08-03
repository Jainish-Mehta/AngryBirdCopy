using UnityEngine;
using UnityEngine.InputSystem;
public class FlyingBird : MonoBehaviour
{
    public float speed;
    public float height = 2f;
    public float frequency = 1f;
    [SerializeField] public float launchForce = 300f;
    [SerializeField] private int predictionSteps = 50;
    [SerializeField] private float timeStep = 0.1f;
    private Vector3 startPosition;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    [SerializeField] private LineRenderer lr;
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private Vector3 dragEndPosition;
    private bool followCursor = false;
    private float timeLimit=5f;
    private bool cond = false;
    private float theta;
    private Vector3 direction;
    private float maxDistance;
    //private bool isGrounded = true;
    void Start()
    {
        startPosition = transform.position;
        sr=GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if(lr == null)
        {
            lr = gameObject.AddComponent<LineRenderer>();
            lr.positionCount = 0; // Initialize with no points
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
           // lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;
        }
    }
    void Update()
    {
        speed=rb.linearVelocity.magnitude;
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        GoDown(keyboard);
        GoUp(keyboard);
        GoLeft(keyboard);
        GoRight(keyboard);
        CursorFollow(mouse);
        birdLaunch(mouse);
        checkBound();
        calculateTheta(dragStartPosition ,  dragEndPosition);
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
            dragStartPosition.z = 0f;
            rb.gravityScale = 0f;
            cond = false;
        }
        else if (isDragging && mouse.leftButton.isPressed)
        {

            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            currentMousePos.z = 0f;
            direction = dragStartPosition - currentMousePos;
            Vector3 launchDirection = direction.normalized;

            float launchSpeed = launchForce; // Use impulse-based speed
            PredictTrajectory(direction, launchForce);
        }
        else if (mouse.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            dragEndPosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            dragEndPosition.z = 0f;
            Vector3 launchDirection = (dragStartPosition - dragEndPosition).normalized;

            rb.AddForce(launchDirection * launchForce);
            rb.gravityScale = 0.5f;
            cond = true;

            lr.positionCount = 0; // Clear prediction line after launch
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
        }
            //Debug.Log(timeLimit);
    }
    void calculateTheta(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        
        
            if (dragStartPosition != Vector3.zero && dragEndPosition != Vector3.zero)
            {
                direction = dragEndPosition - dragStartPosition;
                theta = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                maxDistance = speed * speed * Mathf.Sin(2 * theta * Mathf.Deg2Rad) / 4.9f;
              //  Debug.Log($"Theta: {((360+theta)%360)%90} ,maxDistance: {maxDistance} ");
                //Debug.Log($"speed: {speed}  ");
                //Debug.Log("Gravity2D: " + Physics2D.gravity);

            }

    }

    void PredictTrajectory(Vector3 launchDirection, float launchSpeed)
    {
        lr.positionCount = predictionSteps;
        Vector3[] points = new Vector3[predictionSteps];
        Vector3 startPos = transform.position;
        float gravity = 4.91f;

        for (int i = 0; i < predictionSteps; i++)
        {
            float t = i * timeStep;
            float dx = launchSpeed * launchDirection.x * t;
            float dy = launchSpeed * launchDirection.y * t + 0.5f * gravity * t * t;
            points[i] = startPos + new Vector3(dx, dy, 0);
        }

        lr.SetPositions(points);
    }
}