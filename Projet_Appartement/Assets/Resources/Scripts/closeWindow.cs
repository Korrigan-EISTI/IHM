using UnityEngine;

public class CloseWindow : MonoBehaviour
{
    // Appelé lorsque le bouton "Close" est cliqué
    public void CloseGame()
    {
#if UNITY_EDITOR
        // En mode éditeur, arrêter le mode Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Fermer l'application dans un build
        Application.Quit();
#endif
    }
}
