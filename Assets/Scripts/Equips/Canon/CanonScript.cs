using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CanonScript : EquipScript
{
    [SerializeField] private Transform target;
    [SerializeField] private float maxRot;
    [SerializeField] private float minRot;
    private GameObject CanonAmmoPrefab;
    private Animator anim;


    public Transform shotPos;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(target);
        transform.localEulerAngles = new Vector3(0, Mathf.Clamp(transform.localEulerAngles.y, minRot, maxRot), 0);
    }

    //[ServerRpc(RequireOwnership = false)]
    override public void UseAbilltyServerRp()
    {
        Debug.Log("CanonFire");
        anim.Play("CanonFire");
        //UseAbilltyServerRp1();
        //cannonBall = ObjectPooler.Instance;
        //GameObject objectToSpawn = Instantiate(CanonAmmoPrefab, shotPos.position, shotPos.rotation);
        //objectToSpawn.SetActive(true);
    }
    //[ServerRpc]
    private void UseAbilltyServerRp1()
    {
        Debug.Log("UseAbilltyClientRpc cannon fire");
        
    }

    override public void SetTarget(Transform target)
    {
        //this.target = target;
    }
    
    //[ServerRpc]
    public void ShootProjectileServerRp()
    {
        //Debug.Log(shotPos.rotation);
        //Debug.Log(shotPos.localEulerAngles);
        Debug.Log("ShootProjectile1");
        ShootProjectileServerRpc();
        //GameObject objectToSpawn = Instantiate(CanonAmmoPrefab, shotPos.position, shotPos.rotation);
        //objectToSpawn.SetActive(true);
    }

    [ServerRpc]
    private void ShootProjectileServerRpc()
    {
        //Debug.Log(shotPos.rotation);
        //Debug.Log(shotPos.localEulerAngles);
        Debug.Log("ShootProjectileServerRpc");
        ShootProjectileClientRpc();
        //GameObject objectToSpawn = Instantiate(CanonAmmoPrefab, shotPos.position, shotPos.rotation);
        //objectToSpawn.SetActive(true);
    }
    [ClientRpc]
    private void ShootProjectileClientRpc()
    {
        Debug.Log("ShootProjectileClientRpc");
        Shoot();
    }
    private void Shoot()
    {
        Debug.Log("shoot");
        GameObject ammo = GeneralFunctions.Pooler.SpawnFromPool(GeneralFunctions.AmmoName, new Vector2(shotPos.position.x, shotPos.position.z), new Quaternion(shotPos.eulerAngles.x, shotPos.eulerAngles.y, shotPos.eulerAngles.z, 0));
    }
}
