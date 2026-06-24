using UnityEngine;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    [Header("Prefabs Béton (Type 0)")]
    public GameObject concreteCubePrefab;
    public GameObject concreteQuadPrefab;

    [Header("Prefabs Dirt (Type 1)")]
    public GameObject dirtCubePrefab; // J'ai renommé pour la casse
    public GameObject dirtQuadPrefab;

    [Header("Prefabs Herbe (Type 2)")] // <-- NOUVEAU
    public GameObject grassCubePrefab;
    public GameObject grassQuadPrefab;

    [Header("Dimensions")]
    public int width = 20;
    public int length = 20;
    public float tileSize = 0.6f;
    public float tileThickness = 0.2f;

    [Header("Effets Visuels")]
    public GameObject vfxConcretePrefab;
    public GameObject vfxDirtPrefab;
    public GameObject vfxGrassPrefab; // <-- NOUVEAU

    private Dictionary<Vector2Int, Tile> gridPositions = new Dictionary<Vector2Int, Tile>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        gridPositions.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                SpawnTileAt(x, z, 0); // 0 = Béton
            }
        }
        RefreshAllTiles();
    }

    // Fait apparaître un objet brut
    void SpawnTileAt(int x, int z, int type)
    {
        // Par sécurité, on fait spawner un Cube au début
        GameObject prefab = concreteCubePrefab; // Par défaut béton

        // --- NOUVEAU : Choix du prefab initial selon le type ---
        switch (type)
        {
            case 0: prefab = concreteCubePrefab; break;
            case 1: prefab = dirtCubePrefab; break;
            case 2: prefab = grassCubePrefab; break; // <-- Herbe
        }
        // -------------------------------------------------------

        float prefabY = prefab.transform.position.y;
        Vector3 pos = new Vector3(x * tileSize, prefabY, z * tileSize) + transform.position;

        GameObject newTileObj = Instantiate(prefab, pos, prefab.transform.rotation, transform);
        newTileObj.transform.localScale = new Vector3(tileSize, tileThickness, tileSize);

        Tile tileScript = newTileObj.GetComponent<Tile>();
        if (tileScript != null)
        {
            tileScript.SetupTile(x, z, true, this, type);
            gridPositions[new Vector2Int(x, z)] = tileScript;
        }
    }

    // --- LE CERVEAU DE LA GEOMETRIE ---

    bool NeedsToBeCube(int x, int z)
    {
        if (x == 0 || x == width - 1 || z == 0 || z == length - 1) return true;
        if (!gridPositions.ContainsKey(new Vector2Int(x + 1, z))) return true;
        if (!gridPositions.ContainsKey(new Vector2Int(x - 1, z))) return true;
        if (!gridPositions.ContainsKey(new Vector2Int(x, z + 1))) return true;
        if (!gridPositions.ContainsKey(new Vector2Int(x, z - 1))) return true;
        return false;
    }

    // Vérifie et corrige l'apparence d'une case
    public void RefreshTile(int x, int z)
    {
        Vector2Int pos = new Vector2Int(x, z);
        if (!gridPositions.ContainsKey(pos)) return;

        Tile currentTile = gridPositions[pos];
        bool shouldBeCube = NeedsToBeCube(x, z);

        if (currentTile.IsBorderTile() != shouldBeCube)
        {
            int type = currentTile.tileType; // 0=béton, 1=Dirt, 2=Herbe

            Destroy(currentTile.gameObject);

            // --- NOUVEAU : Choix de la nouvelle forme incluant l'Herbe ---
            GameObject newPrefab = null;
            if (type == 0) // Béton
            {
                newPrefab = shouldBeCube ? concreteCubePrefab : concreteQuadPrefab;
            }
            else if (type == 1) // Dirt
            {
                newPrefab = shouldBeCube ? dirtCubePrefab : dirtQuadPrefab;
            }
            else if (type == 2) // Herbe <-- NOUVEAU
            {
                newPrefab = shouldBeCube ? grassCubePrefab : grassQuadPrefab;
            }
            // -----------------------------------------------------------

            float prefabY = newPrefab.transform.position.y;
            Vector3 newWorldPos = new Vector3(x * tileSize, prefabY, z * tileSize) + transform.position;

            GameObject newTileObj = Instantiate(newPrefab, newWorldPos, newPrefab.transform.rotation, transform);
            newTileObj.transform.localScale = new Vector3(tileSize, tileThickness, tileSize);

            Tile newTileScript = newTileObj.GetComponent<Tile>();
            newTileScript.SetupTile(x, z, shouldBeCube, this, type);
            gridPositions[pos] = newTileScript;
        }
    }

    void RefreshAllTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                RefreshTile(x, z);
            }
        }
    }

    void RefreshNeighbors(int x, int z)
    {
        RefreshTile(x + 1, z);
        RefreshTile(x - 1, z);
        RefreshTile(x, z + 1);
        RefreshTile(x, z - 1);
    }

    // --- LES OUTILS DU JOUEUR ---

    public void DestroyTile(int x, int z)
    {
        Vector2Int pos = new Vector2Int(x, z);
        if (gridPositions.ContainsKey(pos))
        {
            if (vfxConcretePrefab != null)
            {
                GameObject vfx = Instantiate(vfxConcretePrefab, gridPositions[pos].transform.position, Quaternion.identity);
                Destroy(vfx, 2f);
            }
            Destroy(gridPositions[pos].gameObject);
            gridPositions.Remove(pos);
            RefreshNeighbors(x, z);
        }
    }

    public void GrowDirt(int x, int z)
    {
        Vector2Int pos = new Vector2Int(x, z);
        if (!gridPositions.ContainsKey(pos))
        {
            if (x >= 0 && x < width && z >= 0 && z < length)
            {
                SpawnTileAt(x, z, 1); // 1 = Dirt
                RefreshTile(x, z);
                RefreshNeighbors(x, z);

                if (vfxDirtPrefab != null)
                {
                    Vector3 spawnPos = new Vector3(x * tileSize, transform.position.y, z * tileSize) + transform.position;
                    GameObject vfx = Instantiate(vfxDirtPrefab, spawnPos, Quaternion.identity);
                    Destroy(vfx, 2f);
                }
            }
        }
    }

    // FONCTION MAGIQUE POUR PLANTER DE L'HERBE <-- NOUVEAU
    public void PlantGrass(int x, int z)
    {
        Vector2Int pos = new Vector2Int(x, z);
        // On ne plante QUE s'il y a une tuile existante
        if (gridPositions.TryGetValue(pos, out Tile currentTile))
        {
            // Et on ne plante QUE sur de la Dirt (Type 1)
            if (currentTile.tileType == 1)
            {
                // Remplacement
                Destroy(currentTile.gameObject);
                SpawnTileAt(x, z, 2); // 2 = Le type Herbe !
                RefreshTile(x, z);
                RefreshNeighbors(x, z); // Important pour recalibrer les voisines

                // VFX d'Herbe
                if (vfxGrassPrefab != null)
                {
                    Vector3 spawnPos = new Vector3(x * tileSize, transform.position.y, z * tileSize) + transform.position;
                    GameObject vfx = Instantiate(vfxGrassPrefab, spawnPos, Quaternion.identity);
                    Destroy(vfx, 2f);
                }
            }
        }
    }
}