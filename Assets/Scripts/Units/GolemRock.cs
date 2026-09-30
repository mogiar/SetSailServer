using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRock : Projectile
{
    private Rigidbody cannonBallRB;
    public Transform shotPos;
    public float firePower = 15;
    private float Timer = 0;
    [SerializeField] private float maxTime;
    bool flag;

    // Start is called before the first frame update
    void Start()
    {
        damage = 10f;
        cannonBallRB = GetComponent<Rigidbody>();
        //cannonBallRB.velocity = transform.forward * firePower;
        StartCoroutine("DelayOnStart");
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            transform.position = Vector3.MoveTowards(transform.position, GeneralFunctions.Player.transform.position, 0.1f);
            //Debug.Log(firePower); // (GeneralFunctions.Player.transform.position - transform.position) * firePower
            //cannonBallRB.AddForce((GeneralFunctions.Player.transform.position - transform.position)*80);
            cannonBallRB.velocity = new Vector3(Mathf.MoveTowards(cannonBallRB.velocity.x, 0, 0.3f), cannonBallRB.velocity.y, Mathf.MoveTowards(cannonBallRB.velocity.z, 0, 0.3f));// transform.forward * 40;
            //cannonBallRB.AddForce((GeneralFunctions.Player.transform.position - transform.position), ForceMode.Impulse);
            //firePower = Mathf.Max(firePower - 0.02f, 0);
            //cannonBallRB.AddForce(transform.up * 50);

            //flag = false;
        }

        //Timer += Time.deltaTime;
        //if (Timer > maxTime)
        //{
        //    //gameObject.SetActive(false);
        //    cannonBallRB.velocity = Vector3.zero;
        //    GeneralFunctions.Pooler.ReturnObjectToPool(gameObject);
        //}
    }
    IEnumerator DelayOnStart()
    {
        yield return new WaitForSeconds(0.8f);
        flag = true;
        cannonBallRB.useGravity = true;
        //cannonBallRB.AddForce((GeneralFunctions.Player.transform.position - transform.position), ForceMode.Impulse);
        cannonBallRB.velocity = transform.forward * 80;
        //StartCoroutine("Tick");
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit OnCollisionEnter " + collision.gameObject.name);
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<Unit>().ReciveDamage(this);
            GeneralFunctions.Pooler.ReturnObjectToPool(gameObject);
        }
    }
    public override void OnHit()
    {
        base.OnHit();
    }
}
