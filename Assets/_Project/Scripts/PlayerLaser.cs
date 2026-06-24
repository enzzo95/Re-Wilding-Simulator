using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [Header("Paramètres du Laser")]
    public float maxDistance = 50f;
    public float sprayRadius = 1.5f;

    [Tooltip("Délai entre deux tirs pour éviter le lag (0.05 = parfait)")]
    public float fireRate = 0.05f;
    private float nextFireTime = 0f;

    [Header("Références")]
    public Camera fpsCamera;

    [Tooltip("N'oublie pas de créer le Layer 'Tiles' et de le sélectionner ici !")]
    public LayerMask tileLayer;

    void Update()
    {
        if (fpsCamera == null) return;

        // On vérifie le clic ET on s'assure que le temps du délai est passé
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
            RaycastHit hit;

            // Le Raycast ne s'embête à taper QUE sur le calque Tiles
            if (Physics.Raycast(ray, out hit, maxDistance, tileLayer))
            {
                Vector3 impactPoint = hit.point;

                // L'OverlapSphere ne cherche QUE les objets sur le calque Tiles (Divise le lag par 10)
                Collider[] collidersInSpray = Physics.OverlapSphere(impactPoint, sprayRadius, tileLayer);

                foreach (Collider col in collidersInSpray)
                {
                    Tile tileHit = col.GetComponentInParent<Tile>();

                    if (tileHit != null)
                    {
                        tileHit.Rewild();
                    }
                }
            }
        }
    }
}