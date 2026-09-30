using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Enemy : GameUnit
{
    [SerializeField] protected float visionRange;
    protected float attackRange;
    [SerializeField] protected float chaseSpeed;
    
    //protected Vector3 playerLocation;
    protected float distanceFromPlayer;

    public EnemyChaseState ChaseState;

    public override void Start()
    {
        base.Start();
        ChaseState = new EnemyChaseState(_rb, chaseSpeed);

    }
    public override void Update()
    {
        base.Update();

        //switch state
        distanceFromPlayer = Vector3.Distance(GeneralFunctions.Player.transform.position, _rb.position);
        if (distanceFromPlayer <= attackRange)
        {
            //GeneralFunctions.Player.GetComponent<HealthSystemHandler>().GetDamage();
        }
        else if (distanceFromPlayer <= visionRange)
        {
            currentState = ChaseState;
        }
        else if (distanceFromPlayer > visionRange)
        {
            currentState = PatrolState;
        }

    }
    public override float Attack(FishScript fish)
    {// enemy tell fish how much damage he will do to him
        //Debug.Log("shark does " + attackValue + " damage to fish");
        return attackValue;
    }
    public override float Attack(SharkScript shark)
    {// enemy tell shark how much damage he will do to him
        //Debug.Log("shark does " + attackValue + " damage to shark");
        return attackValue;
    }
    public override float Attack(PlayerStats player)
    {// enemy tell player how much damage he will do to him
        //Debug.Log("shark does " + attackValue + " damage to player");
        return attackValue;
    }


}

