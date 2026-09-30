using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public InventoryController inventory;
    public GameObject inventoryUI;
    private float widthInventory;
    private float heightInventory;
    private Vector3 posInventory;
    public Touch touch;
    private int mySlotNumber;
    public bool isTouch;
    public bool isTouchES;

    

    void Start()
    {
        inventory = GeneralFunctions.inventoryControl;
        if (inventoryUI == null)
        {
            inventoryUI = GameObject.Find("ShipInventory_Items");
        }
        widthInventory = inventoryUI.GetComponent<RectTransform>().rect.width;
        heightInventory = inventoryUI.GetComponent<RectTransform>().rect.height;
        posInventory = inventoryUI.GetComponent<Transform>().position;
        mySlotNumber = SlotIndex();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && GeneralFunctions.inventoryControl.indexOfTouch >= 0 && mySlotNumber != -1)
        {
            //if (GeneralFunctions.Player.GetComponent<PlayerControls>().wheelFinger == 1)
            //    touch = Input.touches[0];
            //else
            //    touch = Input.touches[Input.touches.Length - 1];
            touch = GetTouch(GeneralFunctions.inventoryControl.indexOfTouch);

            if(touch.fingerId != -1)
            {
                isTouch = true;

                if (((touch.phase == TouchPhase.Began && mySlotNumber < 15) || ((touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began) && mySlotNumber > 15 && inventory.indexSlotNeedChange == -1)) && GeneralFunctions.CheckMouseOnObject(transform.parent.gameObject, touch.position))
                { // began phase - store the index of the first slot the player touch
                    if (inventory.inventoryUI.activeSelf)
                    {
                        if (inventory.BuyMode && transform.GetComponent<Image>().color != Color.gray && !(inventory.isEquipSlot(mySlotNumber) || inventory.isSpellSlot(mySlotNumber)) && !inventory.inventory.isEmptySlot(mySlotNumber))
                        {
                            if(inventory.offerNumber[mySlotNumber - 1] < inventory.inventory.GetAmountByIndex(mySlotNumber))
                            inventory.offerNumber[mySlotNumber - 1]++;
                            //transform.GetComponent<Image>().color = Color.black; // for debud
                            inventory.AddAmountToTrade(mySlotNumber);
                            inventory.indexOfTouch = -1;
                            inventory.RefreshInventoryUI();
                        }
                        else
                        {
                            BeganPart(); // first touch
                        }
                    }
                }
                if (mySlotNumber == inventory.indexSlotNeedChange && touch.phase == TouchPhase.Moved)
                { // check if the first touch is moving
                    transform.position = touch.position;
                }
                if (touch.phase == TouchPhase.Ended)
                { // end phase - when the player finish to touch the screen

                    if (mySlotNumber == inventory.indexSlotNeedChange)
                    { // the player didn't finish the touch on a diffrent slot from the began
                        if (!GeneralFunctions.CheckMouseOnObject(widthInventory, heightInventory, posInventory, touch.position))
                        { // the touch end out side the inventory -> item thrown out from inventory
                            if (!inventory.inventory.isEmptySlot(mySlotNumber))
                            {
                                if (mySlotNumber < 15)
                                {
                                    inventory.indexOfTouch = -1;
                                    transform.parent.parent = inventory.inventorySlots;
                                    transform.parent.SetSiblingIndex(mySlotNumber - 1);
                                    inventory.emptySlot.SetAsLastSibling();
                                    inventory.emptySlot.gameObject.SetActive(false);
                                }
                                DoEmptySlot();
                            }
                        }
                        else if (!inventory.isSlotNeedChange)
                        { // there is'nt item to change -> item return to the original slot
                            ReturnPart();
                        }
                    }
                    else if (inventory.indexSlotNeedChange != -1 && GeneralFunctions.CheckMouseOnObject(transform.parent.gameObject, touch.position))
                    { // the defult if non of the above didn't happend -> try to swap slots
                        inventory.indexOfTouch = -1;
                        if (inventory.indexSlotNeedChange < 15)
                        {
                            //Debug.Log("end SwapPart");
                            Transform a = inventory.inventorySlots.Find("Slot (" + inventory.indexSlotNeedChange + ")");
                            if (a == null)
                                a = inventory.temp.GetChild(0);
                            a.parent = inventory.inventorySlots;
                            a.SetSiblingIndex(inventory.indexSlotNeedChange);
                            inventory.emptySlot.SetAsLastSibling();
                            inventory.emptySlot.gameObject.SetActive(false);
                        }
                        SwapPart();
                    }
                }
            }
        }
        else if (isTouch || inventory.isSlotNeedChange)
        { // reset the check flag
            inventory.isSlotNeedChange = false;
            isTouchES = false;
            if (inventory.temp.childCount > 0 && inventory.indexSlotNeedChange < 15)
            {
                inventory.emptySlot.SetAsLastSibling();
                Transform a = inventory.temp.GetChild(0);
                a.parent = inventory.inventorySlots;
                a.SetSiblingIndex(inventory.indexSlotNeedChange - 1);
                inventory.emptySlot.gameObject.SetActive(false);
            }
            inventory.indexSlotNeedChange = -1;
            inventory.indexOfTouch = -1;
            isTouch = false;
        }
    }
    public void ReturnPart()
    {
        //Debug.Log("ReturnPart");
        transform.position = inventory.positionSlotNeedChange;
        inventory.RefreshInventoryUI();
    }
    public void BeganPart()
     { // touch at first slot
        //Debug.Log("BeganPart");
        inventory.indexSlotNeedChange = SlotIndex();
        inventory.positionSlotNeedChange = transform.position;
        if(mySlotNumber < 15)
        {
            //Debug.Log("end BeganPart");
            if(inventory.temp.childCount > 0)
            {
                Transform a = inventory.temp.GetChild(0);
                a.parent = inventory.inventorySlots;
                int index = a.gameObject.GetComponentInChildren<ItemController>().mySlotNumber;
                a.SetSiblingIndex(index - 1);
            }
            transform.parent.parent = inventory.temp;
            inventory.emptySlot.gameObject.SetActive(true);
            inventory.emptySlot.SetSiblingIndex(inventory.indexSlotNeedChange - 1);
            //inventory.temp.SetAsLastSibling();
        }
        
    }
    public void SwapPart()
     { // touch at second slot
        //Debug.Log("SwapPart");
        int indexTo = SlotIndex();
        if (inventory.indexSlotNeedChange != indexTo)
        {
            if (!inventory.SwapSlot(indexTo))
            {
                transform.position = inventory.positionSlotNeedChange;
            }
            inventory.RefreshInventoryUI();
        }
        inventory.isSlotNeedChange = true;
    }
    public void DoEmptySlot()
    {
        //Debug.Log("DoEmptySlot");
        inventory.RemoveSlot();
        transform.position = inventory.positionSlotNeedChange;
        inventory.isSlotNeedChange = true;
        if((inventory.isEquipSlot(mySlotNumber) || inventory.isSpellSlot(mySlotNumber)))
        {
            GeneralFunctions.Player.GetComponent<PlayerInteraction>().TurnOffEquipModel(inventory.inventory.GetSideByIndex(mySlotNumber));
        }
    }
    public void EndPart()
    {
        touch = Input.touches[Input.touches.Length - 1];
        if (mySlotNumber == inventory.indexSlotNeedChange)
        { // the player didn't finish the touch on a diffrent slot from the began
            if (!GeneralFunctions.CheckMouseOnObject(widthInventory, heightInventory, posInventory, touch.position))
            { // the touch end out side the inventory -> item thrown out from inventory
                DoEmptySlot();
            }
            else if (!inventory.isSlotNeedChange)
            { // there is'nt item to change -> item return to the original slot
                ReturnPart();
            }
        }
        else if (inventory.indexSlotNeedChange != -1 && GeneralFunctions.CheckMouseOnObject(transform.parent.gameObject, touch.position))
        { // the defult if non of the above didn't happend -> try to swap slots
            SwapPart();
        }
    }
    void UseAbility()
    { // touch at slot
        int i = SlotIndex();
        if (i != -1)
            inventory.UseAbility(i);
    }

    private int SlotIndex()
    { // get the index of this slot
        if (!transform.name.Substring(0, 4).Equals("Item") && transform.parent.name.Substring(0, 8).Equals("Abillity"))
        {
            return inventory.ConvertSlotNameToIndex(transform.parent.name);
        }
        string SlotName = transform.parent.name;
        if (SlotName.Equals("EmptySlot"))
            return -1;
        string[] arr = SlotName.Split('(');
        string[] arr1 = arr[1].Split(')');
        return int.Parse(arr1[0]);
    }
    public void returnToPlace(Vector3 pos)
    {
        transform.position = pos;
    }
    public void SetIndexOnTouch()
    {
        inventory.indexSlotThatTouch = mySlotNumber;
    }
    private Touch GetTouch(int id)
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == id)
                return touch;
        }
        Touch temp = new Touch();
        temp.fingerId = -1;
        return temp;
    }
}
