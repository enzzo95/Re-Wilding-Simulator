using UnityEngine;

public class DirtProjectile : MonoBehaviour
{
    public float sprayRadius = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    void OnCollisionEnter(Collision collision)
    {
        // On évite que le caillou s'auto-exécute s'il frôle le joueur
        if (collision.gameObject.CompareTag("Player")) return;

        // 1. On récupère le point d'impact exact de la collision physique
        Vector3 impactPoint = collision.contacts[0].point;

        // 2. On cherche dynamiquement le GridGenerator présent dans la scène
        GridGenerator generator = Object.FindFirstObjectByType<GridGenerator>();

        if (generator != null)
        {
            // 3. On applique la logique du pinceau de terre à l'endroit de l'impact
            Vector3 localImpact = impactPoint - generator.transform.position;

            int centerCol = Mathf.RoundToInt(localImpact.x / generator.tileSize);
            int centerRow = Mathf.RoundToInt(localImpact.z / generator.tileSize);

            int radiusInTiles = Mathf.CeilToInt(sprayRadius / generator.tileSize);

            for (int x = centerCol - radiusInTiles; x <= centerCol + radiusInTiles; x++)
            {
                for (int z = centerRow - radiusInTiles; z <= centerRow + radiusInTiles; z++)
                {
                    float distance = Vector2.Distance(new Vector2(centerCol, centerRow), new Vector2(x, z));

                    // Si la case de la grille se trouve dans le rayon de l'impact du projectile
                    if (distance <= radiusInTiles)
                    {
                        generator.GrowDirt(x, z);
                    }
                }
            }
        }

        // 4. On détruit le projectile physique puisqu'il s'est écrasé au sol
        Destroy(gameObject);
    }
}
