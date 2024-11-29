using UnityEngine;

public class Door : MonoBehaviour
{
    public void Initialize(Vector3 position, float width)
    {
        transform.position = position;
        transform.localScale = new Vector3(width, 2f, 0.1f);
    }
}
