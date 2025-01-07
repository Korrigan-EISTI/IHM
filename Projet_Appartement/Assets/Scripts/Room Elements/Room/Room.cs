using UnityEngine;
using System.Collections.Generic;

public class Room
{
    public List<Wall> walls = new List<Wall>();
    public List<Vector3> corners = new List<Vector3>();
    public Transform roomParent;
    private GameObject wallPrefab;

    public Room(GameObject wallPrefab)
    {
        this.wallPrefab = wallPrefab;
    }

    /// <summary>
    /// Ajoute un coin et crée un mur si nécessaire.
    /// </summary>
    public void AddCorner(Vector3 position, List<Room> allRooms, Room apartmentRoom)
    {
        corners.Add(position);

        if (corners.Count > 1)
        {
            // Crée un mur entre les deux derniers coins
            Vector3 start = corners[corners.Count - 2];
            Vector3 end = corners[corners.Count - 1];
            AddWall(start, end);
        }
    }

    /// <summary>
    /// Ajoute un mur entre deux points.
    /// </summary>
    public void AddWall(Vector3 start, Vector3 end)
    {
        GameObject wallObject = Object.Instantiate(wallPrefab);
        Wall wall = wallObject.GetComponent<Wall>();

        if (wall != null)
        {
            wall.Initialize(start, end);
            walls.Add(wall);
        }
        else
        {
            Debug.LogError("Wall prefab does not contain a Wall component.");
        }
    }

    /// <summary>
    /// Supprime un mur spécifique de la pièce.
    /// </summary>
    public void RemoveWall((Vector3 start, Vector3 end) wallSegment)
    {
        Wall wallToRemove = null;

        foreach (var wall in walls)
        {
            if (wall.startPoint == wallSegment.start && wall.endPoint == wallSegment.end)
            {
                wallToRemove = wall;
                break;
            }
        }

        if (wallToRemove != null)
        {
            walls.Remove(wallToRemove);
            Object.Destroy(wallToRemove.gameObject);
        }
        else
        {
            Debug.LogWarning("Wall segment not found in this room.");
        }
    }

    /// <summary>
    /// Vérifie si la pièce est fermée.
    /// </summary>
    public bool IsClosed()
    {
        return corners.Count > 2 && Vector3.Distance(corners[corners.Count - 1], corners[0]) < 0.25;
    }

    public Vector3 GetNextCorner(Vector3 currentCorner)
    {
        int index = corners.IndexOf(currentCorner);
        if (index == -1)
        {
            Debug.LogError($"Corner {currentCorner} not found in the room.");
            return Vector3.zero; // Retourne un vecteur neutre si le coin n'est pas trouvé
        }

        // Retourne le coin suivant, ou le premier coin si on boucle
        return corners[(index + 1) % corners.Count];
    }

    public void DestroyAllWalls()
    {
        foreach (Wall wall in walls)
        {
            Object.Destroy(wall.gameObject);
        }
    }

}
