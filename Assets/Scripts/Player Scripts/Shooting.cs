using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Rigidbody cannonBallRB;
    public Transform shotPos;
    public float firePower = 10;

    public float Timer = 0;



    // Update is called once per frame
    void Update()
    {
        cannonBallRB.AddForce(transform.forward * firePower);
        Timer += Time.deltaTime;

        if (Timer > 5)
        {
            gameObject.SetActive(false);
            cannonBallRB.velocity = Vector3.zero;
        }

        Timer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
