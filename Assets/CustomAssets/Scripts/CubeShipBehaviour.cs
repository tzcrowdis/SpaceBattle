using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using UnityEngine;

public class CubeShipBehaviour : MonoBehaviour
{
    public float moveDistance;
    public float moveSpeed;
    int moveDirection;
    Vector3 direction;
    Vector3 startPosition;

    public float patrolSpeed;

    public float turnSpeed;
    Vector3 lookRotation;
    float turnTime;
    bool turnDone;

    GameObject projectileResource;
    GameObject projectile;
    public float projectileSpeed;
    Transform shootPosition;
    public float shootEndTime;
    float shootTime;
    bool reloading;
    
    public GameObject enemy;
    
    enum State
    {
        Move,
        Spin,
        Shoot,
        Patrol
    }
    State state;

    void Start()
    {
        moveDirection = 0;

        turnTime = 0f;
        turnDone = false;

        projectileResource = Resources.Load("CubeShipProjectile") as GameObject;
        shootPosition = transform.GetChild(0);
        shootTime = 0f;
        reloading = false;
        
        state = State.Patrol;
    }

    void Update()
    {
        GetSetState();
        
        lookRotation = (enemy.transform.position - transform.position).normalized;
        lookRotation.x = 0f;
        lookRotation.z = 0f;

        Debug.Log(state);
    }

    void GetSetState()
    {
        if (enemy != null)
        {
            if (state == State.Patrol)
                state = State.Move;
            else if (state == State.Move & Vector3.Distance(transform.position, startPosition) >= moveDistance)
                state = State.Spin;
            else if (state == State.Spin & turnDone)
                state = State.Shoot;
            else if (state == State.Shoot & reloading)
                state = State.Move;
        }
        else
        {
            state = State.Patrol;
        }

        if (state == State.Move)
            Move();
        else if (state == State.Spin)
            Spin();
        else if (state == State.Shoot)
            Shoot();
        else if (state == State.Patrol)
            Patrol();
    }

    void Move()
    {
        //pick vector to best move away from enemy
        //current implementation is random
        if (moveDirection == 0)
        {
            moveDirection = Random.Range(1, 6);
            switch (moveDirection)
            {
                case 1: //up
                    direction = transform.up;
                    break;
                case 2: //down
                    direction = -transform.up;
                    break;
                case 3: //right
                    direction = transform.right;
                    break;
                case 4: //left
                    direction = -transform.right;
                    break;
                case 5: //forwards
                    direction = transform.forward;
                    break;
                case 6: //backwards
                    direction = -transform.forward;
                    break;
            }
            startPosition = transform.position;
            reloading = false;
            turnDone = false;
        }
            
        transform.position += moveSpeed * direction * Time.deltaTime;
    }

    void Spin()
    {
        //rotate around y axis to face enemy

        //lookRotation = (enemy.transform.position - transform.position).normalized;
        //lookRotation.x = 0f;
        //lookRotation.z = 0f;
        //transform.Rotate(new Vector3(0f, turnSpeed, 0f));
        
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(transform.forward), Quaternion.Euler(lookRotation), turnSpeed * Time.deltaTime);

        turnTime += turnSpeed * Time.deltaTime;
        if (turnTime >= 1f)
        {
            turnDone = true;
        }
    }

    void Shoot()
    {
        //shoot projectile from shoot position at enemy
        shootTime += Time.deltaTime;
        if (shootTime >= shootEndTime)
        {
            projectile = Instantiate(projectileResource, shootPosition.position, transform.rotation);
            projectile.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            projectile.GetComponent<Rigidbody>().velocity = projectileSpeed * (enemy.transform.position - shootPosition.position).normalized;

            reloading = true;
            moveDirection = 0;
            shootTime = 0;
        }
    }

    void Patrol()
    {
        //meander around slowly
    }
}
