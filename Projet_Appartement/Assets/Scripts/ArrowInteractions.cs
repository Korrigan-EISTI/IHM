using UnityEngine;

public class ArrowInteraction : MonoBehaviour
{
    public Vector3 direction; // Direction associ�e � la fl�che
    public HoleManipulator holeManipulator;

    public bool isDragging = false; // Indique si l'utilisateur est en train de faire glisser la fl�che
    public Vector3 dragStartPosition; // Position initiale de la souris lors du d�but du drag

    private void OnMouseDown()
    {
        // Enregistrer la position initiale de la souris
        isDragging = true;
        holeManipulator.XArrow.GetComponent<Renderer>().material.color = Color.yellow;
    }

    private void OnMouseDrag()
    {
        if (isDragging && holeManipulator != null)
        {
            // Calculer le d�placement
            Vector3 currentMousePosition = GetMouseWorldPosition();
            Vector3 delta = currentMousePosition - dragStartPosition;

            // Projeter le d�placement sur la direction de la fl�che
            float movementInDirection = Vector3.Dot(delta, direction.normalized);
            Vector3 movement = direction.normalized * movementInDirection;

            // D�placer le trou
            holeManipulator.MoveHole(movement);

            // Mettre � jour la position de la souris
            dragStartPosition = currentMousePosition;
        }
    }


    private void OnMouseUp()
    {
        // Terminer le drag
        isDragging = false;
        holeManipulator.XArrow.GetComponent<Renderer>().material.color = Color.red;
    }

    public Vector3 GetMouseWorldPosition()
    {
        // Convertir la position de la souris en coordonn�es du monde
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0); // Assumer un plan horizontal
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
