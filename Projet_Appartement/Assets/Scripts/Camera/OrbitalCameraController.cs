using UnityEngine;

public class OrbitalCameraController : MonoBehaviour
{
    public Transform target; // Cible autour de laquelle orbiter (peut être null)
    public float distance = 10.0f; // Distance par défaut
    public float zoomSpeed = 5.0f; // Vitesse de zoom
    public float minDistance = 5.0f; // Distance minimale
    public float maxDistance = 20.0f; // Distance maximale

    public float rotationSpeed = 300.0f; // Vitesse de rotation
    public Vector2 rotationLimits = new Vector2(-90f, 90f); // Limites de rotation verticale

    public float moveSpeed = 10.0f; // Vitesse de déplacement avec ZQSD
    public KeyCode deselectKey = KeyCode.R; // Touche pour désélectionner la cible

    private float currentYaw = 0.0f; // Angle horizontal
    private float currentPitch = 0.0f; // Angle vertical
    
    private Renderer currentTargetRenderer; // Référence au renderer de la cible
    
    void Update()
    {
        HandleZoom();
        HandleRotation();
        HandleMovement();
        HandleClickToSetTarget();
        HandleDeselectTarget();
    }

    void HandleZoom()
    {
        // Gestion du zoom
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            distance -= scrollInput * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    void HandleRotation()
    {
        // Rotation autour de la cible (ou de la caméra elle-même si aucune cible n'est définie)
        if (Input.GetMouseButton(1)) // Bouton droit de la souris
        {
            currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentPitch = Mathf.Clamp(currentPitch, rotationLimits.x, rotationLimits.y);
        }

        UpdateCameraPosition();
    }

    void HandleMovement()
    {
        // Déplacement avec les touches ZQSD (ou WASD en fonction du clavier)
        float horizontal = Input.GetAxis("Horizontal"); // Q/D ou A/D
        float vertical = Input.GetAxis("Vertical"); // Z/S ou W/S

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
    }

    void HandleClickToSetTarget()
    {
        // Détection du clic gauche pour définir une nouvelle cible
        if (Input.GetMouseButtonDown(0)) // Bouton gauche de la souris
        {
            Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.name.Contains("wall"))
                {
                    // Si un objet est cliqué, il devient la nouvelle cible
                    SetTarget(hit.collider.gameObject.transform);
                }
            }
            else
            {
                // Si aucun objet n'est cliqué, désélectionne la cible
                ClearTarget();
            }
        }
    }

    void HandleDeselectTarget()
    {
        // Désélectionner la cible si la touche spécifiée est pressée
        if (Input.GetKeyDown(deselectKey))
        {
            ClearTarget();
        }
    }

    void UpdateCameraPosition()
    {
        if (target != null)
        {
            // Orbite autour de la cible
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
            Vector3 offset = rotation * new Vector3(0, 0, -distance);

            transform.position = target.position + offset;
            transform.LookAt(target.position);
        }
        else
        {
            // Rotation sur place si aucune cible n'est définie
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
            transform.rotation = rotation;
        }
    }

    void SetTarget(Transform newTarget)
    {
        // Supprimer l'effet de contour de l'ancienne cible
        ClearTarget();

        target = newTarget;

        // Récupérer le centre du collider si présent
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            target.position = collider.bounds.center; // Met le centre du collider comme point de focus
        }

        // Ajouter un effet de contour orange à la nouvelle cible
        currentTargetRenderer = target.GetComponent<Renderer>();
        if (currentTargetRenderer != null)
        {
            currentTargetRenderer.material.SetColor("_OutlineColor", Color.yellow);
            currentTargetRenderer.material.SetFloat("_OutlineWidth", 0.03f); // Exemple de largeur de contour
        }
    }


    void ClearTarget()
    {
        // Supprimer l'effet de contour de la cible actuelle
        if (currentTargetRenderer != null)
        {
            currentTargetRenderer.material.SetFloat("_OutlineWidth", 0f); // Réinitialise le contour
            currentTargetRenderer = null;
        }

        target = null;
    }
    
    public string GetTargetParameters()
    {
        if (target == null)
        {
            return "Aucune cible sélectionnée.";
        }

        string targetName = target.name;
        Vector3 position = target.position;
        Quaternion rotation = target.rotation;

        string dimensions = "Inconnues";
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            dimensions = collider.bounds.size.ToString();
        }

        return $"Nom : {targetName}\n" +
               $"Position : {position}\n" +
               $"Rotation : {rotation.eulerAngles}\n" +
               $"Dimensions (bounds) : {dimensions}";
    }
}
