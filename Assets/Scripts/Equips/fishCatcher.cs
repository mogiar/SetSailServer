using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class fishCatcher : EquipScript
{
    private Material collectMatt;
    private float fill = 0;
    [SerializeField] private float fillSpeed = 1;
    private int fishContact = 0;

    // Start is called before the first frame update
    void Start()
    {
        collectMatt = gameObject.GetComponent<Renderer>().material;
        
    }

    // Update is called once per frame
    void Update()
    {
        // GenerateRay();

        if (fishContact <= 0 && fill > 0)
        {
            fill -= Time.deltaTime * fillSpeed / 2;
            collectMatt.SetFloat("Vector1_faa8736e76db49a38dfb4dc943da72dc", fill);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 12 && IsFish(other.name))
        {
            if (fill >= 1) //1
            {
                fill = 0;
                other.GetComponent<FishScript>().Death(); // fishScript
                fishContact--;
            }

            fill += Time.deltaTime * fillSpeed;
            collectMatt.SetFloat("Vector1_faa8736e76db49a38dfb4dc943da72dc", fill);
        }
    }    

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 12 && IsFish(other.name))
        {
            fishContact--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12 && IsFish(other.name))
        {
            fishContact++;
        }
    }
    private void GenerateRay()
    {
        RaycastHit hit;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y - 3f, transform.position.z);
        Ray fishRay = new Ray(transform.position, Vector3.down);

        if (Physics.CapsuleCast(transform.position, endPos, 3f, Vector3.one, out hit,3f, 1 << 12) && IsFish(hit.rigidbody.gameObject.name))
        {
            if (fill >= 1)
            {
                fill = 0;
                Debug.Log("CaughtFish");
            }

            //Debug.Log("HitFish");
            fill += Time.deltaTime * fillSpeed;
            collectMatt.SetFloat("Vector1_faa8736e76db49a38dfb4dc943da72dc", fill);
        }
        else if (fill > 0)
        {
            fill -= Time.deltaTime * fillSpeed / 2;
            collectMatt.SetFloat("Vector1_faa8736e76db49a38dfb4dc943da72dc", fill);
        }
        /*

        if (Physics.CheckCapsule(transform.position, endPos, 3f, 1 << 15))
        {
            if (fill >= 1)
            {
                fill = 0;
                Debug.Log("CaughtFish");
            }

            //Debug.Log("HitFish");
            fill += Time.deltaTime * fillSpeed;
            collectMatt.SetFloat("Vector1_faa8736e76db49a38dfb4dc943da72dc", fill);
        }
        else if (fill > 0)
        {
            fill -= Time.deltaTime * fillSpeed / 2;
            collectMatt.SetFloat("Vector1_faa8736e76db49a38dfb4dc943da72dc", fill);
        }*/
    }
    private bool IsFish(string name)
    {
        return new Regex(@"([a-zA-Z]+)").Match(name).Value.ToLower().Equals("fish");
    }
}
