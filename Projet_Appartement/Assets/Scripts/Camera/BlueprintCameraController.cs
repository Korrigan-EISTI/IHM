using UnityEngine;

public class BlueprintCameraController : MonoBehaviour
{
    public float zoomSpeed = 10.0f; // Vitesse de zoom
    public float minZoom = 5.0f; // Zoom minimal (taille orthographique minimale)
    public float maxZoom = 50.0f; // Zoom maximal (taille orthographique maximale)

    public float panSpeed = 20.0f; // Vitesse de translation (pan)
    public KeyCode panKey = KeyCode.Mouse1; // Touche pour activer la translation (clic droit par défaut)

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Assurez-vous que la caméra est en mode orthographique pour une vue blueprint
        if (!cam.orthographic)
        {
            Debug.LogWarning("La caméra n'est pas en mode orthographique. Le script fonctionne mieux avec une caméra orthographique.");
        }
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    private void HandleZoom()
    {
        // Gestion du zoom avec la molette de la souris
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    private void HandlePan()
    {
        // Gestion de la translation (pan) sur les axes X et Z
        if (Input.GetKey(panKey)) // Clic droit pour activer la translation
        {
            float horizontal = -Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

            // Déplacer la caméra sur le plan XZ
            transform.Translate(new Vector3(horizontal, 0, vertical), Space.World);
        }
    }
}