using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerControls : NetworkBehaviour
{
    public Rigidbody _rb;
    //private bool isDead;


    [SerializeField] public Transform _modelTransform;


    //public bool isPlayerHold;

    //public bool deleteAfterDemo;


    
    public void InitMatch()
    {
        gameObject.SetActive(true);
        //GeneralFunctions.GameManager.PlayerInsertGame();
        GeneralFunctions.ControlMove.SetActive(true);
        GeneralFunctions.GameManager.ControlMove.GetComponent<MovementControl>().NewPlayer();
        GeneralFunctions.Camera.GetComponent<CameraScript>()._target = GeneralFunctions.Player.transform.GetChild(2);
    }
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //isPlayerHold = false;
        gameObject.DrawCircle(250, 0.5f);
        
        //deleteAfterDemo = true;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;
        //if (GeneralFunctions.Player == null)
        //{
        //    Debug.Log("checcck");
        //    GeneralFunctions.Player = this.gameObject;
        //    //_Rotater = GeneralFunctions.Rotator.transform;
        //    //_TargetRotation = GeneralFunctions.Rotator.transform.GetChild(0).transform;
        //    _wheel = GeneralFunctions.Wheel;
        //    _wheelParent = GeneralFunctions.WheelParent;
        //    _speedButton = GeneralFunctions.SpeedButton;
        //    _compass = GeneralFunctions.Compass;
        //    widthWheel = _wheel.rect.width;
        //    heightWheel = _wheel.rect.height;
        //    widthSpeed = _speedButton.rect.width;
        //    heightSpeed = _speedButton.rect.height;
        //    originalPosSpeed = _speedButton.transform.position;
        //}
        //if (deleteAfterDemo)
        //{
        //    GeneralFunctions.inventoryControl.deleteAfterDemo();
        //    deleteAfterDemo = false;
        //}
        //Debug.Log(UnityEngine.Random.Range(0, 2));
        //CheckSpherecast(this.gameObject, 100, 5, LayerMask.GetMask("Wall"));
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    GeneralFunctions.PathManager.map.ExpandBoard();
        //}
        //EFFECT - rotate ship to direction of rotation





    }

    

    //private float AngleToCover(Vector2 from, Vector2 to)
    //{
    //    float a = Vector2.Angle(from, to);
    //    return a;
    //}
    

    public void OpenOrCloseInventory()
    { // when player hit inventory buttom the inventory open or close
        GeneralFunctions.inventoryControl.OpenOrCloseInventoryUI();
    }
    public void OperateTradeButton()
    { // after the player accept the trade offer and the store aprove his trade
        GeneralFunctions.inventoryControl.OperateTrade();
        GeneralFunctions.StoreController.AcceptTradeButton.interactable = false;
        
        //Transform slot = GeneralFunctions.StoreController.slotStore.gameObject.transform.GetChild(0).GetChild(0).GetChild(0);
        //slot.Find("ItemImage").GetComponent<Image>().color = Color.grey;
        //slot.Find("ItemBarText").GetComponent<Text>().text = "SOLD";
        //slot.Find("ItemBar").GetChild(0).GetComponent<Image>().color = Color.green;
    }
    public void CancelTrade()
    { // after the player accept the trade offer and the store aprove his trade
        GeneralFunctions.inventoryControl.CancelPurchase();
    }

    //public bool TakeControlOverStore(GameObject store)
    //{
    //    if (store != null && store.GetComponent<StoreController>().Manager == null)
    //        return false;
    //    store.GetComponent<StoreController>().UpdateManager(gameObject);
    //    return true;
    //}

    public void OpenEndGameUI()
    {
        //_endGameUI.gameObject.SetActive(true);
        Debug.Log("you dead son");
        //isDead = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
