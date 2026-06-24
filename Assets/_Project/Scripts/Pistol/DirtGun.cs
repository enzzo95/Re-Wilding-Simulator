using UnityEngine;

public class DirtGun : MonoBehaviour
{
    [Header("Config")]
    public Camera playerCamera;
    public Transform shootPoint;
    public GameObject dirtProjectilePrefab;

    [Header("Settings")]
    public float maxDistance = 100f;
    public float fireRate = 0.1f;
    public float hTarget = 1.5f;

    [Header("Flux Dispersion (Game Feel)")]
    public bool applySpread = true;
    public float spreadAngle = 1.5f;

    private float nextFireTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            ShootProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootProjectile()
    {
        if (shootPoint == null || playerCamera == null || dirtProjectilePrefab == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPoint = hit.point;
        }

        else
        {
            targetPoint = ray.origin + (ray.direction * maxDistance);
        }

        int fragmentsCount = 1;

        for (int i = 0; i < fragmentsCount; i++)
        {
            Vector3 fragmentedTarget = targetPoint;
            fragmentedTarget += new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle)
            ) * (Vector3.Distance(shootPoint.position, targetPoint) * 0.08f);

            Quaternion randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

            GameObject projectile = Instantiate(dirtProjectilePrefab, shootPoint.position, randomRotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float randomMultiplier = Random.Range(0.1f, 0.25f);

                Vector3 irregularScale = new Vector3(
                    Random.Range(0.05f, 0.15f),
                    Random.Range(0.05f, 0.15f),
                    Random.Range(0.05f, 0.15f)
                );
                projectile.transform.localScale = irregularScale;

                Vector3 velocity = CalculateBallisticVelocity(shootPoint.position, fragmentedTarget, hTarget);

                velocity += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                rb.linearVelocity = velocity;

                rb.angularVelocity = new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), Random.Range(-15f, 15f));
            }
        }
    }

    Vector3 CalculateBallisticVelocity(Vector3 start, Vector3 end, float h)
    {
        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);

        float minH = Mathf.Max(0.1f, displacementY + 0.1f);
        if (h < minH)
        {
            h = minH;
        }

        float gravity = Mathf.Abs(Physics.gravity.y);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * h);

        float timeUp = Mathf.Sqrt(2 * h / gravity);
        float timeDown = Mathf.Sqrt(2 * Mathf.Abs(displacementY - h) / gravity);
        float totalTime = timeUp + timeDown;

        if (totalTime <= 0f)
        {
            totalTime = 0.01f;
        }

        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }
}
