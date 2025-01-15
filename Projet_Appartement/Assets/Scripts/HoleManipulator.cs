using Unity.VisualScripting;
using UnityEngine;

public class HoleManipulator : MonoBehaviour
{
    public Transform leftWall; // Référence au mur gauche
    public Transform rightWall; // Référence au mur droit
    public Transform topWall; // Référence au mur supérieur
    public Transform bottomWall; // Référence au mur inférieur

    private Vector3 lastStartPoint;
    private Vector3 lastEndPoint;

    private GameObject xArrow; // Flèche pour le mur gauche

    public float arrowOffset = 0.5f; // Décalage de la flèche par rapport au mur

    // Getter et Setter pour lastStartPoint
    public Vector3 LastStartPoint
    {
        get { return lastStartPoint; }
        set { lastStartPoint = value; }
    }

    public GameObject XArrow
    {
        get { return xArrow; }
        private set { xArrow = value; }
    }

    // Getter et Setter pour lastEndPoint
    public Vector3 LastEndPoint
    {
        get { return lastEndPoint; }
        set { lastEndPoint = value; }
    }


    private void Start()
    {
        // Créer les flèches indépendantes
        if (xArrow == null)
            xArrow = CreateArrow(Color.red, "XArrow");
    }

    private Vector3 CalculateWallDirection()
    {
        Vector3 start = leftWall.GetComponent<Wall>().startPoint;
        Vector3 end = rightWall.GetComponent<Wall>().endPoint;

        // Direction du mur (normalisée)
        return (start - end).normalized;
    }


    private GameObject CreateArrow(Color color, string name)
    {
        // Créer un cylindre pour représenter la flèche
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.parent = GameObject.Find("TranslationGizmos").transform;
        arrow.name = name;
        arrow.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f); // Échelle ajustée
        arrow.GetComponent<Renderer>().material.color = color; // Appliquer la couleur

        // Calculer dynamiquement la direction basée sur le mur
        Vector3 wallDirection = CalculateWallDirection();

        // Ajouter le script d'interaction à la flèche
        ArrowInteraction arrowInteraction = arrow.AddComponent<ArrowInteraction>();
        arrowInteraction.direction = wallDirection; // Définit la direction de l'interaction
        arrowInteraction.holeManipulator = this; // Passe une référence au HoleManipulator

        // Initialiser les points de départ et d'arrivée
        lastStartPoint = leftWall.GetComponent<Wall>().endPoint;
        lastEndPoint = rightWall.GetComponent<Wall>().startPoint;

        return arrow;
    }


    private void Update()
    {
        // Mettre à jour dynamiquement les positions et rotations des flèches
        if (xArrow && leftWall)
        {
            UpdateArrowBasedOnWall(xArrow, leftWall);
        }
    }

    private void OrientArrow(GameObject arrow, Transform wall)
    {
        Wall wallComponent = wall.GetComponent<Wall>();
        if (wallComponent == null)
        {
            Debug.LogWarning($"Wall component missing on the wall object: {wall.name}");
            return;
        }

        Vector3 startPoint = wallComponent.startPoint;
        Vector3 endPoint = wallComponent.endPoint;

        // Calculer la direction du mur
        Vector3 wallDirection = (endPoint - startPoint).normalized;

        // Déterminer un vecteur perpendiculaire pour éviter que la flèche s’aligne mal
        Vector3 upDirection = Vector3.Cross(wallDirection, Vector3.up).normalized;

        // Aligner la flèche avec le mur
        Quaternion arrowRotation = Quaternion.LookRotation(wallDirection, upDirection);

        // Appliquer la rotation à la flèche
        arrow.transform.rotation = arrowRotation;

        // Ajuster pour pointer correctement selon l'axe du cylindre
        arrow.transform.Rotate(90, 0, 0, Space.Self); // Réorientation locale
    }


    private void UpdateArrowBasedOnWall(GameObject arrow, Transform wall)
    {
        Wall wallComponent = wall.GetComponent<Wall>();
        if (wallComponent == null)
        {
            Debug.LogWarning($"Wall component missing on the wall object: {wall.name}");
            return;
        }

        Vector3 startPoint = wallComponent.startPoint;
        Vector3 endPoint = wallComponent.endPoint;

        // Calculer la position au centre du mur
        Vector3 wallDirection = (endPoint - startPoint).normalized;
        Vector3 arrowPosition = startPoint + (endPoint - startPoint) * 0.5f;

        // Décalage de la flèche hors du mur
        Vector3 wallRight = Vector3.Cross(wallDirection, Vector3.up).normalized;
        arrowPosition += wallRight * arrowOffset;

        // Positionner la flèche
        arrow.transform.position = arrowPosition;

        // Orienter la flèche avec la nouvelle méthode
        OrientArrow(arrow, wall);
    }

    private Vector3 ProjectOntoWallDirection(Vector3 movement, Transform wall)
    {
        Wall wallComponent = wall.GetComponent<Wall>();
        if (wallComponent == null)
        {
            Debug.LogWarning("Le composant Wall est manquant.");
            return Vector3.zero;
        }

        // Direction correcte du mur
        Vector3 wallDirection = (wallComponent.endPoint - wallComponent.startPoint).normalized;

        // Projeter le mouvement sur le vecteur directionnel
        return Vector3.Project(movement, wallDirection);
    }

    public void MoveHole(Vector3 direction)
    {
        if (leftWall && rightWall && topWall && bottomWall)
        {
            // Récupérer les points initiaux des murs gauche et droit
            Vector3 leftStart = leftWall.GetComponent<Wall>().startPoint;
            Vector3 leftEnd = leftWall.GetComponent<Wall>().endPoint;

            Vector3 rightStart = rightWall.GetComponent<Wall>().startPoint;
            Vector3 rightEnd = rightWall.GetComponent<Wall>().endPoint;

            // Calculer l'orientation (positive ou négative)
            float orientation = (rightEnd - leftStart).x * (rightEnd - leftStart).z -
                                (rightEnd - leftStart).z * (rightEnd - leftStart).x;

            // Si l'orientation est négative, inverser les points
            if (orientation < 0)
            {
                leftStart = rightWall.GetComponent<Wall>().endPoint;
                leftEnd = leftWall.GetComponent<Wall>().startPoint;
            }

            // Projeter le mouvement sur l'axe des murs latéraux
            Vector3 projectedDirectionLeft = ProjectOntoWallDirection(direction, leftWall);
            Vector3 projectedDirectionRight = ProjectOntoWallDirection(direction, rightWall);

            // Calculer les nouvelles positions projetées
            Vector3 newWindowStart = ConstrainPositionToWalls(
                leftEnd + projectedDirectionLeft,
                leftWall, rightWall, topWall, bottomWall
            );

            Vector3 newWindowEnd = ConstrainPositionToWalls(
                rightStart + projectedDirectionRight,
                leftWall, rightWall, topWall, bottomWall
            );


            float test1 = Vector3.Distance(newWindowStart, leftStart);
            float test2 = Vector3.Distance(newWindowEnd, rightEnd);

            // Comparer les positions projetées
            if (Vector3.Distance(newWindowStart, leftStart) < 0.25f || Vector3.Distance(newWindowEnd, rightEnd) < 0.25f)
            {
                // Revenir aux derniers points valides
                newWindowStart = lastStartPoint;
                newWindowEnd = lastEndPoint;
            }
            else
            {
                // Mettre à jour les points finaux
                lastStartPoint = newWindowStart;
                lastEndPoint = newWindowEnd;
            }

            // Mettre à jour les segments des murs latéraux
            leftWall.GetComponent<Wall>().AdjustWallSegmentMove(leftStart, newWindowStart);
            rightWall.GetComponent<Wall>().AdjustWallSegmentMove(newWindowEnd, rightEnd);

            // Contraintes pour les murs supérieurs et inférieurs
            Vector3 topWindowStart = new Vector3(newWindowStart.x, topWall.position.y, newWindowStart.z);
            Vector3 topWindowEnd = new Vector3(newWindowEnd.x, topWall.position.y, newWindowEnd.z);
            topWall.GetComponent<Wall>().AdjustWallSegmentMove(topWindowStart, topWindowEnd);

            Vector3 botWindowStart = new Vector3(newWindowStart.x, bottomWall.position.y, newWindowStart.z);
            Vector3 botWindowEnd = new Vector3(newWindowEnd.x, bottomWall.position.y, newWindowEnd.z);
            bottomWall.GetComponent<Wall>().AdjustWallSegmentMove(botWindowStart, botWindowEnd);

            // Mettre à jour les positions des flèches
            UpdateArrowBasedOnWall(xArrow, leftWall);
        }
    }




    private Vector3 ConstrainPositionToWalls(Vector3 position, Transform leftWall, Transform rightWall, Transform topWall, Transform bottomWall)
    {
        // Vecteurs normaux pour les murs
        Vector3 leftToRight = (rightWall.position - leftWall.position).normalized;
        Vector3 bottomToTop = (topWall.position - bottomWall.position).normalized;

        // Projection sur chaque axe
        position -= Vector3.Project(position - leftWall.position, Vector3.Cross(Vector3.up, leftToRight));
        position -= Vector3.Project(position - bottomWall.position, Vector3.Cross(Vector3.up, bottomToTop));

        return position;
    }

}
