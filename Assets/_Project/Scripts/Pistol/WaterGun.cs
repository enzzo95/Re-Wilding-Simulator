using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [Header("Config")]
    public Camera playerCamera;
    public Transform shootPoint;
    public ParticleSystem waterParticles;

    [Header("Settings")]
    public float maxDistance = 15f;

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
                print("[DEBUG]: Dirt collide water");
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
