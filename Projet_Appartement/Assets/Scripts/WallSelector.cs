using UnityEngine;

public class WallSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        // Vérifie si un clic gauche est effectué
        if (Input.GetMouseButtonDown(0) && mainCamera == Camera.current)
        {
            // Lance un raycast depuis la position de la souris
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Si le raycast touche quelque chose
            if (Physics.Raycast(ray, out hit, 1500))
            {
                // Vérifie si l'objet touché est un mur
                if (hit.collider != null && hit.collider.name.Contains("Wall"))
                {
                    // Récupère les informations du mur
                    GameObject wall = hit.collider.gameObject;
                    Vector3 position = wall.transform.position;
                    Vector3 rotation = wall.transform.eulerAngles;
                    Vector3 scale = wall.transform.localScale;

                    changeSceneCenter(wall);

                    // Affiche ou renvoie les informations du mur
                    DisplayWallInfo(position, rotation, scale);
                }
            }
        }
    }

    private void changeSceneCenter(GameObject wall)
    {
        GameObject roomParent = wall.transform.root.gameObject;
        Renderer[] renderers = roomParent.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogError("Aucun Renderer trouvé dans l'objet de la pièce.");
            return;
        }

        // Initialise la Bounding Box avec le premier Renderer
        Bounds combinedBounds = renderers[0].bounds;

        // Étend la Bounding Box pour inclure tous les autres Renderers
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        CameraMouvements.sceneCenter = combinedBounds.center;
    }


    // Méthode pour afficher les informations du mur dans la console
    private void DisplayWallInfo(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Debug.Log($"Position du mur: {position}");
        Debug.Log($"Rotation du mur: {rotation}");
        Debug.Log($"Taille du mur: {scale}");
    }
}
