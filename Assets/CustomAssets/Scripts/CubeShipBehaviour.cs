using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using UnityEngine;

public class CubeShipBehaviour : MonoBehaviour
{
    //movement vars
    public float moveDistance;
    public float moveSpeed;
    bool moving;
    Vector3 direction;
    Vector3 startPosition;
    Vector3 opposite;
    float minAngle;
    float angle;
    List<Vector3> directions;

    //patrol vars
    public float patrolSpeed;

    //shooting vars
    GameObject projectileResource;
    GameObject projectile;
    public float projectileSpeed;
    Transform shootPivot;
    Transform shootPosition;
    public float shootEndTime;
    float shootTime;
    bool reloading;
    
    public GameObject enemy;
    
    enum State
    {
        Move,
        Shoot,
        Patrol
    }
    State state;

    void Start()
    {
        moving = false;
        directions = new List<Vector3>{
                transform.up,
                -transform.up,
                transform.right,
                -transform.right,
                transform.forward,
                -transform.forward
            };

        projectileResource = Resources.Load("CubeShipProjectile") as GameObject;
        shootPivot = transform.GetChild(0);
        shootPosition = shootPivot.GetChild(0);
        shootTime = 0f;
        reloading = false;
        
        state = State.Patrol;
    }

    void Update()
    {
        GetState();
        Act();
    }

    void GetState()
    {
        if (enemy != null)
        {
            if (state == State.Patrol)
                state = State.Move;
            else if (state == State.Move & Vector3.Distance(transform.position, startPosition) >= moveDistance)
                state = State.Shoot;
            else if (state == State.Shoot & reloading)
                state = State.Move;
        }
        else
        {
            state = State.Patrol;
        }
    }

    void Act()
    {
        if (state == State.Move)
            Move();
        else if (state == State.Shoot)
            Shoot();
        else if (state == State.Patrol)
            Patrol();
    }

    void Move()
    {
        //pick axis to best move away from enemy
        if (!moving)
        {
            opposite = (transform.position - enemy.transform.position).normalized;
            minAngle = 180f;
            foreach (Vector3 dir in directions)
            {
                angle = Vector3.Angle(opposite, dir);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    direction = dir;
                }
            }

            startPosition = transform.position;
            moving = true;
            reloading = false;
        }
            
        transform.position += moveSpeed * direction * Time.deltaTime;
    }

    void Shoot()
    {
        //shoot projectile from shoot position at enemy
        shootTime += Time.deltaTime;

        //TODO: play a charge up animation here

        if (shootTime >= shootEndTime)
        {
            shootPivot.rotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
            
            projectile = Instantiate(projectileResource, shootPosition.position, shootPosition.rotation);
            projectile.transform.Rotate(new Vector3(90f, 0f, 0f));
            projectile.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            projectile.GetComponent<Rigidbody>().velocity = projectileSpeed * (enemy.transform.position - shootPosition.position).normalized;

            reloading = true;
            moving = false;
            shootTime = 0;
        }
    }

    void Patrol()
    {
        //meander around slowly
    }
}
