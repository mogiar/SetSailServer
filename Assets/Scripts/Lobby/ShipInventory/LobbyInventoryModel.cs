using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInventoryModel
{
    public ItemData[] PlayerEquipInventory;
    public ItemData[] PlayerUseableInventory;
    public ItemData[] PlayerMaterialInventory;
    public ItemData[] LoadoutInventory;

    public ItemData RightEquip;
    public ItemData LeftEquip;
    public ItemData UltimateEquip;

    private const string EQUIP_SLOT = "ShipEquipSlot";
    private const string INVENTORY_SLOT = "Inventory";
    private const string LOADOUT_SLOT = "LoadoutSlot";
    private const string ULTIMATE_EQUIP_SLOT = "UltimateEquipSlot";

    public void SwapSlotsByView(LobbyInventorySlot from, LobbyInventorySlot to)
    {
        ItemData a = GetItemFromSlot(from);
        ItemData b = GetItemFromSlot(to);

        SetItemBySlot(from, b);
        SetItemBySlot(to, a);
    }

    private ItemData GetItemFromSlot(LobbyInventorySlot slot)
    {
        if (slot.slotData == null) return null;

        ItemData a = null;
        /*
        if (slot.slotType.("Left"))
        {
            a = LeftEquip;
            LeftEquip = null;
        }

        if (slot.name.Contains("Right"))
        {
            a = RightEquip;
            RightEquip = null;
        }

        if (slot.name.Contains(ULTIMATE_EQUIP_SLOT))
        {
            a = UltimateEquip;
            UltimateEquip = null;
        }

        if (slot.name.Contains(INVENTORY_SLOT))
        {
            if (slot.slotData.Type == eItemType.Equip || slot.slotData.Type == eItemType.UltimateEquip)
            {
                a = PlayerEquipInventory[slot.slotNumber];
                PlayerEquipInventory[slot.slotNumber] = null;
            }

            if (slot.slotData.Type == eItemType.Useable)
            {
                a = PlayerUseableInventory[slot.slotNumber];
                PlayerUseableInventory[slot.slotNumber] = null;
            }

            if (slot.slotData.Type == eItemType.Materials)
            {
                a = PlayerMaterialInventory[slot.slotNumber];
                PlayerMaterialInventory[slot.slotNumber] = null;
            }
        }

        if (slot.name.Contains(LOADOUT_SLOT))
        {
            a = LoadoutInventory[slot.slotNumber];
            LoadoutInventory[slot.slotNumber] = null;
        }*/

        switch (slot.slotType)
        {
            case eSlotType.inventoryEquip:
                a = PlayerEquipInventory[slot.slotNumber];
                PlayerEquipInventory[slot.slotNumber] = null;
                break;

            case eSlotType.inventoryMaterial:
                a = PlayerMaterialInventory[slot.slotNumber];
                PlayerMaterialInventory[slot.slotNumber] = null;
                break;

            case eSlotType.inventoryUseable:
                a = PlayerUseableInventory[slot.slotNumber];
                PlayerUseableInventory[slot.slotNumber] = null;
                break;

            case eSlotType.shipEquipRight:
                a = RightEquip;
                RightEquip = null;
                break;

            case eSlotType.shipEquipLeft:
                a = LeftEquip;
                LeftEquip = null;
                break;

            case eSlotType.shipEquipUltimate:
                a = UltimateEquip;
                UltimateEquip = null;
                break;

            case eSlotType.shipLoadout:
                a = LoadoutInventory[slot.slotNumber];
                LoadoutInventory[slot.slotNumber] = null;
                break;

            default:
                break;
        }

        return a;
    }

    private void SetItemBySlot(LobbyInventorySlot slot, ItemData item)
    {
        //if (slot.name.Contains("Right"))
        //{
        //    RightEquip = item;
        //}

        //if (slot.name.Contains("Left"))
        //{
        //    LeftEquip = item;
        //}

        //if (slot.name.Contains(ULTIMATE_EQUIP_SLOT))
        //{
        //    UltimateEquip = item;
        //}

        //if (slot.name.Contains(INVENTORY_SLOT))
        //{
        //    if (slot.name.Contains("Equip"))
        //    {
        //        PlayerEquipInventory[slot.slotNumber] = item;
        //    }

        //    if (slot.name.Contains("Useable"))
        //    {
        //        PlayerUseableInventory[slot.slotNumber] = item;
        //    }

        //    if (slot.name.Contains("Material"))
        //    {
        //        PlayerMaterialInventory[slot.slotNumber] = item;
        //    }
        //}

        //if (slot.name.Contains(LOADOUT_SLOT))
        //{
        //    LoadoutInventory[slot.slotNumber] = item;
        //}

        switch (slot.slotType)
        {
            case eSlotType.inventoryEquip:
                PlayerEquipInventory[slot.slotNumber] = item;
                break;

            case eSlotType.inventoryMaterial:
                PlayerMaterialInventory[slot.slotNumber] = item;
                break;

            case eSlotType.inventoryUseable:
                PlayerUseableInventory[slot.slotNumber] = item;
                break;

            case eSlotType.shipEquipRight:
                RightEquip = item;
                break;

            case eSlotType.shipEquipLeft:
                LeftEquip = item;
                break;

            case eSlotType.shipEquipUltimate:
                UltimateEquip = item;
                break;

            case eSlotType.shipLoadout:
                LoadoutInventory[slot.slotNumber] = item;
                break;

            default:
                break;
        }
    }
}
public class ItemData
{
    public int ID;
    public int UniqueID;
    public string Name;
    public Sprite Icon; //Todo - Move to global object 
    public eItemType Type;
    public string Description; //Todo - Move to global object 
    public int Amount; // -1 -> remove item
    public Mesh Model; // Todo - Move to global object 
    public int UpgradeSlot;

    public ItemData(int id, string name, Sprite icon, eItemType type, int amount)
    {
        ID = id;
        Name = name;
        Icon = icon;
        Type = type;
        Amount = amount;
    }
    public ItemData()
    {

    }
}

public enum eItemType
{
    Equip,
    UltimateEquip,
    Useable,
    Materials
}
