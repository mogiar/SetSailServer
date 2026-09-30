using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    float speed;

    //RayCast Var
    bool rayClear;

    public EnemyChaseState(Rigidbody rigidbody, float speed)
    {
        this._rb = rigidbody;
        this.speed = speed;
    }
    public override void UpdateState()
    {
        Debug.Log("EnemyChaseState");
        var rotationTarget = PathFinding(_rb, GeneralFunctions.Player.transform.position, speed);
        Turn(_rb, rotationTarget);
        Move(_rb, speed);
    }

}
