using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple AStarManager instances detected!");
            Destroy(this);
        }
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        if (start == null || end == null)
        {
            Debug.LogWarning("Start or End node is null in GeneratePath.");
            return null;
        }

        List<Node> openSet = new List<Node>();

        foreach (Node node in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            node.gScore = float.MaxValue;
            node.cameFrom = null;
        }

        start.gScore = 0;
        start.hScore = Vector3.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            foreach (Node node in openSet)
            {
                if (node.FScore() < currentNode.FScore())
                    currentNode = node;
            }

            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                return ReconstructPath(currentNode, start);
            }

            foreach (Node neighbor in currentNode.connections)
            {
                if (neighbor == null) continue;

                float tentativeG = currentNode.gScore + Vector3.Distance(currentNode.transform.position, neighbor.transform.position);

                if (tentativeG < neighbor.gScore)
                {
                    neighbor.cameFrom = currentNode;
                    neighbor.gScore = tentativeG;
                    neighbor.hScore = Vector3.Distance(neighbor.transform.position, end.transform.position);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // No path found
    }

    private List<Node> ReconstructPath(Node end, Node start)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != null && current != start)
        {
            path.Insert(0, current);
            current = current.cameFrom;
        }

        if (start != null)
            path.Insert(0, start);

        return path;
    }

    public Node FindNearestNode(Vector3 position)
    {
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        Node closest = null;
        float minDist = float.MaxValue;

        foreach (Node node in allNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    public Node FindFurthestNode(Vector3 position)
    {
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        Node furthest = null;
        float maxDist = 0f;

        foreach (Node node in allNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist > maxDist)
            {
                maxDist = dist;
                furthest = node;
            }
        }

        return furthest;
    }

    public Node[] AllNodes()
    {
        return FindObjectsByType<Node>(FindObjectsSortMode.None);
    }
}
