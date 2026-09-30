using System.Collections;
using UnityEngine;


public class HarponProjectile : Projectile
{
    override public float DoDamage(SharkScript shark)
    {
        GeneralFunctions.GameManager.FloatingText("-" + (damage + GeneralFunctions.Player.GetComponent<PlayerStats>().GetPlayerDamage()).ToString(),
                                                    Color.red, this.transform.position);
        return damage + GeneralFunctions.Player.GetComponent<PlayerStats>().GetPlayerDamage();
    }

    override public float DoDamage(FishScript fish)
    {
        float a = Mathf.Ceil(Mathf.Max(1f, Random.Range((damage + GeneralFunctions.Player.GetComponent<PlayerStats>().GetPlayerDamage()) * 0.5f
            , (damage + GeneralFunctions.Player.GetComponent<PlayerStats>().GetPlayerDamage()) * 1.5f)));

        GeneralFunctions.GameManager.FloatingText("-" + a, Color.red, this.transform.position);

        return a;
    }
    //override public float DoDamage(EnemyShipScript shark)
    //{
    //    return damage;
    //}
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter HarponProjectile");
        if (other.gameObject.layer == 12)
        {
            other.GetComponent<Unit>().ReciveDamage(this);
        }
    }
}
