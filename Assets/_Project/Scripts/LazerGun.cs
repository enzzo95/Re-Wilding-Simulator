using System.Collections;
using UnityEngine;

public class LazerGun : MonoBehaviour
{
    [Header("Config")]
    public Camera playerCamera;
    public Transform shootPoint;
    public LineRenderer lineRenderer;

    [Header("Laser Settings")]
    public float maxDistance = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ShootLaser();
        }

        else
        {
            lineRenderer.enabled = false;
        }
    }

    void ShootLaser()
    {
        if (shootPoint == null || playerCamera == null) return;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shootPoint.position);

        // Rayon
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPosition = hit.point;
        }

        else
        {
            targetPosition = ray.origin + (ray.direction * maxDistance);
        }

        lineRenderer.SetPosition(1, targetPosition);
    }
}
