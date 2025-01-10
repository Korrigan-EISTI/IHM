using UnityEngine;
using System.Collections.Generic;

public class ApartmentManager : MonoBehaviour
{
    public GameObject wallPrefab;
    public LineRenderer lineRenderer;
    public Material floorMaterial;
    private Room currentRoom;
    public Transform apartmentParent;
    private GameObject apartmentObject;
    private float y = 1f;

    void Start()
    {
        InitializeRoom();
    }

    void Update()
    {
        HandleMouseInput();
    }

    private void InitializeRoom()
    {
        Debug.Log("Initializing a new room...");
        currentRoom = new Room(wallPrefab);
        ActivateLineRenderer();
    }

    private void ActivateLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true; // Enable the LineRenderer
            lineRenderer.positionCount = 0; // Reset position count
        }
    }

    private void DeactivateLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Disable the LineRenderer
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && Camera.current.name == "BlueprintCamera" && apartmentObject == null)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            mousePosition = SnapToGrid(mousePosition, 0.25f);
            mousePosition.y = y;

            if (currentRoom == null)
            {
                InitializeRoom();
            }

            // Add the clicked point as a corner
            currentRoom.AddCorner(mousePosition, null, null);
            UpdateLineRenderer();

            if (currentRoom.corners.Count > 2 && IsNearFirstCorner(mousePosition, currentRoom))
            {
                Debug.Log("Room contour closed!");
                CompleteRoom();
            }
        }
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer == null || currentRoom == null)
            return;

        // Update the LineRenderer with the corners
        lineRenderer.positionCount = currentRoom.corners.Count;
        for (int i = 0; i < currentRoom.corners.Count; i++)
        {
            lineRenderer.SetPosition(i, currentRoom.corners[i]);
        }
    }

    private void CompleteRoom()
    {
        Debug.Log("Room outline completed. Grouping walls...");
        CreateRoomParent();
        GenerateFloorMesh();

        // Deactivate LineRenderer for the current room
        DeactivateLineRenderer();
    }

    private void CreateRoomParent()
    {
        apartmentObject = new GameObject("Apartment");
        apartmentParent = apartmentObject.transform;

        foreach (var wall in currentRoom.walls)
        {
            wall.transform.SetParent(apartmentParent);
        }

        Debug.Log("Room parent created and walls grouped.");
    }

    private void GenerateFloorMesh()
    {
        if (currentRoom.corners.Count < 3)
        {
            Debug.LogError("Not enough corners to generate a floor mesh.");
            return;
        }

        GameObject floor = new GameObject("Floor");
        floor.transform.SetParent(apartmentParent);

        MeshFilter meshFilter = floor.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = floor.AddComponent<MeshRenderer>();

        // Assign a basic material
        meshRenderer.material = floorMaterial;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        List<Vector3> vertices = new List<Vector3>();
        foreach (var corner in currentRoom.corners)
        {
            vertices.Add(new Vector3(corner.x, 0f, corner.z));
        }

        int[] triangles = EarClippingTriangulation(vertices);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Debug.Log("Floor mesh generated.");
    }

    private int[] EarClippingTriangulation(List<Vector3> vertices)
    {
        List<int> indices = new List<int>();
        List<int> remainingVertices = new List<int>();
        for (int i = 0; i < vertices.Count; i++) remainingVertices.Add(i);

        while (remainingVertices.Count > 3)
        {
            bool earFound = false;
            for (int i = 0; i < remainingVertices.Count; i++)
            {
                int prev = remainingVertices[(i - 1 + remainingVertices.Count) % remainingVertices.Count];
                int curr = remainingVertices[i];
                int next = remainingVertices[(i + 1) % remainingVertices.Count];

                if (IsEar(vertices, prev, curr, next, remainingVertices))
                {
                    indices.Add(prev);
                    indices.Add(curr);
                    indices.Add(next);
                    remainingVertices.RemoveAt(i);
                    earFound = true;
                    break;
                }
            }

            if (!earFound)
            {
                Debug.LogError("Failed to triangulate: Input might be non-simple or degenerate.");
                break;
            }
        }

        if (remainingVertices.Count == 3)
        {
            indices.Add(remainingVertices[0]);
            indices.Add(remainingVertices[1]);
            indices.Add(remainingVertices[2]);
        }

        return indices.ToArray();
    }

    private bool IsEar(List<Vector3> vertices, int prev, int curr, int next, List<int> remainingVertices)
    {
        Vector3 a = vertices[prev];
        Vector3 b = vertices[curr];
        Vector3 c = vertices[next];

        if (Vector3.Cross(b - a, c - a).y <= 0)
            return false;

        for (int i = 0; i < remainingVertices.Count; i++)
        {
            int testIndex = remainingVertices[i];
            if (testIndex == prev || testIndex == curr || testIndex == next)
                continue;

            if (PointInTriangle(vertices[testIndex], a, b, c))
                return false;
        }

        return true;
    }

    private bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v < 1);
    }

    private bool IsNearFirstCorner(Vector3 point, Room room)
    {
        return Vector3.Distance(point, room.corners[0]) < 0.8f;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private Vector3 SnapToGrid(Vector3 position, float gridSize)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, position.y, z);
    }
}
