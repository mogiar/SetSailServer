using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Unity.Netcode;



public class EquipScript : NetworkBehaviour
{
    public int ammoAmount;

    //[ServerRpc(RequireOwnership = false)]
    virtual public void UseAbilltyServerRp()
    {
        Debug.Log("UseAbilltyServerRpc on EquipScript");
    }
    [ServerRpc]
    virtual public void AbilityOffServerRpc()
    {
        
    }

    virtual public void SetTarget(Transform target)
    {

    }
    virtual public int GetAmmoNumber()
    {
        return ammoAmount;
    }
    virtual public void SetAmmo(GameObject ammo)
    {

    }
    


}
