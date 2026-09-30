using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusEnemy : Enemy
{
    private Transform player;
    private Vector3 rotationTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GeneralFunctions.Player.transform;
        rotationTarget = new Vector3(0, 185, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotationTarget), .05f);
        if (transform.rotation == Quaternion.Euler(rotationTarget))
        {
            rotationTarget = rotationTarget.y  == 185 ? new Vector3(0, 175, 0) : new Vector3(0, 185, 0);
        }
    }
}
