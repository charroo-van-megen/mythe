using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [HideInInspector] public float gScore = float.MaxValue;
    [HideInInspector] public float hScore = 0f;
    [HideInInspector] public Node cameFrom;

    public List<Node> connections = new List<Node>();

    public float FScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.15f); // Slightly larger for 3D clarity

        if (connections == null) return;

        Gizmos.color = Color.yellow;
        foreach (Node connection in connections)
        {
            if (connection != null)
            {
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
}
