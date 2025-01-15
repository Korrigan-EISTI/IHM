using Unity.VisualScripting;
using UnityEngine;

public class HoleManipulator : MonoBehaviour
{
    public Transform leftWall; // R�f�rence au mur gauche
    public Transform rightWall; // R�f�rence au mur droit
    public Transform topWall; // R�f�rence au mur sup�rieur
    public Transform bottomWall; // R�f�rence au mur inf�rieur

    private Vector3 lastStartPoint;
    private Vector3 lastEndPoint;

    private GameObject xArrow; // Fl�che pour le mur gauche

    public float arrowOffset = 0.5f; // D�calage de la fl�che par rapport au mur

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
        // Cr�er les fl�ches ind�pendantes
        if (xArrow == null)
            xArrow = CreateArrow(Color.red, "XArrow");
    }

    private Vector3 CalculateWallDirection()
    {
        Vector3 start = leftWall.GetComponent<Wall>().startPoint;
        Vector3 end = rightWall.GetComponent<Wall>().endPoint;

        // Direction du mur (normalis�e)
        return (start - end).normalized;
    }


    private GameObject CreateArrow(Color color, string name)
    {
        // Cr�er un cylindre pour repr�senter la fl�che
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.parent = GameObject.Find("TranslationGizmos").transform;
        arrow.name = name;
        arrow.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f); // �chelle ajust�e
        arrow.GetComponent<Renderer>().material.color = color; // Appliquer la couleur

        // Calculer dynamiquement la direction bas�e sur le mur
        Vector3 wallDirection = CalculateWallDirection();

        // Ajouter le script d'interaction � la fl�che
        ArrowInteraction arrowInteraction = arrow.AddComponent<ArrowInteraction>();
        arrowInteraction.direction = wallDirection; // D�finit la direction de l'interaction
        arrowInteraction.holeManipulator = this; // Passe une r�f�rence au HoleManipulator

        // Initialiser les points de d�part et d'arriv�e
        lastStartPoint = leftWall.GetComponent<Wall>().endPoint;
        lastEndPoint = rightWall.GetComponent<Wall>().startPoint;

        return arrow;
    }


    private void Update()
    {
        // Mettre � jour dynamiquement les positions et rotations des fl�ches
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

        // D�terminer un vecteur perpendiculaire pour �viter que la fl�che s�aligne mal
        Vector3 upDirection = Vector3.Cross(wallDirection, Vector3.up).normalized;

        // Aligner la fl�che avec le mur
        Quaternion arrowRotation = Quaternion.LookRotation(wallDirection, upDirection);

        // Appliquer la rotation � la fl�che
        arrow.transform.rotation = arrowRotation;

        // Ajuster pour pointer correctement selon l'axe du cylindre
        arrow.transform.Rotate(90, 0, 0, Space.Self); // R�orientation locale
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

        // D�calage de la fl�che hors du mur
        Vector3 wallRight = Vector3.Cross(wallDirection, Vector3.up).normalized;
        arrowPosition += wallRight * arrowOffset;

        // Positionner la fl�che
        arrow.transform.position = arrowPosition;

        // Orienter la fl�che avec la nouvelle m�thode
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
            // R�cup�rer les points initiaux des murs gauche et droit
            Vector3 leftStart = leftWall.GetComponent<Wall>().startPoint;
            Vector3 leftEnd = leftWall.GetComponent<Wall>().endPoint;

            Vector3 rightStart = rightWall.GetComponent<Wall>().startPoint;
            Vector3 rightEnd = rightWall.GetComponent<Wall>().endPoint;

            // Calculer l'orientation (positive ou n�gative)
            float orientation = (rightEnd - leftStart).x * (rightEnd - leftStart).z -
                                (rightEnd - leftStart).z * (rightEnd - leftStart).x;

            // Si l'orientation est n�gative, inverser les points
            if (orientation < 0)
            {
                leftStart = rightWall.GetComponent<Wall>().endPoint;
                leftEnd = leftWall.GetComponent<Wall>().startPoint;
            }

            // Projeter le mouvement sur l'axe des murs lat�raux
            Vector3 projectedDirectionLeft = ProjectOntoWallDirection(direction, leftWall);
            Vector3 projectedDirectionRight = ProjectOntoWallDirection(direction, rightWall);

            // Calculer les nouvelles positions projet�es
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

            // Comparer les positions projet�es
            if (Vector3.Distance(newWindowStart, leftStart) < 0.25f || Vector3.Distance(newWindowEnd, rightEnd) < 0.25f)
            {
                // Revenir aux derniers points valides
                newWindowStart = lastStartPoint;
                newWindowEnd = lastEndPoint;
            }
            else
            {
                // Mettre � jour les points finaux
                lastStartPoint = newWindowStart;
                lastEndPoint = newWindowEnd;
            }

            // Mettre � jour les segments des murs lat�raux
            leftWall.GetComponent<Wall>().AdjustWallSegmentMove(leftStart, newWindowStart);
            rightWall.GetComponent<Wall>().AdjustWallSegmentMove(newWindowEnd, rightEnd);

            // Contraintes pour les murs sup�rieurs et inf�rieurs
            Vector3 topWindowStart = new Vector3(newWindowStart.x, topWall.position.y, newWindowStart.z);
            Vector3 topWindowEnd = new Vector3(newWindowEnd.x, topWall.position.y, newWindowEnd.z);
            topWall.GetComponent<Wall>().AdjustWallSegmentMove(topWindowStart, topWindowEnd);

            Vector3 botWindowStart = new Vector3(newWindowStart.x, bottomWall.position.y, newWindowStart.z);
            Vector3 botWindowEnd = new Vector3(newWindowEnd.x, bottomWall.position.y, newWindowEnd.z);
            bottomWall.GetComponent<Wall>().AdjustWallSegmentMove(botWindowStart, botWindowEnd);

            // Mettre � jour les positions des fl�ches
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
