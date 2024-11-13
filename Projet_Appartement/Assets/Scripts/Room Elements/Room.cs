using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room
{
    public List<Vector3> Corners { get; private set; }
    public GameObject RoomObject { get; private set; }
    private GameObject floorObject;
    private Mesh floorMesh;
    private float wallWidth;
    private float wallHeight;
    private GameObject wallPrefab;
    private Material floorMaterial;

    public Room(float wallWidth, float wallHeight, GameObject wallPrefab, Material floorMaterial, string roomName = "Room")
    {
        this.wallWidth = wallWidth;
        this.wallHeight = wallHeight;
        this.wallPrefab = wallPrefab;
        this.floorMaterial = floorMaterial;

        // Initialisation de la liste des coins de la pièce et du GameObject parent de la pièce
        Corners = new List<Vector3>();
        RoomObject = new GameObject(roomName);

        // Création de l'objet du sol et configuration du Mesh
        floorObject = new GameObject("Floor");
        floorObject.transform.SetParent(RoomObject.transform);
        MeshRenderer renderer = floorObject.AddComponent<MeshRenderer>();
        renderer.material = floorMaterial;
        MeshFilter filter = floorObject.AddComponent<MeshFilter>();
        floorMesh = new Mesh();
        filter.mesh = floorMesh;
    }

    public void AddCorner(Vector3 corner)
    {
        Corners.Add(corner);
    }

    public void CompleteRoom()
    {
        if (Corners.Count < 3) return;

        // Fermer la pièce en reliant le dernier point au premier
        Corners.Add(Corners[0]);
        CreateWalls();
        UpdateFloorMesh();
    }

    private void CreateWalls()
    {
        for (int i = 0; i < Corners.Count - 1; i++)
        {
            CreateWallBetweenPoints(Corners[i], Corners[i + 1]);
        }
    }

    private void CreateWallBetweenPoints(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 wallPosition = (startPoint + endPoint) / 2;
        float wallLength = Vector3.Distance(startPoint, endPoint);

        GameObject wall = GameObject.Instantiate(wallPrefab, wallPosition, Quaternion.identity, RoomObject.transform);
        wall.transform.localScale = new Vector3(wallWidth, wallHeight, wallLength);
        wall.transform.LookAt(endPoint);
    }

    private void UpdateFloorMesh()
    {
        Vector3[] vertices = Corners.ToArray();
        int[] triangles = new int[(Corners.Count - 2) * 3];

        for (int i = 0; i < Corners.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        floorMesh.Clear();
        floorMesh.vertices = vertices;
        floorMesh.triangles = triangles;
        floorMesh.RecalculateNormals();
    }
}
