using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour
{
    //public GameObject inventory;
    public GameObject mainStore;
    public Transform target;

    private Transform RightEquip;
    private Transform LeftEquip;
    private Transform MidEquip;
    
    void Start()
    {
        //inventory = GameObject.Find("ShipInventory");
        RightEquip = transform.Find("RightEquip");
        LeftEquip = transform.Find("LeftEquip");
        MidEquip = transform.Find("MidEquip");

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;
    }
    private void OnTriggerEnter(Collider other)
    {
        // when player hit drop
        if (other.gameObject.layer == 6) // drop layer
        {
            if (GeneralFunctions.inventoryControl.PickUpItem(other))
            {
                GeneralFunctions.Pooler.ReturnObjectToPool(other.transform.parent.gameObject);
                GeneralFunctions.Player.GetComponent<PlayerStats>().AddExp((float)other.GetComponent<DropScript>().drop.rarity);
            }
                
        }
        
            
    }
    private void OnTriggerExit(Collider other)
    { // when player leave store radius
        if (other.gameObject.layer == 7) // store layer
        {
            //other.GetComponent<StoreInteraction>().ExitStore();
        }

    }

    [ServerRpc]
    public void SetEquipModelServerRpc(string equipName, SideEquip side)
    {
        Debug.Log("SetEquipModelServerRpc"); 
        SetEquipModelClientRpc(equipName, side);
    }
    [ClientRpc]
    public void SetEquipModelClientRpc(string equipName, SideEquip side)
    {
        Debug.Log("SetEquipModelClientRpc client");
        if (equipName == null)
        {
            TurnOffEquipModel(side);
            return;
        }
        Transform equip = AllDrops.GetEquipByName(equipName);
        //Transform ammo = AllDrops.GetAmmoByName("Ammo"+equipName);
        //GameObject ammo = GeneralFunctions.Pooler.SpawnFromPool(GeneralFunctions.AmmoName, Vector2.zero);
        Transform equipObject = Instantiate(equip);

        switch (side)
        {
            case SideEquip.Left:
                SetEquipOnPlayer(LeftEquip, equipObject, equip);
                equipObject.localRotation = Quaternion.Euler(new Vector3(equipObject.localEulerAngles.x + LeftEquip.localEulerAngles.x, 180, equipObject.localEulerAngles.z));
                equipObject.localPosition = new Vector3(-1 * (equip.localPosition.x + LeftEquip.localPosition.x), LeftEquip.localPosition.y + equip.localPosition.y, 0);
                break;
            case SideEquip.Right:
                SetEquipOnPlayer(RightEquip, equipObject, equip);
                break;
            case SideEquip.Mid:
                SetEquipOnPlayer(MidEquip, equipObject, equip);
                break;
        }
    }
    public void TurnOffEquipModel(SideEquip side)
    {
        switch (side)
        {
            case SideEquip.Left:
                TurnOffEquip(LeftEquip);
                break;
            case SideEquip.Right:
                TurnOffEquip(RightEquip);
                break;
            case SideEquip.Mid:
                TurnOffEquip(MidEquip);
                break;
        }
    }
    public void UseAbillity(SideEquip side)
    {
        switch (side)
        {
            case SideEquip.Left:
                UseAbilityEquip(LeftEquip);
                break;
            case SideEquip.Right:
                UseAbilityEquip(RightEquip);
                break;
            case SideEquip.Mid:
                UseAbilityEquip(MidEquip);
                break;
        }
    }

    private void SetEquipOnPlayer(Transform SideEquip, Transform equipObject, Transform equip)
    {
        if (SideEquip.childCount > 0)
        {
            var prevEuipt = SideEquip.GetChild(0);
            prevEuipt.SetParent(null);
            //prevEuipt.gameObject.SetActive(false);
            Destroy(prevEuipt.gameObject);
        }
        equipObject.SetParent(SideEquip);
        equipObject.localPosition = new Vector3(equip.localPosition.x - SideEquip.localPosition.x, SideEquip.localPosition.y + equip.localPosition.y, 0);
        equipObject.localRotation = Quaternion.Euler(SideEquip.localEulerAngles + equip.localEulerAngles);
        var EuiptRight = SideEquip.GetComponentsInChildren<EquipScript>();
        var script = EuiptRight[0];
        //if (ammo != null)
        //    script.SetAmmo(ammo.gameObject);
        equipObject.gameObject.SetActive(true);
    }
    private void TurnOffEquip(Transform SideEquip)
    {
        var prevEuiptleft = SideEquip.GetChild(0);
        prevEuiptleft.SetParent(null);
        Destroy(prevEuiptleft.gameObject);
    }
    
    private void UseAbilityEquip(Transform SideEquip)
    {
        if (!IsOwner)
            return;
        var EquipSide = SideEquip.GetComponentsInChildren<EquipScript>();
        if (EquipSide.Length == 0)
            Debug.Log("can't yet use ability");
        else
        {
            var script = EquipSide[0];
            script.SetTarget(target);
            if (GeneralFunctions.inventoryControl.inventory.thereIsEnoughFromItem(GeneralFunctions.AmmoName, script.GetAmmoNumber()))
            {
                GeneralFunctions.inventoryControl.inventory.RemoveItemFromPlayer(script.GetAmmoNumber(), GeneralFunctions.AmmoName);
                Debug.Log("script.UseAbilltyClientRpc(); on playerInteraction");
                Debug.Log(script.ammoAmount);
                //script.UseAbilltyServerRp();

                Debug.Log(((CanonScript)script).shotPos.position);
                var data = new CannonNetworkData()
                {
                    Position = ((CanonScript)script).shotPos.position,
                    Rotation = ((CanonScript)script).shotPos.rotation.eulerAngles
                };
                ShootProjectileServerRpc(data);
            }
            else
            {
                Debug.Log("there is not eoungh of " + GeneralFunctions.AmmoName);
            }
        }
    }

    [ServerRpc]
    private void ShootProjectileServerRpc(CannonNetworkData data)
    {
        //Debug.Log(shotPos.rotation);
        //Debug.Log(shotPos.localEulerAngles);
        Debug.Log("ShootProjectileServerRpc on player " + OwnerClientId);
        ShootProjectileClientRpc(data);
        //GameObject objectToSpawn = Instantiate(CanonAmmoPrefab, shotPos.position, shotPos.rotation);
        //objectToSpawn.SetActive(true);
    }
    [ClientRpc]
    private void ShootProjectileClientRpc(CannonNetworkData data)
    {
        Debug.Log("shoot on player");
        Debug.Log(data.Position);
        Debug.Log(data.Rotation);
        //GameObject ammo = GeneralFunctions.Pooler.SpawnFromPool(GeneralFunctions.AmmoName, new Vector2(shotPos.position.x, shotPos.position.z), new Quaternion(shotPos.eulerAngles.x, shotPos.eulerAngles.y, shotPos.eulerAngles.z, 0));
        GameObject ammo = GeneralFunctions.Pooler.SpawnFromPool(GeneralFunctions.AmmoName, new Vector2(data.Position.x/100, data.Position.z/100), new Quaternion(data.Rotation.x, data.Rotation.y, data.Rotation.z, 0));
        Debug.Log(ammo);
    }

    struct CannonNetworkData : INetworkSerializable
    {
        private short _x, _y, _z;
        private short _xRot, _yRot, _zRot;


        internal Vector3 Position
        {
            get => new Vector3(_x, _y, _z);
            set
            {
                _x = (short)(value.x * 100);
                _y = (short)(value.y * 100);
                _z = (short)(value.z * 100);
            }
        }
        internal Vector3 Rotation
        {
            get => new Vector3((float)_xRot / 100, (float)_yRot / 100, (float)_zRot / 100);
            set
            {
                _xRot = (short)(value.x * 100);
                _yRot = (short)(value.y * 100);
                _zRot = (short)(value.z * 100);
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _x);
            serializer.SerializeValue(ref _y);
            serializer.SerializeValue(ref _z);

            serializer.SerializeValue(ref _xRot);
            serializer.SerializeValue(ref _yRot);
            serializer.SerializeValue(ref _zRot);
        }
    }



}
