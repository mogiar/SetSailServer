using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Default,
    Wildlife,
    Ammo,
    Equips,
    Spells,
    Gold,
    Unlocks,
    Gems,
    Fishs,
    Sharks
}
[CreateAssetMenu(fileName = "New Drop", menuName = "Drop")]
public class Drop : ScriptableObject
{
    //public string name;
    //public float amount;
    public Sprite image;
    public ItemType type;
    public int rarity;
    public string description;

    public Drop()
    {
        //name = "defult";
        //amount = 1;
        //isEquipItem = false;
    }
    public Drop(ItemType type)
    {
        //this.name = _name;
        //this.amount = amount;
        this.type = type;
    }
    public Drop(/*string _name, float amount, */ItemType type, Sprite image)
    {
        //this.name = _name;
        //this.amount = amount;
        this.type = type;
        this.image = image;
    }
    public Drop(Drop clone)
    {
        name = clone.name;
        //amount = clone.amount;
        type = clone.type;
        rarity = clone.rarity;
        image = clone.image;
        description = clone.description;

    }
    //public float GetAmount() { return amount; }
    //public void SetAmount(float amount) { this.amount = amount; }
    //public void AddAmount(float amount) { this.amount += amount; }
    //public void SubAmount(float amount) { this.amount -= amount; }
    virtual public void UseAbility()
    {
        if (type == ItemType.Equips)
        {
            Debug.Log("retular not abillity");
        }
    }


}
