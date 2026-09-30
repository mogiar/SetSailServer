using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    
    bool firstRun = true;

    float speed;
    int counter = 0;
    float timer = 0;
    public float secondTimer = 0;
    // Patroling Var
    public Vector3 walkPoint;
    public Vector3 lastOkWalkPoint;
    public Vector3 patrolTarget;
    public Vector3 rotationTarget;
    private float _targetRotSpeed;
    bool walkPointSet;
    float walkPointRangeX = 20;
    float walkPointRangeZ = 10;

    public EnemyPatrolState(Rigidbody rigidbody, float speed)
    {
        _rb = rigidbody;
        patrolTarget = _rb.transform.position + new Vector3(5, 0, 5);
        SearchWalkPoint();
        rotationTarget = patrolTarget;
        this.speed = speed;

        // ----- change
        _targetRotSpeed = Random.Range(-25f, 25f);
    }
    public void SetUp()
    {
        patrolTarget = _rb.transform.position + new Vector3(5, 0, 5);
        firstRun = true;
        SearchWalkPoint();
        rotationTarget = patrolTarget;
    }
    public override void UpdateState()
    {
        Patrol();
        //Debug.Log(secondTimer);
        
        // ---- change
        _rb.transform.Rotate(0, _targetRotSpeed * Time.deltaTime, 0, Space.Self);
    }
    public void Patrol()
    {
        
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            if (Vector3.Distance(patrolTarget, _rb.position) > 50)
            {
                //Debug.Log("too far");
                SetUp();
            }

            Debug.DrawLine(_rb.transform.position, walkPoint);
            lastOkWalkPoint = walkPoint;
            
            if (secondTimer > 4)
            {
                secondTimer = 0;
                rotationTarget = PathFinding(_rb, patrolTarget, speed);
                
            }
            PrintRay(rays);  
            Turn(_rb, rotationTarget);
            Move(_rb, speed);

            if (timer > 10)
            {
                timer = 0;
                patrolTarget = lastOkWalkPoint;
                SearchWalkPoint();
            }
        }

        Vector3 distanceToWalkPoint = _rb.transform.position - walkPoint;
        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 8)
            walkPointSet = false;
        timer += 1 * Time.deltaTime;
        secondTimer += 1 * Time.deltaTime;

    }
    private void PrintRay(List<RayObj> lst)
    {
        foreach (RayObj ray in lst)
        {
            if (ray.toCheck)
                Debug.DrawLine(ray.ray.origin, ray.ray.origin + (ray.ray.direction * base.detectionDistance), Color.green);
        }
    }

    void SearchWalkPoint()
    {
        
        // If it's the first time we generate a random number to decide patrol direction
        if (firstRun)
        {
            int randomNumber = Random.Range(0, 3);
            //Debug.Log("number:" + randomNumber);
            if (randomNumber == 1)
                counter = 0;
            else
                counter = 15;
        }
        firstRun = false;
        timer = 0;

        float randomZ = 0;
        float randomX = 0;

        //Calculate random point in range
        if (counter < 15)
        {
            randomZ = Random.Range(5, walkPointRangeZ);
            randomX = Random.Range(5, walkPointRangeX);
            counter++;
        }
        else if (counter >= 15)
        {
            // ---- change
            _targetRotSpeed = Random.Range(-25f, 25f); 

            randomZ = Random.Range(-walkPointRangeZ, 5);
            randomX = Random.Range(-walkPointRangeX, 5);
            counter++;
            if (counter == 30)
                counter = 0;
        }
        walkPoint = new Vector3(patrolTarget.x + randomX, patrolTarget.y, patrolTarget.z + randomZ);
        patrolTarget = walkPoint;
        //isHit = false;

        //Finished creating a walk point
        walkPointSet = true;

        
    }

    

}
