using UnityEngine;

public class DirtGrower : MonoBehaviour
{
    [Header("Paramètres")]
    public float maxDistance = 50f;
    public float sprayRadius = 1.5f;
    public float fireRate = 0.05f;
    private float nextFireTime = 0f;

    [Header("Références")]
    public Camera fpsCamera;
    public GridGenerator generator; // Glisse ton objet GridGenerator ici

    [Tooltip("Choisis le Layer 'GroundBase' (Le sol invisible)")]
    public LayerMask groundBaseLayer;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
            RaycastHit hit;

            // On tire sur le sol invisible !
            if (Physics.Raycast(ray, out hit, maxDistance, groundBaseLayer))
            {
                SprayDirt(hit.point);
            }
        }
    }

    void SprayDirt(Vector3 impactPoint)
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
                    generator.GrowDirt(x, z);
                }
            }
        }
    }
}