using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    [SerializeField] private float wallWidth = 0.15f;
    [SerializeField] private float wallHeight = 2.5f;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private Camera blueprintCamera;
    [SerializeField] private LineRenderer previewLine;
    [SerializeField] private Material floorMaterial; // Matériau pour le sol

    private int roomCounter = 1;
    private Room currentRoom;
    private bool isRoomComplete = false;

    private void Start()
    {
        Camera.SetupCurrent(blueprintCamera);
        if (blueprintCamera == null || wallPrefab == null || previewLine == null || floorMaterial == null)
        {
            Debug.LogError("Veuillez assigner tous les éléments nécessaires (caméra, prefab de mur, LineRenderer, matériau du sol).");
            return;
        }

        previewLine.transform.position = Vector3.zero;
        StartNewRoom();
    }

    private void Update()
    {
        if (isRoomComplete || Camera.current == null || Camera.current.name != blueprintCamera.name) return;

        DrawPreviewLine();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = blueprintCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1500))
            {
                Vector3 worldPosition = hit.point;
                worldPosition.y = 1.26f;

                if (currentRoom.Corners.Count > 0 && Vector3.Distance(worldPosition, currentRoom.Corners[0]) < 0.5f)
                {
                    isRoomComplete = true;
                    currentRoom.CompleteRoom();
                    previewLine.enabled = false;
                }
                else
                {
                    currentRoom.AddCorner(worldPosition);
                }
            }
        }
    }

    public void StartNewRoom()
    {
        previewLine.enabled = true;
        currentRoom = new Room(wallWidth, wallHeight, wallPrefab, floorMaterial, $"Room {roomCounter}");
        isRoomComplete = false;
        roomCounter++;
    }

    private void DrawPreviewLine()
    {
        if (currentRoom.Corners.Count <= 0) return;

        previewLine.enabled = true;
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = blueprintCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1500))
        {
            Vector3 worldPosition = hit.point;
            worldPosition.y = 0;

            previewLine.positionCount = currentRoom.Corners.Count + 1;

            for (int i = 0; i < currentRoom.Corners.Count; i++)
            {
                previewLine.SetPosition(i, currentRoom.Corners[i]);
            }

            previewLine.SetPosition(currentRoom.Corners.Count, worldPosition);
        }
    }
}