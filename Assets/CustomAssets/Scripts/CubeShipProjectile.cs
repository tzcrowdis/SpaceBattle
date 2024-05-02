using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeShipProjectile : MonoBehaviour
{
    GameObject explosionResource;
    GameObject explosion;
    
    Vector3 rotationVec = new Vector3(0f, 0f, 1f);

    float lifetime = 20f;
    float t = 0f;

    void Start()
    {
        explosionResource = Resources.Load("ExplosionParticles") as GameObject;
    }

    void Update()
    {
        transform.Rotate(rotationVec);
        
        t += Time.deltaTime;
        if (t >= lifetime)
            DestroyProjectile();
    }

    void DestroyProjectile()
    {
        explosion = Instantiate(explosionResource, transform.position, Quaternion.identity);
        var main = explosion.GetComponent<ParticleSystem>().main;
        main.startColor = Color.green;

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);

        DestroyProjectile();

        //do damage if hit ship
        if (collision.gameObject.name.Contains("TriangleShip"))
        {
            collision.gameObject.GetComponent<TriangleShipBehaviour>().health -= 1;
        }
        //else if (collision.gameObject.name.Contains("CubeShip"))
        //{
            //friendly fire
        //    collision.gameObject.GetComponent<CubeShipBehaviour>().health -= 1;
        //}
    }
}
