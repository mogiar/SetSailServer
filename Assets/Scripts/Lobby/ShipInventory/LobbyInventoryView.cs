using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInventoryView : MonoBehaviour
{
    private LobbyInventoryModel _model;

    [SerializeField] private GameObject _slot;
    [SerializeField] private RectTransform _inventoryPanel;
    [SerializeField] private RectTransform _shipInventoryPanel;
    [SerializeField] private LobbyInventorySlot _leftEquipSlot;
    [SerializeField] private LobbyInventorySlot _rightEquipSlot;
    [SerializeField] private LobbyInventorySlot _UltimateEquipSlot;

    private LobbyInventorySlot _selectedSlot;
    private LobbyInventorySlot _hoveredSlot;

    private LobbyInventorySlot[] _slots;
    private LobbyInventorySlot[] _shipSlots;

    public Sprite[] tempSprites;

    private const string EQUIP_SLOT = "ShipEquipSlot";
    private const string INVENTORY_SLOT = "InventorySlot";
    private const string INVENTORY_EQUIP_SLOT = "InventoryEquipSlot";
    private const string INVENTORY_MATERIAL_SLOT = "InventoryMaterialSlot";
    private const string INVENTORY_USEABLE_SLOT = "InventoryUseableSlot";
    private const string LOADOUT_SLOT = "LoadoutSlot";
    private const string ULTIMATE_EQUIP_SLOT = "UltimateEquipSlot";

    public void Init(LobbyInventoryModel model)
    {
        _model = model;
        CreateDubugInventory();

        _slot.GetComponent<LobbyInventorySlot>().InitializeSlot(null);

        _slots = new LobbyInventorySlot[20];
        _shipSlots = new LobbyInventorySlot[5];
        _slots[0] = _slot.GetComponent<LobbyInventorySlot>();
        _slots[0].slotNumber = 0;

        for (int i = 1; i < 20; i++)
        {
            var slot = Instantiate(_slot, _inventoryPanel);
            _slots[i] = slot.GetComponent<LobbyInventorySlot>();
           // _slots[i].name = INVENTORY_SLOT;
            _slots[i].slotType = eSlotType.inventoryEquip;
            _slots[i].slotNumber = i;
        }

        for (int i = 0; i < 5; i++)
        {
            var slot = Instantiate(_slot, _shipInventoryPanel);
            _shipSlots[i] = slot.GetComponent<LobbyInventorySlot>();
           // _shipSlots[i].name = LOADOUT_SLOT;
            _shipSlots[i].slotType = eSlotType.shipLoadout;
            _shipSlots[i].slotNumber = i;
        }

        _leftEquipSlot.InitializeSlot(null);
        _rightEquipSlot.InitializeSlot(null);
        _UltimateEquipSlot.InitializeSlot(null);

        OnEquipTab();
    }


    #region --- Items Swap ---

    public void OnItemDown(LobbyInventorySlot item)
    {
        if (item.slotData == null)
        {
            _selectedSlot = null;
            return;
        }

        _selectedSlot = item;
    }

    public void OnItemEnter(LobbyInventorySlot item)
    {
        if (_selectedSlot == null) return;

        _hoveredSlot = item;
    }

    public void OnItemUp()
    {
        if (!ShouldSawp()) return;

       // _model.SwapSlots(_selectedSlot.name, _hoveredSlot.name, _selectedSlot.slotData, _hoveredSlot.slotData);
        _model.SwapSlotsByView(_selectedSlot, _hoveredSlot);

        var tempSlot = new LobbyInventorySlot(_selectedSlot.slotData);        
        _selectedSlot.InitializeSlot(_hoveredSlot.slotData);
        _hoveredSlot.InitializeSlot(tempSlot.slotData);

        _selectedSlot = null;
        _hoveredSlot = null;

    }

    public void OnItemExit()
    {
        if (_selectedSlot == null) return;

        _hoveredSlot = _selectedSlot;
    }

    private bool ShouldSawp()
    {
        if (_selectedSlot == null) return false;

        /*if (_hoveredSlot.name.StartsWith(EQUIP_SLOT) && _selectedSlot.slotData.Type != eItemType.Equip) return false;
        if (_hoveredSlot.name.StartsWith(ULTIMATE_EQUIP_SLOT) && _selectedSlot.slotData.Type != eItemType.UltimateEquip) return false;*/

        if ((_hoveredSlot.slotType == eSlotType.shipEquipLeft || 
                _hoveredSlot.slotType == eSlotType.shipEquipRight) && 
                _selectedSlot.slotData.Type != eItemType.Equip) 
            return false;

        if (_hoveredSlot.slotType == eSlotType.shipEquipUltimate && 
                _selectedSlot.slotData.Type != eItemType.UltimateEquip) 
            return false;

        if (_hoveredSlot.slotData != null)
        {
            if (_selectedSlot.slotType == eSlotType.shipEquipUltimate && 
                    _hoveredSlot.slotData.Type != eItemType.UltimateEquip)
                return false;
            if ((_selectedSlot.slotType == eSlotType.shipEquipLeft ||
                    _selectedSlot.slotType == eSlotType.shipEquipRight) &&
                    _hoveredSlot.slotData.Type != eItemType.Equip) 
                return false;
        }

        // _selectedSlot - first touch
        // _hoveredSlot - second touch
        return true;
    }

    #endregion Items Swap


    #region --- Buttons ---

    public void OnEquipTab()
    {
        for (int i = 0; i < _model.PlayerEquipInventory.Length; i++)
        {
            _slots[i].InitializeSlot(_model.PlayerEquipInventory[i]);
           // _slots[i].name = INVENTORY_EQUIP_SLOT;
            _slots[i].slotType = eSlotType.inventoryEquip;
        }
    }

    public void OnUseableTab()
    {
        for (int i = 0; i < _model.PlayerUseableInventory.Length; i++)
        {
            _slots[i].InitializeSlot(_model.PlayerUseableInventory[i]);
           // _slots[i].name = INVENTORY_USEABLE_SLOT;
            _slots[i].slotType = eSlotType.inventoryUseable;
        }
    }

    public void OnMaterialsTab()
    {
        for (int i = 0; i < _model.PlayerMaterialInventory.Length; i++)
        {
            _slots[i].InitializeSlot(_model.PlayerMaterialInventory[i]);
          //  _slots[i].name = INVENTORY_MATERIAL_SLOT;
            _slots[i].slotType = eSlotType.inventoryMaterial;
        }
    }

    public void OnPlayButton()
    {
        // Queue to match
        // ----- turn off lobby

        GeneralFunctions.GameManager.PlayerInsertGame();
        

    }

    #endregion Buttons


    #region --- Debug Methoods ---
    public void CreateDubugInventory()
    {
        _model.PlayerEquipInventory = new ItemData[20];
        _model.PlayerUseableInventory = new ItemData[20];
        _model.PlayerMaterialInventory = new ItemData[20];
        _model.LoadoutInventory = new ItemData[5];
        int itemsInInventory = Random.Range(10, 30);

        for (int i = 0; i < 5; i++)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    var bullets = new ItemData(5, "Bullets", tempSprites[4], eItemType.Useable, 99);
                    _model.LoadoutInventory[i] = bullets;
                    break;

                case 1:
                    var gold = new ItemData(7, "Gold", tempSprites[6], eItemType.Materials, 99);
                    _model.LoadoutInventory[i] = gold;
                    break;
            }
        }

        for (int i = 0; i < itemsInInventory; i++)
        {
            int random = Random.Range(0, 7);

            switch (random)
            {
                case 0:
                    var canon = new ItemData(1, "Canon", tempSprites[0], eItemType.Equip, 1);
                    for (int j = 0; j < _model.PlayerEquipInventory.Length; j++)
                    {
                        if (_model.PlayerEquipInventory[j] == null)
                        {
                            _model.PlayerEquipInventory[j] = canon;
                            break;
                        }
                    } 
                    break;

                case 1:
                    var harpon = new ItemData(2, "Harpon", tempSprites[1], eItemType.UltimateEquip, 1);
                    for (int j = 0; j < _model.PlayerEquipInventory.Length; j++)
                    {
                        if (_model.PlayerEquipInventory[j] == null)
                        {
                            _model.PlayerEquipInventory[j] = harpon;
                            break;
                        }
                    }
                    break;

                case 2:
                    var fish = new ItemData(3, "Fish", tempSprites[2], eItemType.Materials, 99);
                    for (int j = 0; j < _model.PlayerMaterialInventory.Length; j++)
                    {
                        if (_model.PlayerMaterialInventory[j] == null)
                        {
                            _model.PlayerMaterialInventory[j] = fish;
                            break;
                        }
                    }
                    break;

                case 3:
                    var wood = new ItemData(4, "Wood", tempSprites[3], eItemType.Materials, 99);
                    for (int j = 0; j < _model.PlayerMaterialInventory.Length; j++)
                    {
                        if (_model.PlayerMaterialInventory[j] == null)
                        {
                            _model.PlayerMaterialInventory[j] = wood;
                            break;
                        }
                    }
                    break;

                case 4:
                    var bullets = new ItemData(5, "Bullets", tempSprites[4], eItemType.Useable, 10);
                    for (int j = 0; j < _model.PlayerUseableInventory.Length; j++)
                    {
                        if (_model.PlayerUseableInventory[j] == null)
                        {
                            _model.PlayerUseableInventory[j] = bullets;
                            break;
                        }
                    }
                    break;

                case 5:
                    var rune = new ItemData(6, "Rune", tempSprites[5], eItemType.Useable, 9);
                    for (int j = 0; j < _model.PlayerUseableInventory.Length; j++)
                    {
                        if (_model.PlayerUseableInventory[j] == null)
                        {
                            _model.PlayerUseableInventory[j] = rune;
                            break;
                        }
                    }
                    break;

                case 6:
                    var gold = new ItemData(7, "Gold", tempSprites[6], eItemType.Materials, 99);
                    for (int j = 0; j < _model.PlayerMaterialInventory.Length; j++)
                    {
                        if (_model.PlayerMaterialInventory[j] == null)
                        {
                            _model.PlayerMaterialInventory[j] = gold;
                            break;
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }

    #endregion Debug Methoods    
}
