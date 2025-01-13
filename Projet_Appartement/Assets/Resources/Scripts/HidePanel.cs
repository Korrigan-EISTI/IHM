using UnityEngine;
using UnityEngine.EventSystems;

public class HidePanel : MonoBehaviour
{
    // Les noms des boutons Dropdown que tu veux surveiller
    private string[] dropdownButtonNames = { "File", "Options", "Window" };

    // Panel actuellement ouvert
    private static GameObject currentOpenPanel = null;

    // Fonction appel�e par un bouton pour afficher un panneau
    public void OpenPanel(GameObject newPanel)
    {
        if (currentOpenPanel != null)
        {
            currentOpenPanel.SetActive(false);
        }

        newPanel.SetActive(true);
        currentOpenPanel = newPanel;
    }

    // Fonction appel�e pour basculer l'�tat d'un panneau
    public void TogglePanel(GameObject newPanel)
    {
        if (currentOpenPanel == newPanel)
        {
            CloseCurrentPanel();
        }
        else
        {
            OpenPanel(newPanel);
        }
    }

    // Ferme le panneau actuellement ouvert
    public void CloseCurrentPanel()
    {
        if (currentOpenPanel != null)
        {
            currentOpenPanel.SetActive(false);
            currentOpenPanel = null;
        }
    }

    void Update()
    {
        // V�rifie si un clic est effectu� en dehors des boutons Dropdown
        if (currentOpenPanel != null && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverDropdownButton())
            {
                CloseCurrentPanel();
            }
        }
    }

    // V�rifie si le pointeur est au-dessus d'un des boutons Dropdown sp�cifiques
    private bool IsPointerOverDropdownButton()
    {
        // Cr�ation des donn�es de l��v�nement � la position actuelle de la souris
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Effectue un raycast sur tous les �l�ments UI sous le pointeur
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // V�rifie si le pointeur est sur un des boutons Dropdown sp�cifiques
        foreach (var result in results)
        {
            if (System.Array.Exists(dropdownButtonNames, name => name == result.gameObject.name))
            {
                return true;
            }
        }

        return false;
    }
}
