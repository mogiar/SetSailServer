using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Text.RegularExpressions;

public static class DAL
{
    private static String Connection_String;
    public static SQLiteConnection connection;

    public static Dictionary<string, int> indexByName;

    public static string PlayersFieldsNames;
    public static string ItemsFieldsNames;

    public static string PlayerIDName;
    public static string ItemIDName;
    public static string ItemPlayerIDName;


    public static void SetUpDB()
    {
        Connection_String = @"Data Source=..\\..\\DataBase\\DataBase.db";
        indexByName = new Dictionary<string, int>();
        connection = null;
        PlayerIDName = "ID";
        ItemIDName = "ID";
        ItemPlayerIDName = "UniqueID";
        PlayersFieldsNames = PlayerIDName + ", Name, Gold, TimeInGame";
        ItemsFieldsNames = "";
    }
    public static void InitDB()
    {
        TextAsset txt = Resources.Load("Players") as TextAsset;
        string[] lines = txt.text.Split("\r\n".ToCharArray());

        string[] category = lines[0].Split(',');
        for (int i = 0; i < category.Length; i++)
        {
            indexByName[category[i]] = i;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length > 0)
            {
                string[] values = lines[i].Split(',');
                string[] arr = new string[indexByName.Count];
                for (int j = 0; j < values.Length; j++)
                {
                    arr[j] = values[j];
                }

            }
        }
    }
    public static void OpenConnect()
    {
        if (!System.IO.File.Exists(Connection_String))
        {
            SQLiteConnection.CreateFile(Connection_String);
            connection = new SQLiteConnection(Connection_String);
            connection.Open();
            Debug.LogError("DB doesn't exist");
        }
        if (connection == null)
            connection = new SQLiteConnection(Connection_String);
        if (connection.State == ConnectionState.Closed)
            connection.Open();

        Console.WriteLine("Open DB.");
    }
    public static void CloseConnect()
    {
        if (connection.State != ConnectionState.Closed)
            connection.Close();
    }
    public static void InsertOrUpdatePlayer(PlayerData player, bool isDBOpen)
    {
        try
        {
            if(!isDBOpen)
                OpenConnect();

            SQLiteCommand commandItems = new SQLiteCommand(null, DAL.connection);
            SQLiteCommand command = new SQLiteCommand("SELECT EXISTS(SELECT 1 FROM Players WHERE ID='" + player.ID + "') as flag", DAL.connection);
            
            command.Prepare();
            var readerID = command.ExecuteReader();
            readerID.Read();
            var result = readerID[readerID.GetName(0)];

            if (result.ToString().Equals("0"))
            {
                command.CommandText = "INSERT INTO Players (ID, Name, TimeInGame) VALUES('" + player.ID + "', '" + player.Name + "', '" + player.TimeInGame + "')";
                commandItems.CommandText = "INSERT INTO Items (ID, UniqueID, PlayerID, Name, Type, Amount, UpgradeSlot, LocationInventory) VALUES ";
                
                commandItems.CommandText += AddItems(player.PlayerEquipInventory, player.ID + "", "PlayerEquipInventory") + ", ";
                commandItems.CommandText += AddItems(player.PlayerUseableInventory, player.ID + "", "PlayerUseableInventory") + ", ";
                commandItems.CommandText += AddItems(player.PlayerMaterialInventory, player.ID + "", "PlayerMaterialInventory") + ", ";
                commandItems.CommandText += AddItems(player.LoadoutInventory, player.ID + "", "LoadoutInventory") + ", ";
                
                commandItems.CommandText += "(" + AddItem(player.RightEquip, player.ID + "", "RightEquip") + "), ";
                commandItems.CommandText += "(" + AddItem(player.LeftEquip, player.ID + "", "LeftEquip") + "), ";
                commandItems.CommandText += "(" + AddItem(player.UltimateEquip, player.ID + "", "UltimateEquip") + ")";

                commandItems.Prepare();
                commandItems.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = "UPDATE Players SET TimeInGame=TimeInGame+'"+player.TimeInGame+"' WHERE ID='" + player.ID + "'";
                foreach(var itemUniID in player.ItemsChange)
                {
                    string change = GetUpdateItemQuery(itemUniID, player);
                    if(change.Equals(""))
                        commandItems.CommandText = "DELETE FROM Items WHERE PlayerID='" + player.ID + "' AND UniqueID='" + itemUniID + "'";
                    else
                        commandItems.CommandText = "UPDATE Items SET " + change + " WHERE PlayerID='" + player.ID + "' AND UniqueID='" + itemUniID + "'";
                    commandItems.Prepare();
                    commandItems.ExecuteNonQuery();
                }
            }

            command.Prepare();
            command.ExecuteNonQuery();
            

            readerID.Close();
            command.Dispose();
            commandItems.Dispose();

            if (!isDBOpen)
                CloseConnect();
        }
        catch (SQLiteException e)
        {
            DAL.CloseConnect();
        }
    }
    public static PlayerData GetPlayer(int playerID, bool isDBOpen)
    {
        PlayerData player = new PlayerData();
        try
        {
            if (!isDBOpen)
                OpenConnect();

            SQLiteCommand command = new SQLiteCommand("SELECT EXISTS(SELECT 1 FROM Players WHERE ID='" + playerID + "') as flag", DAL.connection);
            SQLiteCommand commandItems = new SQLiteCommand("", DAL.connection);

            command.Prepare();
            var reader = command.ExecuteReader();
            reader.Read();
            var res = reader[reader.GetName(0)];

            if (res.ToString().Equals("0"))
            {
                return null; // player doesn't exist
            }
            else
            {
                commandItems.CommandText = "";
                commandItems.Prepare();
                var readerItems = commandItems.ExecuteReader();
                readerItems.Read();
                var resItems = readerItems[readerItems.GetName(0)];



                ItemData[] PlayerEquipInventory = new ItemData[1];
                ItemData[] PlayerUseableInventory = new ItemData[1];
                ItemData[] PlayerMaterialInventory = new ItemData[1];
                ItemData[] LoadoutInventory = new ItemData[1];
                ItemData RightEquip = new ItemData();
                ItemData LeftEquip = new ItemData();
                ItemData UltimateEquip = new ItemData();

                player = new PlayerData(int.Parse(reader["ID"].ToString()), reader["Name"].ToString(), 0, PlayerEquipInventory, PlayerUseableInventory, PlayerMaterialInventory, LoadoutInventory, RightEquip, LeftEquip, UltimateEquip);
            }

            reader.Close();
            command.Dispose();

            if (!isDBOpen)
                CloseConnect();
        }
        catch (SQLiteException e)
        {
            DAL.CloseConnect();
        }
        return null;
    }
    private static void ResetDB()
    {
        SQLiteCommand command = new SQLiteCommand("DROP TABLE Items", DAL.connection);
        command.Prepare();
        command.ExecuteNonQuery();
        command = new SQLiteCommand("DROP TABLE Players", DAL.connection);
        command.Prepare();
        command.ExecuteNonQuery();
        command.Dispose();
    }

    #region help functions
    private static string AddItems(ItemData[] arr, string playerID, string LocationInventory)
    { // with ()
        string ans = "";
        for (int i=0; i<arr.Length; i++)
        {
            ans += "( " + AddItem(arr[i], playerID, LocationInventory) + " ), ";
        }
        ans = ans.Substring(0, ans.Length - 2);
        return ans;
    }
    private static string AddItem(ItemData item, string playerID, string LocationInventory)
    { // without ()
        return "'" + item.ID + "', '" + item.UniqueID + "', '" + playerID + "', '" + item.Name + "', '" + item.Type + "', '" + item.Amount + "', '" + item.UpgradeSlot + "', '" + LocationInventory + "'";
    }//ID, UniqueID, PlayerID, Name, Type, Amount, UpgradeSlot, LocationInventory

    private static string GetUpdateItemQuery(int UniqueID, PlayerData player)
    {
        string location = GetLocation(UniqueID, player);
        ItemData item = GetLocation(location, player, UniqueID);
        if (item.Amount == -1)
            return "";
        return "Name='" + item.Name + "', Amount='" +item.Amount + "', UpgradeSlot='" + item.UpgradeSlot + "', LocationInventory='" + location + "'";
    }
    private static ItemData GetLocation(string location, PlayerData player, int UniqueID)
    {
        List<ItemData> list = new List<ItemData>();
        switch (location)
        {
            case "RightEquip":
                return player.RightEquip;
            case "LeftEquip":
                return player.LeftEquip;
            case "UltimateEquip":
                return player.UltimateEquip;
            case "PlayerEquipInventory":
                list = new List<ItemData>(player.PlayerEquipInventory);
                break;
            case "PlayerUseableInventory":
                list = new List<ItemData>(player.PlayerUseableInventory);
                break;
            case "PlayerMaterialInventory":
                list = new List<ItemData>(player.PlayerMaterialInventory);
                break;
            case "LoadoutInventory":
                list = new List<ItemData>(player.LoadoutInventory);
                break;
        }
        return list.Find(item => item.UniqueID == UniqueID);
    }
    private static string GetLocation(int UniqueID, PlayerData player)
    {
        if (player.RightEquip.UniqueID == UniqueID)
            return "RightEquip";

        if (player.LeftEquip.UniqueID == UniqueID)
            return "LeftEquip";

        if (player.UltimateEquip.UniqueID == UniqueID)
            return "UltimateEquip";

        List<ItemData> list = new List<ItemData>(player.PlayerEquipInventory);
        if (list.Find(item => item.UniqueID == UniqueID) != null)
            return "PlayerEquipInventory";

        list = new List<ItemData>(player.PlayerUseableInventory);
        if (list.Find(item => item.UniqueID == UniqueID) != null)
            return "PlayerUseableInventory";

        list = new List<ItemData>(player.PlayerMaterialInventory);
        if (list.Find(item => item.UniqueID == UniqueID) != null)
            return "PlayerMaterialInventory";

        list = new List<ItemData>(player.LoadoutInventory);
        if (list.Find(item => item.UniqueID == UniqueID) != null)
            return "LoadoutInventory";

        return "";
    }
    
    #endregion

}
/*public int ID;
    public int UniqueID;
    public string Name;
    public eItemType Type;
    public int Amount;
    public int UpgradeSlot;*/
public class PlayerData
{
    public int ID;
    public string Name;
    public float TimeInGame;

    public ItemData[] PlayerEquipInventory;
    public ItemData[] PlayerUseableInventory;
    public ItemData[] PlayerMaterialInventory;
    public ItemData[] LoadoutInventory;
    public HashSet<int> ItemsChange;

    public ItemData RightEquip;
    public ItemData LeftEquip;
    public ItemData UltimateEquip;

    public PlayerData(int ID, string Name, float TimeInGame, ItemData[] PlayerEquipInventory, ItemData[] PlayerUseableInventory, ItemData[] PlayerMaterialInventory, ItemData[] LoadoutInventory, ItemData RightEquip, ItemData LeftEquip, ItemData UltimateEquip)
    {
        this.ID = ID;
        this.Name = Name;
        this.TimeInGame = TimeInGame;

        this.PlayerEquipInventory = PlayerEquipInventory;
        this.PlayerUseableInventory = PlayerUseableInventory;
        this.PlayerMaterialInventory = PlayerMaterialInventory;
        this.LoadoutInventory = LoadoutInventory;
        this.ItemsChange = new HashSet<int>();

        this.RightEquip = RightEquip;
        this.LeftEquip = LeftEquip;
        this.UltimateEquip = UltimateEquip;
       }
    public PlayerData(int ID, string Name, float TimeInGame)
    {
        this.ID = ID;
        this.Name = Name;
        this.TimeInGame = TimeInGame;
    }
    public PlayerData()
    {

    }
}

