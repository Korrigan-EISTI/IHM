using UnityEngine;

public class CloseWindow : MonoBehaviour
{
    // Appel� lorsque le bouton "Close" est cliqu�
    public void CloseGame()
    {
#if UNITY_EDITOR
        // En mode �diteur, arr�ter le mode Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Fermer l'application dans un build
        Application.Quit();
#endif
    }
}
