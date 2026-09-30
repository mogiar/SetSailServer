using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DynamicRandom : MonoBehaviour
{
    public List<string> ignoreUnits;
    // Start is called before the first frame update
    void Init()
    {
        ignoreUnits.Add("Drop");
    }

    // Update is called once '/per frame
    void Update()
    {
        
    }
    
    public void PlaceRandomDynamic(float out_radius, float in_radius, Dictionary<string, int[]> unitsToSpawn, Vector3 PlayerPosition)
    {
        foreach (var unitName in unitsToSpawn.Keys)
        {
            //Debug.Log("left " + GeneralFunctions.Pooler.GetCountInGame(unitName) + " from " + unitName);
            int numberSpawn = Mathf.Max(unitsToSpawn[unitName][GeneralFunctions.GameManager.level - 1] - GeneralFunctions.Pooler.GetCountInGame(unitName), 0);
            if (numberSpawn < 0)
            { // ------- what i do?
                numberSpawn = GeneralFunctions.Pooler.GetCurrentNumerInPool(unitName);
                Debug.Log("left only " + numberSpawn + " from " + unitName);
            }
                
            for (int i = 0; i < numberSpawn; i++)
            {
                Vector2 pos = RandomDynamicInCircle(out_radius, in_radius);
                GeneralFunctions.Pooler.SpawnFromPool(unitName, pos + new Vector2(PlayerPosition.x, PlayerPosition.z));
            }
            //Debug.Log("left " + GeneralFunctions.Pooler.GetCountInGame(unitName) + " from " + unitName);
        }

    }
    public void PlaceRandomStatic(float range, float in_radius, Dictionary<string, int[]> unitsToSpawn, Vector2 PathPosition, Vector2 playerPosition)
    {
        foreach (var unitName in unitsToSpawn.Keys)
        {
            int numberSpawn = unitsToSpawn[unitName][GeneralFunctions.GameManager.level - 1];
            int high = GeneralFunctions.Pooler.GetHighetLevel(unitName);
            if (!unitName.Equals("equipshop"))
            {
                int numInGroup = (int)(0.4 * numberSpawn);

                // random in groups
                int currentNumber = Random.Range(numInGroup / 10, numInGroup);
                while (currentNumber > 6)
                { // every 10 radius we can insert 30
                    Vector2 positionCurrentCircle = RandomDynamicInCircle(range - 50, in_radius + 10) + PathPosition;
                    float radius = Mathf.Max(10, currentNumber / 3);
                    for (int i = 0; i < currentNumber; i++)
                    {
                        Vector2 pos = RandomStaticInCircle(radius, 0, high, positionCurrentCircle);
                        GameObject obj = null;
                        if (unitName.Equals("equipshop"))
                            AllDrops.GenerateShopDemo(pos, unitName);
                        else
                            obj = GeneralFunctions.Pooler.SpawnFromPool(unitName, pos, GetRandomQ());
                        if (unitName == "Drop")
                        {
                            obj.GetComponentInChildren<DropScript>().drop = AllDrops.GetDropByName("Coins");
                            obj.GetComponentInChildren<DropScript>().amount = 1;
                        }
                    }
                    numInGroup -= currentNumber;
                    currentNumber = Random.Range(numInGroup / 10, numInGroup);
                }
            }
            

            for (int i = 0; i < (int)(0.6 * numberSpawn); i++)
            {
                Vector2 pos = RandomStaticInSquare(range, in_radius, high, PathPosition, playerPosition);
                GameObject obj = null;
                if (unitName.Equals("equipshop")) // demo
                    AllDrops.GenerateShopDemo(pos, unitName); // demo
                else
                    obj = GeneralFunctions.Pooler.SpawnFromPool(unitName, pos, GetRandomQ());
                if (unitName == "Drop")
                {
                    obj.GetComponentInChildren<DropScript>().drop = AllDrops.GetDropByName("Coins");
                    obj.GetComponentInChildren<DropScript>().amount = 1;
                }
            }
        }
    }
    

    public Vector2 RandomDynamicInCircle(float out_radius, float in_radius)
    {
        Vector2 pos = Random.insideUnitCircle * out_radius;
        
        while (Vector2.Distance(pos, new Vector2(0, 0)) < in_radius)
        {
            pos = Random.insideUnitCircle * out_radius;
        }

        return pos;
    }
    public Vector2 RandomStaticInSquare(float range, float in_radius, int highetLevel, Vector2 PathPosition, Vector2 playerPosition)
    {
        Vector2 pos = new Vector2(Random.Range(PathPosition.x - range, PathPosition.x + range),
                                        Random.Range(PathPosition.y - range, PathPosition.y + range));
        int dir = 1;
        if (highetLevel == -1)
            dir = -1;

        // check if position is near to player
        while (Vector2.Distance(pos, playerPosition) < in_radius)
        {
            pos = new Vector2(Random.Range(PathPosition.x - range, PathPosition.x + range),
                                        Random.Range(PathPosition.y - range, PathPosition.y + range));
        }

        RaycastHit[] hits = Physics.RaycastAll(new Vector3(pos.x, highetLevel, pos.y), new Vector3(0, dir, 0), 1 << 12);
        for (int i = 0; i < hits.Length; i++)
        {
            if (GeneralFunctions.Pooler.IsStatic(GetName(hits[i].transform.gameObject.name)))
            {
                return RandomStaticInSquare(range, in_radius, highetLevel, PathPosition, playerPosition);
            }

        }

        return pos;
    }
    public Vector2 RandomStaticInCircle(float out_radius, float in_radius, int highetLevel, Vector2 startPosition)
    {
        Vector2 pos = (Random.insideUnitCircle * out_radius) + startPosition;
        int dir = 1;
        if (highetLevel == -1)
            dir = -1;

        while (Vector2.Distance(pos, startPosition) < in_radius)
        {
            pos = (Random.insideUnitCircle * out_radius) +startPosition;
        }

        RaycastHit[] hits = Physics.RaycastAll(new Vector3(pos.x, highetLevel, pos.y), new Vector3(0, dir, 0), 1 << 12);
        for (int i = 0; i < hits.Length; i++)
        {
            if (GeneralFunctions.Pooler.IsStatic(GetName(hits[i].transform.gameObject.name)))
            {
                return RandomStaticInCircle(out_radius, in_radius, highetLevel, startPosition);
            }
        }

        return pos;
    }

    private string GetName(string name)
    {
        return new Regex(@"([a-zA-Z]+)").Match(name).Value;
    }
    private Quaternion GetRandomQ()
    {
        return new Quaternion(0, Random.Range(0, 360), 0, 0);
    }

}
