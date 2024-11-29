using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Transform cameras;
    public KeyCode switchKey = KeyCode.C; // Touche pour changer de caméra
    private int currentCameraIndex = 0;
    

    void Start()
    {
        // Désactiver toutes les caméras sauf la première
        for (int i = 0; i < cameras.childCount; i++)
        {
            cameras.GetChild(i).gameObject.SetActive(i == currentCameraIndex);
            Camera.SetupCurrent(cameras.GetChild(currentCameraIndex).GetComponent<Camera>());
        }
    }

    void Update()
    {
        // Vérifier si la touche de changement de caméra est pressée
        if (Input.GetKeyDown(switchKey))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        // Désactiver la caméra actuelle
        cameras.GetChild(currentCameraIndex).gameObject.SetActive(false);

        // Passer à la caméra suivante
        currentCameraIndex = (currentCameraIndex + 1) % cameras.childCount;

        // Activer la nouvelle caméra
        cameras.GetChild(currentCameraIndex).gameObject.SetActive(true);
        Camera.SetupCurrent(cameras.GetChild(currentCameraIndex).GetComponent<Camera>());
    }
}
