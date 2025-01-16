using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardShortcuts : MonoBehaviour
{
    [SerializeField] private ApartmentManager apartmentManager;
    [SerializeField] private ToggleButton createButton;
    [SerializeField] private OrbitalCameraController orbitalCamera;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitClicked();
        }
        
        if (Input.GetKeyDown(KeyCode.Z) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
        {
            apartmentManager.onCreateWallButtonClicked();
            createButton.ToggleState();
        }

        if (orbitalCamera.target != null)
        {
            if (orbitalCamera.target.name.Contains("wall"))
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    orbitalCamera.OnAddWindowClicked();
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    orbitalCamera.OnAddDoorClicked();
                }

                if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace) ||
                    Input.GetKeyDown(KeyCode.X))
                {
                    orbitalCamera.onDestroyWall();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                orbitalCamera.ClearTarget();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraSwitcher.SwitchCamera();
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                orbitalCamera.onUndo();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                orbitalCamera.onRedo();
            }
        }
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
