using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class StoreController : MonoBehaviour
{
    // Vars
    public GameObject storeObject;
    public InventoryObject store;
    public GameObject storeUI;
    public Dictionary<string, GameObject> itemsOnStore;
    private Transform inventorySlots;

    // buy 
    public Button AcceptTradeButton;
    public string itemNameToBuy;// for debug
    public bool offerAccpeted;
    public int slotNumberToBuy;
    public InventorySlot selectedStoreSlotToTrade;
    //public List<int> slotStoreCancel = new List<int>();
    public SlotStoreController slotStore;

    //public GameObject Manager;
    //public Dictionary<string, float> ItemCost; //  + "item cost:" + itemCost[slot.name] * store[index].drop.amount
    //public Dictionary<string, string> ItemInfo;

    
    public void Init() // run in the start of the game, set up for inventory
    { // set up
        storeUI = GameObject.Find("StoreUI");
        inventorySlots = storeUI.transform.Find("ShipStore_Items");
        AcceptTradeButton = storeUI.transform.Find("AcceptTrade").GetComponent<Button>();
        AcceptTradeButton.interactable = false;
        storeUI.SetActive(false);
        
        // insert all type of drops in game
        itemsOnStore = new Dictionary<string, GameObject>();

        //store.ResetInventoryByAmount(1);
        selectedStoreSlotToTrade.drop = null;
        slotNumberToBuy = -1;
    }
    public void LoadStore(GameObject realStoreObject) // open or close the inventory UI
    {
        storeUI.SetActive(true);
        store = realStoreObject.GetComponent<StoreObjScript>().shopInventory;
        storeUI.transform.Find("StoreNameBoard").Find("StoreName").GetComponent<Text>().text = "Shop";//realStoreObject.name;
        LoadStoreToUI(store.ConvertToUI());
        GeneralFunctions.inventoryControl.RefreshInventoryUI();
    }
    public void LoadStoreOnly() // update store UI
    {
        if(store != null)
            LoadStoreToUI(store.ConvertToUI());
    }
    private void LoadStoreToUI(InventorySlot[] realInventory) // update values of drop to UI 
    { // realInventory = the new and update container
        Debug.Log("LoadStoreToUI");
        // add more slot to ui if nessesery
        var tempSlot = inventorySlots.Find("Slot1");
        var slotActiveNumber = inventorySlots.childCount;
        CreateSlots(Mathf.Max(0, realInventory.Length - slotActiveNumber), tempSlot);

        // insert information to UI
        int currentIndex = 0;
        foreach (Transform slot in inventorySlots.GetComponent<Transform>())
        { // pass on every item in the slots UI and check if he change and need to be present
            if (realInventory.Length > currentIndex)
            { // slot = UI; slot.getChild(0) = item (drop object); slot.getChild(1) = price of the drop; slot.getChild(2) = description; slot.getChild(3) = slider; slot.getChild(4) = name of the item
                //slot.GetComponent<SlotStoreController>().store = storeObject.GetComponent<StoreController>().storeUI;
                slot.gameObject.SetActive(true);
                slot.Find("ItemBack").gameObject.SetActive(false);
                var left = slot.Find("Scroll View").Find("Content").Find("Left");
                var right = slot.Find("Scroll View").Find("Content").Find("Right");
                
                var barTrade = left.Find("ItemBar").Find("ItemFillingBar");
                //barTrade.localScale = new Vector3(0, barTrade.localScale.y, barTrade.localScale.z);
                if (!realInventory[currentIndex].isEmpty() && !realInventory[currentIndex].isLock())
                { // update the info on UI from the inventoryObject
                    left.Find("ItemImage").GetComponent<Image>().sprite = realInventory[currentIndex].drop.image; //itemsOnStore[realInventory[currentIndex].dropName].GetComponent<Image>().sprite;
                    left.Find("ItemImage").GetComponent<Image>().enabled = true;
                    //left.Find("ItemImage").GetComponent<Image>().color = Color.white;
                    left.Find("ItemNameText").GetComponent<Text>().text = realInventory[currentIndex].drop.name;
                    right.Find("ItemName").GetComponent<Text>().text = realInventory[currentIndex].drop.name;
                    right.Find("Explanation").GetComponent<Text>().text = realInventory[currentIndex].drop.description;

                    if (AllDrops.activeStore != null && AllDrops.activeStore.GetComponent<StoreObjScript>().slotStoreCancel.Contains(currentIndex + 1))
                    {
                        left.Find("ItemImage").GetComponent<Image>().color = Color.grey;
                        left.Find("ItemBarText").GetComponent<Text>().text = "SOLD";
                        left.Find("ItemBar").GetChild(0).GetComponent<Image>().color = Color.green;
                    }
                    else
                    {
                        barTrade.localScale = new Vector3(0, barTrade.localScale.y, barTrade.localScale.z);
                        left.Find("ItemImage").GetComponent<Image>().color = Color.white;
                        left.Find("ItemBar").GetChild(0).GetComponent<Image>().color = Color.red;
                        left.Find("ItemBarText").GetComponent<Text>().text = "0 / " + realInventory[currentIndex].drop.rarity;
                    }

                    if(GeneralFunctions.StoreController.itemNameToBuy.Equals(realInventory[currentIndex].drop.name))
                        slot.Find("ItemBack").gameObject.SetActive(true);
                }
                else
                {
                    slot.gameObject.SetActive(false);
                }
            }
            else if(currentIndex <= 6)
            {
                slot.gameObject.SetActive(false);
            }
            currentIndex++;
        }
    }
    private void CreateSlots(int SlotsNumberToAdd, Transform tempSlot)
    {
        for (int i = 0; i < SlotsNumberToAdd; i++)
        {
            var temp = Instantiate(tempSlot);
            temp.name = "Slot" + (i + 2);
            temp.SetParent(inventorySlots);
        }
    }
    public void ExitStore() // exit store
    {
        Debug.Log("ExitStore");
        CancelPurchase();
        storeUI.SetActive(false);
        Camera.main.GetComponent<worldInteraction>().flag = true;
        Camera.main.GetComponent<worldInteraction>().enabled = false;
        Camera.main.GetComponent<worldInteraction>().enabled = true;
        AllDrops.activeStore = null;
        GeneralFunctions.inventoryControl.CancelPurchase();
    }
    public bool AddNewItem(GameObject item)
    {
        itemsOnStore[item.name] = item;
        //LoadStoreOnly();
        return true;
    }
    public float AddExistItem(Drop itemToAdd, int amount)
    {
        if (!IsItemExist(itemToAdd.name))
            return -1;
        float remain = store.AddItem(itemToAdd, amount);
        //LoadStoreOnly();
        return remain;
    }
    public bool IsItemExist(GameObject item)
    {
        return itemsOnStore.ContainsKey(item.name);
    }
    public bool IsItemExist(string itemName)
    {
        return itemsOnStore.ContainsKey(itemName);
    }
    /*
    public bool RemoveItemByAmount(string itemName, float amountToRemove, string itemToTrade) // when player buy item from store
    {
        if (!itemsOnStore.ContainsKey(itemName))
            return false; // item doesn't exist in store
        float cost = amountToRemove * ItemCost[itemName];
        if (playerInventory.GetComponent<InventoryController>().AmountOfItemOnPlayer(itemToTrade) < cost)
             return false;

        int indexSlot = 1;
        foreach (InventorySlot slot in store.Container)
        { // ------- i assumed that every item show one time in store
            if (slot.drop != null && slot.drop.name == itemName && amountToRemove > 0f)
            { // we found the item on the store
                if (slot.drop.GetAmount() < amountToRemove)
                    return false; // ---------there is not enough in stock, what we do in this case?
                else
                {
                    store.RemoveSlotByAmount(indexSlot, amountToRemove); // Subtract from the quantity in store
                    Drop itemPlayerBuy = DropFactory.CreateDrop(itemName, amountToRemove); // create new item
                    playerInventory.GetComponent<InventoryController>().RemoveItemFromPlayer(cost, itemToTrade);
                    playerInventory.GetComponent<InventoryController>().AddItem(itemPlayerBuy); // add new item to player buy to him
                }
            }
            indexSlot++;
        }

        playerInventory.GetComponent<InventoryController>().LoadInventoryOnly();
        LoadStoreOnly();
        return true;
    }
    */
    public bool RemoveItem(string itemName) // delete item from store
    {
        if (!itemsOnStore.ContainsKey(itemName))
            return true;
        foreach (InventorySlot slot in store.Container)
        {
            if (slot.drop.name == itemName)
            {
                slot.RemoveAmount(-1);
            }
        }
        itemsOnStore.Remove(itemName);
        //ItemCost.Remove(itemName);
        LoadStoreOnly();
        return true;
    }
    //public void UpdateCostOfItem(string itemName, float newCost) // change cost of item
    //{
    //    ItemCost[itemName] = newCost;
    //    LoadStoreOnly();
    //}
    //public void UpdateManager(GameObject manager)
    //{
    //    Manager = manager;
    //}
    //public string GetDescriptionOfItem(string itemName)
    //{
    //    return ItemInfo[itemName];
    //}

    public void SetBuyModeOnInventory(int slotNumber, SlotStoreController SlotStore)
    {
        slotStore = SlotStore;
        GeneralFunctions.inventoryControl.BuyMode = true;
        //GeneralFunctions.inventoryControl.StoreToTrade = AllDrops.activeStore;
        GeneralFunctions.StoreController.slotStore = SlotStore;
        GeneralFunctions.StoreController.slotNumberToBuy = slotNumber;
    }
    public void SetNewTradeOffer(float amountToTrade)
    {
        float priceOfChoosenItem = store.GetRarityByIndex(slotNumberToBuy);
        var left = inventorySlots.GetChild(slotNumberToBuy - 1).Find("Scroll View").Find("Content").Find("Left");
        left.Find("ItemBarText").GetComponent<Text>().text = amountToTrade + " / " + priceOfChoosenItem;
        Debug.Log("the item cost " + priceOfChoosenItem);
        var fillingBar = storeUI.transform.Find("ShipStore_Items").Find("Slot" + slotNumberToBuy).Find("Scroll View").Find("Content").Find("Left").Find("ItemBar").Find("ItemFillingBar");
        fillingBar.localScale = new Vector3(Mathf.Min(1, amountToTrade / priceOfChoosenItem), fillingBar.localScale.y, fillingBar.localScale.z);
        offerAccpeted = priceOfChoosenItem <= amountToTrade;
        if (offerAccpeted)
        {
            selectedStoreSlotToTrade = store.GetDropByIndex(slotNumberToBuy);
            left.Find("ItemImage").GetComponent<Image>().color = Color.blue; //  for debug
            // Activate ConfirmButton
            AcceptTradeButton.interactable = true;

        }
    }
    public void CancelPurchase() // cancel buy mode
    {
        slotNumberToBuy = -1;
        itemNameToBuy = ""; // for debug
        LoadStoreOnly();
        AcceptTradeButton.interactable = false;
        //if (GeneralFunctions.StoreController.slotStore != null)
        //{
        //    Transform slot = GeneralFunctions.StoreController.slotStore.gameObject.transform.GetChild(0).GetChild(0).GetChild(0);
        //    slot.Find("ItemImage").GetComponent<Image>().color = Color.grey;
        //    slot.Find("ItemBarText").GetComponent<Text>().text = "0 / " + AllDrops.activeStore.GetComponent<StoreController>().store.Container[slotNumberToBuy].drop.rarity;
        //}        

        //selectedStoreSlotToTrade.drop = null; // mabye bug
    }
}

