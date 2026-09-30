using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class InventoryController : MonoBehaviour
{
    // Vars

    // inventory vars
    public InventoryObject inventory;
    public GameObject inventoryUI;
    [SerializeField] private int slotsAmount;
    [SerializeField] private Transform inventorySlot;
    public Transform inventorySlots;
    public GameObject StoreToTrade;
    public int[] offerNumber;

    // abillity buttoms
    public Transform AbillityTop;
    public Transform AbillityBot;
    public Transform AbillityMid;


    // slots vars
    public int indexSlotNeedChange;
    public Vector3 positionSlotNeedChange;
    public bool isSlotNeedChange;
    public int indexSlotThatTouch;
    public int indexOfTouch;
    public Vector2 endTouchPos;
    public Transform temp;
    public Transform emptySlot;

    // buy item from store
    public bool BuyMode;
    public float amountToTrade;
    private Dictionary<string[], int> itemsToTrade;

    bool deletethisflag;

    //Sound
    [SerializeField] private AudioClip swapItemSound;
    
    public void Init() // run in the start of the game, set up for inventory
    { // set up
        //objectPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
        indexOfTouch = -1;
        itemsToTrade = new Dictionary<string[], int>();
        inventory = new InventoryObject(slotsAmount, 3, 3, 5);
        indexSlotNeedChange = -1; // -1 -> nothing to change
        inventorySlots = GeneralFunctions.PlayerInventory.transform.Find("ShipInventory_Items");
        for (int i = 2; i <= slotsAmount; i++)
        {
            Transform a = Instantiate(inventorySlot, inventorySlots);
            a.name = "Slot (" + i + ")";
        }
        emptySlot.SetAsLastSibling();
        temp.SetAsLastSibling();
        emptySlot.gameObject.SetActive(false);
        offerNumber = new int[slotsAmount];

        //inventory.AddItem(AllDrops.rarity[1].drops[3], 1);

        //inventory.AddItem(AllDrops.rarity[1].drops[5], 1);
        //inventory.AddItem(AllDrops.rarity[1].drops[6], 1);

        inventory.AddItem(AllDrops.rarity[0].drops[0], 15);
        inventory.AddItem(AllDrops.rarity[2].drops[0], 1);
        //inventory.AddItem(AllDrops.rarity[0].drops[1], 10);
        //inventory.AddItem(AllDrops.rarity[1].drops[8], 10);
        deletethisflag = true;
        inventoryUI.SetActive(false);
        RefreshInventoryUI();
    }
    public void deleteAfterDemo()
    {
        indexSlotNeedChange = 18;
        SwapSlot(1);
        indexSlotNeedChange = -1;
        RefreshInventoryUI();
    }
    public void Update()
    {
        //if (deletethisflag)
        //{
        //    //OpenOrCloseInventoryUI();
        //    deleteAfterDemo();
        //    deletethisflag = false;
        //    //OpenOrCloseInventoryUI();
        //}
        //if(indexOfTouch >= 0)
        //{
        //    endTouchPos = Input.GetTouch(indexOfTouch).position;
        //}
    }
    public bool OpenOrCloseInventoryUI() // open or close the inventory UI
    {
        // active or deactive 
        if (inventoryUI.activeSelf)
        {
            ExitInventory();
        }
        else
        {
            inventoryUI.SetActive(true);
            LoadInventoryToUI(inventory.ConvertToUI());
        }
        return inventoryUI.activeSelf;
    }
    public void OpenInventoryUI()
    {
        inventoryUI.SetActive(true);
    }
    public void RefreshInventoryUI()
    {
        LoadInventoryToUI(inventory.ConvertToUI());
    }
    private void LoadInventoryToUI(InventorySlot[] realInventory) // update values of drop to UI 
    { // realInventory = the new and update container
        int currentIndex = 0;
        foreach (Transform slot in inventorySlots.GetComponentInChildren<Transform>())
        { // pass on every item in the slots UI and check if he change and need to be present
            if (realInventory.Length > currentIndex && !slot.name.Equals("tempParent"))
            { // slot = UI; slot.getChild(0) = item (drop object); slot.getChild(1) = text on the slot
                UpdateSlotUI(slot, realInventory[currentIndex], false, currentIndex);
            }
            currentIndex++;
        }

        while (currentIndex < realInventory.Length)
        {
            if (realInventory[currentIndex].isEquip() || realInventory[currentIndex].isSpell())
            { // equipSlot case
                UpdateSlotUI(ConvertIndexToSlotUI(currentIndex + 1), realInventory[currentIndex], true, currentIndex);
            }
            currentIndex++;
        }
    }
    private void UpdateSlotUI(Transform slotUI, InventorySlot slotStorge, bool isEquip, int currentIndex)
    {
        if (!slotStorge.isEmpty() && !slotStorge.isLock())
        {
            slotUI.GetChild(0).GetComponent<Image>().enabled = true;
            slotUI.GetChild(0).GetComponent<Image>().sprite = slotStorge.drop.image;
            if (GeneralFunctions.StoreController.storeUI.activeSelf && AllDrops.activeStore != null)
            {
                // demo
                if (!GeneralFunctions.IsTypeSuitStoreToRecive(GeneralFunctions.StoreController.itemNameToBuy, slotStorge.drop.type))
                    slotUI.GetChild(0).GetComponent<Image>().color = Color.gray;
                else
                    slotUI.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);

                // original
                //string storeName = AllDrops.activeStore.name;
                //storeName = new Regex(@"([a-zA-Z]+)").Match(storeName).Value;
                //if(!GeneralFunctions.IsTypeSuitStoreToRecive(storeName, slotStorge.drop.type))
                //    slotUI.GetChild(0).GetComponent<Image>().color = Color.gray;
                //else
                //    slotUI.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
                slotUI.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
            slotUI.GetChild(0).position = slotUI.position;
            if (!isEquip)
            {
                if (slotStorge.drop.type == ItemType.Equips)
                    slotUI.GetChild(1).GetComponent<Text>().text = "E";
                else if (slotStorge.drop.type == ItemType.Spells)
                    slotUI.GetChild(1).GetComponent<Text>().text = "S";
                else
                    slotUI.GetChild(1).GetComponent<Text>().text = slotStorge.currentAmount - offerNumber[currentIndex] + "";
            }
            
        }
        else
        {
            slotUI.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
            slotUI.GetChild(1).GetComponent<Text>().text = "";
        }
    }
    public void ExitInventory()
    {
        inventoryUI.SetActive(false);
        GeneralFunctions.StoreController.ExitStore();
    }
    public bool PickUpItem(Collider other)
    {
        bool ans = false;
        var drop = other.gameObject.GetComponent<DropScript>().drop;
        if (drop) // check not null
        {
            int totalAmount = other.gameObject.GetComponent<DropScript>().amount;
            int remain = inventory.AddItem(drop, totalAmount);
            if (remain == 0)
            {
                //other.gameObject.SetActive(false);
                ans = true;
            }
            else if (remain != -1) // no space left on inventory, the remain Amount left on original drop
                other.gameObject.GetComponent<DropScript>().amount = remain;
        }
        RefreshInventoryUI();
        return ans;
    }
    public void AddItem(Drop dropToAdd, int amount)
    {
        float remain = inventory.AddItem(dropToAdd, amount);
        // ----------- if the player have not enough space (remain > 0) what we do?
    }
    public bool SwapSlot(int indexTo)
    { // swap slot from indexSlotNeedChange to indexTo
        if (indexSlotNeedChange >= 0)
        {
            var arr = inventory.SwapSlots(indexSlotNeedChange, indexTo);
            if (arr == null)
                return false;
            if (inventory.isEquipSlot(indexTo) || inventory.isSpellSlot(indexTo))
            {
                if (GeneralFunctions.Player != null)
                {
                    GeneralFunctions.Player.GetComponent<PlayerInteraction>().SetEquipModelServerRpc(inventory.GetDropNameByIndex(indexTo), inventory.GetSideByIndex(indexTo));
                }
            }
            if (inventory.isEquipSlot(indexSlotNeedChange) || inventory.isSpellSlot(indexSlotNeedChange))
            {
                if (GeneralFunctions.Player != null)
                    GeneralFunctions.Player.GetComponent<PlayerInteraction>().SetEquipModelServerRpc(inventory.GetDropNameByIndex(indexSlotNeedChange), inventory.GetSideByIndex(indexSlotNeedChange));
            }

            indexSlotNeedChange = -1;            
            LoadInventoryToUI(arr);

            Camera.main.GetComponent<AudioSource>().clip = swapItemSound;
            Camera.main.GetComponent<AudioSource>().Play();

            return true;
        }
        indexSlotNeedChange = -1;
        return false;
    }
    public void RemoveSlot()
    {
        InventorySlot dropToOutSide = inventory.RemoveSlot(indexSlotNeedChange); // remove item
        LoadInventoryToUI(inventory.Container);
        if (dropToOutSide != null) // if the item drop out side then instantiate it out side
        {
            GeneralFunctions.Player.GetComponent<PlayerStats>().RemoveExp((float)(dropToOutSide.currentAmount * dropToOutSide.drop.rarity));
            InstantiateDrop(dropToOutSide);
        }
        indexSlotNeedChange = -1;
    }
    private void InstantiateDrop(InventorySlot slot) // init drop outside the inventory
    {
        GeneralFunctions.Pooler.CreateDrop(slot.drop, GeneralFunctions.Player.transform.position + new Vector3(5, 1, 4), slot.currentAmount);
        
        //temp.SetActive(true);
        //temp.AddComponent<DropScript>();
        //temp.GetComponent<DropScript>().drop = slot.drop;
        //temp.GetComponent<DropScript>().amount = slot.currentAmount;
    }
    public float AmountOfItemOnPlayer(string itemName)
    {
        return inventory.AmountItemOnInventory(itemName);
    }
    public void RemoveItemFromPlayer(int amount, string itemToTrade)
    {
        inventory.RemoveItemFromPlayer(amount, itemToTrade);
    }
    public void UseAbility(int slotNumber) // when press of slot its use drop ability
    {
        if (inventory.isEquipItem(slotNumber) || inventory.isSpellItem(slotNumber))
        {
            Debug.Log("UseAbility on inventory");
            GeneralFunctions.Player.GetComponent<PlayerInteraction>().UseAbillity(inventory.GetSideByIndex(slotNumber));
        }
    }
    public int ConvertSlotNameToIndex(string name)
    {
        if (name.Equals("AbillityTop"))
            return inventory.Container.Length;
        if (name.Equals("AbillityMid"))
            return inventory.Container.Length - 1;
        if (name.Equals("AbillityDown"))
            return inventory.Container.Length - 2;
        return -1;
    }
    public Transform ConvertIndexToSlotUI(int index)
    {
        if (index == inventory.Container.Length)
            return AbillityTop;
        if (index == inventory.Container.Length - 1)
            return AbillityMid;
        if (index == inventory.Container.Length - 2)
            return AbillityBot;
        return null;
    }
    public bool isInventoryOpen() { return inventoryUI.activeSelf; }
    public void AddAmountToTrade(int index)
    {
        var drop = inventory.GetDropByIndex(index);
        string[] newArray = new string[] { drop.drop.name, index + "" };
        string[] array = DicContaionSlot(newArray);
        if (array == null)
        {
            array = newArray;
            itemsToTrade[array] = 0;
        }
        if (drop.currentAmount > itemsToTrade[array])
        {
            itemsToTrade[array]++;
            amountToTrade += drop.drop.rarity;
            Debug.Log("player offer: " + itemsToTrade[array] + " of " + drop.drop.name);
            Debug.Log("the cost of all items that the player trade is " + amountToTrade);
            GeneralFunctions.StoreController.SetNewTradeOffer(amountToTrade);
        }
        else
        {
            Debug.Log("there is not enough of " + drop.drop.name);
        }
    }
    public void OperateTrade()
    {
        string itemName = GeneralFunctions.StoreController.itemNameToBuy; // for debug
        Debug.Log("you buy " + itemName + "!");

        foreach (string[] array in itemsToTrade.Keys)
        {
            inventory.RemoveItemFromPlayer(itemsToTrade[array], array[0]);
        }
        CancelPurchase();


        //Debug.Log(GeneralFunctions.StoreController.slotStore.gameObject.name);
        AllDrops.activeStore.GetComponent<StoreObjScript>().slotStoreCancel.Add(GeneralFunctions.StoreController.slotNumberToBuy);
        //GeneralFunctions.StoreController.slotStoreCancel.Add(GeneralFunctions.StoreController.slotNumberToBuy);
        InventorySlot dropToAdd = GeneralFunctions.StoreController.selectedStoreSlotToTrade;
        GeneralFunctions.StoreController.CancelPurchase();
        inventory.AddItem(dropToAdd.drop, dropToAdd.currentAmount);
        RefreshInventoryUI();
    }
    public void CancelPurchase()
    {
        offerNumber = new int[slotsAmount];
        itemsToTrade = new Dictionary<string[], int>();
        amountToTrade = 0;
        BuyMode = false;
        //if(AllDrops.activeStore.GetComponent<StoreController>().slotStore != null)
        //{
        //    AllDrops.activeStore.GetComponent<StoreController>().slotStore.isTouch = false;
        //}
        RefreshInventoryUI();
        //AllDrops.activeStore.GetComponent<StoreController>().CancelPurchase();
        //if(StoreToTrade != null)
            //StoreToTrade.GetComponent<StoreController>().CancelPurchase();
    }
    public bool isEquipSlot(int index)
    {
        return inventory.isEquipSlot(index);
    }
    public bool isSpellSlot(int index)
    {
        return inventory.isSpellSlot(index);
    }
    public void informSlotForTouch()
    {
        Debug.Log("informSlotForTouch");
        inventorySlots.GetChild(indexSlotThatTouch - 1).GetComponent<ItemController>().isTouch = true;
    }
    private string[] DicContaionSlot(string[] array)
    {
        foreach (var key in itemsToTrade.Keys)
        {
            if (array[0].Equals(key[0]) && array[1].Equals(key[1]))
                return key;
        }
        return null;
    }
}
