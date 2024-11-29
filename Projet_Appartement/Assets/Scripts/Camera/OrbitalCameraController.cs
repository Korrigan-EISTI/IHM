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

    private float _currentYaw = 0.0f; // Angle horizontal
    private float _currentPitch = 0.0f; // Angle vertical

    private Renderer _currentTargetRenderer; // Renderer de la cible
    private GameObject _dimensionText; // GameObject pour afficher les dimensions

    public Camera orbitalCamera; // Caméra principale à suivre

    void Update()
    {
        HandleZoom();
        HandleRotation();
        HandleMovement();
        HandleClickToSetTarget();
        HandleDeselectTarget();
        UpdateTextOrientation();
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            distance -= scrollInput * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Bouton droit de la souris
        {
            _currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            _currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            _currentPitch = Mathf.Clamp(_currentPitch, rotationLimits.x, rotationLimits.y);
        }

        UpdateCameraPosition();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Q/D ou A/D
        float vertical = Input.GetAxis("Vertical"); // Z/S ou W/S

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        transform.Translate(direction * (moveSpeed * Time.deltaTime), Space.Self);
    }

    void HandleClickToSetTarget()
    {
        if (Input.GetMouseButtonDown(0)) // Bouton gauche de la souris
        {
            Ray ray = orbitalCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.name.Contains("wall"))
                {
                    SetTarget(hit.collider.gameObject.transform);
                }
            }
            else
            {
                ClearTarget();
            }
        }
    }

    void HandleDeselectTarget()
    {
        if (Input.GetKeyDown(deselectKey))
        {
            ClearTarget();
        }
    }

    void UpdateCameraPosition()
    {
        if (target)
        {
            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
            Vector3 offset = rotation * new Vector3(0, 0, -distance);

            transform.position = target.position + offset;
            transform.LookAt(target.position);
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
            transform.rotation = rotation;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void SetTarget(Transform newTarget)
    {
        if (newTarget != target)
        {
            ClearTarget();
            target = newTarget;

            Collider collider = target.GetComponent<Collider>();
            if (collider != null)
            {
                target.position = collider.bounds.center;
            }

            Outline outline = target.GetComponent<Outline>();
            if (!outline)
            {
                outline = target.gameObject.AddComponent<Outline>();
            }

            outline.OutlineColor = new Color(1, 0.5f, 0);
            outline.OutlineWidth = 7f;
            outline.OutlineMode = Outline.Mode.OutlineAll;

            _currentTargetRenderer = target.GetComponent<Renderer>();

            // Afficher les dimensions
            ShowWallDimensions(target);
        }
    }

    public void ClearTarget()
    {
        if (target)
        {
            Outline outline = target.GetComponent<Outline>();
            if (outline)
            {
                Destroy(outline); // Supprimer l'effet de contour
            }
        }

        if (_dimensionText)
        {
            Destroy(_dimensionText); // Supprimer le texte affiché
        }

        _currentTargetRenderer = null;
        target = null;
    }


    // ReSharper disable Unity.PerformanceAnalysis
    void ShowWallDimensions(Transform wall)
    {
        // Création d'un texte 3D pour afficher les dimensions
        if (_dimensionText != null)
        {
            Destroy(_dimensionText);
        }

        _dimensionText = new GameObject("WallDimensions");
        TextMesh textMesh = _dimensionText.AddComponent<TextMesh>();
        textMesh.fontSize = 24;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;

        Collider collider = wall.GetComponent<Collider>();
        if (collider)
        {
            Vector3 size = collider.bounds.size;
            textMesh.text = $"Width: {size.x:F2}\nHeight: {size.y:F2}\nDepth: {size.z:F2}";
        }
        else
        {
            textMesh.text = "Dimensions inconnues";
        }

        textMesh.color = Color.black;

        // Positionner le texte au-dessus du mur
        _dimensionText.transform.position = wall.position + Vector3.up * 2;
        _dimensionText.transform.localScale = Vector3.one * 0.1f;
    }

    void UpdateTextOrientation()
    {
        if (_dimensionText && orbitalCamera)
        {
            // Faire en sorte que le texte regarde la caméra
            _dimensionText.transform.LookAt(orbitalCamera.transform);

            // Inverser l'axe pour éviter une orientation inversée
            _dimensionText.transform.rotation = Quaternion.LookRotation(orbitalCamera.transform.forward);

            // Assurer une échelle uniforme
            _dimensionText.transform.localScale = Vector3.one * 0.1f;
        }
    }
}
