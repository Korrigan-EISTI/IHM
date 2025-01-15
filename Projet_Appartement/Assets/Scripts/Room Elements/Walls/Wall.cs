using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;

    public GameObject windowPrefab; // Prefab de la fen�tre � instancier.
    public GameObject doorPrefab; // Prefab de la porte � instancier.

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

        foreach (Camera camera in Camera.allCameras)
        {
            if (camera.name.Contains("Orbital"))
            {
                camera.GetComponent<OrbitalCameraController>().addWallToUndo(gameObject);
            }
        }
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
    public void AdjustWallSegment(Vector3 startPoint, Vector3 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;

        Vector3 direction = endPoint - startPoint;
        transform.position = (startPoint + endPoint) / 2;
        transform.localScale = new Vector3(0.15f, transform.localScale.y, direction.magnitude);
    }

    public void AdjustWallSegmentMove(Vector3 startPoint, Vector3 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;

        // Calcul de la direction
        Vector3 direction = endPoint - startPoint;

        // Ajuster la position au milieu entre les deux points
        transform.position = (startPoint + endPoint) / 2;

        // Ajuster l'�chelle : L'axe z repr�sente la longueur du mur
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, direction.magnitude);

        // Ajuster la rotation pour aligner le mur avec la direction
        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    public GameObject AddWindow(Vector3 windowCenter, float windowWidth)
    {
        if (windowWidth <= 0)
        {
            Debug.LogError("Window width must be greater than zero.");
            return null;
        }

        // Calcul de la distance entre les points de d�part et d'arriv�e
        float wallLength = Vector3.Distance(startPoint, endPoint);
        if (windowWidth >= wallLength)
        {
            Debug.LogError("Window width cannot exceed or match wall length.");
            return null;
        }

        // V�rifier que la fen�tre peut �tre plac�e sur le mur
        Vector3 wallDirection = (endPoint - startPoint).normalized;
        Vector3 localStartToWindow = windowCenter - startPoint;
        float distanceToWindow = Vector3.Dot(localStartToWindow, wallDirection);

        if (distanceToWindow - windowWidth / 2 < 0 || distanceToWindow + windowWidth / 2 > wallLength)
        {
            Debug.LogError("Window placement is outside of wall boundaries.");
            return null;
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


        gameObject.transform.SetParent(window.transform);
        // D�truire le mur d'origine
        gameObject.name = "OldWall";
        gameObject.SetActive(false);

        return gameObject;
    }

    public GameObject AddDoor(Vector3 doorCenter, float doorWidth)
    {
        if (doorWidth <= 0)
        {
            Debug.LogError("Window width must be greater than zero.");
            return null;
        }

        // Calcul de la distance entre les points de d�part et d'arriv�e
        float wallLength = Vector3.Distance(startPoint, endPoint);
        if (doorWidth >= wallLength)
        {
            Debug.LogError("Window width cannot exceed or match wall length.");
            return null;
        }

        // V�rifier que la fen�tre peut �tre plac�e sur le mur
        Vector3 wallDirection = (endPoint - startPoint).normalized;
        Vector3 localStartToWindow = doorCenter - startPoint;
        float distanceToWindow = Vector3.Dot(localStartToWindow, wallDirection);

        if (distanceToWindow - doorWidth / 2 < 0 || distanceToWindow + doorWidth / 2 > wallLength)
        {
            Debug.LogError("Window placement is outside of wall boundaries.");
            return null;
        }

        // Instancier le prefab de la fen�tre
        GameObject door = Instantiate(doorPrefab, transform.parent);
        door.transform.position = doorCenter;
        door.transform.rotation = transform.rotation;

        // Ajuster la taille de la fen�tre pour qu'elle corresponde � la largeur sp�cifi�e
        Vector3 doorScale = door.transform.localScale;
        door.transform.localScale = new Vector3(doorWidth, doorScale.y, doorScale.z);

        // Cr�er deux nouveaux morceaux de mur (gauche et droit)
        Vector3 leftEnd = startPoint + wallDirection * (distanceToWindow - doorWidth / 2);
        Vector3 rightStart = startPoint + wallDirection * (distanceToWindow + doorWidth / 2);

        foreach (Wall wall in door.GetComponentsInChildren<Wall>())
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


        gameObject.transform.SetParent(door.transform);
        // D�truire le mur d'origine
        gameObject.name = "OldWall";
        gameObject.SetActive(false);

        return gameObject;
    }
}
