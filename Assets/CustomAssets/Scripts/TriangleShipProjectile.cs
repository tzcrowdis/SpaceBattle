using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleShipProjectile : MonoBehaviour
{
    GameObject explosionResource;
    GameObject explosion;

    float lifetime = 20f;
    float t = 0f;

    void Start()
    {
        explosionResource = Resources.Load("ExplosionParticles") as GameObject;
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t >= lifetime)
            DestroyProjectile();
    }

    void DestroyProjectile()
    {
        explosion = Instantiate(explosionResource, transform.position, Quaternion.identity);
        var main = explosion.GetComponent<ParticleSystem>().main;
        main.startColor = Color.yellow;

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);

        DestroyProjectile();

        //do damage if hit ship(   322211111111) <- cats contribution
        //if (collision.gameObject.name.Contains("TriangleShip"))
        //{
            //friendly fire
        //    collision.gameObject.GetComponent<TriangleShipBehaviour>().health -= 1;
        //}
        if (collision.gameObject.name.Contains("CubeShip"))
        {
            collision.gameObject.GetComponent<CubeShipBehaviour>().health -= 1;
        }
    }
}
