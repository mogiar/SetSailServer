using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class EnemyBaseState : MonoBehaviour
{
    // Moving functions Variables
    float rotationSpeed = 0.5f;
    float rayCastOffset = 5; // The offset of rays from ship base
    protected float detectionDistance = 20;
    float sideDetectionDistance = 3;
    protected List<RayObj> rays = new List<RayObj>();

    public Rigidbody _rb;
    //public abstract void EnterState(EnemyStateManager enemy);

    public abstract void UpdateState();



    // General movement functions
    public virtual void Move(Rigidbody _rb, float speed)
    {
        _rb.position += _rb.transform.forward * speed * Time.deltaTime;
    }

    public virtual void Turn(Rigidbody _rb, Vector3 playerLocation)
    {
        Vector3 pos = playerLocation - _rb.transform.position;
        Quaternion rotation = Quaternion.LookRotation(pos);
        _rb.transform.rotation = Quaternion.Slerp(_rb.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    public virtual Vector3 PathFinding(Rigidbody _rb, Vector3 destination, float speed)
    {
        rays = new List<RayObj>();
        
        rays.Add(new RayObj(new Ray(_rb.transform.position, _rb.transform.forward), true));
        rays.Add(new RayObj(new Ray(_rb.transform.position, _rb.transform.right), true));
        rays.Add(new RayObj(new Ray(_rb.transform.position, _rb.transform.forward * -1), true));
        rays.Add(new RayObj(new Ray(_rb.transform.position, _rb.transform.right * -1), true));

        rays = GetMidRays(rays);
        rays = RayCast(rays);
        //PrintRay(rays);

        return GetDestination(rays, destination);
    }
    private Ray GetMidRay(Ray ray1, Ray ray2)
    {
        return new Ray(ray1.origin, ray1.direction + ray2.direction);
    }
    private List<RayObj> GetMidRays(List<RayObj> lst)
    {
        List<RayObj> ans = new List<RayObj>();
        Vector3 temp;
        RayObj prev = new RayObj();
        RayObj first = new RayObj();
        foreach (RayObj element in lst)
        {
            if (!prev.ray.direction.Equals(Vector3.zero))
            {
                if(!(!prev.toCheck && !element.toCheck))
                {
                    ans.Add(prev);
                    temp = prev.ray.direction + element.ray.direction;
                    ans.Add(new RayObj(new Ray(element.ray.origin, new Vector3(temp.x / 2, temp.y, temp.z / 2)), true));
                }
            }
            else
                first = element;
            prev = element;
        }
        ans.Add(prev);
        temp = prev.ray.direction + first.ray.direction;
        ans.Add(new RayObj(new Ray(prev.ray.origin, new Vector3(temp.x / 2, temp.y, temp.z / 2)), true));
        return ans;
    }
    private void PrintRay(List<RayObj> lst)
    {
        foreach (RayObj ray in lst)
        {
            if(ray.toCheck)
                Debug.DrawLine(ray.ray.origin, ray.ray.origin + (ray.ray.direction * detectionDistance), Color.green);
        }
    }
    private List<RayObj> RayCast(List<RayObj> rays)
    {
        //List<RayObj> ans = rays;
        foreach (RayObj element in rays)
        {
            if (element.toCheck)
            {
                RaycastHit[] hits = Physics.RaycastAll(element.ray.origin, element.ray.direction, detectionDistance, 1 << 13);
                if (hits.Length > 0)
                    element.toCheck = false;
            }
        }
        return rays;
    }
    private Vector3 GetDestination(List<RayObj> rays, Vector3 destination)
    {
        float dis = 1000;
        Ray ray = new Ray();
        foreach (RayObj element in rays)
        {
            if (element.toCheck && Vector3.Distance(destination, element.ray.origin + element.ray.direction) < dis)
            {
                dis = Vector3.Distance(destination, element.ray.origin + element.ray.direction);
                ray = element.ray;
            }
        }
        if (ray.direction.Equals(Vector3.zero))
            return destination;
        return ray.origin + (ray.direction * 5);
    }
    public virtual void PathFinding1(Rigidbody _rb, Vector3 playerLocation, float speed)
    {
        bool rayHit;
        bool needToTurnRight = false;
        bool needToTurnLeft = false;
        RaycastHit hit;

        Vector3 left = _rb.transform.position - _rb.transform.right * rayCastOffset;
        Vector3 right = _rb.transform.position + _rb.transform.right * rayCastOffset;
        Ray rightRay = new Ray(_rb.transform.position, _rb.transform.right);
        Ray leftRay = new Ray(_rb.transform.position, _rb.transform.right * -1);


        Debug.DrawLine(rightRay.origin, rightRay.origin + rightRay.direction * 5, Color.green);
        Debug.DrawLine(leftRay.origin, leftRay.origin + leftRay.direction * 5, Color.green);
        Debug.DrawRay(left, _rb.transform.forward * detectionDistance, Color.cyan);
        Debug.DrawRay(right, _rb.transform.forward * detectionDistance, Color.cyan);


        if (Physics.Raycast(left, _rb.transform.forward, out hit, detectionDistance) && Physics.Raycast(right, _rb.transform.forward, out hit, detectionDistance))
        {
            // Return to patrol, how?? + shouldnt happen with continues dynamic
        }

        if (Physics.Raycast(left, _rb.transform.forward, out hit, detectionDistance))
        {
            needToTurnRight = true;
            _rb.transform.Rotate(0, _rb.transform.rotation.y * rotationSpeed, 0);
            Debug.DrawRay(left, _rb.transform.forward * detectionDistance, Color.red);
            Debug.DrawLine(leftRay.origin, leftRay.origin + leftRay.direction * 5, Color.red);

        }
        else if (Physics.Raycast(right, _rb.transform.forward, out hit, detectionDistance))
        {
            needToTurnLeft = true;
            _rb.transform.Rotate(0, -_rb.transform.rotation.y * rotationSpeed, 0);
            Debug.DrawRay(right, _rb.transform.forward * detectionDistance, Color.red);
            Debug.DrawLine(rightRay.origin, rightRay.origin + rightRay.direction * 5, Color.red);
        }
        else
        {
            needToTurnLeft = false; needToTurnRight = false;
        }
        rayHit = false;

        //if (Physics.Raycast(rightRay, out hit, sideDetectionDistance) || Physics.Raycast(leftRay, out hit, sideDetectionDistance))
        //    rayHit = true;
        //else
        //    rayHit = false;

        //if (needToTurnRight)
        //    _rb.transform.Rotate(0, _rb.transform.rotation.y * rotationSpeed, 0);
        //else if (needToTurnLeft)
        //    _rb.transform.Rotate(0, -_rb.transform.rotation.y * rotationSpeed, 0);


        if (!rayHit && !needToTurnRight && !needToTurnLeft)
        {
            Move(_rb, speed);
            Turn(_rb, playerLocation);
        }
    }
     
    public class RayObj
    {
        public Ray ray;
        public bool toCheck;
        public RayObj()
        {
            this.ray = new Ray();
            this.toCheck = false;
        }
        public RayObj(Ray ray, bool toCheck)
        {
            this.ray = ray;
            this.toCheck = toCheck;
        }

    }
}
