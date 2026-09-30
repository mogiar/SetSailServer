using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SlotStoreController : MonoBehaviour
{
    public GameObject store;
    private Touch touch;
    public bool isTouch;

    // UI
    private Slider _descriptionSlider;
    public RectTransform _descriptionBG;
    private void Start()
    {
        //isTouch = false;
        _descriptionSlider = GetComponentInChildren<Slider>();
        //transform.Find("ItemBack").gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.touchCount > 0)
        { // test
            touch = Input.touches[Input.touchCount-1];
            
            if (!isTouch && touch.phase == TouchPhase.Began && GeneralFunctions.CheckMouseOnObject(transform.gameObject, touch.position))
            {
                SelectItem(); // buy the item
            }
        }
    }
    
    private void SelectItem()
    {
        //store = AllDrops.activeStore;
        if (!AllDrops.activeStore.GetComponent<StoreObjScript>().slotStoreCancel.Contains(SlotIndex()))
        { 
            GeneralFunctions.StoreController.CancelPurchase();
            GeneralFunctions.inventoryControl.CancelPurchase();

            //isTouch = true;
            // for debug
            GeneralFunctions.StoreController.itemNameToBuy = transform.Find("Scroll View").Find("Content").Find("Left").Find("ItemNameText").GetComponent<Text>().text;
            GeneralFunctions.inventoryControl.RefreshInventoryUI();
            transform.Find("ItemBack").gameObject.SetActive(true);
            GeneralFunctions.StoreController.slotNumberToBuy = SlotIndex();
            Debug.Log("player want to buy " + store.GetComponent<StoreController>().itemNameToBuy);
            GeneralFunctions.StoreController.SetBuyModeOnInventory(SlotIndex(), this);
        }
        
        //store.GetComponent<StoreController>().RemoveItemByAmount(transform.GetChild(4).GetComponent<Text>().text, 1, itemToTrade);
    }
    private int SlotIndex()
    {
        //string SlotName = transform.parent.name;
        //if (SlotName.Equals("ShipStore_Items"))
        //return -1;
        return int.Parse(Regex.Match(transform.name, @"\d+").Value);
    }
    public void descriptionSlider(float value)
    {
        _descriptionBG.sizeDelta = new Vector2(value, _descriptionBG.sizeDelta.y);
    }
}
