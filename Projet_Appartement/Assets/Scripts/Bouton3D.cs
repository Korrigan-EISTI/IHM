using UnityEngine;
using System;

public class Button3D : MonoBehaviour
{
    public Action OnButtonClicked;

    private void OnMouseDown()
    {
        OnButtonClicked?.Invoke();
    }
}
