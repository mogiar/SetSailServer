using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

// Random store generator. 

[System.Serializable]
public class dropsList
{
    [HideInInspector]
    public string name;
    public Drop[] drops;

    public dropsList(string name, Drop[] drops)
    {
        this.name = name;
        this.drops = drops;
    }
}
public static class AllDrops
{
    private static int StoreIndex;
    public static dropsList[] rarity = new dropsList[8];
    public static dropsList[] rarityByFishShop = new dropsList[8];
    public static dropsList[] rarityByEquipShop = new dropsList[8];
    public static dropsList[] rarityByGemShop = new dropsList[8];
    //public static GameObject[] _storePrefab;
    //private static Queue dropForStore;


    public static Dictionary<string, Transform> EquipObject = new Dictionary<string, Transform>();
    public static Dictionary<string, Transform> AmmoObject = new Dictionary<string, Transform>();
    public static GameObject activeStore;
    
    
    public static void Start()
    {
        //PlayerInventory = GameObject.Find("UI").transform.Find("Menus").Find("ShipInventory").gameObject;
        //_storePrefab = Resources.LoadAll<GameObject>("Store");
        StoreIndex = 1;
        for (int i = 1; i <= rarity.Length; i++)
        {
            var fileEntriesDrops = Resources.LoadAll<Drop>("Drops\\Rarity-" + i);
            rarity[i-1] = new dropsList("", fileEntriesDrops);
        }
        InitDropByStore(rarityByFishShop, "FishShop");
        InitDropByStore(rarityByEquipShop, "EquipShop");
        InitDropByStore(rarityByGemShop, "GemShop");


        var fileEntriesEquip = Resources.LoadAll<Transform>("EquipModels");
        foreach (Transform tran in fileEntriesEquip)
        {
            EquipObject[tran.name] = tran;
        }

        var fileEntriesAmmo = Resources.LoadAll<Transform>("Ammo"); 
        foreach (Transform tran in fileEntriesAmmo)
        {
            AmmoObject[tran.name] = tran;
        }
    }
    private static void InitDropByStore(dropsList[] rarityBy, string storeName)
    {
        for (int i = 0; i < rarity.Length; i++)
        {
            List<Drop> lst = new List<Drop>();
            foreach (var element in rarity[i].drops)
            {
                if (GeneralFunctions.IsTypeSuitStoreToSell(storeName, element.type))
                {
                    lst.Add(element);
                }
            }
            rarityBy[i] = new dropsList(storeName + "-"+(i+1), lst.ToArray());
        }
    }
    public static Transform GetEquipByName(string nameEquip)
    {
        if(EquipObject.ContainsKey(nameEquip))
            return EquipObject[nameEquip];
        return null;
    }
    public static Transform GetAmmoByName(string nameEquip)
    {
        if(AmmoObject.ContainsKey(nameEquip))
            return AmmoObject[nameEquip];
        return null;
    }

    public static void GenerateShop(int shopLevel, Vector3 position, string storeName)
    {
        int numberOfDrops = 6;
        InventoryObject shopInventory = new InventoryObject(numberOfDrops, 20);

        int maxRarity = shopLevel;
        int minRarity = Mathf.Max(shopLevel - 2,0);

        for (int i = 0; i < numberOfDrops; i++)
        {
            Drop a = RandomDrop(maxRarity, minRarity, storeName);
            if (!shopInventory.thereIsEnoughFromItem(a.name, 1))
            {
                if (a.type == ItemType.Ammo)
                    shopInventory.AddItem(a, shopLevel);
                if (a.type == ItemType.Gold)
                    shopInventory.AddItem(a, Mathf.Max(shopLevel - 1, 1));
                else
                    shopInventory.AddItem(a, 1);
            }
            
        }

        shopInventory.ResetInventoryByAmount(1);

        GameObject generatedStore = GeneralFunctions.Pooler.SpawnFromPool(storeName, new Vector2(position.x, position.z));
        generatedStore.name = storeName;
        generatedStore.GetComponent<StoreObjScript>().shopInventory = shopInventory;
        
        //generatedStore.GetComponent<StoreController>().StoreName = storeName;
        //generatedStore.GetComponent<StoreController>().store = shopInventory;
        //generatedStore.GetComponent<StoreController>().storeObject = generatedStore;
        //generatedStore.GetComponent<StoreController>().storeUI = GameObject.Find("UI").transform.Find("Menus").Find("StoreUI").gameObject;
        //generatedStore.GetComponent<StoreController>().playerInventory = GeneralFunctions.PlayerInventory;
        //StoreIndex++;
        //generatedStore.SetActive(true);
        //generatedStore.GetComponent<StoreController>().Init();
    }
    public static void GenerateShopDemo(Vector2 position, string storeName)
    {
        int numberOfDrops = 6;
        InventoryObject shopInventory = new InventoryObject(numberOfDrops, 50);
        shopInventory.AddItem(AllDrops.rarity[1].drops[5], 1);
        shopInventory.AddItem(AllDrops.rarity[1].drops[6], 1);
        shopInventory.AddItem(AllDrops.rarity[2].drops[0], 1);
        shopInventory.ResetInventoryByAmount(1);
        shopInventory.AddItem(AllDrops.rarity[0].drops[0], 50);

        GameObject generatedStore = GeneralFunctions.Pooler.SpawnFromPool(storeName, new Vector2(position.x, position.y));
        //generatedStore.name = storeName;
        generatedStore.GetComponent<StoreObjScript>().shopInventory = shopInventory;
    }

    private static Drop RandomDrop(int maxRarity, int minRarity, string storeName)
    {
        int dropsAmount = 0;

        for (int i = minRarity; i < maxRarity + 1; i++)
        {
            switch (storeName)
            {
                case "FishShop":
                    dropsAmount += rarityByFishShop[i].drops.Length;
                    break;
                case "EquipShop":
                    dropsAmount += rarityByEquipShop[i].drops.Length;
                    break;
                case "GemShop":
                    dropsAmount += rarityByGemShop[i].drops.Length;
                    break;
                case "RandomShop":
                    dropsAmount += rarity[i].drops.Length;
                    break;
                default:
                    dropsAmount += rarity[i].drops.Length;
                    break;
            }
            
        }

        int n = Random.Range(0, dropsAmount);
        switch (storeName)
        {
            case "FishShop":
                return (ItemRecursive(minRarity, n, rarityByFishShop));
            case "EquipShop":
                return (ItemRecursive(minRarity, n, rarityByEquipShop));
            case "GemShop":
                return (ItemRecursive(minRarity, n, rarityByGemShop));
            case "RandomShop":
                return (ItemRecursive(minRarity, n, rarity));
            default:
                return (ItemRecursive(minRarity, n, rarity));
        }
    }

    private static Drop ItemRecursive(int randomDropRarity, int randomNumb, dropsList[] rarityCurrent)
    {
        Drop a;
        if (randomNumb >= rarityCurrent[randomDropRarity].drops.Length)
        {
            randomNumb -= rarityCurrent[randomDropRarity].drops.Length;
            randomDropRarity++;
            a = ItemRecursive(randomDropRarity, randomNumb, rarityCurrent);
            return (a);
        }
        a = rarityCurrent[randomDropRarity].drops[randomNumb];
        return (a);
    }

    public static void CreateDrop(string DropName, int rarity,  Vector3 position)
    {
        for (int i = 0; i < AllDrops.rarity[rarity - 1].drops.Length; i++)
        {
            if (AllDrops.rarity[rarity - 1].drops[i].name == DropName)
            {
                GeneralFunctions.Pooler.CreateDrop(AllDrops.rarity[rarity - 1].drops[i], position, 1);
                break;
            }
        }
    }
    public static void CreateDrop(string DropName, int rarity, Vector3 position, string storeName)
    {

        for (int i = 0; i < AllDrops.rarity[rarity - 1].drops.Length; i++)
        {
            if (AllDrops.rarity[rarity - 1].drops[i].name == DropName)
            {
                switch (storeName)
                {
                    case "FishShop":
                        GeneralFunctions.Pooler.CreateDrop(AllDrops.rarityByFishShop[rarity - 1].drops[i], position, 1);
                        break;
                    case "EquipShop":
                        GeneralFunctions.Pooler.CreateDrop(AllDrops.rarityByEquipShop[rarity - 1].drops[i], position, 1);
                        break;
                    case "GemShop":
                        GeneralFunctions.Pooler.CreateDrop(AllDrops.rarityByGemShop[rarity - 1].drops[i], position, 1);
                        break;
                    case "RandomShop":
                        GeneralFunctions.Pooler.CreateDrop(AllDrops.rarity[rarity - 1].drops[i], position, 1);
                        break;
                    default:
                        GeneralFunctions.Pooler.CreateDrop(AllDrops.rarity[rarity - 1].drops[i], position, 1);
                        break;
                }
                break;
            }
        }
    }
    public static Drop GetDropByName(string DropName, int rarity)
    {
        for (int i = 0; i < AllDrops.rarity[rarity - 1].drops.Length; i++)
        {
            if (AllDrops.rarity[rarity - 1].drops[i].name == DropName)
            {
                return AllDrops.rarity[rarity - 1].drops[i];
            }
        }
        return null;
    }
    public static Drop GetDropByName(string DropName)
    {
        for(int j=0; j< AllDrops.rarity.Length; j++)
        {
            for (int i = 0; i < AllDrops.rarity[j].drops.Length; i++)
            {
                if (AllDrops.rarity[j].drops[i].name == DropName)
                {
                    return AllDrops.rarity[j].drops[i];
                }
            }
        }
        
        return null;
    }
    public static void SetDropToObject(GameObject obj, string dropName, int amount)
    {
        //obj.transform.position = new Vector3(position.x, a.transform.position.y, position.z);
        obj.GetComponentInChildren<DropScript>().drop = GetDropByName(dropName);
        obj.GetComponentInChildren<DropScript>().amount = amount;
    }
}
