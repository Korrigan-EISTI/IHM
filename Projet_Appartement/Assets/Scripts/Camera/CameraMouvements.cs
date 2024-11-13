using UnityEngine;

public class CameraMouvements : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public static Vector3 sceneCenter = Vector3.zero;  // Centre autour duquel tourner
    public float rotationSpeed = 0.5f;  // Sensibilité de la rotation
    public float distanceFromCenter = 10f;  // Distance entre la caméra et le centre
    public float minZoomDistance = 5f;  // Distance minimale de zoom
    public float maxZoomDistance = 20f;  // Distance maximale de zoom
    public float zoomSpeed = 5f;  // Vitesse de zoom

    private float angleX = 30f;  // Angle vertical (élévation)
    private float angleY = 0f;   // Angle horizontal (azimut)

    private void Start()
    {
        // Positionne initialement la caméra principale à une distance fixe du centre
        UpdateCameraPosition();
    }

    private void Update()
    {
        // Vérifie que `mainCamera` est active avant d'effectuer les mouvements et le zoom
        if (mainCamera.enabled)
        {
            RotateWithMouseDrag();
            ZoomWithScroll();
        }
    }

    private void RotateWithMouseDrag()
    {
        if (Input.GetMouseButton(1)) // Bouton droit de la souris pour la rotation
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Ajuste les angles en fonction du mouvement de la souris
            angleY += mouseX * rotationSpeed;
            angleX -= mouseY * rotationSpeed;

            // Limite l'angle vertical pour éviter de tourner complètement autour
            angleX = Mathf.Clamp(angleX, -80f, 80f);

            // Met à jour la position de la caméra principale en fonction des nouveaux angles
            UpdateCameraPosition();
        }
    }

    private void ZoomWithScroll()
    {
        // Capture le défilement de la molette pour ajuster la distance
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distanceFromCenter -= scrollInput * zoomSpeed;
        distanceFromCenter = Mathf.Clamp(distanceFromCenter, minZoomDistance, maxZoomDistance);

        // Met à jour la position de la caméra pour refléter le zoom
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        // Calcule la position de la caméra en utilisant les angles d'Euler et la distance du centre
        Vector3 offset = new Vector3(
            distanceFromCenter * Mathf.Cos(angleX * Mathf.Deg2Rad) * Mathf.Sin(angleY * Mathf.Deg2Rad),
            distanceFromCenter * Mathf.Sin(angleX * Mathf.Deg2Rad),
            distanceFromCenter * Mathf.Cos(angleX * Mathf.Deg2Rad) * Mathf.Cos(angleY * Mathf.Deg2Rad)
        );

        // Place `mainCamera` à la nouvelle position
        mainCamera.transform.position = sceneCenter + offset;

        // Oriente `mainCamera` pour qu'elle regarde toujours `sceneCenter`
        mainCamera.transform.LookAt(sceneCenter);
    }
}
