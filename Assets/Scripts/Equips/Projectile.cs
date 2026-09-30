using System.Collections;
using UnityEngine;
using Unity.Netcode;


public class Projectile : MonoBehaviour
{
    public float damage;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    virtual public float DoDamage(FishScript fish)
    {
        return 0;
    }
    virtual public float DoDamage(SharkScript shark)
    {
        return 0;
    }

    virtual public void OnHit()
    {
        
    }
}
