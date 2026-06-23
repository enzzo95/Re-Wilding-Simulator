using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LazerGun : MonoBehaviour
{
    [Header("Config")]
    public Camera playerCamera;
    public Transform shootPoint;
    public LineRenderer lineRenderer;

    [Header("Laser Settings")]
    public float maxDistance = 100f;
    public float destroyTime = 5f;

    [Header("UI")]
    public Slider progressBar;

    private float timer = 0f;
    private GameObject lastHitObject = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressBar.gameObject.SetActive(false);
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
            ResetLaser();
        }
    }

    void ShootLaser()
    {
        if (shootPoint == null || playerCamera == null) return;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shootPoint.position);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPosition = hit.point;

            if (hit.collider.CompareTag("Concrete"))
            {
                if (hit.collider.gameObject == lastHitObject)
                {
                    timer += Time.deltaTime;
                    progressBar.value = timer;
                    progressBar.gameObject.SetActive(true);

                    if (timer >= destroyTime)
                    {
                        Destroy(hit.collider.gameObject);
                        ResetLaser();
                    }
                }
                else
                {
                    lastHitObject = hit.collider.gameObject;
                    timer = 0f;
                    progressBar.value = 0f;
                }
            }
            else
            {
                ResetLaser();
            }
        }

        else
        {
            targetPosition = ray.origin + (ray.direction * maxDistance);
            ResetLaser();
        }

        targetPosition += Random.insideUnitSphere * 0.02f;
        lineRenderer.SetPosition(1, targetPosition);
    }

    void ResetLaser()
    {
        timer = 0f;
        lastHitObject = null;
        progressBar.gameObject.SetActive(false);
    }
}
