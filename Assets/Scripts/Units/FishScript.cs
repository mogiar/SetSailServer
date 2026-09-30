using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishScript : GameUnit
{    
    [Header("Fish Target")]
    //[SerializeField] private Transform _target;

    //[Header("Fish")]
    //[SerializeField] private float _speed;
    //[SerializeField] private float _minRandomChangeRotationTime;
    //[SerializeField] private float _maxnRandomChangeRotationTime;
    private float _targetRotSpeed;
    //private float _rotSpeed;

    [Header("UI")]
    [SerializeField] private Image healthUI;

    public override void Start()
    {
        //this.maxHP = 1;
        //this.regularSpeed = 2;
        //armorValue = 0;

        _targetRotSpeed = Random.Range(-180f, 180);
        transform.rotation = new Quaternion(transform.rotation.x, _targetRotSpeed, transform.rotation.z, 0);

        base.Start();

        PossibleDrops.Add("FishMeat,1");
        PossibleDrops.Add("RedFish,1");
        PossibleDrops.Add("StandardFish,1");
        PossibleDrops.Add("FishMeat,1");
        PossibleDrops.Add("RedFish,1");
        PossibleDrops.Add("StandardFish,1");
        PossibleDrops.Add("FishMeat,1");
        PossibleDrops.Add("RedFish,1");
        PossibleDrops.Add("StandardFish,1");
        PossibleDrops.Add("FishMeat,1");
        PossibleDrops.Add("RedFish,1");
        PossibleDrops.Add("StandardFish,1");
        PossibleDrops.Add("FishMeat,1");
        PossibleDrops.Add("RedFish,1");
        PossibleDrops.Add("StandardFish,1");
        PossibleDrops.Add("FishMeat,1");
        PossibleDrops.Add("RedFish,1");
        PossibleDrops.Add("StandardFish,1");
        PossibleDrops.Add("YellowFish,2");
        PossibleDrops.Add("YellowFish,2");
        PossibleDrops.Add("YellowFish,2");
        PossibleDrops.Add("GreenFish,3");
        PossibleDrops.Add("Coins,2");
        PossibleDrops.Add("Coins,2");
        PossibleDrops.Add("Coins,2");

        //transform.Rotate(0, _targetRotSpeed * Time.deltaTime, 0, Space.Self);
        //StartCoroutine("NewRotationTarget");
    }

    public override void Encounter(Visitor attacker) // 10 - 5
    {
        float damage = attacker.Attack(this); // attacker return the damage he will does to fish
        currentHP += Mathf.Min(0, armorValue - damage); // calc damage
        healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);
    }
    override public void ReciveDamage(Projectile item)
    {
        currentHP += Mathf.Min(0, armorValue - item.DoDamage(this));
        healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);
    }
    // Update is called once per frame
    //void Update()
    //{
    //    //Rotate();
    //    //healthUI.transform.LookAt(Camera.main.transform);
    //}

    //private void FixedUpdate()
    //{
    //    //Move();
    //}

    //private void Rotate()
    //{
    //    transform.LookAt(_target, Vector3.up);
    //    _rotSpeed = Mathf.Lerp(_rotSpeed, _targetRotSpeed, Time.deltaTime);
    //    transform.Rotate(0, _targetRotSpeed * Time.deltaTime, 0, Space.Self);
    //}

    //private void Move()
    //{
    //    _rb.MovePosition(transform.position + (transform.forward * _speed * Time.deltaTime));              
    //}

    //IEnumerator NewRotationTarget()
    //{
    //    _targetRotSpeed = Random.Range(-25f, 25f);
    //    yield return new WaitForSeconds(Random.Range(_minRandomChangeRotationTime,
    //                                                _maxnRandomChangeRotationTime));
    //    StartCoroutine("NewRotationTarget");
    //}


}
