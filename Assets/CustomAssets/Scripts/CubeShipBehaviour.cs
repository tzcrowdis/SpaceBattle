using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using UnityEngine;

public class CubeShipBehaviour : MonoBehaviour
{
    //combat vars
    public int health;
    GameObject explosionResource;
    GameObject explosion;
    
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
    Vector3 patrolDirection = Vector3.up; //temp, should get from central command

    //charge vars
    Vector3 center;
    public float jitter;
    float x;
    float y;
    float z;

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
        explosionResource = Resources.Load("ExplosionParticles") as GameObject;
        
        moving = false;
        directions = new List<Vector3>{
                transform.up,
                -transform.up,
                transform.right,
                -transform.right,
                transform.forward,
                -transform.forward
            };

        projectileResource = Resources.Load("CubeProjectile") as GameObject;
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

        if (health <= 0)
            BlowUp();
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
        //charge up animation
        if (shootTime == 0)
            center = transform.position;
        x = Random.Range(center.x - jitter, center.x + jitter);
        y = Random.Range(center.y - jitter, center.y + jitter);
        z = Random.Range(center.z - jitter, center.z + jitter);
        transform.position = new Vector3(x, y, z);

        //shoot projectile from shoot position at enemy
        shootTime += Time.deltaTime;
        if (shootTime >= shootEndTime)
        {
            transform.position = center;
            shootPivot.rotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
            
            projectile = Instantiate(projectileResource, shootPosition.position, shootPosition.rotation);
            projectile.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            projectile.GetComponent<Rigidbody>().velocity = projectileSpeed * (enemy.transform.position - shootPosition.position).normalized;

            reloading = true;
            moving = false;
            shootTime = 0;
        }
    }

    void Patrol()
    {
        //move slowly in given direction
        transform.position += patrolSpeed * patrolDirection * Time.deltaTime;
    }

    void BlowUp()
    {
        explosion = Instantiate(explosionResource, transform.position, Quaternion.identity);
        var main = explosion.GetComponent<ParticleSystem>().main;
        main.startSize = 3f;
        main.startColor = Color.white;

        Destroy(gameObject);
    }
}
