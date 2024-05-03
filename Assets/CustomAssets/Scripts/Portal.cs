using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    //SPAWN SHIPS AT RATE DEPENDENT ON PORTAL SCALE

    GameObject shipResource;
    GameObject ship;

    //spawn points
    //tier 1
    Transform sp1;

    //tier 2
    Transform sp2;
    Transform sp3;

    //tier 3
    Transform sp4;
    Transform sp5;
    Transform sp6;
    
    int releaseRate = 0;
    public float releaseTime;
    float t;

    void Start()
    {
        shipResource = Resources.Load("TriangleShip") as GameObject;

        sp1 = transform.GetChild(0);
        sp2 = transform.GetChild(1);
        sp3 = transform.GetChild(2);
        sp4 = transform.GetChild(3);
        sp5 = transform.GetChild(4);
        sp6 = transform.GetChild(5);

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
                Instantiate(shipResource, sp1.position, sp1.rotation);
            }
            else if (releaseRate == 2)
            {
                Instantiate(shipResource, sp2.position, sp2.rotation);
                Instantiate(shipResource, sp3.position, sp3.rotation);
            }
            else if (releaseRate == 3)
            {
                Instantiate(shipResource, sp4.position, sp4.rotation);
                Instantiate(shipResource, sp5.position, sp5.rotation);
                Instantiate(shipResource, sp6.position, sp6.rotation);
            }
            
            t = 0f;
        }
    }
}
