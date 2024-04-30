using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeShipProjectile : MonoBehaviour
{
    Vector3 rotationVec = new Vector3(0f, 0f, 1f);

    float lifetime = 20f;
    float t = 0f;

    void Update()
    {
        transform.Rotate(rotationVec);
        
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
