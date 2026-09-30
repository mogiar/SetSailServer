using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] Transform target;
    bool canShoot;
    [SerializeField] float hitRange;

    public Collider coll;


    void Update()
    {
        // RayCast
        Ray ray = new Ray(transform.position, transform.right);
        Ray ray2 = new Ray(transform.position, transform.right*-1);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 15, Color.green);
        Debug.DrawLine(ray2.origin, ray2.origin + ray2.direction * 15, Color.green);

        RaycastHit hitInfo;
        if (coll.Raycast(ray, out hitInfo, hitRange) || coll.Raycast(ray2, out hitInfo, hitRange))
        {
            Debug.Log("SHoot");
            canShoot = true;
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 15, Color.red);
            Debug.DrawLine(ray2.origin, ray2.origin + ray2.direction * 15, Color.red);
        }

    }

}
