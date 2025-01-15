using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform player; // Assurez-vous d'assigner le Transform du joueur ou de la caméra.

    void Update()
    {
        if (player == null) return;

        // Calculez l'angle entre la direction du nord et la direction actuelle du joueur
        float angle = Mathf.Atan2(player.transform.forward.x, player.transform.forward.z) * Mathf.Rad2Deg;

        // Appliquez cet angle à la rotation du sprite pour qu'il pointe vers le nord
        transform.eulerAngles = new Vector3(0, 0, -angle);
    }
}
