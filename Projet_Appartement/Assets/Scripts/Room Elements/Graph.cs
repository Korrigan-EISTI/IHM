using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Node> Nodes = new List<Node>();

    public Node AddNode(Vector3 position)
    {
        Node existingNode = Nodes.Find(node => Vector3.Distance(node.Position, position) < 0.1f);
        if (existingNode != null)
            return existingNode;

        Node newNode = new Node(position);
        Nodes.Add(newNode);
        return newNode;
    }

    public void AddEdge(Node nodeA, Node nodeB, GameObject wall)
    {
        nodeA.AddEdge(nodeB, wall);
        nodeB.AddEdge(nodeA, wall);
    }

    // Détection des cycles dans le graphe
    public List<List<Node>> DetectCycles()
    {
        List<List<Node>> cycles = new List<List<Node>>();
        HashSet<Node> visited = new HashSet<Node>();

        foreach (Node node in Nodes)
        {
            if (!visited.Contains(node))
            {
                List<Node> cycle = new List<Node>();
                if (FindCycle(node, null, cycle, visited))
                {
                    // Enregistrer les cycles fermés
                    if (IsCycleClosed(cycle))
                    {
                        cycles.Add(new List<Node>(cycle));
                    }
                }
            }
        }

        return cycles;
    }

    private bool FindCycle(Node current, Node parent, List<Node> cycle, HashSet<Node> visited)
    {
        visited.Add(current);
        cycle.Add(current);

        foreach (Edge edge in current.Edges)
        {
            Node next = edge.ConnectedNode;

            if (next == parent) continue;

            if (visited.Contains(next))
            {
                // Cycle détecté
                cycle.Add(next);
                return true;
            }

            if (FindCycle(next, current, cycle, visited))
                return true;
        }

        cycle.Remove(current);
        return false;
    }

    private bool IsCycleClosed(List<Node> cycle)
    {
        // Vérifie si le cycle est fermé (premier et dernier nœuds identiques)
        if (cycle.Count < 3) return false;
        return cycle[0] == cycle[cycle.Count - 1];
    }
}

public class Node
{
    public Vector3 Position { get; private set; }
    public List<Edge> Edges = new List<Edge>();

    public Node(Vector3 position)
    {
        Position = position;
    }

    public void AddEdge(Node connectedNode, GameObject wall)
    {
        if (!Edges.Exists(edge => edge.ConnectedNode == connectedNode))
        {
            Edges.Add(new Edge(connectedNode, wall));
        }
    }
}

public class Edge
{
    public Node ConnectedNode { get; private set; }
    public GameObject Wall { get; private set; }

    public Edge(Node connectedNode, GameObject wall)
    {
        ConnectedNode = connectedNode;
        Wall = wall;
    }
}

