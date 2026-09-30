using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameUnit : Unit
{

    [SerializeField] protected List<string> PossibleDrops;
    [SerializeField] protected int experienceValue;
    [SerializeField] protected float regularSpeed;

    public EnemyBaseState currentState;

    public EnemyIdleState IdleState;
    public EnemyPatrolState PatrolState;

    public override void Start()
    {
        base.Start();

        //healthBarTransform.gameObject.SetActive(false);
        PossibleDrops = new List<string>();

        IdleState = new EnemyIdleState();
        PatrolState = new EnemyPatrolState(_rb, regularSpeed);
        currentState = PatrolState;
    }
    public override void Update()
    {
        base.Update();
        currentState.UpdateState();
    }
    protected void RandomDrop()
    {
        int randomNumb = Random.Range(0, PossibleDrops.Count);
        string dropName = PossibleDrops[randomNumb];
        string[] splitString = dropName.Split(',');
        AllDrops.CreateDrop(splitString[0], int.Parse(splitString[1]), gameObject.transform.position + new Vector3(2, 0, 2));
    }
    public override void Death()
    {
        RandomDrop();
        base.Death();
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == 11)
        {
            base.Death();
            //GeneralFunctions.Pooler.ReturnObjectToPool(gameObject);
        }
    }
}

