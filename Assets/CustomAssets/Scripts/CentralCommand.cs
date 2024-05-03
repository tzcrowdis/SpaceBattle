using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class CentralCommand : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();
    List<GameObject> triangles = new List<GameObject>();
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public GameObject NearestEnemy(GameObject ship)
    {
        GameObject target = null;
        if (ship.name.Contains("Triangle"))
        {
            if (cubes.Count > 0)
            {
                float dist;
                float minDistance = Mathf.Infinity;
                foreach (GameObject cube in cubes)
                {
                    if (cube != null)
                    {
                        dist = Vector3.Distance(ship.transform.position, cube.transform.position);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            target = cube;
                        }
                    }
                }
            }
        }
        else
        {
            if (triangles.Count > 0)
            {
                float dist;
                float minDistance = Mathf.Infinity;
                foreach (GameObject triangle in triangles)
                {
                    if (triangle != null)
                    {
                        dist = Vector3.Distance(ship.transform.position, triangle.transform.position);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            target = triangle;
                        }
                    }
                }
            }
        }

        return target;
    }

    public void AddTriangle(GameObject triangle)
    {
        triangles.Add(triangle);
    }

    public void RemoveShip(GameObject ship)
    {
        if (gameObject.name.Contains("Triangle"))
            triangles.Remove(gameObject);
        else
            cubes.Remove(gameObject);
    }
}
