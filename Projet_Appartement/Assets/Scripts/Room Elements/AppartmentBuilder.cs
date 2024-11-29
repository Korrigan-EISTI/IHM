using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.UI;

public enum ConstructionMode
{
    Apartment,
    Room
}

public class ApartmentManager : MonoBehaviour
{
    public GameObject wallPrefab; // Assign in Unity Editor
    public List<Room> rooms = new List<Room>();
    private Room currentRoom;
    private bool isCreatingRoom = false; // Booléen pour indiquer si une pièce est en cours de création
    private int nbWallClicked = 0; // Booléen pour indiquer si une pièce est en cours de création
    public Transform apartmentParent;
    public ConstructionMode mode = ConstructionMode.Apartment;

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
        Debug.Log($"Initializing a new {mode}...");
        currentRoom = new Room(wallPrefab);
        rooms.Add(currentRoom);
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = GetMouseWorldPosition();

            if (mode == ConstructionMode.Apartment)
            {
                if (currentRoom == null)
                {
                    InitializeRoom();
                }

                currentRoom.AddCorner(mousePosition, rooms, null);

                if (currentRoom.corners.Count > 2 && IsNearFirstCorner(mousePosition, currentRoom))
                {
                    Debug.Log("Apartment contour closed!");
                    CompleteApartment();
                }
            }
            else if (mode == ConstructionMode.Room)
            {
                if (!isCreatingRoom)
                {
                    // Démarrer la création d'une nouvelle pièce
                    isCreatingRoom = true;
                    InitializeRoom();
                    Debug.Log("Room creation started.");
                }
                if (isCreatingRoom)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                    {
                        Wall clickedWall = hit.collider.GetComponent<Wall>();
                        if (nbWallClicked > 0)
                            currentRoom.AddCorner(hit.point, rooms, null);
                        if (clickedWall != null)
                        {
                            if (nbWallClicked <= 0)
                                currentRoom.AddCorner(hit.point, rooms, null);
                            nbWallClicked++;

                            if (currentRoom.corners.Count >= 2 && nbWallClicked >= 2)
                            {
                                Debug.Log("Room closed. Finalizing room creation.");

                                Room parentRoom = FindRoomContainingWall(clickedWall);
                                if (parentRoom != null)
                                {
                                    FinalizeRoomWithIntersection(parentRoom, currentRoom);
                                }

                                InitializeRoom(); // Préparer la prochaine pièce
                                isCreatingRoom = false; // Terminer l'état de création
                            }
                        }
                        else
                        {
                            Debug.Log("Clicked object is not a wall. Ignoring input.");
                        }
                    }
                }
            }
        }
    }

    private void CompleteApartment()
    {
        Debug.Log("Apartment outline completed. Switching to Room mode...");
        CreateApartmentParent();
        mode = ConstructionMode.Room;
        InitializeRoom();
    }

    private void CreateApartmentParent()
    {
        if (apartmentParent == null)
        {
            apartmentParent = new GameObject("Apartment").transform;
        }

        foreach (var room in rooms)
        {
            foreach (var wall in room.walls)
            {
                wall.transform.SetParent(apartmentParent);
            }
        }
        Debug.Log("Apartment parent created and walls grouped.");
    }

    private void FinalizeRoomWithIntersection(Room parentRoom, Room newRoom)
    { 
        Vector3 firstCorner = newRoom.corners[0];
        Vector3 lastCorner = newRoom.corners[newRoom.corners.Count - 1];

        (Vector3 start1, Vector3 end1) = FindSegmentAssociatedWithPoint(parentRoom, firstCorner);
        (Vector3 start2, Vector3 end2) = FindSegmentAssociatedWithPoint(parentRoom, lastCorner);

        Vector3 intersectionPoint = CalculateIntersection(start1, end1, start2, end2);

        if (intersectionPoint != Vector3.zero)
        {
            newRoom.AddCorner(intersectionPoint, rooms, parentRoom);

            foreach (Vector3 corner in newRoom.corners)
            {
                Vector3 nextCorner = newRoom.GetNextCorner(corner);
                if (nextCorner != Vector3.zero)
                {
                    newRoom.AddWall(corner, nextCorner);
                }
            }

            Room remainingRoom = CreateRemainingRoom(parentRoom, newRoom);
            rooms.Remove(parentRoom);
            rooms.Add(newRoom);
            rooms.Add(remainingRoom);

            parentRoom.DestroyAllWalls();

            OrganizeRoomHierarchy(newRoom);
            OrganizeRoomHierarchy(remainingRoom);

            Debug.Log("Room finalized with intersection point.");
        }
        else
        {
            Debug.LogError("No valid intersection found. Aborting room creation.");
        }
    }

    private (Vector3 start, Vector3 end) FindSegmentAssociatedWithPoint(Room room, Vector3 point)
    {
        foreach (var wall in room.walls)
        {
            if (wall.IsPointOnWall(point))
            {
                return (wall.startPoint, wall.endPoint);
            }
        }
        return (Vector3.zero, Vector3.zero);
    }

    private Vector3 CalculateIntersection(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
    {
        Vector3 direction1 = end1 - start1;
        Vector3 direction2 = end2 - start2;

        float a1 = direction1.z;
        float b1 = -direction1.x;
        float c1 = a1 * start1.x + b1 * start1.z;

        float a2 = direction2.z;
        float b2 = -direction2.x;
        float c2 = a2 * start2.x + b2 * start2.z;

        float determinant = a1 * b2 - a2 * b1;

        if (Mathf.Abs(determinant) < 0.0001f)
        {
            return Vector3.zero;
        }

        float intersectX = (b2 * c1 - b1 * c2) / determinant;
        float intersectZ = (a1 * c2 - a2 * c1) / determinant;

        return new Vector3(intersectX, 0, intersectZ);
    }

    private Room CreateRemainingRoom(Room parentRoom, Room newRoom)
    {
        Room remainingRoom = new Room(wallPrefab);

        foreach (var wall in parentRoom.walls)
        {
            bool belongsToNewRoom = false;

            foreach (var corner in newRoom.corners)
            {
                if (wall.IsPointOnWall(corner))
                {
                    belongsToNewRoom = true;
                    break;
                }
            }

            if (!belongsToNewRoom)
            {
                remainingRoom.AddWall(wall.startPoint, wall.endPoint);
            }
        }
        return remainingRoom;
    }

    private bool IsNearFirstCorner(Vector3 point, Room room)
    {
        return Vector3.Distance(point, room.corners[0]) < 0.8f;
    }

    private Room FindRoomContainingWall(Wall wall)
    {
        foreach (var room in rooms)
        {
            if (room.walls.Contains(wall))
            {
                return room;
            }
        }
        return null;
    }
    private void OrganizeRoomHierarchy(Room room)
    {
        // Vérifie si un GameObject parent existe déjà pour cette pièce
        if (room.roomParent == null)
        {
            // Crée un nouveau GameObject pour la pièce
            GameObject roomObject = new GameObject($"Room_{rooms.IndexOf(room)}");
            room.roomParent = roomObject.transform;

            // Place ce parent dans l'arbre de l'appartement
            if (apartmentParent == null)
            {
                apartmentParent = new GameObject("Apartment").transform;
            }
            room.roomParent.SetParent(apartmentParent);
        }

        // Regroupe les murs de la pièce sous le parent
        foreach (var wall in room.walls)
        {
            wall.transform.SetParent(room.roomParent);
        }

        Debug.Log($"Room hierarchy updated for Room_{rooms.IndexOf(room)}");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
