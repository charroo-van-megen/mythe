using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public int maxHealth = 100;
    public int curHealth;
    public int panicMultiplier = 1;

    public Node currentNode;
    public List<Node> path = new List<Node>();

    public enum StateMachine
    {
        Patrol,
        Engage,
        Evade
    }

    public StateMachine currentState;

    public PlayerController player;
    public float speed = 3f;

    private void Start()
    {
        curHealth = maxHealth;

        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogWarning("NPC_Controller: No PlayerController found in scene.");
            }
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        HandleState();
        MoveAlongPath();
    }

    void HandleState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > 5.0f && currentState != StateMachine.Patrol && curHealth > maxHealth * 0.2f)
        {
            currentState = StateMachine.Patrol;
            path.Clear();
        }
        else if (distanceToPlayer <= 5.0f && currentState != StateMachine.Engage && curHealth > maxHealth * 0.2f)
        {
            currentState = StateMachine.Engage;
            path.Clear();
        }
        else if (currentState != StateMachine.Evade && curHealth <= maxHealth * 0.2f)
        {
            panicMultiplier = 2;
            currentState = StateMachine.Evade;
            path.Clear();
        }

        switch (currentState)
        {
            case StateMachine.Patrol:
                Patrol();
                break;
            case StateMachine.Engage:
                Engage();
                break;
            case StateMachine.Evade:
                Evade();
                break;
        }
    }

    void Patrol()
    {
        if (path.Count == 0 && currentNode != null)
        {
            Node[] allNodes = AStarManager.instance.AllNodes();
            if (allNodes.Length > 0)
            {
                Node randomTarget = allNodes[Random.Range(0, allNodes.Length)];
                path = SafeGeneratePath(currentNode, randomTarget);
            }
        }
    }

    void Engage()
    {
        if (path.Count == 0 && currentNode != null)
        {
            Node target = AStarManager.instance.FindNearestNode(player.transform.position);
            path = SafeGeneratePath(currentNode, target);
        }
    }

    void Evade()
    {
        if (path.Count == 0 && currentNode != null)
        {
            Node target = AStarManager.instance.FindFurthestNode(player.transform.position);
            path = SafeGeneratePath(currentNode, target);
        }
    }

    void MoveAlongPath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            Vector3 targetPos = new Vector3(path[x].transform.position.x, transform.position.y, path[x].transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * panicMultiplier * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
    }

    List<Node> SafeGeneratePath(Node start, Node end)
    {
        if (start == null || end == null)
        {
            Debug.LogWarning("Start or End node is null in GeneratePath.");
            return new List<Node>();
        }

        return AStarManager.instance.GeneratePath(start, end) ?? new List<Node>();
    }
}
