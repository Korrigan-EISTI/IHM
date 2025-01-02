using UnityEngine;

public class ArrowMover : MonoBehaviour
{
    public Vector3 direction; // La direction de déplacement (X, Y ou Z)
    public Transform targetObject; // L'objet cible à déplacer
    public float moveSpeed = 0.1f; // Vitesse de déplacement

    private void OnMouseDrag()
    {
        if (targetObject)
        {
            Vector3 movement = direction * moveSpeed * Time.deltaTime;
            targetObject.position += movement;
        }
    }
}
