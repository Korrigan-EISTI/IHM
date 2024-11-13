using UnityEngine;

public class BluePrintCameraMouvements : MonoBehaviour
{
    [SerializeField] private Camera bluePrintCamera;

    public static Vector3 sceneCenter = Vector3.zero;  // Centre autour duquel regarder
    public float minZoomSize = 2f;  // Taille minimale pour le zoom
    public float maxZoomSize = 20f;  // Taille maximale pour le zoom
    public float zoomSpeed = 5f;  // Vitesse de zoom

    private void Start()
    {
        // Positionne la caméra sur `sceneCenter` et regarde ce point
        UpdateCameraPosition();
    }

    private void Update()
    {
        // Vérifie que `bluePrintCamera` est active avant d'effectuer le zoom
        if (bluePrintCamera.enabled)
        {
            ZoomWithScroll();
        }
    }

    private void ZoomWithScroll()
    {
        // Capture le défilement de la molette pour ajuster la taille orthographique
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        bluePrintCamera.orthographicSize -= scrollInput * zoomSpeed;
        bluePrintCamera.orthographicSize = Mathf.Clamp(bluePrintCamera.orthographicSize, minZoomSize, maxZoomSize);
    }

    private void UpdateCameraPosition()
    {
        // Place la caméra sur `sceneCenter` sans changement de position pour le zoom
        bluePrintCamera.transform.position = new Vector3(sceneCenter.x, sceneCenter.y + bluePrintCamera.transform.position.y, bluePrintCamera.transform.position.z);

        // Oriente la caméra pour qu'elle regarde `sceneCenter`
        bluePrintCamera.transform.LookAt(sceneCenter);
    }
}
