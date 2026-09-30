using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    public bool flag;
    private bool UIactive;
    public GameObject UI_Menus;
    //public GameObject player;
    public RectTransform wheel;
    public RectTransform inventoryUI;
    public RectTransform storeUI;
    public RectTransform inventoryButton;
    public RectTransform AbillityTop;
    public RectTransform AbillityMid;
    public RectTransform AbillityDown;

    public int[] touchesID;
    public string[] touchesPlace;
    private int prevTouchCount;
    void Start()
    {
        flag = true;
        UIactive = false;
        storeUI = GeneralFunctions.StoreUI;
        wheel = GeneralFunctions.Wheel;

        touchesID = new int[5];
        touchesPlace = new string[5];
        prevTouchCount = 0;
        for (int i = 0; i < touchesID.Length; i++)
        {
            touchesID[i]= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(prevTouchCount > 0)
            ResetTouch();

        if (Input.touchCount > prevTouchCount)
        {
            int index = 0;
            for (int i=0; i< touchesID.Length; i++)
            {
                if(touchesID[i] == -1)
                {
                    index = i;
                    break;
                }
            }
            Touch t = Input.touches[index];
            //Debug.Log(t.fingerId);
            touchesID[index] = Input.touches[index].fingerId;
            prevTouchCount++;

            Ray ray = Camera.main.ScreenPointToRay(t.position);
            RaycastHit hit;
            string place = "";
            if (FingerOnInventory(t) && !FingerOnBottun(t, inventoryButton))
            {
                place = "inventory";
                GeneralFunctions.inventoryControl.indexOfTouch = t.fingerId;
                //Debug.Log("FingerOnInventory");
            }
            else if (FingerOnStore(t))
            {
                place = "store";
                //Debug.Log("ingerOnStore");
            }
            else if (FingerOnWheel(t))
            {
                place = "wheel";
                //Debug.Log("FingerOnWheel");
            }
            else if (FingerOnBottun(t, AbillityTop))
            {
                place = "inventory";
                if (!inventoryUI.gameObject.activeSelf)
                    GeneralFunctions.inventoryControl.UseAbility(GeneralFunctions.inventoryControl.ConvertSlotNameToIndex("AbillityTop"));
                //Debug.Log("AbillityTop");
            }
            else if (FingerOnBottun(t, AbillityMid))
            {
                place = "inventory";
                if (!inventoryUI.gameObject.activeSelf)
                    GeneralFunctions.inventoryControl.UseAbility(GeneralFunctions.inventoryControl.ConvertSlotNameToIndex("AbillityMid"));
                //Debug.Log("AbillityMid");
            }
            else if (FingerOnBottun(t, AbillityDown))
            {
                place = "inventory";
                if (!inventoryUI.gameObject.activeSelf)
                    GeneralFunctions.inventoryControl.UseAbility(GeneralFunctions.inventoryControl.ConvertSlotNameToIndex("AbillityDown"));
                //Debug.Log("AbillityDown");
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6))
            {
                if (GeneralFunctions.inventoryControl.PickUpItem(hit.collider))
                {
                    GeneralFunctions.Pooler.ReturnObjectToPool(hit.transform.parent.gameObject);
                    GeneralFunctions.Player.GetComponent<PlayerStats>().AddExp((float)(hit.collider.GetComponent<DropScript>().drop.rarity * hit.collider.GetComponent<DropScript>().amount));
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7))
            {
                AllDrops.activeStore = hit.transform.gameObject;
                GeneralFunctions.StoreController.itemNameToBuy = "Harpon";
                GeneralFunctions.StoreController.slotNumberToBuy = 1;
                GeneralFunctions.StoreController.SetBuyModeOnInventory(1, GeneralFunctions.FirstSlot.GetComponentInChildren<SlotStoreController>());
                GeneralFunctions.inventoryControl.OpenInventoryUI();
                Debug.Log("Raycast");
                GeneralFunctions.StoreController.LoadStore(hit.transform.gameObject);
                GeneralFunctions.inventoryControl.RefreshInventoryUI();
            }
            touchesPlace[index] = place;
            //if (flag)
            //{
            //    var temp = UI_Menus.GetComponent<Transform>();
            //    foreach (Transform t in temp)
            //    {
            //        if(!t.name.Equals("ShipInventory"))
            //            UIactive = UIactive || t.gameObject.activeSelf;
            //    }
            //    if (!UIactive)
            //    {
            //        Touch touch = Input.touches[0];
            //        if (!FingerOnInventory(touch) && !FingerOnStore(touch) && !FingerOnWheel(touch) && !FingerOnBottun(touch, inventoryButton) && !FingerOnBottun(touch, AbillityDown) && !FingerOnBottun(touch, AbillityTop) && !FingerOnBottun(touch, AbillityMid))
            //        {
            //            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            //            RaycastHit hit;
            //            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7))
            //            {// load store and store ui
            //                flag = false;
            //                AllDrops.activeStore = hit.transform.gameObject;
            //                //hit.transform.gameObject.GetComponent<StoreController>().LoadStore(hit.transform.gameObject);

            //                //if (!GeneralFunctions.inventoryControl.OpenOrCloseInventoryUI())
            //                //    GeneralFunctions.inventoryControl.OpenOrCloseInventoryUI();
            //                GeneralFunctions.inventoryControl.OpenInventoryUI();
            //                GeneralFunctions.StoreController.LoadStore(hit.transform.gameObject);
            //                GeneralFunctions.inventoryControl.RefreshInventoryUI();
            //            }
            //            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6))
            //            {
            //                if (GeneralFunctions.inventoryControl.PickUpItem(hit.collider))
            //                    GeneralFunctions.Pooler.ReturnObjectToPool(hit.transform.parent.gameObject);
            //            }
            //        }
            //    }
            //    UIactive = false;
            //}

        }
        
    }

    bool FingerOnInventory(Touch touch)
    {
        return GeneralFunctions.CheckMouseNearObject(inventoryUI.rect.width,
                                inventoryUI.rect.height,
                                inventoryUI.GetComponent<Transform>().position,
                                touch.position, 5) && inventoryUI.gameObject.activeSelf;
    }
    bool FingerOnStore(Touch touch)
    {
        return GeneralFunctions.CheckMouseNearObject(storeUI, touch.position, 5) && storeUI.gameObject.activeSelf;
    }
    bool FingerOnWheel(Touch touch)
    {
        return GeneralFunctions.CheckMouseNearObject(wheel, touch.position, 100);
    }
    bool FingerOnBottun(Touch touch, RectTransform button)
    {
        return GeneralFunctions.CheckMouseOnObject(button, touch.position);
    }
    private void ResetTouch()
    {
        //foreach (Touch touch in touches)
        //{
        //    Debug.Log(touch.fingerId + " is " + touch.phase);
        //    if(touch.phase == TouchPhase.Ended)
        //    {
        //        touches.Remove(touch);
        //    }
        //}
        for (int i=0; i< touchesID.Length; i++)
        {
            if (touchesID[i] != -1)
            {
                bool isExist = false;
                foreach (Touch realTouch in Input.touches)
                {
                    if (realTouch.fingerId == touchesID[i])
                        isExist = true;
                }
                if (!isExist)
                {
                    touchesID[i] = -1;
                    if (touchesPlace[i].Equals("inventory"))
                        GeneralFunctions.inventoryControl.indexOfTouch = -1;
                    touchesPlace[i] = "";
                    prevTouchCount--;
                }
            }
        }
    }
}
