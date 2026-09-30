using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;

public enum Theme
{
    ice,
    stone,
    cristal,
    random
}
public class ObjectPooler : MonoBehaviour
{
    public static Dictionary<string, int> indexByName = new Dictionary<string, int>();
    public class PathInfo
    {
        public string name;
        public Dictionary<string, int[]> staticUnits;
        public Dictionary<string, int[]> dynamicUnits;

        public PathInfo(string name)
        {
            this.name = name;
        }
        public PathInfo(string name, Dictionary<string, int[]> staticUnits, Dictionary<string, int[]> dynamicUnits)
        {
            this.name = name;
            this.staticUnits = staticUnits;
            this.dynamicUnits = dynamicUnits;
            
        }
        public int StaticAmount(string unitName, int level)
        {
            return staticUnits[unitName][level-1];
        }
        public int DynamicAmount(string unitName, int level)
        {
            return dynamicUnits[unitName][level-1];
        }
        
    }

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public Transform prefabTran;
        public string[] Properties;

        public Pool()
        {

        }
        public Pool(GameObject gameObject, string[] prope)
        {
            this.prefab = gameObject;
            this.Properties = prope;
        }
        public Pool(Transform gameObject, string[] prope)
        {
            this.prefabTran = gameObject;
            this.Properties = prope;
        }
        public int GetInt(string key)
        {
            if (!indexByName.ContainsKey(key))
                Debug.Log("g");
            int y = indexByName[key];
            return int.Parse(Properties[y]);
        }
        public string GetString(string key)
        {
            return Properties[indexByName[key]];
        }
        public float GetFloat(string key)
        {
            int test = indexByName[key];
            return float.Parse(Properties[test]);
        }
    }

    #region Singelton
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion


    private List<Pool> pools = new List<Pool>();
    public Dictionary<string, Queue<GameObject>> poolQueue;
    public Dictionary<string, Pool> poolProperties = new Dictionary<string, Pool>();
    public Dictionary<string, List<PathInfo>> pathInfo = new Dictionary<string, List<PathInfo>>();
    
    public void Init()
    {
        TextAsset txt = Resources.Load("ResourcesInfo") as TextAsset;
        string[] lines = txt.text.Split("\r\n".ToCharArray());

        string[] category = lines[0].Split(',');
        for (int i=0; i< category.Length; i++)
        {
            indexByName[category[i]] = i;
        }
        indexByName["position y"] = category.Length;
        indexByName["number in queue"] = category.Length + 1;
        indexByName["number in game"] = category.Length + 2;

        for (int i = 1; i < lines.Length; i++)
        {
            if(lines[i].Length > 0)
            {
                string[] values = lines[i].Split(',');
                string[] arr = new string[indexByName.Count];
                for (int j = 0; j < values.Length; j++)
                {
                    arr[j] = values[j];
                }
                //arr[indexByName["probability"]] = (float.Parse(arr[indexByName["probability"]])/100) + "";
                arr[indexByName["number in queue"]] = arr[indexByName["respawn number"]];
                arr[indexByName["number in game"]] = "0";
                arr[indexByName["name"]] = arr[indexByName["name"]].ToLower();

                //if (!arr[indexByName["theme"]].Equals(""))
                //{
                    
                //}
                GameObject obj = (GameObject)Resources.Load(values[indexByName["path"]]);
                Pool pool = new Pool(obj, arr);
                pools.Add(pool);
                poolProperties[pool.GetString("name")] = pool;
            }
        }

        ObjectPoolStart();

        var txt1 = Resources.LoadAll("PathInfo");
        foreach (TextAsset text in txt1)
        {
            string pathName = text.name;
            lines = text.text.Split("\r\n".ToCharArray());
            Dictionary<string, int[]> stat = new Dictionary<string, int[]>();
            Dictionary<string, int[]> dyna = new Dictionary<string, int[]>();
            for (int i = 2; i < lines.Length; i+=2)
            {
                if(lines[i].Length > 0)
                {
                    string[] values = lines[i].Split(',');

                    string[] amounts = values[1].Split(';');
                    int[] correctAmounts = new int[amounts.Length];
                    for (int j = 0; j < correctAmounts.Length; j++)
                    {
                        if (amounts[j].Equals(""))
                            correctAmounts[j] = 0;
                        else
                            correctAmounts[j] = int.Parse(amounts[j]);
                    }
                    if(!values[0].Equals(""))
                        stat[values[0].ToLower()] = correctAmounts;

                    amounts = values[3].Split(';');
                    correctAmounts = new int[amounts.Length];
                    for (int j = 0; j < correctAmounts.Length; j++)
                    {
                        if (amounts[j].Equals(""))
                            correctAmounts[j] = 0;
                        else
                            correctAmounts[j] = int.Parse(amounts[j]);
                    }
                    if (!values[2].Equals(""))
                        dyna[values[2].ToLower()] = correctAmounts;
                }
            }
            pathName = pathName.ToLower();
            string theme = pathName[0] + "";
            if (!pathInfo.ContainsKey(theme))
            {
                pathInfo[theme] = new List<PathInfo>();
            }
            pathInfo[theme].Add(new PathInfo(pathName, stat, dyna));
            //pathInfo[pathName] = new PathInfo(pathName, stat, dyna);
        }
        
    }
    public void ObjectPoolStart()
    {
        poolQueue = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.GetInt("respawn number"); i++)
            {

                GameObject obj = Instantiate(pool.prefab, gameObject.transform);
                pool.Properties[indexByName["position y"]] = obj.transform.position.y + "";
                obj.name = pool.GetString("name") + i.ToString("D3");
                obj.transform.position += new Vector3(0, 50, 0);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolQueue.Add(pool.GetString("name"), objectPool);
        }
    }
    
    public GameObject SpawnFromPool(string tag, Vector2 position)
    {
        tag = tag.ToLower();

        if (!poolQueue.ContainsKey(tag) || poolQueue[tag].Count == 0)
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }
        //Add Ins

        GameObject objectToSpawn = poolQueue[tag].Dequeue();

        Vector3 newPosition = new Vector3(position.x, objectToSpawn.transform.position.y - 50, position.y);
        objectToSpawn.transform.position = newPosition;
        //objectToSpawn.transform.rotation = Quaternion.Euler(0, 0, 0);
        poolProperties[tag].Properties[indexByName["number in game"]] = (int.Parse(poolProperties[tag].Properties[indexByName["number in game"]]) + 1) + "";
        objectToSpawn.SetActive(true);

        if(poolQueue[tag].Count == 0 /*&& IsStatic(tag)*/)
        {
            int addNumber = poolProperties[tag].GetInt("respawn number")/10;
            int numberInGame = poolProperties[tag].GetInt("number in queue");
            for (int i = 0; i < addNumber; i++)
            {
                GameObject obj = Instantiate(poolProperties[tag].prefab, gameObject.transform);
                poolProperties[tag].Properties[indexByName["position y"]] = obj.transform.position.y + "";
                obj.name = tag + (i + 1 + numberInGame).ToString("D3");
                obj.transform.position += new Vector3(0, 50, 0);
                obj.SetActive(false);
                poolQueue[tag].Enqueue(obj);
            }
            poolProperties[tag].Properties[indexByName["number in queue"]] = (numberInGame + addNumber) + "";
        }

        return objectToSpawn;
    }
    public GameObject SpawnFromPool(string tag, Vector2 position, Quaternion rotation)
    {
        tag = tag.ToLower();

        if (!poolQueue.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }
        //Add Ins

        GameObject objectToSpawn = poolQueue[tag].Dequeue();

        Vector3 newPosition = new Vector3(position.x, objectToSpawn.transform.position.y - 50, position.y);
        objectToSpawn.transform.position = newPosition;
        //if (tag.Equals("islandsmallicecollectiona"))
            //objectToSpawn.transform.Rotate();
        objectToSpawn.transform.eulerAngles = new Vector3(objectToSpawn.transform.eulerAngles.x, rotation.y, objectToSpawn.transform.eulerAngles.z);
        poolProperties[tag].Properties[indexByName["number in game"]] = (int.Parse(poolProperties[tag].Properties[indexByName["number in game"]]) + 1) + "";
        objectToSpawn.SetActive(true);

        if (poolQueue[tag].Count == 0 /*&& IsStatic(tag)*/)
        {
            int addNumber = poolProperties[tag].GetInt("respawn number") / 10;
            int numberInGame = poolProperties[tag].GetInt("number in queue");
            for (int i = 0; i < addNumber; i++)
            {
                GameObject obj = Instantiate(poolProperties[tag].prefab, gameObject.transform);
                poolProperties[tag].Properties[indexByName["position y"]] = obj.transform.position.y + "";
                obj.name = tag + (i + 1 + numberInGame).ToString("D3");
                obj.transform.position += new Vector3(0, 50, 0);
                obj.SetActive(false);
                poolQueue[tag].Enqueue(obj);
            }
            poolProperties[tag].Properties[indexByName["number in queue"]] = (numberInGame + addNumber) + "";
        }

        return objectToSpawn;
    }
    
    public void ReturnObjectToPool(GameObject obj)
    {
        Regex filter = new Regex(@"([a-zA-Z]+)");
        var match = filter.Match(obj.name).Value.ToLower();
        if (poolQueue.ContainsKey(match))
        {
            obj.SetActive(false);
            obj.transform.position = new Vector3(0, poolProperties[match].GetFloat("position y") + 50, 0);
            //Debug.Log(match + " ?");
            poolProperties[match].Properties[indexByName["number in game"]] = (int.Parse(poolProperties[match].Properties[indexByName["number in game"]]) - 1) + "";
            poolQueue[match].Enqueue(obj);
        }
        else
            Debug.Log("bug in ReturnObjectToPool");
    }

    public void CreateDrop(Drop drop, Vector3 position, int amount)
    {
        GameObject a = SpawnFromPool("Drop", position);
        //a.SetActive(false);
        a.transform.position = new Vector3(position.x, a.transform.position.y, position.z);
        a.GetComponentInChildren<DropScript>().drop = drop;
        a.GetComponentInChildren<DropScript>().amount = amount;
        a.GetComponentInChildren<DropScript>().Init();
    }
    public PathInfo GetRandomPath(string theme)
    {
        return pathInfo[theme][Random.Range(0, pathInfo[theme].Count)];
    }
    public GameObject CreatePath(Vector2 position)
    {
        return null;
    }
    //public float GetProbability(string name)
    //{
    //    name = name.ToLower();
    //    return poolProperties[name].GetFloat("probability");
    //}
    public int GetNumer(string name)
    {
        name = name.ToLower();
        return poolProperties[name].GetInt("respawn number");
    }
    public int GetHighetLevel(string name)
    {
        name = name.ToLower();
        return poolProperties[name].GetInt("highet level");
    }
    public bool IsStatic(string name)
    {
        name = name.ToLower();
        if (!poolProperties.ContainsKey(name))
            Debug.Log("g");
        return poolProperties[name].GetInt("is static") == 1;
    }
    public int GetCurrentNumerInPool(string name)
    {
        name = name.ToLower();
        return poolQueue[name].Count;
    }
    public int GetCountInGame(string tag)
    {
        return int.Parse(poolProperties[tag].Properties[indexByName["number in game"]]);
    }
    public PathInfo GetFirstUnitsDic(string icon)
    {
        foreach (var element in pathInfo[icon])
        {
            return element;
        }
        return new PathInfo("empty");
    }
    public PathInfo GetPathInfoDic(string icon, string name)
    {
        foreach (var element in pathInfo[icon])
        {
            if(element.name.Equals(name))
                return element;
        }
        return new PathInfo("empty");
    }

}
