using UnityEngine;

public class ArrowMover : MonoBehaviour
{
    public Vector3 direction; // La direction de d�placement (X, Y ou Z)
    public Transform targetObject; // L'objet cible � d�placer
    public float moveSpeed = 0.1f; // Vitesse de d�placement

    private void OnMouseDrag()
    {
        if (targetObject)
        {
            Vector3 movement = direction * moveSpeed * Time.deltaTime;
            targetObject.position += movement;
        }
    }
}
