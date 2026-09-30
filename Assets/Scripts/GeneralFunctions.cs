using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public static class GeneralFunctions
{ // some general function

    // Vars
    public static GameManager GameManager;
    public static GameObject Player;
    public static ObjectPooler Pooler;
    public static GameObject PlayerInventory;
    public static InventoryController inventoryControl;
    public static RectTransform Wheel;
    public static RectTransform WheelParent;
    public static RectTransform SpeedButton;
    public static RectTransform Compass;
    public static StoreController StoreController;
    public static PathManager PathManager;
    public static Dictionary<string, List<ItemType>> StoreSell;
    public static Dictionary<string, List<ItemType>> StoreRecive;
    public static Dictionary<string, List<ItemType>> StoreMust;
    public static RectTransform StoreUI;
    //public static Transform HealthBar;
    public static string AmmoName;
    public static GameObject FirstSlot;
    public static GameObject ControlMove;
    public static GameObject Camera;

    public static bool CheckMouseOnObject(float width, float height, Vector3 pos, Vector3 mousePos)
    {
        Vector2 Start = new Vector2(pos.x - (width / 2), pos.y - (height / 2));
        Vector2 End = new Vector2(pos.x + (width / 2), pos.y + (height / 2));
        return ((mousePos.x <= End.x && mousePos.x >= Start.x) && (mousePos.y <= End.y && mousePos.y >= Start.y));
    }
    public static bool CheckMouseOnObject(GameObject box, Vector3 mousePos)
    {
        float width = box.GetComponent<RectTransform>().rect.width;
        float height = box.GetComponent<RectTransform>().rect.height;
        Vector3 pos = box.GetComponent<Transform>().position;
        Vector2 Start = new Vector2(pos.x - (width / 2), pos.y - (height / 2));
        Vector2 End = new Vector2(pos.x + (width / 2), pos.y + (height / 2));
        return ((mousePos.x <= End.x && mousePos.x >= Start.x) && (mousePos.y <= End.y && mousePos.y >= Start.y));
    }
    public static bool CheckMouseOnObject(RectTransform box, Vector3 mousePos)
    {
        float width = box.rect.width;
        float height = box.rect.height;
        Vector3 pos = box.GetComponent<Transform>().position;
        Vector2 Start = new Vector2(pos.x - (width / 2), pos.y - (height / 2));
        Vector2 End = new Vector2(pos.x + (width / 2), pos.y + (height / 2));
        return ((mousePos.x <= End.x && mousePos.x >= Start.x) && (mousePos.y <= End.y && mousePos.y >= Start.y));
    }
    public static bool CheckMouseNearObject(RectTransform box, Vector3 mousePos, float sensetivite)
    {
        float width = box.rect.width;
        float height = box.rect.height;
        Vector3 pos = box.GetComponent<Transform>().position; // .GetComponent<Transform>()
        Vector2 Start = new Vector2(pos.x - (width / 2), pos.y - (height / 2));
        Vector2 End = new Vector2(pos.x + (width / 2), pos.y + (height / 2));
        return ((mousePos.x <= (End.x + sensetivite) && mousePos.x >= (Start.x - sensetivite)) && (mousePos.y <= (End.y + sensetivite) && mousePos.y >= (Start.y - sensetivite)));
    }
    public static bool CheckMouseNearObject(float width, float height, Vector3 pos, Vector3 mousePos, float sensetivite)
    {
        Vector2 Start = new Vector2(pos.x - (width / 2), pos.y - (height / 2));
        Vector2 End = new Vector2(pos.x + (width / 2), pos.y + (height / 2));
        return ((mousePos.x <= (End.x + sensetivite) && mousePos.x >= (Start.x - sensetivite)) && (mousePos.y <= (End.y + sensetivite) && mousePos.y >= (Start.y - sensetivite)));
    }
    public static Vector2[] CalcIntersectionOfCircleAndLine(Vector3 pointLineA, Vector3 pointLineB, float r_2, Vector3 CircleCenter)
    {
        r_2 = Mathf.Pow(r_2, 2);

        float mLine = (pointLineB.y - pointLineA.y) / (pointLineB.x - pointLineA.x);
        float nLine = (pointLineA.y - (mLine * pointLineA.x));

        float aX_2 = Mathf.Pow(mLine, 2) + 1;
        float bX = (2 * mLine * (nLine - CircleCenter.y)) - 2 * CircleCenter.x;
        float c = Mathf.Pow((nLine - CircleCenter.y), 2) - r_2 + Mathf.Pow(CircleCenter.x, 2);

        float temp = Mathf.Pow(bX, 2) - (4 * aX_2 * c);
        float q = Mathf.Sqrt(temp);
        float X_opt1 = (-bX + q) / (2 * aX_2);
        float X_opt2 = (-bX - q) / (2 * aX_2);

        float Y_opt1 = (mLine * X_opt1) + nLine;
        float Y_opt2 = (mLine * X_opt2) + nLine;
        if(X_opt1 == float.NaN || Y_opt1 == float.NaN || X_opt2 == float.NaN || Y_opt2 == float.NaN)
        {
            Debug.Log("?");
        }
        return new Vector2[] {new Vector2(X_opt1, Y_opt1), new Vector2(X_opt2, Y_opt2) };
    }
    public static void SetObject(GameManager gameManager, GameObject player, ObjectPooler pooler, GameObject playerInventory, InventoryController InventoryControl, RectTransform wheel,RectTransform compass, RectTransform wheelParent, RectTransform speedButton, StoreController storeController, PathManager pathManager, Dictionary<string, List<ItemType>> storeSell, Dictionary<string, List<ItemType>> storeRecive, Dictionary<string, List<ItemType>> storeMust, RectTransform storeUI, string ammoName, GameObject firstSlot, GameObject controlMove, GameObject camera)
    {
        GameManager = gameManager;
        Player = player;
        Pooler = pooler;
        PlayerInventory = playerInventory;
        inventoryControl = InventoryControl;
        Wheel = wheel;
        WheelParent = wheelParent;
        SpeedButton = speedButton;
        Compass = compass;
        StoreController = storeController;
        PathManager = pathManager;
        StoreSell = storeSell;
        StoreRecive = storeRecive;
        StoreMust = storeMust;
        StoreUI = storeUI;
        AmmoName = ammoName;
        FirstSlot = firstSlot;
        ControlMove = controlMove;
        Camera = camera;
    }
    public static bool IsTypeSuitStoreToSell(string storeName, ItemType type)
    {
        if (!StoreSell.ContainsKey(storeName))
            return true;
        return StoreSell[storeName].Contains(type);
    }
    public static bool IsTypeSuitStoreToRecive(string storeName, ItemType type)
    {
        if (!StoreRecive.ContainsKey(storeName))
            return true;
        return StoreRecive[storeName].Contains(type);
    }
    
    //public void CalcRandomPosition(float out_x, float out_z, float out_width, float out_height, float out_length, float in_x, float in_z, float in_width, float in_height, float in_length)
    //{
    //    CalcRandomPositionInTrapezoid(out_x, out_z, in_x, in_z, out_width, in_width);
    //    CalcRandomPositionInTrapezoid((out_x + out_width), out_z, (in_x + in_width), in_z, out_width, in_width);
    //    CalcRandomPositionInTrapezoid((out_x + out_width + out_length), (out_z + (out_height / 2)), (in_x + in_width + in_length), (in_z + (in_height / 2)), out_width, in_width);
    //    CalcRandomPositionInTrapezoid((out_x + out_width), (out_z + out_height), (in_x + in_width), (in_z + in_height), out_width, in_width);
    //    CalcRandomPositionInTrapezoid(out_x, (out_z + out_height), in_x, (in_z + in_height), out_width, in_width);
    //    CalcRandomPositionInTrapezoid((out_x - out_length), (out_z + (out_height / 2)), (in_x - in_length), (in_z + (in_height / 2)), out_width, in_width);
    //}
    //public void CalcRandomPositionInTrapezoid(float bot_x, float bot_z, float top_x, float top_z, float bot_width, float top_width)
    //{


    //}
}

