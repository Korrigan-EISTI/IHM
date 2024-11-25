using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PerfectGridTextureForPlane : MonoBehaviour
{
    public Color gridColor = Color.white; // Couleur des lignes de la grille
    public Color backgroundColor = new Color(0f, 0.443f, 0.631f); // Couleur #0071A1
    public int pixelsPerUnit = 50; // Contrôle la densité de la grille
    public float lineThickness = 1.5f; // Épaisseur des lignes de la grille (en pixels)

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Vector3 size = renderer.bounds.size; // Taille réelle du Plane
        Texture2D gridTexture = GeneratePerfectGridTexture(size);
        renderer.material.mainTexture = gridTexture;
    }

    Texture2D GeneratePerfectGridTexture(Vector3 size)
    {
        // Calcul de la taille de texture basé sur les dimensions du Plane
        int width = Mathf.RoundToInt(size.x * pixelsPerUnit);
        int height = Mathf.RoundToInt(size.z * pixelsPerUnit); // Taille Z pour le Plane

        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Déterminer si le pixel appartient à une ligne de grille avec la bonne épaisseur
                bool isGridLineX = (x % pixelsPerUnit) < lineThickness || (x % pixelsPerUnit) > (pixelsPerUnit - lineThickness);
                bool isGridLineY = (y % pixelsPerUnit) < lineThickness || (y % pixelsPerUnit) > (pixelsPerUnit - lineThickness);

                if (isGridLineX || isGridLineY)
                {
                    texture.SetPixel(x, y, gridColor);
                }
                else
                {
                    texture.SetPixel(x, y, backgroundColor);
                }
            }
        }
        
        texture.filterMode = FilterMode.Point; // Pas d'interpolation pour garder des lignes nettes
        texture.Apply();
        return texture;
    }
}
