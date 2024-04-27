using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeShipProjectile : MonoBehaviour
{
    float lifetime = 20f;
    float t = 0f;

    void Update()
    {
        t += Time.deltaTime;
        if (t >= lifetime)
            DestroyProjectile();
    }

    void DestroyProjectile()
    {
        //add effects here

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);

        DestroyProjectile();

        //do damage if hit ship
    }
}
