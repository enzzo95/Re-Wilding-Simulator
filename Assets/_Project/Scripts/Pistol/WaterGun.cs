using System.Collections.Generic;
using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [Header("Config")]
    public Camera playerCamera;
    public Transform shootPoint;
    public ParticleSystem waterParticles;

    [Header("Settings")]
    public float maxDistance = 15f; 
    public float sprayRadius = 0.6f;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var emission = waterParticles.emission;
        emission.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ShootWater();
        }

        else
        {
            StopWater();
        }
    }

    void ShootWater()
    {
        if (playerCamera == null || waterParticles == null) return;

        var emission = waterParticles.emission;
        if (!emission.enabled)
        {
            emission.enabled = true;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Dirt"))
            {
                GridGenerator generator = Object.FindFirstObjectByType<GridGenerator>();
                if (generator == null) return;

                Vector3 impactPoint = hit.point;
                Vector3 localImpact = impactPoint - generator.transform.position;

                int centerCol = Mathf.RoundToInt(localImpact.x / generator.tileSize);
                int centerRow = Mathf.RoundToInt(localImpact.z / generator.tileSize);

                int radiusInTiles = Mathf.CeilToInt(sprayRadius / generator.tileSize);

                for (int x = centerCol - radiusInTiles; x <= centerCol + radiusInTiles; x++)
                {
                    for (int z = centerRow - radiusInTiles; z <= centerRow + radiusInTiles; z++)
                    {
                        float distance = Vector2.Distance(new Vector2(centerCol, centerRow), new Vector2(x, z));

                        if (distance <= radiusInTiles)
                        {
                            generator.PlantGrass(x, z);
                        }
                    }
                }
            }
        }
    }

    void StopWater()
    {
        if (waterParticles == null) return;

        var emission = waterParticles.emission;
        if (emission.enabled)
        {
            emission.enabled = false;
        }
    }
}
