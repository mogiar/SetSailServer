using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SetSailClient : MonoBehaviour
{
    private LoginController _loginController;
    private LobbyInventoryController _lobbyInventoryController;
    //private StoreController _inventoryController;
    //private KingdomController _inventoryController;
    //private ForgeController _inventoryController;

    [Header("Screens")]
    [SerializeField] private LobbyInventoryView _ShipView;
    [SerializeField] private GameObject _logginView;

    [Header("LoginFields")]
    [SerializeField] private InputField _userName;
    [SerializeField] private InputField _password;
    private TouchScreenKeyboard _keyboard;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        //_lobbyInventoryController = new LobbyInventoryController();
        //_lobbyInventoryController.Init(_ShipView);
        _logginView.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLoginSuccess()
    {
        _lobbyInventoryController = new LobbyInventoryController();
        _lobbyInventoryController.Init(_ShipView);
        _logginView.SetActive(false);
        _ShipView.gameObject.SetActive(true);
    }

    public void LoginButton()
    {
        //if success
        Debug.Log(_userName.text);
        NetworkManager.Singleton.StartClient();
        OnLoginSuccess();
    }

    public void OnInputFieldClicked()
    {
        _keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
}
