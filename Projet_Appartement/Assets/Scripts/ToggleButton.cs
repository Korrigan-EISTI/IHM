using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    private Button button;
    private bool isToggled = false;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleState);
        UpdateButtonVisual();
    }

    void ToggleState()
    {
        isToggled = !isToggled;
        UpdateButtonVisual();
    }

    void UpdateButtonVisual()
    {
        if (isToggled)
        {
            button.image.color = new Color(71f / 255f, 103f / 255f, 103f / 255f, 117f / 255f);
        }
        else
        {
            button.image.color = Color.white;
        }
    }

}
