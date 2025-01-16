using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverInfoButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string hoverMessage; // Message à afficher
    [SerializeField] private Canvas canvas;       // Référence au Canvas principal
    private GameObject hoverTextObject;           // Objet TextMeshPro créé dynamiquement

    private void Start()
    {
        if (canvas == null)
        {
            Debug.LogError("Assurez-vous de lier le Canvas dans l'inspecteur !");
            return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Crée dynamiquement un TextMeshProUGUI pour afficher le message
        hoverTextObject = new GameObject("HoverText");
        hoverTextObject.transform.SetParent(canvas.transform, false);

        // Configure le TextMeshProUGUI
        TextMeshProUGUI hoverText = hoverTextObject.AddComponent<TextMeshProUGUI>();
        hoverText.text = hoverMessage;
        hoverText.fontSize = 16;
        hoverText.color = Color.white;
        hoverText.alignment = TextAlignmentOptions.Left;

        // Ajoute un RectTransform pour positionner le texte
        RectTransform rectTransform = hoverText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50); // Taille par défaut du texte
        rectTransform.anchorMin = new Vector2(0, 1);    // En haut à gauche
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);

        // Place le texte à la position de la souris
        UpdateHoverTextPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Détruit le TextMeshPro créé dynamiquement
        if (hoverTextObject != null)
        {
            Destroy(hoverTextObject);
        }
    }

    private void Update()
    {
        // Met à jour la position du texte si la souris est sur le bouton
        if (hoverTextObject != null)
        {
            UpdateHoverTextPosition();
        }
    }

    private void UpdateHoverTextPosition()
    {
        if (hoverTextObject != null)
        {
            RectTransform rectTransform = hoverTextObject.GetComponent<RectTransform>();
            Vector2 mousePosition = Input.mousePosition;
            rectTransform.position = mousePosition + new Vector2(10, -10); // Décalage pour éviter de cacher la souris
        }
    }
}
