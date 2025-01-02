using Unity.VisualScripting;
using UnityEngine;

public class HoleManipulator : MonoBehaviour
{
    public Transform leftWall;
    public Transform rightWall;
    public Transform topWall;
    public Transform bottomWall;
    public Wall oldWall;

    public float moveSpeed = 0.1f;

    private GameObject xArrow, yArrow;

    private void Start()
    {
        CreateArrows();
    }

    private void CreateArrows()
    {
        xArrow = CreateArrow(Vector3.right, Color.red);
        xArrow.name = "XArrow";

        yArrow = CreateArrow(Vector3.up, Color.green);
        yArrow.name = "YArrow";
    }

    private GameObject CreateArrow(Vector3 direction, Color color)
    {
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrow.transform.SetParent(transform);
        arrow.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
        arrow.transform.localPosition = direction.normalized * 0.5f;
        arrow.transform.up = direction;
        arrow.GetComponent<Renderer>().material.color = color;

        ArrowInteraction arrowInteraction = arrow.AddComponent<ArrowInteraction>();
        arrowInteraction.direction = direction;
        arrowInteraction.holeManipulator = this;

        return arrow;
    }

    public void MoveHole(Vector3 direction)
    {
        if (leftWall && rightWall && topWall && bottomWall)
        {
            // Calculer la nouvelle position potentielle de windowStart et windowEnd
            Vector3 newWindowStart = leftWall.GetComponent<Wall>().endPoint + direction;
            Vector3 newWindowEnd = rightWall.GetComponent<Wall>().startPoint + direction;

            // Mettre à jour les segments des murs gauche et droit
            leftWall.GetComponent<Wall>().AdjustWallSegmentMove(leftWall.GetComponent<Wall>().startPoint, newWindowStart);
            rightWall.GetComponent<Wall>().AdjustWallSegmentMove(newWindowEnd, rightWall.GetComponent<Wall>().endPoint);

            // Contraintes pour les murs haut et bas
            Vector3 topWindowStart = new Vector3(newWindowStart.x, topWall.position.y, newWindowStart.z);
            Vector3 topWindowEnd = new Vector3(newWindowEnd.x, topWall.position.y, newWindowEnd.z);

            Vector3 botWindowStart = new Vector3(newWindowStart.x, bottomWall.position.y, newWindowStart.z);
            Vector3 botWindowEnd = new Vector3(newWindowEnd.x, bottomWall.position.y, newWindowEnd.z);

            // Mettre à jour les segments des murs haut et bas
            topWall.GetComponent<Wall>().AdjustWallSegmentMove(topWindowStart, topWindowEnd);
            bottomWall.GetComponent<Wall>().AdjustWallSegmentMove(botWindowStart, botWindowEnd);

            // Mettre à jour les gizmos pour qu'ils suivent le trou
            UpdateArrowPositions();
        }
    }

    /// <summary>
    /// Contraindre une position entre deux points limites.
    /// </summary>
    /// <param name="position">Position à contraindre.</param>
    /// <param name="minLimit">Limite minimale.</param>
    /// <param name="maxLimit">Limite maximale.</param>
    /// <returns>Position contrainte.</returns>
    private Vector3 ConstrainPositionToWalls(Vector3 position, Vector3 minLimit, Vector3 maxLimit)
    {
        Vector3 constrainedPosition = position;

        // Contraindre chaque axe séparément
        if (position.x < minLimit.x) constrainedPosition.x = minLimit.x;
        if (position.x > maxLimit.x) constrainedPosition.x = maxLimit.x;

        if (position.z < minLimit.z) constrainedPosition.z = minLimit.z;
        if (position.z > maxLimit.z) constrainedPosition.z = maxLimit.z;

        return constrainedPosition;
    }

    public void UpdateArrowPositions()
    {
        if (xArrow) xArrow.transform.localPosition = Vector3.right * 0.5f;
        if (yArrow) yArrow.transform.localPosition = Vector3.up * 0.5f;
    }
}
