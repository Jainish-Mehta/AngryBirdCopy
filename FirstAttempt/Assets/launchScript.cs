using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour
{
    LineRenderer lr;
    [SerializeField] private FlyingBird bird;

    public float velocity;
    public float angle;
    public int resolution;
    private Vector3 dragStartPosition;
    private Vector3 dragEndPosition;
    private bool isDragging = false;
    float g; //force of gravity on the y axis
    float radianAngle;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        g = (Mathf.Abs(Physics2D.gravity.y))/2;
    }

    private void OnValidate()
    {
        if (lr != null && Application.isPlaying)
        {
            RenderArc();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        velocity = 0.06651f*(bird.launchForce);
        var mouse = Mouse.current;
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
        }
        if (resolution < 1)
        {
            resolution = 20; // Ensure resolution is at least 1
        }
        if (velocity <= 0)
        {
            velocity = 1; // Ensure velocity is positive
        }
    }
    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragStartPosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            dragStartPosition.z = 0;
            isDragging = true;
        }

        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            dragEndPosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            dragEndPosition.z = 0;

            Vector3 dragVector = dragStartPosition - dragEndPosition;

           // velocity = 9.97f; // Scale as needed
            angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;

            RenderArc();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            // You can trigger the launch here
            Debug.Log("Launch with velocity: " + velocity + " and angle: " + angle);
        }
    }

    //initialization
    void RenderArc()
    {
        // obsolete: lr.SetVertexCount(resolution + 1);
        lr.positionCount = resolution + 1;
        lr.SetPositions(CalculateArcArray());
    }
    //Create an array of Vector 3 positions for the arc
    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        Vector3 origin = new Vector3(-10f, -5f, 0f);
        return origin + new Vector3(x, y, 0f);

    }

}