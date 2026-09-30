using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, Visitor
{
    [SerializeField] protected float currentHP;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float attackValue;
    [SerializeField] protected float armorValue;
    protected Rigidbody _rb;
    protected Transform healthBarTransform;

    public virtual void Start()
    {
        _rb = this.gameObject.GetComponent<Rigidbody>();
        healthBarTransform = GeneralFunctions.Pooler.SpawnFromPool("HealthBar", new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)).transform;
        
        currentHP = maxHP;
        //healthBarTransform.Find("Bar").localScale = new Vector3((currentHP / maxHP), 1);

    }
    protected virtual void OnEnable()
    {
        if (healthBarTransform != null)
        {
            healthBarTransform = GeneralFunctions.Pooler.SpawnFromPool("HealthBar", new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)).transform;
        }
        //currentHP = maxHP;
        //healthBarTransform = GeneralFunctions.Pooler.SpawnFromPool("HealthBar", new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)).transform;
    }
    public virtual void Update()
    {
        healthBarTransform.position = new Vector3(gameObject.transform.position.x, healthBarTransform.position.y, gameObject.transform.position.z);
        if (isDead())
        {
            Death();
        }
        
    }
    virtual public void Death()
    {
        currentHP = maxHP;
        GeneralFunctions.Pooler.ReturnObjectToPool(gameObject);
        GeneralFunctions.Pooler.ReturnObjectToPool(healthBarTransform.gameObject);
    }

    virtual public void Spawn()
    {

    }

    virtual public void ReciveDamage(Projectile item)
    {
        GeneralFunctions.GameManager.FloatingText("-" + item.damage.ToString(), Color.red, this.transform.position);
    }
    public bool isDead()
    {
        if (currentHP <= 0)
            return true;
        return false;
    }
    public virtual void Encounter(Visitor visitor)
    {
        
    }
    public virtual float Attack(FishScript fish)
    { // unit tell fish how much damage he will do to him
        return 0;
    }
    public virtual float Attack(SharkScript shark)
    {// unit tell shark how much damage he will do to him
        return 0;
    }
    public virtual float Attack(PlayerStats player)
    {// unit tell player how much damage he will do to him
        return 0;
    }
    protected virtual void OnCollisionEnter(Collision collision)
    { // unit met collision, so unit let collision know that he met him.
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 10)
        {
            //Debug.Log(this.gameObject.name + " met " + collision.gameObject.name);
            this.Encounter(collision.gameObject.GetComponent<Unit>());

        }
    }
    




}
