using System.Collections.Generic;
using UnityEngine;

public class EpicNodeMaker : MonoBehaviour
{
    public Node nodePrefab;
    public List<Node> nodeList = new List<Node>();

    [ContextMenu("Create Nodes")]
    public void MakeNodes()
    {
        nodeList.Clear(); // Clear existing nodes if any

        // Create nodes in a grid on the XZ plane at y=0
        for (int x = -12; x < 12; x += 2)
        {
            for (int z = -2; z < 10; z += 2)
            {
                Vector3 position = new Vector3(x, 0f, z);
                Node n = Instantiate(nodePrefab, position, Quaternion.identity);
                nodeList.Add(n);
            }
        }
    }

    [ContextMenu("Remove Empty Nodes")]
    public void RemoveNodes()
    {
        // Remove null nodes safely
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i] == null)
            {
                nodeList.RemoveAt(i);
                i--;
            }
        }
    }

    [ContextMenu("Connect Nodes")]
    public void ConnectNodes()
    {
        // Connect nodes within a certain distance (2.1 units) bidirectionally
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = i + 1; j < nodeList.Count; j++)
            {
                if (Vector3.Distance(nodeList[i].transform.position, nodeList[j].transform.position) <= 2.1f)
                {
                    nodeList[i].connections.Add(nodeList[j]);
                    nodeList[j].connections.Add(nodeList[i]);
                }
            }
        }
    }
}
