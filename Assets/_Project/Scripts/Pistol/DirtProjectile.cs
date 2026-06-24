using UnityEngine;

public class DirtProjectile : MonoBehaviour
{
    public GameObject finalDirtPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        if (finalDirtPrefab != null)
        {
            Vector3 spawnPoint = collision.contacts[0].point;

            Instantiate(finalDirtPrefab, spawnPoint, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
