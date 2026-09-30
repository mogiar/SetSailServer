using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static ObjectPooler;

public class PathManager : MonoBehaviour
{
    private List<GameObject> _paths = new List<GameObject>();

    //Starter plane (0 , 0)
    [SerializeField] private Transform _playerPos;
    [SerializeField] private GameObject activePath;
    public string[] mapArea;

    private float _activeMax_X;
    private float _activeMax_Y;
    private float _activeMin_X;
    private float _activeMin_Y;

    public Map map;

    void Start()
    {
        //_playerPos = GeneralFunctions.Player.transform;

        activePath.GetComponent<Path>().isActivePath = true;
        activePath.GetComponent<Path>().Init();
        _paths.Add(activePath);

        //GeneralFunctions.GameManager.SetUnitsToSpawn(GeneralFunctions.Pooler.GetFirstUnitsDic("s").staticUnits, true);
        //GeneralFunctions.GameManager.PlaceStaticRandomObject(activePath.transform.position);

        map = new Map();
        map.Add(activePath.name);
        Debug.Log(map.Print());
        mapArea = new string[] { "ice", "crystal", "stone", "random" };
    }

    private void Update()
    {
        //CheckActivePath();
    }

    //Checks if there is already a sea plane, if there is not one - build one.
    public bool CheckPlane(Vector2 target, bool toCreatePlane)
    {
        Vector3 target3 = new Vector3(target.x, 0f, target.y);

        foreach (GameObject path in _paths)
        {
            if (path.transform.position == target3)
            {
                return true;
            }
        }

        if (toCreatePlane)
        {
            CreatePlane(target);
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void CreatePlane(Vector2 location)
    {
        string nextTheme = map.GetIcon(location); // get the current theme of the location
        PathInfo pathInfo = new PathInfo("empty");
        if (char.IsLower(nextTheme[0]))
        {
            switch (nextTheme)
            {
                case "x": // if there is not theme, search theme in nearby location
                    pathInfo = GeneralFunctions.Pooler.GetRandomPath(map.GetNextIcon(location));
                    break;
                default: // from random path with the theme
                    pathInfo = GeneralFunctions.Pooler.GetRandomPath(nextTheme);
                    break;
            }
        }

        GameObject a = GeneralFunctions.Pooler.SpawnFromPool(pathInfo.name, location);
        a.gameObject.name += " ("+location.x +" , " + location.y + ")";

        string name = a.gameObject.name;
        name = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
        map.Add(name); // add the new path to the map
        Debug.Log(map.Print()); // ------- debug
        int rand = Random.Range(0, 3);
        int deg = 0;
        switch (rand)
        { // random rotation of the path
            case 1:
                deg = 90;
                break;
            case 2:
                deg = 180;
                break;
            case 3:
                deg = 270;
                break;
        }
        // set up of path
        a.transform.rotation = Quaternion.Euler(0, deg, 0);
        a.GetComponent<Path>().isActivePath = true;
        a.GetComponent<Path>().Init();
        _paths.Add(a);
        //GeneralFunctions.GameManager.SetUnitsToSpawn(pathInfo.dynamicUnits, false);
        GeneralFunctions.GameManager.SetUnitsToSpawn(pathInfo.staticUnits, true);
        GeneralFunctions.GameManager.PlaceStaticRandomObject(a.transform.position);
    }

    //Checks if active path needed to be changed
    private void CheckActivePath()
    {
        if (_playerPos.position.x > _activeMax_X 
            || _playerPos.position.x < _activeMin_X
            || _playerPos.position.z > _activeMax_Y
            || _playerPos.position.z < _activeMin_Y)
        {
            ActivePathPeak();
        }
    }

    //Activate path rays and return its transform
    private void ActivePathPeak()
    {
        Transform ans = activePath.transform;
        foreach (GameObject path in _paths)
        {
            if (CheckInRangeOf(_playerPos.position, path.transform.position, 500f))
            {
                path.GetComponent<Path>().isActivePath = true;
                ans = path.transform;
                // set dynamic units by active path
                string name = path.name;
                string[] arr = name.Split(" (".ToCharArray());
                Regex filter = new Regex(@"([a-zA-Z_]+)");
                var match = filter.Match(arr[0]).Value.ToLower();
                PathInfo pathInfo = GeneralFunctions.Pooler.GetPathInfoDic(match[0] + "", match);
                GeneralFunctions.GameManager.SetUnitsToSpawn(pathInfo.dynamicUnits, false);
            }
            else
            {
                path.GetComponent<Path>().isActivePath = false;
            }
        }

        //Get minimum and maximun for active path
        _activeMax_X = ans.position.x + 500f;
        _activeMax_Y = ans.position.z + 500f;
        _activeMin_X = ans.position.x - 500f;
        _activeMin_Y = ans.position.z - 500f;
    }
    
    private bool CheckInRangeOf(Vector3 position, Vector3 middlePosition, float range)
    {
        if (position.x <= (middlePosition.x + range) &&
            (position.x >= middlePosition.x - range) &&
            position.z <= (middlePosition.z + range) &&
            position.z >= (middlePosition.z - range))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public string GetNextArea()
    {
        Vector3 playerPosition = _playerPos.position;
        int countArea = mapArea.Length;

        return "";
    }
    
    private void createRandomStaticObjects(Transform path , float range, float minimumDistance, Transform objectToSpawn, int NumberOfObjects)
    {
        Vector2[] positions = new Vector2[NumberOfObjects];

        for (int i = 0; i < NumberOfObjects; i++)
        {
            positions[i] = new Vector2(Random.Range(path.position.x - range, path.position.x + range),
                                        Random.Range(path.position.z - range, path.position.z + range));

            Transform item = Instantiate(objectToSpawn, new Vector3 (positions[i].x, 0 ,positions[i].y), path.rotation);
            item.parent = path;
        }
    }
}
