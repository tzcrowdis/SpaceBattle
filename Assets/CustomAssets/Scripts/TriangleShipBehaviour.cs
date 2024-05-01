using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TriangleShipBehaviour : MonoBehaviour
{
    //combat vars
    public int health;
    GameObject explosionResource;
    GameObject explosion;

    //distances
    public float attackDistance;
    public float evadeDistance;
    public float fleeDistance;
    public float turnDistance;
    public float findDistance;

    //speeds
    public float fleeSpeed;
    public float attackSpeed;
    public float turnSpeed;

    //angles
    public float angleMax;
    float angle;

    int turnDirection;

    //projectile vars
    GameObject projectileResource;
    GameObject projectile;
    Transform shootPosition;
    public float rateOfFire;
    float shootTime;
    float projectileSpeed = 0.5f;

    //enemy vars
    public GameObject enemy;
    float distanceToEnemy;

    //wait vars
    public float waitNewRotationTime;
    float waitTime;

    enum State
    {
        Attack,
        Evade,
        Flee,
        Turn,
        Find
    }
    State state;

    void Start()
    {
        explosionResource = Resources.Load("ExplosionParticles") as GameObject;

        projectileResource = Resources.Load("TriangleProjectile") as GameObject;
        shootPosition = transform.GetChild(0);
        shootTime = 0f;

        turnDirection = 0;

        waitTime = waitNewRotationTime;
        
        state = State.Find;
    }

    void Update()
    {
        GetNextState();
        Act();

        if (health <= 0)
            BlowUp();
    }

    void Act()
    {
        if (state == State.Attack)
            Attack();
        else if (state == State.Evade)
            TurnForFlee();
        else if (state == State.Flee)
            Flee();
        else if (state == State.Turn)
            TurnBackForAttack();
        else if (state == State.Find)
            ApproachAttackRange();
    }

    void GetNextState()
    {
        if (enemy != null)
        {
            distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            angle = Vector3.Angle(transform.forward, (enemy.transform.position - transform.position).normalized);
        }

        if (state == State.Find && distanceToEnemy < attackDistance)
            state = State.Attack;
        else if (state == State.Attack && distanceToEnemy < evadeDistance)
            state = State.Evade;
        else if (state == State.Evade && distanceToEnemy > fleeDistance)
            state = State.Flee;
        else if (state == State.Flee && distanceToEnemy > turnDistance)
            state = State.Turn;
        else if (state == State.Turn && (distanceToEnemy < attackDistance | angle < angleMax))
            state = State.Attack;

        if (enemy == null | distanceToEnemy > findDistance)
            state = State.Find;

        if (state != State.Turn & state != State.Evade & state != State.Find)
            turnDirection = 0;
    }

    void Attack()
    {
        //fly straight towards enemy at attack speed
        transform.rotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
        transform.position += attackSpeed * (enemy.transform.position - transform.position).normalized * Time.deltaTime;

        //shoot projectiles straight ahead at rate of fire
        shootTime += Time.deltaTime;
        if (shootTime >= 1 / rateOfFire)
        {
            projectile = Instantiate(projectileResource, shootPosition.position, shootPosition.rotation);
            projectile.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            projectile.GetComponent<Rigidbody>().velocity = projectileSpeed * transform.forward + attackSpeed * transform.forward;
            shootTime = 0f;
        }
    }

    void TurnForFlee()
    {
        //pick random direction
        if (turnDirection == 0)
            turnDirection = Random.Range(1, 5);

        //turn away from enemy at flee speed
        transform.position += fleeSpeed * transform.forward * Time.deltaTime;
        if (turnDirection == 1)
            transform.Rotate(turnSpeed * Vector3.up);
        else if (turnDirection == 2)
            transform.Rotate(turnSpeed * -Vector3.up);
        else if (turnDirection == 3)
            transform.Rotate(turnSpeed * Vector3.right);
        else if (turnDirection == 4)
            transform.Rotate(turnSpeed * -Vector3.right);
    }

    void Flee()
    {
        //move straight away from enemy at flee speed
        transform.rotation = Quaternion.LookRotation((transform.position - enemy.transform.position).normalized);
        transform.position += fleeSpeed * (transform.position - enemy.transform.position).normalized * Time.deltaTime;
    }

    void TurnBackForAttack()
    {
        //pick random direction
        if (turnDirection == 0)
            turnDirection = Random.Range(1, 5);

        //turn at flee speed
        transform.position += fleeSpeed * transform.forward * Time.deltaTime;
        if (turnDirection == 1)
            transform.Rotate(turnSpeed * Vector3.up);
        else if (turnDirection == 2)
            transform.Rotate(turnSpeed * -Vector3.up);
        else if (turnDirection == 3)
            transform.Rotate(turnSpeed * Vector3.right);
        else if (turnDirection == 4)
            transform.Rotate(turnSpeed * -Vector3.right);
    }

    void ApproachAttackRange()
    {
        if (enemy == null)
            GetNextEnemy();

        if (enemy != null)
        {
            //move straight towards enemy
            transform.rotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
            transform.position += fleeSpeed * (enemy.transform.position - transform.position).normalized * Time.deltaTime;
        }
        else
        {
            //wait for an enemy to be assigned
            if (turnDirection == 0)
                turnDirection = Random.Range(1, 5);

            transform.position += fleeSpeed * transform.forward * Time.deltaTime;
            if (turnDirection == 1)
                transform.Rotate(2 * turnSpeed * Vector3.up);
            else if (turnDirection == 2)
                transform.Rotate(2 * turnSpeed * -Vector3.up);
            else if (turnDirection == 3)
                transform.Rotate(2 * turnSpeed * Vector3.right);
            else if (turnDirection == 4)
                transform.Rotate(2 * turnSpeed * -Vector3.right);

            waitTime += Time.deltaTime;
            if (waitTime >= waitNewRotationTime)
            {
                turnDirection = 0;
                waitTime = 0;
            }
        }
    }

    void GetNextEnemy()
    {
        //get game object of enemy
        //closest or some other criteria
    }

    void BlowUp()
    {
        explosion = Instantiate(explosionResource, transform.position, Quaternion.identity);
        var main = explosion.GetComponent<ParticleSystem>().main;
        main.startSize = 2.5f;
        main.startColor = Color.blue;

        Destroy(gameObject);
    }
}
