using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreObjScript : MonoBehaviour
{
    public InventoryObject shopInventory;
    public List<int> slotStoreCancel = new List<int>();
    public Vector3 pos;
    private bool checkDistanceFromPlayer = false;

    public void OnEnable()
    {
        string[] storeNameCheck = gameObject.name.Split('(');
        if (storeNameCheck.Length <= 1)
        {
            checkDistanceFromPlayer = true;
        }
    }
    public void Update()
    {
        pos = transform.position;
        if (checkDistanceFromPlayer && AllDrops.activeStore != null && AllDrops.activeStore.name.Equals(gameObject.name))
        {
            Debug.Log(transform.position);
            if(Vector3.Distance(GeneralFunctions.Player.transform.position, transform.position) > 80)
            {
                Debug.Log("im far " + name);
                GeneralFunctions.StoreController.ExitStore();
            }
        }
    }
}
