using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //General Var
    [SerializeField] Rigidbody _rb;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float rayCastOffset; // The offset of rays from ship base
    [SerializeField] float detectionDistance;
    [SerializeField] float sideDetectionDistance;
    public GameObject playerPivot;
    public Transform playerLocation;
    //RayCast Var
    bool rayClear;

    //Patroling Var
    [SerializeField] Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField] float walkPointRange;
    [SerializeField] Transform patrolTarget;


    //Player Chase
    [SerializeField] Transform playerTarget;

    void Update()
    {
        // ------ Patrol code, might do a comeback ----------------

        //Ray ray = new Ray(transform.position, transform.forward);
        //RaycastHit hitInfo;

        //if (Physics.Raycast(ray, out hitInfo, 30)) {
        //    Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        //    rayClear = false;
        //} else {
        //    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 30, Color.green);
        //    rayClear = true;
        //}
        PathFinding();
        Move();

        //float distance = Vector3.Distance(playerLocation.position, transform.position);
        //if (distance < 10)
        //    RotateAround();

    }

    void Move()
    {
        _rb.position += transform.forward * speed * Time.deltaTime;
    }

    void Turn()
    {
        Vector3 pos = playerTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    void PathFinding()
    {
        bool rayHit;
        bool needToTurn = false;
        RaycastHit hit;

        Vector3 left = transform.position - transform.right * rayCastOffset;
        Vector3 right = transform.position + transform.right * rayCastOffset;
        Ray rightRay = new Ray(transform.position, transform.right);
        Ray leftRay = new Ray(transform.position, transform.right * -1);


        Debug.DrawLine(rightRay.origin, rightRay.origin + rightRay.direction * 5, Color.cyan);
        Debug.DrawLine(leftRay.origin, leftRay.origin + leftRay.direction * 5, Color.cyan);
        Debug.DrawRay(left, transform.forward * detectionDistance, Color.cyan);
        Debug.DrawRay(right, transform.forward * detectionDistance, Color.cyan);


        if (Physics.Raycast(left, transform.forward, out hit, detectionDistance))
            needToTurn = true;
        else if (Physics.Raycast(right, transform.forward, out hit, detectionDistance))
            needToTurn = true;

        if (Physics.Raycast(rightRay, out hit, sideDetectionDistance) || Physics.Raycast(leftRay, out hit, sideDetectionDistance))
            rayHit = true;
        else
            rayHit = false;

        if (needToTurn)
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        else if (rayHit == false && needToTurn == false)
            Turn();
    }


}

//public void Patrol()
//{
//    if (!walkPointSet || !rayClear) SearchWalkPoint();

//    if (walkPointSet)
//    {
//        var targetRotation = Quaternion.LookRotation(patrolTarget.transform.position - transform.position);

//        _rb.position = Vector3.MoveTowards(transform.position, patrolTarget.position, speed * Time.deltaTime);
//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//    }

//    Vector3 distanceToWalkPoint = transform.position - walkPoint;
//    //Walkpoint reached
//    if (distanceToWalkPoint.magnitude < 1f)
//        walkPointSet = false;
//}


//private void SearchWalkPoint()
//{
//    //Calculate random point in range
//    float randomZ = Random.Range(-walkPointRange, walkPointRange);
//    float randomX = Random.Range(-walkPointRange, walkPointRange);

//    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
//    patrolTarget.position = walkPoint;
//    //Finished creating a walk point
//    walkPointSet = true;
//}

