using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleShipBehaviour : MonoBehaviour
{
    public float attackDistance;
    public float evadeDistance;
    public float fleeDistance;
    public float turnDistance;
    public float findDistance;

    public float fleeSpeed;
    public float attackSpeed;
    public float turnSpeed;

    int turnDirection;

    GameObject projectileResource;
    GameObject projectile;
    Transform shootPosition;
    public float rateOfFire;
    float shootTime;
    float projectileSpeed = 0.5f;

    public GameObject enemy;
    float distanceToEnemy;

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
        projectileResource = Resources.Load("TriangleShipProjectile") as GameObject;
        shootPosition = transform.GetChild(0);
        shootTime = 0f;

        turnDirection = 0;
        
        state = State.Find;
    }

    void Update()
    {
        GetNextState();
        Act();
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
        distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

        if (state == State.Find && distanceToEnemy < attackDistance)
            state = State.Attack;
        else if (state == State.Attack && distanceToEnemy < evadeDistance)
            state = State.Evade;
        else if (state == State.Evade && distanceToEnemy > fleeDistance)
            state = State.Flee;
        else if (state == State.Flee && distanceToEnemy > turnDistance)
            state = State.Turn;
        else if (state == State.Turn && distanceToEnemy < attackDistance)
            state = State.Attack;

        if (enemy == null | distanceToEnemy > findDistance)
            state = State.Find;

        if (state != State.Turn | state != State.Evade)
            turnDirection = 0;
    }

    void Attack()
    {
        //fly straight towards enemy at attack speed
        transform.rotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
        transform.position += attackSpeed * (enemy.transform.position - transform.position).normalized * Time.deltaTime;

        //shoot projectiles straight ahead at rate of fire
        shootTime += Time.deltaTime;
        if (shootTime >= rateOfFire)
        {
            projectile = Instantiate(projectileResource, shootPosition.position, Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
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
            transform.Rotate(turnSpeed * transform.up);
        else if (turnDirection == 2)
            transform.Rotate(turnSpeed * -transform.up);
        else if (turnDirection == 3)
            transform.Rotate(turnSpeed * transform.right);
        else if (turnDirection == 4)
            transform.Rotate(turnSpeed * -transform.right);
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
            transform.Rotate(turnSpeed * transform.up);
        else if (turnDirection == 2)
            transform.Rotate(turnSpeed * -transform.up);
        else if (turnDirection == 3)
            transform.Rotate(turnSpeed * transform.right);
        else if (turnDirection == 4)
            transform.Rotate(turnSpeed * -transform.right);
    }

    void ApproachAttackRange()
    {
        if (enemy == null)
            GetNextEnemy();

        //move straight towards enemy
        transform.rotation = Quaternion.LookRotation((enemy.transform.position - transform.position).normalized);
        transform.position += fleeSpeed * (enemy.transform.position - transform.position).normalized * Time.deltaTime;
    }

    void GetNextEnemy()
    {
        //get game object of enemy
        //closest or some other criteria
    }
}
