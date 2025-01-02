using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private GameObject _canvasObject; // Canvas dynamique

    public Camera orbitalCamera; // Caméra principale à suivre

    public GameObject buttonPrefab; // Prefab du bouton à ajouter au canvas
    private GameObject _translationGizmos;
    ArrowInteraction _currentArrowInteraction;

    private void Update()
    {
        HandleZoom();
        HandleRotation();
        HandleMovement();
        HandleClickToSetTarget();
        HandleDeselectTarget();
        UpdateTextOrientation();
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            distance -= scrollInput * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Bouton droit de la souris
        {
            _currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            _currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            _currentPitch = Mathf.Clamp(_currentPitch, rotationLimits.x, rotationLimits.y);
        }

        UpdateCameraPosition();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Q/D ou A/D
        float vertical = Input.GetAxis("Vertical"); // Z/S ou W/S

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        transform.Translate(direction * (moveSpeed * Time.deltaTime), Space.Self);
    }

    private void HandleClickToSetTarget()
    {
        if (Input.GetMouseButtonDown(0)) // Bouton gauche de la souris
        {
            Ray ray = orbitalCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Vérifier si l'utilisateur a cliqué sur une flèche
                if (hit.collider != null && hit.collider.gameObject.name.Contains("Arrow"))
                {
                    // Obtenir la direction associée à la flèche
                    ArrowInteraction arrowInteraction = hit.collider.GetComponent<ArrowInteraction>();
                    if (arrowInteraction != null && target != null)
                    {
                        _currentArrowInteraction = arrowInteraction;
                        _currentArrowInteraction.isDragging = true;
                        _currentArrowInteraction.dragStartPosition = _currentArrowInteraction.GetMouseWorldPosition();
                    }

                    return; // Éviter tout autre comportement après avoir cliqué sur une flèche
                }

                // Sélectionner un mur
                if (hit.collider.name.Contains("wall"))
                {
                    SetTarget(hit.collider.gameObject.transform);
                    OnAddWindowClicked();
                }

                // Sélectionner une fenêtre
                if (hit.collider.transform.parent != null && hit.collider.transform.parent.name.Contains("WindowPrefab"))
                {
                    SetTarget(hit.collider.transform.parent);

                    if (_translationGizmos)
                        Destroy(_translationGizmos);

                    // Créer les gizmos
                    _translationGizmos = new GameObject("TranslationGizmos");
                    HoleManipulator manipulator = _translationGizmos.AddComponent<HoleManipulator>();

                    // Passer les sous-objets du trou au HoleManipulator
                    manipulator.leftWall = target.Find("LeftWall");
                    manipulator.rightWall = target.Find("RightWall");
                    manipulator.topWall = target.Find("TopWall");
                    manipulator.bottomWall = target.Find("BottomWall");
                    manipulator.oldWall = target.Find("OldWall").GetComponent<Wall>();

                    // Positionner les flèches au centre du trou
                    _translationGizmos.transform.position = target.position;
                }
                else
                {
                    ClearTarget();
                }
            }
        }
    }


    private void HandleDeselectTarget()
    {
        if (Input.GetKeyDown(deselectKey))
        {
            ClearTarget();
        }
    }

    private void UpdateCameraPosition()
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

    private void SetTarget(Transform newTarget)
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

            // Créer un Canvas attaché au mur
            CreateCanvasOnWall(target);
        }
    }

    public void ClearTarget()
    {
        if (target)
        {
            Outline outline = target.GetComponent<Outline>();
            if (outline)
            {
                Destroy(outline);
            }
        }

        if (_translationGizmos)
        {
            Destroy(_translationGizmos);
        }

        if (_dimensionText)
        {
            Destroy(_dimensionText);
        }

        if (_canvasObject)
        {
            Destroy(_canvasObject);
        }

        _currentTargetRenderer = null;
        target = null;
    }


    private void ShowWallDimensions(Transform wall)
    {
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

    private void CreateCanvasOnWall(Transform wall)
    {
        if (_canvasObject != null)
        {
            Destroy(_canvasObject);
        }

        // Créer un Canvas attaché au mur
        _canvasObject = new GameObject("WallCanvas");
        _canvasObject.transform.SetParent(wall);
        _canvasObject.transform.localPosition = Vector3.up * 2; // Positionner au-dessus du mur

        Canvas canvas = _canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = _canvasObject.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        // Ajouter un bouton au Canvas
        if (buttonPrefab)
        {
            GameObject button = Instantiate(buttonPrefab, _canvasObject.transform);
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 100); // Taille du bouton
            rectTransform.localPosition = Vector3.zero; // Centrer dans le Canvas

            Button uiButton = button.GetComponent<Button>();
            if (uiButton != null)
            {
                uiButton.onClick.AddListener(() => OnAddWindowClicked());
            }
        }
    }

    private void OnAddWindowClicked()
    {
        if (target)
        {
            Wall wallScript = target.GetComponent<Wall>();
            if (wallScript != null)
            {
                // Définir les paramètres de la fenêtre
                Vector3 wallCenter = target.position; // Centre du mur
                float windowWidth = 1.0f; // Largeur fixe pour la fenêtre (modifiable)

                // Ajouter une fenêtre au mur
                wallScript.AddWindow(wallCenter, windowWidth);
            }
            else
            {
                Debug.LogError("Le script 'Wall' est manquant sur le mur sélectionné.");
            }
        }
    }


    private void UpdateTextOrientation()
    {
        if (_dimensionText && orbitalCamera)
        {
            _dimensionText.transform.LookAt(orbitalCamera.transform);
            _dimensionText.transform.rotation = Quaternion.LookRotation(orbitalCamera.transform.forward);
            _dimensionText.transform.localScale = Vector3.one * 0.1f;
        }
    }
}
