using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TelescopeScript : EquipScript
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //[ServerRpc]
    public override void UseAbilltyServerRp()
    {
        Camera.main.GetComponent<CameraScript>().TelescopeButton(0);
    }
    [ServerRpc]
    public override void AbilityOffServerRpc()
    {
        Camera.main.GetComponent<CameraScript>().TelescopeButton(1);
    }
}
