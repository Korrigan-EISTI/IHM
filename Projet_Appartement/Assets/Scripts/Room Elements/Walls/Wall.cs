using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;

    public GameObject windowPrefab; // Prefab de la fen�tre � instancier.

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
    /// V�rifie si un point donn� est situ� sur ce mur.
    /// </summary>
    public bool IsPointOnWall(Vector3 point)
    {
        Collider wallCollider = GetComponent<Collider>();
        if (wallCollider != null && wallCollider.bounds.Contains(point))
        {
            return true; // Le point est sur ce mur
        }

        return false; // Le point n'est pas sur ce mur
    }
    private void AdjustWallSegment(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direction = endPoint - startPoint;
        transform.position = (startPoint + endPoint) / 2;
        transform.localScale = new Vector3(0.1f, transform.localScale.y, direction.magnitude);
    }

    public void AddWindow(Vector3 windowCenter, float windowWidth)
    {
        if (windowWidth <= 0)
        {
            Debug.LogError("Window width must be greater than zero.");
            return;
        }

        // Calcul de la distance entre les points de d�part et d'arriv�e
        float wallLength = Vector3.Distance(startPoint, endPoint);
        if (windowWidth >= wallLength)
        {
            Debug.LogError("Window width cannot exceed or match wall length.");
            return;
        }

        // V�rifier que la fen�tre peut �tre plac�e sur le mur
        Vector3 wallDirection = (endPoint - startPoint).normalized;
        Vector3 localStartToWindow = windowCenter - startPoint;
        float distanceToWindow = Vector3.Dot(localStartToWindow, wallDirection);

        if (distanceToWindow - windowWidth / 2 < 0 || distanceToWindow + windowWidth / 2 > wallLength)
        {
            Debug.LogError("Window placement is outside of wall boundaries.");
            return;
        }

        // Instancier le prefab de la fen�tre
        GameObject window = Instantiate(windowPrefab, transform.parent);
        window.transform.position = windowCenter;
        window.transform.rotation = transform.rotation;

        // Ajuster la taille de la fen�tre pour qu'elle corresponde � la largeur sp�cifi�e
        Vector3 windowScale = window.transform.localScale;
        window.transform.localScale = new Vector3(windowWidth, windowScale.y, windowScale.z);

        // Cr�er deux nouveaux morceaux de mur (gauche et droit)
        Vector3 leftEnd = startPoint + wallDirection * (distanceToWindow - windowWidth / 2);
        Vector3 rightStart = startPoint + wallDirection * (distanceToWindow + windowWidth / 2);

        foreach (Wall wall in window.GetComponentsInChildren<Wall>())
        {
            if (wall.name.Contains("LeftWall"))
            {
                wall.AdjustWallSegment(startPoint, leftEnd);
            }
            else if (wall.name.Contains("RightWall"))
            {
                wall.AdjustWallSegment(rightStart, endPoint);
            }
        }

        // D�truire le mur d'origine
        Destroy(gameObject);
    }
}
