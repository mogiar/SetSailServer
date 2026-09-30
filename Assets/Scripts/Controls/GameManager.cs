using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using static ObjectPooler;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public ObjectPooler Pooler;
    public DynamicRandom dynamicRandom;
    public GameObject PlayerInventory;
    public InventoryController inventoryControl;
    public RectTransform Wheel;
    public RectTransform WheelParent;
    public RectTransform SpeedButton;
    public RectTransform Compass;
    public GameObject UnitDetector;
    public StoreController StoreController;
    public PathManager PathManager;
    public RectTransform StoreUI;
    public string AmmoName;
    public GameObject FirstSlot;
    public GameObject ControlMove;
    public GameObject Camera;


    //public List<UnitList> unitsTest = new List<UnitList>();
    public float distanceToSpawn = 50;
    private Vector3 PlayerPositionPrev;
    public int level;
    private Dictionary<string, int[]> unitsDynamicToSpawn;
    private Dictionary<string, int[]> unitsStaticToSpawn;
    public Dictionary<string, List<string>> unitsByTheme = new Dictionary<string, List<string>>();
    public Dictionary<string, List<ItemType>> storeSell = new Dictionary<string, List<ItemType>>();
    public Dictionary<string, List<ItemType>> storeRecive = new Dictionary<string, List<ItemType>>();
    public Dictionary<string, List<ItemType>> storeMust = new Dictionary<string, List<ItemType>>();

    //[SerializeField] private GameObject FloatingTextOBJ;

    void Awake()
    {
        //NetworkManager.Singleton.StartClient();
        level = 1;
        SetStoreDescription();
        AmmoName = "CanonBalls";
        GeneralFunctions.SetObject(this, Player, Pooler, PlayerInventory, inventoryControl, Wheel, Compass, WheelParent, SpeedButton, StoreController, PathManager, storeSell, storeRecive, storeMust, StoreUI, AmmoName, FirstSlot, ControlMove, Camera);
        Pooler.Init();
        AllDrops.Start();
        PathInfo path = GeneralFunctions.Pooler.GetFirstUnitsDic("s");
        NetworkManager.Singleton.StartServer();


        // init
        //StoreController.Init();
        //inventoryControl.Init(); // ------- maybe change
        SetUnitsToSpawn(path.dynamicUnits, false);
        SetUnitsToSpawn(path.staticUnits, true);
        ControlMove.SetActive(false);
        //Dictionary<string, int[]> unitsName = GeneralFunctions.Pooler.pathInfo["starting"].


        //AllDrops.GenerateShopDemo(new Vector3(0, 0, 30f), "EquipShop");
        //AllDrops.GenerateShop(1, new Vector3(0, 0, 50f), "FishShop");
        //AllDrops.GenerateShop(1, new Vector3(30, 0, 50f), "EquipShop");

        //StartCoroutine("Tick");
    }
    public void PlayerInsertGame()
    {
        Player.SetActive(true);
        Player.GetComponent<PlayerControls>().InitMatch();
        Player.GetComponent<PlayerStats>().StartingState();
        PlayerPositionPrev = Player.transform.position;
    }
    public void PlayerStartMatch()
    {
        // ----------- move to server
        
        


        // ----------- move to server
        
        PlaceDynamicRandomObject();
        
        //StartCoroutine("Tick");
    }
    private void SetStoreDescription()
    {
        List<ItemType> lst = new List<ItemType>();

        // FishShop
        lst.Add(ItemType.Gold);
        lst.Add(ItemType.Gems);
        //lst.Add(ItemType.Spells);
        lst.Add(ItemType.Ammo);
        storeSell["FishShop"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Wildlife);
        storeRecive["FishShop"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Ammo);
        storeMust["FishShop"] = lst;

        // EquipShop
        lst = new List<ItemType>();
        lst.Add(ItemType.Equips);
        lst.Add(ItemType.Spells);
        lst.Add(ItemType.Ammo);
        storeSell["EquipShop"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Gold);
        lst.Add(ItemType.Gems);
        lst.Add(ItemType.Equips);
        lst.Add(ItemType.Spells);
        storeRecive["EquipShop"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Ammo);
        storeMust["EquipShop"] = lst;

        // GemShop
        lst = new List<ItemType>();
        lst.Add(ItemType.Unlocks);
        storeSell["GemShop"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Gems);
        storeRecive["GemShop"] = lst;

        lst = new List<ItemType>();
        storeMust["GemShop"] = lst;

        // DemoShop
        lst = new List<ItemType>();
        lst.Add(ItemType.Gold);
        lst.Add(ItemType.Fishs);
        storeRecive["Harpon"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Sharks);
        storeRecive["BasicCanon"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Sharks);
        storeRecive["CanonBalls"] = lst;

        lst = new List<ItemType>();
        lst.Add(ItemType.Sharks);
        lst.Add(ItemType.Gold);
        lst.Add(ItemType.Fishs);
        storeRecive["HealingHammer"] = lst;
    }
    
    IEnumerator Tick()
    {
        yield return new WaitForSeconds(7);
        UnitDetector.transform.position = Player.transform.position;
        CheckIfPlayerFar();
        StartCoroutine("Tick");
    }
    private void CheckIfPlayerFar()
    {
        if(Vector3.Distance(PlayerPositionPrev, Player.transform.position) > 50)
        {
            PlayerPositionPrev = Player.transform.position;
            PlaceDynamicRandomObject();
        }
    }
    public void PlaceDynamicRandomObject()
    {
        dynamicRandom.PlaceRandomDynamic(250, 130, unitsDynamicToSpawn, PlayerPositionPrev);
    }
    public void PlaceStaticRandomObject(Vector3 PathPosition)
    {
        dynamicRandom.PlaceRandomStatic(500, 130, unitsStaticToSpawn, new Vector2(PathPosition.x, PathPosition.z), new Vector2(Player.transform.position.x, Player.transform.position.z));
    }
    public void SetUnitsToSpawn(Dictionary<string, int[]> unitsName, bool isStatic)
    {
        if(isStatic)
            unitsStaticToSpawn = new Dictionary<string, int[]>();
        else
            unitsDynamicToSpawn = new Dictionary<string, int[]>();
        foreach (string name in unitsName.Keys)
        {
            int[] realAmounts = new int[Mathf.Max(8, unitsName[name].Length)];
            int unitNumber = unitsName[name][unitsName[name].Length-1];// GetUnitNumber(name);
            for (int i=0; i< realAmounts.Length; i++)
            {
                if (unitsName[name].Length > i && unitsName[name][i] > -1)
                    realAmounts[i] = unitsName[name][i];
                else
                    realAmounts[i] = unitNumber;
            }
            if (isStatic)
                unitsStaticToSpawn[name] = realAmounts;
            else
                unitsDynamicToSpawn[name] = realAmounts;
        }
    }
    //public void AddUnitsToSpawn(List<string> unitsName, bool isStatic)
    //{
    //    if (isStatic)
    //    {
    //        if (unitsStaticToSpawn != null)
    //        {
    //            foreach (string name in unitsName)
    //            {
    //                unitsStaticToSpawn[name] = GetUnitNumber(name);
    //            }
    //        }
    //        else
    //            SetUnitsToSpawn(unitsName, isStatic);
    //    }
    //    else
    //    {
    //        if (unitsDynamicToSpawn != null)
    //        {
    //            foreach (string name in unitsName)
    //            {
    //                unitsDynamicToSpawn[name] = GetUnitNumber(name);
    //            }
    //        }
    //        else
    //            SetUnitsToSpawn(unitsName, isStatic);
    //    }
        
    //}
    private int GetUnitNumber(string name)
    {
        //float prob = Pooler.GetProbability(name);
        //if (prob >= 1)
        //    prob = -1;
        return Pooler.GetNumer(name);
    }
    public string GetCurrentTheme()
    {
        return "";
    }
    public void startGame()
    {
        
    }

    public void FloatingText(string text ,Color color, Vector3 position)
    {
        Vector2 newPos = new Vector2(position.x, position.z);
        //GameObject a = Instantiate(FloatingTextOBJ, newPos, Quaternion.Euler(90,0,0));
        GameObject a = Pooler.SpawnFromPool("TextMesh", newPos, Quaternion.Euler(90, 0, 0));
        a.GetComponent<TextMeshPro>().text = text;
        a.GetComponent<TextMeshPro>().color = color;
    }
    public void g()
    {
        
    }
}
