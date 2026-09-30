using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SideEquip
{
    Right, Left, Mid, NoSide
}
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/inventory")]
public class InventoryObject : ScriptableObject
{
    public InventorySlot[] Container;
    public int maximumAmount; // max Amount that every slot can hold
    public int numberOpenSlots;

    public InventoryObject(int numberOpenSlots, int numberLockedSlot, int numberEquipSlot, int sizeSlot)
    {
        maximumAmount = sizeSlot;
        this.numberOpenSlots = numberOpenSlots;
        Container = new InventorySlot[numberOpenSlots + numberLockedSlot + numberEquipSlot];

        for (int i = 0; i < numberOpenSlots; i++)
        {
            Container[i] = new InventorySlot(sizeSlot, ItemType.Default); // create available slots
        }
        for (int i = numberOpenSlots; i < (numberOpenSlots + numberLockedSlot); i++)
        {
            Container[i] = new InventorySlot(); // create lock slot
        }
        for (int i = (numberOpenSlots + numberLockedSlot); i < Container.Length; i++)
        { // bot , mid, top
            Container[i] = new InventorySlot(sizeSlot, ItemType.Equips); // create equip slot
        }
        Container[Container.Length - 2] = new InventorySlot(sizeSlot, ItemType.Spells);
    }
    public InventoryObject(int numberOpenSlots, int sizeSlot)
    { // for store
        maximumAmount = sizeSlot;
        Container = new InventorySlot[numberOpenSlots];
        for (int i = 0; i < Container.Length; i++)
        {
            Container[i] = new InventorySlot(sizeSlot, ItemType.Default); // create slot
        }
    }

    public int AddItem(Drop _drop, int index, int amountOnPickDrop, bool tryFromStart)
    { // by index
        int remainAmount = Container[index].AddAmount(_drop, index, amountOnPickDrop);
        while (remainAmount > 0) // if left some Amount, try to place it in another slot
        {
            if (index + 1 >= numberOpenSlots)
            {
                if (tryFromStart)
                {
                    index = -1; // try one time from the start
                    tryFromStart = false;
                }
                else // tell the player that there is no more space
                    return -1;
            }
            remainAmount = AddItem(_drop, index + 1, remainAmount, tryFromStart);
        }
        return remainAmount;
    }
    public int AddItem(Drop _drop, int amountOnPickDrop)
    { // try to insert item in the first free slot
        bool isE = false;
        if (_drop.type == ItemType.Equips || _drop.type == ItemType.Spells)
            isE = true;
        int index = checkFreeSlot(_drop.name, isE);
        if (index == -1)
            return -1;
        int remainAmount = Container[index].AddAmount(_drop, index, amountOnPickDrop);
        //Container[index].currentAmount = Container[index].drop.GetAmount();
        bool tryFromStart = true; // sometimes there is still free slots at the start
        while (remainAmount > 0)
        {
            //int remainAmount_temp = remainAmount;
            if (index + 1 >= numberOpenSlots)
            {
                if (tryFromStart)
                {
                    index = -1; // try one time from the start
                    tryFromStart = false;
                }
                else // tell the player that there is no more space
                {
                    GeneralFunctions.GameManager.FloatingText("FULL", Color.black, GeneralFunctions.Player.transform.position);
                    return -1;
                }
                    
            }
            remainAmount = AddItem(_drop, index + 1, remainAmount, tryFromStart);
        }
        return remainAmount;
    }

    public void AddE(Drop _drop, int index)
    {
        Container[index] = new InventorySlot(_drop, 8, ItemType.Equips);
        if (index == Container.Length - 2)
            Container[index] = new InventorySlot(_drop, 8, ItemType.Spells);
    }
    public InventorySlot[] ConvertToUI()
    {
        return Container;
    }
    public InventorySlot[] SwapSlots(int indexFrom, int indexTo)
    {
        if (isEquipSlot(indexTo) && !isEquipItem(indexFrom))
            return null;
        if (isSpellSlot(indexTo) && !isSpellItem(indexFrom))
            return null;
        InventorySlot temp = Container[indexFrom - 1].RemoveAmount(-1); // remove all Amount 
        if (!Container[indexTo - 1].isEmpty()) // check
        {
            Container[indexFrom - 1] = new InventorySlot(Container[indexTo - 1], Container[indexFrom - 1].Equip);
        }
        Container[indexTo - 1] = new InventorySlot(temp, Container[indexTo - 1].Equip);
        return Container;
    }
    public void ResetInventoryByAmount(int amount)
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].drop != null && Container[i].drop.type != ItemType.Ammo && Container[i].drop.type != ItemType.Gold)
                Container[i].currentAmount = amount;
        }
    }

    public bool isEquipSlot(int index)
    {
        if (Container.Length < index -1)
            Debug.Log("bug");
        return Container[index - 1].isEquip();
    }
    public bool isSpellSlot(int index)
    {
        return Container[index - 1].isSpell();
    }
    public bool isEquipItem(int index)
    {
        if (Container.Length < index - 1)
            Debug.Log("bug");
        return (Container[index - 1].drop != null && Container[index - 1].drop.type == ItemType.Equips);
    }
    public bool isSpellItem(int index)
    {
        return (Container[index - 1].drop != null && Container[index - 1].drop.type == ItemType.Spells);
    }
    public InventorySlot RemoveSlot(int indexFrom)
    {
        return Container[indexFrom - 1].RemoveAmount(-1);
    }
    public int AmountItemOnInventory(string itemName)
    {
        int ans = 0;
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].drop.name.Equals(itemName))
                ans += Container[i].currentAmount;
        }
        return ans;
    }
    public float GetRarityByIndex(int index)
    {
        return Container[index - 1].drop.rarity;
    }
    public int GetAmountByIndex(int index)
    {
        return Container[index - 1].currentAmount;
    }
    public bool thereIsEnoughFromItem(string itemName, int count)
    {
        if (itemName.Length == 0)
            return true;
        int sum = 0;
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].drop != null && Container[i].drop.name.Equals(itemName))
                sum += Container[i].currentAmount;
        }
        return sum >= count;
    }
    public void RemoveItemFromPlayer(int amount, string itemToTrade)
    {
        int current = Mathf.Min(amount, maximumAmount);
        //if (current < maximumAmount)
        amount -= current;
        if (current > 0)
        {
            foreach (var slot in Container)
            {
                if (!slot.isEmpty())
                {
                    int slotAmaount = slot.currentAmount;
                    if (current > 0 && slot.drop.name.Equals(itemToTrade) && slotAmaount < maximumAmount)
                    {
                        var temp = slot.RemoveAmount(current);
                        //current = Mathf.Max(current - slotAmaount, 0);
                        current -= slotAmaount;
                        amount += current;
                        current = Mathf.Min(amount, maximumAmount);
                        amount -= current;
                    }
                }
            }
        }
        if (current > 0)
        {
            foreach (var slot in Container)
            {
                if (!slot.isEmpty())
                {
                    int slotAmaount = slot.currentAmount;
                    if (current > 0 && slot.drop.name.Equals(itemToTrade))
                    {
                        var temp = slot.RemoveAmount(current);
                        //current = Mathf.Max(current - slotAmaount, 0);
                        current -= slotAmaount;
                        amount += current;
                        current = Mathf.Min(amount, maximumAmount);
                        amount -= current;
                    }
                }
            }
        }

    }
    public string GetDropNameByIndex(int index)
    {
        if (Container[index - 1].drop == null)
            return null;
        return Container[index - 1].drop.name;
    }
    public SideEquip GetSideByIndex(int index)
    {// bot , mid, top
        int n = Container.Length;
        if (index == n)
            return SideEquip.Right;
        if (index == n - 1)
            return SideEquip.Mid;
        if (index == n - 2)
            return SideEquip.Left;
        return SideEquip.NoSide;
    }
    private int checkFreeSlot(string dropName, bool isE)
    { // return the first available slot in container 
        if (!isE)
        {
            for (int i = 0; i < Container.Length; i++)
            {
                if (checkEqual(i, dropName)) // return slot with the same type
                    return i;
            }
        }
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].drop == null && !Container[i].isEquip()) // return free slot
                return i;
        }
        return -1;
    }
    private bool checkEqual(int index, string dropName)
    {
        return (Container[index].drop != null && Container[index].drop.name.Equals(dropName) && !Container[index].isFull() && !Container[index].isEquip());
    }
    public InventorySlot GetDropByIndex(int index)
    {
        return Container[index - 1];
    }
    public bool isEmptySlot(int index)
    {
        return Container[index - 1].isEmpty();
    }
}

[System.Serializable]
public class InventorySlot
{
    public Drop drop;
    public int maximumAmount;
    public int currentAmount;
    public bool isSold;
    //public string dropName;
    public ItemType Equip;
    public InventorySlot(Drop _drop, int _maxAmount, ItemType _isEquip)
    {
        drop = _drop;
        maximumAmount = _maxAmount;
        Equip = _isEquip;
        currentAmount = 1;
        isSold = false;
    }
    public InventorySlot(Drop _drop, int _maxAmount)
    {
        drop = _drop;
        maximumAmount = _maxAmount;
        currentAmount = 1;
        isSold = false;
    }
    public InventorySlot(int _maxAmount, ItemType _isEquip)
    {
        drop = null;
        maximumAmount = _maxAmount;
        Equip = _isEquip;
        isSold = false;
    }
    public InventorySlot(InventorySlot slot, ItemType item)
    {
        drop = slot.drop;
        maximumAmount = slot.maximumAmount;
        Equip = item;
        currentAmount = slot.currentAmount;
        isSold = false;
    }
    public InventorySlot()
    {
        drop = null;
        maximumAmount = -1;
        Equip = ItemType.Default;
        currentAmount = 0;
        isSold = false;
    }

    public int AddAmount(Drop dropToAdd, int index, int amountOnPickDrop)
    {
        if (maximumAmount == -1) // slot is locked
            return -1;
        if (drop == null) // free slot
        {
            drop = new Drop(dropToAdd);
            currentAmount = 0;
        }
        int remainAmount = RemainAmount(amountOnPickDrop, currentAmount); // check how much Amount remain after the insert
        currentAmount += InsertAmount(currentAmount, amountOnPickDrop); // insert as much Amount that possible
        return remainAmount;
    }

    private int RemainAmount(int amountOnPickDrop, int currentAmount)
    {
        return Mathf.Max(0, (amountOnPickDrop + currentAmount) - maximumAmount);
    }
    private int InsertAmount(int currentAmount, int amountOnPickDrop)
    { // insert Amount
        float remain = maximumAmount - (amountOnPickDrop + currentAmount);
        if (remain > 0)
            return amountOnPickDrop;
        return Mathf.Min(amountOnPickDrop, maximumAmount - currentAmount);
    }

    public InventorySlot RemoveAmount(int amount)
    {
        InventorySlot temp = new InventorySlot(this, Equip);
        if (amount == -1 || amount >= currentAmount)
        {
            drop = null;
            currentAmount = 0;
            return temp;
        }
        currentAmount -= amount;
        //drop.SubAmount(amount);
        return this;
    }
    public bool isFull()
    {
        if (drop == null || currentAmount < maximumAmount)
            return false;
        return true;
    }
    public bool isLock()
    {
        return maximumAmount == -1;
    }
    public bool isEmpty()
    {
        return drop == null && currentAmount <= 0;
    }
    public bool isEquip()
    {
        return Equip == ItemType.Equips;
    }
    public bool isSpell()
    {
        return Equip == ItemType.Spells;
    }
    public bool IsSold()
    {
        return isSold;
    }
}
