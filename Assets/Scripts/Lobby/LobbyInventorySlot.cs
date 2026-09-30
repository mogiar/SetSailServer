using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyInventorySlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Text _text;
    public ItemData slotData = null;
    public int slotNumber;
    public eSlotType slotType;

    public void SetupSlotView()
    {        
        if (slotData == null)
        {           
            _icon.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
        }
        else
        {
            _icon.gameObject.SetActive(true);            
            _icon.sprite = slotData.Icon;

            _text.gameObject.SetActive(true);
            _text.text = GetTextByItem(slotData);
        }
    }

    public void InitializeSlot(ItemData item)
    {
        if (_icon == null)// || _text == null)
        {
            _icon = transform.Find("Icon").GetComponent<Image>();
            _text = transform.Find("Text").GetComponent<Text>();
        }
        slotData = item;
        SetupSlotView();
    }

    public LobbyInventorySlot(ItemData item)
    {
        if (item != null)
        {
            slotData = item;
        }
    }

    private string GetTextByItem(ItemData item)
    {
        string result = string.Empty;

        if (item.Type == eItemType.Equip)
        {
            result = "E";
            return result;
        }

        if (item.Type == eItemType.UltimateEquip)
        {
            result = "S";
            return result;
        }
        
        result = item.Amount.ToString();

        return result;
    }        
}

public enum eSlotType
{
    inventoryEquip,
    inventoryMaterial,
    inventoryUseable,
    shipEquipRight,
    shipEquipLeft,
    shipEquipUltimate,
    shipLoadout,
}
