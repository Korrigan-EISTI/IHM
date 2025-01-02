using UnityEngine;

public class TranslationGizmos : MonoBehaviour
{
    public Transform targetObject; // L'objet à déplacer (fenêtre)
    public float arrowLength = 0.5f; // Longueur des flèches
    public float moveSpeed = 0.5f; // Vitesse de déplacement

    private GameObject _xArrow, _yArrow;

    private void Start()
    {
        CreateArrows();
    }

    private void Update()
    {
        if (targetObject)
        {
            // Suivre la position de l'objet cible
            transform.position = targetObject.position;
        }
    }

    private void CreateArrows()
    {
        // Créer l'axe X (rouge)
        _xArrow = CreateArrow(Vector3.right, Color.red);
        _xArrow.name = "XArrow";

        // Créer l'axe Y (vert)
        _yArrow = CreateArrow(Vector3.up, Color.green);
        _yArrow.name = "YArrow";
    }

    private GameObject CreateArrow(Vector3 direction, Color color)
    {
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.SetParent(transform);

        arrow.transform.localScale = new Vector3(0.1f, arrowLength / 2, 0.1f); // Cylindre aplati
        arrow.transform.localPosition = direction * arrowLength / 2;
        arrow.transform.up = direction; // Aligner le cylindre sur la direction
        arrow.GetComponent<Renderer>().material.color = color;

        ArrowMover mover = arrow.AddComponent<ArrowMover>();
        mover.direction = direction;
        mover.targetObject = targetObject;
        mover.moveSpeed = moveSpeed;

        return arrow;
    }

    public void SetTarget(Transform newTarget)
    {
        targetObject = newTarget;
        transform.position = newTarget.position;
    }

    public void ClearTarget()
    {
        targetObject = null;
        Destroy(gameObject);
    }
}
