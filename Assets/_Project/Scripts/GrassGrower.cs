using UnityEngine;

public class GrassGrower : MonoBehaviour
{
    [Header("Paramètres du Pinceau")]
    public float maxDistance = 50f;
    public float sprayRadius = 0.6f; // Un peu plus grand que tileSize pour déborder un peu
    public float fireRate = 0.05f;
    private float nextFireTime = 0f;

    [Header("Références")]
    public Camera fpsCamera;
    public GridGenerator generator; // Glisse ton objet GridGenerator ici

    [Tooltip("Choisis le Layer 'Tiles' car on doit toucher la dirt !")]
    public LayerMask tileLayer;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
            RaycastHit hit;

            // On tire sur les tiles (on cherche la dirt !)
            if (Physics.Raycast(ray, out hit, maxDistance, tileLayer))
            {
                SprayGrass(hit.point);
            }
        }
    }

    void SprayGrass(Vector3 impactPoint)
    {
        // On aligne les coordonnées par rapport à l'origine de la grille
        Vector3 localImpact = impactPoint - generator.transform.position;

        int centerCol = Mathf.RoundToInt(localImpact.x / generator.tileSize);
        int centerRow = Mathf.RoundToInt(localImpact.z / generator.tileSize);

        int radiusInTiles = Mathf.CeilToInt(sprayRadius / generator.tileSize);

        for (int x = centerCol - radiusInTiles; x <= centerCol + radiusInTiles; x++)
        {
            for (int z = centerRow - radiusInTiles; z <= centerRow + radiusInTiles; z++)
            {
                float distance = Vector2.Distance(new Vector2(centerCol, centerRow), new Vector2(x, z));

                // Si la case est dans le cercle du "pinceau"
                if (distance <= radiusInTiles)
                {
                    // ON APPELLE PLANTGRASS ICI <-- Changement clé
                    generator.PlantGrass(x, z);
                }
            }
        }
    }
}