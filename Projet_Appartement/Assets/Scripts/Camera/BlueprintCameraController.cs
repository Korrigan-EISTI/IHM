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
        HandlePanSpeedControl();
    }

    private void HandleZoom()
    {
        // Gestion du zoom avec la molette de la souris
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 & !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            cam.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    private void HandlePan()
    {
        // Gestion de la translation (pan) sur les axes X et Z avec clic droit
        if (Input.GetKey(panKey)) // Clic droit pour activer la translation
        {
            float horizontal = -Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

            // Déplacer la caméra sur le plan XZ avec la souris
            transform.Translate(new Vector3(horizontal, 0, vertical), Space.World);
        }

        // Gestion de la translation (pan) sur les axes X et Z avec touches ZQSD
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        // Détection des touches ZQSD
        if (Input.GetKey(KeyCode.W)) // Avancer (axe Z+)
        {
            moveVertical += panSpeed/2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) // Reculer (axe Z-)
        {
            moveVertical -= panSpeed/2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) // Aller à gauche (axe X-)
        {
            moveHorizontal -= panSpeed/2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) // Aller à droite (axe X+)
        {
            moveHorizontal += panSpeed/2 * Time.deltaTime;
        }

        // Détection des flèches directionnelles
        if (Input.GetKey(KeyCode.UpArrow)) // Flèche haut (axe Z+)
        {
            moveVertical += panSpeed/2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow)) // Flèche bas (axe Z-)
        {
            moveVertical -= panSpeed/2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) // Flèche gauche (axe X-)
        {
            moveHorizontal -= panSpeed/2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow)) // Flèche droite (axe X+)
        {
            moveHorizontal += panSpeed/2 * Time.deltaTime;
        }

        // Appliquer la translation sur les axes X et Z
        transform.Translate(new Vector3(moveHorizontal, 0, moveVertical), Space.World);
    }
    
    private void HandlePanSpeedControl()
    {
        // Si la touche Ctrl est enfoncée
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // Récupère l'entrée de la molette de la souris
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            // Ajuste la vitesse de panSpeed en fonction de la molette
            if (scroll != 0)
            {
                panSpeed += scroll * 10f; // Ajustez le multiplicateur si nécessaire

                // Limite la vitesse pour éviter des valeurs trop basses ou trop élevées
                panSpeed = Mathf.Clamp(panSpeed, 1f, 100f);

                Debug.Log($"Nouvelle vitesse de pan : {panSpeed}");
            }
        }
    }

}