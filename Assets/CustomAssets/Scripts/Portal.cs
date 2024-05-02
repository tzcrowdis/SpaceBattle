using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    //SPAWN SHIPS AT RATE DEPENDENT ON PORTAL SCALE

    GameObject shipResource;
    GameObject ship;
    
    int releaseRate = 0;
    public float releaseTime;
    float t;

    void Start()
    {
        shipResource = Resources.Load("TriangleShip") as GameObject;
        t = releaseTime;
    }

    void Update()
    {
        if (releaseRate > 0f)
            ReleaseShips();
    }

    public void SetRate(float min, float mid, float max)
    {
        if (transform.localScale.x == min)
            releaseRate = 1;
        else if (transform.localScale.x == mid)
            releaseRate = 2;
        else if (transform.localScale.x == max)
            releaseRate = 3;
    }

    void ReleaseShips()
    {
        t += Time.deltaTime;
        if (t > releaseTime)
        {
            if (releaseRate == 1)
            {
                Instantiate(shipResource, transform.position + 1.1f * transform.forward, transform.rotation);
            }
            else if (releaseRate == 2)
            {
                Instantiate(shipResource);
                Instantiate(shipResource);
            }
            else if (releaseRate == 3)
            {
                Instantiate(shipResource);
                Instantiate(shipResource);
                Instantiate(shipResource);
            }
            
            t = 0f;
        }
    }
}
