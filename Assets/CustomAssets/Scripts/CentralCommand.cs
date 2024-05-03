using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class CentralCommand : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();
    List<GameObject> triangles = new List<GameObject>();

    //returns the nearest ship that isn't already targeted
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
                        if (dist < minDistance & !Targeted(cube))
                        {
                            minDistance = dist;
                            target = cube;
                        }
                    }
                }

                //in case all ships are targeted focus remaining fleet
                if (cubes.Count > 0 & target == null)
                    target = cubes[0];
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
                        if (dist < minDistance & !Targeted(triangle))
                        {
                            minDistance = dist;
                            target = triangle;
                        }
                    }
                }

                //in case all ships are targeted focus remaining fleet
                if (triangles.Count > 0 & target == null)
                    target = triangles[0];
            }
        }

        return target;
    }

    //returns true if a friendly ship is already attacking the target
    bool Targeted(GameObject target)
    {
        if (target.name.Contains("Cube"))
        {
            foreach (GameObject triangle in triangles)
            {
                if (triangle != null && triangle.GetComponent<TriangleShipBehaviour>().enemy == target)
                    return true;
            }
        }
        else
        {
            foreach (GameObject cube in cubes)
            {
                if (cube != null && cube.GetComponent<CubeShipBehaviour>().enemy == target)
                    return true;
            }
        }

        return false;
    }

    //adds triangle
    public void AddTriangle(GameObject triangle)
    {
        triangles.Add(triangle);
    }

    //removes ship from list
    public void RemoveShip(GameObject ship)
    {
        if (gameObject.name.Contains("Triangle"))
            triangles.Remove(gameObject);
        else
            cubes.Remove(gameObject);
    }
}
