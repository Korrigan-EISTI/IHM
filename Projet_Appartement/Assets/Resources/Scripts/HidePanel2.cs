using UnityEngine;

public class HidePanel2 : MonoBehaviour
{
    // Le panel à cacher
    public GameObject gameObject2;
    bool active;

    public void Close()
    {
        if (active == false)
        {
            gameObject2.transform.gameObject.SetActive(true);
            active = true;
        }
        else
        {
            gameObject2.transform.gameObject.SetActive(false);
            active = false;
        }
    }
}



