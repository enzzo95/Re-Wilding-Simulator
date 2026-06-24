using UnityEngine;

public class Tile : MonoBehaviour
{
    private int gridX;
    private int gridZ;
    private bool isBorder;
    private GridGenerator generator;
    public int tileType; // 0 = Béton, 1 = Nature
    private bool isDestroyed = false;

    public void SetupTile(int x, int z, bool border, GridGenerator gridGen, int type)
    {
        gridX = x;
        gridZ = z;
        isBorder = border;
        generator = gridGen;
        tileType = type;
    }

    public bool IsBorderTile()
    {
        return isBorder;
    }

    // Appelé par le Laser Destructeur
    public void Rewild()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (generator != null)
        {
            generator.DestroyTile(gridX, gridZ);
        }
    }
}