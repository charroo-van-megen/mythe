using UnityEngine;

public class GrappleLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;  // Reference to the LineRenderer
    public Transform startTransform;   // Starting point (usually the player or gun)
    public Transform endTransform;     // Ending point (the grapple point)

    void Start()
    {
        // Ensure we have a LineRenderer component
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Set the number of points in the LineRenderer (start and end)
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        // Update the positions of the line renderer to follow the start and end points
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, startTransform.position);  // Set start point
            lineRenderer.SetPosition(1, endTransform.position);    // Set end point
        }
    }
}
