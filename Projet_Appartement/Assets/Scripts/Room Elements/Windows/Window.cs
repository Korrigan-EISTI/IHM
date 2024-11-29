using UnityEngine;

public class Window : MonoBehaviour
{
    public void Initialize(Vector3 position, float width)
    {
        transform.position = position;
        transform.localScale = new Vector3(width, 1f, 0.1f);
    }
}
