using UnityEngine;

public class ArrowInteraction : MonoBehaviour
{
    public Vector3 direction; // Direction associée à la flèche
    public HoleManipulator holeManipulator;

    public bool isDragging = false; // Indique si l'utilisateur est en train de faire glisser la flèche
    public Vector3 dragStartPosition; // Position initiale de la souris lors du début du drag

    private void OnMouseDown()
    {
        // Enregistrer la position initiale de la souris
        isDragging = true;
        
    }

    private void OnMouseDrag()
    {
        if (isDragging && holeManipulator != null)
        {
            // Calculer le déplacement
            Vector3 currentMousePosition = GetMouseWorldPosition();
            Vector3 delta = currentMousePosition - dragStartPosition;

            // Projeter le déplacement sur la direction de la flèche
            float movementInDirection = Vector3.Dot(delta, direction.normalized);
            Vector3 movement = direction.normalized * movementInDirection;

            // Déplacer le trou
            holeManipulator.MoveHole(movement);

            // Mettre à jour la position de la souris
            dragStartPosition = currentMousePosition;

            // Mettre à jour la position des flèches
            holeManipulator.UpdateArrowPositions();
        }
    }

    private void OnMouseUp()
    {
        // Terminer le drag
        isDragging = false;
    }

    public Vector3 GetMouseWorldPosition()
    {
        // Convertir la position de la souris en coordonnées du monde
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0); // Assumer un plan horizontal
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
