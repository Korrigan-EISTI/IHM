using UnityEngine;

public class Wall : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;

    /// <summary>
    /// Initialise un mur avec ses points de d�part et d'arriv�e.
    /// </summary>
    public void Initialize(Vector3 start, Vector3 end)
    {
        if (start == end)
        {
            Debug.LogError("Cannot initialize a wall with identical start and end points.");
            return;
        }

        startPoint = start;
        endPoint = end;

        // Position au milieu des deux points
        transform.position = (start + end) / 2;

        // Calcul de la direction et application de la rotation
        Vector3 direction = end - start;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            Debug.LogWarning("Direction is zero; no rotation applied.");
            transform.rotation = Quaternion.identity;
        }

        // Ajuster la taille du mur en fonction de la distance entre start et end
        transform.localScale = new Vector3(0.1f, 2f, direction.magnitude);
    }

    /// <summary>
    /// Divise le mur en deux parties au point donn�.
    /// </summary>
    public void SplitWall(Vector3 splitPoint)
    {
        // Cr�er un nouveau mur pour la seconde moiti�
        Wall newWall = Instantiate(this, transform.parent);
        newWall.Initialize(splitPoint, endPoint);

        // Mettre � jour ce mur pour la premi�re moiti�
        Initialize(startPoint, splitPoint);

        Debug.Log($"Wall split at {splitPoint}. Created new wall: {newWall.name}");
    }

    /// <summary>
    /// V�rifie si un point donn� est situ� sur ce mur.
    /// </summary>
    public bool IsPointOnWall(Vector3 point)
    {
        // V�rifie si le point est dans les bounds du GameObject
        Collider wallCollider = GetComponent<Collider>();
        if (wallCollider != null && wallCollider.bounds.Contains(point))
        {
            return true; // Le point est sur ce mur
        }

        return false; // Le point n'est pas sur ce mur
    }


}
