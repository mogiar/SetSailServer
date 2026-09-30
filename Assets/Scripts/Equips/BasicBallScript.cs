using System.Collections;
using UnityEngine;
using Unity.Netcode;


public class BasicBallScript : Projectile
{

    private Rigidbody cannonBallRB;
    public Transform shotPos;
    public float firePower = 10;
    private float Timer = 0;
    [SerializeField] private float maxTime;


    private void Start()
    {
        damage = 10f;
        cannonBallRB = GetComponent<Rigidbody>();
        cannonBallRB.velocity = transform.forward * firePower;
        
    }
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > maxTime)
        {
            //gameObject.SetActive(false);
            cannonBallRB.velocity = Vector3.zero;
            GeneralFunctions.Pooler.ReturnObjectToPool(gameObject);
            
        }
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12 || other.gameObject.layer == 10)
        {
            //Debug.Log("hit " + other.name);
            other.GetComponent<Unit>().ReciveDamage(this);
            GeneralFunctions.Pooler.ReturnObjectToPool(gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("hit OnCollisionEnter " + collision.gameObject.name);
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<Unit>().ReciveDamage(this);
            GeneralFunctions.Pooler.ReturnObjectToPool(gameObject);
        }
    }

    //override public float DoDamage(EnemyShipScript shark)
    //{
    //    return damage;
    //}

    public override void OnHit()
    {
        base.OnHit();
    }
}
